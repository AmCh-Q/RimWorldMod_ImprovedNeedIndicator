using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Improved_Need_Indicator
{

    public class NeedBeautyAddendum : NeedAddendum
    {
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

        private readonly Need_Beauty needBeauty;

        public NeedBeautyAddendum(Need_Beauty need) : base(need)
        {
            needBeauty = need;

            fallingAddendums = new ThresholdAddendum[] {
                new ThresholdAddendum(
                    (byte)BeautyCategory.VeryPretty,
                    (byte)BeautyCategory.Beautiful,
                    fr_ThreshVeryPretty(needBeauty),
                    "INI.Beauty.Beautiful"
                ),
                new ThresholdAddendum(
                    (byte)BeautyCategory.Pretty,
                    (byte)BeautyCategory.VeryPretty,
                    fr_ThreshPretty(needBeauty),
                    "INI.Beauty.Pretty"
                ),
                new ThresholdAddendum(
                    (byte)BeautyCategory.Neutral,
                    (byte)BeautyCategory.Pretty,
                    fr_ThreshNeutral(needBeauty),
                    "INI.Beauty.Neutral"
                ),
                new ThresholdAddendum(
                    (byte)BeautyCategory.Ugly,
                    (byte)BeautyCategory.Neutral,
                    fr_ThreshUgly(needBeauty),
                    "INI.Beauty.Unsightly"
                ),
                new ThresholdAddendum(
                    (byte)BeautyCategory.VeryUgly,
                    (byte)BeautyCategory.Ugly,
                    fr_ThreshVeryUgly(needBeauty),
                    "INI.Beauty.Ugly"
                ),
                new ThresholdAddendum(
                    (byte)BeautyCategory.Hideous,
                    (byte)BeautyCategory.VeryUgly,
                    0f,
                    "INI.Beauty.Hideous"
                ),
            };
        }
    }
}
