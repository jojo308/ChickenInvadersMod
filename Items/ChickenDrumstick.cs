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
            item.noMelee = true;
            item.width = 25;
            item.height = 40;
            item.useTime = 32;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.value = Item.sellPrice(copper: 50);
            item.rare = ItemRarityID.White;
            item.autoReuse = false;
            item.maxStack = 99;
            item.UseSound = SoundID.Item2;
            item.buffType = BuffID.WellFed;
            item.buffTime = 7200;
            item.consumable = true;
        }
    }
}
