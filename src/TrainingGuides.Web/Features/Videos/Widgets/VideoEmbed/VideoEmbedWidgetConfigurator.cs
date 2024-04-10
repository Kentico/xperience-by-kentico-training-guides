using Kentico.Xperience.Admin.Base.Forms;

namespace TrainingGuides.Web.Features.Videos.Widgets.VideoEmbed;
public class VideoEmbedWidgetConfigurator : FormComponentConfigurator<NumberInputComponent>
{
    public override async Task Configure(NumberInputComponent formComponent, IFormFieldValueProvider formFieldValueProvider, CancellationToken cancellationToken)
    {
        if (!GetShowDimensions(formFieldValueProvider))
        {
            HideField(formComponent);
        }
    }

    private bool GetShowDimensions(IFormFieldValueProvider formFieldValueProvider)
    {
        if (formFieldValueProvider.TryGet(nameof(VideoEmbedWidgetProperties.DynamicSize), out bool dynamicSize))
        {
            if (formFieldValueProvider.TryGet(nameof(VideoEmbedWidgetProperties.Service), out string service))
            {
                return service == VideoEmbedWidgetProperties.YOUTUBE || !dynamicSize;
            }
        }
        return true;
    }

    private void HideField(NumberInputComponent formComponent) =>
        formComponent.VisibilityConditions.Add(new Invisible());
}

// Custom visibility condition that is always false, used to dynamically hide fields
public class Invisible : VisibilityCondition
{
    public override bool Evaluate(IFormFieldValueProvider formFieldValueProvider) => false;
}