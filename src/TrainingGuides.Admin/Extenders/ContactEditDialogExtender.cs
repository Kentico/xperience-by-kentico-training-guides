using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.DigitalMarketing.UIPages;
using TrainingGuides.Admin.Extenders;

[assembly: PageExtender(typeof(ContactEditDialogExtender))]

namespace TrainingGuides.Admin.Extenders;
public class ContactEditDialogExtender : PageExtender<ContactEditDialog>
{
    public override Task ConfigurePage()
    {
        Page.PageConfiguration.SubmitConfiguration.Label = "Ahojteee";
        return base.ConfigurePage();
    }

    // Configures the properties of the client template this page is based on
    // Called after the 'ConfigurePage' method
    public override Task<TemplateClientProperties> ConfigureTemplateProperties(TemplateClientProperties properties)
    {
        // Manipulate properties as desired
        var x = properties;
        return base.ConfigureTemplateProperties(properties);
    }
}