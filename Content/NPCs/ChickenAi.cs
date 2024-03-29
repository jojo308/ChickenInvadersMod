﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace ChickenInvadersMod.Content.NPCs
{
    /// <summary>
    /// Utility class for the chicken AI. It contains common actions used by most chickens.
    /// </summary>
    public static class ChickenAi
    {
        /// <summary>
        /// Targets the nearest player and return their location
        /// </summary>
        /// <param name="npc">the npc</param>
        /// <returns>The location of the nearest player</returns>
        public static Vector2 GetTargetPos(this NPC npc)
        {
            npc.TargetClosest(true);
            return npc.HasPlayerTarget ? Main.player[npc.target].Center : Main.npc[npc.target].Center;
        }

        /// <summary>
        /// Shoots a projectile at the player
        /// </summary>
        /// <param name="npc">the npc that shoots the projectile</param>
        /// <param name="position">The position the projectile is shot from</param>
        /// <param name="projectileId">The ID of the projectile</param>
        /// <param name="speed">The speed of the projectile</param>
        /// <param name="damage">the damage of the projectile</param>
        /// <param name="kb">the knockback of the projectile</param>
        public static void ShootAtPlayer(this NPC npc, Vector2 position, int projectileId, float speed, int damage, float kb = 0f)
        {
            // target player
            Vector2 targetPosition = GetTargetPos(npc);
            Vector2 direction = targetPosition - position;
            direction.Normalize();

            // shoot
            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), position, direction * speed, projectileId, damage, kb, Main.myPlayer);
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
        }

        /// <summary>
        /// Shoots a projectile straight down
        /// </summary>
        /// <param name="npc">The npc that shoots the projectile</param>
        public static void ShootDown(this NPC npc, int projectileId, float speed, int damage, float kb = 0f)
        {
            ShootDown(npc, npc.Bottom, projectileId, speed, damage, 0f);
        }

        public static void ShootDown(this NPC npc, Vector2 position, int projectileId, float speed, int damage, float kb = 0f)
        {
            // target down           
            Vector2 target = new Vector2(position.X, position.Y + 1);
            Vector2 dir = target - position;
            dir.Normalize();

            // shoot
            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), position, dir * speed, projectileId, damage, kb, Main.myPlayer);
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
        }

        /// <summary>
        /// Checks collision for the given npc
        /// </summary>
        /// <param name="npc">The NPC to check the collision with</param>
        public static void CheckCollision(this NPC npc)
        {
            npc.velocity = Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height, true, false, 1);
        }
    }
}
