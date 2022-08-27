using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Content.Items.Consumables
{
    public class ChickenTwinLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicken Twin Legs");
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.width = 37;
            Item.height = 31;
            Item.useTime = 32;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.White;
            Item.autoReuse = false;
            Item.maxStack = 30;
            Item.UseSound = SoundID.Item2;
            Item.buffType = BuffID.WellFed;
            Item.buffTime = 18000;
            Item.consumable = true;
        }
    }
}
