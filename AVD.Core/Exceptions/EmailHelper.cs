using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.Common.Configuration;
using AVD.Common.Logging;
using AVD.Core.Communication;
using AVD.Core.Communication.Dtos;
using AVD.Core.Users;

namespace AVD.Core.Exceptions
{
    public class EmailHelper
    {
        internal static bool SendSupportEmailMessage(string subject, string message)
        {
            Logger.Instance.LogFunctionEntry(typeof(EmailHelper).Name, "SendSupportEmailMessage");

            var emailMsg = new EmailRequestDto
            {
                To = new List<EmailAddressDto>
                {
                    new EmailAddressDto
                    {
                        Address = EmailConfigSettings.Instance().DefaultSupportEmailAddress
                    }
                },
                Subject = subject,
                Body = message,
                IsBodyHtml = false
            };

            var resp = new EmailWorker().SendSystemEmail(emailMsg);

            if (!resp.IsSuccessful)
            {
                if (resp.ValidationMessages.Any())
                {
                    Logger.Instance.Error(typeof(EmailHelper).Name, "SendSupportEmailMessage", resp.ValidationMessages);
                }

                if (resp.FailureMessages.Any())
                {
                    // We can't gracefully handle any of these failure messages so just
                    // log them and throw a business exception to try again later.
                    Logger.Instance.Error(typeof(EmailHelper).Name, "SendSupportEmailMessage", resp.FailureMessages);
                }

                Logger.Instance.Error(typeof(EmailHelper).Name, "SendSupportEmailMessage", "Email could not be sent, no validation or failure messages returned.");
                
            }

            Logger.Instance.LogFunctionExit(typeof(EmailHelper).Name, "SendSupportEmailMessage");

            return resp.IsSuccessful;
        }


    }
}
