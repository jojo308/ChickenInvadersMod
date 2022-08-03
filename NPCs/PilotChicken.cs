using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    public class PilotChicken : ModNPC
    {
        int projectileType;
        int projectileDamage;
        float projectileSpeed;

        int shotsLeft = 2;
        bool shooting = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pilot Chicken");
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 64;
            npc.aiStyle = 2;
            npc.damage = 30;
            npc.defense = 12;
            npc.lifeMax = 250;
            npc.value = 50f;
            npc.friendly = false;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Confused] = true;
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/Chicken_Hit2").WithVolume(1f).WithPitchVariance(.3f); ;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chicken_Death2").WithVolume(1f).WithPitchVariance(.3f);

            projectileType = ModContent.ProjectileType<Projectiles.FallingEggProjectile>();
            projectileDamage = npc.damage / 2;
            projectileSpeed = 7f;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 20) npc.frameCounter = 0;
            npc.frame.Y = (int)npc.frameCounter / 10 * frameHeight;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(5))
            {
                var dropChooser = new WeightedRandom<int>();
                dropChooser.Add(ModContent.ItemType<Items.ChickenDrumstick>(), 0.8);
                dropChooser.Add(ModContent.ItemType<Items.ChickenRoast>(), 0.05);
                dropChooser.Add(ModContent.ItemType<Items.DoubleHamburger>(), 0.1);
                dropChooser.Add(ModContent.ItemType<Items.QuadHamburger>(), 0.05);
                int choice = dropChooser;
                Item.NewItem(npc.getRect(), choice);
            }

            if (Main.rand.NextBool(500))
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
            npc.ai[0]--;

            if (Main.netMode != NetmodeID.MultiplayerClient && (npc.ai[0] <= 0 || shooting))
            {
                shooting = true;
                npc.ai[0] = Main.rand.NextFloat(240, 480);
                npc.ai[1]--;

                // stop shooting afer 2 times
                if (shotsLeft <= 0)
                {
                    shotsLeft = 2;
                    shooting = false;
                }

                if (npc.ai[1] <= 0)
                {
                    npc.ai[1] = 16;
                    shotsLeft--;
                    npc.ShootAtPlayer(npc.Bottom, projectileType, projectileSpeed, projectileDamage);
                }
            }
            base.AI();
        }
    }
}
