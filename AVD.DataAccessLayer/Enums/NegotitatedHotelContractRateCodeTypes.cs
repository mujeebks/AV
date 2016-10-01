using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.DataAccessLayer.Enums
{
    public enum NegotitatedHotelContractRateCodeTypes
    {
        [Description("Virtuoso")]
        VIRTUOSO = 1,
        [Description("LXV:LUXURY PRIVILEGES")]
        LXV = 2,
        [Description("Bellini Club")]
        OBP = 3
    }
}
