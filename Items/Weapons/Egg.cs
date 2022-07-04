using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Items.Weapons
{
    class Egg : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Egg");
        }

        public override void SetDefaults()
        {
            item.damage = 15;
            item.noMelee = true;
            item.thrown = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;
            item.value = 10;
            item.rare = ItemRarityID.White;           
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("EggProjectile");
            item.shootSpeed = 8f;
            item.useTurn = true;
            item.maxStack = 999;            
            item.consumable = true;
            item.noUseGraphic = true;
        }

        public override void OnConsumeItem(Player player)
        {
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Egg_Drop").WithVolume(.5f).WithPitchVariance(.3f));
            base.OnConsumeItem(player);
        }              
    }
}
