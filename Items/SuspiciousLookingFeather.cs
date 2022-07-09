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
            item.noMelee = true;
            item.width = 12;
            item.height = 15;
            item.useTime = 32;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.value = Item.sellPrice(gold: 2);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = false;
            item.maxStack = 1;
            item.UseSound = SoundID.Item1;
            item.consumable = true;
        }

        public override bool UseItem(Player player)
        {
            return true;
        }

        public override bool ConsumeItem(Player player)
        {
            return CIWorld.StartChickenInvasion(player.position);
        }
    }
}
