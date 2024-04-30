using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_Rate: AddendumManager_Need
    {
        protected int ratesUpdatedAt;

        protected Addendum_Need_Rate[] FallingRateAddendums {
            get { return (Addendum_Need_Rate[])FallingAddendums; }
            set { FallingAddendums = value; }
        }

        public AddendumManager_Need_Rate(Need need): base(need)
        {
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
            float curLevel = need.CurLevel;
            float curInstantLevel = need.CurInstantLevel;

            if (curInstantLevel < curLevel)
                UpdateBasicTipFalling(tickNow, curLevel);
            else
                UpdateBasicTipRising(tickNow, curInstantLevel);

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

            basicTip = "";
            foreach (Addendum_Need_Rate thresholdAddendum in FallingRateAddendums)
            {
                if (levelAccumulator >= thresholdAddendum.Max)
                {
                    ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, thresholdAddendum.Max, thresholdAddendum.Rate);
                    tickAccumulator += ticksUntilThreshold;
                    levelAccumulator -= ticksUntilThreshold * thresholdAddendum.Rate;

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

            basicTip = "";
            foreach (Addendum_Need_Rate thresholdAddendum in FallingRateAddendums)
            {
                if (levelAccumulator >= thresholdAddendum.Max)
                {
                    ticksUntilThreshold = TicksUntilThreshold(levelAccumulator, thresholdAddendum.Max, thresholdAddendum.Rate);
                    tickAccumulator += ticksUntilThreshold;
                    levelAccumulator -= ticksUntilThreshold * thresholdAddendum.Rate;

                    thresholdAddendum.Basic = thresholdAddendum.Translation.Translate(tickAccumulator.TicksToPeriod());
                }

                if (curLevel >= thresholdAddendum.Max)
                    basicTip += "\n" + thresholdAddendum.Basic;
            }

            basicTip = basicTip.Trim();
        }

        public override void UpdateDetailTip(int tickNow)
        {
            float curLevel;
            float levelAccumulator;
            int tickAccumulator;
            int ticksUntilThreshold;

            curLevel = need.CurLevel;
            levelAccumulator = need.MaxLevel;
            tickAccumulator = 0;

            detailedTip = string.Empty;
            foreach (Addendum_Need_Rate thresholdAddendum in FallingRateAddendums)
            {
                if (levelAccumulator >= thresholdAddendum.Max)
                {
                    ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, thresholdAddendum.Max, thresholdAddendum.Rate);
                    tickAccumulator += ticksUntilThreshold;
                    levelAccumulator -= ticksUntilThreshold * thresholdAddendum.Rate;

                    thresholdAddendum.Detail = (
                        thresholdAddendum.Basic
                        + "\n    " + "INI.Max".Translate(tickAccumulator.TicksToPeriod())
                    );
                }

                if (curLevel >= thresholdAddendum.Max)
                    detailedTip += "\n" + thresholdAddendum.Detail;
            }

            detailedTip = detailedTip.Trim();
            base.UpdateDetailTip(tickNow);
        }

        public virtual void UpdateRates(int tickNow)
        {
            ratesUpdatedAt = tickNow;
        }
    }
}