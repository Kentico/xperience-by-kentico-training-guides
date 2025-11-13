namespace TrainingGuides.Web.Features.ContactImport;

internal static class ContactishValues
{
    // Properties of the standard contact group
    internal const string ContactGroupName = "ContactishRecipientSegment";
    internal const string ContactGroupDisplayName = "Contactish recipient segment";
    internal const string ContactGroupDescription = "Contact group representing the 'Recipients' segment from Contactish.";
    // You can create a dummy contact group so you can see how its dynamic condition is structured in the database, in order to define similar conditions in code.
    internal const string ContactGroupDynamicCondition = "{%Rule(\"(Contact.ContactFieldContainsValue(\\\"TrainingGuidesContactishSegmentIdentifiers\\\"," +
        "\\\"Contains\\\", \\\"c2e5f9a1-7b4d-4e8c-a3f6-9d1b5c8e2a7f\\\"))\", \"<rules><r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" /><r pos=\\\"0\\\"" +
        " par=\\\"0\\\" op=\\\"and\\\" n=\\\"CMSContactFieldContainsValue\\\" ><p n=\\\"op\\\"><t>contains</t><v>Contains</v><r>1</r>" +
        "<d>select operator</d><vt>text</vt><tv>0</tv></p><p n=\\\"field\\\"><t>#select field</t><v>TrainingGuidesContactishSegmentIdentifiers</v>" +
        "<r>1</r><d>select field</d><vt>text</vt><tv>0</tv></p><p n=\\\"value\\\"><t>#enter value</t><v>c2e5f9a1-7b4d-4e8c-a3f6-9d1b5c8e2a7f</v>" +
        "<r>1</r><d>enter value</d><vt>text</vt><tv>0</tv></p></r></rules>\")%}";

    // Properties of the recipient list
    internal const string RecipientListName = "ContactishRecipients";
    internal const string RecipientListDisplayName = "Contactish recipients";
    internal const string RecipientListDescription = "Recipient list representing 'Recipients' segment from Contactish";

    // GUIDs for pages related to the Contactish recipient list
    // Using a GUID is more reliable than using the tree path, since editors can change the tree path of pages, but not their GUIDs.
    internal static readonly Guid RecipientListThankYouPageGuid = new("B70F47CC-81B8-40F2-8527-178C0ECFFFF2");
    internal static readonly Guid RecipientListGoodbyePageGuid = new("B3EA83AF-E976-47D8-B408-3D1B12351361");

    // Names of elements found in XML file
    internal const string CONTACTS_ELEMENT = "Contacts";
    internal const string CONTACT_ELEMENT = "Contact";
    internal const string FIRST_NAME_ELEMENT = "FirstName";
    internal const string LAST_NAME_ELEMENT = "LastName";
    internal const string EMAIL_ELEMENT = "Email";
    internal const string IDENTIFIER_ELEMENT = "Identifier";
    internal const string SEGMENT_IDENTIFIERS_ELEMENT = "SegmentIdentifiers";
    internal const string LAST_UPDATED_ELEMENT = "LastUpdated";

    // Names of custom fields in ContactInfo class
    internal const string IDENTIFIER_FIELD = "TrainingGuidesContactishIdentifier";
    internal const string SEGMENT_IDENTIFIERS_FIELD = "TrainingGuidesContactishSegmentIdentifiers";
    internal const string LAST_SYNCED_FIELD = "TrainingGuidesContactishLastSynced";
    internal const string LAST_UPDATED_FIELD = "TrainingGuidesContactishLastUpdated";
}