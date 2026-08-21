using Avalonia.Controls;
using System.ComponentModel;

namespace AvaloniaModel.Models
{
    public enum TravelMode
    {
        [Description("Walk")]
        Walk = 1,

        [Description("Drive & park")] 
        Drive_park =2,

        [Description("Taxi")]
        Taxi = 3,

        [Description("Bus")] 
        Bus = 4
    }

    

    public enum AgeRange
    {
        [Description("10-15")]
        age_10_15 = 13,

        [Description("16-20")]
        age_16_20 = 18,

        [Description("21-25")]
        age_21_25 = 23,

        [Description("26-30")] 
        age_26_30 = 28,

        [Description("31-35")] 
        age_31_35 = 33,

        [Description("36-40")] 
        age_36_40 = 38,

        [Description("41-45")] 
        age_41_45 = 43,

        [Description("46-50")] 
        age_46_50 = 48,

        [Description("51-55")] 
        age_51_55 = 53,

        [Description("56-60")] 
        age_56_60 = 58
    }
}
