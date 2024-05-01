using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;


namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_RoofEnclosure : AddendumManager_Need
    {
        protected float curTickRate;
        protected int ratesUpdatedAt;

        public AddendumManager_Need_RoofEnclosure(Need need) : base(need)
        {
            showDetails = string.Empty;

            curTickRate = 0f;
            ratesUpdatedAt = -1;
        }

        public virtual bool IsRatesStale(int tickNow)
        {
            return (
                (tickNow != ratesUpdatedAt
                    && pawn.IsHashIntervalTick(NeedTunings.NeedUpdateInterval)
                )
                || (tickNow - ratesUpdatedAt > 150)
                || ratesUpdatedAt == -1
            );
        }

        public override void UpdateBasicTip(int tickNow)
        {
            if (need.GUIChangeArrow < 0)
                UpdateBasicTipFalling(tickNow, need.CurLevel);

            else if (need.GUIChangeArrow > 0)
                UpdateBasicTipRising(tickNow, need.CurLevel);


            base.UpdateBasicTip(tickNow);
        }

        protected virtual void UpdateBasicTipFalling(int tickNow, float curLevel)
        {
            float levelAccumulator;
            int tickAccumulator;
            int tickOffset;
            int ticksUntilThreshold;

            levelAccumulator = curLevel;
            tickAccumulator = 0;
            tickOffset = pawn.TicksUntilNextUpdate();

            basicTip = string.Empty;
            foreach (Addendum_Need thresholdAddendum in FallingAddendums)
            {
                if (levelAccumulator >= thresholdAddendum.Max)
                {
                    ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, thresholdAddendum.Max, curTickRate);
                    tickAccumulator += ticksUntilThreshold;
                    levelAccumulator -= ticksUntilThreshold * curTickRate;

                    thresholdAddendum.Basic = thresholdAddendum.Translation.Translate((tickAccumulator - tickOffset).TicksToPeriod());
                }

                if (curLevel >= thresholdAddendum.Max)
                    basicTip += "\n" + thresholdAddendum.Basic;
            }

            basicTip = basicTip.Trim();
        }

        protected virtual void UpdateBasicTipRising(int tickNow, float curLevel)
        {
            float levelAccumulator;
            int tickAccumulator;
            int ticksUntilThreshold;

            levelAccumulator = curLevel;
            tickAccumulator = 0;

            basicTip = string.Empty;
            foreach (Addendum_Need thresholdAddendum in FallingAddendums)
            {
                if (levelAccumulator <= thresholdAddendum.Min)
                {
                    ticksUntilThreshold = TicksUntilThreshold(thresholdAddendum.Min, levelAccumulator, curTickRate);
                    tickAccumulator += ticksUntilThreshold;
                    levelAccumulator += ticksUntilThreshold * curTickRate;

                    thresholdAddendum.Basic = thresholdAddendum.Translation.Translate(tickAccumulator.TicksToPeriod());
                }

                if (curLevel <= thresholdAddendum.Min)
                    basicTip += "\n" + thresholdAddendum.Basic;
            }

            basicTip = basicTip.Trim();
        }

        public override void UpdateDetailTip(int tickNow)
        {
            detailedTip = basicTip;
            detailUpdatedAt = tickNow;
        }

        public virtual void UpdateRates(int tickNow)
        {
            ratesUpdatedAt = tickNow;
        }
    }
}
