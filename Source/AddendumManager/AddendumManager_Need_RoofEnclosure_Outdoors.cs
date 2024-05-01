using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;


namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_RoofEnclosure_Outdoors : AddendumManager_Need_RoofEnclosure
    {
        private static readonly AccessTools.FieldRef<Need_Outdoors, float>
            fr_lastEffectiveDelta = AccessTools.FieldRefAccess<Need_Outdoors, float>("lastEffectiveDelta");


        public AddendumManager_Need_RoofEnclosure_Outdoors(Need_Outdoors need) : base(need)
        {
            // Tresholds are hard-coded but aren't in any place where
            // we can grab them. And rates depend on where the pawn is and if
            // they are asleep.
            FallingAddendums = new Addendum_Need[] {
                new Addendum_Need(
                    (byte)OutdoorsCategory.Free,
                    need.MaxLevel,
                    .8f,
                    "INI.Neutral"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.NeedFreshAir,
                    .8f,
                    .6f,
                    "INI.Outdoors.StuckIndoors"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.CabinFeverLight,
                    .6f,
                    .4f,
                    "INI.Outdoors.TrappedIndoors"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.CabinFeverSevere,
                    .4f,
                    .2f,
                    "INI.Outdoors.CabinFever"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.Trapped,
                    .2f,
                    .05f,
                    "INI.Outdoors.TrappedUnderground"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.Entombed,
                    .05f,
                    0f,
                    "INI.Outdoors.EntombedUnderground"
                )
            };
        }

        public override void UpdateRates(int tickNow)
        {
            curTickRate = System.Math.Abs(fr_lastEffectiveDelta((Need_Outdoors)need)) / NeedTunings.NeedUpdateInterval;

            base.UpdateRates(tickNow);
        }
    }
}
