using ChickenInvadersMod.Common.Systems;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod
{
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
                    ChickenInvasionSystem.StartChickenInvasion(pos);
                    break;
            }
        }
    }

    public enum CIMessageType : byte
    {
        StartChickenInvasion
    }
}