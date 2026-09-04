using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.DigitalMarketing.UIPages;
using TrainingGuides.Admin.Extenders;

[assembly: PageExtender(typeof(TrainingGuidesRightToBeForgottenExtender))]

namespace TrainingGuides.Admin.Extenders;

public class TrainingGuidesRightToBeForgottenExtender : PageExtender<RightToBeForgotten>
{
    public override Task ConfigurePage()
    {
        // Assigns a custom erasure dialog model to the page configuration
        Page.PageConfiguration.DataErasureDialogModel = new TrainingGuidesDataErasureDialogModel();

        return base.ConfigurePage();
    }
}