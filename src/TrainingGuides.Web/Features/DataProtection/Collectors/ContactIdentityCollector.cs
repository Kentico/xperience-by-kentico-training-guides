﻿using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;

namespace TrainingGuides.Web.Features.DataProtection.Collectors;

public class ContactIdentityCollector : IIdentityCollector
{
    private const string EMAIL_KEY = "email";

    public void Collect(IDictionary<string, object> dataSubjectFilter, List<BaseInfo> identities)
    {
        string? email = dataSubjectFilter.ContainsKey(EMAIL_KEY)
            ? dataSubjectFilter[EMAIL_KEY] as string
            : string.Empty;

        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var contacts = ContactInfo.Provider
            .Get()
            .WhereEquals(nameof(ContactInfo.ContactEmail), email)
            .ToList();

        if (contacts.Count() == 0)
        {
            contacts.Add(new ContactInfo() { ContactEmail = email });
        }

        identities.AddRange(contacts);
    }
}