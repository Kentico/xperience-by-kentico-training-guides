using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Shared.OptionsProviders;

public class DropdownEnumOptionsProvider<T> : IDropDownOptionsProvider where T : struct, Enum
{
    private readonly IEnumerable<T>? subset;

    public DropdownEnumOptionsProvider(IEnumerable<T>? subset = null)
    {
        this.subset = subset;
    }

    public Task<IEnumerable<DropDownOptionItem>> GetOptionItems()
    {
        var items = subset ?? Enum.GetValues<T>();

        return Task.FromResult(
            items.Select(x => new DropDownOptionItem { Text = Enum.GetName(x), Value = Enum.GetName(x) })
        );
    }

    public virtual T? Parse(string value)
    {
        if (!Enum.TryParse<T>(value, out var parsed))
            return null;
        else
            return parsed;
    }
}
