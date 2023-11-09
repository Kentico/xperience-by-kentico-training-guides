using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;
using System.Collections.Generic;
using System.Linq;

namespace KBank.Web.DataProtection;

public class ContactIdentityCollector : IIdentityCollector
{
    private const string EMAIL_KEY = "email";

    public void Collect(IDictionary<string, object> dataSubjectFilter, List<BaseInfo> identities)
    {
        if (!dataSubjectFilter.ContainsKey(EMAIL_KEY))
        {
            return;
        }

        var email = dataSubjectFilter[EMAIL_KEY] as string;

        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var contacts = ContactInfo.Provider.Get()
                                            .WhereEquals(nameof(ContactInfo.ContactEmail), email)
                                            .ToList();
        if (contacts.Count() == 0)
        {
            contacts.Add(new ContactInfo() { ContactEmail = email });
        }

        identities.AddRange(contacts);
    }
}
