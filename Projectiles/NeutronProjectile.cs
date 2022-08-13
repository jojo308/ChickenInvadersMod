using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
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
            Projectile.width = 14;
            Projectile.height = 31;
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
            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.NPCHit3.WithVolume(2f).WithPitchVariance(.3f), Projectile.position);
            }
        }

        public override void AI()
        {
            // rotate the projectile to the direction it was shot to
            Vector2 direction = Projectile.position - Projectile.oldPosition;
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            Projectile.rotation = rotation + ((float)Math.PI * 0.5f);
        }
    }
}
