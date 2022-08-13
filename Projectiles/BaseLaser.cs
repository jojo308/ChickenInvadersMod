using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Projectiles
{
    // This projectile and its subclasses were made using code from the Example Mod https://github.com/tModLoader/tModLoader/blob/1.4/ExampleMod/Old/Projectiles/ExampleLaser.cs

    /// <summary>
    /// Base class for a laser projectile
    /// </summary>
    public abstract class BaseLaser : ModProjectile
    {
        /// <summary>
        /// The NPC that owns this projectile
        /// </summary>      
        public NPC Owner => Main.npc[(int)Projectile.ai[0]];

        /// <summary>
        /// The amount of ticks before updating to the next frame. A higher number means a slower animation
        /// </summary>
        public int frameSpeed = 8;

        /// <summary>
        /// The distance between the laser and the ground
        /// </summary>
        public float Distance = 50f;

        /// <summary>
        /// The maximum distance the laser can travel
        /// </summary>
        public float MaxDistance = 2200f;

        /// <summary>
        /// returns te end point of the laser
        /// </summary>
        /// <param name="npc">The NPC that owns this projectile</param>
        /// <returns></returns>
        public abstract Vector2 GetEndPoint(NPC npc);

        /// <summary>
        /// Draws the laser. Should be called inside the PreDraw() method of the projectile
        /// </summary>       
        /// <param name="spriteBatch">The sprite batch to use</param>
        /// <param name="texture">The texture to use</param>
        /// <param name="start">Where the laser should start</param>
        /// <param name="rotation">the rotation of the laser (radians)</param>
        /// <param name="scale">The scale of the laser</param>
        /// <param name="offset">The offset to draw the laser from</param>
        public abstract void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, float rotation = -1.57f, float scale = 1f, int offset = 0);

        /// <summary>
        /// Updates the velocity of the projectile. Can be used to move or rotate the laser
        /// </summary>
        /// <param name="npc">The NPC that shoots this projectile</param>
        public abstract void UpdateVelocity(NPC npc);

        /// <summary>
        /// Sets the end of the laser position based on where it collides with something
        /// </summary>
        /// <param name="npc">The NPC that shoots this projectile</param>
        public abstract void SetLaserPosition(NPC npc);

        /// <summary>
        /// Casts light for the laser
        /// </summary>
        /// <param name="npc">The NPC that shoots this projectile</param>
        public abstract void CastLights(NPC npc);

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            drawCacheProjsBehindNPCs.Add(index);
            base.DrawBehind(index, drawCacheProjsBehindNPCsAndTiles, drawCacheProjsBehindNPCs, drawCacheProjsBehindProjectiles, drawCacheProjsOverWiresUI);
        }

        public override bool ShouldUpdatePosition() => false;

        /// <summary>
        /// Allows you to modify the frame from this projectiles texture that is drawn
        /// </summary>
        public void FindFrame()
        {
            if (Main.projFrames[Projectile.type] <= 1) return;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void AI()
        {
            NPC npc = Owner;

            // Kill the projectile of the npc is no longer active (alive)
            if (!npc.active) Projectile.Kill();

            FindFrame();

            // The laser calls these 3 methods in order to function properly. It is up to the subclasses to implement them
            UpdateVelocity(npc);
            SetLaserPosition(npc);
            CastLights(npc);
        }
    }
}
