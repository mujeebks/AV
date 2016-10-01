using System;
using AVD.Common.Auth;

namespace AVD.Core.Auth.Dtos
{
    /// <summary>
    /// An identity object represents the user on whose behalf the code is running.
    /// </summary>
    [Serializable]
    public class AVDUserIdentity : AVDIdentityBase
    {
        #region Properties

        public int? AgentId { get; private set; }

        public int? OfficeId { get; private set; }

        public int? AgencyId { get; private set; }

        public bool? IsManager { get; private set; }

        public bool? IsEmployee { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Email { get; private set; }
        #endregion

        #region Implementation of IIdentity

        #endregion

        #region Constructor

        public AVDUserIdentity(String name, bool isAuthenticated, String authenticationType, String firstName, String lastName, String email, int userId, int? agentId, int? officeId, int? agencyId, bool? isManager, bool? isEmployee)
            : base(name, isAuthenticated, authenticationType, userId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AgentId = agentId;
            OfficeId = officeId;
            AgencyId = agencyId;
            IsManager = isManager;
            IsEmployee = isEmployee;
        }

        #endregion
    }
}