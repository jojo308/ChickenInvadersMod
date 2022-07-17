using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    class EggShipChicken : ModNPC
    {
        int projectileType;
        int projectileDamage;
        float projectileSpeed;

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
            npc.knockBackResist = 0.5f;
            npc.friendly = false;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Confused] = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            projectileType = ModContent.ProjectileType<Projectiles.NeutronProjectile>();
            projectileSpeed = 7f;
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
            npc.ai[0]--;

            // ocasionally shoot 3 neutrons spaced out 120° between each other 
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[0] <= 0)
            {
                npc.ai[0] = Main.rand.NextFloat(200, 600);
                var degrees = Main.rand.Next(0, 120);
                Vector2 velocity1 = new Vector2(0, -projectileSpeed).RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 velocity2 = new Vector2(0, -projectileSpeed).RotatedBy(MathHelper.ToRadians(degrees + 120));
                Vector2 velocity3 = new Vector2(0, -projectileSpeed).RotatedBy(MathHelper.ToRadians(degrees + 240));

                Projectile.NewProjectile(npc.Center, velocity1, projectileType, projectileDamage, 0f, Main.myPlayer);
                Projectile.NewProjectile(npc.Center, velocity2, projectileType, projectileDamage, 0f, Main.myPlayer);
                Projectile.NewProjectile(npc.Center, velocity3, projectileType, projectileDamage, 0f, Main.myPlayer);

                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Neutron").WithVolume(3f).WithPitchVariance(.3f), npc.position);
            }

            npc.velocity *= 0.98f;

            // make dust for engine
            var dust = DustID.MagnetSphere;
            Dust.NewDust(new Vector2(npc.Bottom.X - 8, npc.Bottom.Y), 16, 16, dust, npc.velocity.X, npc.velocity.Y);
            Dust.NewDust(new Vector2(npc.BottomLeft.X - 10, npc.BottomLeft.Y - 8), 16, 16, dust, npc.velocity.X, npc.velocity.Y);
            Dust.NewDust(new Vector2(npc.BottomRight.X - 10, npc.BottomRight.Y - 9), 16, 16, dust, npc.velocity.X, npc.velocity.Y);

            // check for collision                      
            npc.CheckCollision();

            // move
            npc.position += npc.velocity;
        }

        public static Vector2 Rotate(Vector2 v, float delta)
        {
            return new Vector2(
                (float)(v.X * Math.Cos(delta) - v.Y * Math.Sin(delta)),
                (float)(v.X * Math.Sin(delta) + v.Y * Math.Cos(delta))
            );
        }
    }
}
