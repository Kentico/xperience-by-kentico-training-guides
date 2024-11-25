namespace TrainingGuides.Web.Features.Shared.Helpers;

internal static class ApplicationConstants
{
    //Multilingual
    public const string LANGUAGE_KEY = "language";

    //Membership
    public const string EXPECTED_SIGN_IN_PATH = "/Sign_in";
    public const string ACCESS_DENIED_ACTION_PATH = "/Authentication/AccessDenied";
    public const string REQUEST_RESET_PASSWORD_ACTION_PATH = "/MembershipManagement/RequestResetPassword";
    public const string PASSWORD_RESET_ACTION_PATH = "/MembershipManagement/ResetPassword";
    public const string REGISTER_ACTION_PATH = "/Registration/Register";
    public const string RETURN_URL_PARAMETER = "returnUrl";
}