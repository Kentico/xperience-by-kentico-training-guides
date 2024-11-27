namespace TrainingGuides.Web.Features.Shared.Helpers;

internal static class ApplicationConstants
{
    //Multilingual
    public const string LANGUAGE_KEY = "language";

    //Membership
    public const string EXPECTED_SIGN_IN_PATH = "/Sign_in";
    public const string ACCESS_DENIED_ACTION_PATH = "/Authentication/AccessDenied";
    public const string REQUEST_RESET_PASSWORD_ACTION_PATH = "/MembershipManagement/RequestResetPassword";
    public const string UPDATE_PROFILE_ACTION_PATH = "/MembershipManagement/UpdateProfile";
    public const string PASSWORD_RESET_ACTION_PATH = "/MembershipManagement/ResetPassword";
    public const string REGISTER_ACTION_PATH = "/Registration/Register";
    public const string CONFIRM_REGISTRATION_ACTION_PATH = "/Registration/Confirm";
    public const string RESEND_VERIFICATION_EMAIL = "/Registration/ResendVerificationEmail";
    public const string AUTHENTICATE_ACTION_PATH = "/Authentication/Authenticate";
    public const string RETURN_URL_PARAMETER = "returnUrl";
}