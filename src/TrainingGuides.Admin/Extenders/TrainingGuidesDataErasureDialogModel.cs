using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.DigitalMarketing;

namespace TrainingGuides.Admin.Extenders;

public class TrainingGuidesDataErasureDialogModel : IDataErasureDialogModel
{
    /// <summary>
    /// Indicates whether corresponding contacts should be deleted.
    /// </summary>
    [CheckBoxComponent(Label = "Delete contact", Order = 0)]
    public bool DeleteContacts { get; set; } = false;


    /// <summary>
    /// Indicates whether all activities of corresponding contacts should be deleted.
    /// </summary>
    [CheckBoxComponent(Label = "Delete activities", Order = 1)]
    public bool DeleteActivities { get; set; } = false;


    /// <summary>
    /// Indicates whether form submission activities of corresponding contacts should be deleted.
    /// </summary>
    [CheckBoxComponent(Label = "Delete submitted form activities", Order = 2)]
    public bool DeleteSubmittedFormsActivities { get; set; } = false;


    /// <summary>
    /// Indicates whether submitted forms of corresponding contacts should be deleted.
    /// </summary>
    [CheckBoxComponent(Label = "Delete submitted form data", Order = 3)]
    public bool DeleteSubmittedFormsData { get; set; } = false;


    /// <summary>
    /// Indicates whether corresponding members should be deleted.
    /// </summary>
    [CheckBoxComponent(Label = "Delete member", Order = 4)]
    public bool DeleteMembers { get; set; } = false;

    /// <summary>
    /// Indicates whether corresponding customers, addresses, and orders should be deleted.
    /// </summary>
    [CheckBoxComponent(Label = "Delete customer and order data", Order = 5)]
    public bool DeleteCustomerAndOrderData { get; set; } = false;


    /// <inheritdoc/>
    public virtual Task<ValidationResult> Validate()
    {
        if (IsAnyOptionSelected())
        {
            return ValidationResult.SuccessResult();
        }

        return ValidationResult.FailResult("No data selected for erasure.");
    }


    /// <summary>
    /// Indicates that at least one of erasure options was selected.
    /// </summary>
    protected virtual bool IsAnyOptionSelected() => DeleteContacts || DeleteActivities || DeleteSubmittedFormsActivities
            || DeleteSubmittedFormsData || DeleteMembers || DeleteCustomerAndOrderData;
}