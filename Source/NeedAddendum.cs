using System;
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
            float updatesUntilThreshold;

            updatesUntilThreshold = (levelOfNeed - threshold) / (perTickLevelChange * NeedTunings.NeedUpdateInterval);

            return Mathf.CeilToInt(updatesUntilThreshold) * NeedTunings.NeedUpdateInterval;
        }

        private static readonly AccessTools.FieldRef<Need, Pawn>
            fr_pawn = AccessTools.FieldRefAccess<Need, Pawn>("pawn");

        protected Need need;
        protected Pawn pawn;

        protected int basicTipAddendumUpdatedAt;
        protected int detailedTipAddendumUpdatedAt;
        protected int ratesUpdatedAt;

        public string basicTipAddendum;
        public string detailedTipAddendum;


        public NeedAddendum(Need need)
        {
            this.need = need;
            pawn = fr_pawn(need);

            basicTipAddendum = string.Empty;
            detailedTipAddendum = string.Empty;

            basicTipAddendumUpdatedAt = -1;
            ratesUpdatedAt = -1;
        }

        public bool IsBasicTipAddendumStale(int tickNow)
        {
            return tickNow != basicTipAddendumUpdatedAt;
        }

        public bool IsRatesStale(int tickNow)
        {
            return (
                (   tickNow != ratesUpdatedAt
                    && pawn.IsHashIntervalTick(NeedTunings.NeedUpdateInterval)
                )
                || (tickNow - ratesUpdatedAt > 150)
                || ratesUpdatedAt == -1
            );
        }

        public bool IsSameNeed(Need need)
        {
            return need == this.need;
        }

        public virtual void UpdateBasicAddendums(int tickNow) { }

        public virtual void UpdateBasicTipAddendum(int tickNow)
        {
            basicTipAddendumUpdatedAt = tickNow;
        }

        public virtual void UpdateDetailedAddendums(int tickNow) { }

        public virtual void UpdateDetailedTipAddendum(int tickNow) { }

        public virtual void UpdateThresholdAts(int tickNow) { }

        public virtual void UpdateTickRates(int tickNow)
        {
            ratesUpdatedAt = tickNow;
        }
    }
}