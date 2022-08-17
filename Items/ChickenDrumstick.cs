using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Items
{
    public class ChickenDrumstick : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicken Drumstick");
            Tooltip.SetDefault("\"It's finger Lickin' Good\"");
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.width = 25;
            Item.height = 40;
            Item.useTime = 32;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.value = Item.sellPrice(copper: 50);
            Item.rare = ItemRarityID.White;
            Item.autoReuse = false;
            Item.maxStack = 30;
            Item.UseSound = SoundID.Item2;
            Item.buffType = BuffID.WellFed;
            Item.buffTime = 7200;
            Item.consumable = true;
        }
    }
}
