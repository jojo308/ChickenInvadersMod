using ChickenInvadersMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Content
{
    public class ChickenInvasionSceneEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/CIEvent");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override bool IsSceneEffectActive(Player player)
        {
            if (Main.gameMenu || !player.active)
            {
                return false;
            }

            // play music if the event is active and the player is near the invasion
            return ChickenInvasionSystem.ChickenInvasionActive && ChickenInvasionSystem.PlayerNearInvasion(player);
        }
    }
}
