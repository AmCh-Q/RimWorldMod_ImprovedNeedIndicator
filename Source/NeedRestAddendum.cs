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

        private int tiredTick;
        private int veryTiredTick;
        private int exhaustedTick;

        private float fallPerTick;
        private float restGainPerTick;

        private string restNeededAddendum;
        private string tiredAddendum;
        private string veryTiredAddendum;
        private string exhaustedAddendum;

        public NeedRestAddendum(Need_Rest need) : base(need)
        {
            needRest = (Need_Rest)this.need;
        }

#if (v1_2 || v1_3)
        // Need.Resting used to be private in 1.2/1.3
        //   we will copy its implementation using field Need.lastRestTick
        private static readonly AccessTools.FieldRef<Need_Rest, float>
            fr_lastRestTick = AccessTools.FieldRefAccess<Need_Rest, float>("lastRestTick");

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
            {
                UpdateTiredAddendum(tickNow);
                UpdateVeryTiredAddendum(tickNow);
                UpdateExhaustedAddendum(tickNow);

                basicTipAddendum = string.Join("\n", new[] { restNeededAddendum, tiredAddendum, veryTiredAddendum, exhaustedAddendum });
            }
            else if (need.CurLevel >= Need_Rest.ThreshVeryTired)
            {
                UpdateVeryTiredAddendum(tickNow);
                UpdateExhaustedAddendum(tickNow);

                basicTipAddendum = string.Join("\n", new[] { restNeededAddendum, veryTiredAddendum, exhaustedAddendum });
            }
            else if (need.CurLevel > 0f)
            {
                UpdateExhaustedAddendum(tickNow);

                basicTipAddendum = string.Join("\n", new[] { restNeededAddendum, exhaustedAddendum });
            }
            else
            {
                basicTipAddendum = restNeededAddendum;
            }

            base.UpdateTickRates(tickNow);
        }

        private void UpdateRestNeededAddendum()
        {
            restNeededAddendum = "INI.Rest.RestNeeded".Translate(
                TicksUntilThreshold(need.MaxLevel, need.CurLevel, restGainPerTick)
                    .TicksToPeriod()
            );
        }

        private void UpdateTiredAddendum(int tickNow)
        {
            tiredAddendum = "INI.Rest.Tired".Translate((tiredTick - tickNow).TicksToPeriod());
        }

        private void UpdateVeryTiredAddendum(int tickNow)
        {
            veryTiredAddendum = "INI.Rest.VeryTired".Translate((veryTiredTick - tickNow).TicksToPeriod());
        }

        private void UpdateExhaustedAddendum(int tickNow)
        {
            exhaustedAddendum = "INI.Rest.Exhausted".Translate((exhaustedTick - tickNow).TicksToPeriod());
        }

        public override void UpdateTickRates(int tickNow)
        {
            float localFallPerTick;
            float levelAccumulator;
            int tickAccumulator;
            int tickOffset;
            int ticksUntilThreshold;

            levelAccumulator = needRest.CurLevel;
            tickAccumulator = 0;
            tickOffset = pawn.TicksUntilNextUpdate();
            ticksUntilThreshold = 0;

            UpdateRestFallPerTick();
            UpdateRestGainPerTick();

            if (IsPawnResting(tickNow))
                levelAccumulator += tickOffset * restGainPerTick;
            else
                levelAccumulator -= tickOffset * fallPerTick;

            void DoThreshold(float threshold)
            {
                ticksUntilThreshold = TicksUntilThreshold(levelAccumulator, threshold, localFallPerTick);
                tickAccumulator += ticksUntilThreshold;
                levelAccumulator -= ticksUntilThreshold * localFallPerTick;
            }

            if (need.CurLevel >= Need_Rest.ThreshTired)
            {
                localFallPerTick = fallPerTick;
                DoThreshold(Need_Rest.ThreshTired);
                tiredTick = tickNow + tickAccumulator;

                localFallPerTick =  fallPerTick * 0.7f;
                DoThreshold(Need_Rest.ThreshVeryTired);
                veryTiredTick = tickNow + tickAccumulator;

                localFallPerTick = fallPerTick * 0.3f;
                DoThreshold(0f);
                exhaustedTick = tickNow + ticksUntilThreshold;
            }
            else if (need.CurLevel >= Need_Rest.ThreshVeryTired)
            {
                localFallPerTick = fallPerTick;
                DoThreshold(Need_Rest.ThreshVeryTired);
                veryTiredTick = tickNow + tickAccumulator;

                localFallPerTick = fallPerTick * (0.3f / 0.7f);
                DoThreshold(0f);
                exhaustedTick = tickNow + ticksUntilThreshold;
            }
            else if (need.CurLevel > 0f)
            {
                localFallPerTick = fallPerTick;
                DoThreshold(0f);
                exhaustedTick = tickNow + ticksUntilThreshold;
            }

            base.UpdateTickRates(tickNow);
        }

#if (v1_2 || v1_3)
        // StatDefOf.RestFallRateFactor did not exist back in 1.2/1.3
        private void UpdateRestFallPerTick()
        {
            fallPerTick = needRest.RestFallPerTick;
        }
#else
        // StatDefOf.RestFallRateFactor did not exist back in 1.2/1.3
        private void UpdateRestFallPerTick()
        {
            fallPerTick = needRest.RestFallPerTick * pawn.GetStatValue(StatDefOf.RestFallRateFactor);
        }
#endif

        private void UpdateRestGainPerTick()
        {
            restGainPerTick = Need_Rest.BaseRestGainPerTick
                * fr_lastRestEffectivness(needRest)
                * pawn.GetStatValue(StatDefOf.RestRateMultiplier);
        }
    }
}
