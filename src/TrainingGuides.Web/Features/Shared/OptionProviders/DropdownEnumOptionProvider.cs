using System.ComponentModel;
using EnumsNET;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Shared.OptionProviders;

public class DropdownEnumOptionProvider<T> : IDropDownOptionsProvider where T : struct, Enum
{
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
}
