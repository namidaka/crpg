using System.Text;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Modes.Dtv;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class CrpgDtvViscountUnderAttackMessage : GameNetworkMessage
{
    public int AgentAttackerIndex { get; set; }
    protected override void OnWrite()
    {
        WriteAgentIndexToPacket(AgentAttackerIndex);
    }

    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        AgentAttackerIndex = ReadAgentIndexFromPacket(ref bufferReadValid);
        return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.GameMode;
    }

    protected override string OnGetLogFormat()
    {
        return "cRPG DTV Viscount Under Attack";
    }
}
