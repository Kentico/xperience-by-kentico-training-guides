using System.Data;
using System.Xml;

using CMS.Activities;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.Globalization;
using CMS.Helpers;
using CMS.OnlineForms;
using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Web.Features.DataProtection.Writers;

namespace TrainingGuides.Web.Features.DataProtection.Collectors;

public class ContactDataCollectorCore
{
    private readonly List<CollectedColumn> contactInfoColumns =
    [
        new CollectedColumn("ContactFirstName", "First name"),
        new CollectedColumn("ContactMiddleName", "Middle name"),
        new CollectedColumn("ContactLastName", "Last name"),
        new CollectedColumn("ContactJobTitle", "Job title"),
        new CollectedColumn("ContactAddress1", "Address"),
        new CollectedColumn("ContactCity", "City"),
        new CollectedColumn("ContactZIP", "ZIP"),
        new CollectedColumn("ContactStateID", ""),
        new CollectedColumn("ContactCountryID", ""),
        new CollectedColumn("ContactMobilePhone", "Mobile phone"),
        new CollectedColumn("ContactBusinessPhone", "Business phone"),
        new CollectedColumn("ContactEmail", "Email"),
        new CollectedColumn("ContactBirthday", "Birthday"),
        new CollectedColumn("ContactGender", "Gender"),
        new CollectedColumn("ContactNotes", "Notes"),
        new CollectedColumn("ContactGUID", "GUID"),
        new CollectedColumn("ContactLastModified", "Last modified"),
        new CollectedColumn("ContactCreated", "Created"),
        new CollectedColumn("ContactCampaign", "Campaign"),
        new CollectedColumn("ContactCompanyName", "Company name")
    ];

    private readonly List<CollectedColumn> consentAgreementInfoColumns =
    [
        new CollectedColumn("ConsentAgreementGuid", "GUID"),
        new CollectedColumn("ConsentAgreementRevoked", "Consent action"),
        new CollectedColumn("ConsentAgreementTime", "Performed on")
    ];

    private readonly List<CollectedColumn> consentInfoColumns =
    [
        new CollectedColumn("ConsentGUID", "GUID"),
        new CollectedColumn("ConsentDisplayName", "Consent name"),
        new CollectedColumn("ConsentContent", "Full text")
    ];

    private readonly List<CollectedColumn> consentArchiveInfoColumns =
    [
        new CollectedColumn("ConsentArchiveGUID", "GUID"),
        new CollectedColumn("ConsentArchiveContent", "Full text")
    ];

    private readonly List<CollectedColumn> activityInfoColumns =
    [
        new CollectedColumn("ActivityId", "ID"),
        new CollectedColumn("ActivityCreated", "Created"),
        new CollectedColumn("ActivityType", "Type"),
        new CollectedColumn("ActivityUrl", "URL"),
        new CollectedColumn("ActivityTitle", "Title"),
        new CollectedColumn("ActivityItemId", "")
    ];

    private readonly List<CollectedColumn> accountInfoColumns =
    [
        new CollectedColumn("AccountName", "Name"),
        new CollectedColumn("AccountAddress1", "Address"),
        new CollectedColumn("AccountAddress2", "Address 2"),
        new CollectedColumn("AccountCity", "City"),
        new CollectedColumn("AccountZip", "ZIP"),
        new CollectedColumn("AccountWebSite", "Web site"),
        new CollectedColumn("AccountEmail", "Email"),
        new CollectedColumn("AccountPhone", "Phone"),
        new CollectedColumn("AccountFax", "Fax"),
        new CollectedColumn("AccountNotes", "Notes"),
        new CollectedColumn("AccountGUID", "GUID")
    ];

    private readonly List<CollectedColumn> countryInfoColumns =
    [
        new CollectedColumn("CountryDisplayName", "Country")
    ];

    private readonly List<CollectedColumn> stateInfoColumns =
    [
        new CollectedColumn("StateDisplayName", "State")
    ];

    private readonly List<CollectedColumn> contactGroupInfoColumns =
    [
        new CollectedColumn("ContactGroupGUID", "GUID"),
        new CollectedColumn("ContactGroupName", "Contact group name"),
        new CollectedColumn("ContactGroupDescription", "Contact group description")
    ];

    private readonly IPersonalDataWriter personalDataWriter;
    private readonly IFormCollectionService formCollectionService;
    private readonly Dictionary<Guid, FormDefinition> forms;

    public ContactDataCollectorCore(IPersonalDataWriter personalDataWriter, IFormCollectionService formCollectionService)
    {
        this.personalDataWriter = personalDataWriter;
        this.formCollectionService = formCollectionService;

        forms = this.formCollectionService.GetForms();
    }

    public string CollectData(IEnumerable<BaseInfo> identities)
    {
        var contacts = identities.OfType<ContactInfo>().ToList();
        if (!contacts.Any())
        {
            return null;
        }

        var contactIDs = contacts.Select(c => c.ContactID).ToList();
        var contactEmails = contacts.Select(c => c.ContactEmail).ToList();

        var contactActivities = ActivityInfo.Provider.Get()
            .Columns(activityInfoColumns.Select(t => t.Name))
            .WhereIn("ActivityContactID", contactIDs).ToList();

        var contactContactGroups = contacts
            .SelectMany(c => c.ContactGroups)
            .GroupBy(c => c.ContactGroupID)
            .Select(group => group.First());

        personalDataWriter.WriteStartSection("OnlineMarketingData", "Online marketing data");

        string before = personalDataWriter.GetResult();

        WriteContacts(contacts);
        WriteConsents(contactIDs);
        WriteContactActivities(contactActivities);
        WriteContactAccounts(contactIDs);
        WriteContactGroups(contactContactGroups);
        WriteSubmittedFormsData(contactEmails, contactIDs);

        string after = personalDataWriter.GetResult();
        //return nothing if the previous methods did not find any data to write
        if (string.IsNullOrWhiteSpace(after) || after.Equals(before))
        {
            return null;
        }

        personalDataWriter.WriteEndSection();

        return personalDataWriter.GetResult();
    }

    private object TransformGenderValue(string columnName, object columnValue)
    {
        if (columnName.Equals("ContactGender", StringComparison.InvariantCultureIgnoreCase))
        {
            int? gender = columnValue as int?;
            return gender switch
            {
                1 => "male",
                2 => "female",
                _ => "undefined",
            };
        }

        return columnValue;
    }

    private object TransformConsentText(string columnName, object columnValue)
    {
        if (columnName.Equals("ConsentContent", StringComparison.InvariantCultureIgnoreCase) ||
            columnName.Equals("ConsentArchiveContent", StringComparison.InvariantCultureIgnoreCase))
        {
            var consentXml = new XmlDocument();
            consentXml.LoadXml((columnValue as string) ?? string.Empty);

            var xmlNode =
                consentXml.SelectSingleNode("/ConsentContent/ConsentLanguageVersions/ConsentLanguageVersion/FullText");

            string result = HTMLHelper.StripTags(xmlNode?.InnerText);

            return result;
        }

        return columnValue;
    }

    private object TransformConsentAction(string columnName, object columnValue)
    {
        if (columnName.Equals("ConsentAgreementRevoked", StringComparison.InvariantCultureIgnoreCase))
        {
            bool revoked = (bool)columnValue;

            return revoked ? "Revoked" : "Agreed";
        }

        return columnValue;
    }

    private void WriteContacts(IEnumerable<ContactInfo> contacts)
    {
        foreach (var contactInfo in contacts.Where(contact => contact.ContactID > 0))
        {
            personalDataWriter.WriteStartSection(ContactInfo.OBJECT_TYPE, "Contact");
            personalDataWriter.WriteBaseInfo(contactInfo, contactInfoColumns, TransformGenderValue);

            int countryId = contactInfo.ContactCountryID;
            int stateId = contactInfo.ContactStateID;

            if (countryId != 0)
            {
                personalDataWriter.WriteBaseInfo(CountryInfo.Provider.Get(countryId), countryInfoColumns);
            }

            if (stateId != 0)
            {
                personalDataWriter.WriteBaseInfo(StateInfo.Provider.Get(stateId), stateInfoColumns);
            }

            personalDataWriter.WriteEndSection();
        }
    }

    private void WriteConsents(ICollection<int> contactIDs)
    {
        DataSet consentsData = ConsentAgreementInfo.Provider.Get()
            .Source(s => s.Join<ConsentInfo>("CMS_ConsentAgreement.ConsentAgreementConsentID", "ConsentID"))
            .Source(s =>
                s.LeftJoin<ConsentArchiveInfo>("CMS_ConsentAgreement.ConsentAgreementConsentHash",
                    "ConsentArchiveHash"))
            .WhereIn("ConsentAgreementContactID", contactIDs)
            .OrderBy("ConsentID")
            .OrderByDescending("ConsentAgreementTime")
            .Result;

        if (DataHelper.DataSourceIsEmpty(consentsData))
        {
            return;
        }
        var consents = new Dictionary<int, ConsentInfo>();
        var consentRevocations = new Dictionary<int, List<ConsentAgreementInfo>>();

        var consentContentArchives = new Dictionary<string, ConsentArchiveInfo>(StringComparer.OrdinalIgnoreCase);
        var consentContentAgreements =
            new Dictionary<string, List<ConsentAgreementInfo>>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in consentsData.Tables[0].AsEnumerable())
        {
            var consentAgreementInfo = new ConsentAgreementInfo(row);

            if (!consents.TryGetValue(consentAgreementInfo.ConsentAgreementConsentID, out var consentInfo))
            {
                consentInfo = new ConsentInfo(row);
                consents.Add(consentAgreementInfo.ConsentAgreementConsentID, consentInfo);
            }

            if (consentAgreementInfo.ConsentAgreementRevoked)
            {
                var revocationsOfSameConsent = GetRevocationsOfSameConsent(consentRevocations,
                    consentAgreementInfo.ConsentAgreementConsentID);
                revocationsOfSameConsent.Add(consentAgreementInfo);
            }
            else
            {
                var agreementsOfSameConsentContent = GetAgreementsOfSameConsentContent(consentContentAgreements,
                    consentAgreementInfo.ConsentAgreementConsentHash);
                agreementsOfSameConsentContent.Add(consentAgreementInfo);

                if (IsAgreementOfDifferentConsentContent(consentAgreementInfo, consentInfo) &&
                    !consentContentArchives.ContainsKey(consentAgreementInfo.ConsentAgreementConsentHash))
                {
                    consentContentArchives.Add(consentAgreementInfo.ConsentAgreementConsentHash,
                        new ConsentArchiveInfo(row));
                }
            }
        }

        WriteConsents(consents, consentContentArchives, consentContentAgreements, consentRevocations);
    }

    private static List<ConsentAgreementInfo> GetRevocationsOfSameConsent(
        Dictionary<int, List<ConsentAgreementInfo>> consentRevocations, int consentId)
    {
        if (!consentRevocations.TryGetValue(consentId, out var revocationsOfSameConsent))
        {
            revocationsOfSameConsent = [];
            consentRevocations.Add(consentId, revocationsOfSameConsent);
        }

        return revocationsOfSameConsent;
    }

    private static List<ConsentAgreementInfo> GetAgreementsOfSameConsentContent(
        Dictionary<string, List<ConsentAgreementInfo>> consentContentAgreements, string consentHash)
    {
        if (!consentContentAgreements.TryGetValue(consentHash, out var agreementsOfSameConsent))
        {
            agreementsOfSameConsent = [];
            consentContentAgreements.Add(consentHash, agreementsOfSameConsent);
        }

        return agreementsOfSameConsent;
    }

    private static bool IsAgreementOfDifferentConsentContent(ConsentAgreementInfo consentAgreementInfo,
        ConsentInfo consentInfo) => consentAgreementInfo.ConsentAgreementConsentHash != consentInfo.ConsentHash;

    private void WriteConsents(Dictionary<int, ConsentInfo> consents,
        Dictionary<string, ConsentArchiveInfo> consentContentArchives,
        Dictionary<string, List<ConsentAgreementInfo>> consentContentAgreements,
        Dictionary<int, List<ConsentAgreementInfo>> consentRevocations)
    {
        foreach (var agreementsOfSameConsentContent in consentContentAgreements.Values)
        {
            var consentAgreement = agreementsOfSameConsentContent.First();
            var consentInfo = consents[consentAgreement.ConsentAgreementConsentID];

            consentContentArchives.TryGetValue(consentAgreement.ConsentAgreementConsentHash, out var consentArchiveInfo);
            consentRevocations.TryGetValue(consentAgreement.ConsentAgreementConsentID, out var revocationsOfSameConsent);

            WriteConsent(consentInfo, consentArchiveInfo, agreementsOfSameConsentContent, revocationsOfSameConsent);
        }
    }

    private void WriteConsent(ConsentInfo consentInfo, ConsentArchiveInfo consentArchiveInfo,
        IEnumerable<ConsentAgreementInfo> consentAgreements,
        IEnumerable<ConsentAgreementInfo> consentRevocations)
    {
        personalDataWriter.WriteStartSection(ConsentInfo.OBJECT_TYPE, "Consent");

        var agreedConsentLastModified =
            consentArchiveInfo?.ConsentArchiveLastModified ?? consentInfo.ConsentLastModified;

        WriteConsentContent(consentInfo, consentArchiveInfo);
        WriteConsentAgreements(consentAgreements);
        WriteConsentRevocations(consentRevocations?.Where(cr => cr.ConsentAgreementTime > agreedConsentLastModified));

        personalDataWriter.WriteEndSection();
    }

    private void WriteConsentContent(ConsentInfo consentInfo, ConsentArchiveInfo consentArchiveInfo)
    {
        if (consentArchiveInfo == null)
        {
            personalDataWriter.WriteBaseInfo(consentInfo, consentInfoColumns, TransformConsentText);
        }
        else
        {
            personalDataWriter.WriteSectionValue("ConsentDisplayName", "Consent name", consentInfo.ConsentDisplayName);
            personalDataWriter.WriteBaseInfo(consentArchiveInfo, consentArchiveInfoColumns, TransformConsentText);
        }
    }

    private void WriteConsentAgreements(IEnumerable<ConsentAgreementInfo> consentAgreements)
    {
        foreach (var consentAgreement in consentAgreements)
        {
            personalDataWriter.WriteBaseInfo(consentAgreement, consentAgreementInfoColumns, TransformConsentAction);
        }
    }

    private void WriteConsentRevocations(IEnumerable<ConsentAgreementInfo> consentRevocations)
    {
        if (consentRevocations == null)
        {
            return;
        }

        foreach (var consentRevocation in consentRevocations)
        {
            personalDataWriter.WriteBaseInfo(consentRevocation, consentAgreementInfoColumns, TransformConsentAction);
        }
    }

    private void WriteContactActivities(IEnumerable<ActivityInfo> contactActivities)
    {
        foreach (var contactActivityInfo in contactActivities)
        {
            personalDataWriter.WriteStartSection(ActivityInfo.OBJECT_TYPE, "Activity");
            personalDataWriter.WriteBaseInfo(contactActivityInfo, activityInfoColumns);
            personalDataWriter.WriteEndSection();
        }
    }

    private void WriteContactAccounts(ICollection<int> contactIDs)
    {
        var accountIDs = AccountContactInfo.Provider.Get()
            .WhereIn("ContactID", contactIDs)
            .Column("AccountID")
            .Distinct();
        var accountInfos = AccountInfo.Provider.Get()
            .Columns(accountInfoColumns.Select(t => t.Name))
            .WhereIn("AccountID", accountIDs)
            .ToList();

        var countryInfos = CountryInfo.Provider.Get()
            .WhereIn("CountryID", accountInfos.Select(r => r.AccountCountryID).ToList())
            .ToDictionary(ci => ci.CountryID);
        var stateInfos = StateInfo.Provider.Get()
            .WhereIn("StateID", accountInfos.Select(r => r.AccountStateID).ToList())
            .ToDictionary(si => si.StateID);

        foreach (var accountInfo in accountInfos)
        {
            countryInfos.TryGetValue(accountInfo.AccountCountryID, out var countryInfo);
            stateInfos.TryGetValue(accountInfo.AccountStateID, out var stateInfo);

            personalDataWriter.WriteStartSection(AccountInfo.OBJECT_TYPE, "Account");

            personalDataWriter.WriteBaseInfo(accountInfo, accountInfoColumns);

            if (countryInfo != null)
            {
                personalDataWriter.WriteBaseInfo(countryInfo, countryInfoColumns);
            }

            if (stateInfo != null)
            {
                personalDataWriter.WriteBaseInfo(stateInfo, stateInfoColumns);
            }

            personalDataWriter.WriteEndSection();
        }
    }

    private void WriteContactGroups(IEnumerable<ContactGroupInfo> contactContactGroups)
    {
        foreach (var contactGroupInfo in contactContactGroups)
        {
            personalDataWriter.WriteStartSection(ContactGroupInfo.OBJECT_TYPE, "Contact group");
            personalDataWriter.WriteBaseInfo(contactGroupInfo, contactGroupInfoColumns);
            personalDataWriter.WriteEndSection();
        }
    }

    private void WriteSubmittedFormsData(ICollection<string> emails, ICollection<int> contactIDs)
    {
        var consentAgreementGuids = ConsentAgreementInfo.Provider.Get()
            .Columns("ConsentAgreementGuid")
            .WhereIn("ConsentAgreementContactID", contactIDs);

        var formClasses = BizFormInfo.Provider.Get()
            .Source(s => s.InnerJoin<DataClassInfo>("CMS_Form.FormClassID", "ClassID"))
            .WhereIn("FormGUID", forms.Keys);

        formClasses.ForEachRow(row =>
        {
            var bizForm = new BizFormInfo(row);
            var formDefinition = forms[bizForm.FormGUID];

            var bizFormItems = formCollectionService.GetBizFormItems(emails, consentAgreementGuids, row, formDefinition);

            foreach (var bizFormItem in bizFormItems)
            {
                personalDataWriter.WriteStartSection("SubmittedForm", "Submitted form");

                personalDataWriter.WriteSectionValue("FormDisplayName", "Form display name", bizForm.FormDisplayName);
                personalDataWriter.WriteSectionValue("FormGUID", "Form GUID", bizForm.FormGUID.ToString());
                personalDataWriter.WriteBaseInfo(bizFormItem, formDefinition.FormColumns);

                personalDataWriter.WriteEndSection();
            }
        });
    }
}
