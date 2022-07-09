using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod
{
    public class CIGlobalNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            // todo different weights for each wave
            if (CIWorld.ChickenInvasionActive && CIWorld.PlayerNearInvasion(spawnInfo.player))
            {               
                pool.Clear();
                pool.Add(ModContent.NPCType<NPCs.Chicken>(), 2f);
                pool.Add(ModContent.NPCType<NPCs.PilotChicken>(), 0.5f);
            }
        }

        public override void NPCLoot(NPC npc)
        {
            if (CIWorld.ChickenInvasionActive)
            {
                CIWorld.ReportKill(npc.type);
            }
        }
    }
}
