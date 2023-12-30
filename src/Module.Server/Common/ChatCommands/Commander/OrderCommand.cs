﻿using System;
using System.Collections.Generic;
using System.Text;
using Crpg.Module.Notifications;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;

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
        MissionPeer? missionPeer = fromPeer.GetComponent<MissionPeer>();

        foreach (NetworkCommunicator targetPeer in GameNetwork.NetworkPeers)
        {
            if (targetPeer.GetComponent<MissionPeer>()?.Team.Side == missionPeer.Team.Side)
            {
                if (true && targetPeer.IsSynchronized) // todo: from true to IsServerPeer
                {
                    GameNetwork.BeginModuleEventAsServer(targetPeer);
                    GameNetwork.WriteMessage(new CrpgNotification
                    {
                        Type = CrpgNotificationType.Commander,
                        Message = message,
                    });
                    GameNetwork.EndModuleEventAsServer();
                }
            }

        }
    }
}