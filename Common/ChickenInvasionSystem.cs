using ChickenInvadersMod.Content.NPCs;
using ChickenInvadersMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace ChickenInvadersMod.Common
{
    public class ChickenInvasionSystem : ModSystem
    {
        public static bool ChickenInvasionActive = false;
        public static bool DownedSuperChicken = false;
        public static Vector2 SpawnLocation;
        public static int[] RequiredPoints = new int[6] { 0, 25, 40, 60, 80, 100 };
        public static Dictionary<int, int> Enemies;

        /// <summary>
        /// Returns the points needed for the specified wave
        /// </summary>
        /// <param name="wave">The current wave</param>
        /// <returns>The points required for the specified wave</returns>
        public static int GetPointsForWave(int wave)
        {
            var playerCount = GetPlayerCount();
            var extra1 = Main.expertMode ? 10 : 0;
            var extra2 = Main.masterMode ? 15 : 0;
            var extra3 = playerCount > 1 ? playerCount * 5 : 0;
            return RequiredPoints[wave] + extra1 + extra2 + extra3;
        }

        /// <summary>
        /// Returns the player count
        /// </summary>
        /// <returns>The player count</returns>
        public static int GetPlayerCount()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                return 1;
            }

            int count = 0;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var p = Main.player[i];
                if (p.active)
                    count++;
            }
            return count;
        }

        public override void OnWorldLoad()
        {
            base.OnWorldLoad();
            ChickenInvasionActive = false;
            DownedSuperChicken = false;

            Enemies = new Dictionary<int, int>
            {
                { ModContent.NPCType<Chick>(), 1 },
                { ModContent.NPCType<Chicken>(), 2 },
                { ModContent.NPCType<PilotChicken>(), 3 },
                { ModContent.NPCType<UfoChicken>(), 3 },
                { ModContent.NPCType<Chickenaut>(), 5 },
                { ModContent.NPCType<ChickGatlingGun>(), 8 },
                { ModContent.NPCType<EggShipChicken>(), 7 },
                { ModContent.NPCType<Egg>(), 2 },
                { ModContent.NPCType<Barrier>(), 0 },
                { ModContent.NPCType<SuperChicken>(), 100 }
            };
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["CIActive"] = ChickenInvasionActive;
            tag["DownedSuperChicken"] = DownedSuperChicken;

            if (ChickenInvasionActive)
            {
                tag["SpawnX"] = SpawnLocation.X;
                tag["SpawnY"] = SpawnLocation.Y;
                tag["Wave"] = Main.invasionProgressWave;
                tag["WaveMax"] = Main.invasionProgressMax;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            ChickenInvasionActive = tag.GetBool("CIActive");
            DownedSuperChicken = tag.GetBool("DownedSuperChicken");

            if (ChickenInvasionActive)
            {
                var x = tag.GetFloat("SpawnX");
                var y = tag.GetFloat("SpawnY");
                SpawnLocation = new Vector2(x, y);
                Main.invasionProgressWave = tag.GetInt("Wave");
                Main.invasionProgressMax = tag.GetInt("WaveMax");
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = ChickenInvasionActive;
            flags[1] = DownedSuperChicken;
            writer.Write(flags);

            if (ChickenInvasionActive)
            {
                writer.WriteVector2(SpawnLocation);
                writer.Write(Main.invasionProgressWave);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ChickenInvasionActive = flags[0];
            DownedSuperChicken = flags[1];

            if (ChickenInvasionActive)
            {
                SpawnLocation = reader.ReadVector2();
                Main.invasionProgressWave = reader.ReadInt32();
            }
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

            ChickenInvasionActive = true;
            SpawnLocation = position;
            Main.invasionType = -1;
            Main.invasionProgressWave = 1;
            Main.invasionSize = GetPointsForWave(1);
            Main.invasionSizeStart = Main.invasionSize;
            Main.invasionProgress = 0;
            Main.invasionProgressIcon = 0;
            Main.invasionProgressMax = Main.invasionSize;
            Main.invasionProgressNearInvasion = true;
            Main.invasionX = SpawnLocation.X;

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
            Main.invasionSize = 0;

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
                if (Main.invasionProgressWave == 5)
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
            Main.invasionSize = GetPointsForWave(Main.invasionProgressWave);
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
            var maxPoints = Main.invasionProgressMax;
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

        internal ChickenInvasionProgressBar InvasionProgressBar;
        private UserInterface InvasionProgressBarInterface;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                InvasionProgressBar = new ChickenInvasionProgressBar();
                InvasionProgressBar.Activate();
                InvasionProgressBarInterface = new UserInterface();
                InvasionProgressBarInterface.SetState(InvasionProgressBar);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            InvasionProgressBarInterface?.Update(gameTime);
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                InvasionProgressBarInterface?.SetState(null);
                InvasionProgressBar = null;
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Invasion Progress Bars"));
            if (index != -1 && index < layers.Count)
            {
                // insert the custom bar AFTER the vanilla bar since we want to draw on top of it and not behind               
                layers.Insert(index + 1, new LegacyGameInterfaceLayer(
                    "Chicken Invaders Mod: Test",
                    delegate
                    {
                        // this will draw the invasion icon
                        InvasionProgressBarInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }

    class ChickenInvasionProgress : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.invasionProgressAlpha <= 0f)
            {
                return;
            }

            if (ChickenInvasionSystem.ChickenInvasionActive && !Main.gameMenu || Main.invasionProgressAlpha > 0)
            {
                Texture2D icon = (Texture2D)ModContent.Request<Texture2D>("ChickenInvadersMod/icon_small");
                string text = "Chicken Invasion";
                float scale = 0.5f + Main.invasionProgressAlpha * 0.5f;
                Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text);
                float offset = textSize.X > 200f ? textSize.X - 200f : 120f;
                Rectangle rect = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offset, Main.screenHeight - 80), (textSize + new Vector2(icon.Width + 12, 6f)) * scale);

                // draws the rectangle
                Utils.DrawInvBG(spriteBatch, rect, new Color(165, 160, 155));
                // draws the icon
                spriteBatch.Draw(icon, rect.Left() + Vector2.UnitX * scale * 8f, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2(0f, icon.Height / 2), scale * 0.8f, SpriteEffects.None, 0f);
                // draws the text
                Utils.DrawBorderString(spriteBatch, text, rect.Right() + Vector2.UnitX * scale * -22f, Color.White * Main.invasionProgressAlpha, scale * 0.9f, 1f, 0.4f);
            }
        }
    }

    class ChickenInvasionProgressBar : UIState
    {
        public ChickenInvasionProgress chickenInvasionProgress;

        public override void OnInitialize()
        {
            chickenInvasionProgress = new ChickenInvasionProgress();
            Append(chickenInvasionProgress);
        }
    }
}
