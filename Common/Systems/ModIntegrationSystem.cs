using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Common.Systems
{
    // This was made with help from the Example Mod https://github.com/tModLoader/tModLoader/blob/1.4/ExampleMod/Common/Systems/ModIntegrationsSystem.cs
    public class BossChecklistIntegration : ModSystem
    {
        public override void PostSetupContent()
        {
            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                return;
            }

            // For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod, in this case 1.3.1
            // Usually mods either provide that information themselves in some way, or it's found on the github through commit history/blame
            if (bossChecklistMod.Version < new Version(1, 3, 1))
            {
                return;
            }

            AddChickenInvasion(bossChecklistMod);
            AddSuperChickenBoss(bossChecklistMod);
        }

        private Action<SpriteBatch, Rectangle, Color> GetDrawing(string name) => (SpriteBatch sb, Rectangle rect, Color color) =>
        {
            Texture2D texture = ModContent.Request<Texture2D>(name).Value;
            Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
            sb.Draw(texture, centered, color);
        };

        private void AddChickenInvasion(Mod bossChecklist)
        {
            string eventName = "Chicken Invasion";

            var enemies = new List<int>() {
                ModContent.NPCType<Content.NPCs.Chick>(),
                ModContent.NPCType<Content.NPCs.Chicken>(),
                ModContent.NPCType<Content.NPCs.PilotChicken>(),
                ModContent.NPCType<Content.NPCs.Barrier>(),
                ModContent.NPCType<Content.NPCs.Egg>(),
                ModContent.NPCType<Content.NPCs.EggShipChicken>(),
                ModContent.NPCType<Content.NPCs.UfoChicken>(),
                ModContent.NPCType<Content.NPCs.Chickenaut>(),
                ModContent.NPCType<Content.NPCs.ChickGatlingGun>(),
                ModContent.NPCType<Content.NPCs.SuperChicken>()
            };

            float weight = 7.68f;
            Func<bool> downed = () => ChickenInvasionSystem.DownedSuperChicken;
            Func<bool> available = () => true;
            var collection = new List<int>();
            var summonItem = ModContent.ItemType<Content.Items.Consumables.SuspiciousLookingFeather>();
            string spawnInfo = $"Use a [i:{summonItem}], which is dropped from a chicken during hardmode near floating islands. Lasts unitl all waves are cleared";
            var drawing = GetDrawing("ChickenInvadersMod/Assets/Textures/EventChickenInvasion");
            var icon = new List<string>() { "ChickenInvadersMod/icon_small" };

            bossChecklist.Call("AddEvent", Mod, eventName, enemies, weight, downed, available, collection, summonItem, spawnInfo, drawing, icon);

        }

        private void AddSuperChickenBoss(Mod bossChecklist)
        {
            string bossName = "Super Chicken";
            int bossType = ModContent.NPCType<Content.NPCs.SuperChicken>();
            float weight = 7.69f;
            Func<bool> downed = () => ChickenInvasionSystem.DownedSuperChicken;
            Func<bool> available = () => true;
            var collection = new List<int>();
            int summonItem = 0;
            string spawnInfo = "Spawns on wave 5, the final wave, of the Chicken invasion";

            bossChecklist.Call("AddBoss", Mod, bossName, bossType, weight, downed, available, collection, summonItem, spawnInfo, null, null);
        }
    }
}
