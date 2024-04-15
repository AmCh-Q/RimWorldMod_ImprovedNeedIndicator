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
            base.UpdateBasicTip(tickNow);
        }

        public override void UpdateDetailedTip(int tickNow)
        {
            float levelAccumulator;
            int tickAccumulator;
            int tickOffset;
            int ticksUntilThreshold;

            levelAccumulator = need.MaxLevel;
            tickAccumulator = 0;
            tickOffset = pawn.TicksUntilNextUpdate();

            void HandleThresholdAddendum(ThresholdAddendum thresholdAddendum)
            {
                ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, thresholdAddendum.Threshold, thresholdAddendum.Rate);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * thresholdAddendum.Rate;

                thresholdAddendum.DetailedAddendum = (
                    thresholdAddendum.BasicAddendum
                    + "\n\t" + "INI.Max".Translate((tickAccumulator - tickOffset).TicksToPeriod())
                );
            }

            detailedTip = "";
            foreach (ThresholdAddendum thresholdAddendum in fallingAddendums)
            {
                HandleThresholdAddendum(thresholdAddendum);
                if (levelAccumulator >= thresholdAddendum.Threshold)
                    detailedTip += "\n" + thresholdAddendum.DetailedAddendum;
            }

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
