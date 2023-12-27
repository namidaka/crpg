using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Commander;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
internal sealed class CommanderUpdateRequest : GameNetworkMessage
{

    protected override bool OnRead()
    {
        bool result = true;
        return result;
    }

    protected override void OnWrite()
    {
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.Administration;
    }

    protected override string OnGetLogFormat()
    {
        return "Requested to update saved commanders";
    }
}
