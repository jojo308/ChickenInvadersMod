using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace ChickenInvadersMod.Projectiles
{
    /// <summary>
    /// A laser that shoots in four directions, 90 degrees from each other, and rotates them
    /// </summary>
    public class LaserWarning : BaseLaser
    {
        /// <summary>
        /// The rotation of the laser (in radians)
        /// </summary>       
        public float Rotation
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        /// <summary>
        /// returns te end point of the laser
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public override Vector2 GetEndPoint(NPC npc) => npc.Center + Projectile.velocity * Distance;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Warning");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 2;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.timeLeft = 60;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Owner.Center, -1.57f);
            return false;
        }

        public override void DrawLaser(Texture2D texture, Vector2 start, float rotation = 0f, float scale = 1f, int offset = 0)
        {
            float r = Projectile.velocity.ToRotation() + rotation;
            int sourceFrameY = Projectile.frame * Projectile.height;
            var step = Projectile.height;
            int i;

            // Draw the laser
            for (i = 0; i <= Distance; i += step)
            {
                Color color = Color.White;
                var origin = start + i * Projectile.velocity;

                Main.EntitySpriteDraw(texture, origin - Main.screenPosition, new Rectangle(0, sourceFrameY, Projectile.width, Projectile.height),
                    color, r, new Vector2(Projectile.width * .5f, Projectile.height * .5f), scale, 0, 0);
            }
        }

        public override void UpdateVelocity(NPC npc)
        {
            var position = npc.Center;
            Vector2 target = (position * Distance).RotatedBy(Rotation, position);
            Vector2 diff = target - position;
            diff.Normalize();
            Projectile.velocity = diff;
            Projectile.position += Projectile.velocity;
            Projectile.netUpdate = true;
        }

        public override void SetLaserPosition(NPC npc)
        {
            for (Distance = 0; Distance <= MaxDistance; Distance += 5f)
            {
                if (!Collision.CanHit(npc.Center, 1, 1, GetEndPoint(npc), 1, 1))
                {
                    Distance -= 5f;

                    if (Distance < 0) Distance = 0;

                    break;
                }
            }
        }

        public override void CastLights(NPC npc)
        {
            Utils.PlotTileLine(npc.Center, GetEndPoint(npc), 22, DelegateMethods.CastLight);
        }
    }
}
