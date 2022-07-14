using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.NPCs
{
    public class Chick : ModNPC
    {
        int projectileType;
        int projectileDamage;
        float projectileSpeed;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chick");
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 32;
            npc.aiStyle = 23;
            npc.damage = 20;
            npc.defense = 10;
            npc.lifeMax = 75;
            npc.value = 50f;
            npc.friendly = false;
            npc.buffImmune[BuffID.Confused] = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Chick_Death").WithVolume(1.5f).WithPitchVariance(.3f);

            projectileType = ModContent.ProjectileType<Projectiles.FallingEggProjectile>();
            projectileDamage = npc.damage / 2;
            projectileSpeed = 5f;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 20) npc.frameCounter = 0;
            npc.frame.Y = (int)npc.frameCounter / 10 * frameHeight;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(7))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.ChickenTwinLegs>(), Main.rand.Next(1, 5));
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
                npc.Shoot(npc.Bottom, projectileType, projectileSpeed, projectileDamage);
            }
            base.AI();
        }
    }
}
