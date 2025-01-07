using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.OrderBy;

namespace TrainingGuides.Web.Features.Gallery.Widgets.GalleryWidget;

public class GalleryWidgetProperties : IWidgetProperties
{
    [SmartFolderSelectorComponent(
        AllowedContentTypeIdentifiersFilter = typeof(GalleryImage),
        Label = "Smart folder",
        ExplanationText = "Select smart folder containing Gallery images you wish tp display",
        Order = 10)]
    public SmartFolderReference SmartFolder { get; set; } = new SmartFolderReference();

    [TextInputComponent(
        Label = "Title",
        Order = 20)]
    public string Title { get; set; } = "Gallery";

    [NumberInputComponent(
        Label = "Number of images to display per page",
        Order = 30)]
    public int TopN { get; set; } = 10;

    [DropDownComponent(
        Label = "Order images by",
        DataProviderType = typeof(DropdownEnumOptionProvider<OrderByOption>),
        Order = 40)]
    public string OrderBy { get; set; } = OrderByOption.NewestFirst.ToString();
}

