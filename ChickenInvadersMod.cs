using Microsoft.Xna.Framework.Audio;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace ChickenInvadersMod
{
    public class ChickenInvadersMod : Mod
    {
        public override void UpdateMusic(ref int music, ref SceneEffectPriority priority)/* tModPorter Note: Removed. Use ModSceneEffect.Music and .Priority, aswell as ModSceneEffect.IsSceneEffectActive */
        {
            if (Main.gameMenu || Main.myPlayer == -1 || !Main.LocalPlayer.active)
            {
                return;
            }

            // change the music if an Chicken Invasion is active and the player is near it
            if (CIWorld.ChickenInvasionActive && CIWorld.PlayerNearInvasion(Main.LocalPlayer))
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/CIEvent");
                priority = SceneEffectPriority.Event;
            }
        }

        public override void Close()
        {
            var slots = new int[] {
                GetSoundSlot(SoundType.Music, "Sounds/Music/CIEvent")
            };

            // stop music. Mod might crash if music is not properly closed
            foreach (var slot in slots)
            {
                if (Main.music.IndexInRange(slot) && Main.music[slot]?.IsPlaying == true)
                {
                    Main.music[slot].Stop(AudioStopOptions.Immediate);
                }
            }
            base.Close();
        }

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