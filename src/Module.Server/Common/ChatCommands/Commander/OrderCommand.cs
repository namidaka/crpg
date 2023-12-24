using System;
using System.Collections.Generic;
using System.Text;
using Crpg.Module.Notifications;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common.ChatCommands.Commander;
internal class OrderCommand : CommanderCommand
{
    public OrderCommand(ChatCommandsComponent chatComponent)
         : base(chatComponent)
    {
        Name = "o";
        Description = $"'{ChatCommandsComponent.CommandPrefix}{Name} message' to send an order to your troops.";
        Overloads = new CommandOverload[]
        {
            new(new[] { ChatCommandParameterType.String }, ExecuteAnnouncement),
        };
    }

    private void ExecuteAnnouncement(NetworkCommunicator fromPeer, object[] arguments)
    {
        string message = (string)arguments[0];
        Team peerTeam = fromPeer.ControlledAgent.Team;

        foreach (Agent targetAgent in peerTeam.TeamAgents)
        {
            NetworkCommunicator? targetPeer = targetAgent?.MissionPeer.GetNetworkPeer();
            if (targetPeer != null && targetPeer.IsServerPeer && targetPeer.IsSynchronized)
            {
                GameNetwork.BeginModuleEventAsServer(targetPeer);
                GameNetwork.WriteMessage(new CrpgNotification
                {
                    Type = CrpgNotificationType.Announcement,
                    Message = message,
                });
                GameNetwork.EndModuleEventAsServer();
            }
        }
    }
}
