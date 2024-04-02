using HarmonyLib;
using System.Reflection;
using UnityEngine;
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
        public static float TicksTo(float updatesTo, Thing thing)
        {
            // Given a float of the number of needed updates
            //   and a Thing's id (for update hash)
            // return the exact integer number of ticks till target

            // We count the number of updates needed by Mathf.Ceil
            //   but we also over-count time by at least one tick
            //   because at least 1 tick, at most all 150 ticks of
            //   our current update interval has passed already
            // We correct for this over-counting by finding
            //   the true number of ticks that has passed
            //   we do so by using HashOffsetTicks() with a modulo
            //     Vanilla use it to determine which tick to update a Thing
            // Since the HashOffset is negative, the range of modulo is -149~0
            //   We +1, mod, negate, then -149
            //     to make the range of the modulo -1~-150
            //     this will count down and correct for the over-counting
            return Mathf.Ceil(updatesTo) * 150f 
                - 149f - ((uint)(thing.HashOffsetTicks() + 1) % 150);
        }
        public static string TimeString(float ticksTo)
        {
            // Given the number of ticks till target
            // return a string describing the time remaining

            // Use real seconds if less than 10 seconds left, round to 1 decimal place
            // Otherwise use game hours, round to 2 decimal places
            if (ticksTo < 600f)
                return string.Concat((ticksTo / 60f).ToString("N1"), " seconds.");
            else
                return string.Concat((ticksTo / 2500f).ToString("N2"), " hours.");
        }
        public static string TimeString(float updatesTo, Thing thing)
        {
            // Given a float of the number of needed updates
            //   and a Thing's id (for update hash)
            // return a string describing the time remaining

            return TimeString(TicksTo(updatesTo, thing));
        }
    }
}
