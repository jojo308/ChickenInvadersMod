using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod.NPCs
{
    /// <summary>
    /// Utility class for the chicken AI. It contains common actions used by most chickens.
    /// </summary>
    public static class ChickenAi
    {
        /// <summary>
        /// Shoots a projectile at the player
        /// </summary>
        /// <param name="projectileId">The ID of the projectile</param>
        /// <param name="speed">The speed of the projectile</param>
        /// <param name="damage">the damage of the projectile</param>
        /// <param name="kb">the knockback of the projectile</param>
        public static void Shoot(this ModNPC modNpc, int projectileId, float speed, int damage, float kb = 0f)
        {
            // target player
            Vector2 position = modNpc.npc.Center;
            Vector2 targetPosition = Main.player[modNpc.npc.target].Center;
            Vector2 direction = targetPosition - position;
            direction.Normalize();

            // shoot
            Projectile.NewProjectile(position, direction * speed, projectileId, damage, kb, Main.myPlayer);
        }

        /// <summary>
        /// Shoots a projectile straight down
        /// </summary>
        /// <param name="modNpc"></param>
        public static void ShootDown(this ModNPC modNpc, int projectileId, float speed, int damage, float kb = 0f)
        {
            // target down
            Vector2 pos = modNpc.npc.Bottom;            
            Vector2 target = new Vector2(pos.X, pos.Y + 1);
            Vector2 dir = target - pos;
            dir.Normalize();

            // shoot
            Projectile.NewProjectile(pos, dir * speed, projectileId, damage, kb, Main.myPlayer);
        }
    }
}
