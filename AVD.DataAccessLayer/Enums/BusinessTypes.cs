using System.ComponentModel;

namespace AVD.DataAccessLayer.Enums
{
    public enum BusinessTypes
    {
        [Description("Leisure")]
        Leisure = 1,
        [Description("Corporate")]
        Corporate = 2,
        [Description("Entertainment")]
        Entertainment = 3,
        [Description("Group Travel")]
        GroupTravel = 4 
    }
}
