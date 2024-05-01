using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_Seeker_Beauty : AddendumManager_Need_Seeker
    {
        public AddendumManager_Need_Seeker_Beauty(Need_Beauty need) : base(need)
        {
            FallingAddendums = new Addendum_Need[] {
                new Addendum_Need(
                    (byte)BeautyCategory.Beautiful,
                    fr_ThreshBeautiful(need),
                    fr_ThreshVeryPretty(need),
                    "INI.Beauty.Beautiful"
                ),
                new Addendum_Need(
                    (byte)BeautyCategory.VeryPretty,
                    fr_ThreshVeryPretty(need),
                    fr_ThreshPretty(need),
                    "INI.Beauty.VeryPretty"
                ),
                new Addendum_Need(
                    (byte)BeautyCategory.Pretty,
                    fr_ThreshPretty(need),
                    fr_ThreshNeutral(need),
                    "INI.Beauty.Pretty"
                ),
                new Addendum_Need(
                    (byte)BeautyCategory.Neutral,
                    fr_ThreshNeutral(need),
                    fr_ThreshUgly(need),
                    "INI.Neutral"
                ),
                new Addendum_Need(
                    (byte)BeautyCategory.Ugly,
                    fr_ThreshUgly(need),
                    fr_ThreshVeryUgly(need),
                    "INI.Beauty.Unsightly"
                ),
                new Addendum_Need(
                    (byte)BeautyCategory.VeryUgly,
                    fr_ThreshVeryUgly(need),
                    0.0001f,
                    "INI.Beauty.Ugly"
                ),
                new Addendum_Need(
                    (byte)BeautyCategory.Hideous,
                    0f,
                    0f,
                    "INI.Beauty.Hideous"
                )
            };
        }

        private static readonly AccessTools.FieldRef<Need_Beauty, float>
            fr_ThreshBeautiful = AccessTools.FieldRefAccess<Need_Beauty, float>("ThreshBeautiful");

        private static readonly AccessTools.FieldRef<Need_Beauty, float>
            fr_ThreshVeryPretty = AccessTools.FieldRefAccess<Need_Beauty, float>("ThreshVeryPretty");

        private static readonly AccessTools.FieldRef<Need_Beauty, float>
            fr_ThreshPretty = AccessTools.FieldRefAccess<Need_Beauty, float>("ThreshPretty");

        private static readonly AccessTools.FieldRef<Need_Beauty, float>
            fr_ThreshNeutral = AccessTools.FieldRefAccess<Need_Beauty, float>("ThreshNeutral");

        private static readonly AccessTools.FieldRef<Need_Beauty, float>
            fr_ThreshUgly = AccessTools.FieldRefAccess<Need_Beauty, float>("ThreshUgly");

        private static readonly AccessTools.FieldRef<Need_Beauty, float>
            fr_ThreshVeryUgly = AccessTools.FieldRefAccess<Need_Beauty, float>("ThreshVeryUgly");
    }
}
