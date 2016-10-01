using System;
using System.Linq;
using System.Text;

namespace AVD.DataAccessLayer.Enums
{
    public enum RoleTypes
    {
        Administrator = 1,
        Agent = 2,
        Finance = 3,
        Manager = 4,
        SuperAgent = 5,
        SuperUser = 6,
        WVS = 8,
        Air = 9,
        WVTAgent = 10,
        AffiliateAgent = 11,
        ADXCashPayment = 12,
        SabreCryptic = 13,
        SabreCrypticNoWhitelist = 14,
        Sabre = 15,
        /// <summary>
        /// A public (generally client/traveler) user that needs to be tracked.
        /// </summary>
        Public = 16,

        /// <summary>
        /// 'Users' which are really external systems accessing aDX via an api
        /// </summary>
        System = 17,
        ADXKTAirCreditCard = 18,
        Hotel = 19,
        Insurance = 20,
        ExternalServices = 21 //Insurance would be 20 when it goes live
    }
}
