using System.Reflection;
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

#if (v1_2 || v1_3)
        // Need.Resting used to be private in 1.2/1.3
        //   we will copy its implementation using field Need.lastRestTick
        private static readonly FieldInfo
            f_lastRestTick = typeof(Need_Rest).GetField("lastRestTick", Utility.flags);
#endif
        private static readonly FieldInfo
            f_lastRestEffectiveness = typeof(Need_Rest).GetField("lastRestEffectiveness", Utility.flags),
            f_pawn = typeof(Need).GetField("pawn", Utility.flags);

        public static string ProcessNeed(Need_Rest need)
        {
            string tipStringAddendum;

            float levelOfNeed;
            int tickNow;
            Pawn pawn;

            float perTickRestGain;
            float perTickRestFall;

            int ticksToFullRestUpdateTick;
            int ticksToTiredUpdateTick;
            int ticksToVeryTiredUpdateTick;
            int ticksToExhaustedUpdateTick;


            tickNow = Find.TickManager.TicksGame;
            levelOfNeed = need.CurLevel;
            pawn = (Pawn)f_pawn.GetValue(need);

            if (cachedPawnId == pawn.thingIDNumber)
            {
                if (pawn.IsHashIntervalTick(NeedTunings.NeedUpdateInterval) == false)
                    return cachedTipStringAddendum;

                // Use cached string if need level and tick match
                if (tickNow == cachedTickNow &&
                    levelOfNeed.IsCloseTo(cachedLevelOfNeed, 0.0001f))
                    return cachedTipStringAddendum;
            }

            cachedTickNow = tickNow;
            cachedLevelOfNeed = levelOfNeed;
            cachedPawnId = pawn.thingIDNumber;

            perTickRestFall = RestFallPerTick(need, pawn);
            perTickRestGain = RestGainPerTick(need, pawn);

            tipStringAddendum = "\n";

            ticksToFullRestUpdateTick = TicksToNeedThresholdUpdateTick(1f, perTickRestGain, levelOfNeed);
            tipStringAddendum += "\n";
            tipStringAddendum += "INI.Rest.Rested".Translate(ticksToFullRestUpdateTick.TicksToPeriod());

            if (levelOfNeed >= Need_Rest.ThreshTired)
            {
                ticksToTiredUpdateTick = TicksToNeedThresholdUpdateTick(levelOfNeed, perTickRestFall, Need_Rest.ThreshTired);
                ticksToVeryTiredUpdateTick = (
                    ticksToTiredUpdateTick +
                    TicksToNeedThresholdUpdateTick(Need_Rest.ThreshTired, perTickRestFall * 0.7f, Need_Rest.ThreshVeryTired));
                ticksToExhaustedUpdateTick = (
                    ticksToVeryTiredUpdateTick +
                    TicksToNeedThresholdUpdateTick(Need_Rest.ThreshVeryTired, perTickRestFall * 0.3f, 0f));

                tipStringAddendum += "\n";
                tipStringAddendum += "INI.Rest.Tired".Translate(ticksToTiredUpdateTick.TicksToPeriod());
                tipStringAddendum += "\n";
                tipStringAddendum += "INI.Rest.VeryTired".Translate(ticksToVeryTiredUpdateTick.TicksToPeriod());
                tipStringAddendum += "\n";
                tipStringAddendum += "INI.Rest.Exhausted".Translate(ticksToExhaustedUpdateTick.TicksToPeriod());

            } else if (levelOfNeed >= Need_Rest.ThreshVeryTired)
            {
                ticksToVeryTiredUpdateTick = TicksToNeedThresholdUpdateTick(levelOfNeed, perTickRestFall, Need_Rest.ThreshVeryTired);
                ticksToExhaustedUpdateTick = (
                    ticksToVeryTiredUpdateTick +
                    TicksToNeedThresholdUpdateTick(Need_Rest.ThreshVeryTired, perTickRestFall * (0.3f/0.7f), 0f));

                tipStringAddendum += "\n";
                tipStringAddendum += "INI.Rest.VeryTired".Translate(ticksToVeryTiredUpdateTick.TicksToPeriod());
                tipStringAddendum += "\n";
                tipStringAddendum += "INI.Rest.Exhausted".Translate(ticksToExhaustedUpdateTick.TicksToPeriod());

            } else
            {
                ticksToExhaustedUpdateTick = TicksToNeedThresholdUpdateTick(levelOfNeed, perTickRestFall, 0f);

                tipStringAddendum += "\n";
                tipStringAddendum += "INI.Rest.Exhausted".Translate(ticksToExhaustedUpdateTick.TicksToPeriod());
            }


            return cachedTipStringAddendum = tipStringAddendum;
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

        private static int TicksToNeedThresholdUpdateTick(float levelOfNeed, float perTickLevelChange, float threshold)
        {
            float levelDeltaToThreshold = (levelOfNeed - threshold);
            float ticksToNeedThreshold = levelDeltaToThreshold / perTickLevelChange;

            return ticksToNeedThreshold.TicksToIntervalAdjustedTicks(NeedTunings.NeedUpdateInterval);
        }
    }
}