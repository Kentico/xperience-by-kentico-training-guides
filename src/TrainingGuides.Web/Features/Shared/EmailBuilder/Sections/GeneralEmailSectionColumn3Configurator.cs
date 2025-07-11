

using Kentico.Xperience.Admin.Base.Forms;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.VisibilityConditions;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.Sections;

public class GeneralEmailSectionColumn3Configurator : FormComponentConfigurator<DropDownComponent>
{
    public override async Task Configure(DropDownComponent formComponent, IFormFieldValueProvider formFieldValueProvider, CancellationToken cancellationToken)
    {
        if (!GetShowColumn3(formFieldValueProvider))
        {
            HideField(formComponent);
        }
    }

    private bool GetShowColumn3(IFormFieldValueProvider formFieldValueProvider)
    {
        if (formFieldValueProvider.TryGet(nameof(GeneralEmailSectionProperties.ColumnLayout), out string columnLayout))
        {
            var layout = new DropdownEnumOptionProvider<ColumnLayoutOption>().Parse(columnLayout, ColumnLayoutOption.OneColumn);
            return layout is ColumnLayoutOption.ThreeColumnEven or ColumnLayoutOption.ThreeColumnSmLgSm;
        }
        return true;
    }

    private void HideField(DropDownComponent formComponent) =>
        formComponent.VisibilityConditions.Add(new Invisible());
}
