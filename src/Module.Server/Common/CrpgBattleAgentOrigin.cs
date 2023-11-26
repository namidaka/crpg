using Crpg.Module.Api.Models.Items;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common;

internal class CrpgBattleAgentOrigin : BasicBattleAgentOrigin
{
    public CharacterSkills Skills { get; }
    public List<(CrpgItemArmorComponent armor, ItemObject.ItemTypeEnum type)> ArmorItems { get; } = new();

    public CrpgBattleAgentOrigin(BasicCharacterObject? troop, CharacterSkills skills)
        : base(troop)
    {
        Skills = skills;
    }
}
