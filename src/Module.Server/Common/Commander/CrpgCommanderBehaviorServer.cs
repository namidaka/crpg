using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common.Commander;
internal class CrpgCommanderBehaviorServer : MissionNetwork
{
    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;
    public Dictionary<BattleSideEnum, NetworkCommunicator?> Commanders = new();
    public NetworkCommunicator? this[BattleSideEnum key]
    {
        // returns value if exists
        get { return _commanders[key]; }

        // updates if exists, adds if doesn't exist
        private set { _commanders[key] = value; }
    }

    public Dictionary<BattleSideEnum, float> LastCommanderMessage { get; private set; } = new();
    private Dictionary<BattleSideEnum, NetworkCommunicator?> _commanders = new();

    public CrpgCommanderBehaviorServer()
    {
        _commanders[BattleSideEnum.Attacker] = null;
        _commanders[BattleSideEnum.Defender] = null;
        _commanders[BattleSideEnum.None] = null;

        LastCommanderMessage.Add(BattleSideEnum.Attacker, 0);
        LastCommanderMessage.Add(BattleSideEnum.Defender, 0);
        LastCommanderMessage.Add(BattleSideEnum.None, 0);
    }

    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();
        MissionPeer.OnTeamChanged += HandlePeerTeamChanged;
    }

    public override void OnRemoveBehavior()
    {
        base.OnRemoveBehavior();
        MissionPeer.OnTeamChanged -= HandlePeerTeamChanged;
    }

    public void CreateCommand(NetworkCommunicator commander)
    {
        BattleSideEnum commanderSide = commander.GetComponent<MissionPeer>().Team.Side;
        Commanders[commanderSide] = commander;
        OnCommanderUpdated(commanderSide);
    }

    public void RemoveCommand(NetworkCommunicator commander)
    {
        foreach (KeyValuePair<BattleSideEnum, NetworkCommunicator?> keyValuePair in Commanders)
        {
            if (keyValuePair.Value == commander)
            {
                Commanders[keyValuePair.Key] = null;
                OnCommanderUpdated(keyValuePair.Key);
            }
        }
    }

    public void SetCommanderMessageSendTime(BattleSideEnum side,  float time)
    {
        LastCommanderMessage[side] = time;
    }

    public void OnCommanderUpdated(BattleSideEnum side)
{
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage(new UpdateCommander { Side = side, Commander = Commanders[side] });
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    public bool IsPlayerACommander(NetworkCommunicator networkCommunicator)
    {
        return Commanders.ContainsValue(networkCommunicator);
    }

    protected override void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
        foreach (KeyValuePair<BattleSideEnum, NetworkCommunicator?> keyValuePair in Commanders)
        {
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage(new UpdateCommander { Side = keyValuePair.Key, Commander = keyValuePair.Value });
            GameNetwork.EndModuleEventAsServer();
        }
    }

    public override void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
    {
        if (IsPlayerACommander(networkPeer))
        {
            RemoveCommand(networkPeer);
        }
    }

    public void HandlePeerTeamChanged(NetworkCommunicator peer, Team previousTeam, Team newTeam)
    {
        if (peer != null)
        {
            if (IsPlayerACommander(peer))
            {
                RemoveCommand(peer);
            }
        }
    }

    public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
    {
        NetworkCommunicator? networkPeer = affectedAgent.MissionPeer?.GetNetworkPeer();
        if (networkPeer != null)
        {
            if (IsPlayerACommander(networkPeer))
            {
                if (agentState == AgentState.Deleted)
                {
                    return;
                }
                else
                {
                    GameNetwork.BeginBroadcastModuleEvent();
                    GameNetwork.WriteMessage(new CommanderKilled { AgentCommanderIndex = affectedAgent.Index, AgentKillerIndex = affectorAgent.Index });
                    GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
                }
            }
        }
    }
}
