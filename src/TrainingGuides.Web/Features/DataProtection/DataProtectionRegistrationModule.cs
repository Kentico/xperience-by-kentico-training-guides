using CMS;
using CMS.DataEngine;
using CMS.DataProtection;
using TrainingGuides.Web.Features.DataProtection;
using TrainingGuides.Web.Features.DataProtection.Collectors;
using TrainingGuides.Web.Features.DataProtection.Erasers;
using TrainingGuides.Web.Features.DataProtection.Services;

[assembly: RegisterModule(typeof(DataProtectionRegistrationModule))]

namespace TrainingGuides.Web.Features.DataProtection;

public class DataProtectionRegistrationModule : Module
{
    private readonly IFormCollectionService formCollectionService;
    public DataProtectionRegistrationModule(IFormCollectionService formCollectionService)
        : base("DataProtectionRegistration")
    {
        this.formCollectionService = formCollectionService;
    }

    // Contains initialization code that is executed when the application starts
    protected override void OnInit()
    {
        base.OnInit();

        // Adds the ContactIdentityCollector to the collection of registered identity collectors
        IdentityCollectorRegister.Instance.Add(new ContactIdentityCollector());

        // Adds the ContactDataCollector to the collection of registered personal data collectors
        PersonalDataCollectorRegister.Instance.Add(new ContactDataCollector());

        // Adds the ContactDataEraser to the collection of registered personal data erasers
        PersonalDataEraserRegister.Instance.Add((IPersonalDataEraser)Activator.CreateInstance(typeof(ContactDataEraser)));
    }
}
