using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.NPCs
{
    /// <summary>
    /// A flying barrier that will fly under chickens to shield them
    /// </summary>
    class Barrier : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barrier");
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[3];
        }

        /// <summary>
        /// The target to shield
        /// </summary>
        public float Target
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
        }

        /// <summary>
        /// Time without a target
        /// </summary>
        private float NoTarget
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        /// <summary>
        /// Checks if the given NPC is a valid target
        /// </summary>
        /// <param name="target">The NPC to be targeted</param>
        /// <returns>True if the NPC is a valid target, false otherwise</returns>
        private bool IsValidTarget(NPC target) =>
            target.type != npc.type
            && target.active
            && target.life > 0
            && target.type != ModContent.NPCType<SuperChicken>()
            && CIWorld.Enemies.ContainsKey(target.type);

        /// <summary>
        /// Returns whether or not the specified NPC can be shielded
        /// </summary>
        /// <param name="other">The NPC to shield</param>
        /// <returns>True if the NPC can be shielded, false otherwise</returns>
        private bool IsShielded(NPC other)
        {
            if (other.modNPC is BaseChicken npc)
            {
                return !npc.IsProtected;
            }

            return false;
        }

        /// <summary>
        /// Shields the specified target
        /// </summary>
        /// <param name="target">The target to be shielded</param>
        private void ShieldTarget(NPC target)
        {
            if (target.modNPC is BaseChicken npc && !npc.IsProtected)
            {
                npc.IsProtected = true;
            }
        }

        /// <summary>
        /// Unshields the specified target
        /// </summary>
        /// <param name="target">The target to be unshielded</param>
        private void UnshieldTarget(NPC target)
        {
            if (target.modNPC is BaseChicken npc && npc.IsProtected)
            {
                npc.IsProtected = false;
            }
        }

        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 26;
            npc.damage = 10;
            npc.defense = 25;
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

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(npc.Center, 32, 32, DustID.Smoke, npc.velocity.X, npc.velocity.Y);
                }

                Main.PlaySound(SoundID.NPCDeath14, npc.position);
            }
            base.HitEffect(hitDirection, damage);
        }

        public override void AI()
        {
            // if the Barrier cannot find an NPC to shield in time, it will explode. Otherwise there will be too much barriers on screen instead of chickens
            if (NoTarget >= 300)
            {
                npc.life = 0;
                npc.active = false;
                npc.HitEffect();
                return;
            }

            // find target
            NPC current = FindTarget();

            // move under target
            if (current != null)
            {
                if (npc.life <= 0)
                {
                    UnshieldTarget(current);
                }
                else
                {
                    ShieldTarget(current);
                }

                var targetPosition = current.Bottom;
                targetPosition.Y += npc.height;

                var velocity = .22f;

                if (targetPosition.X < npc.Center.X && npc.velocity.X > -3)
                {
                    npc.velocity.X -= velocity;
                }
                else if (targetPosition.X > npc.Center.X && npc.velocity.X < 3)
                {
                    npc.velocity.X += velocity;
                }

                if (targetPosition.Y < npc.position.Y && npc.velocity.Y > -3)
                {
                    npc.velocity.Y -= velocity;
                }
                else if (targetPosition.Y > npc.position.Y && npc.velocity.Y < 3)
                {
                    npc.velocity.Y += velocity;
                    npc.velocity.Y *= 0.9f;
                }

                npc.position += npc.velocity;
            }
            else
            {
                // if there is no target
                npc.velocity.Y = 0f;
                Target = -1f;
                NoTarget++;
            }
        }

        /// <summary>
        /// Finds a NPC to shield
        /// </summary>
        /// <returns>The NPC to shield</returns>
        public NPC FindTarget()
        {
            // if there already is a target
            if (Target > 0)
            {
                var current = Main.npc[(int)Target];

                // check if the target is stil lalive
                if (current.life <= 0)
                {
                    Target = -1;
                    return null;
                }

                return current;
            }
            float closest = float.MaxValue;
            NPC target = null;

            // if there is no target, loop through all NPCs and find the closest
            for (int i = 0; i < 200; i++)
            {
                var other = Main.npc[i];

                // Check if NPC can be shielded
                if (IsValidTarget(other) && IsShielded(other))
                {
                    var dist = Vector2.Distance(npc.Center, other.Center);
                    if (dist < closest)
                    {
                        closest = dist;
                        target = other;
                    }
                }
            }

            Target = target == null ? -1f : target.whoAmI;
            return target;
        }
    }
}
