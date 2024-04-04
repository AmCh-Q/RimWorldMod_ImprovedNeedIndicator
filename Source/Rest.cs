using System.Reflection;
using UnityEngine;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public static class Rest
    {
        private static int tickCache = -1;
        private static float levelCache = -1f;
        private static int pawnIdCache = -1;
        private static string tipMsgCache = string.Empty;

        private static readonly FieldInfo
#if (v1_2 || v1_3)
        // Need.Resting used to be private in 1.2/1.3
        //   we will copy its implementation using field Need.lastRestTick
            f_lastRestTick = typeof(Need_Rest).GetField("lastRestTick", Utility.flags),
#endif
            f_lastRestEffectiveness = typeof(Need_Rest).GetField("lastRestEffectiveness", Utility.flags),
            f_pawn = typeof(Need).GetField("pawn", Utility.flags);
        public static string ProcessNeed(Need_Rest need)
        {
            // Use cached string if need level and tick match
            int currTick = Find.TickManager.TicksGame;
            float curLevel = need.CurLevel;
            Pawn pawn = (Pawn)f_pawn.GetValue(need);
            if (currTick == tickCache &&
                curLevel == levelCache &&
                pawnIdCache == pawn.thingIDNumber)
                return tipMsgCache;
            tickCache = currTick;
            levelCache = curLevel;
            pawnIdCache = pawn.thingIDNumber;

#if (v1_2 || v1_3)
            bool resting = Find.TickManager.TicksGame < (int)f_lastRestTick.GetValue(need) + 2;
#else
            bool resting = need.Resting;
#endif
            string newTip = "\n";
            int tickCorrection = pawn.IntervalCorretion();
            // Using "|" to avoid short-circuiting
            if (HandleResting(need, pawn, resting ? tickCorrection : 0, ref newTip) |
                HandleAwake(need, pawn, resting ? 0 : tickCorrection, ref newTip))
                return tipMsgCache = newTip;
            return tipMsgCache = string.Empty;
        }

        private static bool HandleResting(
            Need_Rest need, Pawn pawn, int tickCorrection, ref string tipMsg)
        {
            float changePerTick = Need_Rest.BaseRestGainPerTick
                * (float)f_lastRestEffectiveness.GetValue(need)
                * pawn.GetStatValue(StatDefOf.RestRateMultiplier);
            if (changePerTick <= 0f)
                return false;

            float curLevel = need.CurLevel;
            float ticksTo = (1f - curLevel) / changePerTick;
            int ticksToInt = (ticksTo <= 0f) ? 0 : ticksTo.CeilToUpdate();
            ticksToInt += tickCorrection;

            tipMsg += "INI.Rest.Rested".Translate(
                ticksToInt.TicksToPeriod());
            return true;
        }
        private static bool HandleAwake(
            Need_Rest need, Pawn pawn, int tickCorrection, ref string tipMsg)
        {
            float changePerTick = need.RestFallPerTick;
#if (!v1_2 && !v1_3)
            // StatDef did not exist back in 1.2/1.3
            changePerTick *= pawn.GetStatValue(StatDefOf.RestFallRateFactor);
#endif
            if (changePerTick <= 0f)
                return false;

            float curLevel = need.CurLevel;
            float ticksTo;
            int ticksToInt = tickCorrection;
            if (curLevel >= Need_Rest.ThreshTired)
            {
                ticksTo = (curLevel - Need_Rest.ThreshTired) / changePerTick;
                ticksToInt += ticksTo.CeilToUpdate();
                tipMsg += "INI.Rest.Tired".Translate(ticksToInt.TicksToPeriod());
                curLevel -= ticksTo.CeilToUpdate() * changePerTick;
                changePerTick *= 0.7f; // rest will fall slower
            }
            if (curLevel >= Need_Rest.ThreshVeryTired)
            {
                ticksTo = (curLevel - Need_Rest.ThreshVeryTired) / changePerTick;
                ticksToInt += ticksTo.CeilToUpdate();
                tipMsg += "INI.Rest.VeryTired".Translate(ticksToInt.TicksToPeriod());
                curLevel -= ticksTo.CeilToUpdate() * changePerTick;
                changePerTick *= 0.3f / 0.7f; // rest will fall slower
            }
            if (curLevel > 0f)
            {
                ticksTo = curLevel / changePerTick;
                ticksToInt += ticksTo.CeilToUpdate();
            }
            tipMsg += "INI.Rest.Exhausted".Translate(
                ticksToInt.TicksToPeriod());
            return true;
        }
    }
}