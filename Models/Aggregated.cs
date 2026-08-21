namespace AvaloniaModel.Models
{
    public class Aggregated : ModelParams
    {
        public Aggregated()
        {

        }
        public Aggregated(ModelParams other) : base(other)
        {
        }

        public int CaseNumber { get; set; }
        public double? Exp { get; set; }
        public double? Prop { get; set; }
        public double? gx
        {
            get
            {
                return Cost + TravelTime + Frict + WalkPlus + Car + Rush + Age + BusConstant;
            }
        }
    }
}
