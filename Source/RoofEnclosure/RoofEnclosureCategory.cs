namespace Improved_Need_Indicator
{
    // RoofEnclosureCategory has no direct parallel within Rimworld's codebase.
    // It is an attempt to label the states a pawn may be in when determining
    // Need_Indoors or Need_Outdoors tick rates.
    public enum RoofEnclosureCategory: byte
    {
        IndoorsNoRoof,
        IndoorsThickRoof,
        IndoorsThinRoof,
        OutdoorsNoRoof,
        OutdoorsThickRoof,
        OutdoorsThinRoof,
    }
}
