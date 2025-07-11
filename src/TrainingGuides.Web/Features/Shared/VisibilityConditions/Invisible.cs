using Kentico.Xperience.Admin.Base.Forms;

namespace TrainingGuides.Web.Features.Shared.VisibilityConditions;

// Custom visibility condition that is always false, used to dynamically hide fields in configurators
public class Invisible : VisibilityCondition
{
    public override bool Evaluate(IFormFieldValueProvider formFieldValueProvider) => false;
}