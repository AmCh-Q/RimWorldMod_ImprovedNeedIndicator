using RimWorld;

namespace Improved_Need_Indicator
{
    public class AddendumManager_Need_Seeker: AddendumManager_Need
    {
        protected Addendum_Need_Seeker[] FallingRateAddendums
        {
            get { return (Addendum_Need_Rate[])FallingAddendums; }
            set { FallingAddendums = value; }
        }

        public AddendumManager_Need_Seeker(Need_Seeker need): base(need)
        {
        }
    }
}
