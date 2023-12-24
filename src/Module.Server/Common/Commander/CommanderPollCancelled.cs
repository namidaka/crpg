using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Commander;
[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class CommanderPollCancelled : GameNetworkMessage
{
    public int TeamIndex { get; set; }
    protected override bool OnRead()
    {
        bool result = true;
        TeamIndex = ReadTeamIndexFromPacket(ref result);
        return result;
    }

    protected override void OnWrite()
    {
        WriteTeamIndexToPacket(TeamIndex);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.Administration;
    }

    protected override string OnGetLogFormat()
    {
        return "Commander poll cancelled.";
    }
}
