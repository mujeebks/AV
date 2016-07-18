using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace AVD.Common.Auth
{
    public class UriAclEntryDto
    {
        /// <summary>
        /// The relative path starting with /
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The role names of whose members can access the above path and resources below it.
        /// </summary>
        public IEnumerable<string> RoleNames { get; set; }
    }
}
