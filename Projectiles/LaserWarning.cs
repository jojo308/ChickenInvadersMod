using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

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
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        /// <summary>
        /// returns te end point of the laser
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public override Vector2 GetEndPoint(NPC npc) => npc.Center + projectile.velocity * Distance;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Warning");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.width = 2;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.timeLeft = 60;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawLaser(spriteBatch, Main.projectileTexture[projectile.type], Owner.Center, -1.57f);
            return false;
        }

        public override void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, float rotation = 0f, float scale = 1f, int offset = 0)
        {
            float r = projectile.velocity.ToRotation() + rotation;
            int sourceFrameY = projectile.frame * projectile.height;
            var step = projectile.height;
            int i;

            // Draw the laser
            for (i = 0; i <= Distance; i += step)
            {
                Color color = Color.White;
                var origin = start + i * projectile.velocity;

                spriteBatch.Draw(texture, origin - Main.screenPosition, new Rectangle(0, sourceFrameY, projectile.width, projectile.height),
                    color, r, new Vector2(projectile.width * .5f, projectile.height * .5f), scale, 0, 0);
            }
        }

        public override void UpdateVelocity(NPC npc)
        {
            var position = npc.Center;
            Vector2 target = (position * Distance).RotatedBy(Rotation, position);
            Vector2 diff = target - position;
            diff.Normalize();
            projectile.velocity = diff;
            projectile.position += projectile.velocity;
            projectile.netUpdate = true;
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
