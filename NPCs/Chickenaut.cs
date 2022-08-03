using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    public class Chickenaut : ModNPC
    {
        int projectileType;
        int projectileDamage;
        float projectileSpeed;

        int shotsLeft = 3;
        bool shooting = false;

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
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/Chicken_Hit1").WithVolume(1f).WithPitchVariance(.3f); ;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chicken_Death1").WithVolume(1f).WithPitchVariance(.3f);

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
            npc.ai[0]--;

            if (Main.netMode != NetmodeID.MultiplayerClient && (npc.ai[0] <= 0 || shooting))
            {
                shooting = true;
                npc.ai[0] = Main.rand.NextFloat(150, 400);
                npc.ai[1]--;

                if (shotsLeft <= 0)
                {
                    shotsLeft = 3;
                    shooting = false;
                }

                if (npc.ai[1] <= 0)
                {
                    npc.ai[1] = 16;
                    shotsLeft--;
                    ShootNeutron(npc.Bottom);
                }
            }
        }

        public void ShootNeutron(Vector2 position)
        {
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Neutron").WithVolume(5f).WithPitchVariance(.3f), position);
            npc.ShootAtPlayer(position, projectileType, projectileSpeed, projectileDamage);
        }
    }
}
