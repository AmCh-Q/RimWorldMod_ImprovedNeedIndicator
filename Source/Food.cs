using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class Food
    {
        private static float cachedLevelOfNeed = -1f;
        private static int cachedPawnId = -1;
        private static int cachedTickNow = -1;
        private static string cachedTipStringAddendum = string.Empty;

        public static string ProcessNeed(Pawn pawn, Need_Food need, int tickNow)
        {
            List<string> tipAddendums = new List<string>() { string.Empty, string.Empty };

            HungerCategory hungerCategory;
            float levelOfNeed;
            float perTickFoodFall;
            float threshold;
            int tickOffset;
            int tickAccumulator;
            int ticksToThresholdUpdate;


            hungerCategory = need.CurCategory;
            levelOfNeed = need.CurLevel;

#if DEBUG
            tipAddendums.Add("Hunger Category: " + hungerCategory);
            tipAddendums.Add("Hunger Level Of Need: " + levelOfNeed);
#endif

            if (cachedPawnId == pawn.thingIDNumber &&
                tickNow == cachedTickNow &&
                levelOfNeed.IsCloseTo(cachedLevelOfNeed, 0.0001f))
            {
                return cachedTipStringAddendum;
            }

            cachedTickNow = tickNow;
            cachedLevelOfNeed = levelOfNeed;
            cachedPawnId = pawn.thingIDNumber;

            tickOffset = pawn.TicksToNextUpdateTick();
            perTickFoodFall = need.FoodFallPerTick;

            levelOfNeed -= tickOffset * perTickFoodFall;

            tickAccumulator = 0;

            switch (hungerCategory)
            {
                case HungerCategory.Fed:
                    threshold = need.PercentageThreshHungry * need.MaxLevel;

                    ticksToThresholdUpdate = TicksToNeedThreshold(levelOfNeed, threshold, perTickFoodFall);
                    tickAccumulator += ticksToThresholdUpdate;
                    tipAddendums.Add("INI.Food.Hungry".Translate(tickAccumulator.TicksToPeriod()));

                    levelOfNeed -= ticksToThresholdUpdate * perTickFoodFall;
                    perTickFoodFall = need.FoodFallPerTickAssumingCategory(hungerCategory);
                    goto case HungerCategory.Hungry;

                case HungerCategory.Hungry:
                    threshold = need.PercentageThreshUrgentlyHungry * need.MaxLevel;

                    ticksToThresholdUpdate = TicksToNeedThreshold(levelOfNeed, threshold, perTickFoodFall);
                    tickAccumulator += ticksToThresholdUpdate;
                    tipAddendums.Add("INI.Food.Ravenous".Translate(tickAccumulator.TicksToPeriod()));

                    levelOfNeed -= ticksToThresholdUpdate * perTickFoodFall;
                    perTickFoodFall = need.FoodFallPerTickAssumingCategory(hungerCategory);
                    goto case HungerCategory.UrgentlyHungry;

                case HungerCategory.UrgentlyHungry:
                    threshold = 0f;

                    tickAccumulator += TicksToNeedThreshold(levelOfNeed, threshold, perTickFoodFall);
                    tipAddendums.Add("INI.Food.Malnourished".Translate(tickAccumulator.TicksToPeriod()));
                    break;

                default:
                    break;
            }

            return cachedTipStringAddendum = ((TaggedString)string.Join("\n", tipAddendums)).Resolve();
        }

        private static int TicksToNeedThreshold(float levelOfNeed, float threshold, float perTickLevelChange)
        {
            float levelDeltaToThreshold = (levelOfNeed - threshold);
            float ticksToNeedThreshold = levelDeltaToThreshold / perTickLevelChange;

            return Mathf.CeilToInt(ticksToNeedThreshold);
        }
    }
}
