using Crpg.Module.Common;
using Crpg.Module.Rewards;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace Crpg.Module.Modes.TrainingGround;

internal class CrpgTrainingGroundServer : MissionMultiplayerGameModeBase
{
    private class DuelInfo
    {
        private enum ChallengerType
        {
            None = -1,
            Requester,
            Requestee,
            NumChallengerType,
        }

        private struct Challenger
        {
            public readonly MissionPeer MissionPeer = default!;
            public readonly NetworkCommunicator? NetworkPeer;
            public Agent? DuelingAgent { get; private set; }
            public Agent? MountAgent { get; private set; }
            public int KillCountInDuel { get; private set; }
            public Challenger(MissionPeer missionPeer)
            {
                MissionPeer = missionPeer;
                NetworkPeer = MissionPeer?.GetNetworkPeer();
                DuelingAgent = null;
                MountAgent = null;
                KillCountInDuel = 0;
            }

            public void OnDuelPreparation(Team duelingTeam)
            {
                MissionPeer.ControlledAgent?.FadeOut(hideInstantly: true, hideMount: true);
                MissionPeer.Team = duelingTeam;
                MissionPeer.HasSpawnedAgentVisuals = true;
            }

            public void OnDuelEnded()
            {
                if (MissionPeer.Peer.Communicator.IsConnectionActive)
                {
                    MissionPeer.Team = Mission.Current.AttackerTeam;
                }
            }

            public void IncreaseWinCount()
            {
                KillCountInDuel++;
            }

            public void SetAgents(Agent agent)
            {
                DuelingAgent = agent;
                MountAgent = DuelingAgent.MountAgent;
            }
        }

        private const float DuelStartCountdown = 3f;
        private readonly Challenger[] _challengers;
        private ChallengerType _winnerChallengerType = ChallengerType.None;
        public MissionPeer RequesterPeer => _challengers[0].MissionPeer;
        public MissionPeer RequesteePeer => _challengers[1].MissionPeer;
        public int DuelAreaIndex { get; private set; }
        public TroopType DuelAreaTroopType { get; private set; }
        public MissionTime Timer { get; private set; }
        public Team DuelingTeam { get; private set; } = default!;
        public bool Started { get; private set; }
        public bool ChallengeEnded { get; private set; }
        public MissionPeer? ChallengeWinnerPeer
        {
            get
            {
                if (_winnerChallengerType != ChallengerType.None)
                {
                    return _challengers[(int)_winnerChallengerType].MissionPeer;
                }

                return null;
            }
        }

        public MissionPeer? ChallengeLoserPeer
        {
            get
            {
                if (_winnerChallengerType != ChallengerType.None)
                {
                    return _challengers[(_winnerChallengerType == ChallengerType.Requester) ? 1 : 0].MissionPeer;
                }

                return null;
            }
        }

        public DuelInfo(MissionPeer requesterPeer, MissionPeer requesteePeer, KeyValuePair<int, TroopType> duelAreaPair)
        {
            DuelAreaIndex = duelAreaPair.Key;
            DuelAreaTroopType = duelAreaPair.Value;
            _challengers = new Challenger[2];
            _challengers[0] = new Challenger(requesterPeer);
            _challengers[1] = new Challenger(requesteePeer);
            Timer = MissionTime.Now + MissionTime.Seconds(10.5f);
        }

        private void DecideRoundWinner()
        {
            bool isConnectionActive = _challengers[0].MissionPeer.Peer.Communicator.IsConnectionActive;
            bool isConnectionActive2 = _challengers[1].MissionPeer.Peer.Communicator.IsConnectionActive;
            if (!Started)
            {
                if (isConnectionActive == isConnectionActive2)
                {
                    ChallengeEnded = true;
                }
                else
                {
                    _winnerChallengerType = (!isConnectionActive) ? ChallengerType.Requestee : ChallengerType.Requester;
                }
            }
            else
            {
                Agent? duelingAgent = _challengers[0].DuelingAgent;
                Agent? duelingAgent2 = _challengers[1].DuelingAgent;
                if (duelingAgent != null && duelingAgent.IsActive())
                {
                    _winnerChallengerType = ChallengerType.Requester;
                }
                else if (duelingAgent2 != null && duelingAgent2.IsActive())
                {
                    _winnerChallengerType = ChallengerType.Requestee;
                }
                else
                {
                    if (!isConnectionActive && !isConnectionActive2)
                    {
                        ChallengeEnded = true;
                    }

                    _winnerChallengerType = ChallengerType.None;
                }
            }

            if (_winnerChallengerType != ChallengerType.None)
            {
                _challengers[(int)_winnerChallengerType].IncreaseWinCount();
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new DuelRoundEnded(_challengers[(int)_winnerChallengerType].NetworkPeer));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
                if (_challengers[(int)_winnerChallengerType].KillCountInDuel == MultiplayerOptions.OptionType.MinScoreToWinDuel.GetIntValue() || !isConnectionActive || !isConnectionActive2)
                {
                    ChallengeEnded = true;
                }
            }
        }

        public void OnDuelPreparation(Team duelTeam)
        {
            if (!Started)
            {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new DuelPreparationStartedForTheFirstTime(_challengers[0].MissionPeer.GetNetworkPeer(), _challengers[1].MissionPeer.GetNetworkPeer(), DuelAreaIndex));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
            }

            Started = false;
            DuelingTeam = duelTeam;
            _winnerChallengerType = ChallengerType.None;
            for (int i = 0; i < 2; i++)
            {
                _challengers[i].OnDuelPreparation(DuelingTeam);
                _challengers[i].MissionPeer.GetComponent<CrpgTrainingGroundMissionRepresentative>().OnDuelPreparation(_challengers[0].MissionPeer, _challengers[1].MissionPeer);
            }

            Timer = MissionTime.Now + MissionTime.Seconds(3f);
        }

        public void OnDuelStarted()
        {
            Started = true;
            DuelingTeam.SetIsEnemyOf(DuelingTeam, isEnemyOf: true);
        }

        public void OnDuelEnding()
        {
            Timer = MissionTime.Now + MissionTime.Seconds(2f);
        }

        public void OnDuelEnded()
        {
            if (Started)
            {
                DuelingTeam.SetIsEnemyOf(DuelingTeam, isEnemyOf: false);
            }

            DecideRoundWinner();
            for (int i = 0; i < 2; i++)
            {
                _challengers[i].OnDuelEnded();
                Agent agent = _challengers[i].DuelingAgent ?? _challengers[i].MissionPeer.ControlledAgent;
                if (ChallengeEnded && agent != null && agent.IsActive())
                {
                    agent.FadeOut(hideInstantly: true, hideMount: false);
                }

                _challengers[i].MissionPeer.HasSpawnedAgentVisuals = true;
            }

            for (int j = 0; j < 2; j++)
            {
                if (_challengers[j].MountAgent != null && _challengers[j].MountAgent!.IsActive() && (ChallengeEnded || _challengers[j].MountAgent!.RiderAgent == null))
                {
                    _challengers[j].MountAgent!.FadeOut(hideInstantly: true, hideMount: false);
                }
            }
        }

        public void OnAgentBuild(Agent agent)
        {
            for (int i = 0; i < 2; i++)
            {
                if (_challengers[i].MissionPeer == agent.MissionPeer)
                {
                    _challengers[i].SetAgents(agent);
                    break;
                }
            }
        }

        public bool IsDuelStillValid(bool doNotCheckAgent = false)
        {
            for (int i = 0; i < 2; i++)
            {
                if (!_challengers[i].MissionPeer.Peer.Communicator.IsConnectionActive || (!doNotCheckAgent && !_challengers[i].MissionPeer.IsControlledAgentActive))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsPeerInThisDuel(MissionPeer peer)
        {
            for (int i = 0; i < 2; i++)
            {
                if (_challengers[i].MissionPeer == peer)
                {
                    return true;
                }
            }

            return false;
        }

        public void UpdateDuelAreaIndex(KeyValuePair<int, TroopType> duelAreaPair)
        {
            DuelAreaIndex = duelAreaPair.Key;
            DuelAreaTroopType = duelAreaPair.Value;
        }
    }

    public delegate void OnDuelEndedDelegate(MissionPeer winnerPeer, TroopType troopType);

    public const float DuelRequestTimeOutInSeconds = 10f;
    public const int NumberOfDuelAreas = 16;
    public const float DuelEndInSeconds = 2f;
    private const int MinBountyGain = 100;
    private const string AreaBoxTagPrefix = "area_box";
    private const string AreaFlagTagPrefix = "area_flag";
    private const float DuelRequestTimeOutServerToleranceInSeconds = 0.5f;
    private const float CorpseFadeOutTimeInSeconds = 1f;

    private readonly CrpgRewardServer _rewardServer;
    private readonly Queue<Team> _deactiveDuelTeams = new();

    private MissionTimer? _rewardTickTimer;
    private List<GameEntity> _duelAreaFlags = new();
    private List<VolumeBox> _areaBoxes = new();
    private List<DuelInfo> _duelRequests = new();
    private List<DuelInfo> _activeDuels = new();
    private List<DuelInfo> _endingDuels = new();
    private List<DuelInfo> _restartingDuels = new();
    private List<DuelInfo> _restartPreparationDuels = new();
    private List<KeyValuePair<MissionPeer, TroopType>> _peersAndSelections = new();
    private VolumeBox[] _cachedSelectedVolumeBoxes = default!;
    private KeyValuePair<int, TroopType>[] _cachedSelectedAreaFlags = default!;
    public override bool IsGameModeHidingAllAgentVisuals => true;
    public override bool IsGameModeUsingOpposingTeams => false;
    public event OnDuelEndedDelegate OnDuelEnded = default!;

    public CrpgTrainingGroundServer(CrpgRewardServer rewardServer)
    {
        _rewardServer = rewardServer;
    }

    public override MultiplayerGameType GetMissionType()
    {
        return MultiplayerGameType.Duel;
    }

    public override void AfterStart()
    {
        base.AfterStart();
        Mission.Current.SetMissionCorpseFadeOutTimeInSeconds(1f);
        BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
        Banner banner = new(@object.BannerKey, @object.BackgroundColor1, @object.ForegroundColor1);
        Mission.Teams.Add(BattleSideEnum.Attacker, @object.BackgroundColor1, @object.ForegroundColor1, banner, isPlayerGeneral: false);
        _rewardTickTimer = new(60);
    }

    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();
        _duelAreaFlags.AddRange(Mission.Current.Scene.FindEntitiesWithTagExpression("area_flag(_\\d+)*"));
        List<GameEntity> list = new();
        list.AddRange(Mission.Current.Scene.FindEntitiesWithTagExpression("area_box(_\\d+)*"));
        _cachedSelectedAreaFlags = new KeyValuePair<int, TroopType>[_duelAreaFlags.Count];
        for (int i = 0; i < list.Count; i++)
        {
            VolumeBox firstScriptOfType = list[i].GetFirstScriptOfType<VolumeBox>();
            _areaBoxes.Add(firstScriptOfType);
        }

        _cachedSelectedVolumeBoxes = new VolumeBox[_areaBoxes.Count];
    }

    protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
        registerer.RegisterBaseHandler<NetworkMessages.FromClient.DuelRequest>(HandleClientEventDuelRequest);
        registerer.RegisterBaseHandler<DuelResponse>(HandleClientEventDuelRequestAccepted);
        registerer.RegisterBaseHandler<RequestChangePreferredTroopType>(HandleClientEventDuelRequestChangePreferredTroopType);
    }

    protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
        networkPeer.AddComponent<CrpgTrainingGroundMissionRepresentative>();
    }

    protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        component.Team = Mission.AttackerTeam;
        _peersAndSelections.Add(new KeyValuePair<MissionPeer, TroopType>(component, TroopType.Invalid));
    }

    private bool HandleClientEventDuelRequest(NetworkCommunicator peer, GameNetworkMessage baseMessage)
    {
        NetworkMessages.FromClient.DuelRequest duelRequest = (NetworkMessages.FromClient.DuelRequest)baseMessage;
        MissionPeer? missionPeer = peer?.GetComponent<MissionPeer>();
        if (missionPeer != null)
        {
            Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(duelRequest.RequestedAgentIndex);
            if (agentFromIndex != null && agentFromIndex.IsActive())
            {
                DuelRequestReceived(missionPeer, agentFromIndex.MissionPeer);
            }
        }

        return true;
    }

    private bool HandleClientEventDuelRequestAccepted(NetworkCommunicator peer, GameNetworkMessage baseMessage)
    {
        DuelResponse duelResponse = (DuelResponse)baseMessage;
        if (peer?.GetComponent<MissionPeer>() != null && peer.GetComponent<MissionPeer>().ControlledAgent != null && duelResponse.Peer?.GetComponent<MissionPeer>() != null && duelResponse.Peer.GetComponent<MissionPeer>().ControlledAgent != null)
        {
            DuelRequestAccepted(duelResponse.Peer.GetComponent<CrpgTrainingGroundMissionRepresentative>().ControlledAgent, peer.GetComponent<CrpgTrainingGroundMissionRepresentative>().ControlledAgent);
        }

        return true;
    }

    private bool HandleClientEventDuelRequestChangePreferredTroopType(NetworkCommunicator peer, GameNetworkMessage baseMessage)
    {
        RequestChangePreferredTroopType requestChangePreferredTroopType = (RequestChangePreferredTroopType)baseMessage;
        OnPeerSelectedPreferredTroopType(peer.GetComponent<MissionPeer>(), requestChangePreferredTroopType.TroopType);
        return true;
    }

    public override bool CheckIfPlayerCanDespawn(MissionPeer missionPeer)
    {
        for (int i = 0; i < _activeDuels.Count; i++)
        {
            if (_activeDuels[i].IsPeerInThisDuel(missionPeer))
            {
                return false;
            }
        }

        return true;
    }

    public void OnPlayerDespawn(MissionPeer missionPeer)
    {
        missionPeer.GetComponent<CrpgTrainingGroundMissionRepresentative>();
    }

    public void DuelRequestReceived(MissionPeer requesterPeer, MissionPeer requesteePeer)
    {
        if (!IsThereARequestBetweenPeers(requesterPeer, requesteePeer) && !IsHavingDuel(requesterPeer) && !IsHavingDuel(requesteePeer))
        {
            DuelInfo duelInfo = new(requesterPeer, requesteePeer, GetNextAvailableDuelAreaIndex(requesterPeer.ControlledAgent));
            _duelRequests.Add(duelInfo);
            ((CrpgTrainingGroundMissionRepresentative)requesteePeer.Representative).DuelRequested(requesterPeer.ControlledAgent, duelInfo.DuelAreaTroopType);
        }
    }

    private KeyValuePair<int, TroopType> GetNextAvailableDuelAreaIndex(Agent requesterAgent)
    {
        TroopType troopType = TroopType.Invalid;
        for (int i = 0; i < _peersAndSelections.Count; i++)
        {
            if (_peersAndSelections[i].Key == requesterAgent.MissionPeer)
            {
                troopType = _peersAndSelections[i].Value;
                break;
            }
        }

        if (troopType == TroopType.Invalid)
        {
            troopType = GetAgentTroopType(requesterAgent);
        }

        bool flag = false;
        int num = 0;
        for (int j = 0; j < _duelAreaFlags.Count; j++)
        {
            GameEntity gameEntity = _duelAreaFlags[j];
            int num2 = int.Parse(gameEntity.Tags.Single((string ft) => ft.StartsWith("area_flag_")).Replace("area_flag_", string.Empty));
            int flagIndex = num2 - 1;
            if (_activeDuels.All((DuelInfo ad) => ad.DuelAreaIndex != flagIndex) && _restartingDuels.All((DuelInfo ad) => ad.DuelAreaIndex != flagIndex) && _restartPreparationDuels.All((DuelInfo ad) => ad.DuelAreaIndex != flagIndex))
            {
                TroopType troopType2 = (!gameEntity.HasTag("flag_infantry")) ? (gameEntity.HasTag("flag_archery") ? TroopType.Ranged : TroopType.Cavalry) : TroopType.Infantry;
                if (!flag && troopType2 == troopType)
                {
                    flag = true;
                    num = 0;
                }

                if (!flag || troopType2 == troopType)
                {
                    _cachedSelectedAreaFlags[num] = new KeyValuePair<int, TroopType>(flagIndex, troopType2);
                    num++;
                }
            }
        }

        return _cachedSelectedAreaFlags[MBRandom.RandomInt(num)];
    }

    public void DuelRequestAccepted(Agent requesterAgent, Agent requesteeAgent)
    {
        DuelInfo duelInfo = _duelRequests.FirstOrDefault((DuelInfo dr) => dr.IsPeerInThisDuel(requesterAgent.MissionPeer) && dr.IsPeerInThisDuel(requesteeAgent.MissionPeer));
        if (duelInfo != null)
        {
            PrepareDuel(duelInfo);
        }
    }

    public override void OnMissionTick(float dt)
    {
        RewardUsers();
        base.OnMissionTick(dt);
        CheckRestartPreparationDuels();
        CheckForRestartingDuels();
        CheckDuelsToStart();
        CheckDuelRequestTimeouts();
        CheckEndedDuels();
    }

    public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
    {
        if (!affectedAgent.IsHuman)
        {
            return;
        }

        if (affectedAgent.MissionPeer.Team.IsDefender)
        {
            DuelInfo? duelInfo = null;
            for (int i = 0; i < _activeDuels.Count; i++)
            {
                if (_activeDuels[i].IsPeerInThisDuel(affectedAgent.MissionPeer))
                {
                    duelInfo = _activeDuels[i];
                }
            }

            if (duelInfo != null && !_endingDuels.Contains(duelInfo))
            {
                duelInfo.OnDuelEnding();
                _endingDuels.Add(duelInfo);
            }

            return;
        }

        for (int num = _duelRequests.Count - 1; num >= 0; num--)
        {
            if (_duelRequests[num].IsPeerInThisDuel(affectedAgent.MissionPeer))
            {
                _duelRequests.RemoveAt(num);
            }
        }
    }

    private Team ActivateAndGetDuelTeam()
    {
        if (_deactiveDuelTeams.Count <= 0)
        {
            return Mission.Teams.Add(BattleSideEnum.Defender, uint.MaxValue, uint.MaxValue, null, isPlayerGeneral: true, isPlayerSergeant: false, isSettingRelations: false);
        }

        return _deactiveDuelTeams.Dequeue();
    }

    private void DeactivateDuelTeam(Team team)
    {
        _deactiveDuelTeams.Enqueue(team);
    }

    private bool IsHavingDuel(MissionPeer peer)
    {
        return _activeDuels.AnyQ((DuelInfo d) => d.IsPeerInThisDuel(peer));
    }

    private bool IsThereARequestBetweenPeers(MissionPeer requesterAgent, MissionPeer requesteeAgent)
    {
        for (int i = 0; i < _duelRequests.Count; i++)
        {
            if (_duelRequests[i].IsPeerInThisDuel(requesterAgent) && _duelRequests[i].IsPeerInThisDuel(requesteeAgent))
            {
                return true;
            }
        }

        return false;
    }

    private void CheckDuelsToStart()
    {
        for (int num = _activeDuels.Count - 1; num >= 0; num--)
        {
            DuelInfo duelInfo = _activeDuels[num];
            if (!duelInfo.Started && duelInfo.Timer.IsPast && duelInfo.IsDuelStillValid())
            {
                StartDuel(duelInfo);
            }
        }
    }

    private void CheckDuelRequestTimeouts()
    {
        for (int num = _duelRequests.Count - 1; num >= 0; num--)
        {
            DuelInfo duelInfo = _duelRequests[num];
            if (duelInfo.Timer.IsPast)
            {
                _duelRequests.Remove(duelInfo);
            }
        }
    }

    private void CheckForRestartingDuels()
    {
        for (int num = _restartingDuels.Count - 1; num >= 0; num--)
        {
            if (!_restartingDuels[num].IsDuelStillValid(doNotCheckAgent: true))
            {
                Debug.Print("!_restartingDuels[i].IsDuelStillValid(true)");
            }

            _duelRequests.Add(_restartingDuels[num]);
            PrepareDuel(_restartingDuels[num]);
            _restartingDuels.RemoveAt(num);
        }
    }

    private void CheckEndedDuels()
    {
        for (int num = _endingDuels.Count - 1; num >= 0; num--)
        {
            DuelInfo duelInfo = _endingDuels[num];
            if (duelInfo.Timer.IsPast)
            {
                EndDuel(duelInfo);
                _endingDuels.RemoveAt(num);
                if (!duelInfo.ChallengeEnded)
                {
                    _restartPreparationDuels.Add(duelInfo);
                }
            }
        }
    }

    private void CheckRestartPreparationDuels()
    {
        for (int num = _restartPreparationDuels.Count - 1; num >= 0; num--)
        {
            DuelInfo duelInfo = _restartPreparationDuels[num];
            Agent controlledAgent = duelInfo.RequesterPeer.ControlledAgent;
            Agent controlledAgent2 = duelInfo.RequesteePeer.ControlledAgent;
            if ((controlledAgent == null || controlledAgent.IsActive()) && (controlledAgent2 == null || controlledAgent2.IsActive()))
            {
                _restartPreparationDuels.RemoveAt(num);
                _restartingDuels.Add(duelInfo);
            }
        }
    }

    private void PrepareDuel(DuelInfo duel)
    {
        _duelRequests.Remove(duel);
        if (!IsHavingDuel(duel.RequesteePeer) && !IsHavingDuel(duel.RequesterPeer))
        {
            _activeDuels.Add(duel);
            Team duelTeam = duel.Started ? duel.DuelingTeam : ActivateAndGetDuelTeam();
            duel.OnDuelPreparation(duelTeam);
            for (int i = 0; i < _duelRequests.Count; i++)
            {
                if (_duelRequests[i].DuelAreaIndex == duel.DuelAreaIndex)
                {
                    _duelRequests[i].UpdateDuelAreaIndex(GetNextAvailableDuelAreaIndex(_duelRequests[i].RequesterPeer.ControlledAgent));
                }
            }
        }
        else
        {
            Debug.FailedAssert("IsHavingDuel(duel.RequesteePeer) || IsHavingDuel(duel.RequesterPeer)", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MultiplayerGameModeLogics\\ServerGameModeLogics\\MissionMultiplayerDuel.cs", "PrepareDuel", 707);
        }
    }

    private void StartDuel(DuelInfo duel)
    {
        duel.OnDuelStarted();
    }

    private void EndDuel(DuelInfo duel)
    {
        _activeDuels.Remove(duel);
        duel.OnDuelEnded();
        CleanSpawnedEntitiesInDuelArea(duel.DuelAreaIndex);
        if (duel.ChallengeEnded)
        {
            TroopType troopType = TroopType.Invalid;
            MissionPeer? challengeWinnerPeer = duel.ChallengeWinnerPeer;
            if (challengeWinnerPeer?.ControlledAgent != null)
            {
                troopType = GetAgentTroopType(challengeWinnerPeer.ControlledAgent);
                OnDuelEnded?.Invoke(challengeWinnerPeer, troopType);
            }

            DeactivateDuelTeam(duel.DuelingTeam);
            HandleEndedChallenge(duel);
        }
    }

    private TroopType GetAgentTroopType(Agent requesterAgent)
    {
        TroopType result = TroopType.Invalid;
        switch (requesterAgent.Character.DefaultFormationClass)
        {
            case FormationClass.Infantry:
            case FormationClass.HeavyInfantry:
                result = TroopType.Infantry;
                break;
            case FormationClass.Ranged:
                result = TroopType.Ranged;
                break;
            case FormationClass.Cavalry:
            case FormationClass.HorseArcher:
            case FormationClass.LightCavalry:
            case FormationClass.HeavyCavalry:
                result = TroopType.Cavalry;
                break;
        }

        return result;
    }

    private void CleanSpawnedEntitiesInDuelArea(int duelAreaIndex)
    {
        int num = duelAreaIndex + 1;
        int num2 = 0;
        for (int i = 0; i < _areaBoxes.Count; i++)
        {
            if (_areaBoxes[i].GameEntity.HasTag(string.Format("{0}_{1}", "area_box", num)))
            {
                _cachedSelectedVolumeBoxes[num2] = _areaBoxes[i];
                num2++;
            }
        }

        for (int j = 0; j < Mission.Current.ActiveMissionObjects.Count; j++)
        {
            if (!(Mission.Current.ActiveMissionObjects[j] is SpawnedItemEntity spawnedItemEntity) || spawnedItemEntity.IsDeactivated)
            {
                continue;
            }

            for (int k = 0; k < num2; k++)
            {
                if (_cachedSelectedVolumeBoxes[k].IsPointIn(spawnedItemEntity.GameEntity.GlobalPosition))
                {
                    spawnedItemEntity.RequestDeletionOnNextTick();
                    break;
                }
            }
        }
    }

    private void HandleEndedChallenge(DuelInfo duel)
    {
        MissionPeer? challengeWinnerPeer = duel.ChallengeWinnerPeer;
        MissionPeer? challengeLoserPeer = duel.ChallengeLoserPeer;
        if (challengeWinnerPeer != null)
        {
            CrpgTrainingGroundMissionRepresentative component = challengeWinnerPeer.GetComponent<CrpgTrainingGroundMissionRepresentative>();
            CrpgTrainingGroundMissionRepresentative? component2 = challengeLoserPeer?.GetComponent<CrpgTrainingGroundMissionRepresentative>();
            MultiplayerClassDivisions.MPHeroClass mPHeroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(challengeWinnerPeer, skipTeamCheck: true);
            MultiplayerClassDivisions.MPHeroClass mPHeroClassForPeer2 = MultiplayerClassDivisions.GetMPHeroClassForPeer(challengeLoserPeer, skipTeamCheck: true);
            float gainedScore = 100 * TaleWorlds.Library.MathF.Max(1f, (float)mPHeroClassForPeer.TroopCasualCost / (float)mPHeroClassForPeer2.TroopCasualCost) * TaleWorlds.Library.MathF.Pow(System.MathF.E, (float)component.NumberOfWins / 10f);
            component.OnDuelWon(gainedScore);
            if (challengeWinnerPeer.Peer.Communicator.IsConnectionActive)
            {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new CrpgTrainingGroundPointsUpdateMessage(component));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
            }

            component2?.ResetBountyAndNumberOfWins();
            if (challengeLoserPeer != null && challengeLoserPeer.Peer.Communicator.IsConnectionActive)
            {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new CrpgTrainingGroundPointsUpdateMessage(component2!));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
            }
        }

        MissionPeer peerComponent = challengeWinnerPeer ?? duel.RequesterPeer;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage(new DuelEnded(peerComponent.GetNetworkPeer()));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    public int GetDuelAreaIndexIfDuelTeam(Team team)
    {
        if (team.IsDefender)
        {
            return _activeDuels.FirstOrDefaultQ((DuelInfo ad) => ad.DuelingTeam == team).DuelAreaIndex;
        }

        return -1;
    }

    public override void OnAgentBuild(Agent agent, Banner banner)
    {
        if (!agent.IsHuman || agent.Team == null || !agent.Team.IsDefender)
        {
            return;
        }

        for (int i = 0; i < _activeDuels.Count; i++)
        {
            if (_activeDuels[i].IsPeerInThisDuel(agent.MissionPeer))
            {
                _activeDuels[i].OnAgentBuild(agent);
                break;
            }
        }
    }

    protected override void HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
        if (networkPeer.IsServerPeer)
        {
            return;
        }

        foreach (NetworkCommunicator networkPeer2 in GameNetwork.NetworkPeers)
        {
            CrpgTrainingGroundMissionRepresentative component = networkPeer2.GetComponent<CrpgTrainingGroundMissionRepresentative>();
            if (component != null)
            {
                GameNetwork.BeginModuleEventAsServer(networkPeer);
                GameNetwork.WriteMessage(new CrpgTrainingGroundPointsUpdateMessage(component));
                GameNetwork.EndModuleEventAsServer();
            }

            if (networkPeer != networkPeer2)
            {
                MissionPeer component2 = networkPeer2.GetComponent<MissionPeer>();
                if (component2 != null)
                {
                    GameNetwork.BeginModuleEventAsServer(networkPeer);
                    GameNetwork.WriteMessage(new SyncPerksForCurrentlySelectedTroop(networkPeer2, component2.Perks[component2.SelectedTroopIndex]));
                    GameNetwork.EndModuleEventAsServer();
                }
            }
        }

        if (SpawnComponent?.SpawningBehavior is CrpgTrainingGroundSpawningBehavior duelSpawningBehavior)
        {
            MissionPeer missionPeer = networkPeer.GetComponent<MissionPeer>();
            duelSpawningBehavior.UpdatedPlayerPreferredArenaOnce.Remove(networkPeer.VirtualPlayer.Id);
            missionPeer?.SpawnTimer?.AdjustStartTime(-3f); // Used to reduce the initial spawn on connect.
        }

        for (int i = 0; i < _activeDuels.Count; i++)
        {
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage(new DuelPreparationStartedForTheFirstTime(_activeDuels[i].RequesterPeer.GetNetworkPeer(), _activeDuels[i].RequesteePeer.GetNetworkPeer(), _activeDuels[i].DuelAreaIndex));
            GameNetwork.EndModuleEventAsServer();
        }
    }

    protected override void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
    {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        for (int i = 0; i < _peersAndSelections.Count; i++)
        {
            if (_peersAndSelections[i].Key == component)
            {
                _peersAndSelections.RemoveAt(i);
                break;
            }
        }
    }

    protected override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
    {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        if (component != null)
        {
            component.Team = null;
        }
    }

    private void OnPeerSelectedPreferredTroopType(MissionPeer missionPeer, TroopType troopType)
    {
        for (int i = 0; i < _peersAndSelections.Count; i++)
        {
            if (_peersAndSelections[i].Key == missionPeer)
            {
                _peersAndSelections[i] = new KeyValuePair<MissionPeer, TroopType>(missionPeer, troopType);
                break;
            }
        }
    }

    private void RewardUsers()
    {
        if (_rewardTickTimer!.Check(reset: true))
        {
            _ = _rewardServer.UpdateCrpgUsersAsync(
                durationRewarded: _rewardTickTimer.GetTimerDuration() * 0.75f,
                durationUpkeep: 0,
                updateUserStats: false);
        }
    }
}
