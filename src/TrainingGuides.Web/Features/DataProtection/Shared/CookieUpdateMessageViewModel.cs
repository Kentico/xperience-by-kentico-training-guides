namespace TrainingGuides.Web.Features.DataProtection.Shared;

public class CookieUpdateMessageViewModel
{

    public string Message { get; set; }

    public CookieUpdateMessageViewModel(string message)
    {
        Message = message;
    }

}