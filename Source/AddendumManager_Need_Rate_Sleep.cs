using HarmonyLib;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_Rate_Sleep : AddendumManager_Need_Rate
    {

        private Need_Rest needRest;

        private float restGainPerTick;
        private string restNeededAddendum;


        public AddendumManager_Need_Rate_Sleep(Need_Rest need) : base(need)
        {
            needRest = (Need_Rest)this.need;

            FallingAddendums = new Addendum_Need_Rate[] {
                new Addendum_Need_Rate(
                    (byte)RestCategory.Tired,
                    Need_Rest.ThreshTired,
                    "INI.Rest.Tired",
                    (byte)RestCategory.Rested
                ),
                new Addendum_Need_Rate(
                    (byte)RestCategory.VeryTired,
                    Need_Rest.ThreshVeryTired,
                    "INI.Rest.VeryTired",
                    (byte)RestCategory.Tired
                ),
                new Addendum_Need_Rate(
                    (byte)RestCategory.Exhausted,
                    0f,
                    "INI.Rest.Exhausted",
                    (byte)RestCategory.VeryTired
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

        public override void UpdateBasicTip(int tickNow)
        {
            if (IsPawnResting(tickNow))
                UpdateBasicTipRising(tickNow, need.CurLevel);
            else
                UpdateBasicTipFalling(tickNow, need.CurLevel);

            basicUpdatedAt = tickNow;
        }

        protected override void UpdateBasicTipFalling(int tickNow, float curLevel)
        {
            int ticksUntilRested =
                TicksUntilThreshold(
                    need.MaxLevel,
                    curLevel - (pawn.TicksUntilNextUpdate() * GetRestFallPerTick()),
                    restGainPerTick
                );
            restNeededAddendum = "INI.Rest.Rested".Translate(ticksUntilRested.TicksToPeriod());

            base.UpdateBasicTipFalling(tickNow, curLevel);

            basicTip = restNeededAddendum + "\n" + basicTip;
            basicTip = basicTip.Trim();
        }

        protected override void UpdateBasicTipRising(int tickNow, float curLevel)
        {
            int tickOffset;
            int ticksUntilThreshold;

            tickOffset = pawn.TicksUntilNextUpdate();
            ticksUntilThreshold =
                TicksUntilThresholdUpdate(
                    need.MaxLevel,
                    curLevel,
                    restGainPerTick
                );
            restNeededAddendum =
                "INI.Rest.Rested".Translate((ticksUntilThreshold - tickOffset).TicksToPeriod());

            base.UpdateBasicTipRising(tickNow, curLevel + (tickOffset * restGainPerTick));

            basicTip = restNeededAddendum + "\n" + basicTip;
            basicTip = basicTip.Trim();
        }

        public override void UpdateDetailTip(int tickNow)
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

            foreach (Addendum_Need_Rate thresholdAddendum in FallingRateAddendums)
            {
                if (levelAccumulator >= thresholdAddendum.Threshold)
                {
                    ticksUntilThreshold =
                        TicksUntilThresholdUpdate(
                            levelAccumulator,
                            thresholdAddendum.Threshold,
                            thresholdAddendum.Rate
                        );
                    tickAccumulator += ticksUntilThreshold;
                    levelAccumulator -= ticksUntilThreshold * thresholdAddendum.Rate;
                    ticksUntilRest =
                        TicksUntilThresholdUpdate(
                            needRest.MaxLevel,
                            levelAccumulator,
                            restGainPerTick
                        );

                    thresholdAddendum.Detail = (
                        thresholdAddendum.Basic
                        + "\n\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod())
                        + "\n\t" + "INI.Rested".Translate(ticksUntilRest.TicksToPeriod())
                    );
                }

                if (curLevel >= thresholdAddendum.Threshold)
                    detailedTip += "\n" + thresholdAddendum.Detail;
            }

            detailedTip = detailedTip.Trim();
            detailUpdatedAt = tickNow;
        }

        public override void UpdateRates(int tickNow)
        {
            float curRestFall = GetRestFallPerTick();

            foreach (Addendum_Need_Rate threshold in FallingRateAddendums)
                threshold.Rate =
                    RestFallPerTickAssumingCategory(
                        (RestCategory)threshold.RateCategory,
                        curRestFall
                    );

            restGainPerTick = Need_Rest.BaseRestGainPerTick
                * fr_lastRestEffectivness(needRest)
                * pawn.GetStatValue(StatDefOf.RestRateMultiplier);

            base.UpdateRates(tickNow);
        }

        private float RestFallPerTickAssumingCategory(RestCategory category, float curRestFall)
        {
            float restFall = curRestFall;

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

        private static readonly AccessTools.FieldRef<Need_Rest, float>
            fr_lastRestEffectivness = AccessTools.FieldRefAccess<Need_Rest, float>("lastRestEffectiveness");
    }
}
