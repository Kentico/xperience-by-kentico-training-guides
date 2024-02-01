namespace TrainingGuides.Web.Features.Shared.Models;
public class LinkViewModel
{
    public string Name { get; set; } = null!;
    public string CTA { get; set; } = null!;
    public string LinkTitleText { get; set; } = null!;
    public string? Page { get; set; }
    public string LinkToExternal { get; set; }
    public bool OpenInNewTab { get; set; }

    //public static LinkViewModel GetViewModel(Cta cta) => new LinkViewModel()
    //{
    //    CTAText = cta.CtaCallToAction,
    //    LinkTitleText = cta.CtaLinkTitleText,
    //    LinkToExternal = cta.CtaLinkToExternal,
    //};
}
