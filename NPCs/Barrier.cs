using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
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
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[3];
        }

        /// <summary>
        /// The target to shield
        /// </summary>
        public float Target
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        /// <summary>
        /// Time without a target
        /// </summary>
        private float NoTarget
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        /// <summary>
        /// Checks if the given NPC is a valid target
        /// </summary>
        /// <param name="target">The NPC to be targeted</param>
        /// <returns>True if the NPC is a valid target, false otherwise</returns>
        private bool IsValidTarget(NPC target) =>
            target.type != NPC.type
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
            if (other.ModNPC is BaseChicken npc)
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
            if (target.ModNPC is BaseChicken npc && !npc.IsProtected)
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
            if (target.ModNPC is BaseChicken npc && npc.IsProtected)
            {
                npc.IsProtected = false;
            }
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 26;
            NPC.damage = 10;
            NPC.defense = 25;
            NPC.aiStyle = 0;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.lifeMax = 400;
            NPC.value = 100f;
            NPC.friendly = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("BarrierBanner").Type;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.life > NPC.lifeMax * 0.66)
            {
                NPC.frameCounter = 0;
            }
            else if (NPC.life <= NPC.lifeMax * 0.66 && NPC.life >= NPC.lifeMax * 0.33)
            {
                NPC.frameCounter = 1;
            }
            else if (NPC.life < NPC.lifeMax * 0.33)
            {
                NPC.frameCounter = 2;
            }
            NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(NPC.Center, 32, 32, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y);
                }

                SoundEngine.PlaySound(SoundID.NPCDeath14, NPC.position);
            }
            base.HitEffect(hitDirection, damage);
        }

        public override void AI()
        {
            // if the Barrier cannot find an NPC to shield in time, it will explode. Otherwise there will be too much barriers on screen instead of chickens
            if (NoTarget >= 300)
            {
                NPC.life = 0;
                NPC.active = false;
                NPC.HitEffect();
                return;
            }

            // find target
            NPC current = FindTarget();

            // move under target
            if (current != null)
            {
                if (NPC.life <= 0)
                {
                    UnshieldTarget(current);
                }
                else
                {
                    ShieldTarget(current);
                }

                var targetPosition = current.Bottom;
                targetPosition.Y += NPC.height;

                var velocity = .22f;

                if (targetPosition.X < NPC.Center.X && NPC.velocity.X > -3)
                {
                    NPC.velocity.X -= velocity;
                }
                else if (targetPosition.X > NPC.Center.X && NPC.velocity.X < 3)
                {
                    NPC.velocity.X += velocity;
                }

                if (targetPosition.Y < NPC.position.Y && NPC.velocity.Y > -3)
                {
                    NPC.velocity.Y -= velocity;
                }
                else if (targetPosition.Y > NPC.position.Y && NPC.velocity.Y < 3)
                {
                    NPC.velocity.Y += velocity;
                    NPC.velocity.Y *= 0.9f;
                }

                NPC.position += NPC.velocity;
            }
            else
            {
                // if there is no target
                NPC.velocity.Y = 0f;
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

                // check if the target is still alive
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
                    var dist = Vector2.Distance(NPC.Center, other.Center);
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
