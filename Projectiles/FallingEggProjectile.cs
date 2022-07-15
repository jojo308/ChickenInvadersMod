using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Projectiles
{
    class FallingEggProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Egg Projectile");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 22;
            projectile.ranged = true;
            projectile.timeLeft = 300;
            projectile.penetrate = 1;
            projectile.ignoreWater = true;
            projectile.hostile = true;          
            projectile.tileCollide = true;
        }

        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Egg_Splash").WithVolume(2f).WithPitchVariance(.3f), projectile.position);
        }

        public override void AI()
        {
            // rotate the projectile to the direction it was shot to
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
