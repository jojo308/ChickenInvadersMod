using Microsoft.Xna.Framework;
using Terraria;
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
            get => npc.ai[3];
            set => npc.ai[3] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Egg Ship Chicken");
        }

        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 81;
            npc.damage = 35;
            npc.defense = 15;
            npc.lifeMax = 500;
            npc.aiStyle = 2;
            npc.defense = 25;
            npc.value = 100f;
            npc.knockBackResist = 0.6f;
            npc.friendly = false;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Confused] = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            projectileType = ModContent.ProjectileType<Projectiles.NeutronProjectile>();
            projectileSpeed = 10f;
            projectileDamage = npc.damage / 2;
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
            TimeLeft--;

            // ocasionally shoot 3 neutrons spaced out 120° between each other 
            if (TimeLeft <= 0)
            {
                TimeLeft = Main.rand.NextFloat(200, 600);
                Degrees = Main.rand.Next(0, 120);
                npc.netUpdate = true;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 velocity1 = new Vector2(0, -projectileSpeed).RotatedBy(MathHelper.ToRadians(Degrees));
                    Vector2 velocity2 = new Vector2(0, -projectileSpeed).RotatedBy(MathHelper.ToRadians(Degrees + 120));
                    Vector2 velocity3 = new Vector2(0, -projectileSpeed).RotatedBy(MathHelper.ToRadians(Degrees + 240));

                    var proj = Projectile.NewProjectile(npc.Center, velocity1, projectileType, projectileDamage, 0f, Main.myPlayer);
                    var proj2 = Projectile.NewProjectile(npc.Center, velocity2, projectileType, projectileDamage, 0f, Main.myPlayer);
                    var proj3 = Projectile.NewProjectile(npc.Center, velocity3, projectileType, projectileDamage, 0f, Main.myPlayer);

                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj2);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj3);

                    if (!Main.dedServ)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Neutron").WithVolume(3f).WithPitchVariance(.3f), npc.position);
                    }
                }
            }

            // make dust for engine
            var dust = DustID.MagnetSphere;
            Dust.NewDust(new Vector2(npc.Bottom.X - 8, npc.Bottom.Y), 16, 16, dust, npc.velocity.X, npc.velocity.Y);
            Dust.NewDust(new Vector2(npc.BottomLeft.X - 10, npc.BottomLeft.Y - 8), 16, 16, dust, npc.velocity.X, npc.velocity.Y);
            Dust.NewDust(new Vector2(npc.BottomRight.X - 10, npc.BottomRight.Y - 9), 16, 16, dust, npc.velocity.X, npc.velocity.Y);
        }
    }
}
