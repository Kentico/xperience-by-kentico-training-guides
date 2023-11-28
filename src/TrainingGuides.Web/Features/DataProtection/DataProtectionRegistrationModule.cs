using CMS;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataProtection;
using TrainingGuides.Web.Features.DataProtection;
using TrainingGuides.Web.Features.DataProtection.Collectors;
using TrainingGuides.Web.Features.DataProtection.Erasers;

[assembly: RegisterModule(typeof(DataProtectionRegistrationModule))]

namespace TrainingGuides.Web.Features.DataProtection;

public class DataProtectionRegistrationModule : Module
{
    public DataProtectionRegistrationModule()
        : base("DataProtectionRegistration")
    {
    }

    // Contains initialization code that is executed when the application starts
    protected override void OnInit(ModuleInitParameters parameters)
    {
        var serviceProvider = parameters.Services.GetRequiredService<IServiceProvider>();

        base.OnInit(parameters);

        // Adds the ContactIdentityCollector to the collection of registered identity collectors
        IdentityCollectorRegister.Instance.Add(new ContactIdentityCollector());

        // Adds the ContactDataCollector to the collection of registered personal data collectors
        PersonalDataCollectorRegister.Instance.Add(ActivatorUtilities.CreateInstance<ContactDataCollector>(serviceProvider));

        // Adds the ContactDataEraser to the collection of registered personal data erasers
        PersonalDataEraserRegister.Instance.Add(ActivatorUtilities.CreateInstance<ContactDataEraser>(serviceProvider));
    }
}
