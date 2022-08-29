using ChickenInvadersMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Content.Projectiles
{
    class FallingEggProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Egg Projectile");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
        }

        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundUtils.EggSplash, Projectile.position);
        }

        public override void AI()
        {
            // rotate the projectile to the direction it was shot to
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
