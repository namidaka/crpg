using Crpg.Module.Common.Commander;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common.ChatCommands.Commander;
internal class CommanderCommand : ChatCommand
{
    private const int MessageCooldown = 30;

    protected CommanderCommand(ChatCommandsComponent chatComponent)
        : base(chatComponent)
    {
    }

    protected override bool CheckRequirements(NetworkCommunicator fromPeer)
    {
        BattleSideEnum side = fromPeer.GetComponent<MissionPeer>().Team.Side;
        CrpgCommanderBehaviorServer? commanderServer = Mission.Current.GetMissionBehavior<CrpgCommanderBehaviorServer>();
        if (commanderServer != null)
        {
            if (commanderServer.IsPlayerACommander(fromPeer))
            {
                return false;
            }

            if (fromPeer.ControlledAgent == null)
            {
                ChatComponent.ServerSendMessageToPlayer(fromPeer, Color.White, "You cannot order troops when you are dead!");
                return false;
            }

            float earliestMessageTime = commanderServer.LastCommanderMessage[side] + MessageCooldown;
            if (earliestMessageTime > Mission.Current.CurrentTime)
            {
                ChatComponent.ServerSendMessageToPlayer(fromPeer, Color.White, $"Please wait {earliestMessageTime - Mission.Current.CurrentTime:0.00} seconds before issuing a new order!");
                return false;
            }

            commanderServer.SetCommanderMessageSendTime(side, Mission.Current.CurrentTime);
            return true;

        }

        return false;
    }
}
