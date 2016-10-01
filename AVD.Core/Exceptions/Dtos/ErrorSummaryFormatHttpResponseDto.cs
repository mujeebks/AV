using System;
using Newtonsoft.Json;
using AVD.Common.Logging;

namespace AVD.Core.Exceptions.Dtos
{
    public class ErrorSummaryFormatHttpResponseDto : IUxNotificationResponseDto
    {
        public String UserDisplayMessage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String DeveloperMessage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String StackTrace { get; set; }
        public Guid RequestId
        {
            get { return Logger.GetIdForCurrentActivity(); }
        }
    }
}