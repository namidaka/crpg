using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Commander;
[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal class CommanderPollProgress : GameNetworkMessage
{
    public int VotesAccepted { get; set; }
    public int VotesRejected { get; set; }
    public int TeamIndex { get; set; }

    protected override bool OnRead()
    {
        bool result = true;
        TeamIndex = ReadTeamIndexFromPacket(ref result);
        VotesAccepted = ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref result);
        VotesRejected = ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref result);
        return result;
    }

    protected override void OnWrite()
    {
        WriteTeamIndexToPacket(TeamIndex);
        WriteIntToPacket(VotesAccepted, CompressionBasic.PlayerCompressionInfo);
        WriteIntToPacket(VotesRejected, CompressionBasic.PlayerCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.Administration;
    }

    protected override string OnGetLogFormat()
    {
        return "Update on the commander voting progress.";
    }
}
