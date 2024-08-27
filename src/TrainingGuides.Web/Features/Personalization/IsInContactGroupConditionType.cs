using System.Collections.Generic;

using CMS.ContactManagement;
using CMS.DataEngine;

using TrainingGuides.Web.Features.Personalization;

using Kentico.PageBuilder.Web.Mvc.Personalization;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;

[assembly: RegisterPersonalizationConditionType(
	"TrainingGuides.Web.Features.Personalization",
	typeof(IsInContactGroupConditionType),
	"Is in contact group",
	Description = "Evaluates if the current contact is in one of the contact groups.", IconClass = "icon-app-contact-groups", Hint = "Display to visitors who match at least one of the selected contact groups:")]

namespace TrainingGuides.Web.Features.Personalization;

/// <summary>
/// Personalization condition type based on contact group.
/// </summary>
public class IsInContactGroupConditionType : ConditionType
{
	/// <summary>
	/// Selected contact group code names.
	/// </summary>
	[ObjectSelectorComponent(PredefinedObjectType.CONTACTGROUP,
    Label = "Contact groups",
    Order = 0,
    MaximumItems = 0)]
	public IEnumerable<ObjectRelatedItem> SelectedContactGroups { get; set; } = Enumerable.Empty<ObjectRelatedItem>();


	/// <summary>
	/// Evaluate condition type.
	/// </summary>
	/// <returns>Returns <c>true</c> if implemented condition is met.</returns>
	public override bool Evaluate()
	{
		var contact = ContactManagementContext.GetCurrentContact();

		if (contact == null)
		{
			return false;
		}

		if (SelectedContactGroups == null || !SelectedContactGroups.Any())
		{
			return contact.ContactGroups.Count == 0;
		}

		return contact.IsInAnyContactGroup(SelectedContactGroups.Select(c => c.ObjectCodeName).ToArray());
	}
}

