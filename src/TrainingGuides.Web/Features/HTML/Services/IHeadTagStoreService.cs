namespace TrainingGuides.Web.Features.Html.Services;

public enum CodeLocation
{
    Widget,
    Head,
    AfterBodyStart,
    BeforeBodyEnd
}
public interface IHeadTagStoreService
{
    Task<List<string>> GetCodeAsync(CodeLocation location);
    Task StoreCodeAsync(CodeLocation location, string code);
}
