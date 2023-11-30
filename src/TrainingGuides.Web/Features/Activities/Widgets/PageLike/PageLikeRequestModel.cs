namespace TrainingGuides.Web.Features.Activities.Widgets.PageLike;

/*
 * This class is only used in the Controller
 * I would recommend moving it to that file since it has tight coupling
 * with the Controller
 */
public class PageLikeRequestModel
{
    public string WebPageItemID { get; set; }

    public string ContentTypeName { get; set; }
}
