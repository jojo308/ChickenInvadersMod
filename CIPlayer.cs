using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod
{
    public class CIPlayer : ModPlayer
    {
        // not using thos right now, the GlobalNPC should handle spawnrate/spawnpool
        //public override void PostUpdate()
        //{
            //const int XOffset = 1200;

            //if (CIWorld.ChickenInvasionActive && CIWorld.PlayerNearInvasion(player))
            //{
            //    if (Main.rand.NextBool(300))
            //        NPC.NewNPC((int)player.Center.X + XOffset, (int)player.Center.Y, ModContent.NPCType<NPCs.Chicken>());
            //    if (Main.rand.NextBool(300))
            //        NPC.NewNPC((int)player.Center.X - XOffset, (int)player.Center.Y, ModContent.NPCType<NPCs.Chicken>());
            //    // todo more enemies
            //}
        //}
    }
}
