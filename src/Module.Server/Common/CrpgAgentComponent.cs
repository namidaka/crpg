using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common;
internal class CrpgAgentComponent : AgentComponent
{
    public CrpgAgentComponent(Agent agent)
        : base(agent)
    {
        agent.OnAgentWieldedItemChange = (Action)Delegate.Combine(agent.OnAgentWieldedItemChange, new Action(DropShieldIfNeeded));
    }
    public virtual void OnDismount(Agent mount)
    {
        DropShieldIfNeeded();
    }
    public void DropShieldIfNeeded()
    {
        if (Agent.HasMount)
        {
            MissionEquipment equipment = Agent.Equipment;
            EquipmentIndex wieldedItemIndex = Agent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
            WeaponComponentData? secondaryItem = wieldedItemIndex != EquipmentIndex.None
                ? equipment[wieldedItemIndex].CurrentUsageItem
                : null;
            if (secondaryItem == null)
            {
                return;
            }

            if (secondaryItem.WeaponClass == WeaponClass.LargeShield)
            {
                Agent.DropItem(wieldedItemIndex);
            }
        }
    }
}
