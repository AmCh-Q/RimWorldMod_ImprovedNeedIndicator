using System.Reflection;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public static class Rest
    {
        private static int tickCache = -1;
        private static float levelCache = -1f;
        private static string tipMsgCache = null;

        private static readonly FieldInfo
            f_lastRestEffectiveness = typeof(Need_Rest).GetField("lastRestEffectiveness", Utility.flags),
            f_pawn = typeof(Need).GetField("pawn", Utility.flags);
        public static string ProcessNeed(Need_Rest need, string originalTip)
        {
            // Use cached string if need level and tick match
            int currTick = Find.TickManager.TicksGame;
            float curLevel = need.CurLevel;
            if (currTick == tickCache &&
                curLevel == levelCache)
                return tipMsgCache;
            tickCache = currTick;
            levelCache = curLevel;

            // Calculate number of updates to the target need level
            //   and get the string describing situation
            float changePerUpdate;
            float updatesTo;
            string situationStr;
            Pawn pawn = (Pawn)f_pawn.GetValue(need);
            if (need.Resting)
            {
                changePerUpdate = 1f / 175f
                    * (float)f_lastRestEffectiveness.GetValue(need)
                    * pawn.GetStatValue(StatDefOf.RestRateMultiplier);
                updatesTo = (1f - curLevel) / changePerUpdate;
                situationStr = "Will sleep for: ";
            }
            else
            {
                changePerUpdate = 150f
                    * need.RestFallPerTick
                    * pawn.GetStatValue(StatDefOf.RestFallRateFactor);
                updatesTo = curLevel / changePerUpdate;
                situationStr = "Awake for: ";
            }
            if (updatesTo <= 0f)
                return originalTip;

            // Get string describing time remaining
            string timeStr = Utility.TimeString(updatesTo, pawn);

            // Update cache
            tipMsgCache = string.Concat(originalTip, "\n", situationStr, timeStr);
            return tipMsgCache;
        }
    }
}
