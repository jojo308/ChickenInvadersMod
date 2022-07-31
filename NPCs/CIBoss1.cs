﻿using ChickenInvadersMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    [AutoloadBossHead]
    public class CIBoss1 : ModNPC
    {
        const float maxRotation = 0.262f; // this is 15 degrees in radians
        const int bulkEggCount = 16;
        const int guanoCount = 30;

        #region Attacks
        const float idle = 0f;
        const float laserBeam = 1f;
        const float guanoSpray = 2f;
        const float bulkEggs = 3f;
        const float quadrupleLaser = 4f;
        #endregion

        int shots = 0;
        QuadrupleLaser? laser;

        #region AI
        public float TimeLeft
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
        }

        public float AttackType
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        public float Interval
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
        }
        #endregion

        public bool IsAttacking { get { return TimeLeft == -1; } }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Chicken");
            //Main.npcFrameCount[npc.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            npc.width = 172;
            npc.height = 156;
            npc.aiStyle = 2;
            npc.damage = 50;
            npc.defense = 40;
            npc.lifeMax = 1000;
            npc.value = 50f;
            npc.knockBackResist = 0f;
            npc.friendly = false;
            npc.boss = true;
            npc.buffImmune[BuffID.Confused] = true;
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/Chicken_Hit1").WithVolume(1f).WithPitchVariance(.3f); ;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chicken_Death1").WithVolume(1f).WithPitchVariance(.3f);
        }

        //public override void FindFrame(int frameHeight)
        //{
        //    npc.frameCounter++;
        //    if (npc.frameCounter >= 20) npc.frameCounter = 0;
        //    npc.frame.Y = (int)npc.frameCounter / 10 * frameHeight;
        //}

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(3))
            {
                var dropChooser = new WeightedRandom<int>();
                dropChooser.Add(ModContent.ItemType<Items.ChickenDrumstick>(), 0.8);
                dropChooser.Add(ModContent.ItemType<Items.ChickenRoast>(), 0.05);
                dropChooser.Add(ModContent.ItemType<Items.DoubleHamburger>(), 0.1);
                dropChooser.Add(ModContent.ItemType<Items.QuadHamburger>(), 0.05);
                int choice = dropChooser;
                Item.NewItem(npc.getRect(), choice);
            }

            var chance = CIWorld.ChickenInvasionActive ? 600 : 100;
            if (Main.rand.NextBool(chance))
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
            if (npc.velocity.X < 0 && npc.rotation > maxRotation * -1)
            {
                npc.rotation -= 0.01f; // rotate to left
            }
            else if (npc.velocity.X > 0 && npc.rotation < maxRotation)
            {
                npc.rotation += 0.01f; // rotate to right
            }

            if (!IsAttacking)
            {
                TimeLeft++;
                AttackType = Main.rand.Next(1, 5);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && (TimeLeft >= 300 || IsAttacking))
            {
                TimeLeft = -1;

                if (AttackType == laserBeam)
                {
                    HandleLaserBeam();
                }

                if (AttackType == guanoSpray)
                {
                    HandleGuanoSpray();
                }

                if (AttackType == bulkEggs)
                {
                    HandleBulkEggs();
                }

                if (AttackType == quadrupleLaser)
                {
                    HandleQuadrupleLaser();
                }
            }
        }

        /// <summary>
        /// Shoots a laser straight down for a few seconds
        /// </summary>
        private void HandleLaserBeam()
        {
            Interval++;

            // Shoot laser and than wait a few seconds
            if (Interval == 1)
            {
                int proj = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("LaserBeam"), npc.damage, 0f, Main.myPlayer, npc.whoAmI);
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Laser").WithVolume(3f).WithPitchVariance(.3f), npc.position);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
            }

            if (Interval >= 120)
            {
                Reset();
            }
        }

        /// <summary>
        /// Shoots 30 Guano projectiles in different directions
        /// </summary>
        private void HandleGuanoSpray()
        {
            Interval++;

            if (shots >= guanoCount)
            {
                Reset();
            }

            if (Interval >= 16)
            {
                Interval = 0;
                shots++;
                var degrees = Main.rand.Next(0, 360);
                Vector2 velocity1 = new Vector2(0, -6f).RotatedBy(MathHelper.ToRadians(degrees));
                Projectile.NewProjectile(npc.Center, velocity1, ModContent.ProjectileType<GuanoProjectile>(), npc.damage, 0f, Main.myPlayer);
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Neutron").WithVolume(3f).WithPitchVariance(.3f), npc.position);
            }
        }

        /// <summary>
        /// Shoots 16 eggs straight down
        /// </summary>
        private void HandleBulkEggs()
        {
            Interval++;

            if (shots >= bulkEggCount)
            {
                Reset();
            }

            if (Interval >= 16)
            {
                Interval = 0;
                shots++;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Egg_Drop").WithVolume(5f).WithPitchVariance(.3f), npc.Center);
                npc.ShootDown(npc.Bottom, ModContent.ProjectileType<FallingEggProjectile>(), 3f, npc.damage, 0f);
            }
        }

        /// <summary>
        ///  Shoots 4 lasers 90 degrees from each other and rotates them
        /// </summary>
        private void HandleQuadrupleLaser()
        {
            Interval++;

            // set value for laser
            if (!laser.HasValue)
            {
                laser = new QuadrupleLaser(Main.rand.NextFloat(-90, 90));
            }

            if (laser.HasValue)
            {
                // show warning
                if (Interval == 1)
                {
                    int proj = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("LaserWarning"), 0, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation1);
                    int proj2 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("LaserWarning"), 0, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation2);
                    int proj3 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("LaserWarning"), 0, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation3);
                    int proj4 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("LaserWarning"), 0, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation4);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj, proj2, proj3, proj4);
                }

                // Shoot laser and than wait a few seconds
                if (Interval == 60)
                {
                    int proj = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation1);
                    int proj2 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation2);
                    int proj3 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation3);
                    int proj4 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation4);
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Laser").WithVolume(3f).WithPitchVariance(.3f), npc.position);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj, proj2, proj3, proj4);
                }
            }

            if (Interval >= 300)
            {
                Reset();
            }
        }

        /// <summary>
        /// Resets the attack
        /// </summary>
        private void Reset()
        {
            TimeLeft = 0;
            Interval = 0;
            AttackType = idle;
            shots = 0;
            laser = default;
        }
    }

    public struct QuadrupleLaser
    {
        public readonly float Degrees;
        public QuadrupleLaser(float degrees)
        {
            Degrees = degrees;
        }

        public float Rotation1 { get => MathHelper.ToRadians(Degrees); }
        public float Rotation2 { get => MathHelper.ToRadians(Degrees + 90); }
        public float Rotation3 { get => MathHelper.ToRadians(Degrees + 180); }
        public float Rotation4 { get => MathHelper.ToRadians(Degrees + 270); }
    }
}