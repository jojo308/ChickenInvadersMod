using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChickenInvadersMod.Items
{
    public class SuspiciousLookingFeather : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Suspicious looking feather");
            Tooltip.SetDefault("Starts a Chicken Invasion");
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.width = 12;
            Item.height = 15;
            Item.useTime = 32;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = false;
            Item.maxStack = 1;
            Item.UseSound = SoundID.Item1;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            if (CIWorld.HasAnyInvasion()) return null;

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                CIWorld.StartChickenInvasion(player.Center);
                return true;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)CIMessageType.StartChickenInvasion);
                packet.Write((byte)player.whoAmI);
                packet.Send();
                return true;
            }
            return null;
        }
    }
}
