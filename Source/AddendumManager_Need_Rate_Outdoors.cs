using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_Rate_Outdoors : AddendumManager_Need_Rate
    {
        public AddendumManager_Need_Rate_Outdoors(Need_Outdoors need) : base(need)
        {
            // Tresholds are hard-coded but aren't in any place where
            // we can grab them. And rates depend on where the pawn is and if
            // they are asleep.
            FallingAddendums = new Addendum_Need[] {
                new Addendum_Need(
                    (byte)OutdoorsCategory.Free,
                    1f,
                    "INI.Neutral"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.NeedFreshAir,
                    .8f,
                    "INI.Outdoors.StuckIndoors"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.CabinFeverLight,
                    .6f,
                    "INI.Outdoors.TrappedIndoors"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.CabinFeverSevere,
                    .4f,
                    "INI.Outdoors.CabinFever"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.Trapped,
                    .2f,
                    "INI.Outdoors.TrappedUnderground"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.Entombed,
                    .05f,
                    "INI.Outdoors.EntombedUnderground"
                )
            };
        }
    }
}
