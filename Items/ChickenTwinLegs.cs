using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Items
{
    public class ChickenTwinLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicken Twin Legs");
        }

        public override void SetDefaults()
        {
            item.noMelee = true;
            item.width = 37;
            item.height = 31;
            item.useTime = 32;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.value = Item.sellPrice(silver: 1);
            item.rare = ItemRarityID.White;
            item.autoReuse = false;
            item.maxStack = 99;
            item.UseSound = SoundID.Item2;
            item.buffType = BuffID.WellFed;
            item.buffTime = 18000;
            item.consumable = true;
        }
    }
}
