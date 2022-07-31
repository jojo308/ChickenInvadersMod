using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace ChickenInvadersMod.Projectiles
{
    /// <summary>
    /// A laser that shoots in four directions, 90 degrees from each other, and rotates them
    /// </summary>
    public class LaserWarning : ModProjectile
    {
        /// <summary>
        /// The NPC that owns this projectile
        /// </summary>      
        public NPC Owner
        {
            get => Main.npc[(int)projectile.ai[0]];
        }

        /// <summary>
        /// The rotation of the laser (in radians)
        /// </summary>       
        public float Rotation
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        /// <summary>
        /// The distance between the laser and the ground
        /// </summary>
        private float Distance = 50f;

        /// <summary>
        /// returns te end point of the laser
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        private Vector2 GetEndPoint(NPC npc) => npc.Center + projectile.velocity * Distance;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Warning");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hide = true;
            projectile.magic = true;
            projectile.timeLeft = 60;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCs.Add(index);
            base.DrawBehind(index, drawCacheProjsBehindNPCsAndTiles, drawCacheProjsBehindNPCs, drawCacheProjsBehindProjectiles, drawCacheProjsOverWiresUI);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawLaser(spriteBatch, Main.projectileTexture[projectile.type], Owner.Center, -1.57f);
            return false;
        }

        /// <summary>
        /// Draws the laser
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use</param>
        /// <param name="texture">The texture to use</param>
        /// <param name="start">Where the laser should start</param>
        /// <param name="rotation">the rotation of the laser</param>
        public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, float rotation = 0f)
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
                    color, r, new Vector2(projectile.width * .5f, projectile.height * .5f), 1f, 0, 0);
            }
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
            MoveLaser(npc);
            SetLaserPosition(npc);
            CastLights(npc);
        }

        /// <summary>
        /// Updates the rotation of the projectile
        /// </summary>
        /// <param name="npc">The NPC that shoots this projectile</param>
        private void MoveLaser(NPC npc)
        {
            var position = npc.Center;
            Vector2 target = (position * Distance).RotatedBy(Rotation, position);
            Vector2 diff = target - position;
            diff.Normalize();
            projectile.velocity = diff;
            projectile.position += projectile.velocity;
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
                if (!Collision.CanHit(npc.Center, 1, 1, GetEndPoint(npc), 1, 1))
                {
                    Distance -= 5f;

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
            Utils.PlotTileLine(npc.Center, GetEndPoint(npc), 22, DelegateMethods.CastLight);
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
