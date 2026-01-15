using CMS.Membership;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.DigitalCommerce.UIPages;
using TrainingGuides.Admin.ProductStock;

// Registers the application in the admin interface
[assembly: UIApplication(ProductStockApplication.IDENTIFIER, typeof(ProductStockApplication), "productstock", "Product stock", DigitalCommerceApplicationCategories.DIGITAL_COMMERCE, Icons.MoneyBill, TemplateNames.SECTION_LAYOUT)]

namespace TrainingGuides.Admin.ProductStock;

// Sets permissions required to access the application
// Create and delete permissions are not needed if stock records are created and deleted automatically with products
[UIPermission(SystemPermissions.VIEW, "{$base.roles.permissions.view$}")]
[UIPermission(SystemPermissions.UPDATE, "{$base.roles.permissions.update}")]
public sealed class ProductStockApplication : ApplicationPage
{
    public const string IDENTIFIER = "TrainingGuides.ProductStockApplication";
}