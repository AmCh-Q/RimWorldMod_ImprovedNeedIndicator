using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;


namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_Rate_Outdoors : AddendumManager_Need_Rate
    {
        // These static values are pulled from Need_Outdoors. They are held
        // inside private constants that can't be accessed through reflection.

        public static readonly float TickRate_IndoorsNoRoof = (5f * 0.0025f) / NeedTunings.NeedUpdateInterval;
        public static readonly float TickRate_IndoorsThickRoof = (.45f * 0.0025f) / NeedTunings.NeedUpdateInterval;
        public static readonly float TickRate_IndoorsThinRoof = (0.32f * 0.0025f) / NeedTunings.NeedUpdateInterval;

        public static readonly float TickRate_OutdoorsNoRoof = (8f * 0.0025f) / NeedTunings.NeedUpdateInterval;
        public static readonly float TickRate_OutdoorsThickRoof = (0.4f * 0.0025f) / NeedTunings.NeedUpdateInterval;
        public static readonly float TickRate_OutdoorsThinRoof = (1f * 0.0025f) / NeedTunings.NeedUpdateInterval;

        public static readonly float TickRateFactor_InBed = .2f;

        public static readonly float Minimum_IndoorsThinRoof = 0.2f;


        private RoofEnclosureCategory curCategory;
        private float curTickRate;

        public AddendumManager_Need_Rate_Outdoors(Need_Outdoors need) : base(need)
        {
            // Tresholds are hard-coded but aren't in any place where
            // we can grab them. And rates depend on where the pawn is and if
            // they are asleep.
            FallingAddendums = new Addendum_Need[] {
                new Addendum_Need(
                    (byte)OutdoorsCategory.Free,
                    need.MaxLevel,
                    .8f,
                    "INI.Neutral"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.NeedFreshAir,
                    .8f,
                    .6f,
                    "INI.Outdoors.StuckIndoors"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.CabinFeverLight,
                    .6f,
                    .4f,
                    "INI.Outdoors.TrappedIndoors"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.CabinFeverSevere,
                    .4f,
                    .2f,
                    "INI.Outdoors.CabinFever"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.Trapped,
                    .2f,
                    .05f,
                    "INI.Outdoors.TrappedUnderground"
                ),
                new Addendum_Need(
                    (byte)OutdoorsCategory.Entombed,
                    .05f,
                    0f,
                    "INI.Outdoors.EntombedUnderground"
                )
            };

            curTickRate = 0f;
            curCategory = RoofEnclosureCategory.OutdoorsNoRoof;

            showDetails = string.Empty;
        }


        public override void UpdateBasicTip(int tickNow)
        {
            base.UpdateBasicTip(tickNow);
        }

        protected override void UpdateBasicTipFalling(int tickNow, float curLevel)
        {
            float levelAccumulator;
            int tickAccumulator;
            int tickOffset;
            int ticksUntilThreshold;

            levelAccumulator = curLevel;
            tickAccumulator = 0;
            tickOffset = pawn.TicksUntilNextUpdate();

            basicTip = string.Empty;
            foreach (Addendum_Need thresholdAddendum in FallingAddendums)
            {
                if (levelAccumulator >= thresholdAddendum.Max)
                {
                    ticksUntilThreshold = TicksUntilThresholdUpdate(levelAccumulator, thresholdAddendum.Max, curTickRate);
                    tickAccumulator += ticksUntilThreshold;
                    levelAccumulator -= ticksUntilThreshold * curTickRate;

                    thresholdAddendum.Basic = thresholdAddendum.Translation.Translate((tickAccumulator - tickOffset).TicksToPeriod());
                }

                if (curLevel >= thresholdAddendum.Max)
                    basicTip += "\n" + thresholdAddendum.Basic;
            }

            basicTip = basicTip.Trim();
        }

        protected override void UpdateBasicTipRising(int tickNow, float curLevel)
        {
            float levelAccumulator;
            int tickAccumulator;
            int ticksUntilThreshold;

            levelAccumulator = curLevel;
            tickAccumulator = 0;

            basicTip = string.Empty;
            foreach (Addendum_Need thresholdAddendum in FallingAddendums)
            {
                if (levelAccumulator <= thresholdAddendum.Min)
                {
                    ticksUntilThreshold = TicksUntilThreshold(thresholdAddendum.Min, levelAccumulator, curTickRate);
                    tickAccumulator += ticksUntilThreshold;
                    levelAccumulator += ticksUntilThreshold * curTickRate;

                    thresholdAddendum.Basic = thresholdAddendum.Translation.Translate(tickAccumulator.TicksToPeriod());
                }

                if (curLevel <= thresholdAddendum.Min)
                    basicTip += "\n" + thresholdAddendum.Basic;
            }

            basicTip = basicTip.Trim();
        }

        public override void UpdateDetailTip(int tickNow)
        {
            detailedTip = basicTip;
            detailUpdatedAt = tickNow;
        }

        public override void UpdateRates(int tickNow)
        {
            bool isPawnInBed;

            curCategory = RoofEnclosureUtility.CurRoofEnclosureCategory(pawn);
            isPawnInBed = pawn.InBed();

            switch (curCategory)
            {
                case RoofEnclosureCategory.IndoorsNoRoof:
                    curTickRate = TickRate_IndoorsNoRoof;
                    break;

                case RoofEnclosureCategory.IndoorsThickRoof:
                    curTickRate = TickRate_IndoorsThickRoof;
                    if (isPawnInBed)
                        curTickRate *= TickRateFactor_InBed;
                    break;

                case RoofEnclosureCategory.IndoorsThinRoof:
                    curTickRate = TickRate_IndoorsThinRoof;
                    if (isPawnInBed)
                        curTickRate *= TickRateFactor_InBed;
                    break;

                case RoofEnclosureCategory.OutdoorsNoRoof:
                    curTickRate = TickRate_OutdoorsNoRoof;
                    break;

                case RoofEnclosureCategory.OutdoorsThickRoof:
                    curTickRate = TickRate_OutdoorsThickRoof;
                    if (isPawnInBed)
                        curTickRate *= TickRateFactor_InBed;
                    break;

                case RoofEnclosureCategory.OutdoorsThinRoof:
                    curTickRate = TickRate_OutdoorsThinRoof;
                    break;
            }

            base.UpdateRates(tickNow);
        }
    }
}
