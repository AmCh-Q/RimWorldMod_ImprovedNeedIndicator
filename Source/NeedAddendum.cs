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

        protected ThresholdAddendum[] fallingAddendums = { };

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

        public virtual void UpdateBasicTip(int tickNow)
        {
            float levelAccumulator;
            int tickAccumulator;
            int tickOffset;
            int ticksUntilThreshold;

            levelAccumulator = need.CurLevel;
            tickAccumulator = 0;
            tickOffset = pawn.TicksUntilNextUpdate();

            void HandleThresholdAddendum(ThresholdAddendum thresholdAddendum)
            {
                ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, thresholdAddendum.Threshold, thresholdAddendum.Rate);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * thresholdAddendum.Rate;

                thresholdAddendum.BasicAddendum = thresholdAddendum.Translation.Translate((tickAccumulator - tickOffset).TicksToPeriod());
            }

            basicTip = "";
            foreach (ThresholdAddendum thresholdAddendum in fallingAddendums)
                if (levelAccumulator > thresholdAddendum.Threshold)
                {
                    HandleThresholdAddendum(thresholdAddendum);
                    basicTip += "\n" + thresholdAddendum.BasicAddendum;
                }

            basicTip = basicTip.Trim();

            basicUpdatedAt = tickNow;
        }

        public virtual void UpdateDetailedTip(int tickNow)
        {
            float curLevel;
            float levelAccumulator;
            int tickAccumulator;
            int ticksUntilThreshold;

            curLevel = need.CurLevel;
            levelAccumulator = need.MaxLevel;
            tickAccumulator = 0;

            void HandleThresholdAddendum(ThresholdAddendum thresholdAddendum)
            {
                ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, thresholdAddendum.Threshold, thresholdAddendum.Rate);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * thresholdAddendum.Rate;

                thresholdAddendum.DetailedAddendum = (
                    thresholdAddendum.BasicAddendum
                    + "\n\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod())
                );
            }

            detailedTip = string.Empty;
            foreach (ThresholdAddendum thresholdAddendum in fallingAddendums)
            {
                if (levelAccumulator >= thresholdAddendum.Threshold)
                    HandleThresholdAddendum(thresholdAddendum);

                if (curLevel >= thresholdAddendum.Threshold)
                    detailedTip += "\n" + thresholdAddendum.DetailedAddendum;
            }

            detailedTip = detailedTip.Trim();
            detailedUpdatedAt = tickNow;
        }

        public virtual void UpdateRates(int tickNow)
        {
            ratesUpdatedAt = tickNow;
        }
    }


    public class ThresholdAddendum
    {
        public byte Category { get; protected set; }
        public float Rate { get; set; }
        public byte RateCategory { get; protected set; }
        public float Threshold { get; protected set; }
        public string Translation { get; protected set; }

        public string BasicAddendum { get; set; }
        public string DetailedAddendum { get; set; }

        public ThresholdAddendum(
            byte category,
            byte rateCategory,
            float threshold,
            string translation
        )
        {
            Category = category;
            Rate = 0f;
            RateCategory = rateCategory;
            Threshold = threshold;
            Translation = translation;
        }
    }
}