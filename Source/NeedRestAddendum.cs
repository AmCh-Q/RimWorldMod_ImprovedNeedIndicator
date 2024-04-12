using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Improved_Need_Indicator
{
    public class NeedRestAddendum : NeedAddendum
    {
        private static readonly AccessTools.FieldRef<Need_Rest, float>
            fr_lastRestEffectivness = AccessTools.FieldRefAccess<Need_Rest, float>("lastRestEffectiveness");

        private Need_Rest needRest;

        private float fallPerTick;
        private float gainPerTick;

        private int restedAt;
        private int tiredAt;
        private int veryTiredAt;
        private int exhaustedAt;

        private string restNeededAddendum;
        private string tiredAddendum;
        private string veryTiredAddendum;
        private string exhaustedAddendum;

        private string maxTiredAddendum;
        private string maxTiredRestNeededAddendum;
        private string maxVeryTiredAddendum;
        private string maxVeryTiredRestNeededAddendum;
        private string maxExhaustedAddendum;
        private string maxExhaustedRestNeededAddendum;


        public NeedRestAddendum(Need_Rest need) : base(need)
        {
            needRest = (Need_Rest)this.need;
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
        public override void UpdateBasicTipAddendum(int tickNow)
        {
            if (need.CurLevel >= Need_Rest.ThreshTired)
                basicTipAddendum = string.Join("\n", new[] { restNeededAddendum, tiredAddendum, veryTiredAddendum, exhaustedAddendum });

            else if (need.CurLevel >= Need_Rest.ThreshVeryTired)
                basicTipAddendum = string.Join("\n", new[] { restNeededAddendum, veryTiredAddendum, exhaustedAddendum });

            else if (need.CurLevel > 0f)
                basicTipAddendum = string.Join("\n", new[] { restNeededAddendum, exhaustedAddendum });

            else
                basicTipAddendum = restNeededAddendum;

            base.UpdateBasicTipAddendum(tickNow);
        }


        public override void UpdateBasicAddendums(int tickNow)
        {
            // If the amount of time needed till is going up rather than down,
            // then the offset is necessary to calculate the difference in
            // time between now and the previous interval update ticks.
            int tickOffset;
            int tickRestNeededOffset;
            int tickThresholdOffset;

            tickOffset = pawn.TicksUntilNextUpdate();
            if (IsPawnResting(tickNow))
            {
                tickRestNeededOffset = tickNow + tickOffset;
                tickThresholdOffset = tickNow; //TODO: blugh some ratio goes here.
            }
            else
            {
                tickRestNeededOffset = tickNow; //TODO: blugh some ratio goes here.
                tickThresholdOffset = tickNow + tickOffset;
            }

            restNeededAddendum = "INI.Rest.RestNeeded".Translate((restedAt - tickRestNeededOffset).TicksToPeriod());
            tiredAddendum = "INI.Rest.Tired".Translate((tiredAt - tickThresholdOffset).TicksToPeriod());
            veryTiredAddendum = "INI.Rest.VeryTired".Translate((veryTiredAt - tickThresholdOffset).TicksToPeriod());
            exhaustedAddendum = "INI.Rest.Exhausted".Translate((exhaustedAt - tickThresholdOffset).TicksToPeriod());

            base.UpdateBasicAddendums(tickNow);
        }

        public override void UpdateDetailedTipAddendum(int tickNow)
        {
            if (need.CurLevel >= Need_Rest.ThreshTired)
                detailedTipAddendum = string.Join("\n", new[] {
                    restNeededAddendum,
                    tiredAddendum,
                    maxTiredAddendum,
                    maxTiredRestNeededAddendum,
                    veryTiredAddendum,
                    maxVeryTiredRestNeededAddendum,
                    maxVeryTiredAddendum,
                    exhaustedAddendum,
                    maxExhaustedAddendum,
                    maxExhaustedRestNeededAddendum
                });

            else if (need.CurLevel >= Need_Rest.ThreshVeryTired)
                detailedTipAddendum = string.Join("\n", new[] {
                    restNeededAddendum,
                    veryTiredAddendum,
                    maxVeryTiredRestNeededAddendum,
                    maxVeryTiredAddendum,
                    exhaustedAddendum,
                    maxExhaustedAddendum,
                    maxExhaustedRestNeededAddendum
                });

            else if (need.CurLevel > 0f)
                detailedTipAddendum = string.Join("\n", new[] {
                    restNeededAddendum,
                    exhaustedAddendum,
                    maxExhaustedAddendum,
                    maxExhaustedRestNeededAddendum
                });

            base.UpdateDetailedTipAddendum(tickNow);
        }

        public override void UpdateDetailedAddendums(int tickNow)
        {
            float levelAccumulator;
            int ticksUntilThreshold;
            int tickAccumulator;
            int ticksUntilRest;

            levelAccumulator = need.MaxLevel;
            tickAccumulator = 0;
            ticksUntilThreshold = 0;

            void DoThreshold(float threshold, float localFallPerTick)
            {
                ticksUntilThreshold = TicksUntilThreshold(levelAccumulator, threshold, localFallPerTick);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * localFallPerTick;
                ticksUntilRest = TicksUntilThreshold(needRest.MaxLevel, levelAccumulator, gainPerTick);
            }

            DoThreshold(Need_Rest.ThreshTired, fallPerTick);
            maxTiredAddendum = "\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod());
            maxTiredRestNeededAddendum = "\t" + "INI.Rest.MaxRestNeeded".Translate(ticksUntilRest.TicksToPeriod());

            DoThreshold(Need_Rest.ThreshVeryTired, fallPerTick * 0.7f);
            maxVeryTiredAddendum = "\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod());
            maxVeryTiredRestNeededAddendum = "\t" + "INI.Rest.MaxRestNeeded".Translate(ticksUntilRest.TicksToPeriod());

            DoThreshold(0f, fallPerTick * 0.3f);
            maxExhaustedAddendum = "\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod());
            maxExhaustedRestNeededAddendum = "\t" + "INI.Rest.MaxRestNeeded".Translate(ticksUntilRest.TicksToPeriod());

            base.UpdateBasicAddendums(tickNow);
        }

        public override void UpdateThresholdAts(int tickNow)
        {
            float levelAccumulator;
            int ticksUntilThreshold;
            int tickAccumulator;

            restedAt = tickNow + TicksUntilThreshold(needRest.MaxLevel, needRest.CurLevel, gainPerTick);

            levelAccumulator = needRest.CurLevel;
            tickAccumulator = 0;
            ticksUntilThreshold = 0;

            void DoThreshold(float threshold, float localFallPerTick)
            {
                ticksUntilThreshold = TicksUntilThreshold(levelAccumulator, threshold, localFallPerTick);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * localFallPerTick;
            }

            if (need.CurLevel >= Need_Rest.ThreshTired)
            {
                DoThreshold(Need_Rest.ThreshTired, fallPerTick);
                tiredAt = tickNow + tickAccumulator;

                DoThreshold(Need_Rest.ThreshVeryTired, fallPerTick * 0.7f);
                veryTiredAt = tickNow + tickAccumulator;

                DoThreshold(0f, fallPerTick * 0.3f);
                exhaustedAt = tickNow + tickAccumulator;
            }
            else if (need.CurLevel >= Need_Rest.ThreshVeryTired)
            {
                DoThreshold(Need_Rest.ThreshVeryTired, fallPerTick);
                veryTiredAt = tickNow + tickAccumulator;

                DoThreshold(0f, fallPerTick * (0.3f / 0.7f));
                exhaustedAt = tickNow + tickAccumulator;
            }
            else if (need.CurLevel > 0f)
            {
                DoThreshold(0f, fallPerTick);
                exhaustedAt = tickNow + tickAccumulator;
            }

            base.UpdateThresholdAts(tickNow);
        }

        public override void UpdateTickRates(int tickNow)
        {

            UpdateRestFallPerTick();
            UpdateRestGainPerTick();

            base.UpdateTickRates(tickNow);
        }

#if (v1_2 || v1_3)
        // StatDefOf.RestFallRateFactor did not exist back in 1.2/1.3
        private void UpdateRestFallPerTick()
        {
            fallPerTick = needRest.RestFallPerTick;
        }
#else
        private void UpdateRestFallPerTick()
        {
            fallPerTick = needRest.RestFallPerTick * pawn.GetStatValue(StatDefOf.RestFallRateFactor);
        }
#endif

        private void UpdateRestGainPerTick()
        {
            gainPerTick = Need_Rest.BaseRestGainPerTick
                * fr_lastRestEffectivness(needRest)
                * pawn.GetStatValue(StatDefOf.RestRateMultiplier);
        }
    }
}
