using ChickenInvadersMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.NPCs
{
    [AutoloadBossHead]
    public class SuperChicken : ModNPC
    {
        const float maxRotation = 0.262f; // this is 15 degrees in radians
        const int bulkEggCount = 16;
        const int guanoCount = 30;

        int shots = 0;
        float hitRecently = 0f;
        QuadrupleLaser? laser;

        #region Attacks
        const float idle = 0f;
        const float laserBeam = 1f;
        const float guanoSpray = 2f;
        const float bulkEggs = 3f;
        const float quadrupleLaser = 4f;
        #endregion       

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
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            npc.width = 172;
            npc.height = 156;
            npc.aiStyle = 2;
            npc.damage = 50;
            npc.defense = 40;
            npc.lifeMax = 10000;
            npc.noTileCollide = true;
            npc.value = Item.buyPrice(gold: 10);
            npc.knockBackResist = 0f;
            npc.friendly = false;
            npc.boss = true;
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/Chicken_Hit1").WithVolume(1f).WithPitchVariance(.3f); ;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chicken_Death1").WithVolume(1f).WithPitchVariance(.3f);
            npc.buffImmune[BuffID.Confused] = true;
        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            // prevent the NPC from spawning inside the ground
            return base.SpawnNPC(tileX, tileY - 50);
        }

        public override void FindFrame(int frameHeight)
        {
            Main.npcTexture[npc.type] = (hitRecently > 0) ? mod.GetTexture("NPCs/SuperChicken_Hit") : mod.GetTexture("NPCs/SuperChicken");

            npc.frameCounter++;
            if (npc.frameCounter >= 20) npc.frameCounter = 0;
            npc.frame.Y = (int)npc.frameCounter / 10 * frameHeight;
        }

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            OnHit();
            base.OnHitByItem(player, item, damage, knockback, crit);
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            OnHit();
            base.OnHitByProjectile(projectile, damage, knockback, crit);
        }

        /// <summary>
        /// Called when the npc was hit by a player. Sets the hitRecently field
        /// </summary>
        public void OnHit()
        {
            // If the NPC is hit, set a short timer where the npc widens it eyes for a moment
            hitRecently = 20f;
        }

        // todo add support for boss checklist
        public override void NPCLoot()
        {
            // if Super Chicken is defeated for the first time, update world and inform server if necessary
            if (!CIWorld.DownedSuperChicken)
            {
                CIWorld.DownedSuperChicken = true;
                if (Main.netMode == NetmodeID.Server) NetMessage.SendData(MessageID.WorldData);
            }

            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Weapons.Egg>(), Main.rand.Next(10, 31));
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.ChickenDrumstick>(), Main.rand.Next(5, 16));
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.ChickenTwinLegs>(), Main.rand.Next(2, 9));
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.DoubleHamburger>(), Main.rand.Next(2, 9));
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.ChickenRoast>(), Main.rand.Next(1, 4));
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.QuadHamburger>(), Main.rand.Next(1, 4));
            base.NPCLoot();
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.invasion && Main.invasionProgressWave == 5)
            {
                return SpawnCondition.Invasion.Chance;
            }

            return 0f;
        }

        public override void AI()
        {
            if (hitRecently > 0) hitRecently--;

            // rotate the NPC to the left or right
            if (npc.velocity.X < 0 && npc.rotation > maxRotation * -1)
            {
                npc.rotation -= 0.01f;
            }
            else if (npc.velocity.X > 0 && npc.rotation < maxRotation)
            {
                npc.rotation += 0.01f;
            }

            // try to stay above target, but don't get too high
            var target = npc.GetTargetPos();
            if ((npc.position.Y + 156) > target.Y && npc.velocity.Y < 3f)
            {
                npc.velocity.Y -= 0.025f;
                npc.position.Y += npc.velocity.Y;
            }
            else if ((npc.Bottom.Y + 400) < target.Y && npc.velocity.Y > -3f)
            {
                npc.velocity.Y += 0.025f;
                npc.position.Y += npc.velocity.Y;
            }

            // decrease attack timer if NPC is not attacking
            if (!IsAttacking)
            {
                TimeLeft++;
                AttackType = Main.rand.Next(1, 5);
            }

            // start or continue the attack
            if (Main.netMode != NetmodeID.MultiplayerClient && (TimeLeft >= 200 || IsAttacking))
            {
                TimeLeft = -1;

                if (AttackType == laserBeam) HandleLaserBeam();
                if (AttackType == guanoSpray) HandleGuanoSpray();
                if (AttackType == bulkEggs) HandleBulkEggs();
                if (AttackType == quadrupleLaser) HandleQuadrupleLaser();
            }
        }

        /// <summary>
        /// Shoots a laser straight down for a few seconds
        /// </summary>
        private void HandleLaserBeam()
        {
            Interval++;

            // move faster horizontally when using the laser beam
            if (npc.velocity.X <= 6f)
            {
                npc.velocity.X += 0.01f;
                npc.position.X += npc.velocity.X;
            }

            // Shoot laser and than wait a few seconds
            if (Interval == 1)
            {
                int proj = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("LaserBeam"), npc.damage, 0f, Main.myPlayer, npc.whoAmI);
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Laser").WithVolume(3f).WithPitchVariance(.3f), npc.position);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
            }

            if (Interval >= 240)
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
                Vector2 velocity1 = new Vector2(0, -9f).RotatedBy(MathHelper.ToRadians(degrees));
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

            if (Interval >= 300)
            {
                Reset();
            }

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
                    int clockwise = Main.rand.Next(0, 2);
                    int proj = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation1);
                    int proj2 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation2);
                    int proj3 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation3);
                    int proj4 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation4);
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Laser").WithVolume(3f).WithPitchVariance(.3f), npc.position);
                    Main.projectile[proj].localAI[0] = clockwise;
                    Main.projectile[proj2].localAI[0] = clockwise;
                    Main.projectile[proj3].localAI[0] = clockwise;
                    Main.projectile[proj4].localAI[0] = clockwise;
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj, proj2, proj3, proj4);
                }
            }
        }

        /// <summary>
        /// Resets the attack
        /// </summary>
        private void Reset()
        {
            TimeLeft = 0;
            AttackType = idle;
            Interval = 0;
            hitRecently = 0;
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
