using CMS.Activities;
using CMS.Base;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.Helpers;
using CMS.OnlineForms;
using TrainingGuides.Web.Features.DataProtection.Collectors;
using TrainingGuides.Web.Features.DataProtection.Services;

namespace TrainingGuides.Web.Features.DataProtection.Erasers;

public class ContactDataEraser : IPersonalDataEraser
{
    private readonly IFormCollectionService formCollectionService;
    private readonly Dictionary<Guid, FormDefinition> forms;

    private readonly IInfoProvider<ContactInfo> contactInfoProvider;
    private readonly IInfoProvider<ActivityInfo> activityInfoProvider;
    private readonly IInfoProvider<ConsentAgreementInfo> consentAgreementInfoProvider;
    private readonly IInfoProvider<BizFormInfo> bizFormInfoProvider;

    public ContactDataEraser(IFormCollectionService formCollectionService,
        IInfoProvider<ContactInfo> contactInfoProvider,
        IInfoProvider<ActivityInfo> activityInfoProvider,
        IInfoProvider<ConsentAgreementInfo> consentAgreementInfoProvider,
        IInfoProvider<BizFormInfo> bizFormInfoProvider)
    {
        this.formCollectionService = formCollectionService;

        this.contactInfoProvider = contactInfoProvider;
        this.activityInfoProvider = activityInfoProvider;
        this.consentAgreementInfoProvider = consentAgreementInfoProvider;
        this.bizFormInfoProvider = bizFormInfoProvider;

        forms = this.formCollectionService.GetForms();
    }

    public void Erase(IEnumerable<BaseInfo> identities, IDictionary<string, object> configuration)
    {
        var contacts = identities.OfType<ContactInfo>().ToList();

        if (!contacts.Any())
        {
            return;
        }

        var contactIds = contacts.Select(c => c.ContactID).ToList();
        var contactEmails = contacts.Select(c => c.ContactEmail).ToList();

        using (new CMSActionContext())
        {
            DeleteSubmittedFormsActivities(contactIds, configuration);
            DeleteActivities(contactIds, configuration);
            DeleteContacts(contacts, configuration);
            DeleteSiteSubmittedFormsData(contactEmails, contactIds, configuration);
        }
    }

    private void DeleteSubmittedFormsActivities(ICollection<int> contactIds,
        IDictionary<string, object> configuration)
    {
        if (configuration.TryGetValue("DeleteSubmittedFormsActivities", out object? deleteSubmittedFormsActivities)
            && ValidationHelper.GetBoolean(deleteSubmittedFormsActivities, false))
        {
            activityInfoProvider.BulkDelete(new WhereCondition()
                .WhereEquals("ActivityType", PredefinedActivityType.BIZFORM_SUBMIT)
                .WhereIn("ActivityContactID", contactIds));
        }
    }

    private void DeleteSiteSubmittedFormsData(ICollection<string> emails, ICollection<int> contactIDs,
        IDictionary<string, object> configuration)
    {
        if (configuration.TryGetValue("DeleteSubmittedFormsData", out object? deleteSubmittedForms)
            && ValidationHelper.GetBoolean(deleteSubmittedForms, false))
        {
            var consentAgreementGuids = consentAgreementInfoProvider.Get()
                .Columns("ConsentAgreementGuid")
                .WhereIn("ConsentAgreementContactID", contactIDs);

            var formClasses = bizFormInfoProvider.Get()
                .Source(s => s.LeftJoin<DataClassInfo>("CMS_Form.FormClassID", "ClassID"))
                .WhereIn("FormGUID", forms.Select(pair => pair.Key).ToList());

            formClasses.ForEachRow(row =>
            {
                var bizForm = new BizFormInfo(row);
                var formDefinition = forms[bizForm.FormGUID];

                var bizFormItems = formCollectionService.GetBizFormItems(emails, consentAgreementGuids, row, formDefinition);

                foreach (var bizFormItem in bizFormItems)
                {
                    bizFormItem.Delete();
                }
            });
        }
    }

    private void DeleteActivities(List<int> contactIds, IDictionary<string, object> configuration)
    {
        if (configuration.TryGetValue("DeleteActivities", out object? deleteActivities)
            && ValidationHelper.GetBoolean(deleteActivities, false))
        {
            activityInfoProvider.BulkDelete(
                new WhereCondition().WhereIn("ActivityContactID", contactIds));
        }
    }

    private void DeleteContacts(IEnumerable<ContactInfo> contacts, IDictionary<string, object> configuration)
    {
        if (configuration.TryGetValue("DeleteContacts", out object? deleteContacts) &&
            ValidationHelper.GetBoolean(deleteContacts, false))
        {
            foreach (var contactInfo in contacts.Where(contact => contact.ContactID > 0))
            {
                contactInfoProvider.Delete(contactInfo);
            }
        }
    }
}
