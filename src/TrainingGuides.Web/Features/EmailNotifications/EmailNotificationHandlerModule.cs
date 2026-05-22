using CMS;
using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;
using TrainingGuides.Web.Features.EmailNotifications;

[assembly: RegisterModule(typeof(EmailNotificationHandlerModule))]

namespace TrainingGuides.Web.Features.EmailNotifications;

public class EmailNotificationHandlerModule : Module
{
    public EmailNotificationHandlerModule() : base(nameof(EmailNotificationHandlerModule))
    { }

    protected override void OnPreInit(ModulePreInitParameters parameters)
    {
        base.OnPreInit(parameters);

        parameters.Services.AddInfoObjectEventHandler<InfoObjectAfterInsertEvent<UserInfo>, UserAfterInsertHandler>();
    }
}

internal class UserAfterInsertHandler(IEmailNotificationService emailNotificationService)
    : IInfoObjectEventHandler<InfoObjectAfterInsertEvent<UserInfo>>
{
    // Fire-and-forget is acceptable for After handlers, because the save operation has already completed.
    // Avoid this pattern in Before handlers, where execution must wait for completion.
    public void Handle(InfoObjectAfterInsertEvent<UserInfo> infoObjectEvent) =>
        _ = SendNotificationAsync(infoObjectEvent.InfoObject, CancellationToken.None);

    public Task HandleAsync(InfoObjectAfterInsertEvent<UserInfo> infoObjectEvent, CancellationToken cancellationToken) =>
        SendNotificationAsync(infoObjectEvent.InfoObject, cancellationToken);

    private async Task SendNotificationAsync(UserInfo user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await emailNotificationService.SendEmailAsync($"New user created ({user.Email})", $"New user inserted with ID {user.UserID}, email {user.Email}, guid {user.UserGUID}");
    }
}
