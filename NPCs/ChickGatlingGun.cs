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

        int shotsLeft = 3;
        bool shooting = false;

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

        public override void AI()
        {
            TimeLeft--;

            // find target
            var targetPosition = npc.GetTargetPos();

            // aim gun at target
            Vector2 direction = npc.Bottom - targetPosition;
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            npc.rotation = rotation + ((float)Math.PI * 0.5f);

            // shoot target occasionally
            if (TimeLeft <= 0 || shooting)
            {
                TimeLeft = Main.rand.NextFloat(240, 420);
                Interval--;
                shooting = true;
                npc.netUpdate = true;

                // eggs should be shot from the gatling gun. Since this npc rotates depending on the players position, the
                // start position of the egg projectile should be rotated too
                var position = Vector2.Transform(npc.Bottom - npc.Center, Matrix.CreateRotationZ(npc.rotation)) + npc.Center;

                // stop shooting afer 3 times
                if (shotsLeft <= 0)
                {
                    shotsLeft = 3;
                    shooting = false;
                }

                if (Interval <= 0)
                {
                    Interval = 16;
                    shotsLeft--;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.ShootAtPlayer(position, projectileType, projectileSpeed, projectileDamage);
                    }
                }
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
                if ((targetPosition.Y - Main.screenHeight / 2) > npc.position.Y)
                {
                    npc.velocity.Y += 0.11f;
                }

                if ((targetPosition.X - Main.screenWidth / 2.5) < npc.position.X)
                {
                    npc.velocity.X -= 0.22f;
                }

                if ((targetPosition.X + Main.screenWidth / 2.5) > npc.position.X)
                {
                    npc.velocity.X += 0.22f;
                }
            }
            npc.velocity *= 0.95f;

            // check for collision
            npc.CheckCollision();

            // move
            npc.position += npc.velocity;
        }
    }
}