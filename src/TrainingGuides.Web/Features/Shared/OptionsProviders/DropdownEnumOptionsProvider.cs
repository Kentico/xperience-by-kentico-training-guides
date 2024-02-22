using System.ComponentModel;
using EnumsNET;
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
        var results = Enums.GetMembers<T>(EnumMemberSelection.All)
            .Select(enumItem =>
            {
                string text = enumItem.Attributes.OfType<DescriptionAttribute>().FirstOrDefault()?.Description ?? enumItem.Name;
                string value = enumItem.Value.ToString();

                return new DropDownOptionItem { Value = value, Text = text };
            });

        return Task.FromResult(results.AsEnumerable());
    }

    public virtual T Parse(string value, T defaultValue) =>
        Enums.TryParse<T>(value, true, out var parsed)
            ? parsed
            : defaultValue;
}
