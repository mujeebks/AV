using System;
using System.Linq;
using System.Text;
using System.Web.Security;
using AVD.Core.Auth.Dtos;

namespace AVD.Core.Auth.Dtos
{
    /// <summary>
    /// A DTO that represents a user of the system (Agent)
    /// </summary>
    public class AgentUserDto : UserDto
    {
        public int AgentId { get; set; }
        public int OfficeId { get; set; }
        
        public AgentUserDto(String providerName, MembershipUser user, int userId, string firstname, string lastname, int agentId, int officeId)
            : base(providerName, user, userId, firstname, lastname)
        {
            AgentId = agentId;
            OfficeId = officeId;
        }

        public override string ToString()
        {
            return String.Format("AgentId:{0}, OfficeId: {1}", AgentId, OfficeId);
        }
    }
}
