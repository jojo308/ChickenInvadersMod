using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;

namespace ChickenInvadersMod.Projectiles
{
    /// <summary>
    /// A laser that shoots straight down for a few seconds
    /// </summary>
    public class LaserBeam : BaseLaser
    {
        public override Vector2 GetEndPoint(NPC npc) => npc.Bottom + Projectile.velocity * Distance;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Beam");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 46;
            Projectile.height = 24;
            Projectile.hostile = true;
            Projectile.timeLeft = 240;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var npc = Owner;
            var start = Vector2.Transform(npc.Bottom - npc.Center, Matrix.CreateRotationZ(npc.rotation)) + npc.Center;
            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, start, -1.57f);
            return false;
        }

        public override void DrawLaser(Texture2D texture, Vector2 start, float rotation = 0f, float scale = 1f, int offset = 0)
        {
            float r = Projectile.velocity.ToRotation() + rotation;
            int sourceFrameY = Projectile.frame * Projectile.height;
            var step = Projectile.height;
            int i;

            // Draw the laser
            for (i = offset; i <= Distance; i += step)
            {
                Color color = i < offset ? Color.Transparent : Color.White;
                var origin = start + i * Projectile.velocity;

                Main.EntitySpriteDraw(texture, origin - Main.screenPosition, new Rectangle(0, sourceFrameY, Projectile.width, Projectile.height),
                    color, r, new Vector2(Projectile.width * .5f, Projectile.height * .5f), scale, 0, 0);
            }

            // get the remaning distance between the last drawn laser sprite and the ground
            float remaining = i >= Distance ? i - Distance : Distance - i;

            // Draw the last part which has the same height as the remaining distance
            Main.EntitySpriteDraw(texture, start + (Distance + remaining) * Projectile.velocity - Main.screenPosition, new Rectangle(0, sourceFrameY, Projectile.width, Projectile.height - (int)remaining),
                    Color.White, r, new Vector2(Projectile.width * .5f, Projectile.height * .5f), scale, 0, 1);

            // cuts tiles. This code belongs in the CutTiles() method, but that method doesn't get called for some reason. So it is placed here...
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(start, start + (Distance + remaining) * Projectile.velocity, (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? CanCutTiles() => true;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            NPC npc = Owner;
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), npc.Bottom, GetEndPoint(npc), Projectile.width, ref point);
        }

        public override void UpdateVelocity(NPC npc)
        {
            var position = npc.Bottom;
            Vector2 target = new Vector2(position.X, position.Y + 1);
            Vector2 diff = target - position;
            diff.Normalize();
            Projectile.velocity = diff;
            Projectile.position += Projectile.velocity;
        }

        public override void SetLaserPosition(NPC npc)
        {
            for (Distance = 0; Distance <= MaxDistance; Distance += 5f)
            {
                var start = new Vector2(npc.Bottom.X, npc.Bottom.Y - 1);
                if (!Collision.CanHit(start, 1, 1, GetEndPoint(npc), 1, 1))
                {
                    Distance -= 5f;
                    break;
                }
            }
        }

        public override void CastLights(NPC npc)
        {
            Utils.PlotTileLine(npc.Bottom, GetEndPoint(npc), 22, DelegateMethods.CastLight);
        }
    }
}
