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
            int tickOffset;
            int tickAccumulator;


            hungerCategory = need.CurCategory;
            levelOfNeed = need.CurLevel;

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

            tickAccumulator = 0;

            switch (hungerCategory)
            {
                case HungerCategory.Fed:
                    tickAccumulator += need.TicksUntilHungryWhenFed;
                    tipAddendums.Add("INI.Food.Hungry".Translate(tickAccumulator.TicksToPeriod()));

                    levelOfNeed -= tickAccumulator * perTickFoodFall;
                    perTickFoodFall *= .5f;
                    goto case HungerCategory.Hungry;

                case HungerCategory.Hungry:
                    tickAccumulator += TicksToNeedThreshold(levelOfNeed, need.PercentageThreshUrgentlyHungry * need.MaxLevel, perTickFoodFall);
                    tipAddendums.Add("INI.Food.Ravenous".Translate(tickAccumulator.TicksToPeriod()));

                    levelOfNeed -= tickAccumulator * perTickFoodFall;
                    perTickFoodFall *= .5f;
                    goto case HungerCategory.UrgentlyHungry;

                case HungerCategory.UrgentlyHungry:
                    tickAccumulator += TicksToNeedThreshold(levelOfNeed, 0f, perTickFoodFall);
                    tipAddendums.Add("INI.Food.Malnourished".Translate(tickAccumulator.TicksToPeriod()));
                    break;

                default:
                    break;
            }

            return string.Join("\n", tipAddendums);
        }

        private static int TicksToNeedThreshold(float levelOfNeed, float threshold, float perTickLevelChange)
        {
            float levelDeltaToThreshold = (levelOfNeed - threshold);
            float ticksToNeedThreshold = levelDeltaToThreshold / perTickLevelChange;

            return Mathf.CeilToInt(ticksToNeedThreshold);
        }
    }
}
