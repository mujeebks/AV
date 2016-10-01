using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using AVD.Common.Exceptions;
using AVD.Common.Logging;
using AVD.DataAccessLayer.Enums;

namespace AVD.Core.Exceptions.Dtos
{
    public class BusinessFormatHttpResponseDto : IUxNotificationResponseDto
    {
        public int ErrorMessageId { get; set; }

        public String UserDisplayMessage { get; set; }

        public String UserDisplayTitle { get; set; }
        
        public Object Body { get; set; }
        public Guid RequestId
        {
            get { return Logger.GetIdForCurrentActivity(); }
        }

        public IEnumerable<RequestValidationException.UxValidationResult> ValidationMessages { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorMessageTypes ErrorType { get; set; }

        /// <summary>
        /// to show the error inline
        /// </summary>
        public bool IsInline { get; set; }

        /// <summary>
        /// To raise support ticket
        /// </summary>
        public bool IsSupportRequestEnabled { get; set; }

        /// <summary>
        /// Only shown to devs for debugging
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String DeveloperMessage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String StackTrace { get; set; }

        /// <summary>
        /// Only shown to devs for debugging
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ErrorMessageDto DeveloperErrorMessage { get; set; }
    }
}