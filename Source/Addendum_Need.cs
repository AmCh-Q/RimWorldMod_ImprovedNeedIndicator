namespace Improved_Need_Indicator
{
    public class Addendum_Need
    {
        public byte Category { get; protected set; }
        public float Threshold { get; protected set; }
        public string Translation { get; protected set; }

        public string Basic { get; set; }
        public string Detail { get; set; }

        public Addendum_Need(
            byte category,
            float threshold,
            string translation
        )
        {
            Category = category;
            Threshold = threshold;
            Translation = translation;

            Basic = string.Empty;
            Detail = string.Empty;
        }
    }
}
