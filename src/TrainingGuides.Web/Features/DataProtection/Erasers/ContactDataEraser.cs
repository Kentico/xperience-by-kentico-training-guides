using CMS.Activities;
using CMS.Base;
using CMS.Commerce;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineForms;
using TrainingGuides.Web.Features.DataProtection.Collectors;
using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Admin.Extenders;

namespace TrainingGuides.Web.Features.DataProtection.Erasers;

public class ContactDataEraser : IPersonalDataEraser
{
    private const string DELETE_FORM_ACTIVITIES = nameof(TrainingGuidesDataErasureDialogModel.DeleteSubmittedFormsActivities);
    private const string DELETE_FORM_DATA = nameof(TrainingGuidesDataErasureDialogModel.DeleteSubmittedFormsData);
    private const string DELETE_ACTIVITIES = nameof(TrainingGuidesDataErasureDialogModel.DeleteActivities);
    private const string DELETE_CONTACTS = nameof(TrainingGuidesDataErasureDialogModel.DeleteContacts);
    private const string DELETE_MEMBERS = nameof(TrainingGuidesDataErasureDialogModel.DeleteMembers);
    private const string DELETE_CUSTOMER_DATA = nameof(TrainingGuidesDataErasureDialogModel.DeleteCustomerAndOrderData);
    private readonly IFormCollectionService formCollectionService;
    private readonly Dictionary<Guid, FormDefinition> forms;

    private readonly IInfoProvider<ContactInfo> contactInfoProvider;
    private readonly IInfoProvider<ActivityInfo> activityInfoProvider;
    private readonly IInfoProvider<ConsentAgreementInfo> consentAgreementInfoProvider;
    private readonly IInfoProvider<BizFormInfo> bizFormInfoProvider;
    private readonly IInfoProvider<MemberInfo> memberInfoProvider;
    private readonly IInfoProvider<CustomerInfo> customerInfoProvider;
    private readonly IInfoProvider<OrderInfo> orderInfoProvider;
    private readonly IInfoProvider<OrderAddressInfo> orderAddressInfoProvider;
    private readonly IInfoProvider<CustomerAddressInfo> customerAddressInfoProvider;

    public ContactDataEraser(IFormCollectionService formCollectionService,
        IInfoProvider<ContactInfo> contactInfoProvider,
        IInfoProvider<ActivityInfo> activityInfoProvider,
        IInfoProvider<ConsentAgreementInfo> consentAgreementInfoProvider,
        IInfoProvider<BizFormInfo> bizFormInfoProvider,
        IInfoProvider<MemberInfo> memberInfoProvider,
        IInfoProvider<CustomerInfo> customerInfoProvider,
        IInfoProvider<OrderInfo> orderInfoProvider,
        IInfoProvider<OrderAddressInfo> orderAddressInfoProvider,
        IInfoProvider<CustomerAddressInfo> customerAddressInfoProvider)
    {
        this.formCollectionService = formCollectionService;

        this.contactInfoProvider = contactInfoProvider;
        this.activityInfoProvider = activityInfoProvider;
        this.consentAgreementInfoProvider = consentAgreementInfoProvider;
        this.bizFormInfoProvider = bizFormInfoProvider;
        this.memberInfoProvider = memberInfoProvider;
        this.customerInfoProvider = customerInfoProvider;
        this.orderInfoProvider = orderInfoProvider;
        this.orderAddressInfoProvider = orderAddressInfoProvider;
        this.customerAddressInfoProvider = customerAddressInfoProvider;
        forms = this.formCollectionService.GetForms();
    }

    public void Erase(IEnumerable<BaseInfo> identities, IDictionary<string, object> configuration)
    {
        var contacts = identities.OfType<ContactInfo>().ToList();
        var members = identities.OfType<MemberInfo>().ToList();
        var customers = identities.OfType<CustomerInfo>().ToList();

        if (!(contacts.Any() || members.Any() || customers.Any()))
        {
            return;
        }

        using (new CMSActionContext())
        {
            if (contacts.Any())
            {
                var contactIds = contacts.Select(c => c.ContactID).ToList();
                var contactEmails = contacts.Select(c => c.ContactEmail).ToList();

                DeleteSubmittedFormsActivities(contactIds, configuration);
                DeleteActivities(contactIds, configuration);
                DeleteContacts(contacts, configuration);
                DeleteSiteSubmittedFormsData(contactEmails, contactIds, configuration);
            }
            if (members.Any())
            {
                DeleteMembers(members, configuration);
            }
            if (customers.Any())
            {
#warning "Check the laws of your jurisdiction before deleting or anonymizing customer orders, as your organization may be legally required to keep them for a certain period of time."
                DeleteThirdPartyAddresses(customers.Select(c => c.CustomerEmail), configuration);
                DeleteCustomerOrders(customers.Select(c => c.CustomerID), configuration);
                DeleteCustomers(customers, configuration);
            }
        }
    }

    private void DeleteSubmittedFormsActivities(ICollection<int> contactIds,
        IDictionary<string, object> configuration)
    {
        if (configuration.TryGetValue(DELETE_FORM_ACTIVITIES, out object? deleteSubmittedFormsActivities)
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
        if (configuration.TryGetValue(DELETE_FORM_DATA, out object? deleteSubmittedForms)
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
        if (configuration.TryGetValue(DELETE_ACTIVITIES, out object? deleteActivities)
            && ValidationHelper.GetBoolean(deleteActivities, false))
        {
            activityInfoProvider.BulkDelete(
                new WhereCondition().WhereIn("ActivityContactID", contactIds));
        }
    }

    private void DeleteContacts(IEnumerable<ContactInfo> contacts, IDictionary<string, object> configuration)
    {
        if (configuration.TryGetValue(DELETE_CONTACTS, out object? deleteContacts) &&
            ValidationHelper.GetBoolean(deleteContacts, false))
        {
            foreach (var contactInfo in contacts.Where(contact => contact.ContactID > 0))
            {
                contactInfoProvider.Delete(contactInfo);
            }
        }
    }

    private void DeleteMembers(IEnumerable<MemberInfo> members, IDictionary<string, object> configuration)
    {
        if (configuration.TryGetValue(DELETE_MEMBERS, out object? deleteMembers)
            && ValidationHelper.GetBoolean(deleteMembers, false))
        {
            foreach (var memberInfo in members.Where(member => member.MemberID > 0))
            {
                memberInfoProvider.Delete(memberInfo);
            }
        }
    }

    private void DeleteCustomers(IEnumerable<CustomerInfo> customers, IDictionary<string, object> configuration)
    {
        if (configuration.TryGetValue(DELETE_CUSTOMER_DATA, out object? deleteCustomers)
            && ValidationHelper.GetBoolean(deleteCustomers, false))
        {
            foreach (var customerInfo in customers.Where(customer => customer.CustomerID > 0))
            {
                // This automatically deletes related customer addresses
                customerInfoProvider.Delete(customerInfo);
            }
        }
    }

    private void DeleteCustomerOrders(IEnumerable<int> customerIds, IDictionary<string, object> configuration)
    {
        if (configuration.TryGetValue(DELETE_CUSTOMER_DATA, out object? deleteCustomersAndOrders)
            && ValidationHelper.GetBoolean(deleteCustomersAndOrders, false))
        {
            var customerOrders = orderInfoProvider.Get()
                .WhereIn(nameof(OrderInfo.OrderCustomerID), customerIds)
                .ToList();

            foreach (var order in customerOrders)
            {
                // This automatically deletes related order items and order addresses.
                orderInfoProvider.Delete(order);
            }
        }
    }

    private void DeleteThirdPartyAddresses(IEnumerable<string> customerEmails, IDictionary<string, object> configuration)
    {
        if (configuration.TryGetValue(DELETE_CUSTOMER_DATA, out object? deleteCustomersAndOrders)
            && ValidationHelper.GetBoolean(deleteCustomersAndOrders, false))
        {
            var orderAddresses = orderAddressInfoProvider.Get()
                .WhereIn(nameof(OrderAddressInfo.OrderAddressEmail), customerEmails)
                .ToList();

            var customerAddresses = customerAddressInfoProvider.Get()
                .WhereIn(nameof(CustomerAddressInfo.CustomerAddressEmail), customerEmails)
                .ToList();

            foreach (var orderAddress in orderAddresses)
            {
                // This automatically deletes related orders and order items.
                orderAddressInfoProvider.Delete(orderAddress);
            }

            foreach (var customerAddress in customerAddresses)
            {
                customerAddressInfoProvider.Delete(customerAddress);
            }
        }
    }
}
