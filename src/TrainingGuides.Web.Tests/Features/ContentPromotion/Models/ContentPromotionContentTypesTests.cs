using System.Reflection;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.ContentPromotion.Models;

/// <summary>
/// An attribute argument must be a compile-time constant, so each selector repeats its content type
/// list instead of reading the array retrieval uses. These tests are what keeps the two in step: a
/// type the selector offers but retrieval does not name comes back as "could not be loaded".
/// </summary>
public class ContentPromotionContentTypesTests
{
    [Fact]
    public void SelectedPageSelector_OffersExactlyThePagesRetrievalNames() =>
        Assert.Equal(
            ContentPromotionContentTypes.PAGES.Order(),
            SelectorContentTypes(nameof(ContentPromotionWidgetProperties.SelectedPage)).Order());

    [Fact]
    public void SelectedContentItemSelector_OffersExactlyTheContentItemsRetrievalNames() =>
        Assert.Equal(
            ContentPromotionContentTypes.CONTENT_ITEMS.Order(),
            SelectorContentTypes(nameof(ContentPromotionWidgetProperties.SelectedContentItem)).Order());

    /// <summary>
    /// Reads the content type names passed to the selector's constructor from metadata. The
    /// attribute's own <c>AllowedContentItemTypeIdentifiers</c> holds GUIDs resolved from those
    /// names, so it cannot be compared with the arrays without a database.
    /// </summary>
    private static IEnumerable<string> SelectorContentTypes(string propertyName) =>
        typeof(ContentPromotionWidgetProperties)
            .GetProperty(propertyName)!
            .GetCustomAttributesData()
            .Single(attribute => attribute.AttributeType == typeof(ContentItemSelectorComponentAttribute))
            .ConstructorArguments
            .Single()
            .Value switch
        {
            IEnumerable<CustomAttributeTypedArgument> names => names.Select(name => (string)name.Value!),
            string name => [name],
            var other => throw new InvalidOperationException($"Unexpected selector argument: {other}")
        };
}
