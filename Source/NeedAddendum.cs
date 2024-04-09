using System.Reflection;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    abstract class NeedAddendum
    {

        private static readonly AccessTools.FieldRef<Need, Pawn>
            fr_pawn = AccessTools.FieldRefAccess<Need, Pawn>("pawn");

        protected Need need;
        protected Pawn pawn;

        public string detailedTipAddendum;
        protected int detailedTipAddendumTick; // when detailed tip addendum was updated.
        protected float needLevel;
        protected int ratesTick; // when need rates were updated.
        public string tipAddendum;
        protected int tipAddendumTick; // when tip addendum was updated.


        protected NeedAddendum(Need need)
        {
            this.need = need;
            pawn = fr_pawn(need);

            detailedTipAddendum = string.Empty;
            detailedTipAddendumTick = -1;
            needLevel = -1f;
            ratesTick = -1;
            tipAddendum = string.Empty;
            tipAddendumTick = -1;
        }

        public bool IsDetailedTipAddendumStale(int tickNow)
        {
            return tickNow != detailedTipAddendumTick;
        }

        public bool IsRatesStale(int tickNow)
        {
            return (
                tickNow != ratesTick
                && pawn.IsHashIntervalTick(NeedTunings.NeedUpdateInterval)
            );
        }

        public bool IsSameNeed(Need need)
        {
            return need == this.need;
        }

        public bool IsTipAddendumStale(int tickNow)
        {
            return tickNow != tipAddendumTick;
        }

        public abstract void UpdateDetailedTipAddendum(int tickNow);
        public abstract void UpdateTipAddendum(int tickNow);
        public abstract bool UpdateTickRates(int tickNow);
    }
}