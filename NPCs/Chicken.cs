using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace ChickenInvadersMod.NPCs
{
    public class Chicken : BaseChicken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicken");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 64;
            NPC.aiStyle = 2;
            NPC.damage = 28;
            NPC.defense = 10;
            NPC.lifeMax = 125;
            NPC.value = 50f;
            NPC.friendly = false;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.HitSound = SoundUtils.ChickenHit;
            NPC.DeathSound = SoundUtils.ChickenDeath;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("ChickenBanner").Type;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // Only spawn in hard mode
            if (Main.hardMode)
                return SpawnCondition.Sky.Chance * 0.2f;

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20) NPC.frameCounter = 0;
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Drop item if the player is in sky height with 2% chance
            npcLoot.Add(ItemDropRule.ByCondition(new InSkyCondition(), ModContent.ItemType<Items.SuspiciousLookingFeather>(), 50));

            // Drop item if a Chicken Invasion is active with 0.5% chance  
            npcLoot.Add(ItemDropRule.ByCondition(new ChickenInvasionCondition(), ModContent.ItemType<Items.SuspiciousLookingFeather>(), 200));

            // Drop 1-5 of this item with 33% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Egg>(), 3, 1, 5));

            // Drop some units of food
            var foodDropRule = ItemDropRule.Common(ModContent.ItemType<Items.ChickenDrumstick>(), 3, 1, 3);
            foodDropRule.OnFailedRoll(ItemDropRule.Common(ItemID.Burger, 4));
            foodDropRule.OnFailedRoll(ItemDropRule.OneFromOptions(4, ModContent.ItemType<Items.DoubleHamburger>(), ModContent.ItemType<Items.ChickenRoast>()));
            npcLoot.Add(foodDropRule);
        }

        public override void AI()
        {
            TimeLeft--;
            int type = ModContent.ProjectileType<Projectiles.FallingEggProjectile>();
            float speed = 7f;
            int damage = NPC.damage / 2;

            // shoot eggs 
            if (Main.netMode != NetmodeID.MultiplayerClient && TimeLeft <= 0)
            {
                TimeLeft = Main.rand.NextFloat(300, 600);
                NPC.ShootAtPlayer(NPC.Center, type, speed, damage);
                NPC.netUpdate = true;
            }
        }
    }
}
