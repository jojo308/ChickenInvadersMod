using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace ChickenInvadersMod.Projectiles
{
    class GuanoProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guano Projectile");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 14; //32
            projectile.height = 23; //32
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
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

            int frameSpeed = 4; //How fast you want it to animate
            projectile.frameCounter++;
            if (projectile.frameCounter >= frameSpeed)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }

    }
}
