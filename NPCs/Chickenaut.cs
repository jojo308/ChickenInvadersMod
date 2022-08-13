using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    public class Chickenaut : BaseChicken
    {
        int projectileType;
        int projectileDamage;
        float projectileSpeed;

        int shotsLeft = 3;
        bool Shooting => TimeLeft <= 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chickenaut");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            NPC.width = 96;
            NPC.height = 96;
            NPC.aiStyle = 2;
            NPC.damage = 28;
            NPC.defense = 20;
            NPC.lifeMax = 600;
            NPC.value = 50f;
            NPC.knockBackResist = 0.7f;
            NPC.friendly = false;
            NPC.buffImmune[BuffID.Confused] = true;
            if (!Main.dedServ)
            {
                NPC.HitSound = Mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/Chicken_Hit1").WithVolume(1f).WithPitchVariance(.3f); ;
                NPC.DeathSound = Mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chicken_Death1").WithVolume(1f).WithPitchVariance(.3f);
            }
            projectileType = ModContent.ProjectileType<Projectiles.NeutronProjectile>();
            projectileDamage = NPC.damage / 2;
            projectileSpeed = 7f;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("ChickenautBanner").Type;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Sky.Chance * 0.2f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20) NPC.frameCounter = 0;
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }

        public override void OnKill()
        {
            if (Main.rand.NextBool(3))
            {
                var dropChooser = new WeightedRandom<int>();
                dropChooser.Add(ModContent.ItemType<Items.ChickenDrumstick>(), 0.8);
                dropChooser.Add(ModContent.ItemType<Items.ChickenRoast>(), 0.05);
                dropChooser.Add(ModContent.ItemType<Items.DoubleHamburger>(), 0.1);
                dropChooser.Add(ModContent.ItemType<Items.QuadHamburger>(), 0.05);
                int choice = dropChooser;
                Item.NewItem(NPC.getRect(), choice);
            }

            if (Main.rand.NextBool(500))
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.SuspiciousLookingFeather>());
            }

            if (Main.rand.NextBool(2))
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Weapons.Egg>(), Main.rand.Next(1, 5));
            }

            base.OnKill();
        }

        public override void AI()
        {
            TimeLeft--;

            if (TimeLeft <= 0 || Shooting)
            {
                // stop attacking after 3 shots have been fired
                if (shotsLeft <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    shotsLeft = 3;
                    Interval = 16f;
                    TimeLeft = Main.rand.Next(150, 400);
                    NPC.netUpdate = true;
                    return;
                }

                Interval--;

                // attack
                if (Interval <= 0)
                {
                    Interval = 16f;
                    shotsLeft--;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (!Main.dedServ)
                        {
                            SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Neutron").WithVolume(5f).WithPitchVariance(.3f), NPC.Bottom);
                        }
                        NPC.ShootAtPlayer(NPC.Bottom, projectileType, projectileSpeed, projectileDamage);
                    }
                }
            }
        }
    }
}