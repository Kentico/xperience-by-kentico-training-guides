using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using TrainingGuides.Admin.ProductStock;
using TrainingGuides.ProductStock;

// Registers this as the main edit page within the ProductStockEditSection
[assembly: UIPage(typeof(ProductStockEditSection), "general", typeof(ProductStockEdit), "General", TemplateNames.EDIT, UIPageOrder.First)]

namespace TrainingGuides.Admin.ProductStock;

// The actual edit page where the editing form is rendered and user interaction happens.
// Inherits from InfoEditPage<ProductAvailableStockInfo> to provide standard CRUD functionality.
public sealed class ProductStockEdit(IFormComponentMapper formComponentMapper,
    IFormDataBinder formDataBinder,
    IProductMetadataRetriever productMetadataRetriever) : InfoEditPage<ProductAvailableStockInfo>(formComponentMapper, formDataBinder)
{
    // Name of the UI form definition created in the admin interface
    private const string UI_FORM_COMPONENT_NAME = "ProductAvailableStockEdit";
    // Name of the field used to display the product name
    private const string PRODUCT_NAME_COMPONENT_NAME = "ProductAvailableStockProductName";

    // Object ID parameter from the URL route (e.g., /edit/123)
    [PageParameter(typeof(IntPageModelBinder))]
    public override int ObjectId { get; set; }

    public override async Task ConfigurePage()
    {
        // Specifies which UI form definition to use
        PageConfiguration.UIFormName = UI_FORM_COMPONENT_NAME;
        PageConfiguration.Headline = "Edit product stock";

        await base.ConfigurePage();
    }

    // Retrieves and modifies form items before they are displayed to the user
    // </summary>
    protected override async Task<ICollection<IFormItem>> GetFormItems()
    {
        var formItems = await base.GetFormItems();

        // Sets up the product name component to show which product this stock record belongs to
        await SetupProductNameComponent(formItems);

        return formItems;
    }

    // Sets up the product name component to display the associated product's name
    // This provides context to administrators about which product they're editing stock for
    private async Task SetupProductNameComponent(ICollection<IFormItem> formItems)
    {
        // Gets the current product stock record being edited
        var productStockInfo = await GetInfoObject();

        // Retrieves the product metadata to get the display name
        var productMetadata = await productMetadataRetriever.GetProductMetadata(productStockInfo);

        // Finds the product name form component and sets its value
        var productNameComponent = formItems.OfType<IFormComponent>()
            .FirstOrDefault(f => f.Name.Equals(PRODUCT_NAME_COMPONENT_NAME, StringComparison.OrdinalIgnoreCase));

        // Sets the product name (this is typically a read-only field for context)
        productNameComponent?.SetObjectValue(productMetadata.DisplayName);
    }
}