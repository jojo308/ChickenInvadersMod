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
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chick Gatling Gun");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            NPC.width = 78;
            NPC.height = 127;
            NPC.damage = 50;
            NPC.defense = 25;
            NPC.lifeMax = 750;
            NPC.aiStyle = 0;
            NPC.value = 150f;
            NPC.knockBackResist = 0f;
            NPC.friendly = false;
            NPC.noGravity = true;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            projectileType = ModContent.ProjectileType<Projectiles.FallingEggProjectile>();
            projectileSpeed = 7f;
            projectileDamage = NPC.damage / 2;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("ChickGatlingGunBanner").Type;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20) NPC.frameCounter = 0;
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }

        // todo implement new loot system
        public override void OnKill()
        {
            if (Main.rand.NextBool(10))
            {
                var dropChooser = new WeightedRandom<int>();
                dropChooser.Add(ModContent.ItemType<Items.DoubleHamburger>(), 0.9);
                dropChooser.Add(ModContent.ItemType<Items.QuadHamburger>(), 0.01);
                int choice = dropChooser;
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), choice);
            }

            if (Main.rand.NextBool(500))
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Items.SuspiciousLookingFeather>());
            }

            if (Main.rand.NextBool(2))
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Items.Weapons.Egg>(), Main.rand.Next(1, 5));
            }

            base.OnKill();
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
                    NPC.netUpdate = true;
                    return null;
                }

                return current;
            }

            // find new target
            NPC.TargetClosest(true);
            Player target = NPC.HasValidTarget ? Main.player[NPC.target] : null;

            Target = target == null ? -1f : target.whoAmI;
            NPC.netUpdate = true;
            return target;
        }

        public override void AI()
        {
            TimeLeft--;

            if (TimeLeft < -48 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                TimeLeft = Main.rand.Next(240, 420);
                Target = -1; // find new target after shooting
                NPC.netUpdate = true;
            }

            // find target            
            var target = FindTarget();

            // no need to execute rest of the code if NPC has no target
            if (target == null) return;

            var targetPosition = target.Center;

            // aim gun at target
            Vector2 direction = NPC.Bottom - targetPosition;
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            NPC.rotation = rotation + ((float)Math.PI * 0.5f);

            // shoot target occasionally           
            if (Shooting && TimeLeft % -16 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                // eggs should be shot from the gatling gun. Since this npc rotates depending on the players position, the
                // start position of the egg projectile should be rotated too
                var position = Vector2.Transform(NPC.Bottom - NPC.Center, Matrix.CreateRotationZ(NPC.rotation)) + NPC.Center;
                NPC.ShootAtPlayer(position, projectileType, projectileSpeed, projectileDamage);
            }

            // move away from player if the player is getting close
            if (Vector2.Distance(NPC.Center, targetPosition) < 150)
            {
                // move horizontally
                if (targetPosition.X < NPC.Center.X && NPC.velocity.X > -3) // target on left
                {
                    NPC.velocity.X += 0.22f; // go right
                }
                else if (targetPosition.X > NPC.Center.X && NPC.velocity.X < 3) // target on right
                {
                    NPC.velocity.X -= 0.22f;  // go left
                }

                // move vertically
                if (targetPosition.Y < NPC.position.Y && NPC.velocity.Y > -3) // target above
                {
                    NPC.velocity.Y += 0.22f; // go down
                }
                else if (targetPosition.Y > NPC.position.Y && NPC.velocity.Y < 3) // target below
                {
                    NPC.velocity.Y -= 0.22f; // go up
                }
            }
            else
            {
                // if npc is too far away from the player, slowly get back
                if ((targetPosition.Y - 600) > NPC.position.Y)
                {
                    NPC.velocity.Y += 0.11f;
                }

                if ((targetPosition.X - 800) < NPC.position.X)
                {
                    NPC.velocity.X -= 0.22f;
                }

                if ((targetPosition.X + 800) > NPC.position.X)
                {
                    NPC.velocity.X += 0.22f;
                }
            }

            NPC.velocity *= 0.95f;
            NPC.CheckCollision();
            NPC.position += NPC.velocity;
        }
    }
}