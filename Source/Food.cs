using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class Food
    {
        private static float cachedLevelOfNeed = -1f;
        private static int cachedPawnId = -1;
        private static int cachedTickNow = -1;
        private static string cachedTipStringAddendum = string.Empty;

        public static string ProcessNeed(Pawn pawn, Need_Food need, int tickNow)
        {
            List<string> tipAddendums = new List<string>();

            float levelOfNeed;
            int tickOffset;
            int tickAccumulator;


            levelOfNeed = need.CurLevel;

            if (cachedPawnId == pawn.thingIDNumber &&
                tickNow == cachedTickNow &&
                levelOfNeed.IsCloseTo(cachedLevelOfNeed, 0.0001f))
            {
                return cachedTipStringAddendum;
            }

            cachedTickNow = tickNow;
            cachedLevelOfNeed = levelOfNeed;
            cachedPawnId = pawn.thingIDNumber;

            return "\n" + string.Join("\n", tipAddendums);
        }
    }
}
