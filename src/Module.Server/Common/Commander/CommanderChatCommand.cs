using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Commander;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class CommanderChatCommand : GameNetworkMessage
{
    public CommanderChatCommandRejectReason RejectReason { get; set; }
    public float Cooldown { get; set; }
    protected override void OnWrite()
    {
        WriteIntToPacket((int)RejectReason, CompressionBasic.DebugIntNonCompressionInfo);
        WriteFloatToPacket(Cooldown, CompressionInfo.Float.FullPrecision);
    }

    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        RejectReason = (CommanderChatCommandRejectReason)ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);
        Cooldown = ReadFloatFromPacket(CompressionInfo.Float.FullPrecision, ref bufferReadValid);
        return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.GameMode;
    }

    protected override string OnGetLogFormat()
    {
        return "cRPG Commander chat command rejected!";
    }
}
