using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Improved_Need_Indicator
{
    public static class Joy
    {
        private const float
            ThreshHigh = 0.85f,
            ThreshSatisfied = 0.7f,
            ThreshLow = 0.3f,
            ThreshVeryLow = 0.15f,
            //changePerTickExtereme = -0.0015f / NeedTunings.NeedUpdateInterval,
            //changePerTickHigh = -0.0015f / NeedTunings.NeedUpdateInterval,
            changePerTickSatisfied = -0.0015f / NeedTunings.NeedUpdateInterval,
            changePerTickLow = -0.00105f / NeedTunings.NeedUpdateInterval,
            changePerTickVeryLow = -0.0006f / NeedTunings.NeedUpdateInterval;

        private static float cachedLevelOfNeed = -1f;
        private static int cachedPawnId = -1;
        private static int cachedTickNow = -1;
        private static string cachedTipStringAddendum = string.Empty;

        public static string ProcessNeed(Pawn pawn, Need_Joy need, int tickNow)
        {
            List<string> tipAddendums = new List<string>() { string.Empty , string.Empty };
            float levelOfNeed;
            float changePerTick;
            int tickOffset;
            int tickAccumulator;

            levelOfNeed = need.CurLevel;

            if (pawn.thingIDNumber == cachedPawnId &&
                tickNow == cachedTickNow &&
                levelOfNeed == cachedLevelOfNeed)
                return cachedTipStringAddendum;

            cachedTickNow = tickNow;
            cachedLevelOfNeed = levelOfNeed;
            cachedPawnId = pawn.thingIDNumber;

            changePerTick = FallPerTick(need, pawn);
            tickOffset = pawn.TicksToNextUpdateTick();
            levelOfNeed += tickOffset * changePerTick;

            tickAccumulator = 0;
            Utility.TickUpdateToThreshold(ref levelOfNeed, ThreshHigh, changePerTick,
                ref tickAccumulator, "INI.Joy.High", tipAddendums);
            Utility.TickUpdateToThreshold(ref levelOfNeed, ThreshSatisfied, changePerTick,
                ref tickAccumulator, "INI.Joy.Satisfied", tipAddendums);
            if (Utility.TickUpdateToThreshold(ref levelOfNeed, ThreshLow, changePerTick,
                ref tickAccumulator, "INI.Joy.Low", tipAddendums))
                changePerTick *= changePerTickLow / changePerTickSatisfied;
            if (Utility.TickUpdateToThreshold(ref levelOfNeed, ThreshVeryLow, changePerTick,
                ref tickAccumulator, "INI.Joy.VeryLow", tipAddendums))
                changePerTick *= changePerTickVeryLow / changePerTickLow;
            Utility.TickUpdateToThreshold(ref levelOfNeed, 0, changePerTick,
                ref tickAccumulator, "INI.Joy.Empty", tipAddendums);

            return cachedTipStringAddendum = string.Join("\n", tipAddendums);
        }

        private static readonly MethodInfo
            get_FallPerInterval = typeof(Need_Joy)
                .GetProperty("FallPerInterval", Utility.flags).GetGetMethod(nonPublic: true);

        private static float FallPerTick(Need_Joy need, Pawn pawn)
        {
            const float negInvInterval = -1f / NeedTunings.NeedUpdateInterval;
            if (get_FallPerInterval == null)
                Log.Error("get_FallPerInterval is null!");
#if v1_2
            return (float)get_FallPerInterval.Invoke(need, null) * negInvInterval;
#elif v1_3 || v1_4
            return (float)get_FallPerInterval.Invoke(need, null)
                * (pawn.IsFormingCaravan() ? 0.5f * negInvInterval : negInvInterval);
#else
            return (float)get_FallPerInterval.Invoke(need, null)
                * (pawn.IsFormingCaravan() ? 0.5f * negInvInterval : negInvInterval)
                * pawn.GetStatValue(StatDefOf.JoyFallRateFactor);
#endif
        }
    }
}
