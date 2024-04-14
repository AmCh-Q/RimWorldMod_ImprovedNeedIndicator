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

        public override void UpdateBasic(int tickNow)
        {
            // If the amount of time needed till is going up rather than down,
            // then the offset is necessary to calculate the difference in
            // time between now and the previous interval update ticks.
            float levelAccumulator;
            int tickAccumulator;
            int tickOffset;
            int ticksUntilThreshold;

            levelAccumulator = 0f;
            tickOffset = pawn.TicksUntilNextUpdate();
            tickAccumulator = 0;

            string DoThreshold(string translation, float threshold, float localFallPerTick)
            {
                ticksUntilThreshold = TicksUntilThreshold(levelAccumulator, threshold, localFallPerTick);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * localFallPerTick;
                return translation.Translate(tickAccumulator.TicksToPeriod());
            }

            string DoThresholdUpdate(string translation, float threshold, float localFallPerTick)
            {
                ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, threshold, localFallPerTick);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * localFallPerTick;
                return translation.Translate((tickAccumulator - tickOffset).TicksToPeriod());
            }

            if (IsPawnResting(tickNow))
            {
                ticksUntilThreshold = TicksUntilThresholdUpdate(need.MaxLevel, needRest.CurLevel, gainPerTick);
                restNeededAddendum = "INI.Rest.RestNeeded".Translate((ticksUntilThreshold - tickOffset).TicksToPeriod());

                levelAccumulator = needRest.CurLevel + (tickOffset * gainPerTick);

                if (need.CurLevel >= Need_Rest.ThreshTired)
                {
                    tiredAddendum = DoThreshold("INI.Rest.Tired", Need_Rest.ThreshTired, fallPerTick);
                    veryTiredAddendum = DoThreshold("INI.Rest.VeryTired", Need_Rest.ThreshVeryTired, fallPerTick * .7f);
                    exhaustedAddendum = DoThreshold("INI.Rest.Exhausted", 0f, fallPerTick * .3f);

                    basicTip = string.Join("\n", new[] { restNeededAddendum, tiredAddendum, veryTiredAddendum, exhaustedAddendum });
                }
                else if (need.CurLevel >= Need_Rest.ThreshVeryTired)
                {
                    veryTiredAddendum = DoThreshold("INI.Rest.VeryTired", Need_Rest.ThreshVeryTired, fallPerTick);
                    exhaustedAddendum = DoThreshold("INI.Rest.Exhausted", 0f, fallPerTick * (.3f / 0.7f));

                    basicTip = string.Join("\n", new[] { restNeededAddendum, veryTiredAddendum, exhaustedAddendum });
                }
                else if (need.CurLevel > 0f)
                {
                    exhaustedAddendum = DoThreshold("INI.Rest.Exhausted", 0f, fallPerTick);

                    basicTip = string.Join("\n", new[] { restNeededAddendum, exhaustedAddendum });
                }
            }
            else
            {
                levelAccumulator = needRest.CurLevel - (tickOffset * fallPerTick);
                ticksUntilThreshold = TicksUntilThreshold(need.MaxLevel, levelAccumulator, gainPerTick);
                restNeededAddendum = "INI.Rest.RestNeeded".Translate(ticksUntilThreshold.TicksToPeriod());

                levelAccumulator = needRest.CurLevel;

                if (need.CurLevel >= Need_Rest.ThreshTired)
                {
                    tiredAddendum = DoThresholdUpdate("INI.Rest.Tired", Need_Rest.ThreshTired, fallPerTick);
                    veryTiredAddendum = DoThresholdUpdate("INI.Rest.VeryTired", Need_Rest.ThreshVeryTired, fallPerTick * .7f);
                    exhaustedAddendum = DoThresholdUpdate("INI.Rest.Exhausted", 0f, fallPerTick * .3f);

                    basicTip = string.Join("\n", new[] { restNeededAddendum, tiredAddendum, veryTiredAddendum, exhaustedAddendum });
                }
                else if (need.CurLevel >= Need_Rest.ThreshVeryTired)
                {
                    veryTiredAddendum = DoThresholdUpdate("INI.Rest.VeryTired", Need_Rest.ThreshVeryTired, fallPerTick);
                    exhaustedAddendum = DoThresholdUpdate("INI.Rest.Exhausted", 0f, fallPerTick * (.3f / 0.7f));

                    basicTip = string.Join("\n", new[] { restNeededAddendum, veryTiredAddendum, exhaustedAddendum });

                }
                else if (need.CurLevel > 0f)
                {
                    exhaustedAddendum = DoThresholdUpdate("INI.Rest.Exhausted", 0f, fallPerTick);

                    basicTip = string.Join("\n", new[] { restNeededAddendum, exhaustedAddendum });
                }
            }

            base.UpdateBasic(tickNow);
        }

        public override void UpdateDetailed(int tickNow)
        {
            float levelAccumulator;
            int ticksUntilThreshold;
            int tickAccumulator;
            int ticksUntilRest;

            levelAccumulator = need.MaxLevel;
            tickAccumulator = 0;
            ticksUntilThreshold = 0;

            void DoThresholdUpdate(float threshold, float localFallPerTick)
            {
                ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, threshold, localFallPerTick);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * localFallPerTick;
                ticksUntilRest = TicksUntilThresholdUpdate(needRest.MaxLevel, levelAccumulator, gainPerTick);
            }

            if (need.CurLevel >= Need_Rest.ThreshTired)
            {
                DoThresholdUpdate(Need_Rest.ThreshTired, fallPerTick);
                maxTiredAddendum = "\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod());
                maxTiredRestNeededAddendum = "\t" + "INI.Rest.MaxRestNeeded".Translate(ticksUntilRest.TicksToPeriod());

                DoThresholdUpdate(Need_Rest.ThreshVeryTired, fallPerTick * 0.7f);
                maxVeryTiredAddendum = "\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod());
                maxVeryTiredRestNeededAddendum = "\t" + "INI.Rest.MaxRestNeeded".Translate(ticksUntilRest.TicksToPeriod());

                DoThresholdUpdate(0f, fallPerTick * 0.3f);
                maxExhaustedAddendum = "\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod());
                maxExhaustedRestNeededAddendum = "\t" + "INI.Rest.MaxRestNeeded".Translate(ticksUntilRest.TicksToPeriod());

                detailedTip = string.Join("\n", new[] {
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
            }
            else if (need.CurLevel >= Need_Rest.ThreshVeryTired)
            {
                DoThresholdUpdate(Need_Rest.ThreshVeryTired, fallPerTick);
                maxVeryTiredAddendum = "\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod());
                maxVeryTiredRestNeededAddendum = "\t" + "INI.Rest.MaxRestNeeded".Translate(ticksUntilRest.TicksToPeriod());

                DoThresholdUpdate(0f, fallPerTick * (0.3f / 0.7f));
                maxExhaustedAddendum = "\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod());
                maxExhaustedRestNeededAddendum = "\t" + "INI.Rest.MaxRestNeeded".Translate(ticksUntilRest.TicksToPeriod());

                detailedTip = string.Join("\n", new[] {
                    restNeededAddendum,
                    veryTiredAddendum,
                    maxVeryTiredRestNeededAddendum,
                    maxVeryTiredAddendum,
                    exhaustedAddendum,
                    maxExhaustedAddendum,
                    maxExhaustedRestNeededAddendum
                });
            }
            else if (need.CurLevel > 0f)
            {
                DoThresholdUpdate(0f, fallPerTick);
                maxExhaustedAddendum = "\t" + "INI.Max".Translate(tickAccumulator.TicksToPeriod());
                maxExhaustedRestNeededAddendum = "\t" + "INI.Rest.MaxRestNeeded".Translate(ticksUntilRest.TicksToPeriod());

                detailedTip = string.Join("\n", new[] {
                    restNeededAddendum,
                    exhaustedAddendum,
                    maxExhaustedAddendum,
                    maxExhaustedRestNeededAddendum
                });
            }


            base.UpdateDetailed(tickNow);
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
