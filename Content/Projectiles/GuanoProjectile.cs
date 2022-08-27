using ChickenInvadersMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Content.Projectiles
{
    class GuanoProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guano Projectile");
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 23;
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
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            int frameSpeed = 4; //How fast you want it to animate
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

    }
}
