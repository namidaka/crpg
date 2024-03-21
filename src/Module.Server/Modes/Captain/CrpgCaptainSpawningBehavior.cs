using Crpg.Module.Api.Models.Captains;
using Crpg.Module.Api.Models.Users;
using Crpg.Module.Common;
using Crpg.Module.Modes.Warmup;
using Crpg.Module.Notifications;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlayerServices;

namespace Crpg.Module.Modes.Captain;

internal class CrpgCaptainSpawningBehavior : CrpgSpawningBehaviorBase
{
    private const float TotalSpawnDuration = 30f;
    private readonly MultiplayerRoundController _roundController;
    private readonly HashSet<PlayerId> _notifiedPlayersAboutSpawnRestriction;
    private MissionTimer? _spawnTimer;
    private MissionTimer? _cavalrySpawnDelayTimer;
    private bool _botsSpawned = false;

    private readonly Dictionary<Team, int> _teamSumOfEquipment = new();
    private readonly Dictionary<Team, int> _teamAverageEquipment = new();
    private readonly Dictionary<Team, int> _teamNumberOfBots = new();

    private readonly int _totalNumberOfBots;
    private bool _isSinglePlayer;

    public CrpgCaptainSpawningBehavior(CrpgConstants constants, MultiplayerRoundController roundController, MultiplayerGameType gameType)
        : base(constants)
    {
        _roundController = roundController;
        _notifiedPlayersAboutSpawnRestriction = new HashSet<PlayerId>();
        GameMode = gameType;
#if CRPG_SERVER
        _totalNumberOfBots = CrpgServerConfiguration.CaptainTotalBotCount;
#endif
    }

    public override void Initialize(SpawnComponent spawnComponent)
    {
        base.Initialize(spawnComponent);
        _roundController.OnPreparationEnded += RequestStartSpawnSession;
        _roundController.OnRoundEnding += RequestStopSpawnSession;
    }

    public override void Clear()
    {
        base.Clear();
        _roundController.OnPreparationEnded -= RequestStartSpawnSession;
        _roundController.OnRoundEnding -= RequestStopSpawnSession;
    }

    public override void OnTick(float dt)
    {
        if (!IsSpawningEnabled || !IsRoundInProgress())
        {
            return;
        }

        if (_spawnTimer!.Check())
        {
            return;
        }

        if (!_botsSpawned)
        {
            SpawnCaptainBots();
            _botsSpawned = true;
        }

        SpawnAgents();
    }

    public override void RequestStartSpawnSession()
    {
        _isSinglePlayer = IsSinglePlayer();

        foreach (Team team in Mission.Current.Teams)
        {
            _teamSumOfEquipment[team] = ComputeTeamSumOfEquipmentValue(team);
            _teamAverageEquipment[team] = ComputeTeamAverageUnitValue(team, _teamSumOfEquipment[team]);
        }

        if (!_isSinglePlayer)
        {
            foreach (Team team in Mission.Current.Teams)
            {
                var peers = GameNetwork.NetworkPeers;
                var teamRelevantPeers =
                peers.Where(p => IsNetworkPeerRelevant(p) && p.GetComponent<MissionPeer>().Team == team).ToList();

                float numerator = _totalNumberOfBots * _teamAverageEquipment.Where(kvp => kvp.Key != team).Sum(kvp => kvp.Value);
                float denominator = (Mission.Current.Teams.Count - 2) * _teamAverageEquipment.Sum(kvp => kvp.Value); // -2 because we also remove spectator
                _teamNumberOfBots[team] = (int)(numerator / denominator) - teamRelevantPeers.Count;
            }
        }
        else
        {
            foreach (Team team in Mission.Current.Teams)
            {
                if (team.Side != BattleSideEnum.None)
                {
                    _teamNumberOfBots[team] = _totalNumberOfBots / (Mission.Current.Teams.Count - 1);
                }
            }
        }

        base.RequestStartSpawnSession();
        _botsSpawned = false;
        _spawnTimer = new MissionTimer(TotalSpawnDuration); // Limit spawning for 30 seconds.
        _cavalrySpawnDelayTimer = new MissionTimer(GetCavalrySpawnDelay()); // Cav will spawn X seconds later.
        _notifiedPlayersAboutSpawnRestriction.Clear();
    }

    public bool SpawnDelayEnded()
    {
        return _cavalrySpawnDelayTimer != null && _cavalrySpawnDelayTimer!.Check();
    }

    protected override bool IsRoundInProgress()
    {
        return _roundController.IsRoundInProgress;
    }

    protected override bool IsPlayerAllowedToSpawn(NetworkCommunicator networkPeer)
    {
        var crpgPeer = networkPeer.GetComponent<CrpgPeer>();
        var missionPeer = networkPeer.GetComponent<MissionPeer>();
        if (crpgPeer?.User == null
            || crpgPeer.LastSpawnInfo != null
            || missionPeer == null)
        {
            return false;
        }

        var characterEquipment = CrpgCharacterBuilder.CreateCharacterEquipment(crpgPeer.User.Character.EquippedItems);
        if (!DoesEquipmentContainWeapon(characterEquipment)) // Disallow spawning without weapons.
        {
            if (_notifiedPlayersAboutSpawnRestriction.Add(networkPeer.VirtualPlayer.Id))
            {
                GameNetwork.BeginModuleEventAsServer(networkPeer);
                GameNetwork.WriteMessage(new CrpgNotificationId
                {
                    Type = CrpgNotificationType.Announcement,
                    TextId = "str_kick_reason",
                    TextVariation = "no_weapon",
                    SoundEvent = string.Empty,
                });
                GameNetwork.EndModuleEventAsServer();
            }

            return false;
        }

        bool hasMount = characterEquipment[EquipmentIndex.Horse].Item != null;
        // Disallow spawning cavalry before the cav spawn delay ended.
        if (hasMount && _cavalrySpawnDelayTimer != null && !_cavalrySpawnDelayTimer.Check())
        {
            if (_notifiedPlayersAboutSpawnRestriction.Add(networkPeer.VirtualPlayer.Id))
            {
                GameNetwork.BeginModuleEventAsServer(networkPeer);
                GameNetwork.WriteMessage(new CrpgNotificationId
                {
                    Type = CrpgNotificationType.Notification,
                    TextId = "str_notification",
                    TextVariation = "cavalry_spawn_delay",
                    SoundEvent = string.Empty,
                    Variables = { ["SECONDS"] = ((int)_cavalrySpawnDelayTimer.GetTimerDuration()).ToString() },
                });
                GameNetwork.EndModuleEventAsServer();
            }

            return false;
        }

        return true;
    }

    protected override void OnPeerSpawned(Agent agent)
    {
        MissionPeer? missionPeer = agent.MissionPeer;

        if (missionPeer == null)
        {
            return;
        }

        CrpgPeer? crpgPeer = missionPeer.Peer.GetComponent<CrpgPeer>();

        if (crpgPeer == null)
        {
            return;
        }

        if (agent.MissionPeer.ControlledFormation != null)
        {
            agent.Team.AssignPlayerAsSergeantOfFormation(agent.MissionPeer, agent.MissionPeer.ControlledFormation.FormationIndex);
        }

        base.OnPeerSpawned(agent);
        agent.MissionPeer.SpawnCountThisRound += 1;

        if (IsRoundInProgress())
        {
            int p = int.Parse(agent.Character.StringId.Split('_').Last());
            var peers = GameNetwork.NetworkPeers;
            var teamRelevantPeers =
                peers.Where(p => IsNetworkPeerRelevant(p) && p.GetComponent<MissionPeer>().Team == missionPeer.Team).ToList();
            float sumOfTeamEquipment = _teamSumOfEquipment[missionPeer.Team];
            float peerSumOfEquipment = ComputeEquipmentValue(crpgPeer);
            int peerNumberOfBots = 0;
            if (teamRelevantPeers.Count - 1 < 1)
            {
                peerNumberOfBots = _teamNumberOfBots[missionPeer.Team] - 1;
            }
            else
            {
                peerNumberOfBots = (int)(_teamNumberOfBots[missionPeer.Team] * (1 - peerSumOfEquipment / sumOfTeamEquipment) /
                         (float)(teamRelevantPeers.Count - 1));
            }

            Dictionary<int, double> formationBotWeight = new();
            var captainFormations = crpgPeer.User!.Captain.Formations.Where(f => f.Character != null);
            if (captainFormations.Any())
            {
                double totalWeight = captainFormations.Sum(cf => cf.Weight);

                foreach (CrpgCaptainFormation captainFormation in captainFormations)
                {
                    double proportion = (double)(captainFormation.Weight / totalWeight);
                    formationBotWeight.Add(captainFormation.Number, proportion);
                }
            }
            else
            {
                for (int i = 0; i < peerNumberOfBots; i++)
                {
                    SpawnBotAgent($"crpg_class_division_{p}", agent.Team, missionPeer, p);
                }
            }

            foreach (KeyValuePair<int, double> captainFormation in formationBotWeight)
            {
                for (int i = 0; i < (int)(captainFormation.Value * peerNumberOfBots); i++)
                {
                    SpawnBotAgent($"crpg_class_division_{p}", agent.Team, missionPeer, p, captainFormation.Key);
                }
            }
        }

    }

    /// <summary>
    /// Cav spawn delay values
    /// 10 => 7sec
    /// 30 => 9sec
    /// 60 => 13sec
    /// 90 => 17sec
    /// 120 => 22sec
    /// 150 => 26sec
    /// 165+ => 28sec.
    /// </summary>
    private int GetCavalrySpawnDelay()
    {
        int currentPlayers = Math.Max(GetCurrentPlayerCount(), 1);
        return Math.Min(28, 5 + currentPlayers / 7);
    }

    private int GetCurrentPlayerCount()
    {
        int counter = 0;
        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        {
            var missionPeer = networkPeer.GetComponent<MissionPeer>();
            if (!networkPeer.IsSynchronized
                || missionPeer == null
                || missionPeer.Team == null
                || missionPeer.Team.Side == BattleSideEnum.None)
            {
                continue;
            }

            counter++;
        }

        return counter;
    }

    private int ComputeTeamSumOfEquipmentValue(Team team)
    {
        var peers = GameNetwork.NetworkPeers;
        var teamRelevantPeers =
            peers.Where(p => IsNetworkPeerRelevant(p) && p.GetComponent<MissionPeer>().Team == team).ToList();
        int valueToReturn = teamRelevantPeers.Sum(p => ComputeEquipmentValue(p.GetComponent<CrpgPeer>()));
        return (int)Math.Max(valueToReturn, 1);
    }

    private int ComputeTeamAverageUnitValue(Team team, int teamSumOfEquipment)
    {
        var peers = GameNetwork.NetworkPeers;
        var teamRelevantPeers =
    peers.Where(p => IsNetworkPeerRelevant(p) && p.GetComponent<MissionPeer>().Team == team).ToList();
        if (teamRelevantPeers.Count < 2)
        {
            return teamRelevantPeers.Sum(p => ComputeEquipmentValue(p.GetComponent<CrpgPeer>()));
        }

        double sumOfEachSquared = teamRelevantPeers.Sum(p => Math.Pow(ComputeEquipmentValue(p.GetComponent<CrpgPeer>()), 2f));
        int valueToReturn = (int)((teamSumOfEquipment - sumOfEachSquared / teamSumOfEquipment) / (float)(teamRelevantPeers.Count - 1));
        return (int)Math.Max(valueToReturn, 1);
    }

    private int ComputeEquipmentValue(CrpgPeer peer)
    {
        int totalValue = 0;

        Dictionary<int, double> formationBotWeight = new();
        var captainFormations = peer?.User?.Captain.Formations.Where(f => f.Character != null);
        if (captainFormations.Any())
        {
            double totalWeight = captainFormations.Sum(cf => cf.Weight);

            foreach (CrpgCaptainFormation captainFormation in captainFormations!)
            {
                double proportion = (double)(captainFormation.Weight / totalWeight);
                formationBotWeight.Add(captainFormation.Number, proportion);

                int characterValue = captainFormation.Character!.EquippedItems
                .Select(i => MBObjectManager.Instance.GetObject<ItemObject>(i.UserItem.ItemId))
                .Where(io => io != null)
                .Sum(io => io.Value);

                totalValue += (int)(characterValue * formationBotWeight[captainFormation.Number]);
            }
        }
        else
        {
            totalValue = peer?.User?.Character.EquippedItems.Select(i => MBObjectManager.Instance.GetObject<ItemObject>(i.UserItem.ItemId)).Sum(io => io.Value) ?? 0;
        }

        return totalValue + 10000; // protection against naked
    }

    private int ComputeSingleEquipmentValue(ItemObject item)
    {
        return 0;
    }

    private bool IsNetworkPeerRelevant(NetworkCommunicator networkPeer)
    {
        MissionPeer missionPeer = networkPeer.GetComponent<MissionPeer>();
        CrpgPeer crpgPeer = networkPeer.GetComponent<CrpgPeer>();
        bool isRelevant = !(!networkPeer.IsSynchronized
                            || missionPeer == null
                            || missionPeer.Team == null
                            || missionPeer.Team == Mission.SpectatorTeam
                            || crpgPeer == null
                            || crpgPeer.UserLoading
                            || crpgPeer.User == null);
        return isRelevant;
    }

    private bool IsSinglePlayer()
    {
        bool isATeamEmpty = false;

        foreach (Team team in Mission.Current.Teams)
        {
            if (team.Side != BattleSideEnum.None)
            {
                var peers = GameNetwork.NetworkPeers;
                var teamRelevantPeers =
                    peers.Where(p => IsNetworkPeerRelevant(p) && p.GetComponent<MissionPeer>().Team == team).ToList();

                if (teamRelevantPeers.Count < 1)
                {
                    isATeamEmpty = true;
                }
            }
        }

        return isATeamEmpty;
    }

    private void SpawnCaptainBots()
    {
        if (IsRoundInProgress())
        {
            foreach (Team team in Mission.Current.Teams)
            {
                if (team.Side != BattleSideEnum.None)
                {
                    var peers = GameNetwork.NetworkPeers;
                    var teamRelevantPeers =
                    peers.Where(p => IsNetworkPeerRelevant(p) && p.GetComponent<MissionPeer>().Team == team).ToList();

                    if (teamRelevantPeers.Count == 0)
                    {
                        for (int i = 0; i < _teamNumberOfBots[team]; i++)
                        {
                            MultiplayerClassDivisions.MPHeroClass botClass = MultiplayerClassDivisions
                            .GetMPHeroClasses()
                            .GetRandomElementWithPredicate<MultiplayerClassDivisions.MPHeroClass>(x => x.StringId.StartsWith("crpg_bot_"));
                            SpawnBotAgent(botClass.StringId, team);
                        }
                    }
                }
            }
        }
    }
}
