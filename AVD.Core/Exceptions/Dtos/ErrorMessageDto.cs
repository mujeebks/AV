using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AVD.Common.Exceptions;
using AVD.DataAccessLayer.Enums;

namespace AVD.Core.Exceptions.Dtos
{
    public class ErrorMessageDto
    {
        /// <summary>
        /// Error Message Id
        /// </summary>
        public int ErrorMessageId { get; set; }
        /// <summary>
        /// Exception Enum Type
        /// </summary>
        public ErrorMessageTypes ErrorTypeId { get; set; }
        /// <summary>
        /// User Title
        /// </summary>
        public string UserTitle { get; set; }
        /// <summary>
        /// User friendly error message
        /// </summary>
        public string UserMessage { get; set; }
        /// <summary>
        /// Component Name
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Exception Class Name or Enum Value
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Flag to show inline error
        /// </summary>
        public bool IsInline { get; set; }
        /// <summary>
        /// Flag to show support request button
        /// </summary>
        public bool IsSupportRequestEnabled { get; set; }

        /// <summary>
        /// If this has been reviewed for copy or not. Simple flag toggled by admins.
        /// </summary>
        public bool Reviewed { get; set; }

        public DateTime? LastOccuranceDateTime { get; set; }

        public int NumberOfOccurancesInLast10Days { get; set; }

        /// <summary>
        /// Exception Full Name
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ExceptionFullName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ProviderId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ProviderCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ProviderMessage { get; set; }

        public string InternalDescription { get; set; }
        public int? ParentErrorMessageId { get; internal set; }
        public string CodeDescription { get; internal set; }
        public List<string> Subsitutions { get; internal set; }
    }
}
