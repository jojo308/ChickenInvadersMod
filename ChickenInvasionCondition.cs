using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace ChickenInvadersMod
{
    class ChickenInvasionCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (Main.hardMode && CIWorld.ChickenInvasionActive)
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
            return "Drops in hardmode during a Chicken Invasion";
        }
    }

    class InSkyCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (Main.hardMode && info.player.ZoneSkyHeight && !CIWorld.ChickenInvasionActive)
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
