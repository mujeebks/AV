using AVD.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AVD.Common.Exceptions
{

    [Description("Some of the items sent have not passed validation. The UI should be using the information returned and" +
                 "presenting it inline for the User to fix. If this fails, then a list of the invalid fields will be" +
                 "included in the message for the User to attempt to correct and resubmit")]
    public class RequestValidationException : TravelEdgeException
    {
        public class UxValidationResult
        {
            public IEnumerable<string> MemberNames { get; set; }
            public IEnumerable<string> DisplayMemberNames { get; set; }
            public String ErrorMessage { get; set; }
            
            public UxValidationResult(string errorMessage, IEnumerable<string> memberNames, IEnumerable<string> displayMemberNames)
            {
                this.ErrorMessage = errorMessage;
                this.MemberNames = memberNames;
                this.DisplayMemberNames = displayMemberNames;
            }
        }

        public IEnumerable<UxValidationResult> ValidationResults { get; set; }

        public RequestValidationException(IEnumerable<ValidationResult> validationResults)
            : base(null, null)
        {
            ValidationResults = validationResults.Select(
                t => new UxValidationResult(t.ErrorMessage, t.MemberNames, t.MemberNames.Select(ToDisplayMemberName) ));
        }

        private string ToDisplayMemberName(string memberName)
        {
            String valToReturn = memberName;

            try
            {
                var split = memberName.Split('.');
                valToReturn = split.Last();

                // Get the first index, if there is one.
                if (memberName.Contains("["))
                {
                    var indexStart = memberName.IndexOf('[');
                    var indexEnd = memberName.IndexOf(']');
                    var numStr = memberName.Substring(indexStart + 1, indexEnd - indexStart - 1);
                    int num;

                    valToReturn = String.Join("", valToReturn.TakeWhile(t => t != '['));

                    String s = null;

                    if (Int32.TryParse(numStr, out num))
                    {
                        // we have a number!
                        switch (num)
                        {
                            case 0:
                                s = "First";
                                break;
                            case 1:
                                s = "Second";
                                break;
                            case 2:
                                s = "Third";
                                break;
                            case 3:
                                s = "Forth";
                                break;
                            case 4:
                                s = "Fifth";
                                break;
                            default:
                                s = "Item " + (num+1) + " ";
                                break;
                        }
                    }
                    else
                    {
                        Logger.Instance.Warn(GetType().Name, "ToDisplayMemberName",
                            "Can not parse " + numStr + " as a number");
                    }

                    valToReturn = s + " " + valToReturn;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Warn(GetType().Name, "ToDisplayMemberName", ex);
            }

            Logger.Instance.Debug(GetType().Name, "ToDisplayMemberName", "MemberName = " + memberName + ", Display = " + valToReturn);
            return valToReturn;
        }
    }
}