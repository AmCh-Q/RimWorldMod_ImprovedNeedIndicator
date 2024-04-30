using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need
    {
        protected Need need;
        protected Pawn pawn;

        protected int basicUpdatedAt;
        protected int detailUpdatedAt;

        public string basicTip;
        public string detailedTip;

        protected Addendum_Need[] FallingAddendums { get; set; }

        public AddendumManager_Need(Need need)
        {
            this.need = need;
            pawn = fr_pawn(need);

            basicTip = string.Empty;
            detailedTip = string.Empty;

            basicUpdatedAt = -1;
            detailUpdatedAt = -1;

            FallingAddendums = new Addendum_Need[]{ };
        }

        public virtual bool IsBasicStale(int tickNow)
        {
            if (basicTip == "")
                return true;

            if (
                need.def.freezeWhileSleeping
                && pawn.Awake() == false
            )
                return false;

            return tickNow != basicUpdatedAt;
        }

        public virtual bool IsDetailStale(int tickNow)
        {
            if (detailedTip == "")
                return true;

            if (
                need.def.freezeWhileSleeping
                && pawn.Awake() == false
            )
                return false;

            return tickNow - detailUpdatedAt > 150;
        }

        public virtual bool IsSameNeed(Need need)
        {
            return need == this.need;
        }

        public virtual void UpdateBasicTip(int tickNow)
        {
            basicUpdatedAt = tickNow;
        }

        public virtual void UpdateDetailTip(int tickNow)
        {
            detailUpdatedAt = tickNow;
        }


        protected static int TicksUntilThreshold(
            float levelOfNeed,
            float threshold,
            float perTickLevelChange
        ) {
            return Mathf.CeilToInt((levelOfNeed - threshold) / perTickLevelChange);
        }

        protected static int TicksUntilThresholdUpdate(
            float levelOfNeed,
            float threshold,
            float perTickLevelChange
        ) {
            float updatesUntilThreshold;

            updatesUntilThreshold = (levelOfNeed - threshold) / (perTickLevelChange * NeedTunings.NeedUpdateInterval);

            return Mathf.CeilToInt(updatesUntilThreshold) * NeedTunings.NeedUpdateInterval;
        }

        private static readonly AccessTools.FieldRef<Need, Pawn>
            fr_pawn = AccessTools.FieldRefAccess<Need, Pawn>("pawn");
    }
}