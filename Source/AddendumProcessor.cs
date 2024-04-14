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
            bool isDetailed = false;
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
                needAddendum.UpdateBasic(tickNow);

                return needAddendum.basicTip;
            }

            if (needAddendum.IsRatesStale(tickNow))
            {
                needAddendum.UpdateTickRates(tickNow);
                needAddendum.UpdateBasic(tickNow);

                return needAddendum.basicTip;
            }

            if (needAddendum.IsBasicStale(tickNow))
            {
                needAddendum.UpdateBasic(tickNow);

                return needAddendum.basicTip;
            }

            return needAddendum.basicTip;
        }

        private static string GetDetailedTipAddendum(Need need, int tickNow)
        {
            if (needAddendum == null || needAddendum.IsSameNeed(need) == false)
            {
                needAddendum = need.ToNeedAddendum();
                needAddendum.UpdateTickRates(tickNow);
                needAddendum.UpdateBasic(tickNow);
                needAddendum.UpdateDetailed(tickNow);

                return needAddendum.detailedTip;
            }

            if (needAddendum.IsRatesStale(tickNow))
            {
                needAddendum.UpdateTickRates(tickNow);
                needAddendum.UpdateBasic(tickNow);
                needAddendum.UpdateDetailed(tickNow);

                return needAddendum.detailedTip;
            }

            if (needAddendum.IsDetailedStale(tickNow))
            {
                needAddendum.UpdateBasic(tickNow);
                needAddendum.UpdateDetailed(tickNow);

                return needAddendum.detailedTip;
            }

            if (needAddendum.IsBasicStale(tickNow))
            {
                needAddendum.UpdateBasic(tickNow);

                return needAddendum.detailedTip;
            }

            return needAddendum.detailedTip;
        }

        private static NeedAddendum ToNeedAddendum(this Need need)
        {
            if (need is Need_Rest need_rest)
                return new NeedRestAddendum(need_rest);

            return new NeedAddendum(need);
        }
    }
}
