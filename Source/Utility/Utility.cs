﻿using System.Collections.Generic;
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

        public static int TicksUntilNextUpdate(this Thing thing)
        {
            // In vanilla, need updates are made when
            //   thing.HashOffsetTicks() % interval == 0
            // We want to find the time since the last update
            //   so that we can correct the (rounded up)
            //   CeilToUpdate() calculation


            // We first calculate the remainder
            //   by doing the same thing as vanilla w/o comparison
            int result = (thing.HashOffsetTicks()) % NeedTunings.NeedUpdateInterval;

            // In vanilla, "to be zero or not to be zero"
            //   was all that mattered
            // We on the other hand need to get
            //   the ticks since last update, aka modulo
            // However, C#'s % returns remainders
            //   and remainder has range [-149,+149], not modulo
            // To actually get modulo, we +150 if it's negative
            //   then we are ready to return
            if (result < 0)
                return result + NeedTunings.NeedUpdateInterval;

            return result;
        }

        public static string TicksToPeriod(this int ticksTo)
        {
            string debugText = string.Empty;

#if DEBUG
            debugText += ticksTo.ToString() + ": ";
#endif
            // If pawn's need level changed in a no-update tick
            // Such as by drugs, dev mode, or other mods
            // The input number may end up 0, so we fix that
            if (ticksTo <= 0)
                return debugText + "PeriodSeconds".Translate("0.00");

            if (ticksTo < 600f)
                return debugText + "PeriodSeconds".Translate((ticksTo / 60f).ToString("N2"));

            if (ticksTo < GenDate.TicksPerDay)
                return debugText + "PeriodHours".Translate((ticksTo / 2500f).ToString("N2"));

            return debugText + ticksTo.ToStringTicksToPeriod();
        }
    }
}
