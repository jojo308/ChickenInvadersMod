using ChickenInvadersMod.Common;
using ChickenInvadersMod.Common.Systems;
using ChickenInvadersMod.Content.Items.Consumables;
using ChickenInvadersMod.Content.Projectiles;
using ChickenInvadersMod.Utilities;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace ChickenInvadersMod.Content.NPCs
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
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public float AttackType
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public float LaserDirection
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        public float LaserStartingPoint
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        public float Degrees;
        #endregion

        public bool IsAttacking { get { return TimeLeft < 0; } }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Chicken");
            Main.npcFrameCount[NPC.type] = 4;

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] { BuffID.Confused }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, 88f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 56f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifier);
        }

        public override void SetDefaults()
        {
            NPC.width = 172;
            NPC.height = 156;
            NPC.aiStyle = 2;
            NPC.damage = 50;
            NPC.defense = 40;
            NPC.lifeMax = 10000;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 10);
            NPC.knockBackResist = 0f;
            NPC.friendly = false;
            NPC.boss = true;
            NPC.HitSound = SoundUtils.ChickenHit;
            NPC.DeathSound = SoundUtils.SuperChickenDeath;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                ModInvasions.Chickens,
                new FlavorTextBestiaryInfoElement("Despite its appearance, the Super Chicken is actually a villain."),
            });
        }

        public override bool CheckActive()
        {
            // prevent boss from despawning
            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            if (Main.expertMode)
            {
                NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
                NPC.damage = (int)(NPC.damage * 0.6f);
                NPC.defense = 45;
            }

            if (Main.masterMode)
            {
                NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
                NPC.damage = (int)(NPC.damage * 0.8f);
                NPC.defense = 50;
            }
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
                    NPC.frame.Y = FrameHit * frameHeight;
                }
                else if (hitRecently < 20)
                {
                    NPC.frame.Y = FrameHit2 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            else
            {
                if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = FrameNormal2 * frameHeight;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = FrameNormal * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            NPC.frameCounter++;
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

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Drop this item with 5% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SuspiciousLookingFeather>(), 20));

            // Drop 25-50 of this item with 100% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Egg>(), 1, 25, 50));

            // Drop plenty units of food            
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ChickenDrumstick>(), 1, 20, 25));
            npcLoot.Add(ItemDropRule.Common(ItemID.Burger, 1, 10, 15));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DoubleHamburger>(), 1, 5, 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<QuadHamburger>(), 1, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ChickenRoast>(), 1, 1, 5));
        }

        // todo add support for boss checklist
        public override void OnKill()
        {
            // if Super Chicken is defeated for the first time, set flag and trigger a lantern night
            if (!ChickenInvasionSystem.DownedSuperChicken)
            {
                ChickenInvasionSystem.DownedSuperChicken = true;

                // triggers a lantern night
                NPC.SetEventFlagCleared(ref ChickenInvasionSystem.DownedSuperChicken, -1);
            }
            base.OnKill();
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Invasion && Main.invasionProgressWave == 5)
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
            if (NPC.velocity.X < 0 && NPC.rotation > maxRotation * -1)
            {
                NPC.rotation -= 0.01f;
            }
            else if (NPC.velocity.X > 0 && NPC.rotation < maxRotation)
            {
                NPC.rotation += 0.01f;
            }

            // try to stay above target, but don't get too high
            var target = NPC.GetTargetPos();
            if ((NPC.position.Y + 156) > target.Y && NPC.velocity.Y < 3f)
            {
                NPC.velocity.Y -= 0.025f;
                NPC.position.Y += NPC.velocity.Y;
            }
            else if ((NPC.Bottom.Y + 400) < target.Y && NPC.velocity.Y > -3f)
            {
                NPC.velocity.Y += 0.025f;
                NPC.position.Y += NPC.velocity.Y;
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
            NPC.netUpdate = true;
            TimeLeft = -1;
            AttackType = Main.rand.Next(1, 5);
        }

        /// <summary>
        /// Shoots a laser straight down for a few seconds
        /// </summary>
        private void HandleLaserBeam()
        {
            // move faster horizontally when using the laser beam
            if (NPC.velocity.X <= 6f)
            {
                NPC.velocity.X += 0.01f;
                NPC.position.X += NPC.velocity.X;
            }

            // Shoot laser and than wait a few seconds
            if (TimeLeft == -1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.netUpdate = true;
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, Mod.Find<ModProjectile>("LaserBeam").Type, NPC.damage, 0f, Main.myPlayer, NPC.whoAmI);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                SoundEngine.PlaySound(SoundUtils.Laser, NPC.position);
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
                NPC.netUpdate = true;
                Vector2 velocity1 = new Vector2(0, -9f).RotatedBy(MathHelper.ToRadians(Degrees));
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity1, ModContent.ProjectileType<GuanoProjectile>(), NPC.damage, 0f, Main.myPlayer);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                SoundEngine.PlaySound(SoundUtils.Neutron, NPC.position);
            }
        }

        /// <summary>
        /// Shoots 16 eggs straight down
        /// </summary>
        private void HandleBulkEggs()
        {
            if (TimeLeft % -16 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.netUpdate = true;
                NPC.ShootDown(NPC.Bottom, ModContent.ProjectileType<FallingEggProjectile>(), 3f, NPC.damage, 0f);
                SoundEngine.PlaySound(SoundUtils.EggDrop, NPC.Center);
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
                NPC.netUpdate = true;
            }

            if (laser.HasValue)
            {
                // show warning
                if (TimeLeft == -1)
                {
                    NPC.netUpdate = true;
                    var entity = NPC.GetSource_FromAI();
                    int proj = Projectile.NewProjectile(entity, NPC.Center, Vector2.Zero, Mod.Find<ModProjectile>("LaserWarning").Type, 0, 0f, Main.myPlayer, NPC.whoAmI, laser.Value.Rotation1);
                    int proj2 = Projectile.NewProjectile(entity, NPC.Center, Vector2.Zero, Mod.Find<ModProjectile>("LaserWarning").Type, 0, 0f, Main.myPlayer, NPC.whoAmI, laser.Value.Rotation2);
                    int proj3 = Projectile.NewProjectile(entity, NPC.Center, Vector2.Zero, Mod.Find<ModProjectile>("LaserWarning").Type, 0, 0f, Main.myPlayer, NPC.whoAmI, laser.Value.Rotation3);
                    int proj4 = Projectile.NewProjectile(entity, NPC.Center, Vector2.Zero, Mod.Find<ModProjectile>("LaserWarning").Type, 0, 0f, Main.myPlayer, NPC.whoAmI, laser.Value.Rotation4);
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
                    NPC.netUpdate = true;
                    var entity = NPC.GetSource_FromAI();
                    int proj = Projectile.NewProjectile(entity, NPC.Center, Vector2.Zero, Mod.Find<ModProjectile>("QuadrupleLaser").Type, NPC.damage, 0f, Main.myPlayer, NPC.whoAmI, laser.Value.Rotation1);
                    int proj2 = Projectile.NewProjectile(entity, NPC.Center, Vector2.Zero, Mod.Find<ModProjectile>("QuadrupleLaser").Type, NPC.damage, 0f, Main.myPlayer, NPC.whoAmI, laser.Value.Rotation2);
                    int proj3 = Projectile.NewProjectile(entity, NPC.Center, Vector2.Zero, Mod.Find<ModProjectile>("QuadrupleLaser").Type, NPC.damage, 0f, Main.myPlayer, NPC.whoAmI, laser.Value.Rotation3);
                    int proj4 = Projectile.NewProjectile(entity, NPC.Center, Vector2.Zero, Mod.Find<ModProjectile>("QuadrupleLaser").Type, NPC.damage, 0f, Main.myPlayer, NPC.whoAmI, laser.Value.Rotation4);

                    Main.projectile[proj].localAI[0] = LaserDirection;
                    Main.projectile[proj2].localAI[0] = LaserDirection;
                    Main.projectile[proj3].localAI[0] = LaserDirection;
                    Main.projectile[proj4].localAI[0] = LaserDirection;

                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj2);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj3);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj4);

                    SoundEngine.PlaySound(SoundUtils.Laser, NPC.position);
                    return;
                }
            }
        }

        /// <summary>
        /// Resets the attack
        /// </summary>
        private void Reset()
        {
            NPC.netUpdate = true;
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
