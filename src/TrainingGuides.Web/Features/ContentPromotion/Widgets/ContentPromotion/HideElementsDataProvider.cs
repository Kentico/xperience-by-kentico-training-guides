using CMS.Core;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;
using TrainingGuides.Web.Features.ContentPromotion.Models;

namespace TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;

/// <summary>
/// Offers the card elements an editor can hide. Values are the stored
/// <see cref="ContentPromotionElement"/> names, so the selector and the resolution rules share one
/// list. Texts are resource strings resolved in the editor's admin UI language.
/// </summary>
public class HideElementsDataProvider(ILocalizationService localizationService) : IGeneralSelectorDataProvider
{
    private const string RESOURCE_KEY_PREFIX = "TrainingGuides.ContentPromotionWidget.HideElements.Options.";

    private static readonly (string Value, string ResourceKey)[] elements =
    [
        (ContentPromotionElement.TITLE, RESOURCE_KEY_PREFIX + "Title"),
        (ContentPromotionElement.DESCRIPTION, RESOURCE_KEY_PREFIX + "Description"),
        (ContentPromotionElement.IMAGE, RESOURCE_KEY_PREFIX + "Image"),
        (ContentPromotionElement.CALL_TO_ACTION, RESOURCE_KEY_PREFIX + "CallToAction")
    ];

    public Task<PagedSelectListItems<string>> GetItemsAsync(string searchTerm, int pageIndex, CancellationToken cancellationToken)
    {
        var items = GetLocalizedItems();

        return Task.FromResult(new PagedSelectListItems<string>
        {
            Items = string.IsNullOrWhiteSpace(searchTerm)
                ? items
                : items.Where(item => item.Text.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)),
            NextPageAvailable = false
        });
    }

    public Task<IEnumerable<ObjectSelectorListItem<string>>> GetSelectedItemsAsync(
        IEnumerable<string> selectedValues,
        CancellationToken cancellationToken)
    {
        var items = GetLocalizedItems();

        return Task.FromResult((selectedValues ?? []).Select(value =>
            items.FirstOrDefault(item => item.Value == value)
                ?? new ObjectSelectorListItem<string> { Value = value, Text = value, IsValid = false }));
    }

    private List<ObjectSelectorListItem<string>> GetLocalizedItems() =>
    [
        .. elements.Select(element => new ObjectSelectorListItem<string>
        {
            Value = element.Value,
            Text = localizationService.GetString(element.ResourceKey),
            IsValid = true
        })
    ];
}
