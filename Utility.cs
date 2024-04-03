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
        public static int TicksTo(this float updatesTo, Thing thing = null)
        {
            // Given a float of the number of needed updates
            //   and a Thing's id (for update hash)
            // return the exact integer number of ticks till target

            // We count the number of updates needed by Mathf.Ceil
            //   but we also over-count time by at least 1 tick
            //   because at least 1 tick, at most all 150 ticks of
            //   our current update interval has passed already
            // If the number is >= 10 seconds, we don't care about that
            const int interval = NeedTunings.NeedUpdateInterval;
            int ticksTo = Mathf.CeilToInt(updatesTo) * interval;
            if (thing == null || ticksTo >= 600)
                return ticksTo;

            // We correct for this over-counting by finding
            //   the true number of ticks that has passed
            //   we do so by using HashOffsetTicks() with a modulo
            //     Vanilla use it to determine which tick to update a Thing
            // C# does not have true Euclidean modulo, % is actually remainder
            // Since the HashOffset is negative,
            //   the range of HashOffsetTicks() % 150 is -149~149
            // We subtract remainder, then conditionally -150 or -1
            //   to make the range of the modulo -1~-150
            //   this will correct for the over-counting
            // We also +1 before the remainder to avoid racing with the updatesTo
            int remainder = (thing.HashOffsetTicks() + 1) % interval;
            return ticksTo - remainder - (remainder <= 0 ? interval : 1);
        }
        public static string PeriodTo(this float updatesTo, Thing thing = null)
        {
            // Given a float of the number of needed updates
            //   and a Thing's id (for update hash)
            // return a string describing the time remaining
            return TicksTo(updatesTo, thing).ToStringTicksToPeriod();
        }
    }
}
