using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod
{
    public class ChickenInvasionSceneEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CIEvent");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override bool IsSceneEffectActive(Player player)
        {
            if (Main.gameMenu || !player.active)
            {
                return false;
            }

            // play music if the event is active and the player is near the invasion
            return CIWorld.ChickenInvasionActive && CIWorld.PlayerNearInvasion(player);
        }
    }

    public class ChickenInvadersMod : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            var messageType = (CIMessageType)reader.ReadByte();
            switch (messageType)
            {
                case CIMessageType.StartChickenInvasion:
                    var playerIndex = reader.ReadByte();
                    var pos = Main.player[playerIndex].Center;
                    CIWorld.StartChickenInvasion(pos);
                    break;
            }
        }
    }

    public enum CIMessageType : byte
    {
        StartChickenInvasion
    }
}