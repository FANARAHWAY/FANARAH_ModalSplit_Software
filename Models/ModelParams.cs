using Avalonia.Media;
using CsvHelper.Configuration.Attributes;

namespace AvaloniaModel.Models
{
    public class ModelParams
    {
        public ModelParams()
        {

        }
        public ModelParams(ModelParams other)
        {
            Mode = other.Mode;
            Cost = other.Cost;
            TravelTime = other.TravelTime;
            Frict = other.Frict;
            WalkPlus = other.WalkPlus;
            Car = other.Car;
            Rush = other.Rush;
            Age = other.Age;
            BusConstant = other.BusConstant;
        }

        public TravelMode Mode { get; set; }
        [Default(0)]
        public float Cost { get; set; }
        [Default(0)]
        public float TravelTime { get; set; }
        [Default(0)]
        public float Frict { get; set; }
        [Default(0)]
        public float WalkPlus { get; set; }
        [Default(0)]
        public float Car { get; set; }
        [Default(0)]
        public float Rush { get; set; }
        [Default(0)]
        public float Age { get; set; }

        [Default(0)]
        public float BusConstant { get; set; }
    }
}
