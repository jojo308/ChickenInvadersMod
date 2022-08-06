using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Items
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
            item.noMelee = true;
            item.width = 42;
            item.height = 42;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.value = Item.sellPrice(gold: 2, silver: 50);
            item.rare = ItemRarityID.Green;
            item.autoReuse = false;
            item.maxStack = 99;
            item.UseSound = SoundID.Item2;
            item.buffType = BuffID.WellFed;
            item.buffTime = 43200;
            item.consumable = true;
        }
    }
}
