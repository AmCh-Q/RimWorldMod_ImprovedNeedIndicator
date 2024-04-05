using System.Reflection;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public static class Rest
    {
        private static int tickCache = -1;
        private static float levelCache = -1f;
        private static int pawnIdCache = -1;
        private static float fallPerTick = 0f;
        private static float gainPerTick = 0f;
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
            // If different pawn or too long since update
            //   update change per tick
            if (pawnIdCache != pawn.thingIDNumber ||
                currTick - tickCache > Utility.interval)
                UpdateChangePerTick(need, pawn);
            tickCache = currTick;
            levelCache = curLevel;
            pawnIdCache = pawn.thingIDNumber;

#if (v1_2 || v1_3)
            bool resting = Find.TickManager.TicksGame < (int)f_lastRestTick.GetValue(need) + 2;
#else
            bool resting = need.Resting;
#endif
            string newTip = "\n";
            int tickCorrection = -pawn.TickSinceUpdate();
            if (tickCorrection == 0)
                UpdateChangePerTick(need, pawn);
            // When time is ticking up, use 0 (disable correction)
            int restingCorrection = resting ? tickCorrection : 0;
            int awakeCorrection = resting ? 0 : tickCorrection;

            // Using "|" to avoid short-circuiting
            if (HandleResting(curLevel, restingCorrection, ref newTip) |
                HandleAwake(curLevel, awakeCorrection, ref newTip))
                return tipMsgCache = newTip;
            return tipMsgCache = string.Empty;
        }

        private static void UpdateChangePerTick(Need_Rest need, Pawn pawn)
        {
            gainPerTick = Need_Rest.BaseRestGainPerTick
                * (float)f_lastRestEffectiveness.GetValue(need)
                * pawn.GetStatValue(StatDefOf.RestRateMultiplier);
            fallPerTick = need.RestFallPerTick;
#if (!v1_2 && !v1_3)
            // StatDef did not exist back in 1.2/1.3
            fallPerTick *= pawn.GetStatValue(StatDefOf.RestFallRateFactor);
#endif
        }
        private static bool HandleResting(
            float curLevel, int tickCorrection, ref string tipMsg)
        {
            if (gainPerTick <= 0f)
                return false;

            int ticksToTotal = tickCorrection;
            int ticksToUpdate = ((1f - curLevel) / gainPerTick).CeilToUpdate();
            ticksToTotal += ticksToUpdate;

            tipMsg += "INI.Rest.Rested".Translate(
                ticksToTotal.TicksToPeriod());
            return true;
        }
        private static bool HandleAwake(
            float curLevel, int tickCorrection, ref string tipMsg)
        {
            if (Rest.fallPerTick <= 0f)
                return false;
            float fallPerTick = Rest.fallPerTick;

            int ticksToTotal = tickCorrection;
            if (curLevel >= Need_Rest.ThreshTired)
            {
                int ticksToUpdate = ((curLevel - Need_Rest.ThreshTired) / fallPerTick).CeilToUpdate();
                ticksToTotal += ticksToUpdate;
                tipMsg += "INI.Rest.Tired".Translate(ticksToTotal.TicksToPeriod());
                curLevel -= ticksToUpdate * fallPerTick;
                fallPerTick *= 0.7f; // rest will fall slower
            }
            if (curLevel >= Need_Rest.ThreshVeryTired)
            {
                int ticksToUpdate = ((curLevel - Need_Rest.ThreshVeryTired) / fallPerTick).CeilToUpdate();
                ticksToTotal += ticksToUpdate;
                tipMsg += "INI.Rest.VeryTired".Translate(ticksToTotal.TicksToPeriod());
                curLevel -= ticksToUpdate * fallPerTick;
                fallPerTick *= 0.3f / 0.7f; // rest will fall slower
            }
            if (curLevel > 0f)
            {
                int ticksToUpdate = (curLevel / fallPerTick).CeilToUpdate();
                ticksToTotal += ticksToUpdate;
            }
            tipMsg += "INI.Rest.Exhausted".Translate(
                ticksToTotal.TicksToPeriod());
            return true;
        }
    }
}