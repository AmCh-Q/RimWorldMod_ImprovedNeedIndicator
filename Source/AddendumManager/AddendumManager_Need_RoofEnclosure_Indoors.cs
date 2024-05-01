#if !v1_2
using HarmonyLib;
using RimWorld;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_RoofEnclosure_Indoors : AddendumManager_Need_RoofEnclosure
    {
        private static readonly AccessTools.FieldRef<Need_Indoors, float>
            fr_lastEffectiveDelta = AccessTools.FieldRefAccess<Need_Indoors, float>("lastEffectiveDelta");


        public AddendumManager_Need_RoofEnclosure_Indoors(Need_Indoors need) : base(need)
        {
            // Tresholds are hard-coded but aren't in any place where
            // we can grab them. And rates depend on where the pawn is and if
            // they are asleep.
            FallingAddendums = new Addendum_Need[] {
                new Addendum_Need(
                    (byte)IndoorsCategory.ComfortablyIndoors,
                    need.MaxLevel,
                    .8f,
                    "INI.Neutral"
                ),
                new Addendum_Need(
                    (byte)IndoorsCategory.JustOutdoors,
                    .8f,
                    .6f,
                    "INI.Indoors.JustOutdoors"
                ),
                new Addendum_Need(
                    (byte)IndoorsCategory.Outdoors,
                    .6f,
                    .4f,
                    "INI.Indoors.Outdoors"
                ),
                new Addendum_Need(
                    (byte)IndoorsCategory.LongOutdoors,
                    .4f,
                    .2f,
                    "INI.Indoors.LongOutdoors"
                ),
                new Addendum_Need(
                    (byte)IndoorsCategory.VeryLongOutdoors,
                    .2f,
                    .05f,
                    "INI.Indoors.VeryLongOutdoors"
                ),
                new Addendum_Need(
                    (byte)IndoorsCategory.BrutalOutdoors,
                    .05f,
                    0f,
                    "INI.Indoors.BrutalOutdoors"
                )
            };

            curTickRate = System.Math.Abs(fr_lastEffectiveDelta(need)) / NeedTunings.NeedUpdateInterval;
        }

        public override void UpdateRates(int tickNow)
        {
            curTickRate = System.Math.Abs(fr_lastEffectiveDelta((Need_Indoors)need)) / NeedTunings.NeedUpdateInterval;

            base.UpdateRates(tickNow);
        }
    }
}
#endif