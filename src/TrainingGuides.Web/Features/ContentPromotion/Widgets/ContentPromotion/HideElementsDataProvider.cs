using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;
using TrainingGuides.Web.Features.ContentPromotion.Models;

namespace TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;

/// <summary>
/// Offers the card elements an editor can hide. Values are the stored
/// <see cref="ContentPromotionElement"/> names, so the selector and the resolution rules share one
/// list.
/// </summary>
public class HideElementsDataProvider : IGeneralSelectorDataProvider
{
    private static readonly ObjectSelectorListItem<string>[] elements =
    [
        Item(ContentPromotionElement.TITLE, "Title"),
        Item(ContentPromotionElement.DESCRIPTION, "Description"),
        Item(ContentPromotionElement.IMAGE, "Image"),
        Item(ContentPromotionElement.CALL_TO_ACTION, "Call to action")
    ];

    public Task<PagedSelectListItems<string>> GetItemsAsync(string searchTerm, int pageIndex, CancellationToken cancellationToken) =>
        Task.FromResult(new PagedSelectListItems<string>
        {
            Items = string.IsNullOrWhiteSpace(searchTerm)
                ? elements
                : elements.Where(element => element.Text.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)),
            NextPageAvailable = false
        });

    public Task<IEnumerable<ObjectSelectorListItem<string>>> GetSelectedItemsAsync(
        IEnumerable<string> selectedValues,
        CancellationToken cancellationToken) =>
        Task.FromResult((selectedValues ?? []).Select(value =>
            elements.FirstOrDefault(element => element.Value == value)
                ?? new ObjectSelectorListItem<string> { Value = value, Text = value, IsValid = false }));

    private static ObjectSelectorListItem<string> Item(string value, string text) =>
        new() { Value = value, Text = text, IsValid = true };
}
