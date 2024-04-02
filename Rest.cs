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
        private static string tipMsgCache = null;

        private static readonly FieldInfo
            f_lastRestEffectiveness = typeof(Need_Rest).GetField("lastRestEffectiveness", Utility.flags),
            f_pawn = typeof(Need).GetField("pawn", Utility.flags);
        public static string ProcessNeed(Need_Rest need, string originalTip)
        {
            // Range check
            bool resting = need.Resting;
            float curLevel = need.CurLevel;
            if ((resting && curLevel >= 1f) ||
                (!resting && curLevel <= 0f))
                return originalTip;

            // Use cached string if need level and tick match
            int currTick = Find.TickManager.TicksGame;
            if (currTick == tickCache &&
                curLevel == levelCache)
                return tipMsgCache;
            tickCache = currTick;
            levelCache = curLevel;

            if (resting)
                return HandleResting(need, originalTip);
            else
                return HandleAwake(need, originalTip);
        }

        private static string HandleResting(Need_Rest need, string originalTip)
        {
            Pawn pawn = (Pawn)f_pawn.GetValue(need);
            string newTip = originalTip;
            float curLevel = need.CurLevel;

            float changePerUpdate = NeedTunings.NeedUpdateInterval
                * Need_Rest.BaseRestGainPerTick
                * (float)f_lastRestEffectiveness.GetValue(need)
                * pawn.GetStatValue(StatDefOf.RestRateMultiplier);
            if (changePerUpdate <= 0f)
                return originalTip;

            float updatesTo = (1f - curLevel) / changePerUpdate;
            newTip += "INI.Rest.Rested".Translate(updatesTo.PeriodTo(pawn));
            return tipMsgCache = newTip; // Update cache
        }
        private static string HandleAwake(Need_Rest need, string originalTip)
        {
            Pawn pawn = (Pawn)f_pawn.GetValue(need);
            string newTip = originalTip;
            float curLevel = need.CurLevel;

            float changePerUpdate = NeedTunings.NeedUpdateInterval
                * pawn.GetStatValue(StatDefOf.RestFallRateFactor)
                * need.RestFallPerTick;
            if (changePerUpdate <= 0f)
                return originalTip;

            if (curLevel >= Need_Rest.ThreshTired)
            {
                float updatesTo = (curLevel - Need_Rest.ThreshTired) / changePerUpdate;
                curLevel -= Mathf.Ceil(updatesTo) * changePerUpdate;
                newTip += "INI.Rest.Tired".Translate(updatesTo.PeriodTo(pawn));
                changePerUpdate *= 0.7f; // rest will fall slower
            }
            if (curLevel >= Need_Rest.ThreshVeryTired)
            {
                float updatesTo = (curLevel - Need_Rest.ThreshVeryTired) / changePerUpdate;
                curLevel -= Mathf.Ceil(updatesTo) * changePerUpdate;
                newTip += "INI.Rest.VeryTired".Translate(updatesTo.PeriodTo(pawn));
                changePerUpdate *= 0.3f / 0.7f; // rest will fall slower
            }
            if (curLevel > 0)
            {
                float updatesTo = curLevel / changePerUpdate;
                newTip += "INI.Rest.Exhausted".Translate(updatesTo.PeriodTo(pawn));
            }
            return tipMsgCache = newTip; // Update cache
        }
    }
}
