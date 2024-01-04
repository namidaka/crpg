using Crpg.Module.Common.Commander;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
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
            if (!commanderServer.IsPlayerACommander(fromPeer))
            {
                return false;
            }

            if (fromPeer.ControlledAgent == null)
            {
                ChatComponent.ServerSendMessageToPlayer(fromPeer, Color.White, new TextObject("{=C7TYZ8s0}You cannot order troops when you are dead!").ToString());
                return false;
            }

            float earliestMessageTime = commanderServer.LastCommanderMessage[side] + MessageCooldown;
            if (earliestMessageTime > Mission.Current.CurrentTime)
            {
                ChatComponent.ServerSendMessageToPlayer(fromPeer, Color.White,
                    new TextObject("{=uRmpZM0q}Please wait {COOLDOWN} seconds before issuing a new order!",
                    new Dictionary<string, object> { ["COOLDOWN"] = (earliestMessageTime - Mission.Current.CurrentTime).ToString("0.0") }).ToString());
                return false;
            }

            commanderServer.SetCommanderMessageSendTime(side, Mission.Current.CurrentTime);
            return true;
        }

        return false;
    }
}
