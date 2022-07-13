using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod
{
    public class CIGlobalNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (CIWorld.ChickenInvasionActive && CIWorld.PlayerNearInvasion(spawnInfo.player))
            {
                pool.Clear();

                // make each wave progressively harder
                if (Main.invasionProgressWave == 1)
                {
                    pool.Add(ModContent.NPCType<NPCs.Chick>(), 3f);
                    pool.Add(ModContent.NPCType<NPCs.Chicken>(), 1f);
                }
                else if (Main.invasionProgressWave == 2)
                {
                    pool.Add(ModContent.NPCType<NPCs.Chick>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.Chicken>(), 2f);
                    pool.Add(ModContent.NPCType<NPCs.PilotChicken>(), 0.5f);
                }
                else if (Main.invasionProgressWave == 3)
                {
                    pool.Add(ModContent.NPCType<NPCs.Chicken>(), 2f);
                    pool.Add(ModContent.NPCType<NPCs.PilotChicken>(), 0.5f);
                    pool.Add(ModContent.NPCType<NPCs.UfoChicken>(), 0.25f);
                }
                else if (Main.invasionProgressWave == 4)
                {
                    pool.Add(ModContent.NPCType<NPCs.Chicken>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.PilotChicken>(), 2f);
                    pool.Add(ModContent.NPCType<NPCs.UfoChicken>(), 0.5f);
                }
                else if (Main.invasionProgressWave == 5)
                {
                    pool.Add(ModContent.NPCType<NPCs.Chicken>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.PilotChicken>(), 2f);
                    pool.Add(ModContent.NPCType<NPCs.UfoChicken>(), 0.2f);
                }
            }
        }

        public override void NPCLoot(NPC npc)
        {
            // only report kills if Chicken Invasion is active
            if (CIWorld.ChickenInvasionActive)
            {
                CIWorld.ReportKill(npc.type);
            }
        }
    }
}
