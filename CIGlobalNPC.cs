using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod
{
    public class CIGlobalNPC : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (CIWorld.ChickenInvasionActive && CIWorld.PlayerNearInvasion(player))
            {
                // do not spawn any NPCs if the boss has spawned
                if (NPC.AnyNPCs(Mod.Find<ModNPC>("SuperChicken").Type))
                {
                    spawnRate = 0;
                    maxSpawns = 0;
                }
                else
                {
                    spawnRate = 30;
                    maxSpawns = 10;
                }
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (CIWorld.ChickenInvasionActive && CIWorld.PlayerNearInvasion(spawnInfo.Player))
            {
                pool.Clear();

                // make each wave progressively harder
                if (Main.invasionProgressWave == 1)
                {
                    pool.Add(ModContent.NPCType<NPCs.Chick>(), 2f);
                    pool.Add(ModContent.NPCType<NPCs.Chicken>(), 2f);
                    pool.Add(ModContent.NPCType<NPCs.PilotChicken>(), .75f);
                    pool.Add(ModContent.NPCType<NPCs.Barrier>(), .25f);
                }
                else if (Main.invasionProgressWave == 2)
                {
                    pool.Add(ModContent.NPCType<NPCs.Chick>(), .5f);
                    pool.Add(ModContent.NPCType<NPCs.Chicken>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.PilotChicken>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.Egg>(), 0.5f);
                    pool.Add(ModContent.NPCType<NPCs.Barrier>(), 0.5f);
                    pool.Add(ModContent.NPCType<NPCs.EggShipChicken>(), 0.5f);
                }
                else if (Main.invasionProgressWave == 3)
                {
                    pool.Add(ModContent.NPCType<NPCs.Chicken>(), .5f);
                    pool.Add(ModContent.NPCType<NPCs.PilotChicken>(), 2f);
                    pool.Add(ModContent.NPCType<NPCs.Egg>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.UfoChicken>(), 0.75f);
                    pool.Add(ModContent.NPCType<NPCs.Barrier>(), .5f);
                    pool.Add(ModContent.NPCType<NPCs.Chickenaut>(), 0.75f);
                    pool.Add(ModContent.NPCType<NPCs.EggShipChicken>(), 0.75f);
                }
                else if (Main.invasionProgressWave == 4)
                {
                    pool.Add(ModContent.NPCType<NPCs.PilotChicken>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.Egg>(), 1.5f);
                    pool.Add(ModContent.NPCType<NPCs.UfoChicken>(), 0.5f);
                    pool.Add(ModContent.NPCType<NPCs.Barrier>(), 0.5f);
                    pool.Add(ModContent.NPCType<NPCs.Chickenaut>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.EggShipChicken>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.ChickGatlingGun>(), 1f);
                }
                else if (Main.invasionProgressWave == 5)
                {
                    pool.Add(ModContent.NPCType<NPCs.SuperChicken>(), 1f);
                }
            }
        }

        public override void OnKill(NPC npc)
        {
            // only report kills if Chicken Invasion is active
            if (CIWorld.ChickenInvasionActive)
            {
                CIWorld.ReportKill(npc.type);
            }
        }
    }
}
