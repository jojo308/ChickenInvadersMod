﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Items.Banners
{
    public class ChickenBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicken banner");
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
            Item.placeStyle = 1;
        }
    }
}
