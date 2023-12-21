using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Modes.TrainingGround;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
public sealed class CrpgTrainingGroundPointsUpdateMessage : GameNetworkMessage
{
    public int Bounty { get; private set; }
    public int Score { get; private set; }
    public int NumberOfWins { get; private set; }
    public NetworkCommunicator NetworkCommunicator { get; private set; } = default!;
    public CrpgTrainingGroundPointsUpdateMessage()
    {
    }

    public CrpgTrainingGroundPointsUpdateMessage(CrpgTrainingGroundMissionRepresentative representative)
    {
        Bounty = representative.Bounty;
        Score = representative.Score;
        NumberOfWins = representative.NumberOfWins;
        NetworkCommunicator = representative.GetNetworkPeer();
    }

    protected override void OnWrite()
    {
        WriteIntToPacket(Bounty, CompressionMatchmaker.ScoreCompressionInfo);
        WriteIntToPacket(Score, CompressionMatchmaker.ScoreCompressionInfo);
        WriteIntToPacket(NumberOfWins, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
        WriteNetworkPeerReferenceToPacket(NetworkCommunicator);
    }

    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        Bounty = ReadIntFromPacket(CompressionMatchmaker.ScoreCompressionInfo, ref bufferReadValid);
        Score = ReadIntFromPacket(CompressionMatchmaker.ScoreCompressionInfo, ref bufferReadValid);
        NumberOfWins = ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref bufferReadValid);
        NetworkCommunicator = ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
        return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.GameMode;
    }

    protected override string OnGetLogFormat()
    {
        return "PointUpdateMessage";
    }
}
