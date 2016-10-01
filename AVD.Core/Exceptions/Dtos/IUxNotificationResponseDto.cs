using System;

namespace AVD.Core.Exceptions.Dtos
{
    public interface IUxNotificationResponseDto
    {
        String DeveloperMessage { get; set; }
        String StackTrace { get; set; }
        Guid RequestId { get; }
    }
}