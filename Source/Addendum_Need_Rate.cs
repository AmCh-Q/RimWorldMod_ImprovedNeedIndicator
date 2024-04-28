namespace Improved_Need_Indicator
{
    public class Addendum_Need_Rate : Addendum_Need
    {
        public float Rate { get; set; }
        public byte RateCategory { get; protected set; }

        public Addendum_Need_Rate(
            byte category,
            float threshold,
            string translation,
            byte rateCategory
        ) : base(category, threshold, translation)
        {
            Rate = 0f;
            RateCategory = rateCategory;
        }
    }
}
