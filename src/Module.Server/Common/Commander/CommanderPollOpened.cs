using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Commander;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class CommanderPollOpened : GameNetworkMessage
{
    public NetworkCommunicator InitiatorPeer { get; set; } = default!;
    public NetworkCommunicator PlayerPeer { get; set; } = default!;
    protected override bool OnRead()
    {
        bool result = true;
        InitiatorPeer = ReadNetworkPeerReferenceFromPacket(ref result, false);
        PlayerPeer = ReadNetworkPeerReferenceFromPacket(ref result, false);
        return result;
    }

    protected override void OnWrite()
    {
        WriteNetworkPeerReferenceToPacket(InitiatorPeer);
        WriteNetworkPeerReferenceToPacket(PlayerPeer);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.Administration;
    }

    protected override string OnGetLogFormat()
    {
        string?[] array = new string[4];
        array[0] = InitiatorPeer.UserName;
        array[1] = " wants to start poll to promote";
        array[2] = " player: ";
        array[3] = PlayerPeer.UserName;
        return string.Concat(array);
    }
}
