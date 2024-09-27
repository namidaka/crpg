using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Modes.TrainingGround;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
internal sealed class ClientRequestRefreshCharacter : GameNetworkMessage
{
    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        return bufferReadValid;
    }

    protected override void OnWrite()
    {
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.GameMode;
    }

    protected override string OnGetLogFormat()
    {
        return "Requested refresh character";
    }
}
