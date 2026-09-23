using CMS.Commerce;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.Membership;

namespace TrainingGuides.Web.Features.DataProtection.Collectors;

public class IdentityCollector(IInfoProvider<ContactInfo> contactInfoProvider, IInfoProvider<MemberInfo> memberInfoProvider, IInfoProvider<CustomerInfo> customerInfoProvider) : IIdentityCollector
{
    public void Collect(IDictionary<string, object> dataSubjectFilter, List<BaseInfo> identities)
    {
        // Does nothing if the identifier input value is not available or empty
        if (!dataSubjectFilter.TryGetValue(PersonalDataConstants.DATA_SUBJECT_IDENTIFIER_KEY, out object? value))
        {
            return;
        }
        string? email = value as string;
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var contacts = contactInfoProvider
            .Get()
            .WhereEquals(nameof(ContactInfo.ContactEmail), email)
            .ToList();

        // If no contact exists with the provided email, create a new one.
        // This will allow us to retrieve form submissions that contain the email even if they are not currently tied to a contact.
        if (contacts.Count() == 0)
        {
            contacts.Add(new ContactInfo() { ContactEmail = email });
        }

        identities.AddRange(contacts);

        var members = memberInfoProvider
            .Get()
            .WhereEquals(nameof(MemberInfo.MemberEmail), email)
            .ToList();

        identities.AddRange(members);

        var customers = customerInfoProvider
            .Get()
            .WhereEquals(nameof(CustomerInfo.CustomerEmail), email)
            .ToList();

        // If no customer exists with the provided email, create a new one.
        // This will allow us to retrieve third-party customer and order addresses that contain the email even if they are associated with a different customer.
        if (customers.Count() == 0)
        {
            customers.Add(new CustomerInfo() { CustomerEmail = email });
        }

        identities.AddRange(customers);
    }
}