using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            npc.width = 96;
            npc.height = 96;
            npc.aiStyle = 2;
            npc.damage = 28;
            npc.defense = 20;
            npc.lifeMax = 600;
            npc.value = 50f;
            npc.knockBackResist = 0.7f;
            npc.friendly = false;
            npc.buffImmune[BuffID.Confused] = true;
            //npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/Chicken_Hit1").WithVolume(1f).WithPitchVariance(.3f); ;
            //npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chicken_Death1").WithVolume(1f).WithPitchVariance(.3f);

            projectileType = ModContent.ProjectileType<Projectiles.NeutronProjectile>();
            projectileDamage = npc.damage / 2;
            projectileSpeed = 7f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Sky.Chance * 0.2f;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 20) npc.frameCounter = 0;
            npc.frame.Y = (int)npc.frameCounter / 10 * frameHeight;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(3))
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
            TimeLeft--;

            if (TimeLeft <= 0 || Shooting)
            {
                // stop attacking after 3 shots have been fired
                if (shotsLeft <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    shotsLeft = 3;
                    Interval = 16f;
                    TimeLeft = Main.rand.Next(150, 400);
                    npc.netUpdate = true;
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
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Neutron").WithVolume(5f).WithPitchVariance(.3f), npc.Bottom);
                        npc.ShootAtPlayer(npc.Bottom, projectileType, projectileSpeed, projectileDamage);
                    }
                }
            }
        }
    }
}