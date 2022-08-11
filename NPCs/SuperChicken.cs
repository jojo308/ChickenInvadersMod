﻿using ChickenInvadersMod.Projectiles;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.NPCs
{
    [AutoloadBossHead]
    public class SuperChicken : ModNPC
    {
        const float maxRotation = 0.262f; // this is 15 degrees in radians      
        const int laserBeamTime = -240;
        const int guanoSprayTime = -480;
        const int bulkEggsTime = -256;
        const int quadrupleLaserTime = -300;

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

        public float LaserDirection
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
        }

        public float LaserStartingPoint
        {
            get => npc.ai[3];
            set => npc.ai[3] = value;
        }

        public float Degrees;
        #endregion

        public bool IsAttacking { get { return TimeLeft < 0; } }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Chicken");
            Main.npcFrameCount[npc.type] = 4;
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
            if (!Main.dedServ)
            {
                npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/Chicken_Hit1").WithVolume(1f).WithPitchVariance(.3f); ;
                npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chicken_Death1").WithVolume(1f).WithPitchVariance(.3f);
            }
            npc.buffImmune[BuffID.Confused] = true;
        }

        private const int FrameNormal = 0;
        private const int FrameNormal2 = 1;
        private const int FrameHit = 2;
        private const int FrameHit2 = 3;

        public override void FindFrame(int frameHeight)
        {
            if (hitRecently > 0)
            {
                if (hitRecently < 10)
                {
                    npc.frame.Y = FrameHit * frameHeight;
                }
                else if (hitRecently < 20)
                {
                    npc.frame.Y = FrameHit2 * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            else
            {
                if (npc.frameCounter < 10)
                {
                    npc.frame.Y = FrameNormal2 * frameHeight;
                }
                else if (npc.frameCounter < 20)
                {
                    npc.frame.Y = FrameNormal * frameHeight;
                }
                else
                {
                    npc.frameCounter = 0;
                }
            }
            npc.frameCounter++;
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

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Degrees);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Degrees = reader.ReadSingle();
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
            }

            if (TimeLeft >= 200 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                StartAttack();
            }

            // start or continue the attack
            if (IsAttacking)
            {
                if (AttackType == laserBeam && TimeLeft >= laserBeamTime)
                {
                    HandleLaserBeam();
                    TimeLeft--;
                    return;
                }
                if (AttackType == guanoSpray && TimeLeft >= guanoSprayTime)
                {
                    HandleGuanoSpray();
                    TimeLeft--;
                    return;
                }
                if (AttackType == bulkEggs && TimeLeft >= bulkEggsTime)
                {
                    HandleBulkEggs();
                    TimeLeft--;
                    return;
                }
                if (AttackType == quadrupleLaser && TimeLeft >= quadrupleLaserTime)
                {
                    HandleQuadrupleLaser();
                    TimeLeft--;
                    return;
                }

                Reset();
            }
        }

        /// <summary>
        /// Starts a random attack
        /// </summary>
        private void StartAttack()
        {
            npc.netUpdate = true;
            TimeLeft = -1;
            AttackType = Main.rand.Next(1, 5);
        }

        /// <summary>
        /// Shoots a laser straight down for a few seconds
        /// </summary>
        private void HandleLaserBeam()
        {
            // move faster horizontally when using the laser beam
            if (npc.velocity.X <= 6f)
            {
                npc.velocity.X += 0.01f;
                npc.position.X += npc.velocity.X;
            }

            // Shoot laser and than wait a few seconds
            if (TimeLeft == -1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.netUpdate = true;
                int proj = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("LaserBeam"), npc.damage, 0f, Main.myPlayer, npc.whoAmI);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                if (!Main.dedServ)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Laser").WithVolume(3f).WithPitchVariance(.3f), npc.position);
                }
            }
        }

        /// <summary>
        /// Shoots 30 Guano projectiles in different directions
        /// </summary>
        private void HandleGuanoSpray()
        {
            if (TimeLeft % -16 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Degrees = Main.rand.Next(0, 360);
                npc.netUpdate = true;
                Vector2 velocity1 = new Vector2(0, -9f).RotatedBy(MathHelper.ToRadians(Degrees));
                int proj = Projectile.NewProjectile(npc.Center, velocity1, ModContent.ProjectileType<GuanoProjectile>(), npc.damage, 0f, Main.myPlayer);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                if (!Main.dedServ)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Neutron").WithVolume(3f).WithPitchVariance(.3f), npc.position);
                }
            }
        }

        /// <summary>
        /// Shoots 16 eggs straight down
        /// </summary>
        private void HandleBulkEggs()
        {
            if (TimeLeft % -16 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.netUpdate = true;
                npc.ShootDown(npc.Bottom, ModContent.ProjectileType<FallingEggProjectile>(), 3f, npc.damage, 0f);
                if (!Main.dedServ)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Egg_Drop").WithVolume(5f).WithPitchVariance(.3f), npc.Center);
                }
            }
        }

        /// <summary>
        ///  Shoots 4 lasers 90 degrees from each other and rotates them
        /// </summary>
        private void HandleQuadrupleLaser()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            // set value for laser
            if (!laser.HasValue)
            {
                LaserStartingPoint = Main.rand.NextFloat(-90, 90);
                laser = new QuadrupleLaser(LaserStartingPoint);
                npc.netUpdate = true;
            }

            if (laser.HasValue)
            {
                // show warning
                if (TimeLeft == -1)
                {
                    npc.netUpdate = true;
                    int proj = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("LaserWarning"), 0, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation1);
                    int proj2 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("LaserWarning"), 0, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation2);
                    int proj3 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("LaserWarning"), 0, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation3);
                    int proj4 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("LaserWarning"), 0, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation4);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj2);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj3);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj4);
                    return;
                }

                // Shoot laser and than wait a few seconds
                if (TimeLeft == -60)
                {
                    LaserDirection = Main.rand.Next(0, 2);
                    npc.netUpdate = true;

                    int proj = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation1);
                    int proj2 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation2);
                    int proj3 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation3);
                    int proj4 = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("QuadrupleLaser"), npc.damage, 0f, Main.myPlayer, npc.whoAmI, laser.Value.Rotation4);

                    Main.projectile[proj].localAI[0] = LaserDirection;
                    Main.projectile[proj2].localAI[0] = LaserDirection;
                    Main.projectile[proj3].localAI[0] = LaserDirection;
                    Main.projectile[proj4].localAI[0] = LaserDirection;
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj2);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj3);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj4);

                    if (!Main.dedServ)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Laser").WithVolume(3f).WithPitchVariance(.3f), npc.position);
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Resets the attack
        /// </summary>
        private void Reset()
        {
            npc.netUpdate = true;
            TimeLeft = 0;
            AttackType = idle;
            LaserDirection = 0;
            LaserStartingPoint = 0;
            Degrees = 0;
            laser = default;
        }
    }

    /// <summary>
    /// Contains the radian values for the quadruple laser using the specifed starting degee
    /// </summary>
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
