using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AVD.Common.Logging;
using AVD.DataAccessLayer.Models;
using AVD.DataAccessLayer.Repositories;

namespace AVD.DataAccessLayer.Enums
{
    /// <summary>
    /// Represents a subset of the User table whose Ids are referenced in code.
    /// Generally system accounts.
    /// </summary>
    /// <remarks>Description maps to UserName field. Ids can be different between environments</remarks>
    public class UserTypes
    {
        [Description("SYSTEM.ACCOUNT")]
        public static int SystemAccount { get; set; }

        [Description("PUBLIC.TRIPVIEW")]
        public static int PublicTripView { get; set; }

        [Description("PUBLIC.AIRSEARCHVIEW")]
        public static int PublicAirSearchView { get; set; }

        /// <summary>
        /// This is the account that TMT/Kensington Tours will connect with.
        /// </summary>
        [Description("SYSTEM.KT")]
        public static int SystemKensingtonTours { get; set; }

        /// <summary>
        /// This will initalize all the ids in the properties in this class
        /// and log errors for those unresolved (that don't exist in the DB)
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            bool allSucceeded = true;

            var props = typeof (UserTypes).GetProperties();//BindingFlags.Static);

            foreach (var prop in props.Where(t => t.PropertyType == typeof (int)))
            {
                int? userId = null;

                var desc = prop.GetCustomAttributes<DescriptionAttribute>().Single().Description;

                if (desc != null)
                {
                    var userRepo = RepositoryFactory.GetUnsecured<User>();
                    var s = userRepo.GetAsQueryable().SingleOrDefault(t => t.Username == desc);
                    if (s != null)
                        userId = s.UserId;
                }

                if (userId != null)
                    prop.SetValue(null, userId);
                else
                {
                    allSucceeded = false;
                    Logger.Instance.Error(typeof (UserTypes).Name, "Init()",
                        prop.Name + "(DB val: " + desc + ") account does not exist in the users table");
                }
            }
            return allSucceeded;
        }
    }
}
