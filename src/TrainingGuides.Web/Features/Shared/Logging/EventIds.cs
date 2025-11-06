namespace TrainingGuides.Web.Features.Shared.Logging;

/// <summary>
/// Contains event IDs for logging.
/// </summary>
internal static class EventIds
{
    public static readonly EventId RetrieveUrl = new(1001, nameof(RetrieveUrl));
    public static readonly EventId MemberRegistration = new(1002, nameof(MemberRegistration));
    public static readonly EventId MemberSignIn = new(1003, nameof(MemberSignIn));
    public static readonly EventId ImportPathNotFound = new(1004, nameof(ImportPathNotFound));
    public static readonly EventId ImportContactsFromFileInfo = new(1005, nameof(ImportContactsFromFileInfo));
    public static readonly EventId ImportContactsFromFileError = new(1006, nameof(ImportContactsFromFileError));
    public static readonly EventId EnsureContactGroupError = new(1007, nameof(EnsureContactGroupError));
    public static readonly EventId EnsureRecipientListError = new(1008, nameof(EnsureRecipientListError));
    public static readonly EventId ContactGroupNotFound = new(1009, nameof(ContactGroupNotFound));
    public static readonly EventId ContactGroupRebuildFailed = new(1010, nameof(ContactGroupRebuildFailed));
}