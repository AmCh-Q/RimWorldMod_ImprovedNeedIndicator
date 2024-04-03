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

            string newTip = "\n";
            // Using "|" to avoid short-circuiting
            if (HandleResting(need, pawn, ref newTip) |
                HandleAwake(need, pawn, ref newTip))
                return tipMsgCache = newTip;
            return tipMsgCache = string.Empty;
        }

        private static bool HandleResting(Need_Rest need, Pawn pawn, ref string tipMsg)
        {
            float changePerUpdate = NeedTunings.NeedUpdateInterval
                * Need_Rest.BaseRestGainPerTick
                * (float)f_lastRestEffectiveness.GetValue(need)
                * pawn.GetStatValue(StatDefOf.RestRateMultiplier);
            if (changePerUpdate <= 0f)
                return false;

            float curLevel = need.CurLevel;
            if (curLevel > 1f)
                curLevel = 1f;

            float updatesTo = (1f - curLevel) / changePerUpdate;
            tipMsg += "INI.Rest.Rested".Translate(
                updatesTo.PeriodTo(need.Resting ? pawn : null));
            return true;
        }
        private static bool HandleAwake(Need_Rest need, Pawn pawn, ref string tipMsg)
        {
            float changePerUpdate = NeedTunings.NeedUpdateInterval
                * need.RestFallPerTick
                * pawn.GetStatValue(StatDefOf.RestFallRateFactor);
            if (changePerUpdate <= 0f)
                return false;

            float updatesTo;
            float curLevel = need.CurLevel;
            int ticksTo = 0;
            pawn = need.Resting ? null : pawn;
            if (curLevel >= Need_Rest.ThreshTired)
            {
                updatesTo = (curLevel - Need_Rest.ThreshTired) / changePerUpdate;
                ticksTo += updatesTo.TicksTo(pawn);
                tipMsg += "INI.Rest.Tired".Translate(ticksTo.ToStringTicksToPeriod());
                curLevel -= Mathf.Ceil(updatesTo) * changePerUpdate;
                changePerUpdate *= 0.7f; // rest will fall slower
            }
            if (curLevel >= Need_Rest.ThreshVeryTired)
            {
                updatesTo = (curLevel - Need_Rest.ThreshVeryTired) / changePerUpdate;
                ticksTo += updatesTo.TicksTo(pawn);
                tipMsg += "INI.Rest.VeryTired".Translate(ticksTo.ToStringTicksToPeriod());
                curLevel -= Mathf.Ceil(updatesTo) * changePerUpdate;
                changePerUpdate *= 0.3f / 0.7f; // rest will fall slower
            }
            if (curLevel > 0f)
            {
                updatesTo = curLevel / changePerUpdate;
                ticksTo += updatesTo.TicksTo(pawn);
            }
            tipMsg += "INI.Rest.Exhausted".Translate(ticksTo.ToStringTicksToPeriod());
            return true;
        }
    }
}
