using CMS.DataEngine;
using CMS.DataProtection;
using CMS.OnlineForms;
using System.Data;
using System.Xml.Linq;
using TrainingGuides.Web.Features.DataProtection.Collectors;

namespace TrainingGuides.Web.Features.DataProtection.Services;

public class FormCollectionService : IFormCollectionService
{
    private readonly IBizFormInfoProvider bizFormInfoProvider;

    public FormCollectionService(IBizFormInfoProvider bizFormInfoProvider)
    {
        this.bizFormInfoProvider = bizFormInfoProvider;
    }

    private bool IsField(XElement element) => element.Name.ToString() == "field";

    private bool UsesComponent(XElement fieldElement, string componentIdentifier)
    {
        var matchingComponents = fieldElement
            .Descendants()
            .Where(desc => desc.Name == "componentidentifier" && desc.Value == componentIdentifier);
        return matchingComponents.Count() > 0;
    }

    private bool IsInList(XElement fieldElement, IEnumerable<string> list) =>
        list.Contains(fieldElement.Attribute("column")?.Value, StringComparer.OrdinalIgnoreCase);

    private bool IsNotInCollection(XElement fieldElement, IEnumerable<XElement> excludedFieldElements) =>
        !excludedFieldElements.Contains(fieldElement);

    private bool IsNotID(XElement fieldElement)
    {
        string isPK = fieldElement.Attribute("isPK")?.Value;
        return !bool.Parse(isPK ?? "false");
    }

    private string GetCaption(XElement fieldElement) =>
        fieldElement
            .Descendants()
            .Where(desc => desc.Name == "fieldcaption")
            .FirstOrDefault()?
            .Value;

    /// <summary>
    /// Gets infromation about all forms in the database, including which of the form's fields are email fields
    /// </summary>
    /// <returns>A <see cref="Dictionary{TKey, TValue}"/> mapping a form's <see cref="Guid"/> to a <see cref="FormDefinition"/>, which specifies the form's email fields and non-email fields.</returns>
    public Dictionary<Guid, FormDefinition> GetForms()
    {
        var result = new Dictionary<Guid, FormDefinition>();
        var bizForms = bizFormInfoProvider.Get();
        foreach (var bizForm in bizForms)
        {
            var dataClassInfo = DataClassInfoProvider.GetDataClassInfo(bizForm.FormClassID);
            if (dataClassInfo == null || string.IsNullOrEmpty(dataClassInfo.ClassFormDefinition))
            {
                continue;
            }

            IEnumerable<string> mappedEmailFields = [];

            if (!string.IsNullOrEmpty(dataClassInfo.ClassContactMapping))
            {
                var contactMapping = XElement.Parse(dataClassInfo.ClassContactMapping);

                //gets lowercase names of fields mapped to contact email (there should be only one unless they mess with the database directly)
                mappedEmailFields = contactMapping?.Elements()
                .Where(child => child.Attribute("column")?.Value == "ContactEmail").Select(field => field.Attribute("mappedtofield")?.Value);
            }

            var formDefinition = XElement.Parse(dataClassInfo.ClassFormDefinition);

            //gets all form fields which are either mapped to the ContactEmail colun, or which use the Kentico.EmailInput form control
            var emailFields = formDefinition?.Elements()
                .Where(child => IsField(child) && (IsInList(child, mappedEmailFields) || UsesComponent(child, "Kentico.EmailInput")));

            //gets the remaining fields
            var otherFields = formDefinition?.Elements()
                .Where(child => IsField(child) && IsNotInCollection(child, emailFields) && IsNotID(child));

            result.Add(
                bizForm.FormGUID,
                new FormDefinition(
                    emailFields.Select(element => element.Attribute("column")?.Value).ToList(),
                    otherFields.Select(element => new CollectedColumn(element.Attribute("column")?.Value, GetCaption(element))).ToList()
                    )
                );
        }
        return result;
    }


    /// <summary>
    /// Gets an object query for all bizform items with any of the supplied emails in any of its email columns, or with any of the supplied consent agreements
    /// </summary>
    /// <param name="emails">The email addresses to look for</param>
    /// <param name="consentAgreementGuids">The guids of consents agreed to by the contact</param>
    /// <param name="row">a DataRow representing the DataClassInfo associated with the given form</param>
    /// <param name="formDefinition">The form definition of the supplied bizform</param>
    /// <returns>An <see cref="ObjectQuery"/>for all <see cref="BizFormItem"/> objects related to the provided contact through a consent agreement or email address</returns>
    public ObjectQuery<BizFormItem> GetBizFormItems(ICollection<string> emails, ObjectQuery<ConsentAgreementInfo> consentAgreementGuids, DataRow row, FormDefinition formDefinition)
    {
        var formClass = new DataClassInfo(row);

        var bizFormItems = BizFormItemProvider.GetItems(formClass.ClassName);
        foreach (string column in formDefinition.EmailColumns)
        {
            bizFormItems.Or().WhereIn(column, emails);
        }

        var consentColumns = formDefinition.FormColumns
            .Where(column => column.Name
            .Contains(FormDefinition.FORM_CONSENT_COLUMN_NAME)).Select(column => column.Name);

        if (formClass.ClassFormDefinition.Contains(FormDefinition.FORM_CONSENT_COLUMN_NAME))
        {
            foreach (string columnName in consentColumns)
            {
                bizFormItems.Or().WhereIn(columnName, consentAgreementGuids);
            }
        }
        return bizFormItems;
    }
}