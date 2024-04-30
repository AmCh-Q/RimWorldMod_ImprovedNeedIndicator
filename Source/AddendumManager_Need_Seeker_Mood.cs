using HarmonyLib;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_Seeker_Mood : AddendumManager_Need_Seeker
    {
#if v1_5
        public AddendumManager_Need_Seeker_Mood(Need_Mood need) : base(need)
        {
            // We'll be using MoodThreshold rather than a float for the thresholds.
            // To check a pawn's threshold,
            //   use MoodThresholdExtensions.CurrentMoodThresholdFor().
            FallingAddendums = new Addendum_Need[] {
                new Addendum_Need_Seeker(
                    (byte)MoodThreshold.None,
                    0f,
                    0f,
                    "INI.Comfort.Content"
                ),
                new Addendum_Need_Seeker(
                    (byte)MoodThreshold.Minor,
                    0f,
                    0f,
                    "INI.Comfort.Minor"
                ),
                new Addendum_Need_Seeker(
                    (byte)MoodThreshold.Major,
                    0f,
                    0f,
                    "INI.Comfort.Major"
                ),
                new Addendum_Need_Seeker(
                    (byte)MoodThreshold.Extreme,
                    0f,
                    0f,
                    "INI.Comfort.Extreme"
                )
            };
        }
#else
        public AddendumManager_Need_Seeker_Mood(Need_Mood need) : base(need)
        {
            // We'll be using MoodThreshold rather than a float for the thresholds.
            // To check a pawn's threshold,
            //   use MoodThresholdExtensions.CurrentMoodThresholdFor().
            FallingAddendums = new Addendum_Need[] {
                new Addendum_Need_Seeker(
                    0,
                    0f,
                    0f,
                    "INI.Comfort.Content"
                ),
                new Addendum_Need_Seeker(
                    1,
                    0f,
                    0f,
                    "INI.Comfort.Minor"
                ),
                new Addendum_Need_Seeker(
                    2,
                    0f,
                    0f,
                    "INI.Comfort.Major"
                ),
                new Addendum_Need_Seeker(
                    3,
                    0f,
                    0f,
                    "INI.Comfort.Extreme"
                )
            };
        }
#endif
    }
}
