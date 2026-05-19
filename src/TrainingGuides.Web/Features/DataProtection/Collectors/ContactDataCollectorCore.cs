using System.Data;
using System.Xml;
using CMS.Activities;
using CMS.Commerce;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineForms;
using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Web.Features.DataProtection.Writers;

namespace TrainingGuides.Web.Features.DataProtection.Collectors;

public class DataCollectorCore
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

    private readonly List<CollectedColumn> memberInfoColumns =
    [
        new CollectedColumn("MemberEmail", "Email"),
        new CollectedColumn("MemberEnabled", "Enabled"),
        new CollectedColumn("MemberCreated", "Created"),
        new CollectedColumn("MemberGuid", "GUID"),
        new CollectedColumn("MemberName", "Name"),
        new CollectedColumn("MemberIsExternal", "Is external"),
        new CollectedColumn("GuidesMemberGivenName", "Given name"),
        new CollectedColumn("GuidesMemberFamilyName", "Family name"),
        new CollectedColumn("GuidesMemberFamilyNameFirst", "Family name first"),
        new CollectedColumn("GuidesMemberFavoriteCoffee", "Favorite coffee")
    ];

    private readonly List<CollectedColumn> memberRoleInfoColumns =
    [
        new CollectedColumn("MemberRoleGUID", "GUID"),
        new CollectedColumn("MemberRoleDisplayName", "Name"),
        new CollectedColumn("MemberRoleDescription", "Description"),
        new CollectedColumn("GuidesRoleBenefitsSummary", "Benefits")
    ];

    private readonly List<CollectedColumn> customerInfoColumns =
    [
        new CollectedColumn("CustomerGUID", "GUID"),
        new CollectedColumn("CustomerCreatedWhen", "Created"),
        new CollectedColumn("CustomerFirstName", "First name"),
        new CollectedColumn("CustomerLastName", "Last name"),
        new CollectedColumn("CustomerEmail", "Email"),
        new CollectedColumn("CustomerPhone", "Phone")
    ];

    private readonly List<CollectedColumn> orderInfoColumns =
    [
        new CollectedColumn("OrderGUID", "GUID"),
        new CollectedColumn("OrderNumber", "Order number"),
        new CollectedColumn("OrderCreatedWhen", "Created"),
        new CollectedColumn("OrderModifiedWhen", "Last modified"),
        new CollectedColumn("OrderTotalPrice", "Total price"),
        new CollectedColumn("OrderTotalShipping", "Total shipping"),
        new CollectedColumn("OrderTotalTax", "Total tax"),
        new CollectedColumn("OrderGrandTotal", "Grand total"),
        new CollectedColumn("OrderPaymentMethodDisplayName", "Payment method"),
        new CollectedColumn("OrderShippingMethodDisplayName", "Shipping method"),
        new CollectedColumn("OrderShippingMethodPrice", "Shipping method price")
    ];

    private readonly List<CollectedColumn> orderItemInfoColumns =
    [
        new CollectedColumn("OrderItemGUID", "GUID"),
        new CollectedColumn("OrderItemSKU", "SKU"),
        new CollectedColumn("OrderItemName", "Name"),
        new CollectedColumn("OrderItemQuantity", "Quantity"),
        new CollectedColumn("OrderItemUnitPrice", "Unit price"),
        new CollectedColumn("OrderItemTotalPrice", "Total price"),
        new CollectedColumn("OrderItemTotalTax", "Total tax"),
        new CollectedColumn("OrderItemTaxRate", "Tax rate")
    ];

    private readonly List<CollectedColumn> customerAddressInfoColumns =
    [
        new CollectedColumn("CustomerAddressGUID", "GUID"),

        new CollectedColumn("CustomerAddressFirstName", "First name"),
        new CollectedColumn("CustomerAddressLastName", "Last name"),
        new CollectedColumn("CustomerAddressCompany", "Company"),
        new CollectedColumn("CustomerAddressEmail", "Email"),
        new CollectedColumn("CustomerAddressPhone", "Phone"),
        new CollectedColumn("CustomerAddressLine1", "Address line 1"),
        new CollectedColumn("CustomerAddressLine2", "Address line 2"),
        new CollectedColumn("CustomerAddressCity", "City"),
        new CollectedColumn("CustomerAddressZip", "ZIP"),
        new CollectedColumn("CustomerAddressCountryID", ""),
        new CollectedColumn("CustomerAddressStateID", "")
    ];

    private readonly List<CollectedColumn> orderAddressInfoColumns =
    [
        new CollectedColumn("OrderAddressGUID", "GUID"),
        new CollectedColumn("OrderAddressType", "Address type"),
        new CollectedColumn("OrderAddressFirstName", "First name"),
        new CollectedColumn("OrderAddressLastName", "Last name"),
        new CollectedColumn("OrderAddressCompany", "Company"),
        new CollectedColumn("OrderAddressEmail", "Email"),
        new CollectedColumn("OrderAddressPhone", "Phone"),
        new CollectedColumn("OrderAddressLine1", "Address line 1"),
        new CollectedColumn("OrderAddressLine2", "Address line 2"),
        new CollectedColumn("OrderAddressCity", "City"),
        new CollectedColumn("OrderAddressZip", "ZIP"),
        new CollectedColumn("OrderAddressCountryID", ""),
        new CollectedColumn("OrderAddressStateID", "")
    ];

    private readonly IPersonalDataWriter personalDataWriter;
    private readonly IFormCollectionService formCollectionService;
    private readonly Dictionary<Guid, FormDefinition> forms;

    private readonly IInfoProvider<ConsentAgreementInfo> consentAgreementInfoProvider;
    private readonly IInfoProvider<ActivityInfo> activityInfoProvider;
    private readonly IInfoProvider<BizFormInfo> bizFormInfoProvider;
    private readonly IInfoProvider<CountryInfo> countryInfoProvider;
    private readonly IInfoProvider<StateInfo> stateInfoProvider;
    private readonly IInfoProvider<OrderInfo> orderInfoProvider;
    private readonly IInfoProvider<OrderItemInfo> orderItemInfoProvider;
    private readonly IInfoProvider<MemberRoleInfo> memberRoleInfoProvider;
    private readonly IInfoProvider<MemberRoleMemberInfo> memberRoleMemberInfoProvider;
    private readonly IInfoProvider<CustomerAddressInfo> customerAddressInfoProvider;
    private readonly IInfoProvider<OrderAddressInfo> orderAddressInfoProvider;

    public DataCollectorCore(IPersonalDataWriter personalDataWriter,
        IFormCollectionService formCollectionService,
        IInfoProvider<ConsentAgreementInfo> consentAgreementInfoProvider,
        IInfoProvider<ActivityInfo> activityInfoProvider,
        IInfoProvider<BizFormInfo> bizFormInfoProvider,
        IInfoProvider<CountryInfo> countryInfoProvider,
        IInfoProvider<StateInfo> stateInfoProvider,
        IInfoProvider<OrderInfo> orderInfoProvider,
        IInfoProvider<OrderItemInfo> orderItemInfoProvider,
        IInfoProvider<MemberRoleInfo> memberRoleInfoProvider,
        IInfoProvider<MemberRoleMemberInfo> memberRoleMemberInfoProvider,
        IInfoProvider<CustomerAddressInfo> customerAddressInfoProvider,
        IInfoProvider<OrderAddressInfo> orderAddressInfoProvider)
    {
        this.personalDataWriter = personalDataWriter;
        this.formCollectionService = formCollectionService;
        this.consentAgreementInfoProvider = consentAgreementInfoProvider;
        this.activityInfoProvider = activityInfoProvider;
        this.bizFormInfoProvider = bizFormInfoProvider;
        this.countryInfoProvider = countryInfoProvider;
        this.stateInfoProvider = stateInfoProvider;
        this.orderInfoProvider = orderInfoProvider;
        this.orderItemInfoProvider = orderItemInfoProvider;
        this.memberRoleInfoProvider = memberRoleInfoProvider;
        this.memberRoleMemberInfoProvider = memberRoleMemberInfoProvider;
        this.customerAddressInfoProvider = customerAddressInfoProvider;
        this.orderAddressInfoProvider = orderAddressInfoProvider;
        forms = this.formCollectionService.GetForms();
    }

    public string? CollectData(IEnumerable<BaseInfo> identities)
    {
        var contacts = identities.OfType<ContactInfo>().ToList();
        var members = identities.OfType<MemberInfo>().ToList();
        var customers = identities.OfType<CustomerInfo>().ToList();

        if (!(contacts.Any() || members.Any() || customers.Any()))
        {
            return null;
        }

        personalDataWriter.WriteStartSection("AllData", "All visitor data");

        string before = personalDataWriter.GetResult();

        if (contacts.Any(c => c.ContactID > 0))
        {
            var contactIDs = contacts.Select(c => c.ContactID).ToList();
            var contactEmails = contacts.Select(c => c.ContactEmail).ToList();

            var contactActivities = activityInfoProvider.Get()
                .Columns(activityInfoColumns.Select(t => t.Name))
                .WhereIn("ActivityContactID", contactIDs).ToList();

            // Includes plain contact groups and recipient lists
            var contactContactGroups = contacts
                .SelectMany(c => c.ContactGroups)
                .GroupBy(c => c.ContactGroupID)
                .Select(group => group.First());

            personalDataWriter.WriteStartSection("OnlineMarketingData", "Online marketing data");
            WriteContacts(contacts);
            WriteConsents(contactIDs);
            WriteContactActivities(contactActivities);
            WriteContactGroups(contactContactGroups);
            WriteSubmittedFormsData(contactEmails, contactIDs);
            personalDataWriter.WriteEndSection();
        }
        if (members.Any())
        {
            var memberIds = members.Select(m => m.MemberID).ToList();

            personalDataWriter.WriteStartSection("MembershipData", "Membership data");
            WriteMembers(members);
            WriteMemberRoles(members);
            personalDataWriter.WriteEndSection();
        }
        if (customers.Any())
        {
            var customerIds = customers.Select(c => c.CustomerID).ToList();
            var customerEmails = customers.Select(c => c.CustomerEmail).ToList();

            personalDataWriter.WriteStartSection("CommerceData", "Commerce data");
            WriteCustomers(customers);
            WriteCustomerOrders(customerIds);
            WriteThirdPartyCustomerAddresses(customerEmails, customerIds);
            WriteThirdPartyOrderAddresses(customerEmails, customerIds);
            personalDataWriter.WriteEndSection();
        }

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

            WriteCountryAndOrState(contactInfo.ContactCountryID, contactInfo.ContactStateID);

            personalDataWriter.WriteEndSection();
        }
    }

    private void WriteCountryAndOrState(int countryId, int stateId)
    {
        if (countryId != 0)
        {
            personalDataWriter.WriteBaseInfo(countryInfoProvider.Get(countryId), countryInfoColumns);
        }

        if (stateId != 0)
        {
            personalDataWriter.WriteBaseInfo(stateInfoProvider.Get(stateId), stateInfoColumns);
        }
    }

    private void WriteConsents(ICollection<int> contactIDs)
    {
        var consentsDataSet = consentAgreementInfoProvider.Get()
            .Source(s => s.Join<ConsentInfo>("CMS_ConsentAgreement.ConsentAgreementConsentID", "ConsentID"))
            .Source(s =>
                s.LeftJoin<ConsentArchiveInfo>("CMS_ConsentAgreement.ConsentAgreementConsentHash",
                    "ConsentArchiveHash"))
            .WhereIn("ConsentAgreementContactID", contactIDs)
            .OrderBy("ConsentID")
            .OrderByDescending("ConsentAgreementTime")
            .Result;

        if (DataHelper.DataSourceIsEmpty(consentsDataSet))
        {
            return;
        }
        var consents = new Dictionary<int, ConsentInfo>();
        var consentRevocations = new Dictionary<int, List<ConsentAgreementInfo>>();

        var consentContentArchives = new Dictionary<string, ConsentArchiveInfo>(StringComparer.OrdinalIgnoreCase);
        var consentContentAgreements =
            new Dictionary<string, List<ConsentAgreementInfo>>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in consentsDataSet.Tables[0].AsEnumerable())
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

    private void WriteConsent(ConsentInfo consentInfo, ConsentArchiveInfo? consentArchiveInfo,
        IEnumerable<ConsentAgreementInfo> consentAgreements,
        IEnumerable<ConsentAgreementInfo>? consentRevocations)
    {
        personalDataWriter.WriteStartSection(ConsentInfo.OBJECT_TYPE, "Consent");

        var agreedConsentLastModified =
            consentArchiveInfo?.ConsentArchiveLastModified ?? consentInfo.ConsentLastModified;

        WriteConsentContent(consentInfo, consentArchiveInfo);
        WriteConsentAgreements(consentAgreements);
        WriteConsentRevocations(consentRevocations?.Where(cr => cr.ConsentAgreementTime > agreedConsentLastModified));

        personalDataWriter.WriteEndSection();
    }

    private void WriteConsentContent(ConsentInfo consentInfo, ConsentArchiveInfo? consentArchiveInfo)
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

    private void WriteConsentRevocations(IEnumerable<ConsentAgreementInfo>? consentRevocations)
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
        var consentAgreementGuids = consentAgreementInfoProvider.Get()
            .Columns("ConsentAgreementGuid")
            .WhereIn("ConsentAgreementContactID", contactIDs);

        var formClasses = bizFormInfoProvider.Get()
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

    private void WriteMembers(IEnumerable<MemberInfo> members)
    {
        foreach (var memberInfo in members.Where(member => member.MemberID > 0))
        {
            personalDataWriter.WriteStartSection(MemberInfo.OBJECT_TYPE, "Member");
            personalDataWriter.WriteBaseInfo(memberInfo, memberInfoColumns);
            personalDataWriter.WriteEndSection();
        }
    }

    private void WriteMemberRoles(IEnumerable<MemberInfo> members)
    {
        foreach (var memberInfo in members.Where(member => member.MemberID > 0))
        {
            // Nested query, do not enumerate to prevent multiple database calls
            var memberRoleBindings = memberRoleMemberInfoProvider.Get()
                .WhereEquals(nameof(MemberRoleMemberInfo.MemberRoleMemberMemberID), memberInfo.MemberID)
                .Column(nameof(MemberRoleMemberInfo.MemberRoleMemberMemberRoleID))
                .Distinct();

            var memberRoles = memberRoleInfoProvider.Get()
                .WhereIn(nameof(MemberRoleInfo.MemberRoleID), memberRoleBindings);

            foreach (var memberRoleInfo in memberRoles)
            {
                personalDataWriter.WriteStartSection("MemberRole", "Member role");
                personalDataWriter.WriteBaseInfo(memberRoleInfo, memberRoleInfoColumns);
                personalDataWriter.WriteEndSection();
            }
        }
    }

    private void WriteCustomers(IEnumerable<CustomerInfo> customers)
    {
        var allCustomerAddresses = customerAddressInfoProvider.Get()
            .WhereIn(nameof(CustomerAddressInfo.CustomerAddressCustomerID), customers.Select(c => c.CustomerID))
            .ToList();

        foreach (var customerInfo in customers.Where(customer => customer.CustomerID > 0))
        {
            personalDataWriter.WriteStartSection(CustomerInfo.OBJECT_TYPE, "Customer");
            personalDataWriter.WriteBaseInfo(customerInfo, customerInfoColumns);

            var customerAddresses = allCustomerAddresses.Where(ca => ca.CustomerAddressCustomerID == customerInfo.CustomerID);
            foreach (var customerAddressInfo in customerAddresses)
            {
                personalDataWriter.WriteStartSection(CustomerAddressInfo.OBJECT_TYPE, "Customer address");
                personalDataWriter.WriteBaseInfo(customerAddressInfo, customerAddressInfoColumns);
                WriteCountryAndOrState(customerAddressInfo.CustomerAddressCountryID, customerAddressInfo.CustomerAddressStateID);

                personalDataWriter.WriteEndSection();
            }

            personalDataWriter.WriteEndSection();
        }
    }

    private void WriteCustomerOrders(IEnumerable<int> customerIds)
    {
        var orders = orderInfoProvider.Get()
            .WhereIn(nameof(OrderInfo.OrderCustomerID), customerIds)
            .ToList();

        var allOrderItems = orderItemInfoProvider.Get()
            .WhereIn(nameof(OrderItemInfo.OrderItemOrderID), orders.Select(o => o.OrderID))
            .ToList();

        var allOrderAddresses = orderAddressInfoProvider.Get()
            .WhereIn(nameof(OrderAddressInfo.OrderAddressOrderID), orders.Select(o => o.OrderID))
            .ToList();

        foreach (var orderInfo in orders)
        {
            personalDataWriter.WriteStartSection(OrderInfo.OBJECT_TYPE, "Order");
            personalDataWriter.WriteBaseInfo(orderInfo, orderInfoColumns);

            var items = allOrderItems.Where(oi => oi.OrderItemOrderID == orderInfo.OrderID);
            foreach (var orderItemInfo in items)
            {
                personalDataWriter.WriteStartSection(OrderItemInfo.OBJECT_TYPE, "Order item");
                personalDataWriter.WriteBaseInfo(orderItemInfo, orderItemInfoColumns);
                personalDataWriter.WriteEndSection();
            }

            var orderAddresses = allOrderAddresses.Where(oa => oa.OrderAddressOrderID == orderInfo.OrderID);
            foreach (var orderAddressInfo in orderAddresses)
            {
                personalDataWriter.WriteStartSection(OrderAddressInfo.OBJECT_TYPE, "Order address");
                personalDataWriter.WriteBaseInfo(orderAddressInfo, orderAddressInfoColumns);
                WriteCountryAndOrState(orderAddressInfo.OrderAddressCountryID, orderAddressInfo.OrderAddressStateID);
                personalDataWriter.WriteEndSection();
            }

            personalDataWriter.WriteEndSection();
        }
    }

    private void WriteThirdPartyCustomerAddresses(IEnumerable<string> customerEmails, IEnumerable<int> customerIds)
    {
        // Customer addresses referencing the data subject's email on other customers' accounts
        var thirdPartyCustomerAddresses = customerAddressInfoProvider.Get()
            .WhereIn(nameof(CustomerAddressInfo.CustomerAddressEmail), customerEmails)
            .WhereNotIn(nameof(CustomerAddressInfo.CustomerAddressCustomerID), customerIds)
            .ToList();

        if (!thirdPartyCustomerAddresses.Any())
        {
            return;
        }

        personalDataWriter.WriteStartSection("ThirdPartyCustomerAddresses", "Customer addresses referencing your data");

        foreach (var customerAddressInfo in thirdPartyCustomerAddresses)
        {
            personalDataWriter.WriteStartSection(CustomerAddressInfo.OBJECT_TYPE, "Customer address");
            personalDataWriter.WriteBaseInfo(customerAddressInfo, customerAddressInfoColumns);
            WriteCountryAndOrState(customerAddressInfo.CustomerAddressCountryID, customerAddressInfo.CustomerAddressStateID);
            personalDataWriter.WriteEndSection();
        }

        personalDataWriter.WriteEndSection();
    }

    private void WriteThirdPartyOrderAddresses(IEnumerable<string> customerEmails, IEnumerable<int> customerIds)
    {
        // Nested query for order IDs belonging to the data subject's own customers
        var ownOrderIds = orderInfoProvider.Get()
            .WhereIn(nameof(OrderInfo.OrderCustomerID), customerIds)
            .Column(nameof(OrderInfo.OrderID));

        // Order addresses referencing the data subject's email on other customers' orders
        var thirdPartyOrderAddresses = orderAddressInfoProvider.Get()
            .WhereIn(nameof(OrderAddressInfo.OrderAddressEmail), customerEmails)
            .WhereNotIn(nameof(OrderAddressInfo.OrderAddressOrderID), ownOrderIds)
            .ToList();

        if (!thirdPartyOrderAddresses.Any())
        {
            return;
        }

        personalDataWriter.WriteStartSection("ThirdPartyOrderAddresses", "Order addresses referencing your data");

        foreach (var orderAddressInfo in thirdPartyOrderAddresses)
        {
            personalDataWriter.WriteStartSection(OrderAddressInfo.OBJECT_TYPE, "Order address");
            personalDataWriter.WriteBaseInfo(orderAddressInfo, orderAddressInfoColumns);
            WriteCountryAndOrState(orderAddressInfo.OrderAddressCountryID, orderAddressInfo.OrderAddressStateID);
            personalDataWriter.WriteEndSection();
        }

        personalDataWriter.WriteEndSection();
    }
}
