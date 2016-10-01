using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AVD.Common.Exceptions;
using AVD.DataAccessLayer;
using AVD.Common.Exceptions;
using AVD.DataAccessLayer.Enums;

namespace AVD.DataAccessLayer.Models
{
    public partial class ErrorMessage : BaseModel
    {
        public ErrorMessage()
        {
            ErrorOccurances = new List<ErrorOccurance>();
        }

        [Key]
        public int ErrorMessageId { get; set; }
        public ErrorMessageTypes ErrorTypeId { get; set; }
        public string ExceptionFullName { get; set; }
        public bool Reviewed { get; set; }
        public Nullable<int> ProviderId { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderMessage { get; set; }
        public string InternalDescription { get; set; }
        public string UserTitle { get; set; }
        public string UserMessage { get; set; }
        public bool IsInline { get; set; }
        public bool IsSupportRequestEnabled { get; set; }

        public virtual List<ErrorOccurance> ErrorOccurances { get; set; } 

        public string ExceptionClassFullName()
        {
            return ExceptionFullName.Split('|').First();
        }

        public string ErrorMessageCategory()
        {
            return GetErrorMessageCategory(ExceptionFullName);
        }
        public string ErrorMessageName()
        {
            return GetErrorMessageName(ExceptionFullName);
        }

        /// <summary>
        /// To get Category from Full Name
        /// </summary>
        /// <param name="exceptionFullName"></param>
        /// <returns>string</returns>
        public static string GetErrorMessageCategory(string exceptionFullName)
        {
            string categoryName = String.Empty;

            if (exceptionFullName.Contains("|"))
            {
                Regex exceptionWithEnum = new Regex(@"[|]{1}TE.Core.([\w.]*)([.]{1})([\w]*)ExceptionTypes",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

                var match = exceptionWithEnum.Match(exceptionFullName);

                if (match.Success)
                {
                    categoryName = match.Groups[1].Value.Replace(@".Exceptions",String.Empty);
                }
            }
            else if (exceptionFullName.StartsWith("TE.Common"))
            {
                categoryName = " Default"; // Space, so they appear first in the UI :)
            }
            else
            {

                exceptionFullName = exceptionFullName.Replace("TE.Core.", "");

                exceptionFullName = exceptionFullName.Substring(0,
                     exceptionFullName.LastIndexOf("."));

                if (exceptionFullName.EndsWith(".Exceptions"))
                    exceptionFullName = exceptionFullName.Replace(".Exceptions", "");

                categoryName = exceptionFullName;
            }
            
            return categoryName;
        }

        /// <summary>
        /// Gets a friendly name for the exception that is recongizable by admins.
        /// </summary>
        /// <param name="exceptionFullName"></param>
        /// <returns>string</returns>
        public static string GetErrorMessageName(string exceptionFullName)
        {
            var name = exceptionFullName.Split('.').LastOrDefault();

            if (name.EndsWith("Exception"))
                name = name.Substring(0, name.Length - "Exception".Length);

            if (!name.Equals("Business") && name.EndsWith("Business"))
                name = name.Substring(0, name.Length - "Business".Length);

            name = Regex.Replace(name, "(\\B[A-Z])", " $1");

            return name;
        }

        /// <summary>
        /// Creates a unique key for the exception
        /// unique key format will be exception type + enum type
        /// </summary>
        /// <returns></returns>
        public static string GetEnumSuffix(Enum errorCode)
        {
            if (errorCode != null)
            {
                return String.Format("{0}{1}.{2}", "|", errorCode.GetType().FullName, errorCode.ToString());
            }

            return null;
        }

        /// <summary>
        /// Creates a unique key for the exception
        /// unique key format will be exception type + enum type
        /// </summary>
        /// <remarks>
        /// Format
        /// With Error Code
        ///  TE.Core.Exceptions.BusinessException|TE.Core.Quotes.Dtos.AirModifyBusinessExceptionTypes.AutoTicketingFailed
        ///  Without Error Code
        /// TE.Core.Quotes.Exceptions.TravelerWithSameNameExistsQuoteBusinessException
        /// </remarks>
        /// <returns></returns>
        public static string GetExceptionFullName(TravelEdgeException exception)
        {
            //Format
            //With Error Code
            //TE.Core.Exceptions.BusinessException|TE.Core.Quotes.Dtos.AirModifyBusinessExceptionTypes.AutoTicketingFailed

            //Without Error Code
            //TE.Core.Quotes.Exceptions.TravelerWithSameNameExistsQuoteBusinessException

            StringBuilder sb = new StringBuilder();

            sb.Append(exception.GetType().FullName);

            if (exception.ErrorCode != null)
            {
                sb.Append(GetEnumSuffix(exception.ErrorCode));
            }

            return sb.ToString();
        }
    }
}
