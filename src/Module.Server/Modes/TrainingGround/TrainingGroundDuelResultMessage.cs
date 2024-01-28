using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Modes.TrainingGround;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
public sealed class TrainingGroundDuelResultMessage : GameNetworkMessage
{
    public bool HasWonDuel { get; set; }
    public int RatingChange { get; set; }

    protected override void OnWrite()
    {
        WriteBoolToPacket(HasWonDuel);
        WriteIntToPacket(RatingChange, CompressionBasic.DebugIntNonCompressionInfo);
    }

    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        HasWonDuel = ReadBoolFromPacket(ref bufferReadValid);
        RatingChange = ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);
        return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.GameMode;
    }

    protected override string OnGetLogFormat()
    {
        return "Duel result message";
    }
}
