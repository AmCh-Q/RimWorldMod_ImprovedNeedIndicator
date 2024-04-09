using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Improved_Need_Indicator
{
    public class NeedAddendum
    {
        public int TicksUntilThreshold(float levelOfNeed, float threshold, float perTickLevelChange)
        {
            float levelDeltaToThreshold = (levelOfNeed - threshold);
            float ticksUntilThreshold = levelDeltaToThreshold / perTickLevelChange;

            return Mathf.CeilToInt(ticksUntilThreshold);
        }

        private static readonly AccessTools.FieldRef<Need, Pawn>
            fr_pawn = AccessTools.FieldRefAccess<Need, Pawn>("pawn");

        protected Need need;
        protected Pawn pawn;

        protected int basicTipAddendumTick; // when tip addendum was updated.
        protected int detailedTipAddendumTick; // when detailed tip addendum was updated.
        protected int ratesTick; // when need rates were updated.

        public string basicTipAddendum;
        public string detailedTipAddendum;


        public NeedAddendum(Need need)
        {
            this.need = need;
            pawn = fr_pawn(need);

            detailedTipAddendum = string.Empty;
            detailedTipAddendumTick = -1;
            ratesTick = -1;
            basicTipAddendum = string.Empty;
            basicTipAddendumTick = -1;
        }

        public bool IsDetailedTipAddendumStale(int tickNow)
        {
            return tickNow != detailedTipAddendumTick;
        }

        public bool IsRatesStale(int tickNow)
        {
            return (
                (   tickNow != ratesTick
                    && pawn.IsHashIntervalTick(NeedTunings.NeedUpdateInterval)
                )
                || ratesTick == -1
            );
        }

        public bool IsSameNeed(Need need)
        {
            return need == this.need;
        }

        public bool IsBasicTipAddendumStale(int tickNow)
        {
            return tickNow != basicTipAddendumTick;
        }

        public virtual void UpdateDetailedTipAddendum(int tickNow)
        {
            detailedTipAddendumTick = tickNow;
        }

        public virtual void UpdateBasicTipAddendum(int tickNow)
        {
            basicTipAddendumTick = tickNow;
        }

        public virtual void UpdateTickRates(int tickNow)
        {
            ratesTick = tickNow;
        }
    }
}