using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class NeedFoodAddendum : NeedAddendum
    {
        private Need_Food needFood;

        private float fallPerTick;

        private string hungryAddendum;
        private string urgentlyHungryAddendum;
        private string starvingAddendum;

        private string maxHungryAddendum;
        private string maxUrgentlyHungryAddendum;
        private string maxStarvingAddendum;

        public NeedFoodAddendum(Need_Food need) : base(need)
        {
            needFood = need;
        }

        public override void UpdateBasic(int tickNow)
        {
            HungerCategory hungerCategory;
            float levelAccumulator;
            int tickAccumulator;
            int tickOffset;
            int ticksUntilThreshold;

            hungerCategory = needFood.CurCategory;
            levelAccumulator = need.CurLevel;
            tickAccumulator = 0;
            tickOffset = pawn.TicksUntilNextUpdate();

            string DoThresholdUpdate(string translation, float threshold, float localFallPerTick)
            {
                ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, threshold, localFallPerTick);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * localFallPerTick;
                return translation.Translate((tickAccumulator - tickOffset).TicksToPeriod());
            }

            switch (hungerCategory)
            {
                case HungerCategory.Fed:
                    hungryAddendum = DoThresholdUpdate("INI.Food.Hungry", needFood.PercentageThreshHungry, fallPerTick);
                    urgentlyHungryAddendum = DoThresholdUpdate("INI.Food.UrgentlyHungry", needFood.PercentageThreshUrgentlyHungry, needFood.FoodFallPerTickAssumingCategory(HungerCategory.Hungry));
                    starvingAddendum = DoThresholdUpdate("INI.Food.Starving", 0f, needFood.FoodFallPerTickAssumingCategory(HungerCategory.UrgentlyHungry));

                    basicTip = string.Join("\n", new[] { hungryAddendum, urgentlyHungryAddendum, starvingAddendum });
                    break;
                case HungerCategory.Hungry:
                    urgentlyHungryAddendum = DoThresholdUpdate("INI.Food.UrgentlyHungry", needFood.PercentageThreshUrgentlyHungry, fallPerTick);
                    starvingAddendum = DoThresholdUpdate("INI.Food.Starving", 0f, needFood.FoodFallPerTickAssumingCategory(HungerCategory.UrgentlyHungry));

                    basicTip = string.Join("\n", new[] { urgentlyHungryAddendum, starvingAddendum });
                    break;
                case HungerCategory.UrgentlyHungry:
                    starvingAddendum = DoThresholdUpdate("INI.Food.Starving", 0f, fallPerTick);

                    basicTip = starvingAddendum;
                    break;

                default:
                    break;
            }

            base.UpdateBasic(tickNow);
        }

        public override void UpdateDetailed(int tickNow)
        {
            HungerCategory hungerCategory;
            float levelAccumulator;
            int tickAccumulator;
            int tickOffset;
            int ticksUntilThreshold;

            hungerCategory = needFood.CurCategory;
            levelAccumulator = need.MaxLevel;
            tickAccumulator = 0;
            tickOffset = pawn.TicksUntilNextUpdate();

            string DoThresholdUpdate(float threshold, float localFallPerTick)
            {
                ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, threshold, localFallPerTick);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * localFallPerTick;
                return "\t" + "INI.Max".Translate((tickAccumulator - tickOffset).TicksToPeriod());
            }

            switch (hungerCategory)
            {
                case HungerCategory.Fed:
                    maxHungryAddendum = DoThresholdUpdate(needFood.PercentageThreshHungry, fallPerTick);
                    maxUrgentlyHungryAddendum = DoThresholdUpdate(needFood.PercentageThreshUrgentlyHungry, needFood.FoodFallPerTickAssumingCategory(HungerCategory.Hungry));
                    maxStarvingAddendum = DoThresholdUpdate(0f, needFood.FoodFallPerTickAssumingCategory(HungerCategory.UrgentlyHungry));

                    detailedTip = string.Join("\n", new[] {
                        hungryAddendum,
                        maxHungryAddendum,
                        urgentlyHungryAddendum,
                        maxUrgentlyHungryAddendum,
                        starvingAddendum,
                        maxStarvingAddendum
                    });
                    break;

                case HungerCategory.Hungry:
                    maxUrgentlyHungryAddendum = DoThresholdUpdate(needFood.PercentageThreshUrgentlyHungry, fallPerTick);
                    maxStarvingAddendum = DoThresholdUpdate(0f, needFood.FoodFallPerTickAssumingCategory(HungerCategory.UrgentlyHungry));

                    detailedTip = string.Join("\n", new[] {
                        urgentlyHungryAddendum,
                        maxUrgentlyHungryAddendum,
                        starvingAddendum,
                        maxStarvingAddendum
                    });
                    break;

                case HungerCategory.UrgentlyHungry:
                    maxStarvingAddendum = DoThresholdUpdate(0f, fallPerTick);

                    detailedTip = string.Join("\n", new[] {
                        starvingAddendum,
                        maxStarvingAddendum
                    });
                    break;

                default:
                    break;
            }

            base.UpdateDetailed(tickNow);
        }

        public override void UpdateRates(int tickNow)
        {
            fallPerTick = needFood.FoodFallPerTick;

            base.UpdateRates(tickNow);
        }
    }
}
