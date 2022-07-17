using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.NPCs
{
    public class Egg : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Egg");
        }

        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 83;
            npc.aiStyle = 23;
            npc.damage = 20;
            npc.defense = 10;
            npc.lifeMax = 300;
            npc.value = 200f;
            npc.noGravity = true;
            npc.friendly = false;
            npc.buffImmune[BuffID.Confused] = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/EggHit").WithVolume(1.5f).WithPitchVariance(.3f);
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/EggDeath").WithVolume(1.5f).WithPitchVariance(.3f);
        }

        public override void NPCLoot()
        {
            // if the NPC dies, it has a chance to spawn a new enemy
            if (Main.rand.NextBool(3))
            {
                var dropChooser = new WeightedRandom<int>();
                dropChooser.Add(ModContent.NPCType<Chick>(), 0.2);
                dropChooser.Add(ModContent.NPCType<Chicken>(), 0.2);
                dropChooser.Add(ModContent.NPCType<PilotChicken>(), 0.2);
                dropChooser.Add(ModContent.NPCType<UfoChicken>(), 0.2);
                dropChooser.Add(ModContent.NPCType<Chickenaut>(), 0.1);
                int choice = dropChooser;
                NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, choice);
            }

            base.NPCLoot();
        }
    }
}
