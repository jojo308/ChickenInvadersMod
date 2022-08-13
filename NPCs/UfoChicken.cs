using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    public class UfoChicken : BaseChicken
    {
        int projectileType;
        int projectileDamage;
        float projectileSpeed;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("UFO Chicken");
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
            npc.knockBackResist = 0.8f;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Confused] = true;
            if (!Main.dedServ)
            {
                npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/Chicken_Hit3").WithVolume(1f).WithPitchVariance(.3f); ;
                npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chicken_Death1").WithVolume(1f).WithPitchVariance(.3f);
            }
            projectileType = ModContent.ProjectileType<Projectiles.FallingEggProjectile>();
            projectileSpeed = 7f;
            projectileDamage = npc.damage / 2;
            banner = npc.type;
            bannerItem = mod.ItemType("UfoChickenBanner");
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(10))
            {
                var dropChooser = new WeightedRandom<int>();
                dropChooser.Add(ModContent.ItemType<Items.DoubleHamburger>(), 0.9);
                dropChooser.Add(ModContent.ItemType<Items.QuadHamburger>(), 0.1);
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
            // target nearest player
            var targetPosition = npc.GetTargetPos();
            var velocityX = 0.2f;

            if (Interval >= 400)
            {
                Interval = 0;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ShootDown(projectileType, projectileSpeed, projectileDamage);
                }
            }

            // move horizontally
            if (targetPosition.X < npc.Center.X && npc.velocity.X > -3) // target is on the left
            {
                npc.velocity.X -= velocityX; // accelerate to the left

                // target is below npc
                if ((npc.position.X - targetPosition.X) < 200) Interval++;


            }
            else if (targetPosition.X > npc.Center.X && npc.velocity.X < 3) // target is on the right
            {
                npc.velocity.X += velocityX; // accelerate to the right

                // target is below npc
                if ((targetPosition.X - npc.position.X) < 200) Interval++;

            }

            // move vertically
            if (targetPosition.Y < npc.position.Y + 200) // target above
            {
                npc.velocity.Y -= (npc.velocity.Y < 0 && npc.velocity.Y > -2) ? 0.5f : 0.7f;
            }
            else if (targetPosition.Y > npc.position.Y) // target below
            {
                if (targetPosition.Y > npc.position.Y + 200) npc.velocity.Y += (npc.velocity.Y > 0 && npc.velocity.Y < 1f) ? 0.1f : 0.15f;

                // slows acceleration when moving down to prevent npc from hitting the player
                npc.velocity.Y *= 0.9f;
            }

            // check for collision
            npc.CheckCollision();

            // move
            npc.position += npc.velocity;
        }
    }
}
