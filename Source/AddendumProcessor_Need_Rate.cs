using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public static class AddendumProcessor_Need_Rate
    {
        private static AddendumManager_Need_Rate needAddendum;

        public static string GetTipAddendum(Need need)
        {
            bool isDetailed = INIKeyBindingDefOf.ShowDetails.IsDown;
            int tickNow = Find.TickManager.TicksGame;

            if (isDetailed)
                return ((TaggedString)("\n\n" + ToDetailTip(need, tickNow))).Resolve();

            return (
                (TaggedString)(
                    "\n\n" + ToBasicTip(need, tickNow)
                    + "\n\n" + "INI.ShowDetails".Translate()
                )
            ).Resolve();
        }

        private static string ToBasicTip(Need need, int tickNow)
        {
            if (needAddendum == null || needAddendum.IsSameNeed(need) == false)
            {
                needAddendum = need.ToManager();
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

        private static string ToDetailTip(Need need, int tickNow)
        {
            if (needAddendum == null || needAddendum.IsSameNeed(need) == false)
            {
                needAddendum = need.ToManager();
                needAddendum.UpdateRates(tickNow);
                needAddendum.UpdateBasicTip(tickNow);
                needAddendum.UpdateDetailTip(tickNow);

                return needAddendum.detailedTip;
            }

            if (needAddendum.IsRatesStale(tickNow))
            {
                needAddendum.UpdateRates(tickNow);
                needAddendum.UpdateBasicTip(tickNow);
                needAddendum.UpdateDetailTip(tickNow);

                return needAddendum.detailedTip;
            }

            if (needAddendum.IsDetailStale(tickNow))
            {
                needAddendum.UpdateBasicTip(tickNow);
                needAddendum.UpdateDetailTip(tickNow);

                return needAddendum.detailedTip;
            }

            if (needAddendum.IsBasicStale(tickNow))
            {
                needAddendum.UpdateBasicTip(tickNow);
                needAddendum.UpdateDetailTip(tickNow);

                return needAddendum.detailedTip;
            }

            return needAddendum.detailedTip;
        }

        private static AddendumManager_Need_Rate ToManager(this Need need)
        {
            if (need is Need_Food needFood)
                return new AddendumManager_Need_Rate_Food(needFood);

            if (need is Need_Joy needJoy)
                return new AddendumManager_Need_Rate_Joy(needJoy);

            if (need is Need_Rest needRest)
                return new AddendumManager_Need_Rate_Sleep(needRest);

            return new AddendumManager_Need_Rate(need);
        }
    }
}
