using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    class ChickGatlingGun : BaseChicken
    {
        int projectileType;
        int projectileDamage;
        float projectileSpeed;

        public bool Shooting => TimeLeft < 0;

        // since we dont use the Interval AI slot, we can override it here
        public float Target
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chick Gatling Gun");
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            npc.width = 78;
            npc.height = 127;
            npc.damage = 50;
            npc.defense = 25;
            npc.lifeMax = 750;
            npc.aiStyle = 0;
            npc.value = 150f;
            npc.knockBackResist = 0f;
            npc.friendly = false;
            npc.noGravity = true;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Confused] = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            projectileType = ModContent.ProjectileType<Projectiles.FallingEggProjectile>();
            projectileSpeed = 7f;
            projectileDamage = npc.damage / 2;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 20) npc.frameCounter = 0;
            npc.frame.Y = (int)npc.frameCounter / 10 * frameHeight;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(10))
            {
                var dropChooser = new WeightedRandom<int>();
                dropChooser.Add(ModContent.ItemType<Items.DoubleHamburger>(), 0.9);
                dropChooser.Add(ModContent.ItemType<Items.QuadHamburger>(), 0.01);
                int choice = dropChooser;
                Item.NewItem(npc.getRect(), choice);
            }

            if (Main.rand.NextBool(500))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.SuspiciousLookingFeather>());
            }

            if (Main.rand.NextBool(2))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Weapons.Egg>(), Main.rand.Next(1, 5));
            }

            base.NPCLoot();
        }

        /// <summary>
        /// Tries to find a target to shoot at. Will target the nearest player who is alive
        /// </summary>
        /// <returns>The target, or null if there is no target</returns>
        public Player FindTarget()
        {
            // if there already is a target
            if (Target >= 0)
            {
                var current = Main.player[(int)Target];

                // check if the target is stil lalive
                if (current.dead || !current.active)
                {
                    Target = -1;
                    npc.netUpdate = true;
                    return null;
                }

                return current;
            }

            // find new target
            npc.TargetClosest(true);
            Player target = npc.HasValidTarget ? Main.player[npc.target] : null;

            Target = target == null ? -1f : target.whoAmI;
            npc.netUpdate = true;
            return target;
        }

        public override void AI()
        {
            TimeLeft--;

            if (TimeLeft < -48 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                TimeLeft = Main.rand.Next(240, 420);
                Target = -1; // find new target after shooting
                npc.netUpdate = true;
            }

            // find target            
            var target = FindTarget();
            var targetPosition = target.Center;

            // no need to execute rest of the code if NPC has no target
            if (target == null) return;

            // aim gun at target
            Vector2 direction = npc.Bottom - targetPosition;
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            npc.rotation = rotation + ((float)Math.PI * 0.5f);

            // shoot target occasionally           
            if (Shooting && TimeLeft % -16 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                // eggs should be shot from the gatling gun. Since this npc rotates depending on the players position, the
                // start position of the egg projectile should be rotated too
                var position = Vector2.Transform(npc.Bottom - npc.Center, Matrix.CreateRotationZ(npc.rotation)) + npc.Center;
                npc.ShootAtPlayer(position, projectileType, projectileSpeed, projectileDamage);
            }

            // move away from player if the player is getting close
            if (Vector2.Distance(npc.Center, targetPosition) < 150)
            {
                // move horizontally
                if (targetPosition.X < npc.Center.X && npc.velocity.X > -3) // target on left
                {
                    npc.velocity.X += 0.22f; // go right
                }
                else if (targetPosition.X > npc.Center.X && npc.velocity.X < 3) // target on right
                {
                    npc.velocity.X -= 0.22f;  // go left
                }

                // move vertically
                if (targetPosition.Y < npc.position.Y && npc.velocity.Y > -3) // target above
                {
                    npc.velocity.Y += 0.22f; // go down
                }
                else if (targetPosition.Y > npc.position.Y && npc.velocity.Y < 3) // target below
                {
                    npc.velocity.Y -= 0.22f; // go up
                }
            }
            else
            {
                // if npc is too far away from the player, slowly get back
                if ((targetPosition.Y - 600) > npc.position.Y)
                {
                    npc.velocity.Y += 0.11f;
                }

                if ((targetPosition.X - 800) < npc.position.X)
                {
                    npc.velocity.X -= 0.22f;
                }

                if ((targetPosition.X + 800) > npc.position.X)
                {
                    npc.velocity.X += 0.22f;
                }
            }

            npc.velocity *= 0.95f;
            npc.CheckCollision();
            npc.position += npc.velocity;
        }
    }
}