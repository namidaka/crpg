using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Modes.TrainingGround;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
internal sealed class TrainingGroundClientDuelRequest : GameNetworkMessage
{
    public int RequestedAgentIndex { get; set; }

    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        RequestedAgentIndex = ReadAgentIndexFromPacket(ref bufferReadValid);
        return bufferReadValid;
    }

    protected override void OnWrite()
    {
        WriteAgentIndexToPacket(RequestedAgentIndex);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.GameMode;
    }

    protected override string OnGetLogFormat()
    {
        return "Request duel from agent with index: " + RequestedAgentIndex;
    }
}
