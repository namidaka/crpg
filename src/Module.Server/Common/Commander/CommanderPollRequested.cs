using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Commander;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
internal sealed class CommanderPollRequested : GameNetworkMessage
{
    public NetworkCommunicator PlayerPeer { get; set; } = default!;

    protected override bool OnRead()
    {
        bool result = true;
        PlayerPeer = ReadNetworkPeerReferenceFromPacket(ref result, true);
        return result;
    }

    protected override void OnWrite()
    {
        WriteNetworkPeerReferenceToPacket(PlayerPeer);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.Administration;
    }

    protected override string OnGetLogFormat()
    {
        string str = "Requested to start poll to promote";
        string str2 = " player: ";
        NetworkCommunicator playerPeer = PlayerPeer;
        return str + str2 + playerPeer?.UserName;
    }
}
