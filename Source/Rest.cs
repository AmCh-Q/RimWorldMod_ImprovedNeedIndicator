﻿using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public static class Rest
    {
        private static float cachedLevelOfNeed = -1f;
        private static int cachedPawnId = -1;
        private static int cachedTickNow = -1;
        private static string cachedTipStringAddendum = string.Empty;

        private static float cachedRestFallPerTick = -1;
        private static float cachedRestGainPerTick = -1;

#if (v1_2 || v1_3)
        // Need.Resting used to be private in 1.2/1.3
        //   we will copy its implementation using field Need.lastRestTick
        private static readonly FieldInfo
            f_lastRestTick = typeof(Need_Rest).GetField("lastRestTick", Utility.flags);
#endif
        private static readonly FieldInfo
            f_lastRestEffectiveness = typeof(Need_Rest).GetField("lastRestEffectiveness", Utility.flags);

        public static string ProcessNeed(Pawn pawn, Need_Rest need, int tickNow)
        {
            List<string> tipAddendums = new List<string>() { string.Empty, string.Empty };

            bool pawnIsResting;
            float levelOfNeed;
            float restFallPerTick;
            float restGainPerTick;
            int tickOffset;
            int tickAccumulator;
            int ticksUntilThreshold;


            levelOfNeed = need.CurLevel;

            if (cachedPawnId == pawn.thingIDNumber &&
                tickNow == cachedTickNow &&
                levelOfNeed.IsCloseTo(cachedLevelOfNeed, 0.0001f))
                return cachedTipStringAddendum;

            if (cachedPawnId != pawn.thingIDNumber ||
                pawn.IsHashIntervalTick(NeedTunings.NeedUpdateInterval))
            {
                cachedRestFallPerTick = RestFallPerTick(need, pawn);
                cachedRestGainPerTick = RestGainPerTick(need, pawn);
            }

            cachedTickNow = tickNow;
            cachedLevelOfNeed = levelOfNeed;
            cachedPawnId = pawn.thingIDNumber;

#if (v1_2 || v1_3)
            pawnIsResting = Find.TickManager.TicksGame < (int)f_lastRestTick.GetValue(need) + 2;
#else
            pawnIsResting = need.Resting;
#endif

            restFallPerTick = cachedRestFallPerTick;
            restGainPerTick = cachedRestGainPerTick;
            tickOffset = pawn.TicksUntilNextUpdate();

            if (pawnIsResting)
                levelOfNeed += tickOffset * restGainPerTick;
            else
                levelOfNeed -= tickOffset * restFallPerTick;


            ticksUntilThreshold = TicksUntilThreshold(1f, levelOfNeed, restGainPerTick);
            tipAddendums.Add("INI.Rest.Rested".Translate(ticksUntilThreshold.TicksToPeriod()));

            tickAccumulator = 0;

            if (levelOfNeed >= Need_Rest.ThreshTired)
            {
                ticksUntilThreshold = TicksUntilThreshold(levelOfNeed, Need_Rest.ThreshTired, restFallPerTick);
                tickAccumulator += ticksUntilThreshold;
                tipAddendums.Add("INI.Rest.Tired".Translate(tickAccumulator.TicksToPeriod()));

                // Set need and restFallPerTick fall so VeryTired can calculate from
                // the edge of where Tired leaves off.
                levelOfNeed -= ticksUntilThreshold * restFallPerTick;
                restFallPerTick *= 0.7f;
            }

            if (levelOfNeed >= Need_Rest.ThreshVeryTired)
            {
                ticksUntilThreshold = TicksUntilThreshold(levelOfNeed, Need_Rest.ThreshVeryTired, restFallPerTick);
                tickAccumulator += ticksUntilThreshold;
                tipAddendums.Add("INI.Rest.VeryTired".Translate(tickAccumulator.TicksToPeriod()));

                // Set need and restFallPerTick so Exhausted can calculate from
                // the edge of where VeryTired leaves off.
                levelOfNeed -= ticksUntilThreshold * restFallPerTick;
                restFallPerTick *= (0.3f/0.7f);
            }

            if (levelOfNeed > 0f)
            {
                ticksUntilThreshold = TicksUntilThreshold(levelOfNeed, 0f, restFallPerTick);
                tickAccumulator += ticksUntilThreshold;
                tipAddendums.Add("INI.Rest.Exhausted".Translate(tickAccumulator.TicksToPeriod()));
            }

            return cachedTipStringAddendum = ((TaggedString)string.Join("\n", tipAddendums)).Resolve();
        }

        private static float RestFallPerTick(Need_Rest need, Pawn pawn)
        {
#if (v1_2 || v1_3)
            // StatDefOf.RestFallRateFactor did not exist back in 1.2/1.3
            return need.RestFallPerTick;
#else
            return need.RestFallPerTick * pawn.GetStatValue(StatDefOf.RestFallRateFactor);
#endif
        }

        private static float RestGainPerTick(Need_Rest need, Pawn pawn)
        {
            return Need_Rest.BaseRestGainPerTick
                * (float)f_lastRestEffectiveness.GetValue(need)
                * pawn.GetStatValue(StatDefOf.RestRateMultiplier);
        }

        private static int TicksUntilThreshold(float levelOfNeed, float threshold, float perTickLevelChange)
        {
            float levelDeltaToThreshold = (levelOfNeed - threshold);
            float ticksUntilThreshold = levelDeltaToThreshold / perTickLevelChange;

            return Mathf.CeilToInt(ticksUntilThreshold);
        }
    }
}