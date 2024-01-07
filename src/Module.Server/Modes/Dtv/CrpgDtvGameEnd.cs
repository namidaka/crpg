using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Modes.Dtv;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class CrpgDtvGameEnd : GameNetworkMessage
{
    public bool VipDead { get; set; }
    public int VipAgentIndex { get; set; }

    protected override void OnWrite()
    {
        WriteBoolToPacket(VipDead);
        WriteAgentIndexToPacket(VipAgentIndex);
    }

    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        VipDead = ReadBoolFromPacket(ref bufferReadValid);
        VipAgentIndex = ReadAgentIndexFromPacket(ref bufferReadValid);
        return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.GameMode;
    }

    protected override string OnGetLogFormat()
    {
        return "cRPG DTV VIP Death Data";
    }
}
