using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AVD.Common.Exceptions;
using AVD.Common.Logging;
using AVD.Common.Version;
using AVD.Core.Auth;
using AVD.Core.Exceptions.Dtos;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Enums;
using AVD.DataAccessLayer.Models;
using AVD.DataAccessLayer.Repositories;
using Newtonsoft.Json;
using AVD.Core.Exceptions;
using RestSharp.Extensions;

namespace AVD.Core.Exceptions
{
    public class ErrorMessagesManager
    {
        /// <summary>
        /// Get the ErrorMessage for a particular exception type
        /// </summary>
        public BusinessFormatHttpResponseDto ConvertToUserErrorMessage(Exception exception, bool log = true)
        {
            using (var errorMessageRepository = RepositoryFactory.Get<ErrorMessage>())
            {
                TravelEdgeException teException;

                if (exception is TravelEdgeException)
                {
                    teException = exception as TravelEdgeException;
                }
                else
                {
                    teException = new TravelEdgeException(null); // default
                }

                //Get the Unique Message key
                var exceptionFullName = ErrorMessage.GetExceptionFullName(teException);

                ErrorMessage errorMessage;

                //Get the error message
                if (teException is ProviderException)
                {
                    var providerException = teException as ProviderException;

                    errorMessage =
                        errorMessageRepository.Get(t => t.ExceptionFullName.Equals(exceptionFullName)
                                                        // todo: compare against ID
                                                        && t.ProviderCode == providerException.ProviderCode
                                                        && t.ProviderMessage == providerException.ProviderMessage
                            // TODO: Should this be part of the Key?
                            ).SingleOrDefault();

                }
                else
                {
                    errorMessage =
                        errorMessageRepository.Get(t => t.ExceptionFullName.Equals(exceptionFullName))
                            .SingleOrDefault();
                }

                //if there is no error message found,then create it by using BusinessException
                if (errorMessage == null)
                {
                    //Create the required Error Messages
                    errorMessage = this.CreateErrorMessage(teException);
                }

                string userTitle = errorMessage.UserTitle;
                string userMessage = errorMessage.UserMessage;

                if (teException is BusinessException && userMessage != null)
                {
                    var be = teException as BusinessException;
                    string messageWithSubsitutions;

                    if (TryStringFormatWithSubsitutions(be, userMessage, out messageWithSubsitutions))
                        userMessage = messageWithSubsitutions;
                }


                if (errorMessage.UserTitle == null || errorMessage.UserMessage == null)
                {
                    ReflectedErrorDetails d = ServiceLocator.CachingManager.GetOrAddToCache(
                       errorMessage.ExceptionFullName, () =>
                       GetReflectedExceptionDetails(errorMessage.ExceptionFullName), 5000
                   );

                    // If not found
                    if (d.ErrorType == null)
                        d = new ReflectedErrorDetails {ErrorType = typeof (TravelEdgeException)};

                    GetDefaultTitleAndUserMessage(errorMessageRepository, d.ErrorType, ref userTitle,
                        ref userMessage);
                }

                //Do the transformation
                // Create the dto to return as the body
                var businessSummary = new BusinessFormatHttpResponseDto()
                {
                    ErrorType = errorMessage.ErrorTypeId,
                    UserDisplayMessage = userMessage,
                    StackTrace = exception.StackTrace,
                    IsInline = errorMessage.IsInline,
                    IsSupportRequestEnabled = errorMessage.IsSupportRequestEnabled,
                    UserDisplayTitle = userTitle,
                    ErrorMessageId = errorMessage.ErrorMessageId

                };

                // TODO: Move into config class. (this was copied from the api proj)
                if (ConfigurationManager.AppSettings["TE.UI.Api.ExceptionHandlingAttribute.ShowDeveloperMessage"] ==
                    "true")
                {
                    var errorMessageDto = GetErrorMessageWithReflectedDetails(errorMessage);

                    var devMessage = exception.Message;
                    if (String.IsNullOrWhiteSpace(devMessage))
                    {
                        devMessage = null;
                    }
                    businessSummary.DeveloperMessage = devMessage;
                    businessSummary.DeveloperErrorMessage = errorMessageDto;
                }

                if (teException is BusinessException)
                {
                    businessSummary.Body = ((BusinessException)teException).Dto;
                }

                if (errorMessage.ErrorTypeId == ErrorMessageTypes.Error)
                    Logger.InstanceRequestErrors.Error(errorMessage.ErrorTypeId.ToString(), errorMessage.ExceptionFullName, exception, JsonConvert.SerializeObject(businessSummary));
                else
                    Logger.InstanceRequestErrors.Warn(errorMessage.ErrorTypeId.ToString(), errorMessage.ExceptionFullName, exception, JsonConvert.SerializeObject(businessSummary));

                // Log occurance
                LogErrorOccurance(businessSummary);

                return businessSummary;
            }
        }


        /// <summary>
        /// Returns the message with subsitutions
        /// </summary>
        /// <param name="message">The user message from the DB with {X} replaced with property value of X</param>
        /// <returns></returns>
        public bool TryStringFormatWithSubsitutions(BusinessException bex, string message, out string formattedMessage)
        {
            // Return if no subsitutions available
            if (!message.Contains("{"))
            {
                formattedMessage = message;
                return true;
            }
            else
            {
                formattedMessage = message;
            }

            // Get the properties we may subsitute
            foreach (var property in bex.GetType().GetProperties().Where(t => t.DeclaringType == bex.GetType()))
            {
                var val = property.GetValue(bex);

                if(val != null && !String.IsNullOrWhiteSpace(val.ToString()))
                    formattedMessage = formattedMessage.Replace("{" + property.Name + "}", val.ToString());
            }

            // Were any missed?
            if (formattedMessage.Contains("{"))
            {
                formattedMessage = null;
                Logger.Instance.Warn(GetType().Name, "TryStringFormatWithSubsitutions", "Variable mismatch or extra { in message - " + formattedMessage);
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Logs the error message for tracking and so we know which ones to focus on for copy.
        /// </summary>
        /// <param name="businessSummary"></param>
        private void LogErrorOccurance(BusinessFormatHttpResponseDto businessSummary)
        {
            try
            {
                using (var errorMessageRepository = RepositoryFactory.Get<ErrorOccurance>())
                {

                    int? userId = null;
                    
                    if(AuthManager.IsCurrentUserAuthenticated())
                        userId = AuthManager.GetCurrentUserId();

                    errorMessageRepository.Insert(
                        new ErrorOccurance
                        {
                            ErrorMessageId = businessSummary.ErrorMessageId,
                            UserId = userId,
                            RequestId = businessSummary.RequestId,
                            AppVersion = VersionInformation.Get().ToString()
                        });
                    errorMessageRepository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Silence
                Logger.Instance.Error(GetType().Name, "LogErrorOccurance", ex);
            }
        }
        internal void AssociateErrorOccuranceWithSupportTicket(string requestId, int supportTicketId)
        {
            try
            {
                using (var repository = RepositoryFactory.Get<ErrorOccurance>())
                {
                    var g = Guid.Parse(requestId);

                    var items = repository.Get(t => t.RequestId == g);
                    if (items.Any())
                    {
                        foreach (var item in items)
                        {
                            item.SupportTicketId = supportTicketId;
                        }
                    }

                    repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Silence
                Logger.Instance.Error(GetType().Name, "AssociateErrorOccuranceWithSupportTicket", ex);
            }
        }

        private void GetDefaultTitleAndUserMessage(IRepository<ErrorMessage> errorMessageRepository, Type exceptionType, ref string title, ref string userMessage)
        {
            if (exceptionType == null)
                throw new ArgumentNullException(nameof(exceptionType));

            // Ensure that the type is of traveledge exception
            if (!typeof(TravelEdgeException).IsAssignableFrom(exceptionType))
            {
                // base case - should not go past this.
                Logger.Instance.Error(GetType().Name, "GetDefaultTitleAndUserMessage",
                    exceptionType.FullName + " does not inherit from " + typeof(TravelEdgeException).FullName + " - ensure that at least the TravelEdgeException has an entry in the ErrorMessages table");
                title = "Error";
                userMessage = "Contact Support";
            }

            // get the current error message
            ErrorMessage errorMessage;
            if (typeof(ProviderException).IsAssignableFrom(exceptionType))
            {
                // Try to find a top level generic provider level message (i.e. no specific provider code or messages).
                errorMessage =
                    errorMessageRepository.Get(
                        t =>
                        t.ExceptionFullName.Equals(exceptionType.FullName) && t.ProviderId == null
                        && t.ProviderCode == null && t.ProviderMessage == null).SingleOrDefault();
            }
            else
            {
                errorMessage =
                    errorMessageRepository.Get(t => t.ExceptionFullName.Equals(exceptionType.FullName))
                        .SingleOrDefault();
            }

            // if there is an entry, assign the title/usermessage.
            if (errorMessage != null)
            {
                if (title == null)
                    title = errorMessage.UserTitle;

                if (userMessage == null)
                    userMessage = errorMessage.UserMessage;
            }

            // if both are filled out, no need to continue to the parent.
            if (title != null && userMessage != null)
                return;

            // Else call the same function but with the base type
            GetDefaultTitleAndUserMessage(errorMessageRepository, exceptionType.BaseType, ref title, ref userMessage);
        }

        /// <summary>
        /// Create a ErrorMessage by using BusinessException
        /// </summary>
        /// <param name="businessException"></param>
        /// <returns>newly created error message</returns>
        private ErrorMessage CreateErrorMessage(TravelEdgeException businessException)
        {
            using (var errorMessageRepository = RepositoryFactory.Get<ErrorMessage>())
            {
                var errorMessage = TransformExceptionToErrorMessage(businessException);

                //Insert the Error Message
                errorMessageRepository.InsertAndSave(errorMessage);

                return errorMessage;
            }
        }

        public static ErrorMessage TransformExceptionToErrorMessage(TravelEdgeException teException)
        {
            var errorMessage = TransformExceptionToErrorMessage(teException.GetType());

            // Default - Dev's can update after the fact
            errorMessage.InternalDescription = teException.Message;

            if (teException is ProviderException)
            {
                var providerException = teException as ProviderException;
                errorMessage.ProviderCode = providerException.ProviderCode;
                errorMessage.ProviderMessage = providerException.ProviderMessage;
            }
            else if (teException.ErrorCode != null)
            {
                // Amend the exception full name with the type
                errorMessage.ExceptionFullName += ErrorMessage.GetEnumSuffix(teException.ErrorCode);
            }

            return errorMessage;
        }

        public static ErrorMessage TransformExceptionToErrorMessage(Type exceptionType)
        {
            var errorMessage = new ErrorMessage();

            //Set the Default Exception Type (note these shouldn't change, except for Error which could be changed to an Error.
            if (typeof(InformatonException).IsAssignableFrom(exceptionType))
                errorMessage.ErrorTypeId = ErrorMessageTypes.Informational;
            else if (typeof(BusinessException).IsAssignableFrom(exceptionType))
                errorMessage.ErrorTypeId = ErrorMessageTypes.Business;
            else if (typeof(ConflictException).IsAssignableFrom(exceptionType))
                errorMessage.ErrorTypeId = ErrorMessageTypes.Conflict;
            else if (typeof(NotFoundException).IsAssignableFrom(exceptionType))
                errorMessage.ErrorTypeId = ErrorMessageTypes.NotFound;
            else if (typeof(UnauthorizedException).IsAssignableFrom(exceptionType))
                errorMessage.ErrorTypeId = ErrorMessageTypes.UnAuthorized;
            else if (typeof(ForbiddenException).IsAssignableFrom(exceptionType))
                errorMessage.ErrorTypeId = ErrorMessageTypes.Forbidden;
            else if (typeof(RequestValidationException).IsAssignableFrom(exceptionType))
                errorMessage.ErrorTypeId = ErrorMessageTypes.Validation;
            else
                errorMessage.ErrorTypeId = ErrorMessageTypes.Error;

            //Get the Unique Message Key
            errorMessage.ExceptionFullName = exceptionType.FullName;
            errorMessage.Reviewed = false; // needs reviewing/copy
            //Set default values
            errorMessage.IsInline = false; // Most errors will be an overlay
            errorMessage.IsSupportRequestEnabled = errorMessage.ErrorTypeId != ErrorMessageTypes.Informational; // Default

            return errorMessage;
        }

        /// <summary>
        /// To get error message by id
        /// </summary>
        /// <param name="errorMessageId"></param>
        /// <returns>ErrorMessageDto</returns>
        public ErrorMessageDto GetErrorMessage(int errorMessageId)
        {
            using (var errorMessageRepository = RepositoryFactory.Get<ErrorMessage>())
            {
                return ExceptionsMapper.Map(errorMessageRepository.GetByID(errorMessageId));
            }
        }

        /// <summary>
        /// To get error message by id
        /// </summary>
        /// <param name="errorMessageId"></param>
        /// <returns>ErrorMessageDto</returns>
        public ErrorMessageDto GetErrorMessage(string fullName)
        {
            using (var errorMessageRepository = RepositoryFactory.Get<ErrorMessage>())
            {
                return ExceptionsMapper.Map(errorMessageRepository.SingleOrDefault(t => t.ExceptionFullName == fullName));
            }
        }


        /// <summary>
        /// To get all the available error messages
        /// </summary>
        /// <returns>Collection of ErrorMessages</returns>
        public IEnumerable<ErrorMessageDto> GetErrorMessages()
        {
            using (var errorMessageRepository = RepositoryFactory.Get<ErrorMessage>())
            {
                var errorEntities = errorMessageRepository.GetAll();
                var errorDtos = new List<ErrorMessageDto>();

                foreach (var errorEntity in errorEntities)
                {
                    ErrorMessageDto error = GetErrorMessageWithReflectedDetails(errorEntity);

                    if(error != null)
                        errorDtos.Add(error);
                }

                return errorDtos;
            }
        }

        private ErrorMessageDto GetErrorMessageWithReflectedDetails(ErrorMessage errorEntity)
        {
            var error = ExceptionsMapper.Map(errorEntity);

            var d = ServiceLocator.CachingManager.GetOrAddToCache(
                errorEntity.ExceptionFullName, () =>
                    GetReflectedExceptionDetails(errorEntity.ExceptionFullName), 5000
                );

            if (d.IsObsolete)
                return null;

            // Any that don't seem to be referenced anymore mark with a warning.
            if (d.ErrorType == null)
            {
                error.Name = "[!!] " + error.Name;
            }
            else
            {
                error.ParentErrorMessageId = d.ParentErrorMessageId;
                error.CodeDescription = d.Description;
                error.Subsitutions = d.Subsitutions;
            }
            return error;
        }

        public class ReflectedErrorDetails
        {
            public Type ErrorType { get; set; }

            /// <summary>
            /// Name/Type
            /// </summary>
            public List<string> Subsitutions { get; set; }

            public Type EnumType { get; set; }
            public Enum EnumValue { get; set; }
            public int? ParentErrorMessageId { get; set; }
            public String Description { get; set; }
            public bool IsObsolete { get; set; }
        }
         
        public ReflectedErrorDetails GetReflectedExceptionDetails(string exceptionClass)
        {
            ReflectedErrorDetails d = new ReflectedErrorDetails();

            try
            {
                string[] exceptionFullName = exceptionClass.Split('|');

                var exceptionTypes =
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Select(t => t.GetType(exceptionFullName[0]))
                        .Where(t => t != null)
                        .ToList();

                if (exceptionTypes.Count() != 1)
                {
                    throw new ApplicationException("Could not get type " + exceptionClass + " as " + exceptionTypes.Count() +
                        " were found.");
                }

                d.ErrorType = exceptionTypes.Single();

                if (d.ErrorType.GetAttribute<DescriptionAttribute>() != null)
                    d.Description = d.ErrorType.GetAttribute<DescriptionAttribute>().Description;

                if (d.ErrorType.GetAttribute<ObsoleteAttribute>() != null)
                    d.IsObsolete = true;

                // See if there are any subsitutions
                if (d.ErrorType.IsSubclassOf(typeof(BusinessException)))
                {
                    var properties = d.ErrorType.GetProperties().Where(t => t.DeclaringType == d.ErrorType); // filter out system and parent properties

                    if (properties.Any())
                    {
                        d.Subsitutions = properties.Select(t => t.Name + " (" + t.PropertyType.Name + ")").ToList();
                    }
                }

                string errorBase = d.ErrorType.BaseType.FullName;

                if (exceptionFullName.Length == 2) // enum
                    errorBase = exceptionFullName[0];
                else
                    errorBase = d.ErrorType.BaseType.FullName;

                using (var errorMessageRepository = RepositoryFactory.Get<ErrorMessage>())
                {
                    var parent =
                        errorMessageRepository.Get(t => t.ExceptionFullName.Equals(errorBase))
                            .SingleOrDefault();

                    if (parent != null)
                    {
                        d.ParentErrorMessageId = parent.ErrorMessageId;
                    }
                }

                if (exceptionFullName.Count() == 2)
                {
                    var enumTypeName = String.Join(".", exceptionFullName[1].Split('.').Reverse().Skip(1).Reverse());

                    // is an error
                    var errorCodeTypes =
                        AppDomain.CurrentDomain.GetAssemblies()
                            .Select(t => t.GetType(enumTypeName))
                            .Where(t => t != null)
                            .ToList();

                    if (errorCodeTypes.Count() != 1)
                    {
                        throw new ApplicationException("Could not get enum type " + enumTypeName + " as " + errorCodeTypes.Count() +
                            " were found.");
                    }

                    d.EnumType = errorCodeTypes.Single();

                    d.EnumValue = (Enum) Enum.Parse(d.EnumType, exceptionFullName[1].Split('.').Last());
                    
                    if(d.EnumValue == null)
                        throw new ApplicationException("Could not get enum type " + enumTypeName + " value " + exceptionFullName[1].Split('.').Last());

                    if (d.EnumType.GetAttribute<ObsoleteAttribute>() != null)
                        d.IsObsolete = true;

                    if (d.EnumValue.GetType().GetAttribute<ObsoleteAttribute>() != null)
                        d.IsObsolete = true;

                    d.Description = d.EnumValue.GetDescription();
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(GetType().Name, "GetReflectedExceptionDetails", ex,
                    "Could not reflect on " + exceptionClass);
                return new ReflectedErrorDetails();
            }

            return d; 
        }

        /// <summary>
        /// To Update an existing error message
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>bool</returns>
        public bool UpdateErrorMessage(ErrorMessageDto errorMessage)
        {
            if (String.IsNullOrWhiteSpace(errorMessage.UserTitle))
                errorMessage.UserTitle = null;
            if (String.IsNullOrWhiteSpace(errorMessage.UserMessage))
                errorMessage.UserMessage = null;
            if (String.IsNullOrWhiteSpace(errorMessage.InternalDescription))
                errorMessage.InternalDescription = null;

            try
            {
                using (var errorMessageRepository = RepositoryFactory.Get<ErrorMessage>())
                {
                    errorMessageRepository.Update(AutoMapper.Mapper.Map<ErrorMessage>(errorMessage));
                    errorMessageRepository.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(this.GetType().Name, "UpdateErrorMessage", ex);
                return false;
            }
        }

        /// <summary>
        /// To delete an error message
        /// </summary>
        /// <param name="errorMessageId"></param>
        /// <returns>bool</returns>
        public bool DeleteErrorMessage(int errorMessageId)
        {
            try
            {
                using (var errorMessageRepository = RepositoryFactory.Get<ErrorMessage>())
                {
                    var errorMessage = errorMessageRepository.GetByID(errorMessageId);
                    errorMessageRepository.Delete(errorMessage);
                    errorMessageRepository.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(this.GetType().Name, "DeleteErrorMessage", ex);
                return false;
            }
        }
    }
}
