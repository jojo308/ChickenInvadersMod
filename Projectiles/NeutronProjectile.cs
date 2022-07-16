using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Projectiles
{
    public class NeutronProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Neutron Projectile");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 31;
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
            Main.PlaySound(SoundID.NPCHit3.WithVolume(2f).WithPitchVariance(.3f), projectile.position);
        }

        public override void AI()
        {
            // rotate the projectile to the direction it was shot to
            Vector2 direction = projectile.position - projectile.oldPosition;
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            projectile.rotation = rotation + ((float)Math.PI * 0.5f);            
        }
    }
}
