using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace ChickenInvadersMod.Projectiles
{
    // Projectile was mode using code from the Example Mod https://github.com/tModLoader/tModLoader/blob/1.4/ExampleMod/Old/Projectiles/ExampleLaser.cs

    /// <summary>
    /// A laser that shoots straight down for a few seconds
    /// </summary>
    public class LaserBeam : ModProjectile
    {
        /// <summary>
        /// The NPC that owns this projectile
        /// </summary>      
        public NPC Owner
        {
            get => Main.npc[(int)projectile.ai[0]];
        }

        /// <summary>
        /// The distance between the laser and the ground
        /// </summary>
        public float Distance
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Beam");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 24;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hide = true;
            projectile.magic = true;
            projectile.timeLeft = 450;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCs.Add(index);
            base.DrawBehind(index, drawCacheProjsBehindNPCsAndTiles, drawCacheProjsBehindNPCs, drawCacheProjsBehindProjectiles, drawCacheProjsOverWiresUI);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var npc = Owner;
            var start = Vector2.Transform(npc.Bottom - npc.Center, Matrix.CreateRotationZ(npc.rotation)) + npc.Center;
            DrawLaser(spriteBatch, Main.projectileTexture[projectile.type], start, -1.57f, 1f, 0);
            return false;
        }

        /// <summary>
        /// Draws the laser
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use</param>
        /// <param name="texture">The texture to use</param>
        /// <param name="start">Where the laser should start</param>
        /// <param name="rotation">the rotation of the laser</param>
        /// <param name="scale">The scale of the laser</param>
        /// <param name="transDist">???</param>
        public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, float rotation = 0f, float scale = 1f, int transDist = 50)
        {
            float r = projectile.velocity.ToRotation() + rotation;
            int sourceFrameY = projectile.frame * projectile.height;
            var step = projectile.height;
            int i;

            // Draws the laser
            for (i = transDist; i <= Distance; i += step)
            {
                Color color = i < transDist ? Color.Transparent : Color.White;
                var origin = start + i * projectile.velocity;

                spriteBatch.Draw(texture, origin - Main.screenPosition, new Rectangle(0, sourceFrameY, projectile.width, projectile.height),
                    color, r, new Vector2(projectile.width * .5f, projectile.height * .5f), scale, 0, 0);
            }

            // get the remaning distance between the last drawn laser sprite and the ground
            float remaining = i >= Distance ? i - Distance : Distance - i;

            // Draw the last part which has the same height as the remaining distance
            spriteBatch.Draw(texture, start + (Distance + remaining) * projectile.velocity - Main.screenPosition, new Rectangle(0, sourceFrameY, projectile.width, projectile.height - (int)remaining),
                    Color.White, r, new Vector2(projectile.width * .5f, projectile.height * .5f), scale, 0, 1);

            // cuts tiles. This code belongs in the CutTiles() method, but that method doesn't get called for some reason. So it is placed here...
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(start, start + (Distance + remaining) * projectile.velocity, (projectile.width + 16) * projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            NPC npc = Owner;
            Vector2 unit = projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), npc.Center,
                npc.Bottom + unit * Distance, projectile.width, ref point);
        }

        private void FindFrame()
        {
            int frameSpeed = 8;

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

        public override void AI()
        {
            NPC npc = Owner;

            // Kill the projectile of the npc is no longer active (alive)
            if (!npc.active) projectile.Kill();

            FindFrame();
            UpdateVelocity(npc);
            SetLaserPosition(npc);
            CastLights(npc);
            projectile.position += projectile.velocity;
            projectile.timeLeft--;
        }

        /// <summary>
        /// Updates the velocity of the projectile
        /// </summary>
        /// <param name="npc">The NPC that shoots this projectile</param>
        private void UpdateVelocity(NPC npc)
        {
            var position = npc.Bottom;
            Vector2 target = new Vector2(position.X, position.Y + 1);
            Vector2 diff = target - position;
            diff.Normalize();
            projectile.velocity = diff;
            projectile.netUpdate = true;
        }

        /// <summary>
        /// Sets the end of the laser position based on where it collides with something
        /// </summary>
        /// <param name="npc">The NPC that shoots this projectile</param>
        private void SetLaserPosition(NPC npc)
        {
            for (Distance = 0; Distance <= 2200f; Distance += 5f)
            {
                var start = npc.Bottom + projectile.velocity * Distance;
                if (!Collision.CanHit(npc.Bottom, 1, 1, start, 1, 1))
                {
                    Distance -= 10f;

                    if (Distance < 0) Distance = 0;

                    break;
                }
            }
        }

        /// <summary>
        /// Casts light for the laser
        /// </summary>
        /// <param name="npc">The NPC that shoots this projectile</param>
        private void CastLights(NPC npc)
        {
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(npc.Bottom, npc.Bottom + unit * Distance, 22, DelegateMethods.CastLight);
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanCutTiles() => true;

        // this method doesn't get called
        //public override void CutTiles()
        //{
        //    var npc = Owner;
        //    var start = Vector2.Transform(npc.Bottom - npc.Center, Matrix.CreateRotationZ(npc.rotation)) + npc.Center;
        //    var remaining = 0;

        //    for (var i = 0; i <= Distance; i += projectile.height)
        //    {
        //        remaining = i;
        //    }

        //    DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
        //    Utils.PlotTileLine(start, start + (Distance + remaining) * projectile.velocity, (projectile.width + 16) * projectile.scale, DelegateMethods.CutTiles);
        //}
    }
}
