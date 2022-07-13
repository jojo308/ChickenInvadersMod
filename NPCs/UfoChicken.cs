using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    class UfoChicken : ModNPC
    {
        int projectileType;
        int damage;
        float speed;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ufo Chicken");
        }

        public override void SetDefaults()
        {
            npc.width = 52;
            npc.height = 60;
            npc.damage = 35;
            npc.defense = 15;
            npc.lifeMax = 300;
            npc.value = 50f;
            npc.friendly = false;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Confused] = true;
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/Chicken_Hit3").WithVolume(1f).WithPitchVariance(.3f); ;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chicken_Death1").WithVolume(1f).WithPitchVariance(.3f);

            projectileType = ModContent.ProjectileType<Projectiles.FallingEggProjectile>();
            speed = 7f;
            damage = npc.damage / 2;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(10))
            {
                var dropChooser = new WeightedRandom<int>();
                dropChooser.Add(ModContent.ItemType<Items.DoubleHamburger>(), 0.9);
                dropChooser.Add(ModContent.ItemType<Items.QuadHamburger>(), 0.01);
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
            // target nearest player
            npc.TargetClosest(true);
            var player = Main.player[npc.target];
            var targetPosition = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;

            var velocityX = Main.rand.NextFloat(0.05f, 1f);

            // move horizontally
            if (targetPosition.X < npc.Center.X && npc.velocity.X > -3) // target on left
            {
                npc.velocity.X -= velocityX; // accelerate to the left

                if ((npc.position.X - targetPosition.X) < 200 && Main.rand.NextBool(500))
                {
                    this.ShootDown(projectileType, speed, damage);
                }
            }
            else if (targetPosition.X > npc.Center.X && npc.velocity.X < 3) // target on right
            {
                npc.velocity.X += velocityX; // accelerate to the right

                if ((targetPosition.X - npc.position.X) < 200 && Main.rand.NextBool(500))
                {
                    this.ShootDown(projectileType, speed, damage);
                }
            }

            // move vertically
            if (targetPosition.Y < npc.position.Y + 300) // target above
            {
                npc.velocity.Y -= (npc.velocity.Y < 0 && npc.velocity.Y > -2) ? 0.5f : 0.7f;
            }
            else if (targetPosition.Y > npc.position.Y) // target below
            {
                if (targetPosition.Y > npc.position.Y + 300) npc.velocity.Y += (npc.velocity.Y > 0 && npc.velocity.Y < 1f) ? 0.1f : 0.15f;

                // slows acceleration when moving down to prevent npc from hitting the player
                npc.velocity.Y *= 0.9f;
            }

            // check for collision                      
            npc.velocity = Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height, true, false, 1);

            // move
            npc.position += npc.velocity;

        }
    }
}
