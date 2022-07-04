using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Items
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
            item.noMelee = true;
            item.width = 42;
            item.height = 42;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.value = Item.sellPrice(gold: 1);
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
