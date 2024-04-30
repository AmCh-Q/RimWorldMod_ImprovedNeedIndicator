#if !v1_2
using RimWorld;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_Rate_Indoors : AddendumManager_Need_Rate
    {
        public AddendumManager_Need_Rate_Indoors(Need_Indoors need) : base(need)
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
        }
    }
}
#endif