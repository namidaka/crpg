using Crpg.Module.Common.Network;
using TaleWorlds.Library;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common.Commander;
internal class CrpgCommanderBehaviorServer : MissionBehavior
{
    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

    public Dictionary<BattleSideEnum, NetworkCommunicator?> Commanders = new();
    public NetworkCommunicator? this[BattleSideEnum key]
    {
        // returns value if exists
        get { return _commanders[key]; }

        // updates if exists, adds if doesn't exist
        set { _commanders[key] = value; }
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

    public void CreateCommand(NetworkCommunicator commander)
    {
        BattleSideEnum commanderSide = commander.GetComponent<MissionPeer>().Team.Side;
        Commanders[commanderSide] = commander;
        OnCommanderUpdated();
    }

    public void RemoveCommand(NetworkCommunicator commander)
    {
        var commanderSide = commander.GetComponent<MissionPeer>().Team.Side;
        Commanders[commanderSide] = null;
        OnCommanderUpdated();
    }

    public void SetCommanderMessageSendTime(BattleSideEnum side,  float time)
    {
        LastCommanderMessage[side] = time;
    }

    public void OnCommanderUpdated()
    {
        Debug.Print("OnCommanderUpdated called!");
        foreach (KeyValuePair<BattleSideEnum, NetworkCommunicator?> keyValuePair in Commanders)
        {
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage(new UpdateCommander { Side = keyValuePair.Key, Commander = keyValuePair.Value });
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
        }
    }

    public bool IsPlayerACommander(NetworkCommunicator networkCommunicator)
    {
        return Commanders.ContainsValue(networkCommunicator);
    }

}
