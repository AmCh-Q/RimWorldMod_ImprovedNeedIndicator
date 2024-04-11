using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public static class AddendumProcessor
    {
        private static NeedAddendum needAddendum;

        public static string GetTipAddendum(Need need)
        {
            bool isDetailed = true;
            int tickNow = Find.TickManager.TicksGame;

            if (isDetailed)
                return "\n\n" + GetDetailedTipAddendum(need, tickNow);

            return "\n\n" + GetBasicTipAddendum(need, tickNow);
        }

        private static string GetBasicTipAddendum(Need need, int tickNow)
        {
            if (needAddendum == null || needAddendum.IsSameNeed(need))
            {
                needAddendum = need.ToNeedAddendum();
                needAddendum.UpdateTickRates(tickNow);
                needAddendum.UpdateThresholdAts(tickNow);
                needAddendum.UpdateBasicAddendums(tickNow);
                needAddendum.UpdateBasicTipAddendum(tickNow);

                return needAddendum.basicTipAddendum;
            }

            if (needAddendum.IsRatesStale(tickNow))
            {
                needAddendum.UpdateTickRates(tickNow);
                needAddendum.UpdateThresholdAts(tickNow);
                needAddendum.UpdateBasicAddendums(tickNow);
                needAddendum.UpdateBasicTipAddendum(tickNow);

                return needAddendum.basicTipAddendum;
            }

            if (needAddendum.IsBasicTipAddendumStale(tickNow))
            {
                needAddendum.UpdateBasicAddendums(tickNow);
                needAddendum.UpdateBasicTipAddendum(tickNow);

                return needAddendum.basicTipAddendum;
            }

            return needAddendum.basicTipAddendum;
        }

        private static string GetDetailedTipAddendum(Need need, int tickNow)
        {
            if (needAddendum == null || needAddendum.IsSameNeed(need) == false)
            {
                needAddendum = need.ToNeedAddendum();
                needAddendum.UpdateTickRates(tickNow);
                needAddendum.UpdateThresholdAts(tickNow);
                needAddendum.UpdateBasicAddendums(tickNow);
                needAddendum.UpdateDetailedAddendums(tickNow);
                needAddendum.UpdateDetailedTipAddendum(tickNow);

                return needAddendum.detailedTipAddendum;
            }

            if (needAddendum.IsRatesStale(tickNow))
            {
                needAddendum.UpdateTickRates(tickNow);
                needAddendum.UpdateThresholdAts(tickNow);
                needAddendum.UpdateBasicAddendums(tickNow);
                needAddendum.UpdateDetailedAddendums(tickNow);
                needAddendum.UpdateDetailedTipAddendum(tickNow);

                return needAddendum.detailedTipAddendum;
            }

            if (needAddendum.IsBasicTipAddendumStale(tickNow))
            {
                needAddendum.UpdateBasicAddendums(tickNow);
                needAddendum.UpdateDetailedTipAddendum(tickNow);

                return needAddendum.detailedTipAddendum;
            }

            return needAddendum.detailedTipAddendum;
        }

        private static NeedAddendum ToNeedAddendum(this Need need)
        {
            if (need is Need_Rest need_rest)
                return new NeedRestAddendum(need_rest);

            return new NeedAddendum(need);
        }
    }
}
