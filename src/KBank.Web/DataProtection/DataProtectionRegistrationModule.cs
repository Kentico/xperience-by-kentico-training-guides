using CMS;
using CMS.DataEngine;
using CMS.DataProtection;
using KBank.Web.DataProtection;

[assembly: RegisterModule(typeof(DataProtectionRegistrationModule))]
namespace KBank.Web.DataProtection
{
    public class DataProtectionRegistrationModule : Module
    {
        public DataProtectionRegistrationModule()
            : base("DataProtectionRegistration")
        {
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
            PersonalDataEraserRegister.Instance.Add(new ContactDataEraser());
        }
    }
}
