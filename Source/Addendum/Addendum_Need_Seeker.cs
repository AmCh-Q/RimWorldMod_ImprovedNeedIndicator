namespace Improved_Need_Indicator
{
    public class Addendum_Need_Seeker: Addendum_Need
    {
        public Addendum_Need_Seeker(
            byte category,
            float max,
            float min,
            string translation
        ): base(category, max, min, translation)
        { }
    }
}
