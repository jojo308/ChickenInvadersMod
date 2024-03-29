﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ChickenInvadersMod.Content.Tiles
{
    public class Banners : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.addTile(Type);
            TileID.Sets.DisableSmartCursor[Type] = true;
            DustType = -1;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("banner");
            AddMapEntry(new Color(13, 88, 130), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int style = frameX / 18;
            string item;
            switch (style)
            {
                case 0:
                    item = "ChickBanner";
                    break;
                case 1:
                    item = "ChickenBanner";
                    break;
                case 2:
                    item = "ChickenautBanner";
                    break;
                case 3:
                    item = "EggShipChickenBanner";
                    break;
                case 4:
                    item = "PilotChickenBanner";
                    break;
                case 5:
                    item = "ChickGatlingGunBanner";
                    break;
                case 6:
                    item = "UfoChickenBanner";
                    break;
                case 7:
                    item = "BarrierBanner";
                    break;
                case 8:
                    item = "EggBanner";
                    break;
                default:
                    return;
            }
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 48, Mod.Find<ModItem>(item).Type);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Player player = Main.LocalPlayer;
                int style = Main.tile[i, j].TileFrameX / 18;
                string type;
                switch (style)
                {
                    case 0:
                        type = "Chick";
                        break;
                    case 1:
                        type = "Chicken";
                        break;
                    case 2:
                        type = "Chickenaut";
                        break;
                    case 3:
                        type = "EggShipChicken";
                        break;
                    case 4:
                        type = "PilotChicken";
                        break;
                    case 5:
                        type = "ChickGatlingGun";
                        break;
                    case 6:
                        type = "UfoChicken";
                        break;
                    case 7:
                        type = "Barrier";
                        break;
                    case 8:
                        type = "Egg";
                        break;
                    default:
                        return;
                }

                Main.SceneMetrics.NPCBannerBuff[Mod.Find<ModNPC>(type).Type] = true;
                Main.SceneMetrics.hasBanner = true;
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
    }
}
