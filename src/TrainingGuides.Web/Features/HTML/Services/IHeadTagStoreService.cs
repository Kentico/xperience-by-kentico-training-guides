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
    public Task<List<string>> GetCodeAsync(CodeLocation location);
    public Task StoreCodeAsync(CodeLocation location, string code);
}
