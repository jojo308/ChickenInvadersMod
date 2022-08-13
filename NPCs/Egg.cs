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
            NPC.width = 64;
            NPC.height = 83;
            NPC.aiStyle = 23;
            NPC.damage = 20;
            NPC.defense = 10;
            NPC.lifeMax = 300;
            NPC.value = 200f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.buffImmune[BuffID.Confused] = true;
            if (!Main.dedServ)
            {
                NPC.HitSound = Mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/EggHit").WithVolume(1.5f).WithPitchVariance(.3f);
                NPC.DeathSound = Mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/EggDeath").WithVolume(1.5f).WithPitchVariance(.3f);
            }
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("EggBanner").Type;
        }

        public override void OnKill()
        {
            // if the NPC dies, it has a chance to spawn a new enemy
            if (Main.rand.NextBool(3))
            {
                // choose random chicken
                var chooser = new WeightedRandom<int>();
                chooser.Add(ModContent.NPCType<Chick>(), 0.2);
                chooser.Add(ModContent.NPCType<Chicken>(), 0.2);
                chooser.Add(ModContent.NPCType<PilotChicken>(), 0.2);
                chooser.Add(ModContent.NPCType<UfoChicken>(), 0.2);
                chooser.Add(ModContent.NPCType<Chickenaut>(), 0.1);
                int choice = chooser;

                // spawn chicken and sync for multiplayer              
                var npcIndex = NPC.NewNPC((int)NPC.position.X, (int)NPC.position.Y, choice);
                Main.npc[npcIndex].whoAmI = npcIndex;
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex, 0f, 0f, 0f, 0, 0, 0);
            }

            base.OnKill();
        }
    }
}
