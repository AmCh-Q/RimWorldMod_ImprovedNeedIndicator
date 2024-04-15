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
                return "\n\n" + ((TaggedString)GetDetailedTipAddendum(need, tickNow)).Resolve();

            return "\n\n" + ((TaggedString)GetBasicTipAddendum(need, tickNow)).Resolve();
        }

        private static string GetBasicTipAddendum(Need need, int tickNow)
        {
            if (needAddendum == null || needAddendum.IsSameNeed(need) == false)
            {
                needAddendum = need.ToNeedAddendum();
                needAddendum.UpdateRates(tickNow);
                needAddendum.UpdateBasicTip(tickNow);

                return needAddendum.basicTip;
            }

            if (needAddendum.IsRatesStale(tickNow))
            {
                needAddendum.UpdateRates(tickNow);
                needAddendum.UpdateBasicTip(tickNow);

                return needAddendum.basicTip;
            }

            if (needAddendum.IsBasicStale(tickNow))
            {
                needAddendum.UpdateBasicTip(tickNow);

                return needAddendum.basicTip;
            }

            return needAddendum.basicTip;
        }

        private static string GetDetailedTipAddendum(Need need, int tickNow)
        {
            if (needAddendum == null || needAddendum.IsSameNeed(need) == false)
            {
                needAddendum = need.ToNeedAddendum();
                needAddendum.UpdateRates(tickNow);
                needAddendum.UpdateBasicTip(tickNow);
                needAddendum.UpdateDetailedTip(tickNow);

                return needAddendum.detailedTip;
            }

            if (needAddendum.IsRatesStale(tickNow))
            {
                needAddendum.UpdateRates(tickNow);
                needAddendum.UpdateBasicTip(tickNow);
                needAddendum.UpdateDetailedTip(tickNow);

                return needAddendum.detailedTip;
            }

            if (needAddendum.IsDetailedStale(tickNow))
            {
                needAddendum.UpdateBasicTip(tickNow);
                needAddendum.UpdateDetailedTip(tickNow);

                return needAddendum.detailedTip;
            }

            if (needAddendum.IsBasicStale(tickNow))
            {
                needAddendum.UpdateBasicTip(tickNow);

                return needAddendum.detailedTip;
            }

            return needAddendum.detailedTip;
        }

        private static NeedAddendum ToNeedAddendum(this Need need)
        {
            if (need is Need_Rest need_rest)
                return new NeedRestAddendum(need_rest);
            else if (need is Need_Food need_Food)
                return new NeedFoodAddendum(need_Food);

            return new NeedAddendum(need);
        }
    }
}
