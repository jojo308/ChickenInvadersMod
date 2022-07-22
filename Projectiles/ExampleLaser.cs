
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Projectiles
{
    public class ExampleLaser : ModProjectile
    {       
        public int Owner
        {
            get => (int)projectile.ai[0];
        }

        public float Distance
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.timeLeft = 300;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var npc = Main.npc[Owner];
            var start = Vector2.Transform(npc.Bottom - npc.Center, Matrix.CreateRotationZ(npc.rotation)) + npc.Center;
            DrawLaser(spriteBatch, Main.projectileTexture[projectile.type], start, projectile.velocity, 10, -1.57f, 1f);
            return false;
        }

        // Draws the laser   
        public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, float rotation = 0f, float scale = 1f, int transDist = 0)
        {
            float r = unit.ToRotation() + rotation;

            // Draws the laser 'body'
            for (float i = transDist; i <= Distance; i += step)
            {
                Color c = Color.White;
                var origin = start + i * unit;
                spriteBatch.Draw(texture, origin - Main.screenPosition, new Rectangle(0, 26, 28, 26), i < transDist ? Color.Transparent : c, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
            }

            // Draws the laser 'tail'
            spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition, new Rectangle(0, 0, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);

            // Draws the laser 'head'
            spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition, new Rectangle(0, 52, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            NPC npc = Main.npc[Owner];
            Vector2 unit = projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, it will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), npc.Bottom, npc.Bottom + unit * Distance, 22, ref point);
        }

        public override void AI()
        {
            NPC npc = Main.npc[Owner];

            // Kill the projectile of the npc is no longer active (alive)
            if (!npc.active)
            {
                projectile.Kill();
            }
            UpdateNPC(npc);
            SetLaserPosition(npc);
            CastLights();
            projectile.timeLeft--;
        }

        /*
		* Sets the end of the laser position based on where it collides with something
		*/
        private void SetLaserPosition(NPC npc)
        {
            for (Distance = 0; Distance <= 2200f; Distance += 5f)
            {
                var start = npc.Bottom + projectile.velocity * Distance;
                if (!Collision.CanHit(npc.Bottom, 1, 1, start, 1, 1))
                {
                    Distance -= 5f;
                    break;
                }
            }
        }

        private void UpdateNPC(NPC npc)
        {
            var position = npc.Bottom;
            Vector2 target = new Vector2(position.X, position.Y + 1);
            Vector2 diff = target - position;
            diff.Normalize();
            projectile.velocity = diff;
            projectile.netUpdate = true;
        }

        private void CastLights()
        {
            NPC npc = Main.npc[Owner];
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(npc.Bottom, npc.Bottom + unit * Distance, 22, DelegateMethods.CastLight);
        }

        public override bool ShouldUpdatePosition() => false;

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(projectile.Bottom, projectile.Bottom + unit * Distance, (projectile.width + 16) * projectile.scale, DelegateMethods.CutTiles);
        }
    }
}
