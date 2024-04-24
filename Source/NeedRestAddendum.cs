using HarmonyLib;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class NeedRestAddendum : NeedAddendum
    {
        private static readonly AccessTools.FieldRef<Need_Rest, float>
            fr_lastRestEffectivness = AccessTools.FieldRefAccess<Need_Rest, float>("lastRestEffectiveness");

        private Need_Rest needRest;

        private float restGainPerTick;
        private string restNeededAddendum;


        public NeedRestAddendum(Need_Rest need) : base(need)
        {
            needRest = (Need_Rest)this.need;

            fallingAddendums = new ThresholdAddendum[] {
                new ThresholdAddendum(
                    (byte)RestCategory.Tired,
                    (byte)RestCategory.Rested,
                    Need_Rest.ThreshTired,
                    "INI.Rest.Tired"
                ),
                new ThresholdAddendum(
                    (byte)RestCategory.VeryTired,
                    (byte)RestCategory.Tired,
                    Need_Rest.ThreshVeryTired,
                    "INI.Rest.VeryTired"
                ),
                new ThresholdAddendum(
                    (byte)RestCategory.Exhausted,
                    (byte)RestCategory.VeryTired,
                    0f,
                    "INI.Rest.Exhausted"
                )
            };
        }

#if (v1_2 || v1_3) 
        // Need.Resting used to be private in 1.2/1.3
        //   we will copy its implementation using field Need.lastRestTick
        private static readonly AccessTools.FieldRef<Need_Rest, int>
            fr_lastRestTick = AccessTools.FieldRefAccess<Need_Rest, int>("lastRestTick");

        private bool IsPawnResting(int tickNow)
        {
            return tickNow < (int)fr_lastRestTick(needRest) + 2;
        }
#else
        private bool IsPawnResting(int tickNow)
        {
            return needRest.Resting;
        }
#endif

        private void UpdateBasicTipWhileResting(int tickNow)
        {
            float levelAccumulator;
            int tickAccumulator;
            int tickOffset;
            int ticksUntilThreshold;

            levelAccumulator = 0f;
            tickOffset = pawn.TicksUntilNextUpdate();
            tickAccumulator = 0;

            void HandleThresholdAddendum(ThresholdAddendum thresholdAddendum)
            {
                ticksUntilThreshold = TicksUntilThreshold(levelAccumulator, thresholdAddendum.Threshold, thresholdAddendum.Rate);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * thresholdAddendum.Rate;

                thresholdAddendum.Translation.Translate(tickAccumulator.TicksToPeriod());
            }

            ticksUntilThreshold = TicksUntilThresholdUpdate(need.MaxLevel, needRest.CurLevel, restGainPerTick);
            restNeededAddendum = "INI.Rest.RestNeeded".Translate((ticksUntilThreshold - tickOffset).TicksToPeriod());

            basicTip = restNeededAddendum;
            levelAccumulator = needRest.CurLevel + (tickOffset * restGainPerTick);

            foreach (ThresholdAddendum thresholdAddendum in fallingAddendums)
                if (levelAccumulator >= thresholdAddendum.Threshold)
                {
                    HandleThresholdAddendum(thresholdAddendum);
                    basicTip += "\n" + thresholdAddendum.BasicAddendum;
                }

            basicTip = basicTip.Trim();
            basicUpdatedAt = tickNow;
        }

        public override void UpdateBasicTip(int tickNow)
        {
            if (IsPawnResting(tickNow))
                UpdateBasicTipWhileResting(tickNow);
            else
            {
                int ticksUntilRested = TicksUntilThreshold(
                    need.MaxLevel,
                    needRest.CurLevel - (pawn.TicksUntilNextUpdate() * GetRestFallPerTick()),
                    restGainPerTick
                );
                restNeededAddendum = "INI.Rest.RestNeeded".Translate(ticksUntilRested.TicksToPeriod());

                base.UpdateBasicTip(tickNow);

                basicTip = restNeededAddendum + "\n" + basicTip;
            }
        }

        public override void UpdateDetailedTip(int tickNow)
        {
            float curLevel;
            float levelAccumulator;
            int ticksUntilThreshold;
            int tickAccumulator;
            int ticksUntilRest;

            curLevel = need.CurLevel;
            levelAccumulator = need.MaxLevel;
            tickAccumulator = 0;
            ticksUntilThreshold = 0;

            detailedTip = restNeededAddendum;

            void HandleThresholdAddendum(ThresholdAddendum thresholdAddendum)
            {
                ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, thresholdAddendum.Threshold, thresholdAddendum.Rate);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * thresholdAddendum.Rate;
                ticksUntilRest = TicksUntilThresholdUpdate(needRest.MaxLevel, levelAccumulator, restGainPerTick);

                thresholdAddendum.DetailedAddendum = (
                    thresholdAddendum.BasicAddendum
                    + "\n\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod())
                    + "\n\t" + "INI.Rest.MaxRestNeeded".Translate(ticksUntilRest.TicksToPeriod())
                );
            }

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

        public override void UpdateRates(int tickNow)
        {
            foreach (ThresholdAddendum threshold in fallingAddendums)
                threshold.Rate = RestFallPerTickAssumingCategory((RestCategory)threshold.RateCategory);

            restGainPerTick = Need_Rest.BaseRestGainPerTick
                * fr_lastRestEffectivness(needRest)
                * pawn.GetStatValue(StatDefOf.RestRateMultiplier);

            base.UpdateRates(tickNow);
        }

        private float RestFallPerTickAssumingCategory(RestCategory category)
        {
            float restFall = GetRestFallPerTick();

            switch (needRest.CurCategory)
            {
                case RestCategory.Rested:
                    break;
                case RestCategory.Tired:
                    restFall = restFall / .7f;
                    break;
                case RestCategory.VeryTired:
                    restFall = restFall / .3f;
                    break;
                case RestCategory.Exhausted:
                    restFall = restFall / .559f;
                    break;
            }

            switch (category)
            {
                case RestCategory.Rested:
                    break;
                case RestCategory.Tired:
                    restFall *= .7f;
                    break;
                case RestCategory.VeryTired:
                    restFall *= .3f;
                    break;
                case RestCategory.Exhausted:
                    restFall *= .559f;
                    break;
            }

            return restFall;
        }

#if (v1_2 || v1_3)
        // StatDefOf.RestFallRateFactor did not exist back in 1.2/1.3
        private float GetRestFallPerTick()
        {
            return needRest.RestFallPerTick;
        }
#else
        private float GetRestFallPerTick()
        {
            return needRest.RestFallPerTick * pawn.GetStatValue(StatDefOf.RestFallRateFactor);
        }
#endif
    }
}
