using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_Seeker_Beauty : AddendumManager_Need_Seeker
    {
        private readonly Need_Beauty needBeauty;

        public AddendumManager_Need_Seeker_Beauty(Need_Beauty need) : base(need)
        {
            needBeauty = need;

            FallingAddendums = new Addendum_Need[] {
                new Addendum_Need(
                    (byte)BeautyCategory.VeryPretty,
                    fr_ThreshVeryPretty(needBeauty),
                    "INI.Beauty.Beautiful"
                ),
                new Addendum_Need(
                    (byte)BeautyCategory.Pretty,
                    fr_ThreshPretty(needBeauty),
                    "INI.Beauty.Pretty"
                ),
                new Addendum_Need(
                    (byte)BeautyCategory.Neutral,
                    fr_ThreshNeutral(needBeauty),
                    "INI.Beauty.Neutral"
                ),
                new Addendum_Need(
                    (byte)BeautyCategory.Ugly,
                    fr_ThreshUgly(needBeauty),
                    "INI.Beauty.Unsightly"
                ),
                new Addendum_Need(
                    (byte)BeautyCategory.VeryUgly,
                    fr_ThreshVeryUgly(needBeauty),
                    "INI.Beauty.Ugly"
                ),
                new Addendum_Need(
                    (byte)BeautyCategory.Hideous,
                    0f,
                    "INI.Beauty.Hideous"
                )
            };
        }

        private static readonly AccessTools.FieldRef<Need_Beauty, float>
            fr_ThreshVeryUgly = AccessTools.FieldRefAccess<Need_Beauty, float>("ThreshVeryUgly");

        private static readonly AccessTools.FieldRef<Need_Beauty, float>
            fr_ThreshUgly = AccessTools.FieldRefAccess<Need_Beauty, float>("ThreshUgly");

        private static readonly AccessTools.FieldRef<Need_Beauty, float>
            fr_ThreshNeutral = AccessTools.FieldRefAccess<Need_Beauty, float>("ThreshNeutral");

        private static readonly AccessTools.FieldRef<Need_Beauty, float>
            fr_ThreshPretty = AccessTools.FieldRefAccess<Need_Beauty, float>("ThreshPretty");

        private static readonly AccessTools.FieldRef<Need_Beauty, float>
            fr_ThreshVeryPretty = AccessTools.FieldRefAccess<Need_Beauty, float>("ThreshVeryPretty");
    }
}
