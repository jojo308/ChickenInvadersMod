using ChickenInvadersMod.Common;
using ChickenInvadersMod.Content.Items.Consumables;
using ChickenInvadersMod.Utilities;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Content.NPCs
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
            NPC.HitSound = SoundUtils.ChickenHit;
            NPC.DeathSound = SoundUtils.ChickenDeath;
            projectileType = ModContent.ProjectileType<Projectiles.FallingEggProjectile>();
            projectileSpeed = 7f;
            projectileDamage = NPC.damage / 2;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("UfoChickenBanner").Type;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                ModInvasions.Chickens,
                new FlavorTextBestiaryInfoElement("The UFO chicken is still learning how to control his UFO, but he's getting the hang of it."),
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Drop this item with 1% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SuspiciousLookingFeather>(), 100));

            // Drop 1-5 of this item with 33% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Egg>(), 3, 1, 5));

            // Drop some units of food
            npcLoot.Add(ItemDropRule.SequentialRules(5,
                ItemDropRule.Common(ItemID.Burger, 2),
                ItemDropRule.Common(ModContent.ItemType<DoubleHamburger>(), 2),
                ItemDropRule.Common(ModContent.ItemType<DoubleHamburger>(), 2)));
        }

        // todo spawn a falling ufo after the npc is killed
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
