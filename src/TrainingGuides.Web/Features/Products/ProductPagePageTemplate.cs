using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TrainingGuides;
using TrainingGuides.Web.Features.Products;

[assembly: RegisterPageTemplate(
    identifier: ProductPagePageTemplate.IDENTIFIER,
    name: "Product page template",
    propertiesType: typeof(ProductPagePageTemplateProperties),
    customViewName: "~/Features/Products/ProductPagePageTemplate.cshtml",
    ContentTypeNames = [ProductPage.CONTENT_TYPE_NAME],
    IconClass = "xp-market")]

namespace TrainingGuides.Web.Features.Products;
public static class ProductPagePageTemplate
{
    public const string IDENTIFIER = "TrainingGuides.ProductPageTemplate";
}
