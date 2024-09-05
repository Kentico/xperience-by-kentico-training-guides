using CMS;
using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;
using TrainingGuides.Web.Features.EmailNotifications;

[assembly: RegisterModule(typeof(EmailNotificationHandlerModule))]

namespace TrainingGuides.Web.Features.EmailNotifications;
public class EmailNotificationHandlerModule : Module
{

    private IEmailNotificationService? emailNotificationService;

    public EmailNotificationHandlerModule() : base("EmailNotificationHandler")
    { }

    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit();

        emailNotificationService = parameters.Services.GetRequiredService<IEmailNotificationService>();

        UserInfo.TYPEINFO.Events.Insert.After += User_InsertAfter;
    }

    private void User_InsertAfter(object? sender, ObjectEventArgs e)
    {
        if (e.Object is not UserInfo user)
        {
            return;
        }

        emailNotificationService?.SendEmailAsync($"New user created ({user.Email})", $"New user inserted with ID {user.UserID}, email {user.Email}, guid {user.UserGUID}");
    }
}