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
        protected static int TicksUntilThreshold(float levelOfNeed, float threshold, float perTickLevelChange)
        {
            return Mathf.CeilToInt((levelOfNeed - threshold) / perTickLevelChange);
        }

        protected int TicksUntilThresholdUpdate(float levelOfNeed, float threshold, float perTickLevelChange)
        {
            float updatesUntilThreshold;

            updatesUntilThreshold = (levelOfNeed - threshold) / (perTickLevelChange * NeedTunings.NeedUpdateInterval);

            return Mathf.CeilToInt(updatesUntilThreshold) * NeedTunings.NeedUpdateInterval;
        }

        private static readonly AccessTools.FieldRef<Need, Pawn>
            fr_pawn = AccessTools.FieldRefAccess<Need, Pawn>("pawn");

        protected Need need;
        protected Pawn pawn;

        protected int basicUpdatedAt;
        protected int detailedUpdatedAt;
        protected int ratesUpdatedAt;

        public string basicTip;
        public string detailedTip;


        public NeedAddendum(Need need)
        {
            this.need = need;
            pawn = fr_pawn(need);

            basicTip = string.Empty;
            detailedTip = string.Empty;

            basicUpdatedAt = -1;
            ratesUpdatedAt = -1;
        }

        public bool IsBasicStale(int tickNow)
        {
            return tickNow != basicUpdatedAt;
        }

        public bool IsDetailedStale(int tickNow)
        {
            return tickNow - detailedUpdatedAt > 150;
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

        public virtual void UpdateBasic(int tickNow)
        {
            basicUpdatedAt = tickNow;
        }

        public virtual void UpdateDetailed(int tickNow)
        {
            detailedUpdatedAt = tickNow;
        }

        public virtual void UpdateRates(int tickNow)
        {
            ratesUpdatedAt = tickNow;
        }
    }
}