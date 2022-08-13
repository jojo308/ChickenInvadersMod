using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;

namespace ChickenInvadersMod.Projectiles
{
    /// <summary>
    /// A laser that shoots in four directions, 90 degrees from each other, and rotates them
    /// </summary>
    public class QuadrupleLaser : BaseLaser
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
        /// Whether or not the projectile is rotating clockwise
        /// </summary>
        public bool Clockwise => Projectile.localAI[0] == 1;

        public override Vector2 GetEndPoint(NPC npc) => npc.Center + Projectile.velocity * Distance;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Quadruple Laser");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 34;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.timeLeft = 240;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawLaser(spriteBatch, TextureAssets.Projectile[Projectile.type].Value, Owner.Center, -1.57f);
            return false;
        }

        public override void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, float rotation = 0f, float scale = 1f, int offset = 0)
        {
            float r = Projectile.velocity.ToRotation() + rotation;
            int sourceFrameY = Projectile.frame * Projectile.height;
            var step = Projectile.height;
            int i;

            // Draw the laser
            for (i = 64; i <= Distance; i += step)
            {
                Color color = Color.White;
                var origin = start + i * Projectile.velocity;

                spriteBatch.Draw(texture, origin - Main.screenPosition, new Rectangle(0, sourceFrameY, Projectile.width, Projectile.height),
                    color, r, new Vector2(Projectile.width * .5f, Projectile.height * .5f), 1f, 0, 0);
            }

            // get the remaning distance between the last drawn laser sprite and the ground          
            float remaining = i - Distance;

            // Draw the last part which has the same height as the remaining distance
            spriteBatch.Draw(texture, start + (Distance - remaining * 1.25f) * Projectile.velocity - Main.screenPosition, new Rectangle(0, sourceFrameY, Projectile.width, Projectile.height - (int)remaining),
                  Color.White, r, new Vector2(Projectile.width * .5f, Projectile.height * .5f), 1f, 0, 0);

            // cuts tiles. This code belongs in the CutTiles() method, but that method doesn't get called for some reason. So it is placed here...
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(start, start + (Distance + remaining) * Projectile.velocity, (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? CanCutTiles() => true;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            NPC npc = Owner;
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), npc.Center, GetEndPoint(npc), Projectile.width, ref point);
        }

        public override void UpdateVelocity(NPC npc)
        {
            var position = npc.Center;
            var degrees = Main.expertMode ? 1f : 0.8f;
            var radians = Clockwise ? Rotation += MathHelper.ToRadians(degrees) : Rotation -= MathHelper.ToRadians(degrees);
            Vector2 target = (position * Distance).RotatedBy(radians, position);
            Vector2 diff = target - position;
            diff.Normalize();
            Projectile.velocity = diff;
            Projectile.position += Projectile.velocity;
        }

        public override void SetLaserPosition(NPC npc)
        {
            for (Distance = 0; Distance <= MaxDistance; Distance += 5f)
            {
                var end = GetEndPoint(npc);
                end = new Vector2(end.X, end.Y - 1);
                if (!Collision.CanHit(npc.Center, 1, 1, end, 1, 1))
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
