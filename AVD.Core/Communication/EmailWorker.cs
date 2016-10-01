using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using AVD.Common.Caching;
using AVD.Common.Exceptions;
using AVD.Common.Logging;
using AVD.Core;
using AVD.Core.Auth;
using AVD.Core.Communication;
using AVD.Core.Communication.Dtos;
using AVD.Core.Communication.Exceptions;
using AVD.DataAccessLayer.Enums;
using AVD.DataAccessLayer.Repositories;
using AVD.Common.Configuration;
using AVD.Common.Exceptions;
using AVD.Common.Logging;
using AVD.Core.Communication.Dtos;
using AVD.Core.Exceptions;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Enums;
using AVD.DataAccessLayer.Models;
using AVD.DataAccessLayer.Repositories;
using AVD.Core.Auth;
using AVD.Core.Communication.Exceptions;
using AVD.Common.Caching;
using AVD.Core.Properties;

namespace AVD.Core.Communication
{
    public class EmailWorker
    {
        public string DoNotReplyEmail { set; get; }
        public int? AgentId { set; get; }

        /// <summary>
        /// This should only be used for the agents
        /// </summary>
        public EmailWorker()
        {
            DoNotReplyEmail = EmailConfigSettings.Instance().EmailSenderAddressForAgent;
        }

        /// <summary>
        /// This should only be used for the clients
        /// </summary>
        /// <param name="agentId"></param>
        public EmailWorker(int agentId)
        {
            AgentId = agentId;


            DoNotReplyEmail = "donotreply@emrm.com";

            if (String.IsNullOrWhiteSpace(DoNotReplyEmail))
            {
                DoNotReplyEmail = EmailConfigSettings.Instance().DefaultEmailSenderAddressForClient;
                Logger.Instance.Error(this.GetType().Name, "LogEmail", String.Format("Agent {0} does not have a DoNotReply email address.  Using default 'DoNotReply' email from config.", agentId));
            }
        }

        private string GetUploadedFilesFolder()
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "GetUploadedFilesFolder");

            var uploadedFilesFoder = ConfigurationManager.AppSettings["UPLOADED_FILES_FOLDER"];
            if (String.IsNullOrEmpty(uploadedFilesFoder))
                throw new ApplicationException("Missing configuration value for the uploaded files folder.");

            Logger.Instance.LogFunctionExit(this.GetType().Name, "GetUploadedFilesFolder");

            return uploadedFilesFoder;
        }

        private string GetAssetsVirtualDirectory()
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "GetAssetsVirtualDirectory");

            var uploadedFilesFoder = ConfigurationManager.AppSettings["COMMON_UPLOAD_VIRTUAL_DIR_PATH"];
            if (String.IsNullOrEmpty(uploadedFilesFoder))
                throw new ApplicationException("Missing configuration value for the common virtual path folder.");

            Logger.Instance.LogFunctionExit(this.GetType().Name, "GetAssetsVirtualDirectory");

            return uploadedFilesFoder;
        }

        public void LogEmail(EmailRequestDto email)
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(Environment.NewLine);
            if (email != null)
            {
                if (email.To != null)
                {
                    if (email.To.Count() != 0)
                    {
                        for (int i = 0; i < email.To.Count; i++)
                        {
                            sb.AppendLine(string.Format("EmailRequestDto.To[{0}].Address = {1}", i, email.To.ElementAt(i).Address));
                            sb.AppendLine(string.Format("EmailRequestDto.To[{0}].DisplayName = {1}", i, email.To.ElementAt(i).DisplayName));
                        }
                    }
                    else
                    {
                        sb.AppendLine("EmailRequestDto.To is empty");
                    }

                }
                else
                {
                    sb.AppendLine("EmailRequestDto.To is null");
                }

                if (email.CC != null)
                {
                    for (int i = 0; i < email.CC.Count; i++)
                    {
                        sb.AppendLine(string.Format("EmailRequestDto.CC[{0}].Address = {1}", i, email.CC.ElementAt(i).Address));
                        sb.AppendLine(string.Format("EmailRequestDto.CC[{0}].DisplayName = {1}", i, email.CC.ElementAt(i).DisplayName));
                    }
                }

                if (email.Bcc != null)
                {
                    for (int i = 0; i < email.Bcc.Count; i++)
                    {
                        sb.AppendLine(string.Format("EmailRequestDto.Bcc[{0}].Address = {1}", i, email.Bcc.ElementAt(i).Address));
                        sb.AppendLine(string.Format("EmailRequestDto.Bcc[{0}].DisplayName = {1}", i, email.Bcc.ElementAt(i).DisplayName));
                    }
                }

                if (email.ReplyToList != null)
                {
                    for (int i = 0; i < email.ReplyToList.Count; i++)
                    {
                        sb.AppendLine(string.Format("EmailRequestDto.ReplyToList[{0}].Address = {1}", i, email.ReplyToList.ElementAt(i).Address));
                        sb.AppendLine(string.Format("EmailRequestDto.ReplyToList[{0}].DisplayName = {1}", i, email.ReplyToList.ElementAt(i).DisplayName));
                    }
                }

                var attachments = email.Attachments.Where(a => a != null);

                foreach (var a in attachments)
                {
                    sb.AppendLine(string.Format("EmailRequestDto.Attachment.FileName = {0}", a.FileName));
                    sb.AppendLine(string.Format("EmailRequestDto.Attachment.EmailContentType = {0}",
                        a.EmailContentType.ToString()));
                }

                sb.AppendLine(string.Format("EmailRequestDto.BodyEncoding = {0}", email.BodyEncoding));
                sb.AppendLine(string.Format("EmailRequestDto.BodyTransferEncoding = {0}", email.BodyTransferEncoding));
                sb.AppendLine(string.Format("EmailRequestDto.BodyType = {0}", email.BodyType));
                sb.AppendLine(string.Format("EmailRequestDto.SubjectEncoding = {0}", email.SubjectEncoding));
                sb.AppendLine(string.Format("EmailRequestDto.Subject = {0}", email.Subject));
            }
            else
            {
                sb.AppendLine("EmailRequestDto object is null");
            }
            Logger.Instance.Info(this.GetType().Name, "LogEmail", sb.ToString());
        }


        public List<ValidationMessage> ValidateEmailReq(EmailRequestDto email)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "ValidateEmailReq");

            var util = new Utilities();
            var result = this.ValidateSystemEmailReq(email);

            if (email != null)
            {
                if (email.EmailType == EmailTypes.Unknown)
                {
                    Logger.Instance.Error(this.GetType().Name, "ValidateEmailReq", string.Format("Validation Error: Invalid Email Type = {0}", EmailDtoField.EmailType.ToString()));
                    result.Add(
                        new ValidationMessage(ValidationMessageType.InvalidEmailField, EmailDtoField.EmailType));
                }

                if (email.QuoteId <= 0)
                {
                    Logger.Instance.Error(this.GetType().Name, "ValidateEmailReq", string.Format("Validation Error: Invalid QuoteId = ({0})", EmailDtoField.QuoteId.ToString()));
                    result.Add(
                        new ValidationMessage(ValidationMessageType.InvalidEmailField, EmailDtoField.QuoteId));
                }
            }

            Logger.Instance.LogFunctionExit(this.GetType().Name, "ValidateEmailReq");
            return result;
        }

        public List<ValidationMessage> ValidateSystemEmailReq(EmailRequestDto email)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "ValidateSystemEmailReq");

            var util = new Utilities();
            var result = new List<ValidationMessage>();
            if (AgentId == null && string.IsNullOrEmpty(DoNotReplyEmail))
            {
                result.Add(
                    new ValidationMessage(ValidationMessageType.MissingConfiguration, EmailDtoField.EmailSenderAddressForAgent));
                Logger.Instance.Error(this.GetType().Name, "ValidateEmailReq", string.Format("Missing EmailSenderAddressForAgent value from configuration"));
            }
            else if (AgentId == null && !util.IsValidEmail(DoNotReplyEmail))
            {
                result.Add(
                    new ValidationMessage(ValidationMessageType.InvalidConfiguration, EmailDtoField.EmailSenderAddressForAgent));
                Logger.Instance.Error(this.GetType().Name, "ValidateEmailReq", string.Format("Invalid EmailSenderAddressForAgent value from configuration = {0}", DoNotReplyEmail));
            }

            if (email != null)
            {
                if (email.To == null || email.To.Count() == 0)
                {
                    Logger.Instance.Error(this.GetType().Name, "ValidateSystemEmailReq", string.Format("Validation Error: Missing {0}", EmailDtoField.To.ToString()));
                    result.Add(
                        new ValidationMessage(ValidationMessageType.MissingEmailField, EmailDtoField.To));
                }
                else
                {
                    email.To.ForEach(e =>
                    {
                        if (string.IsNullOrEmpty(e.Address))
                        {
                            Logger.Instance.Error(this.GetType().Name, "ValidateSystemEmailReq", string.Format("Validation Error: Missing {0}", EmailDtoField.ToAddress.ToString()));
                            result.Add(new ValidationMessage(ValidationMessageType.MissingEmailField, EmailDtoField.ToAddress));
                        }
                        else if (!util.IsValidEmail(e.Address))
                        {
                            Logger.Instance.Error(this.GetType().Name, "ValidateSystemEmailReq", string.Format("Validation Error: Invalid {0} = {1}", EmailDtoField.ToAddress.ToString(), e.Address));
                            result.Add(
                                new ValidationMessage(ValidationMessageType.InvalidEmailField, EmailDtoField.ToAddress));
                        }
                    });
                }
                if (email.CC != null)
                {
                    email.CC.ForEach(e =>
                    {
                        if (string.IsNullOrEmpty(e.Address))
                        {
                            Logger.Instance.Error(this.GetType().Name, "ValidateSystemEmailReq", string.Format("Validation Error: Missing {0}", EmailDtoField.CCAddress.ToString()));
                            result.Add(new ValidationMessage(ValidationMessageType.MissingEmailField, EmailDtoField.CCAddress));
                        }
                        else if (!util.IsValidEmail(e.Address))
                        {
                            Logger.Instance.Error(this.GetType().Name, "ValidateSystemEmailReq", string.Format("Validation Error: Invalid {0} = {1}", EmailDtoField.CCAddress.ToString(), e.Address));
                            result.Add(
                                new ValidationMessage(ValidationMessageType.InvalidEmailField, EmailDtoField.CCAddress));
                        }
                    });
                }

                if (email.Bcc != null)
                {
                    email.Bcc.ForEach(e =>
                    {
                        if (string.IsNullOrEmpty(e.Address))
                        {
                            Logger.Instance.Error(this.GetType().Name, "ValidateSystemEmailReq", string.Format("Validation Error: Missing {0}", EmailDtoField.BccAddress.ToString()));
                            result.Add(new ValidationMessage(ValidationMessageType.MissingEmailField, EmailDtoField.BccAddress));
                        }
                        else if (!util.IsValidEmail(e.Address))
                        {
                            Logger.Instance.Error(this.GetType().Name, "ValidateSystemEmailReq", string.Format("Validation Error: Invalid {0} = {1}", EmailDtoField.BccAddress.ToString(), e.Address));
                            result.Add(
                                new ValidationMessage(ValidationMessageType.InvalidEmailField, EmailDtoField.BccAddress));
                        }
                    });
                }
                var attachments = email.Attachments.Where(a => a != null);
                foreach (var a in attachments)
                {
                    if (!util.IsValidFileName(a.FileName))
                    {
                        Logger.Instance.Error(this.GetType().Name, "ValidateSystemEmailReq", string.Format("Validation Error: Invalid {0} = {1}", EmailDtoField.AttachmentFileName.ToString(), a.FileName));
                        result.Add(
                            new ValidationMessage(ValidationMessageType.InvalidEmailField, EmailDtoField.AttachmentFileName));
                    }
                }
            }
            else
            {
                Logger.Instance.Error(this.GetType().Name, "ValidateSystemEmailReq", string.Format("Validation Error: Missing {0}", "Email Object"));
                result.Add(
                    new ValidationMessage(ValidationMessageType.MissingEmailField, EmailDtoField.Email));
            }

            Logger.Instance.LogFunctionExit(this.GetType().Name, "ValidateSystemEmailReq");
            return result;
        }

        public SystemEmailResponse SendSystemEmail(EmailRequestDto email)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "SendSystemEmail");
            var response = new SystemEmailResponse();
            response.ValidationMessages.AddRange(ValidateSystemEmailReq(email));
            if (response.ValidationMessages.Any())
            {
                Logger.Instance.LogFunctionExit(this.GetType().Name, "SendSystemEmail");
                return response;
            }

            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = email.Subject;
            mailMessage.Body = email.Body;

            mailMessage.BodyEncoding = email.BodyEncoding;
            mailMessage.BodyTransferEncoding = email.BodyTransferEncoding;
            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess | DeliveryNotificationOptions.OnFailure | DeliveryNotificationOptions.Delay;
            mailMessage.IsBodyHtml = email.IsBodyHtml;
            mailMessage.SubjectEncoding = email.SubjectEncoding;

            try
            {
                mailMessage.From = new MailAddress(DoNotReplyEmail);
                mailMessage.Sender = new MailAddress(DoNotReplyEmail);

                foreach (var to in email.To)
                {
                    mailMessage.To.Add(new MailAddress(to.Address, to.DisplayName));
                }
                if (email.CC != null)
                {
                    foreach (var cc in email.CC)
                    {
                        mailMessage.CC.Add(new MailAddress(cc.Address, cc.DisplayName));
                    }
                }

                if (email.Bcc != null)
                {
                    foreach (var bcc in email.Bcc)
                    {
                        mailMessage.Bcc.Add(new MailAddress(bcc.Address, bcc.DisplayName));
                    }
                }

                if (email.ReplyToList != null)
                {
                    foreach (var replyTo in email.ReplyToList)
                    {
                        mailMessage.ReplyToList.Add(new MailAddress(replyTo.Address, replyTo.DisplayName));
                    }
                }

                SmtpClient smtpClient = new SmtpClient(EmailConfigSettings.Instance().SmtpHostName,
                    EmailConfigSettings.Instance().SmtpPortNumber);
                smtpClient.EnableSsl = EmailConfigSettings.Instance().SmtpEnableSsl;
                smtpClient.UseDefaultCredentials = EmailConfigSettings.Instance().SmtpUseDefaultCredentials;
                smtpClient.Credentials = new NetworkCredential(EmailConfigSettings.Instance().SmtpUserName,
                    EmailConfigSettings.Instance().SmtpPassword);

                LogSystemEmailToDB(email);
                smtpClient.Send(mailMessage);
                response.IsSuccessful = true;
            }
            catch (SmtpFailedRecipientException ex)
            {
                response.IsSuccessful = false;
                response.FailureMessages.Add(new FailureMessage(FailureType.SmtpFailedRecipientException, ex));
                Logger.Instance.Debug(this.GetType().Name, "InsertFile", ex, "Exception Sending Email");
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.FailureMessages.Add(new FailureMessage(FailureType.Exception, ex));
                Logger.Instance.Debug(this.GetType().Name, "InsertFile", ex, "Exception Sending Email");
            }
            finally
            {
                var attachments = email.Attachments.Where(a => a != null);
                foreach (var a in attachments.Where(a => a.MemoryStream != null))
                {
                    try
                    {
                        a.MemoryStream.Close();
                    }
                    catch (Exception ex)
                    {
                        //this Exception isn't critival, memorystream is managed well and should be collected in short order
                        //but since it impliments Close(), we should try to close it.
                        Logger.Instance.Warn(this.GetType().Name, "SendSystemEmail", ex, "MemoryStream failed to close");
                    }
                }

            }
            Logger.Instance.LogFunctionExit(this.GetType().Name, "SendEmail");
            return response;
        }
        public EmailResponse SendEmail(EmailRequestDto email, bool logEmail = true)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "SendEmail");
            var response = new EmailResponse();

            LogEmail(email);
            if (logEmail)
            {
                response.ValidationMessages.AddRange(ValidateEmailReq(email));
                if (response.ValidationMessages.Any())
                {
                    Logger.Instance.LogFunctionExit(this.GetType().Name, "SendEmail");
                    return response;
                }
            }
          
           
            if (response.ValidationMessages.Any())
            {
                Logger.Instance.LogFunctionExit(this.GetType().Name, "SendEmail");
                return response;
            }

            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = email.Subject;

            mailMessage.Body = UserMessages.Val_ShoreEx_Email;

            mailMessage.Body += string.Join(" ", "EMRM Admin");


            mailMessage.BodyEncoding = email.BodyEncoding;
            mailMessage.BodyTransferEncoding = email.BodyTransferEncoding;
            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess | DeliveryNotificationOptions.OnFailure | DeliveryNotificationOptions.Delay;
            mailMessage.IsBodyHtml = true;
            mailMessage.SubjectEncoding = email.SubjectEncoding;

            try
            {
                mailMessage.From = new MailAddress(DoNotReplyEmail);
                mailMessage.Sender = new MailAddress(DoNotReplyEmail);

                foreach (var to in email.To)
                {
                    mailMessage.To.Add(new MailAddress(to.Address, to.DisplayName));
                }
                if (email.CC != null)
                {
                    foreach (var cc in email.CC)
                    {
                        mailMessage.CC.Add(new MailAddress(cc.Address, cc.DisplayName));
                    }
                }

                if (email.Bcc != null)
                {
                    foreach (var bcc in email.Bcc)
                    {
                        mailMessage.Bcc.Add(new MailAddress(bcc.Address, bcc.DisplayName));
                    }
                }

                if (email.ReplyToList != null)
                {
                    foreach (var replyTo in email.ReplyToList)
                    {
                        mailMessage.ReplyToList.Add(new MailAddress(replyTo.Address, replyTo.DisplayName));
                    }
                }

                var agentEmail = new MailAddress("userEmailid", "EMRM No Reply");
                mailMessage.ReplyToList.Add(agentEmail);
                mailMessage.CC.Add(agentEmail);

                var sendDate = DateTime.Now;
                var attachments = email.Attachments.Where(a => a != null);
                foreach (var a in attachments.Where(a => a.MemoryStream != null))
                {
                    if (!email.IsResending)
                        {
                            a.FileName = string.Format(
                                "{0}-{1}{2}",
                                Path.GetFileNameWithoutExtension(a.FileName),
                                sendDate.ToString("yyyyMMddHHmmss"),
                                Path.GetExtension(a.FileName));
                        }

                        ContentType contentType = new ContentType(a.EmailContentType);
                        Attachment attach = new Attachment(a.MemoryStream, contentType);
                        attach.ContentDisposition.FileName = a.FileName;

                        mailMessage.Attachments.Add(attach);
                }

                SmtpClient smtpClient = new SmtpClient(EmailConfigSettings.Instance().SmtpHostName, EmailConfigSettings.Instance().SmtpPortNumber);
                smtpClient.EnableSsl = EmailConfigSettings.Instance().SmtpEnableSsl;
                smtpClient.UseDefaultCredentials = EmailConfigSettings.Instance().SmtpUseDefaultCredentials;
                smtpClient.Credentials = new NetworkCredential(EmailConfigSettings.Instance().SmtpUserName, EmailConfigSettings.Instance().SmtpPassword);
                if (logEmail)
                {
                    email.Body = mailMessage.Body;
                    response.EmailId = SaveEmail(email, DateTime.Now);
                    if (response.EmailId != 0)
                    {
                        smtpClient.Send(mailMessage);
                        response.IsSuccessful = true;
                    }
                }
                else
                {
                    smtpClient.Send(mailMessage);
                    response.IsSuccessful = true;
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                response.IsSuccessful = false;
                response.FailureMessages.Add(new FailureMessage(FailureType.SmtpFailedRecipientException, ex));
                Logger.Instance.Debug(this.GetType().Name, "SendEmail", ex, "Exception Sending Email");
                //CleanDBLogEntry(response.EmailId);
                UpdateDBLogEntry(response.EmailId);
                response.EmailId = 0;
                throw new BusinessException(CommunicationBusinessExceptionTypes.Email_Invalid_Recipient,
                    FailureType.SmtpFailedRecipientException.ToString(), email, ex);
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.FailureMessages.Add(new FailureMessage(FailureType.Exception, ex));
                Logger.Instance.Debug(this.GetType().Name, "SendEmail", ex, "Exception Sending Email");
                //CleanDBLogEntry(response.EmailId);
                UpdateDBLogEntry(response.EmailId);
                response.EmailId = 0;
                throw ex;
            }
            finally
            {
                var attachments = email.Attachments.Where(a => a != null);
                foreach (var a in attachments.Where(a => a.MemoryStream != null))
                {
                    try
                    {
                        a.MemoryStream.Close();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Warn(this.GetType().Name, "SendEmail", ex, "MemoryStream failed to close");
                    }
                }

            }
            Logger.Instance.LogFunctionExit(this.GetType().Name, "SendEmail");
            return response;
        }

        private int SaveEmail(EmailRequestDto email, DateTime sendDate)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "SaveEmail");
            if (email == null) return 0;
            var emailId = 0;
            var needsToLogToDB = true;
            var fileNames = default(string);

            if (email.Attachments != null)
            {
                // this will not save if attachment is null or the isStatic property is true;
                fileNames = SaveEmailAttachments(EmailConfigSettings.Instance().AttachmentsFolder, email);
                if (string.IsNullOrEmpty(fileNames))
                {
                    needsToLogToDB = false;
                }
            }

            if (needsToLogToDB)
            {
                
                emailId = LogEmailToDB(email, sendDate, fileNames);
                if (emailId == 0)
                {
                    CleanDiskEmailAttachment(fileNames);
                }            
            }

            Logger.Instance.LogFunctionExit(this.GetType().Name, "SaveEmail");
            return emailId;
        }

        private void CleanDBLogEntry(int emailId)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "CleanDBLogEntry");
            if (emailId == 0) return;
            var repo = RepositoryFactory.Get<Email>();
            var emailLog = repo.FirstOrDefault(e => e.EmailId == emailId);
            if (emailLog != null)
            {
                try
                {
                    CleanDiskEmailAttachment(emailLog.DocumentLink);
                    repo.Delete(emailLog);
                    repo.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(this.GetType().Name, "CleanDBLogEntry", ex, "Exception logging email to DB");
                }
            }
            Logger.Instance.LogFunctionExit(this.GetType().Name, "CleanDBLogEntry");
        }

        public void UpdateDBLogEntry(int emailId)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "UpdateDBLogEntry");
            if (emailId == 0) return;
            var repo = RepositoryFactory.Get<Email>();
            var emailLog = repo.FirstOrDefault(e => e.EmailId == emailId);
            //Change status to false  
            if (emailLog != null)
            {
                try
                {
                    emailLog.IsSuccess = false;
                    repo.Update(emailLog);
                    repo.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(this.GetType().Name, "UpdateDBLogEntry", ex, "Exception logging email to DB");
                }
            }
            Logger.Instance.LogFunctionExit(this.GetType().Name, "UpdateDBLogEntry");
        }


        private void CleanDiskEmailAttachment(string fileName)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "CleanDiskEmailAttachment");
            if (string.IsNullOrEmpty(fileName)) return;

            FileInfo fi = new FileInfo(fileName);
            try
            {
                fi.Delete();
            }
            catch (IOException e)
            {
                Logger.Instance.Error(this.GetType().Name, "CleanDiskEmailAttachment", e, "Exception deleting email attachment");
                return;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(this.GetType().Name, "CleanDiskEmailAttachment", ex, "Exception deleting email attachment");
                return;
            }
            Logger.Instance.LogFunctionExit(this.GetType().Name, "CleanDiskEmailAttachment");
        }

        public string SaveEmailAttachments(string path, EmailRequestDto email)
        {
           
            string filenames = string.Empty;
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "SaveEmailAttachment");
            if (string.IsNullOrEmpty(path)) return null;
            if (email == null) return null;
            
            var serverAttachmentsDirPath = Path.Combine(GetUploadedFilesFolder(), GetAssetsVirtualDirectory());
            serverAttachmentsDirPath = Path.Combine(serverAttachmentsDirPath, path);

            bool serverAttachmentsDirectoryExists = Directory.Exists(serverAttachmentsDirPath);
            DirectoryInfo di = null;
            if (serverAttachmentsDirectoryExists == false)
            {
                di = Directory.CreateDirectory(serverAttachmentsDirPath);
            }
            if (di == null)
                di = new DirectoryInfo(serverAttachmentsDirPath);

            try
            {
                foreach (var a in email.Attachments.Where(a=> a != null && a.isStatic == false ))
                {
                    var ms = a.MemoryStream;
                    var attachmentPath = Path.Combine(di.FullName, a.FileName);

                    filenames += a.FileName + ",";
                    //cc will overwrite it, if it exists as per the ticket created DT-4006, created by Maria
                    using (var fs = new FileStream(attachmentPath, FileMode.Create))
                    {
                        ms.WriteTo(fs);
                        fs.Close();
                    }
                }
                filenames = filenames.Substring(0, filenames.Length - 1);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(
                    this.GetType().Name,
                    "SaveEmailAttachments",
                    ex,
                    "Exception saving email attachment");
                return null;
            }

            Logger.Instance.LogFunctionExit(this.GetType().Name, "SaveEmailAttachment");
            return filenames;
        }

        public int LogEmailToDB(EmailRequestDto email, DateTime sendDate, string fileName)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "LogEmailToDB");
            if (email == null) return 0;
            var repo = RepositoryFactory.Get<Email>();
            var emailLog = new Email
            {
                EmailTypeId = (int)email.EmailType,
                DocumentLink = fileName,
                ToAddress = string.Join(",", email.To.Select(e => e.Address)),
                SendDate = sendDate,
                IsSuccess=true,
                UserId = AuthManager.GetCurrentUserId(),
                Subject = email.Subject,
                Body = email.Body,
                InvoiceId = email.InvoiceId,
                Url = email.Url
            };
            try
            {
                //Note: Some times the client itinerary page was posted twice, so the emails were sent back to back
                //before sending the email,it will check the cache.If the DTO is not in the cache then the email will be sent
                if (!string.IsNullOrWhiteSpace(email.EmailIdentifier))
                {
                    ICachingManager cachingManager = ServiceLocator.CachingManager;
                    //Check it in the Cache
                    var emails = cachingManager.Get<Email>(email.EmailIdentifier);

                    if (emails != null)
                    {
                        return 0;
                    }
                    else
                    {
                        //Add it to the cache
                        cachingManager.Add<Email>(email.EmailIdentifier, emailLog);
                    }
                }
                repo.Insert(emailLog);
                repo.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(this.GetType().Name, "LogEmailToDB", ex, "Exception logging email to DB");
            }
            Logger.Instance.LogFunctionExit(this.GetType().Name, "LogEmailToDB");
            return emailLog.EmailId;
        }

        public int LogSystemEmailToDB(EmailRequestDto email)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "LogSystemEmailToDB");
            if (email == null) return 0;
            var repo = RepositoryFactory.Get<Email>();
            var emailLog = new Email
            {
                EmailTypeId = (int)email.EmailType,
                DocumentLink = "",
                ToAddress = string.Join(",", email.To.Select(e => e.Address)),
                SendDate = System.DateTime.Now,
                IsSuccess = true,
                UserId = 1,
                Subject = email.Subject,
                Body = email.Body
            };
            try
            {
                repo.Insert(emailLog);
                repo.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(this.GetType().Name, "LogSystemEmailToDB", ex, "Exception logging email to DB");
            }
            Logger.Instance.LogFunctionExit(this.GetType().Name, "LogSystemEmailToDB");
            return emailLog.EmailId;
        }


        /// <summary>
        /// Emails notification to the user
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool EmailNotification(EmailNotificationDto dto)
        {
            var response = SendSystemEmail(dto);

            return response.IsSuccessful;
        }

    }
}
