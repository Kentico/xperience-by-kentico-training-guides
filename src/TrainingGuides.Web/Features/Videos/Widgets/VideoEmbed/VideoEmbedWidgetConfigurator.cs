using Kentico.Xperience.Admin.Base.Forms;
using TrainingGuides.Web.Features.Shared.VisibilityConditions;

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