using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ChickenInvadersMod
{
    public class CIWorld : ModWorld
    {
        private static readonly int MaxWaves = 5;
        private static Color DefaultColor = Color.MediumPurple;

        public static bool ChickenInvasionActive = false;
        public static bool DownedSuperChicken = false;
        public static Vector2 SpawnLocation;

        // The required amount of points for each wave
        public class RequiredPoints
        {
            public const int Wave1 = 25;
            public const int Wave2 = 40;
            public const int Wave3 = 60;
            public const int Wave4 = 80;
            public const int Wave5 = 100;
        }

        public static int GetRequiredPoints()
        {
            var requiredPoints = 0;
            switch (Main.invasionProgressWave)
            {
                case 1:
                    requiredPoints = RequiredPoints.Wave1;
                    break;
                case 2:
                    requiredPoints = RequiredPoints.Wave2;
                    break;
                case 3:
                    requiredPoints = RequiredPoints.Wave3;
                    break;
                case 4:
                    requiredPoints = RequiredPoints.Wave4;
                    break;
                case 5:
                    requiredPoints = RequiredPoints.Wave5;
                    break;
            }

            return requiredPoints;
        }

        // the enemies that belong to the Chicken Invasion
        public static Dictionary<int, int> Enemies;

        public override void Initialize()
        {
            base.Initialize();
            ChickenInvasionActive = false;
            DownedSuperChicken = false;

            Enemies = new Dictionary<int, int>
            {
                { ModContent.NPCType<NPCs.Chick>(), 1 },
                { ModContent.NPCType<NPCs.Chicken>(), 2 },
                { ModContent.NPCType<NPCs.PilotChicken>(), 3 },
                { ModContent.NPCType<NPCs.UfoChicken>(), 3 },
                { ModContent.NPCType<NPCs.Chickenaut>(), 5 },
                { ModContent.NPCType<NPCs.ChickGatlingGun>(), 8 },
                { ModContent.NPCType<NPCs.EggShipChicken>(), 7 },
                { ModContent.NPCType<NPCs.Egg>(), 2 },
                { ModContent.NPCType<NPCs.Barrier>(), 1 },
                { ModContent.NPCType<NPCs.SuperChicken>(), 100 }
            };
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                { "CIActive", ChickenInvasionActive},
                { "DownedSuperChicken", DownedSuperChicken },
                { "SpawnX", SpawnLocation.X },
                { "SpawnY", SpawnLocation.Y }
            };
        }

        public override void Load(TagCompound tag)
        {
            ChickenInvasionActive = tag.GetBool("CIActive");
            DownedSuperChicken = tag.GetBool("DownedSuperChicken");
            var x = tag.GetFloat("SpawnX");
            var y = tag.GetFloat("SpawnY");
            SpawnLocation = new Vector2(x, y);
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = ChickenInvasionActive;
            flags[1] = DownedSuperChicken;
            writer.Write(flags);
            writer.WriteVector2(SpawnLocation);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ChickenInvasionActive = flags[0];
            DownedSuperChicken = flags[1];
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
            Main.invasionProgressWave = 1;
            Main.invasionSize = GetRequiredPoints();
            Main.invasionSizeStart = Main.invasionSize;
            Main.invasionProgress = 0;
            Main.invasionProgressIcon = 0;
            Main.invasionProgressMax = Main.invasionSizeStart;
            Main.invasionProgressNearInvasion = true;
            Main.invasionX = SpawnLocation.X;
            Main.FakeLoadInvasionStart();
            ChickenInvasionActive = true;
            SpawnLocation = position;
            ChatUtils.SendMessage("Chickens are invading!", DefaultColor);
            return true;
        }

        /// <summary>
        /// Stops the Chicken Invasion
        /// </summary>
        public static void StopChickenInvasion()
        {
            if (!ChickenInvasionActive) return;

            ChickenInvasionActive = false;
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
            Main.invasionProgressNearInvasion = PlayerNearInvasion(Main.LocalPlayer);

            if (Main.invasionSize <= 0)
            {
                if (Main.invasionProgressWave == MaxWaves)
                {
                    StopChickenInvasion();
                    return;
                }
                UpdateWave();
            }

            if (Main.invasionProgressNearInvasion)
            {
                ReportInvasionProgress();
            }
        }

        /// <summary>
        /// Starts the new wave
        /// </summary>
        private static void UpdateWave()
        {
            Main.invasionProgressWave++;
            Main.invasionSize = GetRequiredPoints();
            Main.invasionProgressMax = Main.invasionSize;

            switch (Main.invasionProgressWave)
            {
                case 1: ChatUtils.SendMessage("Wave 1: Chicks, Chickens and Pilot Chickens", DefaultColor); break;
                case 2: ChatUtils.SendMessage("Wave 2: Chicks, Chickens, Pilot Chickens and Egg Ship Chickens", DefaultColor); break;
                case 3: ChatUtils.SendMessage("Wave 3: Pilot Chickens, Eggs, UFO chickens, Chickenaut and Egg Ship Chickens", DefaultColor); break;
                case 4: ChatUtils.SendMessage("Wave 4: Pilot Chickens, Eggs, UFO Chickens, Chickenauts, Egg Ship Chicken and ChickGatlingGun", DefaultColor); break;
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
            var pos1 = new Vector2(player.position.X - 2400, SpawnLocation.Y);
            var pos2 = new Vector2(player.position.X + 2400, SpawnLocation.Y);
            return SpawnLocation.Between(pos1, pos2);
        }

        /// <summary>
        /// Reports the progress of the Chicken Invasion
        /// </summary>
        public static void ReportInvasionProgress()
        {
            var maxPoints = GetRequiredPoints();
            int progress = Main.invasionProgressMax - Main.invasionSize;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (ChickenInvasionActive)
                {
                    Main.ReportInvasionProgress(progress, maxPoints, 0, Main.invasionProgressWave);
                }
            }
            else
            {
                // syncing for multiplayer
                foreach (Player p in Main.player)
                {
                    if (ChickenInvasionActive && PlayerNearInvasion(p))
                    {
                        NetMessage.SendData(MessageID.InvasionProgressReport, p.whoAmI, -1, null, progress, maxPoints, 0, Main.invasionProgressWave, 0, 0, 0);
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
            foreach (int enemy in Enemies.Keys)
            {
                if (npc == enemy)
                {
                    Enemies.TryGetValue(enemy, out int value);
                    Main.invasionSize -= value;
                    if (Main.invasionSize < 0) Main.invasionSize = 0;
                    ReportInvasionProgress();
                    break;
                }
            }
        }
    }
}
