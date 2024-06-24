using CMS.ContactManagement;
using CMS.DataEngine;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Admin.DigitalMarketing.UIPages;
using TrainingGuides.Admin.Pages;

[assembly: UIPage(
    parentType: typeof(ContactEditSection),
    slug: "trainingguidesfieldsedit",
    uiPageType: typeof(ContactCustomFieldsEditPage),
    name: "Custom fields",
    templateName: TemplateNames.EDIT,
    order: 150)]

namespace TrainingGuides.Admin.Pages;
public class ContactCustomFieldsEditPage : InfoEditPage<ContactInfo>
{
    [PageParameter(typeof(IntPageModelBinder))]
    public override int ObjectId { get; set; }

    private readonly IInfoProvider<ContactInfo> contactInfoProvider;

    private const string CONTACT_IS_MEMBER_FIELD_NAME = "TrainingGuidesContactIsMember";
    private const string CONTACT_MEMBER_ID_FIELD_NAME = "TrainingGuidesContactMemberId";

    public ContactCustomFieldsEditPage(
        IFormComponentMapper formComponentMapper,
        IFormDataBinder formDataBinder,
        IInfoProvider<ContactInfo> contactInfoProvider)
             : base(formComponentMapper, formDataBinder)
    {
        this.contactInfoProvider = contactInfoProvider;
    }

    public override Task ConfigurePage()
    {
        PageConfiguration.UIFormName = "trainingguidesfieldsedit";
        return base.ConfigurePage();
    }

    protected override async Task<ICollection<IFormItem>> GetFormItems()
    {
        var items = await base.GetFormItems();
        return SetContactIsMember(items);
    }

    private string? GetContactMemberId() =>
        contactInfoProvider.Get()
            .WhereEquals(nameof(ContactInfo.ContactID), ObjectId)
            .FirstOrDefault()
            ?.GetValue(CONTACT_MEMBER_ID_FIELD_NAME) as string;

    /// <summary>
    /// Sets the value of the "Is Member" field based on the value of the "Member ID" field.
    /// </summary>
    /// <param name="items">Collection of form items</param>
    /// <returns>The updated collection of items</returns>
    private ICollection<IFormItem> SetContactIsMember(ICollection<IFormItem> items)
    {
        var contactIsMemberField = items.OfType<IFormComponent>()
            .FirstOrDefault(i => i.Name == CONTACT_IS_MEMBER_FIELD_NAME) as TextWithLabelComponent;

        string contactIsMember = string.IsNullOrEmpty(GetContactMemberId())
            ? "No"
            : "Yes";

        contactIsMemberField?.SetValue(contactIsMember);

        return items;
    }
}