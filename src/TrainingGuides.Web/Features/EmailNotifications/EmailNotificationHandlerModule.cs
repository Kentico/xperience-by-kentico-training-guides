using CMS;
using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;
using TrainingGuides.Web.Features.EmailNotifications;

[assembly: RegisterModule(typeof(EmailNotificationHandlerModule))]

namespace TrainingGuides.Web.Features.EmailNotifications;
public class EmailNotificationHandlerModule : Module
{

    private IServiceProvider serviceProvider;

    public EmailNotificationHandlerModule() : base("EmailNotificationHandler")
    {
    }

    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit();

        serviceProvider = parameters.Services;

        UserInfo.TYPEINFO.Events.Insert.After += User_InsertAfter;
    }

    private void User_InsertAfter(object? sender, ObjectEventArgs e)
    {
        using var scope = serviceProvider.CreateScope();

        var emailNotificationService = scope.ServiceProvider.GetRequiredService<IEmailNotificationService>();

        if (e.Object is not UserInfo user)
        {
            return;
        }

        emailNotificationService.SendEmailAsync($"New user created ({user.Email})", $"New user inserted with ID {user.UserID}, email {user.Email}, guid {user.UserGUID}");
    }
}