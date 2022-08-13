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
            NPC.width = 52;
            NPC.height = 60;
            NPC.damage = 35;
            NPC.defense = 15;
            NPC.lifeMax = 300;
            NPC.value = 50f;
            NPC.friendly = false;
            NPC.knockBackResist = 0.8f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Confused] = true;
            if (!Main.dedServ)
            {
                NPC.HitSound = Mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/Chicken_Hit3").WithVolume(1f).WithPitchVariance(.3f); ;
                NPC.DeathSound = Mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chicken_Death1").WithVolume(1f).WithPitchVariance(.3f);
            }
            projectileType = ModContent.ProjectileType<Projectiles.FallingEggProjectile>();
            projectileSpeed = 7f;
            projectileDamage = NPC.damage / 2;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("UfoChickenBanner").Type;
        }

        public override void OnKill()
        {
            if (Main.rand.NextBool(10))
            {
                var dropChooser = new WeightedRandom<int>();
                dropChooser.Add(ModContent.ItemType<Items.DoubleHamburger>(), 0.9);
                dropChooser.Add(ModContent.ItemType<Items.QuadHamburger>(), 0.1);
                int choice = dropChooser;
                Item.NewItem(NPC.getRect(), choice);
            }

            if (Main.rand.NextBool(500))
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.SuspiciousLookingFeather>());
            }
            if (Main.rand.NextBool(2))
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Weapons.Egg>(), Main.rand.Next(1, 5));
            }

            base.OnKill();
        }

        public override void AI()
        {
            // target nearest player
            var targetPosition = NPC.GetTargetPos();
            var velocityX = 0.2f;

            if (Interval >= 400)
            {
                Interval = 0;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ShootDown(projectileType, projectileSpeed, projectileDamage);
                }
            }

            // move horizontally
            if (targetPosition.X < NPC.Center.X && NPC.velocity.X > -3) // target is on the left
            {
                NPC.velocity.X -= velocityX; // accelerate to the left

                // target is below npc
                if ((NPC.position.X - targetPosition.X) < 200) Interval++;


            }
            else if (targetPosition.X > NPC.Center.X && NPC.velocity.X < 3) // target is on the right
            {
                NPC.velocity.X += velocityX; // accelerate to the right

                // target is below npc
                if ((targetPosition.X - NPC.position.X) < 200) Interval++;

            }

            // move vertically
            if (targetPosition.Y < NPC.position.Y + 200) // target above
            {
                NPC.velocity.Y -= (NPC.velocity.Y < 0 && NPC.velocity.Y > -2) ? 0.5f : 0.7f;
            }
            else if (targetPosition.Y > NPC.position.Y) // target below
            {
                if (targetPosition.Y > NPC.position.Y + 200) NPC.velocity.Y += (NPC.velocity.Y > 0 && NPC.velocity.Y < 1f) ? 0.1f : 0.15f;

                // slows acceleration when moving down to prevent npc from hitting the player
                NPC.velocity.Y *= 0.9f;
            }

            // check for collision
            NPC.CheckCollision();

            // move
            NPC.position += NPC.velocity;
        }
    }
}
