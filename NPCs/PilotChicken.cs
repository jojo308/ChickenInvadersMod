using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                ModInvasions.Chickens,
                new FlavorTextBestiaryInfoElement("A chicken that is specialized in... flying."),
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20) NPC.frameCounter = 0;
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Drop this item with 0.5% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.SuspiciousLookingFeather>(), 200));

            // Drop 1-5 of this item with 33% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Egg>(), 3, 1, 5));

            var foodDropRule = ItemDropRule.Common(ModContent.ItemType<Items.ChickenDrumstick>(), 5);
            foodDropRule.OnFailedRoll(ItemDropRule.Common(ItemID.Burger, 5));
            foodDropRule.OnFailedRoll(ItemDropRule.OneFromOptions(5, ModContent.ItemType<Items.DoubleHamburger>(), ModContent.ItemType<Items.ChickenRoast>()));
            npcLoot.Add(foodDropRule);
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
