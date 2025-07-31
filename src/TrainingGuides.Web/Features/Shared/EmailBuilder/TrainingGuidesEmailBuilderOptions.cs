namespace TrainingGuides.Web.Features.Shared.EmailBuilder;

public class TrainingGuidesEmailBuilderOptions
{
    public IEnumerable<string> AllowedArticleContentTypes { get; set; } = [];

    public IEnumerable<string> AllowedGeneralEmailTemplateContentTypes { get; set; } = [];
}