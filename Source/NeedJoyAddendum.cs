using RimWorld;
using RimWorld.Planet;
using System.Reflection;
using Verse;

namespace Improved_Need_Indicator
{

    public class NeedJoyAddendum : NeedAddendum
    {
        private Need_Joy needJoy;

        public NeedJoyAddendum(Need_Joy need) : base(need)
        {
            needJoy = need;

            // For future sanity.
            //
            // JoyCategory.High      => JoyTunings.ThreshVeryHigh  => "INI.Joy.Satisfied"
            // JoyCategory.Satisfied => JoyTunings.ThreshHigh      => "INI.Joy.Neutral"
            // JoyCategory.Low       => JoyTunings.ThreshSatisfied => "INI.Joy.Unfulfilled"
            // JoyCategory.VeryLow   => JoyTunings.ThreshLow       => "INI.Joy.Deprived"
            // JoyCategory.Empty     => 0f                         => "INI.Joy.Starved"
            fallingAddendums = new ThresholdAddendum[] {
                new ThresholdAddendum(
                    (byte)JoyCategory.High,
                    (byte)JoyCategory.Extreme,
                    JoyTunings.ThreshVeryHigh,
                    "INI.Joy.Satisfied"
                ),
                new ThresholdAddendum(
                    (byte)JoyCategory.Satisfied,
                    (byte)JoyCategory.High,
                    JoyTunings.ThreshHigh,
                    "INI.Joy.Neutral"
                ),
                new ThresholdAddendum(
                    (byte)JoyCategory.Low,
                    (byte)JoyCategory.Satisfied,
                    JoyTunings.ThreshSatisfied,
                    "INI.Joy.Unfulfilled"
                ),
                new ThresholdAddendum(
                    (byte)JoyCategory.VeryLow,
                    (byte)JoyCategory.Low,
                    JoyTunings.ThreshLow,
                    "INI.Joy.Deprived"
                ),
                new ThresholdAddendum(
                    (byte)JoyCategory.Empty,
                    (byte)JoyCategory.VeryLow,
                    0f,
                    "INI.Joy.Starved"
                ),
            };
        }

        public override void UpdateBasicTip(int tickNow)
        {
            base.UpdateBasicTip(tickNow);
        }

        public override void UpdateDetailedTip(int tickNow)
        {
            base.UpdateDetailedTip(tickNow);
        }

        public override void UpdateRates(int tickNow)
        {
            float curJoyFall = GetJoyFallPerTick();

            foreach (ThresholdAddendum threshold in fallingAddendums)
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
                default:
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
                default:
                    break;
            }

            return joyFall;
        }


        private static readonly MethodInfo
            get_FallPerInterval = typeof(Need_Joy)
                .GetProperty("FallPerInterval", Utility.flags).GetGetMethod(nonPublic: true);

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
            else
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
            else
                return (
                    (float)get_FallPerInterval.Invoke(needJoy, null)
                    / NeedTunings.NeedUpdateInterval
                    * pawn.GetStatValue(StatDefOf.JoyFallRateFactor)
                );
        }
#endif
    }
}
