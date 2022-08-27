using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Content.Items.Consumables
{
    public class DoubleHamburger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Double Hamburger");
            Tooltip.SetDefault("'Double the toppings, double the fun'");
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.width = 42;
            Item.height = 42;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.maxStack = 30;
            Item.UseSound = SoundID.Item2;
            Item.buffType = BuffID.WellFed;
            Item.buffTime = 36000;
            Item.consumable = true;
        }
    }
}
