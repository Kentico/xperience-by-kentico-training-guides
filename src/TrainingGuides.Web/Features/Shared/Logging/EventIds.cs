namespace TrainingGuides.Web.Features.Shared.Logging;

/// <summary>
/// Contains event IDs for logging.
/// </summary>
internal static class EventIds
{
    public static readonly EventId RetrieveUrl = new(1001, nameof(RetrieveUrl));
    public static readonly EventId MemberRegistration = new(1002, nameof(MemberRegistration));
    public static readonly EventId MemberSignIn = new(1003, nameof(MemberSignIn));
}