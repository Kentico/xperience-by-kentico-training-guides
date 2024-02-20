namespace TrainingGuides.Web.Features.Html.Services;

public class HeadTagStoreService : IHeadTagStoreService
{
    private readonly SortedDictionary<CodeLocation, List<string>> store = [];

    /// <summary>
    /// Retrieves Html code from the service's store.
    /// </summary>
    /// <param name="location">The designated <see cref="CodeLocation"/> value representing where the code is meant to be rendered.</param>
    /// <returns>A <see cref="List{string}"/> containing the Html markup meant for the specified location.</returns>
    public async Task<List<string>> GetCodeAsync(CodeLocation location)
    {
        var result = new List<string>();

        if (store.TryGetValue(location, out var values))
            result = await Task.FromResult(values);

        return result;
    }

    /// <summary>
    /// Saves Html code in the service's store.
    /// </summary>
    /// <param name="location">The designated <see cref="CodeLocation"/> value representing where the code is meant to be rendered.</param>
    /// <param name="tag">The Html markup meant to be saved.</param>
    /// <returns></returns>
    public async Task StoreCodeAsync(CodeLocation location, string tag)
    {
        if (store.TryGetValue(location, out var values))
        {
            values.Add(tag);
        }
        else
        {
            store[location] = [tag];
        }

        await Task.CompletedTask;
    }
}
