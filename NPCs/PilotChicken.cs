using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    public class PilotChicken : BaseChicken
    {
        int projectileType;
        int projectileDamage;
        float projectileSpeed;

        int shotsLeft = 2;
        bool shooting = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pilot Chicken");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 64;
            NPC.aiStyle = 2;
            NPC.damage = 30;
            NPC.defense = 12;
            NPC.lifeMax = 250;
            NPC.value = 50f;
            NPC.friendly = false;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.HitSound = SoundUtils.ChickenHit;
            NPC.DeathSound = SoundUtils.ChickenDeath;
            projectileType = ModContent.ProjectileType<Projectiles.FallingEggProjectile>();
            Mod.Find<ModProjectile>("FallingEggProjectile");
            projectileDamage = NPC.damage / 2;
            projectileSpeed = 7f;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("PilotChickenBanner").Type;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20) NPC.frameCounter = 0;
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }

        public override void OnKill()
        {
            if (Main.rand.NextBool(5))
            {
                var dropChooser = new WeightedRandom<int>();
                dropChooser.Add(ModContent.ItemType<Items.ChickenDrumstick>(), 0.8);
                dropChooser.Add(ModContent.ItemType<Items.ChickenRoast>(), 0.05);
                dropChooser.Add(ModContent.ItemType<Items.DoubleHamburger>(), 0.1);
                dropChooser.Add(ModContent.ItemType<Items.QuadHamburger>(), 0.05);
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

        public override void AI()
        {
            TimeLeft--;

            if (TimeLeft <= 0 || shooting)
            {
                shooting = true;
                TimeLeft = Main.rand.NextFloat(240, 480);
                Interval--;
                NPC.netUpdate = true;

                // stop shooting afer 2 times
                if (shotsLeft <= 0)
                {
                    shotsLeft = 2;
                    shooting = false;
                }

                if (Interval <= 0)
                {
                    Interval = 16;
                    shotsLeft--;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ShootAtPlayer(NPC.Bottom, projectileType, projectileSpeed, projectileDamage);
                    }
                }
            }
        }
    }
}
