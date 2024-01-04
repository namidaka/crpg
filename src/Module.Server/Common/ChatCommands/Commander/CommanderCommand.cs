using System.Drawing;
using Crpg.Module.Common.Commander;
using Crpg.Module.Common.Network;
using TaleWorlds.Core;
using TaleWorlds.Diamond;
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
                GameNetwork.BeginModuleEventAsServer(fromPeer);
                GameNetwork.WriteMessage(new CommanderChatCommand { RejectReason = CommanderChatCommandRejectReason.NotCommander });
                GameNetwork.EndModuleEventAsServer();
                return false;
            }

            if (fromPeer.ControlledAgent == null)
            {
                GameNetwork.BeginModuleEventAsServer(fromPeer);
                GameNetwork.WriteMessage(new CommanderChatCommand { RejectReason = CommanderChatCommandRejectReason.Dead });
                GameNetwork.EndModuleEventAsServer();
                return false;
            }

            float earliestMessageTime = commanderServer.LastCommanderMessage[side] + MessageCooldown;
            if (earliestMessageTime > Mission.Current.CurrentTime)
            {
                GameNetwork.BeginModuleEventAsServer(fromPeer);
                GameNetwork.WriteMessage(new CommanderChatCommand { RejectReason = CommanderChatCommandRejectReason.Cooldown, Cooldown = earliestMessageTime - Mission.Current.CurrentTime });
                GameNetwork.EndModuleEventAsServer();
                    
                return false;
            }

            commanderServer.SetCommanderMessageSendTime(side, Mission.Current.CurrentTime);
            return true;
        }

        return false;
    }
}
