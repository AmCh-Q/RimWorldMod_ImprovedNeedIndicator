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

        public static int TicksToIntervalAdjustedTicks(this float ticksTo, int interval)
        {
            // Round up to multiples of 150 ticks
            return Mathf.CeilToInt(ticksTo / interval) * interval;
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
