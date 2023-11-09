using CMS.Activities;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.Globalization;
using CMS.Helpers;
using CMS.OnlineForms;
using KBank.Web.DataProtection.Writers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

namespace KBank.Web.DataProtection;

public class DataCollectorCore
{
    private readonly IPersonalDataWriter _writer;

    private readonly List<CollectedColumn> _contactInfoColumns = new()
    {
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
    };

    private readonly List<CollectedColumn> _consentAgreementInfoColumns = new()
    {
        new CollectedColumn("ConsentAgreementGuid", "GUID"),
        new CollectedColumn("ConsentAgreementRevoked", "Consent action"),
        new CollectedColumn("ConsentAgreementTime", "Performed on")
    };

    private readonly List<CollectedColumn> _consentInfoColumns = new()
    {
        new CollectedColumn("ConsentGUID", "GUID"),
        new CollectedColumn("ConsentDisplayName", "Consent name"),
        new CollectedColumn("ConsentContent", "Full text")
    };

    private readonly List<CollectedColumn> _consentArchiveInfoColumns = new()
    {
        new CollectedColumn("ConsentArchiveGUID", "GUID"),
        new CollectedColumn("ConsentArchiveContent", "Full text")
    };

    private readonly List<CollectedColumn> _activityInfoColumns = new()
    {
        new CollectedColumn("ActivityId", "ID"),
        new CollectedColumn("ActivityCreated", "Created"),
        new CollectedColumn("ActivityType", "Type"),
        new CollectedColumn("ActivityUrl", "URL"),
        new CollectedColumn("ActivityTitle", "Title"),
        new CollectedColumn("ActivityItemId", "")
    };

    private readonly List<CollectedColumn> _accountInfoColumns = new()
    {
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
    };

    private readonly List<CollectedColumn> _countryInfoColumns = new()
    {
        new CollectedColumn("CountryDisplayName", "Country")
    };

    private readonly List<CollectedColumn> _stateInfoColumns = new()
    {
        new CollectedColumn("StateDisplayName", "State")
    };

    private readonly List<CollectedColumn> _contactGroupInfoColumns = new()
    {
        new CollectedColumn("ContactGroupGUID", "GUID"),
        new CollectedColumn("ContactGroupName", "Contact group name"),
        new CollectedColumn("ContactGroupDescription", "Contact group description")
    };

    private readonly Dictionary<Guid, FormDefinition> _siteForms = FormCollectionHelper.GetSiteForms();

    private static object TransformGenderValue(string columnName, object columnValue)
    {
        if (columnName.Equals("ContactGender", StringComparison.InvariantCultureIgnoreCase))
        {
            var gender = columnValue as int?;
            switch (gender)
            {
                case 1:
                    return "male";
                case 2:
                    return "female";
                case 0:
                default:
                    return "undefined";
            }
        }

        return columnValue;
    }

    private static object TransformConsentText(string columnName, object columnValue)
    {
        if (columnName.Equals("ConsentContent", StringComparison.InvariantCultureIgnoreCase) ||
            columnName.Equals("ConsentArchiveContent", StringComparison.InvariantCultureIgnoreCase))
        {
            var consentXml = new XmlDocument();
            consentXml.LoadXml((columnValue as string) ?? String.Empty);

            var xmlNode =
                consentXml.SelectSingleNode("/ConsentContent/ConsentLanguageVersions/ConsentLanguageVersion/FullText");

            var result = HTMLHelper.StripTags(xmlNode?.InnerText);

            return result;
        }

        return columnValue;
    }

    private static object TransformConsentAction(string columnName, object columnValue)
    {
        if (columnName.Equals("ConsentAgreementRevoked", StringComparison.InvariantCultureIgnoreCase))
        {
            var revoked = (bool)columnValue;

            return revoked ? "Revoked" : "Agreed";
        }

        return columnValue;
    }

    public DataCollectorCore(IPersonalDataWriter writer)
    {
        _writer = writer;
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
            .Columns(_activityInfoColumns.Select(t => t.Name))
            .WhereIn("ActivityContactID", contactIDs).ToList();

        var contactContactGroups = contacts
            .SelectMany(c => c.ContactGroups)
            .GroupBy(c => c.ContactGroupID)
            .Select(group => group.First());

        _writer.WriteStartSection("OnlineMarketingData", "Online marketing data");

        string before = _writer.GetResult();

        WriteContacts(contacts);
        WriteConsents(contactIDs);
        WriteContactActivities(contactActivities);
        WriteContactAccounts(contactIDs);
        WriteContactGroups(contactContactGroups);
        WriteSubmittedFormsData(contactEmails, contactIDs);

        string after = _writer.GetResult();
        //return nothing if the previous methods did not find any data to write
        if (string.IsNullOrWhiteSpace(after) || after.Equals(before))
        {
            return null;
        }

        _writer.WriteEndSection();

        return _writer.GetResult();
    }

    private void WriteContacts(IEnumerable<ContactInfo> contacts)
    {
        foreach (var contactInfo in contacts.Where(contact => contact.ContactID > 0))
        {
            _writer.WriteStartSection(ContactInfo.OBJECT_TYPE, "Contact");
            _writer.WriteBaseInfo(contactInfo, _contactInfoColumns, TransformGenderValue);

            var countryId = contactInfo.ContactCountryID;
            var stateId = contactInfo.ContactStateID;

            if (countryId != 0)
            {
                _writer.WriteBaseInfo(CountryInfo.Provider.Get(countryId), _countryInfoColumns);
            }

            if (stateId != 0)
            {
                _writer.WriteBaseInfo(StateInfo.Provider.Get(stateId), _stateInfoColumns);
            }

            _writer.WriteEndSection();
        }
    }

    private void WriteConsents(ICollection<int> contactIDs)
    {
        var consentsData = ConsentAgreementInfo.Provider.Get()
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
            revocationsOfSameConsent = new List<ConsentAgreementInfo>();
            consentRevocations.Add(consentId, revocationsOfSameConsent);
        }

        return revocationsOfSameConsent;
    }

    private static List<ConsentAgreementInfo> GetAgreementsOfSameConsentContent(
        Dictionary<string, List<ConsentAgreementInfo>> consentContentAgreements, string consentHash)
    {
        if (!consentContentAgreements.TryGetValue(consentHash, out var agreementsOfSameConsent))
        {
            agreementsOfSameConsent = new List<ConsentAgreementInfo>();
            consentContentAgreements.Add(consentHash, agreementsOfSameConsent);
        }

        return agreementsOfSameConsent;
    }

    private static bool IsAgreementOfDifferentConsentContent(ConsentAgreementInfo consentAgreementInfo,
        ConsentInfo consentInfo)
    {
        return consentAgreementInfo.ConsentAgreementConsentHash != consentInfo.ConsentHash;
    }

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
        _writer.WriteStartSection(ConsentInfo.OBJECT_TYPE, "Consent");

        var agreedConsentLastModified =
            consentArchiveInfo?.ConsentArchiveLastModified ?? consentInfo.ConsentLastModified;

        WriteConsentContent(consentInfo, consentArchiveInfo);
        WriteConsentAgreements(consentAgreements);
        WriteConsentRevocations(consentRevocations?.Where(cr => cr.ConsentAgreementTime > agreedConsentLastModified));

        _writer.WriteEndSection();
    }

    private void WriteConsentContent(ConsentInfo consentInfo, ConsentArchiveInfo consentArchiveInfo)
    {
        if (consentArchiveInfo == null)
        {
            _writer.WriteBaseInfo(consentInfo, _consentInfoColumns, TransformConsentText);
        }
        else
        {
            _writer.WriteSectionValue("ConsentDisplayName", "Consent name", consentInfo.ConsentDisplayName);
            _writer.WriteBaseInfo(consentArchiveInfo, _consentArchiveInfoColumns, TransformConsentText);
        }
    }

    private void WriteConsentAgreements(IEnumerable<ConsentAgreementInfo> consentAgreements)
    {
        foreach (var consentAgreement in consentAgreements)
        {
            _writer.WriteBaseInfo(consentAgreement, _consentAgreementInfoColumns, TransformConsentAction);
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
            _writer.WriteBaseInfo(consentRevocation, _consentAgreementInfoColumns, TransformConsentAction);
        }
    }

    private void WriteContactActivities(IEnumerable<ActivityInfo> contactActivities)
    {
        foreach (var contactActivityInfo in contactActivities)
        {
            _writer.WriteStartSection(ActivityInfo.OBJECT_TYPE, "Activity");
            _writer.WriteBaseInfo(contactActivityInfo, _activityInfoColumns);
            _writer.WriteEndSection();
        }
    }

    private void WriteContactAccounts(ICollection<int> contactIDs)
    {
        var accountIDs = AccountContactInfo.Provider.Get()
            .WhereIn("ContactID", contactIDs)
            .Column("AccountID")
            .Distinct();
        var accountInfos = AccountInfo.Provider.Get()
            .Columns(_accountInfoColumns.Select(t => t.Name))
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

            _writer.WriteStartSection(AccountInfo.OBJECT_TYPE, "Account");

            _writer.WriteBaseInfo(accountInfo, _accountInfoColumns);

            if (countryInfo != null)
            {
                _writer.WriteBaseInfo(countryInfo, _countryInfoColumns);
            }

            if (stateInfo != null)
            {
                _writer.WriteBaseInfo(stateInfo, _stateInfoColumns);
            }

            _writer.WriteEndSection();
        }
    }

    private void WriteContactGroups(IEnumerable<ContactGroupInfo> contactContactGroups)
    {
        foreach (var contactGroupInfo in contactContactGroups)
        {
            _writer.WriteStartSection(ContactGroupInfo.OBJECT_TYPE, "Contact group");
            _writer.WriteBaseInfo(contactGroupInfo, _contactGroupInfoColumns);
            _writer.WriteEndSection();
        }
    }

    private void WriteSubmittedFormsData(ICollection<string> emails, ICollection<int> contactIDs)
    {
        var consentAgreementGuids = ConsentAgreementInfo.Provider.Get()
            .Columns("ConsentAgreementGuid")
            .WhereIn("ConsentAgreementContactID", contactIDs);

        var formClasses = BizFormInfo.Provider.Get()
            .Source(s => s.InnerJoin<DataClassInfo>("CMS_Form.FormClassID", "ClassID"))
            .WhereIn("FormGUID", _siteForms.Keys);

        formClasses.ForEachRow(row =>
        {
            var bizForm = new BizFormInfo(row);
            var formDefinition = _siteForms[bizForm.FormGUID];

            var bizFormItems = FormCollectionHelper.GetBizFormItems(emails, consentAgreementGuids, row, formDefinition);

            foreach (var bizFormItem in bizFormItems)
            {
                _writer.WriteStartSection("SubmittedForm", "Submitted form");

                _writer.WriteSectionValue("FormDisplayName", "Form display name", bizForm.FormDisplayName);
                _writer.WriteSectionValue("FormGUID", "Form GUID", bizForm.FormGUID.ToString());
                _writer.WriteBaseInfo(bizFormItem, formDefinition.FormColumns);

                _writer.WriteEndSection();
            }
        });
    }
}