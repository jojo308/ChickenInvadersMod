using ChickenInvadersMod.Common.Systems;
using ChickenInvadersMod.Content.NPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Common
{
    public class CIGlobalNPC : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (ChickenInvasionSystem.ChickenInvasionActive && ChickenInvasionSystem.PlayerNearInvasion(player))
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
            if (ChickenInvasionSystem.ChickenInvasionActive && ChickenInvasionSystem.PlayerNearInvasion(spawnInfo.Player))
            {
                pool.Clear();

                // make each wave progressively harder
                if (Main.invasionProgressWave == 1)
                {
                    pool.Add(ModContent.NPCType<Chick>(), 2f);
                    pool.Add(ModContent.NPCType<Chicken>(), 2f);
                    pool.Add(ModContent.NPCType<PilotChicken>(), .75f);
                    pool.Add(ModContent.NPCType<Barrier>(), .25f);
                }
                else if (Main.invasionProgressWave == 2)
                {
                    pool.Add(ModContent.NPCType<Chick>(), .5f);
                    pool.Add(ModContent.NPCType<Chicken>(), 1f);
                    pool.Add(ModContent.NPCType<PilotChicken>(), 1f);
                    pool.Add(ModContent.NPCType<Egg>(), 0.5f);
                    pool.Add(ModContent.NPCType<Barrier>(), 0.5f);
                    pool.Add(ModContent.NPCType<EggShipChicken>(), 0.5f);
                }
                else if (Main.invasionProgressWave == 3)
                {
                    pool.Add(ModContent.NPCType<Chicken>(), .5f);
                    pool.Add(ModContent.NPCType<PilotChicken>(), 2f);
                    pool.Add(ModContent.NPCType<Egg>(), 1f);
                    pool.Add(ModContent.NPCType<UfoChicken>(), 0.75f);
                    pool.Add(ModContent.NPCType<Barrier>(), .5f);
                    pool.Add(ModContent.NPCType<Chickenaut>(), 0.75f);
                    pool.Add(ModContent.NPCType<EggShipChicken>(), 0.75f);
                }
                else if (Main.invasionProgressWave == 4)
                {
                    pool.Add(ModContent.NPCType<PilotChicken>(), 1f);
                    pool.Add(ModContent.NPCType<Egg>(), 1.5f);
                    pool.Add(ModContent.NPCType<UfoChicken>(), 0.5f);
                    pool.Add(ModContent.NPCType<Barrier>(), 0.5f);
                    pool.Add(ModContent.NPCType<Chickenaut>(), 1f);
                    pool.Add(ModContent.NPCType<EggShipChicken>(), 1f);
                    pool.Add(ModContent.NPCType<ChickGatlingGun>(), 1f);
                }
                else if (Main.invasionProgressWave == 5)
                {
                    pool.Add(ModContent.NPCType<SuperChicken>(), 1f);
                }
            }
        }

        public override void OnKill(NPC npc)
        {
            // only report kills if Chicken Invasion is active
            if (ChickenInvasionSystem.ChickenInvasionActive)
            {
                ChickenInvasionSystem.ReportKill(npc.type);
            }
        }
    }
}
