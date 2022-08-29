using ChickenInvadersMod.Common;
using ChickenInvadersMod.Content.Items.Consumables;
using ChickenInvadersMod.Utilities;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ChickenInvadersMod.Content.NPCs
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
            NPC.HitSound = SoundUtils.EggHit;
            NPC.DeathSound = SoundUtils.EggDeath;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("EggBanner").Type;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                ModInvasions.Chickens,
                new FlavorTextBestiaryInfoElement("It may or may not contain a nasty surprise."),
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Drop this item with 1% chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SuspiciousLookingFeather>(), 100));
        }

        public override void OnKill()
        {
            // if the NPC dies, it has a 1/3 chance to spawn a new enemy
            if (Main.rand.NextBool(3))
            {
                // Add chickens to be spawned depending on the current wave
                var choice = new WeightedRandom<int>();
                choice.Add(ModContent.NPCType<Chick>(), 2);
                choice.Add(ModContent.NPCType<Chicken>(), 2);
                choice.Add(ModContent.NPCType<PilotChicken>(), 2);

                if (Main.invasionProgressWave >= 2)
                {
                    choice.Add(ModContent.NPCType<EggShipChicken>(), 2);

                }
                if (Main.invasionProgressWave >= 3)
                {
                    choice.Add(ModContent.NPCType<Chickenaut>(), 2);
                    choice.Add(ModContent.NPCType<UfoChicken>(), 2);
                }

                if (Main.invasionProgressWave >= 4)
                {
                    choice.Add(ModContent.NPCType<ChickGatlingGun>(), 2);
                }

                // spawn chicken and sync for multiplayer                
                var npcIndex = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)NPC.position.Y, choice);
                Main.npc[npcIndex].whoAmI = npcIndex;
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex, 0f, 0f, 0f, 0, 0, 0);
            }

            base.OnKill();
        }
    }
}
