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
                pool.Add(ModContent.NPCType<NPCs.Chicken>(), 2f);
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
