using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Projectiles
{
    public class EggProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Egg Projectile");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.ranged = true;
            projectile.timeLeft = 300;            
            projectile.penetrate = 1;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.tileCollide = true;
        }

        public override void Kill(int timeLeft)
        {            
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Egg_Splash").WithVolume(2f).WithPitchVariance(.3f), projectile.position);            
        }
    }
}
