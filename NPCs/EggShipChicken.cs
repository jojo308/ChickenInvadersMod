using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    class EggShipChicken : BaseChicken
    {
        int projectileType;
        int projectileDamage;
        float projectileSpeed;

        public float Degrees
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Egg Ship Chicken");
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 81;
            NPC.damage = 35;
            NPC.defense = 15;
            NPC.lifeMax = 500;
            NPC.aiStyle = 2;
            NPC.defense = 25;
            NPC.value = 100f;
            NPC.knockBackResist = 0.6f;
            NPC.friendly = false;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            projectileType = ModContent.ProjectileType<Projectiles.NeutronProjectile>();
            projectileSpeed = 10f;
            projectileDamage = NPC.damage / 2;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("EggShipChickenBanner").Type;
        }

        public override void OnKill()
        {
            if (Main.rand.NextBool(10))
            {
                var dropChooser = new WeightedRandom<int>();
                dropChooser.Add(ModContent.ItemType<Items.DoubleHamburger>(), 0.9);
                dropChooser.Add(ModContent.ItemType<Items.QuadHamburger>(), 0.01);
                int choice = dropChooser;
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), choice);
            }

            if (Main.rand.NextBool(500))
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Items.SuspiciousLookingFeather>());
            }

            if (Main.rand.NextBool(2))
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Items.Weapons.Egg>(), Main.rand.Next(1, 5));
            }

            base.OnKill();
        }

        public override void AI()
        {
            TimeLeft--;

            // ocasionally shoot 3 neutrons spaced out 120° between each other 
            if (TimeLeft <= 0)
            {
                TimeLeft = Main.rand.NextFloat(200, 600);
                Degrees = Main.rand.Next(0, 120);
                NPC.netUpdate = true;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 velocity1 = new Vector2(0, -projectileSpeed).RotatedBy(MathHelper.ToRadians(Degrees));
                    Vector2 velocity2 = new Vector2(0, -projectileSpeed).RotatedBy(MathHelper.ToRadians(Degrees + 120));
                    Vector2 velocity3 = new Vector2(0, -projectileSpeed).RotatedBy(MathHelper.ToRadians(Degrees + 240));

                    var proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity1, projectileType, projectileDamage, 0f, Main.myPlayer);
                    var proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity2, projectileType, projectileDamage, 0f, Main.myPlayer);
                    var proj3 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity3, projectileType, projectileDamage, 0f, Main.myPlayer);

                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj2);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj3);
                    SoundEngine.PlaySound(SoundUtils.Neutron, NPC.position);
                }
            }

            // make dust for engine
            var dust = DustID.MagnetSphere;
            Dust.NewDust(new Vector2(NPC.Bottom.X - 8, NPC.Bottom.Y), 16, 16, dust, NPC.velocity.X, NPC.velocity.Y);
            Dust.NewDust(new Vector2(NPC.BottomLeft.X - 10, NPC.BottomLeft.Y - 8), 16, 16, dust, NPC.velocity.X, NPC.velocity.Y);
            Dust.NewDust(new Vector2(NPC.BottomRight.X - 10, NPC.BottomRight.Y - 9), 16, 16, dust, NPC.velocity.X, NPC.velocity.Y);
        }
    }
}
