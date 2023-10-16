using CMS.DocumentEngine.Types.KBank;

namespace KBank.Web.Components.PageTemplates;

public class HeadingAndSubViewModel
{
    public string Heading { get; set; }
    public string Subheading { get; set; }

    public static HeadingAndSubViewModel GetViewModel(HeadingAndSub headingAndSub)
    {
        return new HeadingAndSubViewModel
        {
            Heading = headingAndSub.Heading,
            Subheading = headingAndSub.Subheading
        };
    }

}
