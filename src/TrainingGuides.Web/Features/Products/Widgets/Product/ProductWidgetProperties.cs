using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Products.Widgets.Product;

public class ProductWidgetProperties : IWidgetProperties
{
    // [PageSelectorComponent(Label = "Selected product", Order = 1, MaximumPages = 1, Sortable = true,
    // ItemModifierType = typeof(ProductComparatorWidgetItemModifier))]
    // public IEnumerable<PageRelatedItem> Product { get; set; }

    [CheckBoxComponent(Label = "Show product features", Order = 1)]
    public bool ShowProductFeatures { get; set; }

    [TextInputComponent(Label = "CTA", Order = 2)]
    public string CTA { get; set; }

    [CheckBoxComponent(Label = "Open in new tab", Order = 3)]
    public bool OpenInNewTab { get; set; }

    [DropDownComponent(Label = "Align content", Order = 4, Options = "L;Left\nR;Right")]
    public string ContentPosition { get; set; }

    [DropDownComponent(Label = "Card size", Order = 5, Options = "1;Full\n2;Wide\n3;Middle\n4;Slim")]
    public string CardSize { get; set; }

    [CheckBoxComponent(Label = "Show product image", Order = 6)]
    public bool ShowProductImage { get; set; } = true;
}