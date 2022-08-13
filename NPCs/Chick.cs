using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.NPCs
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
            if (!Main.dedServ)
            {
                NPC.DeathSound = Mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chick_Death").WithVolume(1.5f).WithPitchVariance(.3f);
            }
            projectileType = ModContent.ProjectileType<Projectiles.GuanoProjectile>();
            projectileDamage = NPC.damage / 2;
            projectileSpeed = 5f;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("ChickBanner").Type;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20) NPC.frameCounter = 0;
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }

        public override void OnKill()
        {
            if (Main.rand.NextBool(7))
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.ChickenTwinLegs>(), Main.rand.Next(1, 5));
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
