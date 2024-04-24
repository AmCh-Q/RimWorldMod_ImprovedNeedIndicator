using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{

    public class NeedFoodAddendum : NeedAddendum
    {
        private Need_Food needFood;

        public NeedFoodAddendum(Need_Food need) : base(need)
        {
            needFood = need;

            fallingAddendums = new ThresholdAddendum[] {
                new ThresholdAddendum(
                    (byte)HungerCategory.Hungry,
                    (byte)HungerCategory.Fed,
                    needFood.PercentageThreshHungry,
                    "INI.Food.Hungry"
                ),
                new ThresholdAddendum(
                    (byte)HungerCategory.UrgentlyHungry,
                    (byte)HungerCategory.Hungry,
                    needFood.PercentageThreshUrgentlyHungry,
                    "INI.Food.UrgentlyHungry"
                ),
                new ThresholdAddendum(
                    (byte)HungerCategory.Starving,
                    (byte)HungerCategory.UrgentlyHungry,
                    0f,
                    "INI.Food.Starving"
                )
            };
        }

        public override void UpdateBasicTip(int tickNow)
        {
            base.UpdateBasicTipFalling(tickNow, need.CurLevel);

            basicUpdatedAt = tickNow;
        }

        public override void UpdateDetailedTip(int tickNow)
        {
            base.UpdateDetailedTip(tickNow);
        }

        public override void UpdateRates(int tickNow)
        {
            foreach (ThresholdAddendum threshold in fallingAddendums)
                threshold.Rate = needFood.FoodFallPerTickAssumingCategory((HungerCategory)threshold.RateCategory);

            base.UpdateRates(tickNow);
        }
    }
}
