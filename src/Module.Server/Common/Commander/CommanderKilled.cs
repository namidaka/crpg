using System.Text;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Commander;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class CommanderKilled : GameNetworkMessage
{
    public int AgentKillerIndex { get; set; }
    public int AgentCommanderIndex { get; set; }
    protected override void OnWrite()
    {
        WriteAgentIndexToPacket(AgentKillerIndex);
        WriteAgentIndexToPacket(AgentCommanderIndex);
    }

    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        AgentKillerIndex = ReadAgentIndexFromPacket(ref bufferReadValid);
        AgentCommanderIndex = ReadAgentIndexFromPacket(ref bufferReadValid);
        return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.GameMode;
    }

    protected override string OnGetLogFormat()
    {
        return "cRPG Commander Killed!";
    }
}
