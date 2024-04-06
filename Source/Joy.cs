using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Improved_Need_Indicator
{
    public static class Joy
    {
        const float
            ThreshHigh = 0.85f,
            ThreshSatisfied = 0.7f,
            ThreshLow = 0.3f,
            ThreshVeryLow = 0.15f,
            //changePerTickExtereme     = -0.0015f  / NeedTunings.NeedUpdateInterval,
            //changePerTickHigh         = -0.0015f  / NeedTunings.NeedUpdateInterval,
            changePerTickSatisfied      = -0.0015f  / NeedTunings.NeedUpdateInterval,
            changePerTickLow            = -0.00105f / NeedTunings.NeedUpdateInterval,
            changePerTickVeryLow        = -0.0006f  / NeedTunings.NeedUpdateInterval;

        private static float cachedLevelOfNeed = -1f;
        private static int cachedPawnId = -1;
        private static int cachedTickNow = -1;
        private static string cachedTipStringAddendum = string.Empty;

        public static string ProcessNeed(Pawn pawn, Need_Joy need, int tickNow)
        {
            List<string> tipAddendums = new List<string>() { string.Empty };
            float levelOfNeed;
            float changePerTick;
            int tickOffset;
            int tickAccumulator;

            levelOfNeed = need.CurLevel;

            if (cachedPawnId == pawn.thingIDNumber &&
                tickNow == cachedTickNow &&
                levelOfNeed.IsCloseTo(cachedLevelOfNeed, 0.0001f))
                return cachedTipStringAddendum;

            cachedTickNow = tickNow;
            cachedLevelOfNeed = levelOfNeed;
            cachedPawnId = pawn.thingIDNumber;

            changePerTick = FallPerTick(need, pawn);
            tickOffset = pawn.TicksToNextUpdateTick();
            levelOfNeed -= tickOffset * changePerTick;

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

        private static MethodInfo
            get_FallPerInterval = typeof(Need_Joy).GetProperty("FallPerInterval", Utility.flags).GetGetMethod();

        private static float FallPerTick(Need_Joy need, Pawn pawn)
        {
            return -(float)get_FallPerInterval.Invoke(need, null)
                * (pawn.IsFormingCaravan() ? 0.5f : 1f) 
                * pawn.GetStatValue(StatDefOf.JoyFallRateFactor);
        }
    }
}
