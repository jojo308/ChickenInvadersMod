using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

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
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), choice);
            }

            var chance = CIWorld.ChickenInvasionActive ? 500 : 100;
            if (Main.rand.NextBool(chance))
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
