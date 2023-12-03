using Crpg.Module.Common.Network;
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

internal class BreakableWeaponsBehaviorServer : MissionBehavior
{
    public static readonly Dictionary<string, short> BreakAbleItemsHitPoints = new()
    {
        { "crpg_joustinglance_striped_ry_v1_h0", 500 },
        { "crpg_joustinglance_striped_ry_v1_h1", 550 },
        { "crpg_joustinglance_striped_ry_v1_h2", 600 },
        { "crpg_joustinglance_striped_ry_v1_h3", 650 },
    };

    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

    public override void OnAgentBuild(Agent agent, Banner banner)
    {
        if (agent == null)
        {
            return;
        }

        if (agent.Equipment == null)
        {
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            MissionWeapon weapon = agent.Equipment[i];

            if (!BreakAbleItemsHitPoints.TryGetValue(weapon.Item?.StringId ?? string.Empty, out short baseHitPoints))
            {
                continue;
            }

            agent.ChangeWeaponHitPoints((EquipmentIndex)i, baseHitPoints);
        }
    }

    public override void OnMeleeHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
    {
        if (attacker?.Equipment[collisionData.AffectorWeaponSlotOrMissileIndex].Item == null)
        {
            return;
        }

        if (!BreakAbleItemsHitPoints.TryGetValue(attacker?.Equipment[collisionData.AffectorWeaponSlotOrMissileIndex].Item.StringId ?? string.Empty, out short baseHitPoints))
        {
            return;
        }

        EquipmentIndex attackerWeaponIndex = (EquipmentIndex)collisionData.AffectorWeaponSlotOrMissileIndex;

        int blowDone = collisionData.AbsorbedByArmor + collisionData.InflictedDamage;

        if (attacker!.WieldedWeapon.HitPoints == 1)
        {
            int randomNumber = MBRandom.RandomInt(0, 1000);
            if (randomNumber >= blowDone)
            {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new UpdateWeaponHealth { Agent = attacker, EquipmentIndex = attackerWeaponIndex, WeaponHealth = 1, LastBlow = blowDone, LastRoll = randomNumber });
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
                return;
            }

            int soundIndex = SoundEvent.GetEventIdFromString("event:/mission/combat/shield/broken");
            attacker.RemoveEquippedWeapon(attackerWeaponIndex);
            Mission.Current.MakeSound(soundIndex, attacker.Position, false, true, -1, -1);
            return;
        }

        short newHealth = (short)Math.Max(1, attacker!.WieldedWeapon.HitPoints - blowDone);

        attacker!.ChangeWeaponHitPoints(attackerWeaponIndex, newHealth);

        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage(new UpdateWeaponHealth { Agent = attacker, EquipmentIndex = attackerWeaponIndex, WeaponHealth = newHealth });
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);

        InformationManager.DisplayMessage(new InformationMessage($"{attacker.WieldedWeapon.Item?.Name.ToString() ?? string.Empty} of {attacker.Name.ToString()} has  {attacker.WieldedWeapon.HitPoints} remaining hp"));
    }
}
