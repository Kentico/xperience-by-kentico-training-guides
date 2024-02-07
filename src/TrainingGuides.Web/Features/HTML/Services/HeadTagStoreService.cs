namespace TrainingGuides.Web.Features.HTML.Services;

public class HeadTagStoreService : IHeadTagStoreService
{
    private readonly SortedDictionary<CodeLocation, List<string>> store = [];

    public async Task<List<string>> GetCodeAsync(CodeLocation location)
    {
        var result = new List<string>();

        if (store.TryGetValue(location, out var values))
            result = await Task.FromResult(values);

        return result;
    }

    public async Task StoreCodeAsync(CodeLocation location, string tag)
    {
        if (store.TryGetValue(location, out var values))
        {
            values.Add(tag);
        }
        else
        {
            store[location] =
            [
                tag
            ];
        }

        await Task.CompletedTask;
    }
}
