namespace TrainingGuides.Web.Features.Shared.Models;
public class LinkViewModel
{
    public string Name { get; set; } = null!;
    public string CTA { get; set; } = null!;
    public string LinkTitleText { get; set; } = null!;
    public string? Page { get; set; }
    public string? LinkToExternal { get; set; }
    public bool OpenInNewTab { get; set; }

    // Is the comment below left here on purpose for a concrete future need? If not, consider removing it.
    //public static LinkViewModel GetViewModel(Cta cta) => new LinkViewModel()
    //{
    //    CTAText = cta.CtaCallToAction,
    //    LinkTitleText = cta.CtaLinkTitleText,
    //    LinkToExternal = cta.CtaLinkToExternal,
    //};
}
