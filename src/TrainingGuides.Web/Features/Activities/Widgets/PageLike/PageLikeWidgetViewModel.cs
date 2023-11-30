namespace TrainingGuides.Web.Features.Activities.Widgets.PageLike;

/*
 * This class is only used in the View Component
 * I would recommend moving it to that file since it has tight coupling
 * with the View Component
 */
public class PageLikeWidgetViewModel
{
    public bool ShowLikeButton { get; set; }

    public int WebPageItemID { get; set; }

    public string ContentTypeName { get; set; }
}

