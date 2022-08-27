using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Content.Items.Banners
{
    public class ChickGatlingGunBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chick Gatling Gun banner");
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 24;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 0, 10, 0);
            Item.createTile = Mod.Find<ModTile>("Banners").Type;
            Item.placeStyle = 5;
        }
    }
}
