using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace Crpg.Module.Modes.TrainingGround;

public class CrpgTrainingGroundMissionMultiplayerClient : MissionMultiplayerGameModeBaseClient
{
    public Action OnMyRepresentativeAssigned = default!;
    public override bool IsGameModeUsingGold => false;
    public override bool IsGameModeTactical => false;
    public override bool IsGameModeUsingRoundCountdown => false;
    public override bool IsGameModeUsingAllowCultureChange => true;
    public override bool IsGameModeUsingAllowTroopChange => true;
    public override MultiplayerGameType GameType => MultiplayerGameType.Duel;
    public bool IsInDuel => (GameNetwork.MyPeer.GetComponent<MissionPeer>()?.Team?.IsDefender).GetValueOrDefault();
    public CrpgTrainingGroundMissionRepresentative MyRepresentative { get; private set; } = default!;
    protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
        base.AddRemoveMessageHandlers(registerer);
        if (GameNetwork.IsClientOrReplay)
        {
            registerer.Register<CrpgUpdateTrainingGroundArenaType>(HandleCrpgDuelArenaType);
        }
    }

    private void OnMyClientSynchronized()
    {
        MyRepresentative = GameNetwork.MyPeer.GetComponent<CrpgTrainingGroundMissionRepresentative>();
        OnMyRepresentativeAssigned?.Invoke();
        MyRepresentative.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
    }

    public override int GetGoldAmount()
    {
        return 0;
    }

    public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
    {
    }

    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();
        MissionNetworkComponent.OnMyClientSynchronized += OnMyClientSynchronized;
    }

    public override void OnRemoveBehavior()
    {
        base.OnRemoveBehavior();
        MissionNetworkComponent.OnMyClientSynchronized -= OnMyClientSynchronized;
        MyRepresentative?.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
    }

    public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
    {
        base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
        if (MyRepresentative != null)
        {
            MyRepresentative.CheckHasRequestFromAndRemoveRequestIfNeeded(affectedAgent.MissionPeer);
        }
    }

    public override bool CanRequestCultureChange()
    {
        MissionPeer? missionPeer = GameNetwork.MyPeer?.GetComponent<MissionPeer>();
        if (missionPeer?.Team != null)
        {
            return missionPeer.Team.IsAttacker;
        }

        return false;
    }

    public override bool CanRequestTroopChange()
    {
        MissionPeer? missionPeer = GameNetwork.MyPeer?.GetComponent<MissionPeer>();
        if (missionPeer?.Team != null)
        {
            return missionPeer.Team.IsAttacker;
        }

        return false;
    }

    private void HandleCrpgDuelArenaType(CrpgUpdateTrainingGroundArenaType message)
    {
        if (GameNetwork.MyPeer == null)
        {
            return;
        }

        MissionPeer myMissionPeer = GameNetwork.MyPeer.GetComponent<MissionPeer>();
        if (myMissionPeer == null)
        {
            return;
        }

        Action<TroopType> onMyPreferredZoneChanged = ((CrpgTrainingGroundMissionRepresentative)myMissionPeer.Representative).OnMyPreferredZoneChanged;
        onMyPreferredZoneChanged?.Invoke(message.PlayerTroopType);
    }
}
