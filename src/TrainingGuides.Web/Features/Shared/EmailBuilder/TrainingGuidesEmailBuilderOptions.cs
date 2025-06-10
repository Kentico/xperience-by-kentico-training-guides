namespace TrainingGuides.Web.Features.Shared.EmailBuilder;

public class TrainingGuidesEmailBuilderOptions
{
    public IEnumerable<string> AllowedArticleContentTypes { get; set; } = Array.Empty<string>();
}