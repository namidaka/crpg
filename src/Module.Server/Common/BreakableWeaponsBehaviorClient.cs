﻿using Crpg.Module.Common.Network;
using Crpg.Module.Notifications;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;
using Timer = TaleWorlds.Core.Timer;

namespace Crpg.Module.Common;

internal class BreakableWeaponsBehaviorClient : MissionNetwork
{

    protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
        base.AddRemoveMessageHandlers(registerer);
        registerer.Register<UpdateWeaponHealth>(HandleUpdateWeaponHealth);

    }

    private void HandleUpdateWeaponHealth(UpdateWeaponHealth message)
    {
        if (message.Agent == null ) return;

        Agent agentToUpdate = Mission.Current.Agents.FirstOrDefault(a => a == message.Agent);
        if (agentToUpdate == null) return;
        agentToUpdate.ChangeWeaponHitPoints(message.EquipmentIndex, (short)message.WeaponHealth);

    }

    public override void OnAgentBuild(Agent agent, Banner banner)
    {
        if (agent == null) return;

        if (agent.Equipment == null)
        {
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            MissionWeapon weapon = agent.Equipment[i];


            if (!BreakableWeaponsBehaviorServer.breakAbleItemsHitPoints.TryGetValue(weapon.Item?.StringId ?? string.Empty, out short baseHitPoints))
            {
                continue;
            }

            agent.ChangeWeaponHitPoints((EquipmentIndex)i, baseHitPoints);
            InformationManager.DisplayMessage(new InformationMessage($"set the hp of weapon {weapon.Item?.Name.ToString() ?? string.Empty}  to {baseHitPoints}"));
        }
    }
}
