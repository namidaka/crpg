using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Commander;
[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class CommanderPollProgress : GameNetworkMessage
{
    public int VotesAccepted { get; set; }
    public int VotesRejected { get; set; }
    public BattleSideEnum Side { get; set; }

    protected override bool OnRead()
    {
        bool result = true;
        Side = (BattleSideEnum)ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref result);
        VotesAccepted = ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref result);
        VotesRejected = ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref result);
        return result;
    }

    protected override void OnWrite()
    {
        WriteIntToPacket((int)Side, CompressionBasic.DebugIntNonCompressionInfo);
        WriteIntToPacket(VotesAccepted, CompressionBasic.DebugIntNonCompressionInfo);
        WriteIntToPacket(VotesRejected, CompressionBasic.DebugIntNonCompressionInfo);
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
