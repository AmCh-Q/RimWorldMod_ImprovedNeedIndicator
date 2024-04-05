using System.Reflection;
using UnityEngine;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public static class Utility
    {
        public const BindingFlags flags
            = BindingFlags.Instance
            | BindingFlags.Static
            | BindingFlags.Public
            | BindingFlags.NonPublic;

        public static bool IsCloseTo(this float a, float b, float margin)
        {
            return System.Math.Abs(a - b) < margin;
        }

        public static int RoundUpTickToMultipleOfInterval(this float ticksTo, int interval)
        {
            // Round up to multiples of interval ticks
            return Mathf.CeilToInt(ticksTo / interval) * interval;
        }

        public static int TicksToNextUpdateTick(this Thing thing, int interval)
        {
            // In vanilla, need updates are made when
            //   thing.HashOffsetTicks() % interval == 0
            // We want to find the time since the last update
            //   so that we can correct the (rounded up)
            //   CeilToUpdate() calculation


            // We first calculate the remainder
            //   by doing the same thing as vanilla w/o comparison
            int result = (thing.HashOffsetTicks()) % interval;

            // In vanilla, "to be zero or not to be zero"
            //   was all that mattered
            // We on the other hand need to get
            //   the ticks since last update, aka modulo
            // However, C#'s % returns remainders
            //   and remainder has range [-149,+149], not modulo
            // To actually get modulo, we +150 if it's negative
            //   then we are ready to return
            if (result < 0)
                return result + interval;
            else
                return result;
        }

        public static string TicksToPeriod(this int ticksTo)
        {
            // If pawn's need level changed in a no-update tick
            // Such as by drugs, dev mode, or other mods
            // The input number may end up 0, so we fix that
            if (ticksTo <= 0)
                return "PeriodSeconds".Translate("0.00");

            if (ticksTo < 600)
                return "PeriodSeconds".Translate((ticksTo / 60f).ToString("N2"));

            if (ticksTo < 25000)
                return "PeriodHours".Translate((ticksTo / 2500f).ToString("N2"));

            return ticksTo.ToStringTicksToPeriod();
        }
    }
}
