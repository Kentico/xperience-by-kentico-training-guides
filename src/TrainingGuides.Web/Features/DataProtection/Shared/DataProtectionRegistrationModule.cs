using CMS;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataProtection;
using TrainingGuides.Web.Features.DataProtection.Collectors;
using TrainingGuides.Web.Features.DataProtection.Erasers;
using TrainingGuides.Web.Features.DataProtection.Shared;

[assembly: RegisterModule(
    type: typeof(DataProtectionRegistrationModule))]

namespace TrainingGuides.Web.Features.DataProtection.Shared;

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

        var contactInfoProvider = Service.Resolve<IInfoProvider<ContactInfo>>();

        base.OnInit(parameters);

        // Adds the ContactIdentityCollector to the collection of registered identity collectors
        IdentityCollectorRegister.Instance.Add(new ContactIdentityCollector(contactInfoProvider));

        // Adds the ContactDataCollector to the collection of registered personal data collectors
        PersonalDataCollectorRegister.Instance.Add(ActivatorUtilities.CreateInstance<ContactDataCollector>(serviceProvider));

        // Adds the ContactDataEraser to the collection of registered personal data erasers
        PersonalDataEraserRegister.Instance.Add(ActivatorUtilities.CreateInstance<ContactDataEraser>(serviceProvider));
    }
}
