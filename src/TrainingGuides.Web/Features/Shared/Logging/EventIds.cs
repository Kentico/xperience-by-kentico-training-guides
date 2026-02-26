namespace TrainingGuides.Web.Features.Shared.Logging;

/// <summary>
/// Contains event IDs for logging.
/// </summary>
internal static class EventIds
{
    public static readonly EventId RetrieveUrl = new(1001, nameof(RetrieveUrl));
    public static readonly EventId MemberRegistration = new(1002, nameof(MemberRegistration));
    public static readonly EventId MemberSignIn = new(1003, nameof(MemberSignIn));
    public static readonly EventId ProductVariantHasMultipleParents = new(1011, nameof(ProductVariantHasMultipleParents));
    public static readonly EventId ProductStockMultipleRecords = new(1012, nameof(ProductStockMultipleRecords));
}