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
            items.Select(item => new DropDownOptionItem
            {
                Text = Enum.GetName(item),
                Value = Enum.GetName(item)
            })
        );
    }

    public virtual T Parse(string value, T defaultValue) =>
        Enum.TryParse<T>(value, true, out var parsed)
            ? parsed
            : defaultValue;
}
