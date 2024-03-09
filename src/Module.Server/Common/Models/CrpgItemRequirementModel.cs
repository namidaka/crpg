using TaleWorlds.Core;

namespace Crpg.Module.Common.Models;

internal class CrpgItemRequirementModel
{
    private readonly CrpgConstants _constants;

    public CrpgItemRequirementModel(CrpgConstants constants)
    {
        _constants = constants;
    }

    public static int ComputeItemRequirement(ItemObject item)
    {
        switch (item.ItemType)
        {
            case ItemObject.ItemTypeEnum.Crossbow:
                return ComputeCrossbowRequirement(item);
            case ItemObject.ItemTypeEnum.Bow:
                return ComputeBowRequirement(item);
            case ItemObject.ItemTypeEnum.Shield:
                return ComputeShieldRequirement(item);
            case ItemObject.ItemTypeEnum.OneHandedWeapon:
                return ComputeOneHandedWeaponRequirement(item);
            case ItemObject.ItemTypeEnum.TwoHandedWeapon:
                return ComputeTwoHandedWeaponRequirement(item);
            case ItemObject.ItemTypeEnum.Polearm:
                return ComputePolearmRequirement(item);
        }

        return 0;
    }

    private static int ComputeCrossbowRequirement(ItemObject item)
    {
        int strengthRequirementForTierTenCrossbow = 24; // Tiers are calulated in CrpgValueModel. 0<Tier=<10 . By design the best is always at Ten.
        if (item.ItemType != ItemObject.ItemTypeEnum.Crossbow)
        {
            throw new ArgumentException(item.Name.ToString() + " is not a crossbow");
        }

        return ((int)(item.Tierf * (strengthRequirementForTierTenCrossbow / 10f)) / 3) * 3;
    }
    private static int ComputeBowRequirement(ItemObject item)
    {
        int strengthRequirementForTierTenBow = 21; // Tiers are calulated in CrpgValueModel. 0<Tier=<10 . By design the best is always at Ten.
        if (item.ItemType != ItemObject.ItemTypeEnum.Bow)
        {
            throw new ArgumentException(item.Name.ToString() + " is not a bow");
        }

        return ((int)(item.Tierf * (strengthRequirementForTierTenBow / 10f)) / 3) * 3;
    }
    private static int ComputeShieldRequirement(ItemObject item)
    {
        int strengthRequirementForTierTenShield = 21; // Tiers are calulated in CrpgValueModel. 0<Tier=<10 . By design the best is always at Ten.
        if (item.ItemType != ItemObject.ItemTypeEnum.Shield)
        {
            throw new ArgumentException(item.Name.ToString() + " is not a shield");
        }

        return ((int)(item.Tierf * (strengthRequirementForTierTenShield / 10f)) / 3) * 3;
    }
    private static int ComputeOneHandedWeaponRequirement(ItemObject item)
    {
        int strengthRequirementForTierTenOneHandedWeapon = 21; // Tiers are calulated in CrpgValueModel. 0<Tier=<10 . By design the best is always at Ten.
        if (item.ItemType != ItemObject.ItemTypeEnum.OneHandedWeapon)
        {
            throw new ArgumentException(item.Name.ToString() + " is not a 1h");
        }

        return ((int)(item.Tierf * (strengthRequirementForTierTenOneHandedWeapon / 10f)) / 3) * 3;
    }
    private static int ComputeTwoHandedWeaponRequirement(ItemObject item)
    {
        int strengthRequirementForTierTenTwoHandedWeapon = 21; // Tiers are calulated in CrpgValueModel. 0<Tier=<10 . By design the best is always at Ten.
        if (item.ItemType != ItemObject.ItemTypeEnum.TwoHandedWeapon)
        {
            throw new ArgumentException(item.Name.ToString() + " is not a 2h");
        }

        return ((int)(item.Tierf * (strengthRequirementForTierTenTwoHandedWeapon / 10f)) / 3) * 3;
    }
    private static int ComputePolearmRequirement(ItemObject item)
    {
        int strengthRequirementForTierTenPolearm = 24; // Tiers are calulated in CrpgValueModel. 0<Tier=<10 . By design the best is always at Ten.
        if (item.ItemType != ItemObject.ItemTypeEnum.Polearm)
        {
            throw new ArgumentException(item.Name.ToString() + " is not a pole");
        }

        return ((int)(item.Tierf * (strengthRequirementForTierTenPolearm / 10f)) / 3) * 3;
    }
}
