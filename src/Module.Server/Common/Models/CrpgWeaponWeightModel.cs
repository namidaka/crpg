using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace Crpg.Module.Common.Models;
internal class CrpgWeaponWeightModel
{
    private static string _debugName = string.Empty;
    public static float GetCustomWeight(WeaponComponent weaponComponent)
    {
        var meleeWeapons = weaponComponent.Weapons.FindAll(w => w.IsMeleeWeapon);
        if (meleeWeapons.IsEmpty())
        {
            return weaponComponent.Item.Weight;
        }

        _debugName = weaponComponent.Item.Name.ToString();
        return meleeWeapons.Max(CalculateWeight);
    }

    private static float CalculateWeight(WeaponComponentData weapon)
    {
        float weight = GetBaseWeight(weapon);
        weight *= (float)Math.Pow(weapon.WeaponLength / 110f, 0.4f);

        float swingDamageFactor = weapon.SwingDamageFactor > 0
            ? (int)(weapon.SwingDamageFactor * CrpgStrikeMagnitudeModel.BladeDamageFactorToDamageRatio) / GetDamageTypeFactor(weapon.SwingDamageType)
            : 0f;
        float thrustDamageFactor = weapon.ThrustDamageFactor > 0
            ? (int)(weapon.ThrustDamageFactor * CrpgStrikeMagnitudeModel.BladeDamageFactorToDamageRatio) / GetDamageTypeFactor(weapon.ThrustDamageType)
            : 0f;
        weight *= Math.Min(swingDamageFactor, thrustDamageFactor) == 0
            ? Math.Max(swingDamageFactor, thrustDamageFactor)
            : 0.8f * Math.Max(swingDamageFactor, thrustDamageFactor) + 0.2f * Math.Min(swingDamageFactor, thrustDamageFactor);

        //if (_debugName.Contains("Harvesting"))
        //{
        //    Debug.Print($"{_debugName} SD Factor: {swingDamageFactor}");
        //    Debug.Print($"{_debugName} TD Factor: {thrustDamageFactor}");
        //    Debug.Print($"{_debugName} Weight: {weight}");
        //}

        float speedFactor = 80f;
        float swingSpeedFactor = swingDamageFactor > 0 ? weapon.SwingSpeed / speedFactor : 0f;
        float thrustSpeedFactor = thrustDamageFactor > 0 ? weapon.ThrustSpeed / speedFactor : 0f;
        weight *= Math.Min(swingSpeedFactor, thrustSpeedFactor) == 0
            ? Math.Max(swingSpeedFactor, thrustSpeedFactor)
            : 0.8f * Math.Max(swingSpeedFactor, thrustSpeedFactor) + 0.2f * Math.Min(swingSpeedFactor, thrustSpeedFactor);

        //if (_debugName.Contains("Wooden"))
        //{
        //    Debug.Print($"{_debugName} SS Factor: {swingSpeedFactor}");
        //    Debug.Print($"{_debugName} TS Factor: {thrustSpeedFactor}");
        //    Debug.Print($"{_debugName} Weight: {weight}");
        //}

        if (weapon.WeaponFlags.HasAnyFlag(WeaponFlags.BonusAgainstShield))
        {
            weight *= 1.2f;
        }

        if (weapon.WeaponFlags.HasAnyFlag(WeaponFlags.CanCrushThrough))
        {
            weight *= 1.4f;
        }

        if (weapon.WeaponFlags.HasAnyFlag(WeaponFlags.CanKnockDown))
        {
            weight *= 1.2f;
        }

        if (weapon.WeaponFlags.HasAnyFlag(WeaponFlags.MultiplePenetration))
        {
            weight *= 1.1f;
        }

        //if (_debugName.Contains("Wooden"))
        //{
        //    Debug.Print($"{_debugName} End Weight: {weight}");
        //}

        return weight;
    }

    private static float GetBaseWeight(WeaponComponentData weapon)
    {
        return weapon.WeaponClass switch
        {
            WeaponClass.Dagger => 0.5f,
            WeaponClass.OneHandedSword => 1f,
            WeaponClass.TwoHandedSword => 1.5f,
            WeaponClass.OneHandedAxe => 0.8f,
            WeaponClass.TwoHandedAxe => 1.3f,
            WeaponClass.Mace => 1.2f,
            WeaponClass.TwoHandedMace => 1.8f,
            WeaponClass.Pick => 1f,
            WeaponClass.OneHandedPolearm => 1f,
            WeaponClass.TwoHandedPolearm => 1f,
            WeaponClass.LowGripPolearm => 1f,
            _ => 1f,
        };
    }

    private static float GetDamageTypeFactor(DamageTypes damageType)
    {
        return damageType switch
        {
            DamageTypes.Blunt => 15f,
            DamageTypes.Pierce => 20f,
            _ => 35f,
        };
    }
}
