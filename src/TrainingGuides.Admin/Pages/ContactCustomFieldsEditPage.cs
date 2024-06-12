using CMS.ContactManagement;
using CMS.DataEngine;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Admin.DigitalMarketing.UIPages;

[assembly: UIPage(
    parentType: typeof(ContactEditSection),
    slug: "trainingguidesfieldsedit",
    uiPageType: typeof(ContactCustomFieldsEditPage),
    name: "Custom fields",
    templateName: TemplateNames.EDIT,
    order: 150)]

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
        return await SetContactIsMember(items);
    }

    private async Task<string?> GetContactMemberId()
    {
        var infoObject = await GetInfoObject();

        return contactInfoProvider.Get()
            .WhereEquals(nameof(ContactInfo.ContactID), infoObject.ContactID)
            .First().GetValue(CONTACT_MEMBER_ID_FIELD_NAME) as string;
    }

    /// <summary>
    /// Sets the value of the "Is Member" field based on the value of the "Member ID" field.
    /// </summary>
    /// <param name="items">Collection of form items</param>
    /// <returns>The updated collection of items</returns>
    private async Task<ICollection<IFormItem>> SetContactIsMember(ICollection<IFormItem> items)
    {
        var contactIsMemberField = items.OfType<IFormComponent>()
            .FirstOrDefault(i => i.Name == CONTACT_IS_MEMBER_FIELD_NAME) as TextWithLabelComponent;

        string contactIsMember = string.IsNullOrEmpty(await GetContactMemberId())
            ? "No"
            : "Yes";

        contactIsMemberField?.SetValue(contactIsMember);

        return items;
    }
}