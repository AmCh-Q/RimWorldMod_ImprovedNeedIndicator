namespace Improved_Need_Indicator
{
    public class Addendum_Need
    {
        public byte Category { get; protected set; }
        public float Max { get; protected set; } // exclusive (x < Max)
        public float Min { get; protected set; } // inclusive (x >= Min)
        public string Translation { get; protected set; }

        public string Basic { get; set; }
        public string Detail { get; set; }

        public Addendum_Need(
            byte category,
            float max,
            float min,
            string translation
        )
        {
            Category = category;
            Max = max;
            Min = min;
            Translation = translation;

            Basic = string.Empty;
            Detail = string.Empty;
        }
    }
}
