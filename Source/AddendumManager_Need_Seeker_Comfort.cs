using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_Seeker_Comfort : AddendumManager_Need_Seeker
    {
        public AddendumManager_Need_Seeker_Comfort(Need_Comfort need) : base(need)
        {
            FallingAddendums = new Addendum_Need[] {
                new Addendum_Need_Seeker(
                    (byte)ComfortCategory.LuxuriantlyComfortable,
                    fr_MinLuxuriantlyComfortable(need),
                    "INI.Comfort.LuxuriantlyComfortable"
                ),
                new Addendum_Need_Seeker(
                    (byte)ComfortCategory.ExtremelyComfortable,
                    fr_MinExtremelyComfortable(need),
                    "INI.Comfort.ExtremelyComfortable"
                ),
                new Addendum_Need_Seeker(
                    (byte)ComfortCategory.VeryComfortable,
                    fr_MinVeryComfortable(need),
                    "INI.Comfort.VeryComfortable"
                ),
                new Addendum_Need_Seeker(
                    (byte)ComfortCategory.Comfortable,
                    fr_MinComfortable(need),
                    "INI.Comfort.Comfortable"
                ),
                new Addendum_Need_Seeker(
                    (byte)ComfortCategory.Normal,
                    fr_MinNormal(need),
                    "INI.Neutral"
                ),
                new Addendum_Need_Seeker(
                    (byte)ComfortCategory.Uncomfortable,
                    0f,
                    "INI.Comfort.Uncomfortable"
                )
            };
        }

        private static readonly AccessTools.FieldRef<Need_Comfort, float>
            fr_MinLuxuriantlyComfortable = AccessTools.FieldRefAccess<Need_Comfort, float>("MinLuxuriantlyComfortable");

        private static readonly AccessTools.FieldRef<Need_Comfort, float>
            fr_MinExtremelyComfortable = AccessTools.FieldRefAccess<Need_Comfort, float>("MinExtremelyComfortablee");

        private static readonly AccessTools.FieldRef<Need_Comfort, float>
            fr_MinVeryComfortable = AccessTools.FieldRefAccess<Need_Comfort, float>("MinVeryComfortable");

        private static readonly AccessTools.FieldRef<Need_Comfort, float>
            fr_MinComfortable = AccessTools.FieldRefAccess<Need_Comfort, float>("MinComfortable");

        private static readonly AccessTools.FieldRef<Need_Comfort, float>
            fr_MinNormal = AccessTools.FieldRefAccess<Need_Comfort, float>("MinNormal");
    }
}
