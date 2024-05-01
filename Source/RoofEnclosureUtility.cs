using Verse;

namespace Improved_Need_Indicator
{
    public static class RoofEnclosureUtility
    {
        // This logic was surmised from Need_Outdoors.NeedInterval() with several differences.
        // Need_Outdoors.NeedInterval()'s logic is concerned with the rate at
        // which the mood's level is changing. While here we are concerned only
        // with the category of the Pawn's enclosure.
        public static RoofEnclosureCategory CurRoofEnclosureCategory(Pawn pawn)
        {
            RoofDef roofDef;
            bool isOutdoors;
            bool isRoofed;

            if (pawn.Spawned)
            {
                roofDef = pawn.Position.GetRoof(pawn.Map);
                isOutdoors = pawn.Position.UsesOutdoorTemperature(pawn.Map);
                isRoofed = (roofDef is null) == false;
            }
            else
            {
                roofDef = null;
                isOutdoors = true;
                isRoofed = false;
            }

            if (isOutdoors)
            {
                if (isRoofed)
                {
                    if (roofDef.isThickRoof)
                        return RoofEnclosureCategory.OutdoorsThickRoof;
                    else
                        return RoofEnclosureCategory.OutdoorsThinRoof;
                }
                else
                    return RoofEnclosureCategory.OutdoorsNoRoof;
            }
            else
            {
                if (isRoofed)
                {
                    if (roofDef.isThickRoof)
                        return RoofEnclosureCategory.IndoorsThickRoof;
                    else
                        return RoofEnclosureCategory.IndoorsThinRoof;
                }
                else
                    return RoofEnclosureCategory.IndoorsNoRoof;
            }

        }
    }
}
