using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Membership;
using CMS.Websites.Routing;
using CMS.Workspaces;

namespace TrainingGuides.Web.OneTimeCode;

public class ArticleConverter
{
    private readonly IWebsiteChannelContext websiteChannelContext;
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IInfoProvider<WorkspaceInfo> workspaceInfoProvider;
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider;

    private readonly IWebPageManager webPageManager;
    private readonly IContentItemManager contentItemManager;

    public ArticleConverter(
        IInfoProvider<UserInfo> userInfoProvider,
        IContentItemManagerFactory contentItemManagerFactory,
        IWebsiteChannelContext websiteChannelContext,
        IWebPageManagerFactory webPageManagerFactory,
        IContentQueryExecutor contentQueryExecutor,
        IInfoProvider<WorkspaceInfo> workspaceInfoProvider,
        IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider)
    {
        this.websiteChannelContext = websiteChannelContext;
        this.contentQueryExecutor = contentQueryExecutor;
        this.workspaceInfoProvider = workspaceInfoProvider;
        this.contentLanguageInfoProvider = contentLanguageInfoProvider;

        var administrator = userInfoProvider.Get().WhereEquals(nameof(UserInfo.UserName), "administrator").FirstOrDefault() ?? new UserInfo();
        contentItemManager = contentItemManagerFactory.Create(administrator.UserID);
        webPageManager = webPageManagerFactory.Create(websiteChannelContext.WebsiteChannelID, administrator.UserID);
    }

    private async Task<IEnumerable<T>> RetrieveArticles<T>(
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter,
        string contentTypeName)
    {
        var builder = new ContentItemQueryBuilder()
            .ForContentType(
                contentTypeName,
                config => queryFilter(config)
                    .WithLinkedItems(3)
            );

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = false
        };

        var items = await contentQueryExecutor.GetMappedResult<T>(builder, queryExecutorOptions);

        return items;
    }

    private async Task<IEnumerable<ArticlePage>> RetrieveArticlePagesLinkingArticle(int oldArticleId)
    {
        var builder = new ContentItemQueryBuilder()
            .ForContentType(
                ArticlePage.CONTENT_TYPE_NAME,
                config => config
                    .Linking(nameof(ArticlePage.ArticlePageContent), [oldArticleId])
                    // If you run this code somewhere that the channel context is not available, you can hard-code this value.
                    .ForWebsite(websiteChannelContext.WebsiteChannelName)
                    .WithLinkedItems(3)
            );

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = false
        };

        var items = await contentQueryExecutor.GetMappedResult<ArticlePage>(builder, queryExecutorOptions);

        return items;
    }

    private string GetContentLanguageName(int contentLanguageId)
    {
        var contentLanguage = contentLanguageInfoProvider.Get(contentLanguageId);
        return contentLanguage.ContentLanguageName;
    }

    private async Task<string> GetWorkspaceName(int contentItemID)
    {
        var metadata = await contentItemManager.GetContentItemMetadata(contentItemID);

        string result = await workspaceInfoProvider.Get()
            .WhereEquals(nameof(WorkspaceInfo.WorkspaceID), metadata.WorkspaceId)
            .AsSingleColumn("WorkspaceName")
            .GetScalarResultAsync<string>();

        return result ?? "KenticoDefault";
    }

    private string GetNewReusableArticleName(Article oldArticle) => $"{oldArticle.SystemFields.ContentItemName}-schema";

    private async Task<string> GetArticleDisplayName(Article oldArticle, string languageName, ConversionAttempt attempt)
    {
        string displayName;

        try
        {
            displayName = (await contentItemManager.GetContentItemLanguageMetadata(oldArticle.SystemFields.ContentItemID, languageName)).DisplayName;
        }
        catch (Exception ex)
        {
            displayName = oldArticle.ArticleTitle;
            string newMessage = $" Failed to retrieve display name for [{oldArticle.ArticleTitle}] with ID [{oldArticle.SystemFields.ContentItemID}] in language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }

        return displayName;
    }

    private async Task<int> CreateNewReusableArticle(Article oldArticle, string languageName, ConversionAttempt attempt)
    {
        // Specify the content type and generic content item properties
        var createParams =
            new CreateContentItemParameters(
                contentTypeName: GeneralArticle.CONTENT_TYPE_NAME,
                name: GetNewReusableArticleName(oldArticle),
                displayName: await GetArticleDisplayName(oldArticle, languageName, attempt),
                languageName: languageName,
                workspaceName: await GetWorkspaceName(oldArticle.SystemFields.ContentItemID)
                );

        // Assemble the field values for a new GeneralArticle item
        var contentItemData = new ContentItemData(new Dictionary<string, object> {
            {nameof(GeneralArticle.ArticleSchemaTitle), oldArticle.ArticleTitle},
            {nameof(GeneralArticle.ArticleSchemaSummary), oldArticle.ArticleSummary},
            {nameof(GeneralArticle.ArticleSchemaTeaser), new List<ContentItemReference>(){
                    new(){ Identifier = oldArticle.ArticleTeaser.FirstOrDefault()?.SystemFields.ContentItemGUID ?? Guid.Empty }
            }},
            {nameof(GeneralArticle.ArticleSchemaText), oldArticle.ArticleText},
            {nameof(GeneralArticle.ArticleSchemaRelatedArticles), oldArticle
                .ArticleRelatedArticles.Select(oldRef => new ContentItemReference()
                {
                    Identifier = oldRef.SystemFields.ContentItemGUID
                })
                .ToList()
            }
        });

        int newId;

        try
        {
            // Create the new item, log any exceptions that occur
            newId = await contentItemManager.Create(createParams, contentItemData);

            if (newId > 0)
            {
                attempt.LogMessages.Add($"Created new article for [{oldArticle.ArticleTitle}] with new ID [{newId}] in language [{languageName}].");
                attempt.FinishedLanguagesReusable.Add(languageName);
            }
            else
            {
                // Log an exception if the ID is not valid
                throw new Exception($"Invalid ID value [{newId}].");
            }
        }
        catch (Exception ex)
        {
            newId = -1; // Indicate failure

            // Combine exception message with our own, if applicable
            string newMessage = $" Failed to create new article for [{oldArticle.ArticleTitle}] with ID [{oldArticle.SystemFields.ContentItemID}] in language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }

        return newId;
    }

    private async Task AddLanguageVersionOfReusableArticle(Article oldArticle, int publishableID, string languageName, ConversionAttempt attempt)
    {
        // Specify namespace to distinguish CreateLanguageVariantParameters from CMS.Websites version
        var languageVariantParams = new CMS.ContentEngine.CreateLanguageVariantParameters(
            publishableID,
            oldArticle.ArticleTitle,
            languageName);

        // Assemble the field values for a neW language version
        var contentItemData = new ContentItemData(new Dictionary<string, object> {
            {nameof(GeneralArticle.ArticleSchemaTitle), oldArticle.ArticleTitle},
            {nameof(GeneralArticle.ArticleSchemaSummary), oldArticle.ArticleSummary},
            {nameof(GeneralArticle.ArticleSchemaTeaser), new List<ContentItemReference>(){
                    new(){ Identifier = oldArticle.ArticleTeaser.FirstOrDefault()?.SystemFields.ContentItemGUID ?? Guid.Empty }
            }},
            {nameof(GeneralArticle.ArticleSchemaText), oldArticle.ArticleText},
            {nameof(GeneralArticle.ArticleSchemaRelatedArticles), oldArticle
                .ArticleRelatedArticles.Select(oldRef => new ContentItemReference()
                {
                    Identifier = oldRef.SystemFields.ContentItemGUID
                })
                .ToList()
            }
        });

        try
        {
            if (await contentItemManager.TryCreateLanguageVariant(languageVariantParams, contentItemData))
            {
                // Log success
                attempt.LogMessages.Add($"Added language version [{languageName}] for article [{oldArticle.ArticleTitle}] with ID [{publishableID}].");
                attempt.FinishedLanguagesReusable.Add(languageName);
            }
            else
            {
                // Log an error if TryCreateLanguageVariant returns false without throwing an exception
                throw new Exception();
            }
        }
        catch (Exception ex)
        {
            // Combine exception message with our own, if applicable
            string newMessage = $" Failed to add language version [{languageName}] for article [{oldArticle.ArticleTitle}] with ID [{publishableID}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }
    }

    private async Task TryScheduleReusableItem(DateTime? unpublishDate, IContentItemFieldsSource oldItem, int newItemId, string languageName, ConversionAttempt attempt)
    {
        try
        {
            // Check if the unpublish date is scheduled
            if (unpublishDate is DateTime unpDate && unpDate > DateTime.MinValue)
            {
                // Schedule unpublish
                await contentItemManager.ScheduleUnpublish(newItemId, languageName, unpDate);
                attempt.LogMessages.Add($"Scheduled unpublish for item [{oldItem.SystemFields.ContentItemName}] with ID [{newItemId}] in language [{languageName}] at [{unpublishDate}].");
            }
        }
        catch (Exception ex)
        {
            // Combine exception message with our own, if applicable
            string newMessage = $" Failed to schedule unpublish for reusable article [{oldItem.SystemFields.ContentItemName}] with ID [{newItemId}] in language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }
    }

    private async Task<DateTime?> GetReusableUnpublishTimeIfScheduled(IContentItemFieldsSource item, string languageName, ConversionAttempt conversionAttempt)
    {
        try
        {
            // Check if the unpublish date is scheduled
            if (await contentItemManager.IsUnpublishScheduled(item.SystemFields.ContentItemID, languageName))
            {
                // If the manager says the item is scheduled for unpublish but there's no date, use the maximum possible date, keeping the item published
                return (await contentItemManager.GetContentItemLanguageMetadata(item.SystemFields.ContentItemID, languageName)).ScheduledUnpublishWhen;
            }
        }
        catch (Exception e)
        {
            string newMessage = $" Failed to check or retrieve unpublish date for item [{item.SystemFields.ContentItemName}] with ID [{item.SystemFields.ContentItemID}] in language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(e.Message) ? $" Error: {e.Message}" : string.Empty);
            conversionAttempt.Exceptions.Add(new Exception(newMessage));
        }

        return null;
    }

    private async Task PublishReusableArticle(Article oldArticle, int publishableID, string languageName, ConversionAttempt attempt)
    {
        try
        {
            // Try to publish the new article
            if (await contentItemManager.TryPublish(publishableID, languageName))
            {
                // Check if the old article was scheduled for unpublish,
                var unpublishDate = await GetReusableUnpublishTimeIfScheduled(oldArticle, languageName, attempt);

                // Schedule for unpublish if applicable
                await TryScheduleReusableItem(unpublishDate, oldArticle, publishableID, languageName, attempt);
            }
            else
            {
                // Log an error if TryPublish returns false without throwing an exception
                throw new Exception();
            }
        }
        catch (Exception ex)
        {
            // Combine exception message with our own, if applicable
            string newMessage = $" Failed to publish article [{oldArticle.ArticleTitle}] with ID [{publishableID}] in language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            attempt.Exceptions.Add(new Exception(newMessage));
        }
    }

    private async Task<List<ConversionAttempt>> ConvertReusableArticles(IEnumerable<Article> oldReusableArticles)
    {
        List<ConversionAttempt> result = [];

        int schemaArticleId;

        foreach (var publishedOldArticle in oldReusableArticles)
        {
            ConversionAttempt currentAttempt;

            // Find existing attempts for the same item.
            // We are working with multiple language versions here, so there may be multiple entries for items with the same ContentItemID.
            var previousAttemptsForSameItem = result.Where(attempt => attempt.OldArticle.SystemFields.ContentItemID == publishedOldArticle.SystemFields.ContentItemID);

            // Filter previous attempts with valid IDs
            var previousAttemptsForSameItemWithValidIds = previousAttemptsForSameItem.Where(attempt => attempt.NewArticleContentItemId is not null and > 0);

            string languageName = GetContentLanguageName(publishedOldArticle.SystemFields.ContentItemCommonDataContentLanguageID);

            // If there are no previous attempts that contain a valid ID
            if (!previousAttemptsForSameItemWithValidIds.Any())
            {
                // If there is already an attempt for the same old article, but it does not have a new article ID greater than 0, this will reuse it.
                currentAttempt = previousAttemptsForSameItem.FirstOrDefault()
                    ?? new ConversionAttempt(publishedOldArticle, null);

                // Create a new article
                schemaArticleId = await CreateNewReusableArticle(publishedOldArticle, languageName, currentAttempt);

                // Log the new article ID in the attempt
                currentAttempt.NewArticleContentItemId = schemaArticleId > 0 ? schemaArticleId : null;

                result.Add(currentAttempt);
            }
            else
            {
                // This should not be null, thanks to the condition of the if statement.
                currentAttempt = previousAttemptsForSameItemWithValidIds.FirstOrDefault() ?? new ConversionAttempt(publishedOldArticle, null);

                // Get the new article ID from the current attempt. This should not be null or 0 thanks to the condition of the if statement.
                schemaArticleId = currentAttempt.NewArticleContentItemId ?? 0;

                // If a version in this language already exists, skip creating a new one.
                if (previousAttemptsForSameItem.Any(attempt => attempt.FinishedLanguagesReusable.Contains(languageName)))
                {
                    // Log an exception, as there is a logical error leading us to process the same language version multiple times.
                    currentAttempt.Exceptions.Add(new Exception($"Skipped creating new language version for [{publishedOldArticle.ArticleTitle}] with ID [{publishedOldArticle.SystemFields.ContentItemID}] in language [{languageName}] because a version in this language already exists."));
                }

                else
                {
                    // If no version exists in this language, add a new language version.
                    await AddLanguageVersionOfReusableArticle(publishedOldArticle, schemaArticleId, languageName, currentAttempt);
                }
            }

            await PublishReusableArticle(publishedOldArticle, schemaArticleId, languageName, currentAttempt);

            // Ensure the current attempt is in the result list.
            if (!result.Contains(currentAttempt))
                result.Add(currentAttempt);
        }
        return result;
    }

    private async Task<List<ConversionAttempt>> UpdateRelatedArticlesInNewArticles(List<ConversionAttempt> conversionAttempts)
    {
        // Retrieve all of our newly created schema articles
        var schemaArticles = await RetrieveArticles<GeneralArticle>(
            query => query
                .Where(where => where.WhereIn(nameof(GeneralArticle.SystemFields.ContentItemID), conversionAttempts
                    .Where(attempt => attempt.NewArticleContentItemId is not null and > 0)
                    .Select(attempt => attempt.NewArticleContentItemId!.Value)))
                .WithLinkedItems(3),
            GeneralArticle.CONTENT_TYPE_NAME);

        // We want to skip this logic for schema articles that aren't related to old articles
        var applicableSchemaArticles = schemaArticles
            .Where(article => article.ArticleSchemaRelatedArticles.Any(relatedArticle => relatedArticle is Article));

        foreach (var schemaArticle in applicableSchemaArticles)
        {
            // Find the corresponding conversion attempt
            var conversionAttempt = conversionAttempts.FirstOrDefault(attempt => attempt.NewArticleContentItemId == schemaArticle.SystemFields.ContentItemID);

            if (conversionAttempt is null)
            {
                // If no conversion attempt is found, create a new one, then log an exception in it
                var oldArticle = new Article()
                {
                    ArticleTitle = $"Error"
                };
                conversionAttempt = new ConversionAttempt(oldArticle, schemaArticle.SystemFields.ContentItemID);

                conversionAttempt.Exceptions.Add(new Exception($"Could not find conversion attempt for new article [{schemaArticle.ArticleSchemaTitle}] with ID [{schemaArticle.SystemFields.ContentItemID}]"));

                conversionAttempts.Add(conversionAttempt);
            }

            string languageName = GetContentLanguageName(schemaArticle.SystemFields.ContentItemCommonDataContentLanguageID);

            // Update the related articles field of the newly created schema articles, so that they point to new articles instead of old ones.
            await UpdateReusableItemRelatedArticles(schemaArticle, languageName, conversionAttempt);
        }

        return conversionAttempts;
    }
    private async Task UpdateReusableItemRelatedArticles(GeneralArticle article, string languageName, ConversionAttempt conversionAttempt)
    {
        // Before we create a new draft, check if the existing article has a scheduled unpublish date
        var unpublishDate = await GetReusableUnpublishTimeIfScheduled(article, languageName, conversionAttempt);

        // Create a list of ContentItemReference objects for the updated related articles
        var relatedArticles = await BuildUpdatedRelatedArticlesList(article, languageName, conversionAttempt);

        // Assemble updated ContentItemData
        var contentItemData = new ContentItemData(new Dictionary<string, object> {
            {nameof(GeneralArticle.ArticleSchemaTitle), article.ArticleSchemaTitle},
            {nameof(GeneralArticle.ArticleSchemaSummary), article.ArticleSchemaSummary},
            {nameof(GeneralArticle.ArticleSchemaTeaser), new List<ContentItemReference>(){
                    new(){ Identifier = article.ArticleSchemaTeaser.FirstOrDefault()?.SystemFields.ContentItemGUID ?? Guid.Empty }
            }},
            {nameof(GeneralArticle.ArticleSchemaText), article.ArticleSchemaText},
            {nameof(GeneralArticle.ArticleSchemaRelatedArticles), relatedArticles}
        });

        bool newCreated = false;

        try
        {
            newCreated = await contentItemManager.TryCreateDraft(article.SystemFields.ContentItemID, languageName);
        }
        catch (Exception ex)
        {
            string newMessage = $" Failed to create draft for schema article [{article.ArticleSchemaTitle}] with ID [{article.SystemFields.ContentItemID}] in language [{languageName}] to update its related articles."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            conversionAttempt.Exceptions.Add(new Exception(newMessage));
        }

        // Update draft with new reference
        bool updated = await UpdateGeneralArticleRelatedArticlesDraft(article, contentItemData, languageName, conversionAttempt, unpublishDate);

        // If we could not create a new draft or update an existing draft, log an exception
        if (!newCreated && !updated)
        {
            conversionAttempt.Exceptions.Add(new Exception($"Could not create draft or update existing draft for schema article [{article.ArticleSchemaTitle}] with ID [{article.SystemFields.ContentItemID}] in language [{languageName}] to update its related articles."));
        }
    }

    private async Task<bool> UpdateGeneralArticleRelatedArticlesDraft(GeneralArticle article, ContentItemData contentItemData, string languageName, ConversionAttempt conversionAttempt, DateTime? unpublishDate)
    {
        try
        {
            if (await contentItemManager.TryUpdateDraft(article.SystemFields.ContentItemID, languageName, contentItemData))
            {
                if (await contentItemManager.TryPublish(article.SystemFields.ContentItemID, languageName))
                {
                    //success if trying to publish
                    conversionAttempt.LogMessages.Add($"The draft of schema article [{article.ArticleSchemaTitle}] with ID [{article.SystemFields.ContentItemID}] in language [{languageName}] was updated with new related articles and published successfully.");

                    await TryScheduleReusableItem(unpublishDate, article, article.SystemFields.ContentItemID, languageName, conversionAttempt);

                    return true;
                }
                // Update draft succeeded, but publish failed
                conversionAttempt.Exceptions.Add(new Exception($"The draft of reusable article [{article.ArticleSchemaTitle}] with ID [{article.SystemFields.ContentItemID}] in language [{languageName}] was updated with new related articles, but it failed to publish."));
                return false;
            }
        }
        catch (Exception ex)
        {
            // Update draft threw an exception
            conversionAttempt.Exceptions.Add(new Exception($"Updating the draft of schema article [{article.ArticleSchemaTitle}] with ID [{article.SystemFields.ContentItemID}] in language [{languageName}] to update related articles failed with an exception: {ex.Message}"));
            return false;
        }
        // Update draft failed
        conversionAttempt.Exceptions.Add(new Exception($"Draft of schema article [{article.ArticleSchemaTitle}] with ID [{article.SystemFields.ContentItemID}] in language [{languageName}] could not be updated with the new related articles."));
        return false;
    }

    private async Task<List<ContentItemReference>> BuildUpdatedRelatedArticlesList(GeneralArticle article, string languageName, ConversionAttempt conversionAttempt)
    {
        List<ContentItemReference> relatedArticles = [];

        foreach (var relatedArticle in article.ArticleSchemaRelatedArticles)
        {
            if (relatedArticle is Article oldArticle)
            {
                var newArticleGuid = await GetNewArticleItemGuid(oldArticle);

                if (newArticleGuid is Guid newGuid)
                {
                    relatedArticles.Add(new ContentItemReference() { Identifier = newGuid });
                }
                else
                {
                    // Log an exception if the new GUID could not be found
                    conversionAttempt.Exceptions.Add(new Exception($" Failed to find new article for related article [{oldArticle.ArticleTitle}] with ID [{oldArticle.SystemFields.ContentItemID}] in language [{languageName}]."));
                }
            }
            else
            {
                // If the related article is not an Article, keep the existing reference
                relatedArticles.Add(new ContentItemReference() { Identifier = relatedArticle.SystemFields.ContentItemGUID });
            }
        }

        return relatedArticles;
    }

    // Get the guid for the GeneralArticle corresponding to the provided Article
    private async Task<Guid?> GetNewArticleItemGuid(Article oldArticle)
    {
        // Use the same method that generated the codename of the new article.
        string newArticleCodeName = GetNewReusableArticleName(oldArticle);

        // Retrieve the new article based on the generated codename
        var newArticle = (await RetrieveArticles<GeneralArticle>(
            query => query
                .Where(where => where.WhereEquals(nameof(Article.SystemFields.ContentItemName), newArticleCodeName))
                .TopN(1),
            GeneralArticle.CONTENT_TYPE_NAME)).FirstOrDefault();

        // Return the guid of the new article, or null if not found
        return newArticle?.SystemFields.ContentItemGUID;
    }

    // Return the guid for the schema article with the provided ID
    private async Task<Guid?> GetNewArticleItemGuid(int? articleItemId)
    {
        if (articleItemId is null)
        {
            return null;
        }

        var articleQuery = await RetrieveArticles<GeneralArticle>(
            query => query
                .Where(where => where.WhereEquals(nameof(Article.SystemFields.ContentItemID), articleItemId))
                .TopN(1),
            GeneralArticle.CONTENT_TYPE_NAME);

        var article = articleQuery.FirstOrDefault();

        if (article is not null)
        {
            return article.SystemFields.ContentItemGUID;
        }
        else
        {
            return null;
        }
    }

    private async Task TrySchedulePage(DateTime? unpublishDate, ArticlePage page, string languageName, ConversionAttempt conversionAttempt)
    {
        try
        {
            // Check if the unpublish date is scheduled
            if (unpublishDate is DateTime unpDate && unpDate > DateTime.MinValue)
            {
                // Schedule unpublish
                await webPageManager.ScheduleUnpublish(page.SystemFields.WebPageItemID, languageName, unpDate);
                conversionAttempt.LogMessages.Add($"Scheduled unpublish for page [{page.SystemFields.WebPageItemTreePath}] with ID [{page.SystemFields.ContentItemID}] in language [{languageName}] at [{unpublishDate}].");
            }
        }
        catch (Exception e)
        {
            string newMessage = $" Failed to schedule unpublish for page [{page.SystemFields.WebPageItemTreePath}] with ID [{page.SystemFields.ContentItemID}] in language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(e.Message) ? $" Error: {e.Message}" : string.Empty);
            conversionAttempt.Exceptions.Add(new Exception(newMessage));
        }
    }

    private async Task<bool> UpdatePageDraft(ArticlePage page, string languageName, ConversionAttempt conversionAttempt, DateTime? unpublishDate)
    {
        // get Guid of new schema article from conversion attempt
        var schemaArticleGuid = await GetNewArticleItemGuid(conversionAttempt.NewArticleContentItemId);

        if (schemaArticleGuid is Guid newItemGuid)
        {
            // Update the reference based on the conversion attempt
            var contentItemData = new ContentItemData(new Dictionary<string, object>{
                // Clear out the old references to avoid confusion
                { nameof(ArticlePage.ArticlePageContent), new List<ContentItemReference>()},
                // Add the new reference
                { nameof(ArticlePage.ArticlePageArticleContent), new List<ContentItemReference>(){
                    new(){ Identifier = newItemGuid }
                }},
                {nameof(ArticlePage.ArticlePagePublishDate), page.ArticlePagePublishDate}
            });

            try
            {
                if (await webPageManager.TryUpdateDraft(page.SystemFields.WebPageItemID, languageName, new UpdateDraftData(contentItemData)))
                {
                    if (await webPageManager.TryPublish(page.SystemFields.WebPageItemID, languageName))
                    {
                        //success if trying to publish
                        conversionAttempt.LogMessages.Add($"The draft of page [{page.SystemFields.WebPageItemTreePath}] with ID [{page.SystemFields.ContentItemID}] in language [{languageName}] was updated with a new reference and published successfully.");

                        await TrySchedulePage(unpublishDate, page, languageName, conversionAttempt);
                        return true;
                    }
                    // Update draft succeeded, but publish failed
                    conversionAttempt.Exceptions.Add(new Exception($"The draft of page [{page.SystemFields.WebPageItemTreePath}] with ID [{page.SystemFields.ContentItemID}] in language [{languageName}] was updated with a new reference, but it failed to publish."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Update draft threw an exception
                conversionAttempt.Exceptions.Add(new Exception($"Updating the draft of page [{page.SystemFields.WebPageItemTreePath}] with ID [{page.SystemFields.ContentItemID}] in language [{languageName}] failed with an exception: {ex.Message}"));
                return false;
            }
            // Update draft failed
            conversionAttempt.Exceptions.Add(new Exception($"Draft of page [{page.SystemFields.WebPageItemTreePath}] with ID [{page.SystemFields.ContentItemID}] in language [{languageName}] could not be updated with the new reference."));
            return false;
        }
        else
        {
            // Failed to retrieve content item guid based on ID - can't find the new article
            conversionAttempt.Exceptions.Add(new Exception($"The new article with ID [{conversionAttempt.NewArticleContentItemId}] could not be found in language [{languageName}] to update the page reference."));
            return false;
        }
    }

    private async Task<DateTime?> GetPageUnpublishTimeIfScheduled(ArticlePage page, string languageName, ConversionAttempt conversionAttempt)
    {
        try
        {
            // Check if the unpublish date is scheduled
            if (await webPageManager.IsUnpublishScheduled(page.SystemFields.WebPageItemID, languageName))
            {
                // If the manager says the item is scheduled for unpublish but there's no date, use the maximum possible date, keeping the item published
                return (await webPageManager.GetContentItemLanguageMetadata(page.SystemFields.WebPageItemID, languageName)).ScheduledUnpublishWhen;
            }
        }
        catch (Exception e)
        {
            string newMessage = $" Failed to check or retrieve unpublish date for page [{page.SystemFields.WebPageItemTreePath}] with ID [{page.SystemFields.ContentItemID}] in language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(e.Message) ? $" Error: {e.Message}" : string.Empty);
            conversionAttempt.Exceptions.Add(new Exception(newMessage));
        }

        return null;
    }

    private async Task CreateAndOrUpdatePageDraft(ArticlePage page, string languageName, ConversionAttempt conversionAttempt)
    {
        // Before we create a new draft, check if the existing page has a scheduled unpublish date.
        var unpublishDate = await GetPageUnpublishTimeIfScheduled(page, languageName, conversionAttempt);

        // create a new draft with the same properties, or update the existing draft if it exists
        // This may be false if the published page already has a draft, in which case we will try to update the existing draft.
        bool newCreated = false;

        try
        {
            newCreated = await webPageManager.TryCreateDraft(page.SystemFields.WebPageItemID, languageName);
        }
        catch (Exception ex)
        {
            // Exception thrown when creating new draft
            string newMessage = $" Failed to create a new draft for the page [{page.SystemFields.WebPageItemTreePath}] with ID [{page.SystemFields.ContentItemID}] in language [{languageName}]."
                + (!string.IsNullOrWhiteSpace(ex.Message) ? $" Error: {ex.Message}" : string.Empty);
            conversionAttempt.Exceptions.Add(new Exception(newMessage));
        }

        // Update draft with new reference
        bool updated = await UpdatePageDraft(page, languageName, conversionAttempt, unpublishDate);

        // If we could not create a new draft or update an existing draft, log an exception
        if (!(updated || newCreated))
        {
            conversionAttempt.Exceptions.Add(new Exception($"A new draft could not be created for the page [{page.SystemFields.WebPageItemTreePath}] with ID [{page.SystemFields.ContentItemID}] in language [{languageName}], and an existing draft could not be updated with a new reference."));
        }
    }

    private async Task<List<ConversionAttempt>> UpdatePageReferences(List<ConversionAttempt> conversionAttempts)
    {
        // Focus on attempts with a valid GeneralArticle ID
        foreach (var conversionAttempt in conversionAttempts.Where(attempt => attempt.NewArticleContentItemId is not null and > 0))
        {
            // Retrieve all published pages linking to the old article. This includes all languages, as the content item ID is shared between languages.
            var publishedPages = await RetrieveArticlePagesLinkingArticle(conversionAttempt.OldArticle.SystemFields.ContentItemID);

            // Iterate through the language versions of the page
            foreach (var page in publishedPages)
            {
                // Get the language name from the ID
                string languageName = GetContentLanguageName(page.SystemFields.ContentItemCommonDataContentLanguageID);

                // Try to create a new draft and update the reference. Use the existing draft if it exists.
                await CreateAndOrUpdatePageDraft(page, languageName, conversionAttempt);
            }
        }
        return conversionAttempts;
    }

    public async Task<List<ConversionAttempt>> Convert()
    {
        var oldArticles = await RetrieveArticles<Article>(para => para, Article.CONTENT_TYPE_NAME);

        // Create new schema-based GeneralArticle items from old Article items
        var reusableAttempts = await ConvertReusableArticles(oldArticles);

        // Update pages that reference old Article items to reference new GeneralArticle items
        var updatedAttempts = await UpdatePageReferences(reusableAttempts);

        // Update related articles in newly created GeneralArticle items to reference other new GeneralArticle items
        return await UpdateRelatedArticlesInNewArticles(updatedAttempts);
    }

}

public class ConversionAttempt
{
    public Article OldArticle { get; set; } = new();
    public int? NewArticleContentItemId { get; set; }
    public List<Exception> Exceptions { get; set; }
    public List<string> LogMessages { get; set; }

    public List<string> FinishedLanguagesReusable { get; set; } = [];
    public List<string> FinishedLanguagesPage { get; set; } = [];

    public ConversionAttempt(
        Article oldArticle,
        int? newContentItemPublishedId,
        Exception? exception = null,
        string? logMessage = null)
    {
        OldArticle = oldArticle;
        NewArticleContentItemId = newContentItemPublishedId;
        Exceptions = exception is null ? [] : [exception];
        LogMessages = logMessage is null ? [] : [logMessage];
    }
}