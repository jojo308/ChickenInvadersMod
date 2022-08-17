using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

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
            NPC.HitSound = SoundUtils.ChickenHit;
            NPC.DeathSound = SoundUtils.ChickenDeath;
            projectileType = ModContent.ProjectileType<Projectiles.NeutronProjectile>();
            projectileDamage = NPC.damage / 2;
            projectileSpeed = 7f;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("ChickenautBanner").Type;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20) NPC.frameCounter = 0;
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Drop this item with 1% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.SuspiciousLookingFeather>(), 100));

            // Drop 1-5 of this item with 33% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Egg>(), 3, 1, 5));

            // Drop 1-5 of this item with 50% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.ChickenDrumstick>(), 2, 1, 5));

            // Smaller chance to drop some extra food
            var foodDropRule = ItemDropRule.OneFromOptions(3, ItemID.Burger, ModContent.ItemType<Items.DoubleHamburger>());
            foodDropRule.OnFailedRoll(ItemDropRule.OneFromOptions(6, ModContent.ItemType<Items.QuadHamburger>(), ModContent.ItemType<Items.ChickenRoast>()));
            npcLoot.Add(foodDropRule);
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
                        SoundEngine.PlaySound(SoundUtils.Neutron, NPC.Bottom);
                        NPC.ShootAtPlayer(NPC.Bottom, projectileType, projectileSpeed, projectileDamage);
                    }
                }
            }
        }
    }
}