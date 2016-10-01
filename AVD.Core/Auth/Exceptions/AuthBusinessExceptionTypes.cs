using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Core.Auth.Exceptions
{
    /// <summary>
    /// All auth related exceptions
    /// </summary>
    public enum AuthBusinessExceptionTypes
    {
        [Description("Your account is currently disabled.  Please contact an administrator." +
                     "|Occurs with IsApproved() = false for the MembershipUser object")]
        Login_AccountDisabled = 1,

        [Description("There is an issue with the setup of your account. Please contact the administrator (L:001)" + "|Occurs when the setup in AD is incorrect for a WVS user (e.g. they do not belong to a valid WVS group, not attached to an Office etc) - Resolution -AD")]
        Login_AccountSetupInvalid = 2,

        [Description("Your account was found in TRAMS however there is an issue bringing your information into TravelEdge ADX. Please contact the system administrator for support. (L:002) " + "|There was a problem with the account mapping from TRAMS to WVS - perhaps the TRAMS groups were not set up properly. Resolution - TRAMS.")]
        Login_ErrorMappingProviderAccountToLocal = 3,

        [Description("Login failed - could not validate credentials. Please retry and if you are still encountering a problem, contact your system administrator. (L:003)" + "|Most common case - incorrect username or password - Resolution - User (incorrect info), AD (username or password)")]
        Login_MembershipAuthenticationFailed = 4,

        [Description("There is an issue with the setup of your account. Please contact the system administrator for support. (L:004)" + "|There is more than one account in TRAMS with the given email address. Email addresses need to be unique - Resolution -TRAMS")]
        Login_MultipleMatchesOnEmail = 5,

        [Description("There is an issue with the setup of your account. Please contact the system administrator for support. (L:005)" + "|There isn't an account in TRAMS with a matching primary email. Resolution - TRAMS")]
        Login_NoMatchOnEmail = 6,

        [Description("There is an issue with the setup of your account. Please contact the system administrator for support. (L:010)" + "|The account isn't the primary one of the agent - the GROUPNAME in trams for this primary account is not set. This could be an indiciation of a bug on the email search code on the trams ws side.")]
        Login_NonPrimaryAgentProfileReturned = 7,

        [Description("Your profile can not be created as TRAMS is down. Please contact the system administrator for support. (L:006)" + "|The connection to TRAMS is down - can not create the profile for the agent. Resolution - IT (Server issue)")]
        Login_ProfileSyncCreateFailedTRAMSUnavailable = 8,

        [Description("Your profile can not be updated as TRAMS is down. Please contact the system administrator for support. (L:007)" + "|The connection to TRAMS is down - can not update the profile for the agent however we can still let them log in using their existing local account.  Resolution - IT (Server issue)")]
        Login_ProfileSyncUpdateFailedTRAMSUnavailable = 9,

        [Description("Your account has been deactivated in TRAMS. Please contact the system administrator for support.(L:008)" + "|TRAMS is no longer returning the given account - quite likely it has been deactivated or deleted. Resolution -TRAMS")]
        Login_ProviderNoLongerReturnsAccount = 10,

        [Description("The timeout period for T&C acceptance has expired. Please go back to the login page and try again." + "|The preauth cookie doesn't have a match in the cache - the most likely explaination is that it has been kicked out of the cache. The timeout for this is 30mins so the user has that long from when they log in to accepting the T&C.")]
        Login_TC_UserNotInCache = 11,

        [Description("Can not find user. Please contact the system administrator for support.  (L:009)" + "|Occurs when the email address the user types in can not be matched to a username in AD. Resolution - User (incorrect email) or AD (email does not exist)")]
        Login_UserNameNotFound = 12,

        [Description("There was an error sending the email, please try again later." + "|This not a validation error, there was a system error sending the email so all they can do is try later or contact support.")]
        Reset_CouldNotSendEmail = 13,

        [Description("The password does not meet the password policy requirements.")]
        Reset_PasswordPolicy = 14,

        [Description("The password reset link has expired.")]
        Reset_TokenExpired = 15
    }
}
