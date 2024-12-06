namespace TrainingGuides.Web.Features.Membership.Widgets.Registration;

public enum EmailConfirmationState
{
    SuccessConfirmed,
    SuccessAlreadyConfirmed,
    FailureNotYetConfirmed,
    FailureConfirmationFailed,
    ConfirmationResent
}