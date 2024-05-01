using RimWorld;
using RimWorld.Planet;
using System.Reflection;
using Verse;

namespace Improved_Need_Indicator
{

    public class AddendumManager_Need_Rate_Joy : AddendumManager_Need_Rate
    {
        private Need_Joy needJoy;

        public AddendumManager_Need_Rate_Joy(Need_Joy need) : base(need)
        {
            needJoy = need;

            FallingAddendums = new Addendum_Need_Rate[] {
                new Addendum_Need_Rate(
                    (byte)JoyCategory.Extreme,
                    need.MaxLevel,
                    JoyTunings.ThreshVeryHigh,
                    "INI.Joy.FullySatisfied",
                    (byte)JoyCategory.Extreme
                ),
                new Addendum_Need_Rate(
                    (byte)JoyCategory.High,
                    JoyTunings.ThreshVeryHigh,
                    JoyTunings.ThreshHigh,
                    "INI.Joy.Satisfied",
                    (byte)JoyCategory.Extreme
                ),
                new Addendum_Need_Rate(
                    (byte)JoyCategory.Satisfied,
                    JoyTunings.ThreshHigh,
                    JoyTunings.ThreshSatisfied,
                    "INI.Neutral",
                    (byte)JoyCategory.High
                ),
                new Addendum_Need_Rate(
                    (byte)JoyCategory.Low,
                    JoyTunings.ThreshSatisfied,
                    JoyTunings.ThreshLow,
                    "INI.Joy.Unfulfilled",
                    (byte)JoyCategory.Satisfied
                ),
                new Addendum_Need_Rate(
                    (byte)JoyCategory.VeryLow,
                    JoyTunings.ThreshLow,
                    0.0001f,
                    "INI.Joy.Deprived",
                    (byte)JoyCategory.Low
                ),
                new Addendum_Need_Rate(
                    (byte)JoyCategory.Empty,
                    0f,
                    0f,
                    "INI.Joy.Starved",
                    (byte)JoyCategory.VeryLow
                )
            };
        }

        public override void UpdateBasicTip(int tickNow)
        {
            base.UpdateBasicTip(tickNow);
        }

        public override void UpdateDetailTip(int tickNow)
        {
            base.UpdateDetailTip(tickNow);
        }

        public override void UpdateRates(int tickNow)
        {
            float curJoyFall = GetJoyFallPerTick();

            foreach (Addendum_Need_Rate threshold in FallingRateAddendums)
                threshold.Rate =
                    JoyFallPerTickAssumingCategory(
                        (JoyCategory)threshold.RateCategory,
                        curJoyFall
                    );

            base.UpdateRates(tickNow);
        }

        private float JoyFallPerTickAssumingCategory(JoyCategory category, float curJoyFall)
        {
            float joyFall = curJoyFall;

            switch (needJoy.CurCategory)
            {
                case JoyCategory.Low:
                    joyFall = joyFall / JoyTunings.FallRateFactorWhenLow;
                    break;

                case JoyCategory.VeryLow:
                    joyFall = joyFall / JoyTunings.FallRateFactorWhenVeryLow;
                    break;
            }

            switch (category)
            {
                case JoyCategory.Low:
                    joyFall = joyFall * JoyTunings.FallRateFactorWhenLow;
                    break;

                case JoyCategory.VeryLow:
                    joyFall = joyFall * JoyTunings.FallRateFactorWhenVeryLow;
                    break;
            }

            return joyFall;
        }

        private static readonly MethodInfo get_FallPerInterval = typeof(Need_Joy).GetProperty("FallPerInterval", Utility.flags).GetGetMethod(true);

#if v1_2
        private float GetJoyFallPerTick()
        {
            return (
                (float)get_FallPerInterval.Invoke(needJoy, null)
                / NeedTunings.NeedUpdateInterval
            );
        }
#elif v1_3 || v1_4
        private float GetJoyFallPerTick()
        {
            if (pawn.IsFormingCaravan())
                return (
                    (float)get_FallPerInterval.Invoke(needJoy, null)
                    / NeedTunings.NeedUpdateInterval
                    * JoyTunings.FallRateFactorWhenFormingCaravan
                );

            return (
                (float)get_FallPerInterval.Invoke(needJoy, null)
                / NeedTunings.NeedUpdateInterval
            );
        }
#else
        private float GetJoyFallPerTick()
        {
            if (pawn.IsFormingCaravan())
                return (
                    (float)get_FallPerInterval.Invoke(needJoy, null)
                    / NeedTunings.NeedUpdateInterval
                    * pawn.GetStatValue(StatDefOf.JoyFallRateFactor)
                    * JoyTunings.FallRateFactorWhenFormingCaravan
                );

            return (
                (float)get_FallPerInterval.Invoke(needJoy, null)
                / NeedTunings.NeedUpdateInterval
                * pawn.GetStatValue(StatDefOf.JoyFallRateFactor)
            );
        }
#endif
    }
}
