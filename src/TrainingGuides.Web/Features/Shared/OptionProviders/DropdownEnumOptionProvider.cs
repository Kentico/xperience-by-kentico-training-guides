using System.ComponentModel;
using CMS.Core;
using EnumsNET;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Shared.OptionProviders;

/// <summary>
/// Offers an enum's members as dropdown options. The text is the member's <see cref="DescriptionAttribute"/>,
/// or its name. A description may hold localization expressions ({$resource.key$}); plain text is shown as is.
/// </summary>
public class DropdownEnumOptionProvider<T>(ILocalizationService localizationService) : IDropDownOptionsProvider
    where T : struct, Enum
{
    public Task<IEnumerable<DropDownOptionItem>> GetOptionItems()
    {
        var results = Enums.GetMembers<T>(EnumMemberSelection.All)
            .Select(enumItem =>
            {
                string text = enumItem.Attributes.OfType<DescriptionAttribute>().FirstOrDefault()?.Description ?? enumItem.Name;
                string value = enumItem.Value.ToString();

                return new DropDownOptionItem { Value = value, Text = localizationService.LocalizeString(text) };
            });

        return Task.FromResult(results.AsEnumerable());
    }
}
