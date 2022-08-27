using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace ChickenInvadersMod.Common.ItemDropRules.DropConditions
{
    class InSkyCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (Main.hardMode && info.player.ZoneSkyHeight && !ChickenInvasionSystem.ChickenInvasionActive)
            {
                return !info.IsInSimulation;
            }

            return false;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Drops in hardmode near floating islands";
        }
    }
}
