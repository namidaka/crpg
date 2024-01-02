using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Modes.TrainingGround;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class TrainingGroundServerDuelRequest : GameNetworkMessage
{
    public int RequesterAgentIndex { get; set; }

    public int RequestedAgentIndex { get; set; }

    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        RequesterAgentIndex = ReadAgentIndexFromPacket(ref bufferReadValid);
        RequestedAgentIndex = ReadAgentIndexFromPacket(ref bufferReadValid);
        return bufferReadValid;
    }

    protected override void OnWrite()
    {
        WriteAgentIndexToPacket(RequesterAgentIndex);
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
