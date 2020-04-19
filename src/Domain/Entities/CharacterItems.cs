using System.Collections.Generic;

namespace Crpg.Domain.Entities
{
    public class CharacterItems
    {
        public int? HeadItemId { get; set; }
        public int? CapeItemId { get; set; }
        public int? BodyItemId { get; set; }
        public int? HandItemId { get; set; }
        public int? LegItemId { get; set; }
        public int? HorseHarnessItemId { get; set; }
        public int? HorseItemId { get; set; }
        public int? Weapon1ItemId { get; set; }
        public int? Weapon2ItemId { get; set; }
        public int? Weapon3ItemId { get; set; }
        public int? Weapon4ItemId { get; set; }

        public Item? HeadItem { get; set; }
        public Item? CapeItem { get; set; }
        public Item? BodyItem { get; set; }
        public Item? HandItem { get; set; }
        public Item? LegItem { get; set; }
        public Item? HorseHarnessItem { get; set; }
        public Item? HorseItem { get; set; }
        public Item? Weapon1Item { get; set; }
        public Item? Weapon2Item { get; set; }
        public Item? Weapon3Item { get; set; }
        public Item? Weapon4Item { get; set; }
    }
}