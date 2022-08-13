using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ChickenInvadersMod
{
    public class CIWorld : ModSystem
    {
        private static readonly int MaxWaves = 5;
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

        public override void OnWorldLoad()
        {
            base.OnWorldLoad();
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
                { ModContent.NPCType<NPCs.Barrier>(), 0 },
                { ModContent.NPCType<NPCs.SuperChicken>(), 100 }
            };
        }

        public override void SaveWorldData(TagCompound tag)/* tModPorter Suggestion: Edit tag parameter instead of returning new TagCompound */
        {
            return new TagCompound
            {
                { "CIActive", ChickenInvasionActive},
                { "DownedSuperChicken", DownedSuperChicken },
                { "SpawnX", SpawnLocation.X },
                { "SpawnY", SpawnLocation.Y },
                { "Wave", Main.invasionProgressWave }
            };
        }

        public override void LoadWorldData(TagCompound tag)
        {
            ChickenInvasionActive = tag.GetBool("CIActive");
            DownedSuperChicken = tag.GetBool("DownedSuperChicken");
            var x = tag.GetFloat("SpawnX");
            var y = tag.GetFloat("SpawnY");
            SpawnLocation = new Vector2(x, y);
            Main.invasionProgressWave = tag.GetInt("Wave");
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = ChickenInvasionActive;
            flags[1] = DownedSuperChicken;
            writer.Write(flags);
            writer.WriteVector2(SpawnLocation);
            writer.Write(Main.invasionProgressWave);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ChickenInvasionActive = flags[0];
            DownedSuperChicken = flags[1];
            SpawnLocation = reader.ReadVector2();
            Main.invasionProgressWave = reader.ReadInt32();
        }

        /// <summary>
        /// Returns whether or not there is any invasion in progress
        /// </summary>
        /// <returns>True if there is an invasion in progress, false otherwise</returns>
        public static bool HasAnyInvasion()
        {
            return ChickenInvasionActive || Main.invasionType != 0;
        }

        /// <summary>
        /// Starts the chicken invasion
        /// </summary>
        /// <param name="position">The position where the invasion is happening</param>
        /// <returns>true if the invasion was started, false otherwise</returns>
        public static bool StartChickenInvasion(Vector2 position)
        {
            // do not start invasion if there already is an invasion active
            if (HasAnyInvasion()) return false;

            // todo scaling for multiplayer
            ChickenInvasionActive = true;
            SpawnLocation = position;
            Main.invasionType = -1;
            Main.invasionProgressWave = 1;
            Main.invasionSize = GetRequiredPoints();
            Main.invasionSizeStart = Main.invasionSize;
            Main.invasionProgress = 0;
            Main.invasionProgressIcon = 0;
            Main.invasionProgressMax = Main.invasionSize;
            Main.invasionProgressNearInvasion = true;
            Main.invasionX = SpawnLocation.X;
            Main.FakeLoadInvasionStart();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.WorldData);
            }

            ChatUtils.SendMessage("Chickens are invading!", ChatUtils.EventColor);
            DisplayWaveMessage(1);
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
            if (Main.netMode != NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.WorldData);

            ChatUtils.SendMessage("Chickens have been defeated, for now...", ChatUtils.EventColor);
        }

        public override void PreUpdateWorld()
        {
            if (ChickenInvasionActive) UpdateCIEvent();
        }

        /// <summary>
        /// Updated the Chicken Invasion
        /// </summary>
        private static void UpdateCIEvent()
        {
            // if wave is completed
            if (Main.invasionSize <= 0)
            {
                // stop invasion if last wave is completed
                if (Main.invasionProgressWave == MaxWaves)
                {
                    StopChickenInvasion();
                    return;
                }

                // proceed to next wave
                UpdateWave();
            }

            // report invasion progress every second so the invasion UI doesn't disappear
            if (Main.GameUpdateCount % 60 == 0)
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
            Main.invasionProgress = 0;
            Main.invasionSize = GetRequiredPoints();
            Main.invasionSizeStart = Main.invasionSize;
            Main.invasionProgressMax = Main.invasionSize;
            DisplayWaveMessage(Main.invasionProgressWave);
        }

        /// <summary>
        /// Displays the message for the specified wave
        /// </summary>
        /// <param name="wave">The current wave</param>
        private static void DisplayWaveMessage(int wave)
        {
            switch (wave)
            {
                case 1: ChatUtils.SendMessage("Wave 1: Chicks, Chickens and Pilot Chickens", ChatUtils.EventColor); break;
                case 2: ChatUtils.SendMessage("Wave 2: Chicks, Chickens, Pilot Chickens and Egg Ship Chickens", ChatUtils.EventColor); break;
                case 3: ChatUtils.SendMessage("Wave 3: Pilot Chickens, Eggs, UFO chickens, Chickenaut and Egg Ship Chickens", ChatUtils.EventColor); break;
                case 4: ChatUtils.SendMessage("Wave 4: Pilot Chickens, Eggs, UFO Chickens, Chickenauts, Egg Ship Chicken and ChickGatlingGun", ChatUtils.EventColor); break;
                default: ChatUtils.SendMessage("Wave 5: Boss battle", ChatUtils.EventColor); break;
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
            int progress = maxPoints - Main.invasionSize;

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.invasionProgressNearInvasion = PlayerNearInvasion(Main.LocalPlayer);

                if (Main.invasionProgressNearInvasion)
                {
                    Main.ReportInvasionProgress(progress, maxPoints, 0, Main.invasionProgressWave);
                }
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                foreach (Player p in Main.player)
                {
                    if (p.active && PlayerNearInvasion(p))
                    {
                        Main.invasionProgressNearInvasion = true;
                        NetMessage.SendData(MessageID.InvasionProgressReport, p.whoAmI, -1, null, progress, maxPoints, 0, Main.invasionProgressWave);
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
