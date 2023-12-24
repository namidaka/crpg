using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common.ChatCommands.Commander;
internal class CommanderCommand : ChatCommand
{
    protected CommanderCommand(ChatCommandsComponent chatComponent)
        : base(chatComponent)
    {
    }

    protected override bool CheckRequirements(NetworkCommunicator fromPeer)
    {
        var command = Mission.Current.GetMissionBehavior<CrpgCommanderBehaviorServer>().GetCommandByNetworkCommunicator(fromPeer);
        if (command?.Commander != fromPeer)
        {
            return false;
        }

        return true;
    }
}
