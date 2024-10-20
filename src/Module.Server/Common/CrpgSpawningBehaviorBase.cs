using Crpg.Module.Api.Models.Captains;
using Crpg.Module.Api.Models.Characters;
using Crpg.Module.Api.Models.Users;
using Microsoft.VisualBasic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using MathF = TaleWorlds.Library.MathF;

namespace Crpg.Module.Common;

internal abstract class CrpgSpawningBehaviorBase : SpawningBehaviorBase
{
    private readonly CrpgConstants _constants;

    private readonly List<WeaponClass> allowedSpawnWeaponClass = new()
    {
        WeaponClass.Dagger,
        WeaponClass.Mace,
        WeaponClass.TwoHandedMace,
        WeaponClass.OneHandedSword,
        WeaponClass.TwoHandedSword,
        WeaponClass.OneHandedAxe,
        WeaponClass.TwoHandedAxe,
        WeaponClass.Pick,
        WeaponClass.LowGripPolearm,
        WeaponClass.OneHandedPolearm,
        WeaponClass.TwoHandedPolearm,
        WeaponClass.Javelin,
        WeaponClass.Stone,
        WeaponClass.ThrowingAxe,
        WeaponClass.ThrowingKnife,
    };
    private MultiplayerGameType gameMode = MultiplayerGameType.Battle;

    public virtual MultiplayerGameType GameMode { get => gameMode; protected set => gameMode = value; }
    public CrpgSpawningBehaviorBase(CrpgConstants constants)
    {
        _constants = constants;
    }

    public override bool AllowEarlyAgentVisualsDespawning(MissionPeer missionPeer)
    {
        return false;
    }

    public override void Initialize(SpawnComponent spawnComponent)
    {
        base.Initialize(spawnComponent);
        base.OnAllAgentsFromPeerSpawnedFromVisuals += OnAllAgentsFromPeerSpawnedFromVisuals;
    }

    public override void RequestStartSpawnSession()
    {
        base.RequestStartSpawnSession();
        ResetSpawnTeams();
    }

    protected virtual bool IsPlayerAllowedToSpawn(NetworkCommunicator networkPeer)
    {
        return true;
    }

    protected virtual bool IsBotTeamAllowedToSpawn(Team team)
    {
        return true;
    }

    protected override void SpawnAgents()
    {
        BasicCultureObject cultureTeam1 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
        BasicCultureObject cultureTeam2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue());
        int p = 1;
        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        {
            MissionPeer missionPeer = networkPeer.GetComponent<MissionPeer>();
            CrpgPeer crpgPeer = networkPeer.GetComponent<CrpgPeer>();
            if (!networkPeer.IsSynchronized
                || missionPeer == null
                || missionPeer.ControlledAgent != null
                || missionPeer.Team == null
                || missionPeer.Team == Mission.SpectatorTeam
                || crpgPeer == null
                || crpgPeer.UserLoading
                || crpgPeer.User == null
                || !IsPlayerAllowedToSpawn(networkPeer))
            {
                continue;
            }

            BasicCultureObject teamCulture = missionPeer.Team == Mission.AttackerTeam ? cultureTeam1 : cultureTeam2;
            var peerClass = MBObjectManager.Instance.GetObject<MultiplayerClassDivisions.MPHeroClass>($"crpg_class_division_{p}");
            // var character = CreateCharacter(crpgPeer.User.Character, _constants);
            var characterSkills = CrpgCharacterBuilder.CreateCharacterSkills(crpgPeer.User!.Character.Characteristics);
            var characterXml = peerClass.HeroCharacter;

            var characterEquipment = CrpgCharacterBuilder.CreateCharacterEquipment(crpgPeer.User.Character.EquippedItems);
            /* CrpgCharacterObject character = CreateCharacter(crpgPeer.User.Character, _constants, characterSkills, characterEquipment);
             * is still not possible. Characters are tightly coupled to xmls. The code does things like

                *MBObjectManager.Instance.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>().FirstOrDefau
                  ((MultiplayerClassDivisions.MPHeroClass x) => x.HeroCharacter == character || x.TroopCharacter == character);
            Expecting that a character always exist in xmls
             */

            bool hasMount = characterEquipment[EquipmentIndex.Horse].Item != null;

            bool firstSpawn = missionPeer.SpawnCountThisRound == 0;
            MatrixFrame spawnFrame = SpawnComponent.GetSpawnFrame(missionPeer.Team, hasMount, firstSpawn);
            Vec2 initialDirection = spawnFrame.rotation.f.AsVec2.Normalized();
            // Randomize direction so players don't go all straight.
            initialDirection.RotateCCW(MBRandom.RandomFloatRanged(-MathF.PI / 3f, MathF.PI / 3f));
            var troopOrigin = new CrpgBattleAgentOrigin(characterXml, characterSkills);
            CrpgCharacterBuilder.AssignArmorsToTroopOrigin(troopOrigin, crpgPeer.User.Character.EquippedItems.ToList());
            Formation formation = missionPeer.ControlledFormation;
            if (formation == null)
            {
                formation = missionPeer.Team.FormationsIncludingEmpty.First((Formation x) => x.PlayerOwner == null && x.CountOfUnits == 0);
                formation.ContainsAgentVisuals = true;
                if (string.IsNullOrEmpty(formation.BannerCode))
                {
                    formation.BannerCode = crpgPeer.Clan?.BannerKey ?? missionPeer.Peer.BannerCode;
                }
            }

            missionPeer.ControlledFormation = formation;
            missionPeer.HasSpawnedAgentVisuals = true;
            AgentBuildData agentBuildData = new AgentBuildData(characterXml)
                .MissionPeer(missionPeer)
                .Equipment(characterEquipment)
                .TroopOrigin(troopOrigin)
                .Team(missionPeer.Team)
                .VisualsIndex(0)
                .IsFemale(missionPeer.Peer.IsFemale)
                // base.GetBodyProperties uses the player-defined body properties but some body properties may have been
                // causing crashes. So here we send the body properties from the characters.xml which we know are safe.
                // Note that what is sent here doesn't matter since it's ignored by the client.
                .BodyProperties(characterXml.GetBodyPropertiesMin())
                .InitialPosition(in spawnFrame.origin)
                .InitialDirection(in initialDirection)
                .Formation(formation);

            if (crpgPeer.Clan != null)
            {
                agentBuildData.ClothingColor1(crpgPeer.Clan.PrimaryColor);
                agentBuildData.ClothingColor2(crpgPeer.Clan.SecondaryColor);
                if (!string.IsNullOrEmpty(crpgPeer.Clan.BannerKey))
                {
                    agentBuildData.Banner(new Banner(crpgPeer.Clan.BannerKey));
                }
            }
            else
            {
                agentBuildData.ClothingColor1(missionPeer.Team == Mission.AttackerTeam
                    ? teamCulture.Color
                    : teamCulture.ClothAlternativeColor);
                agentBuildData.ClothingColor2(missionPeer.Team == Mission.AttackerTeam
                    ? teamCulture.Color2
                    : teamCulture.ClothAlternativeColor2);
            }

            Agent agent = Mission.SpawnAgent(agentBuildData);
            OnPeerSpawned(agent);
            CrpgAgentComponent agentComponent = new(agent);
            agent.AddComponent(agentComponent);

            bool hasExtraSlotEquipped = characterEquipment[EquipmentIndex.ExtraWeaponSlot].Item != null;
            if (!agent.HasMount || hasExtraSlotEquipped)
            {
                agent.WieldInitialWeapons();
            }

            if (IsRoundInProgress() && gameMode == MultiplayerGameType.Captain)
            {
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
                var captainFormations = crpgPeer.User.Captain.Formations.Where(f => f.Character != null);
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
                        SpawnBotAgent(peerClass.StringId, agent.Team, missionPeer, p);
                    }
                }

                foreach (KeyValuePair<int, double> captainFormation in formationBotWeight)
                {
                    for (int i = 0; i < (int)(captainFormation.Value * peerNumberOfBots); i++)
                    {
                        SpawnBotAgent(peerClass.StringId, agent.Team, missionPeer, p, captainFormation.Key);
                    }
                }

            }

            p++;
            // AgentVisualSpawnComponent.RemoveAgentVisuals(missionPeer, sync: true);
        }
    }

    private CrpgCharacterObject CreateCharacter(CrpgCharacter character, CrpgConstants constants, CharacterSkills skills, Equipment equipment)
    {
        CrpgCharacterObject characterObject = new(skills, equipment)
        {
            Level = character.Level,
        };
        return characterObject;
    }

    protected Agent SpawnBotAgent(string classDivisionId, Team team, MissionPeer? peer = null, int peerId = 0, int formationId = 0)
    {
        var teamCulture = team.Side == BattleSideEnum.Attacker
            ? MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue())
            : MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue());

        MultiplayerClassDivisions.MPHeroClass botClass = MultiplayerClassDivisions
            .GetMPHeroClasses()
        .First(h => h.StringId == classDivisionId);
        BasicCharacterObject character = botClass.HeroCharacter;

        bool hasMount = character.Equipment[EquipmentIndex.Horse].Item != null;
        MatrixFrame spawnFrame = SpawnComponent.GetSpawnFrame(team, hasMount, true);
        Vec2 initialDirection = spawnFrame.rotation.f.AsVec2.Normalized();

        AgentBuildData agentBuildData = new AgentBuildData(character)
            .Equipment(character.AllEquipments[MBRandom.RandomInt(character.AllEquipments.Count)])
            .TroopOrigin(new BasicBattleAgentOrigin(character))
            .EquipmentSeed(MissionLobbyComponent.GetRandomFaceSeedForCharacter(character))
            .Team(team)
            .VisualsIndex(0)
            .InitialPosition(in spawnFrame.origin)
            .InitialDirection(in initialDirection)
            .IsFemale(character.IsFemale)
            .ClothingColor1(
                team.Side == BattleSideEnum.Attacker ? teamCulture.Color : teamCulture.ClothAlternativeColor)
            .ClothingColor2(team.Side == BattleSideEnum.Attacker
                ? teamCulture.Color2
                : teamCulture.ClothAlternativeColor2);

        if (peer != null)
        {
            var crpgPeer = peer.GetComponent<CrpgPeer>();
            if (crpgPeer != null && crpgPeer?.User != null)
            {
                var formation = GetFormationFromPeer(peer, formationId);

                var formationCharacter = formation?.Character ?? crpgPeer.User.Character;
                string characterClass = formation != null ? $"crpg_class_division_{peerId}_{formation.Number}" : $"crpg_class_division_{peerId}";
                botClass = MultiplayerClassDivisions
                    .GetMPHeroClasses()
                    .First(h => h.StringId == characterClass);
                character = botClass.HeroCharacter;
                agentBuildData = new AgentBuildData(character)
                    .Equipment(character.AllEquipments[MBRandom.RandomInt(character.AllEquipments.Count)])
                    .TroopOrigin(new BasicBattleAgentOrigin(character))
                    .EquipmentSeed(MissionLobbyComponent.GetRandomFaceSeedForCharacter(character))
                    .Team(team)
                    .VisualsIndex(0)
                    .InitialPosition(in spawnFrame.origin)
                    .InitialDirection(in initialDirection)
                    .IsFemale(character.IsFemale)
                    .ClothingColor1(
                        team.Side == BattleSideEnum.Attacker ? teamCulture.Color : teamCulture.ClothAlternativeColor)
                    .ClothingColor2(team.Side == BattleSideEnum.Attacker
                        ? teamCulture.Color2
                        : teamCulture.ClothAlternativeColor2);
                var characterEquipment = CrpgCharacterBuilder.CreateBotCharacterEquipment(formationCharacter.EquippedItems);
                var peerClass = MBObjectManager.Instance.GetObject<MultiplayerClassDivisions.MPHeroClass>(characterClass);
                var characterSkills = CrpgCharacterBuilder.CreateCharacterSkills(formationCharacter.Characteristics);
                var characterXml = peerClass.HeroCharacter;
                var troopOrigin = new CrpgBattleAgentOrigin(characterXml, characterSkills);
                agentBuildData.OwningMissionPeer(peer);
                agentBuildData.Formation(peer.ControlledFormation);
                agentBuildData.Equipment(characterEquipment);
                agentBuildData.TroopOrigin(troopOrigin);
                agentBuildData.Banner(new Banner(peer.Peer.BannerCode));

                if (crpgPeer.Clan != null)
                {
                    agentBuildData.ClothingColor1(crpgPeer.Clan.PrimaryColor);
                    agentBuildData.ClothingColor2(crpgPeer.Clan.SecondaryColor);
                }
            }
        }

        var bodyProperties = BodyProperties.GetRandomBodyProperties(
            character.Race,
            character.IsFemale,
            character.GetBodyPropertiesMin(),
            character.GetBodyPropertiesMax(),
            (int)agentBuildData.AgentOverridenSpawnEquipment.HairCoverType,
            agentBuildData.AgentEquipmentSeed,
            character.HairTags,
            character.BeardTags,
            character.TattooTags);
        agentBuildData.BodyProperties(bodyProperties);

        Agent agent = Mission.SpawnAgent(agentBuildData);
#if CRPG_SERVER
        if (!CrpgServerConfiguration.FrozenBots)
        {
            agent.SetWatchState(Agent.WatchState.Alarmed);
        }
#endif
        agent.WieldInitialWeapons();
        return agent;
    }

    protected void SpawnBotAgents()
    {
        int botsTeam1 = MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue();
        int botsTeam2 = MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue();

        if (botsTeam1 <= 0 && botsTeam2 <= 0)
        {
            return;
        }

        Mission.Current.AllowAiTicking = false;
        foreach (Team team in Mission.Teams)
        {
            if (Mission.AttackerTeam != team && Mission.DefenderTeam != team)
            {
                continue;
            }

            if (!IsBotTeamAllowedToSpawn(team))
            {
                continue;
            }

            int numberOfBots = team.Side == BattleSideEnum.Attacker
                ? botsTeam1
                : botsTeam2;
            int numberOfPlayers = GameNetwork.NetworkPeers.Count(p => p.IsSynchronized && p.GetComponent<MissionPeer>()?.Team == team);
            int botsAlive = team.ActiveAgents.Count(a => a.IsAIControlled && a.IsHuman);

            for (int i = 0 + botsAlive + numberOfPlayers; i < numberOfBots; i += 1)
            {
                MultiplayerClassDivisions.MPHeroClass botClass = MultiplayerClassDivisions
                    .GetMPHeroClasses()
                    .GetRandomElementWithPredicate<MultiplayerClassDivisions.MPHeroClass>(x => x.StringId.StartsWith("crpg_bot_"));
                SpawnBotAgent(botClass.StringId, team);
            }
        }
    }

    protected virtual void OnPeerSpawned(Agent agent)
    {
        var crpgPeer = agent.MissionPeer.GetComponent<CrpgPeer>();
        crpgPeer.LastSpawnInfo = new SpawnInfo(agent.MissionPeer.Team, crpgPeer.User!.Character.EquippedItems);
    }

    protected bool DoesEquipmentContainWeapon(Equipment equipment)
    {
        foreach (var weaponClass in allowedSpawnWeaponClass)
        {
            if (equipment.HasWeaponOfClass(weaponClass))
            {
                return true;
            }
        }

        return false;
    }

    private void ResetSpawnTeams()
    {
        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        {
            var crpgPeer = networkPeer.GetComponent<CrpgPeer>();
            if (crpgPeer != null && networkPeer.ControlledAgent == null)
            {
                crpgPeer.LastSpawnInfo = null;
            }
        }
    }

    private new void OnAllAgentsFromPeerSpawnedFromVisuals(MissionPeer peer)
    {
        if (peer.ControlledFormation != null)
        {
            peer.ControlledFormation.OnFormationDispersed();
            peer.ControlledFormation.SetMovementOrder(MovementOrder.MovementOrderFollow(peer.ControlledAgent));
            NetworkCommunicator networkPeer = peer.GetNetworkPeer();
            if (peer.BotsUnderControlAlive != 0 || peer.BotsUnderControlTotal != 0)
            {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new BotsControlledChange(networkPeer, peer.BotsUnderControlAlive, peer.BotsUnderControlTotal));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
                Mission.GetMissionBehavior<MissionMultiplayerGameModeFlagDominationClient>().OnBotsControlledChanged(peer, peer.BotsUnderControlAlive, peer.BotsUnderControlTotal);
            }

            if (peer.Team == Mission.AttackerTeam)
            {
                Mission.NumOfFormationsSpawnedTeamOne++;
            }
            else
            {
                Mission.NumOfFormationsSpawnedTeamTwo++;
            }

            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage(new SetSpawnedFormationCount(Mission.NumOfFormationsSpawnedTeamOne, Mission.NumOfFormationsSpawnedTeamTwo));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
        }
    }
    private CrpgCaptainFormation? GetFormationFromPeer(MissionPeer peer, int formationId)
    {
        var crpgPeer = peer.GetComponent<CrpgPeer>();
        if (crpgPeer != null && crpgPeer.User != null)
        {
            return crpgPeer.User.Captain.Formations.Where(f => f.Number == formationId).FirstOrDefault();
        }

        return null;
    }
}
