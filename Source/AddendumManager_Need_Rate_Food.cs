using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_Rate_Food : AddendumManager_Need_Rate
    {
        private Need_Food NeedFood { get; set; }

        public AddendumManager_Need_Rate_Food(Need_Food need) : base(need)
        {
            NeedFood = need;

            FallingAddendums = new Addendum_Need_Rate[] {
                new Addendum_Need_Rate(
                    (byte)HungerCategory.Fed,
                    need.MaxLevel,
                    need.PercentageThreshHungry,
                    "INI.Food.Fed",
                    (byte)HungerCategory.Fed
                ),
                new Addendum_Need_Rate(
                    (byte)HungerCategory.Hungry,
                    need.PercentageThreshHungry,
                    need.PercentageThreshUrgentlyHungry,
                    "INI.Food.Hungry",
                    (byte)HungerCategory.Fed
                ),
                new Addendum_Need_Rate(
                    (byte)HungerCategory.UrgentlyHungry,
                    need.PercentageThreshUrgentlyHungry,
                    0.00001f,
                    "INI.Food.Ravenous",
                    (byte)HungerCategory.Hungry
                ),
                new Addendum_Need_Rate(
                    (byte)HungerCategory.Starving,
                    0f,
                    0f,
                    "INI.Food.Malnourished",
                    (byte)HungerCategory.UrgentlyHungry
                )
            };
        }

        public override void UpdateBasicTip(int tickNow)
        {
            base.UpdateBasicTipFalling(tickNow, need.CurLevel);

            basicUpdatedAt = tickNow;
        }

        public override void UpdateDetailTip(int tickNow)
        {
            base.UpdateDetailTip(tickNow);
        }

        public override void UpdateRates(int tickNow)
        {
            foreach (Addendum_Need_Rate threshold in FallingRateAddendums)
                threshold.Rate = NeedFood.FoodFallPerTickAssumingCategory((HungerCategory)threshold.RateCategory);

            base.UpdateRates(tickNow);
        }
    }
}
