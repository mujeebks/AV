using System;
using System.Linq;
using System.Text;
using AVD.Core.Auth.Dtos;

namespace AVD.Core.Users
{
    public class AuthenticatedUserDto
    {
        /// <summary>
        /// The temp GUID assigned to this users session
        /// </summary>
        public Guid AuthGuid { get; set; }

        /// <summary>
        /// The context of the session (e.g. AgentUser of it's an Agent that is logged in)
        /// </summary>
        public UserDto UserDto { get; set; }

        public override string ToString()
        {
            return String.Format("{0}: UserDto:{1}", AuthGuid, UserDto);
        }

        public string UserSessionId { get; set; }
    }
}
