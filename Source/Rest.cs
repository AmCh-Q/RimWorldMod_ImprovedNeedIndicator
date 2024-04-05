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

            int tickOffset;
            int ticksToThresholdUpdateTick;


            tickNow = Find.TickManager.TicksGame;
            levelOfNeed = need.CurLevel;
            pawn = (Pawn)f_pawn.GetValue(need);

            if (cachedPawnId == pawn.thingIDNumber &&
                tickNow == cachedTickNow &&
                levelOfNeed.IsCloseTo(cachedLevelOfNeed, 0.0001f))
            {
                return cachedTipStringAddendum;
            }

            cachedTickNow = tickNow;
            cachedLevelOfNeed = levelOfNeed;
            cachedPawnId = pawn.thingIDNumber;

            tickOffset = pawn.TicksToNextUpdateTick(NeedTunings.NeedUpdateInterval);

            perTickRestFall = RestFallPerTick(need, pawn);
            perTickRestGain = RestGainPerTick(need, pawn);


            tipStringAddendum = "\n";


            ticksToThresholdUpdateTick = tickOffset;
            ticksToThresholdUpdateTick += TicksToNeedThresholdUpdateTick(1f, perTickRestGain, levelOfNeed);
            tipStringAddendum += "\n";
            tipStringAddendum += "INI.Rest.Rested".Translate(ticksToThresholdUpdateTick.TicksToPeriod());


            ticksToThresholdUpdateTick = tickOffset;


            if (levelOfNeed >= Need_Rest.ThreshTired)
            {
                ticksToThresholdUpdateTick += TicksToNeedThresholdUpdateTick(levelOfNeed, perTickRestFall, Need_Rest.ThreshTired);

                tipStringAddendum += "\n";
                tipStringAddendum += "INI.Rest.Tired".Translate(ticksToThresholdUpdateTick.TicksToPeriod());

                // Set need and perTickRestFall fall so VeryTired can calculate from
                // the edge of where Tired leaves off.
                levelOfNeed -= ticksToThresholdUpdateTick * perTickRestFall;
                perTickRestFall *= 0.7f;
            }

            if (levelOfNeed >= Need_Rest.ThreshVeryTired)
            {
                ticksToThresholdUpdateTick += TicksToNeedThresholdUpdateTick(levelOfNeed, perTickRestFall, Need_Rest.ThreshVeryTired);

                tipStringAddendum += "\n";
                tipStringAddendum += "INI.Rest.VeryTired".Translate(ticksToThresholdUpdateTick.TicksToPeriod());

                // Set need and perTickRestFall so Exhausted can calculate from
                // the edge of where VeryTired leaves off.
                levelOfNeed -= ticksToThresholdUpdateTick * perTickRestFall;
                perTickRestFall *= (0.3f/0.7f);
            }

            if (levelOfNeed > 0)
            {
                ticksToThresholdUpdateTick += TicksToNeedThresholdUpdateTick(levelOfNeed, perTickRestFall, 0f);

                tipStringAddendum += "\n";
                tipStringAddendum += "INI.Rest.Exhausted".Translate(ticksToThresholdUpdateTick.TicksToPeriod());
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