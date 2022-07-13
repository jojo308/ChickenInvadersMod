using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ChickenInvadersMod
{
    public class CIWorld : ModWorld
    {
        private static int WaveRequiredKillCount = 50;
        private static int MaxWaves = 5;
        private static Color DefaultColor = Color.MediumPurple;

        public static bool ChickenInvasionActive = false;
        public static bool DownedBoss = false;
        public static Vector2 SpawnLocation;

        // the enemies that belong to the Chicken Invasion
        public static int[] Enemies;

        public override void Initialize()
        {
            base.Initialize();
            ChickenInvasionActive = false;
            DownedBoss = false;
            Enemies = new int[] {
                ModContent.NPCType<NPCs.Chick>(),
                ModContent.NPCType<NPCs.Chicken>(),
                ModContent.NPCType<NPCs.PilotChicken>(),
                ModContent.NPCType<NPCs.UfoChicken>(),
            };
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                { "CIActive", ChickenInvasionActive},
                { "DownedBoss", DownedBoss },
                { "SpawnX", SpawnLocation.X },
                { "SpawnY", SpawnLocation.Y }
            };
        }

        public override void Load(TagCompound tag)
        {
            ChickenInvasionActive = tag.GetBool("CIActive");
            DownedBoss = tag.GetBool("DownedBoss");
            var x = tag.GetFloat("SpawnX");
            var y = tag.GetFloat("SpawnY");
            SpawnLocation = new Vector2(x, y);
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = ChickenInvasionActive;
            flags[1] = DownedBoss;
            writer.Write(flags);
            writer.WriteVector2(SpawnLocation);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ChickenInvasionActive = flags[0];
            DownedBoss = flags[1];
            SpawnLocation = reader.ReadVector2();
        }

        /// <summary>
        /// Starts the chicken invasion
        /// </summary>
        /// <param name="position">The position where the invasion is happening</param>
        /// <returns>true if the invasion was started, false otherwise</returns>

        public static bool StartChickenInvasion(Vector2 position)
        {
            if (ChickenInvasionActive && Main.invasionType == -1 || Main.invasionType != 0)
            {
                ChatUtils.SendMessage($"An invasion is already in progress ({Main.invasionType})", Color.White);
                return false;
            }

            // todo scaling for multiplayer
            Main.invasionType = -1;
            Main.invasionSize = WaveRequiredKillCount;
            Main.invasionSizeStart = Main.invasionSize;
            Main.invasionProgress = 0;
            Main.invasionProgressIcon = 0;
            Main.invasionProgressWave = 1;
            Main.invasionProgressMax = Main.invasionSizeStart;
            Main.invasionProgressNearInvasion = true;
            Main.invasionX = SpawnLocation.X;
            Main.FakeLoadInvasionStart();
            ChickenInvasionActive = true;
            DownedBoss = false;
            SpawnLocation = position;
            ChatUtils.SendMessage("Chickens are invading!", DefaultColor);
            return true;
        }

        /// <summary>
        /// Stops the Chicken Invasion
        /// </summary>
        public static void StopChickenInvasion()
        {
            if (!ChickenInvasionActive)
            {
                return;
            }

            ChickenInvasionActive = false;
            DownedBoss = false;
            Main.invasionType = 0;

            // inform server about new state
            if (Main.netMode == NetmodeID.Server) NetMessage.SendData(MessageID.WorldData);

            ChatUtils.SendMessage("Chickens have been defeated, for now...", DefaultColor);
        }

        public override void PreUpdate()
        {
            if (ChickenInvasionActive) UpdateCIEvent();
        }

        /// <summary>
        /// Updated the Chicken Invasion
        /// </summary>
        private static void UpdateCIEvent()
        {
            var isNear = PlayerNearInvasion(Main.LocalPlayer);
            Main.invasionProgressNearInvasion = isNear;

            if (isNear)
            {
                ReportInvasionProgress();
            }

            if (Main.invasionSize <= 0)
            {
                if (Main.invasionProgressWave == MaxWaves)
                {
                    StopChickenInvasion();
                    return;
                }
                UpdateWave();
            }
        }

        /// <summary>
        /// Starts the new wave
        /// </summary>
        private static void UpdateWave()
        {
            Main.invasionSize = WaveRequiredKillCount;
            Main.invasionSizeStart = Main.invasionSize;
            Main.invasionProgress = 0;
            Main.invasionProgressWave++;
            Main.invasionProgressMax = Main.invasionSizeStart;

            switch (Main.invasionProgressWave)
            {
                case 1: ChatUtils.SendMessage("Wave 1: Chicks and chickens", DefaultColor); break;
                case 2: ChatUtils.SendMessage("Wave 2: Chicks, chickens and Pilot chickens", DefaultColor); break;
                case 3: ChatUtils.SendMessage("Wave 3: Chickens, Pilot chickens and UFO chickens", DefaultColor); break;
                case 4: ChatUtils.SendMessage("Wave 4: Chicken Overload", DefaultColor); break;
                default: ChatUtils.SendMessage("Wave 5: Boss battle", DefaultColor); break;
            }
        }

        /// <summary>
        /// Checks if the given player is near the invasion
        /// </summary>
        /// <param name="player">the player to be checked</param>
        /// <returns>true if the player is near the invasion, false otherwise</returns>
        public static bool PlayerNearInvasion(Player player)
        {
            var pos1 = new Vector2(player.position.X - 1200, SpawnLocation.Y);
            var pos2 = new Vector2(player.position.X + 1200, SpawnLocation.Y);
            return SpawnLocation.Between(pos1, pos2);
        }

        /// <summary>
        /// Reports the progress of the Chicken Invasion
        /// </summary>
        public static void ReportInvasionProgress()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                if (ChickenInvasionActive)
                {
                    Main.ReportInvasionProgress(WaveRequiredKillCount - Main.invasionSize, 50, 0, Main.invasionProgressWave);
                }
            }
            else
            {
                // syncing for multiplayer
                foreach (Player p in Main.player)
                {
                    if (PlayerNearInvasion(p))
                    {
                        NetMessage.SendData(MessageID.InvasionProgressReport, p.whoAmI, -1, null, WaveRequiredKillCount - Main.invasionSize, 50, 0, Main.invasionProgressWave, 0, 0, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Reports the killed NPC. If the npc was part of the event, updates the progress
        /// </summary>
        /// <param name="npc">The NPC that was killed</param>
        public static void ReportKill(int npc)
        {
            foreach (int enemy in Enemies)
            {
                if (npc == enemy)
                {
                    Main.invasionSize--;
                    ReportInvasionProgress();
                    break;
                }
            }
        }
    }
}
