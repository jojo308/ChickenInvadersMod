using ChickenInvadersMod.Common;
using ChickenInvadersMod.Content.Items.Consumables;
using ChickenInvadersMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Content.NPCs
{
    public class Chick : BaseChicken
    {
        int projectileType;
        int projectileDamage;
        float projectileSpeed;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chick");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[2];

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, -12f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -24f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = 2;
            NPC.damage = 20;
            NPC.defense = 5;
            NPC.lifeMax = 75;
            NPC.value = 50f;
            NPC.friendly = false;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundUtils.ChickDeath;
            projectileType = ModContent.ProjectileType<Projectiles.GuanoProjectile>();
            projectileDamage = NPC.damage / 2;
            projectileSpeed = 5f;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("ChickBanner").Type;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                ModInvasions.Chickens,
                new FlavorTextBestiaryInfoElement("It's small and cute, but has a murderous look in its eyes."),
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20) NPC.frameCounter = 0;
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Drop this item with 0.5% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SuspiciousLookingFeather>(), 200));

            // Drop 1-5 of this item with 33% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Egg>(), 3, 1, 5));

            // Drop 1-5 of this item with 20% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ChickenTwinLegs>(), 5, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ItemID.ChickenNugget, 20));
        }

        public override void AI()
        {
            TimeLeft--;

            if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient && TimeLeft <= 0)
            {
                TimeLeft = Main.rand.NextFloat(400, 600);
                NPC.ShootAtPlayer(NPC.Bottom, projectileType, projectileSpeed, projectileDamage);
                NPC.netUpdate = true;
            }
        }
    }
}
