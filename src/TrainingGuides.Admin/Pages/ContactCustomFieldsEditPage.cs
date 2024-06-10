using CMS.ContactManagement;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Admin.DigitalMarketing.UIPages;

// Registers the UI page
[assembly: UIPage(
    parentType: typeof(ContactEditSection),
    slug: "trainingguidescontactfields",
    uiPageType: typeof(ContactCustomFieldsEditPage),
    name: "Custom fields",
    templateName: TemplateNames.EDIT,
    order: 150)]

public class ContactCustomFieldsEditPage : InfoEditPage<ContactInfo>
{
    [PageParameter(typeof(IntPageModelBinder))]
    public override int ObjectId { get; set; }

    public ContactCustomFieldsEditPage(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder)
             : base(formComponentMapper, formDataBinder)
    {
    }

    public override Task ConfigurePage()
    {
        PageConfiguration.UIFormName = "trainingguidescontactfields";
        return base.ConfigurePage();
    }
}