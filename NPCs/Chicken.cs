using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    public class Chicken : ModNPC
    {
        private readonly string[] HitSounds = new[] { "Sounds/NPCHit/Chicken_Hit1", "Sounds/NPCHit/Chicken_Hit2", "Sounds/NPCHit/Chicken_Hit3", "Sounds/NPCHit/Chicken_Hit4" };
        private readonly string[] DeathSounds = new[] { "Sounds/NPCKilled/Chicken_Death1", "Sounds/NPCKilled/Chicken_Death2" };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicken");
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 64;
            npc.aiStyle = 2;
            npc.damage = 28;
            npc.defense = 10;
            npc.lifeMax = 125;
            npc.value = 50f;
            npc.friendly = false;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Confused] = true;

            var hitIndex = Main.rand.Next(0, 3);
            var hitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, HitSounds[hitIndex]).WithVolume(1f).WithPitchVariance(.3f);
            npc.HitSound = hitSound;

            var deathIndex = Main.rand.Next(0, 1);
            var deathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, DeathSounds[deathIndex]).WithVolume(1f).WithPitchVariance(.3f);
            npc.DeathSound = deathSound;
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
                dropChooser.Add(ModContent.ItemType<Items.ChickenDrumstick>(), 0.7);
                dropChooser.Add(ModContent.ItemType<Items.ChickenTwinLegs>(), 0.125);
                dropChooser.Add(ModContent.ItemType<Items.ChickenRoast>(), 0.025);
                dropChooser.Add(ModContent.ItemType<Items.DoubleHamburger>(), 0.125);
                dropChooser.Add(ModContent.ItemType<Items.QuadHamburger>(), 0.025);
                int choice = dropChooser;
                Item.NewItem(npc.getRect(), choice);
            }

            var chance = CIWorld.ChickenInvasionActive ? 600 : 100;
            if (Main.rand.NextBool(chance))
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
            npc.TargetClosest();
            if (npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(900))
            {
                // target player
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();

                // set params
                float speed = 7f;
                int type = ModContent.ProjectileType<Projectiles.FallingEggProjectile>();
                int damage = npc.damage / 2;

                // shoot
                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
            }
            base.AI();
        }
    }
}
