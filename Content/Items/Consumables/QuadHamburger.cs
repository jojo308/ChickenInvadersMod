using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Content.Items.Consumables
{
    public class QuadHamburger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Quad Hamburger");
            Tooltip.SetDefault("'Can you even eat this in one bite? yes'");
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.width = 42;
            Item.height = 42;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.maxStack = 30;
            Item.UseSound = SoundID.Item2;
            Item.buffType = BuffID.WellFed;
            Item.buffTime = 43200;
            Item.consumable = true;
        }
    }
}
