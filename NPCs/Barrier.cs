using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.NPCs
{
    class Barrier : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barrier");
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[3];
        }

        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 26;
            npc.damage = 10;
            npc.defense = 20;
            npc.aiStyle = 0;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.lifeMax = 400;
            npc.value = 100f;
            npc.friendly = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.life > npc.lifeMax * 0.66)
            {
                npc.frameCounter = 0;
            }
            else if (npc.life <= npc.lifeMax * 0.66 && npc.life >= npc.lifeMax * 0.33)
            {
                npc.frameCounter = 1;
            }
            else if (npc.life < npc.lifeMax * 0.33)
            {
                npc.frameCounter = 2;
            }
            npc.frame.Y = (int)npc.frameCounter * frameHeight;
        }

        public override void AI()
        {
            NPC current;

            // only try to find a target if npc doesn't already have one
            if (npc.ai[0] > 0)
            {
                current = Main.npc[(int)npc.ai[0]];
            }
            else
            {
                current = FindTarget();
            }

            // move under target
            if (current != null)
            {
                npc.ai[0] = current.whoAmI;
                var targetPosition = current.Bottom;
                targetPosition.Y += npc.height;

                var velocityX = .22f;

                if (targetPosition.X < npc.Center.X && npc.velocity.X > -3)
                {
                    npc.velocity.X -= velocityX;
                }
                else if (targetPosition.X > npc.Center.X && npc.velocity.X < 3)
                {
                    npc.velocity.X += velocityX;
                }

                if (targetPosition.Y < npc.position.Y && npc.velocity.Y > -3)
                {
                    npc.velocity.Y -= velocityX;
                }
                else if (targetPosition.Y > npc.position.Y && npc.velocity.Y < 3)
                {
                    npc.velocity.Y += velocityX;
                    npc.velocity.Y *= 0.9f;
                }
            }
            else
            {
                npc.velocity.Y = 0;
                npc.ai[0] = 0;
            }
            npc.position += npc.velocity;
        }

        /// <summary>
        /// Finds a NPC to protect
        /// </summary>
        /// <returns>The NPC to protect</returns>
        public NPC FindTarget()
        {
            float closest = float.MaxValue;
            NPC target = null;

            // loop through all NPCs
            for (int i = 0; i < 200; i++)
            {
                var other = Main.npc[i];

                // Check if npc is chicken
                if (other.type != npc.type && other.whoAmI != npc.whoAmI && CIWorld.Enemies.Contains(other.type))
                {
                    var dist = Vector2.Distance(npc.Center, other.Center);
                    if (dist < closest)
                    {
                        closest = dist;
                        target = other;
                    }
                }
            }
            return target;
        }
    }
}
