using Terraria;
using Terraria.Audio;
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
            Item.damage = 15;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = 10;
            Item.rare = ItemRarityID.White;
            Item.autoReuse = false;
            Item.shoot = Mod.Find<ModProjectile>("EggProjectile").Type;
            Item.shootSpeed = 8f;
            Item.useTurn = true;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.noUseGraphic = true;
        }

        public override void OnConsumeItem(Player player)
        {
            SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Egg_Drop").WithVolume(.5f).WithPitchVariance(.3f));
            base.OnConsumeItem(player);
        }
    }
}
