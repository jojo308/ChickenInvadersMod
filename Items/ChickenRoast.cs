using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Items
{
    public class ChickenRoast : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicken Roast");
        }

        public override void SetDefaults()
        {
            item.noMelee = true;
            item.width = 42;
            item.height = 42;
            item.useTime = 32;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.value = Item.sellPrice(silver: 10);
            item.rare = ItemRarityID.Green;
            item.autoReuse = false;
            item.maxStack = 99;
            item.UseSound = SoundID.Item2;
            item.buffType = BuffID.WellFed;
            item.buffTime = 36000;
            item.consumable = true;
        }
    }
}
