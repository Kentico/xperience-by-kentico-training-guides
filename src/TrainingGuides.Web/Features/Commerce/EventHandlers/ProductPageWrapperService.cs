using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;

using Kentico.PageBuilder.Web.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using TrainingGuides.ProductStock;
using TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;
using TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductWidget;
using TrainingGuides.Web.Features.Shared.Logging;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.Sections.General;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Commerce.EventHandlers;

// Shared service containing the page wrapper orchestration logic, injected into each handler
internal class ProductPageWrapperService
{
    // Username of an admin user
    private const string ADMIN = "administrator";

    // Path to the store section of the site
    private const string STORE_PATH = "/Store";

    // Name of the website channel
    private const string CHANNEL_NAME = "TrainingGuidesPages";

    // GUID of the website channel (Note this is the value of WebsiteChannelGUID, not ChannelGUID)
    private const string WEB_CHANNEL_GUID = "FDBA40FE-1ECE-4821-9D57-EAA1D89E13B1";

    private readonly IWebPageManager webPageManager;
    private readonly IContentItemRetrieverService contentItemRetrieverService;
    private readonly ILogger<ProductPageWrapperService> logger;

    public ProductPageWrapperService(
        IWebPageManagerFactory webPageManagerFactory,
        IContentItemRetrieverService contentItemRetrieverService,
        IInfoProvider<UserInfo> userInfoProvider,
        IInfoProvider<WebsiteChannelInfo> websiteChannelInfoProvider,
        ILogger<ProductPageWrapperService> logger)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.logger = logger;

        var user = userInfoProvider.Get()
            .WhereEquals(nameof(UserInfo.UserName), ADMIN)
            .FirstOrDefault();

        var webChannel = websiteChannelInfoProvider.Get()
            .WhereEquals(nameof(WebsiteChannelInfo.WebsiteChannelGUID), new Guid(WEB_CHANNEL_GUID))
            .FirstOrDefault();

        webPageManager = webPageManagerFactory.Create(webChannel?.WebsiteChannelID ?? 0, user?.UserID ?? 0);
    }

    /// <summary>
    /// Creates a page wrapper for the specified product content item in the specified language, and returns the ID of the page. Ensures the parent page exists.
    /// </summary>
    /// <param name="displayName">The display name of the product content item, used for naming the page wrapper.</param>
    /// <param name="languageName">The language for which to create the page wrapper.</param>
    /// <param name="languageId">The ID of the language for which to create the page wrapper.</param>
    /// <param name="contentTypeId">The content type ID of the product content item, used to find or create a parent page if necessary.</param>
    /// <param name="contentItemGuid">The GUID of the product content item, used to link the page wrapper to the product.</param>
    /// <returns>The ID of the created page.</returns>
    /// </summary>
    internal async Task<int> CreatePageWrapperForProduct(string displayName, string languageName, int languageId, int contentTypeId, Guid contentItemGuid)
    {
        var itemData = new ContentItemData(new Dictionary<string, object>
        {
            { nameof(ProductPage.ProductPageProducts), new List<ContentItemReference>()
                { new() { Identifier = contentItemGuid } } },
        });

        var contentItemParameters = new ContentItemParameters(ProductPage.CONTENT_TYPE_NAME, itemData);

        var createPageParameters = new CreateWebPageParameters(displayName, languageName, contentItemParameters)
        {
            ParentWebPageItemID = await EnsureParentPageInLanguage(contentTypeId, languageName, languageId)
        };

        createPageParameters.SetPageBuilderConfiguration(GetProductWidgetsConfiguration(), GetPageTemplateConfiguration());
        int id = await webPageManager.Create(createPageParameters);

        if (id <= 0)
        {
            logger.LogError(EventIds.ProductWrapperCreateFailed,
                "Page wrapper creation failed for product content item with GUID {ContentItemGuid} in language {LanguageName}.",
                contentItemGuid,
                languageName);
        }

        return id;
    }

    /// <summary>
    /// Ensures that a product page exists for the specified product and language. If it doesn't exist, it will be created. If a page exists in another language, a language variant will be created for the current language.
    /// </summary>
    /// <param name="displayName">Display name for new variant if creation required</param>
    /// <param name="languageName">Language name of variant to retrieve</param>
    /// <param name="languageId">Language ID of variant to retrieve (must match language name)</param>
    /// <param name="contentTypeId">ID of the reusable product item's content type</param>
    /// <param name="contentItemGuid">GUID of the reusable content item</param>
    /// <returns>An enumerable collection of wrapper pages linking the specified product</returns>
    internal async Task<IEnumerable<IWebPageFieldsSource>> EnsureProductPageWrapperForLanguage(string displayName, string languageName, int languageId, int contentTypeId, Guid contentItemGuid)
    {
        var langSpecificProductPages = await GetProductPagesForProduct(contentItemGuid, languageName, languageId);

        if (langSpecificProductPages.Any())
        {
            return langSpecificProductPages;
        }

        var allLanguageProductPages = await GetProductPagesForProduct(contentItemGuid, null, null);

        if (allLanguageProductPages.Any())
        {
            var uniquePageIds = allLanguageProductPages.Select(page => page.SystemFields.WebPageItemID).Distinct();

            foreach (int pageId in uniquePageIds)
            {
                // We should still make sure the parent exists in the current language, but we don't need its ID to create a new page
                _ = await EnsureParentPageInLanguage(contentTypeId, languageName, languageId);

                await CreatePageWrapperLanguageVariantForProduct(displayName, languageName, contentItemGuid, pageId);
            }

            return await GetProductPagesForProduct(contentItemGuid, languageName, languageId);
        }

        _ = await CreatePageWrapperForProduct(displayName, languageName, languageId, contentTypeId, contentItemGuid);

        return await GetProductPagesForProduct(contentItemGuid, languageName, languageId);
    }

    /// <summary>
    /// Publishes all page wrappers for a given product content item in a specific language.
    /// </summary>
    /// <param name="displayName">Display name of the product content item</param>
    /// <param name="languageName">Language name of the variant to publish</param>
    /// <param name="languageId">Language ID of the variant to publish (must match language name)</param>
    /// <param name="contentTypeId">Content type ID of the product content item</param>
    /// <param name="contentItemGuid">GUID of the product content item</param>
    /// <param name="contentItemId">ID of the product content item</param>
    internal async Task PublishProductPageWrappers(string displayName, string languageName, int languageId, int contentTypeId, Guid contentItemGuid, int contentItemId)
    {
        var productPages = await EnsureProductPageWrapperForLanguage(displayName, languageName, languageId, contentTypeId, contentItemGuid);

        foreach (var productPage in productPages)
        {
            bool published = productPage.SystemFields.ContentItemCommonDataVersionStatus is VersionStatus.Published;
            bool unpublished = productPage.SystemFields.ContentItemCommonDataVersionStatus is VersionStatus.Unpublished;

            // If the page is unpublished, try to create a draft.
            bool draftCreated = unpublished && await webPageManager.TryCreateDraft(productPage.SystemFields.WebPageItemID, languageName);

            // Try to publish the associated page wrapper IF it is NOT already published.
            if (!published && !await webPageManager.TryPublish(productPage.SystemFields.WebPageItemID, languageName))
            {
                string errorMessage = "Publish failed for product page with ID {WebPageItemID} for product content item ID {ContentItemID} in language {LanguageName}."
                    + ((unpublished && !draftCreated) ? " Draft creation failed for the unpublished page." : string.Empty);

                logger.LogError(EventIds.ProductWrapperPublishFailed,
                    errorMessage,
                    productPage.SystemFields.WebPageItemID,
                    contentItemId,
                    languageName);
            }
        }
    }

    /// <summary>
    /// Deletes all page wrappers for a given product content item in a specific language.
    /// </summary>
    /// <param name="guid">GUID of the reusable content item</param>
    /// <param name="languageName">Language name of the variant to delete</param>
    /// <param name="languageId">Language ID of the variant to delete (must match language name)</param>
    internal async Task DeleteProductPageWrappers(Guid guid, string? languageName, int? languageId)
    {
        var productPages = await GetProductPagesForProduct(guid, languageName, languageId);

        foreach (var productPage in productPages)
        {
            await webPageManager.Delete(
                new DeleteWebPageParameters(productPage.SystemFields.WebPageItemID, languageName)
                {
                    Permanently = true,
                });
        }
    }

    /// <summary>
    /// Unpublishes all page wrappers for a given product content item in a specific language.
    /// </summary>
    /// <param name="contentItemGuid">GUID of the product content item</param>
    /// <param name="languageName">Language name of the variant to unpublish</param>
    /// <param name="languageId">Language ID of the variant to unpublish (must match language name)</param>
    /// <param name="contentItemId">ID of the product content item</param>
    internal async Task UnpublishProductPageWrappers(Guid contentItemGuid, string languageName, int languageId, int contentItemId)
    {
        var productPages = await GetProductPagesForProduct(contentItemGuid, languageName, languageId);

        foreach (var productPage in productPages)
        {
            if (!await webPageManager.TryUnpublish(productPage.SystemFields.WebPageItemID, languageName))
            {
                logger.LogError(EventIds.ProductWrapperUnpublishFailed,
                    "Unpublish failed for product page with ID {WebPageItemID} for product content item ID {ContentItemID} in language {LanguageName}.",
                    productPage.SystemFields.WebPageItemID,
                    contentItemId,
                    languageName);
            }
        }
    }

    /// <summary>
    /// Checks if the specified content type is one that should have a product page wrapper.
    /// </summary>
    /// <param name="contentTypeName">The name of the content type to check.</param>
    /// <returns>True if the content type should have a product page wrapper; otherwise, false.</returns>
    internal bool IsApplicableType(string contentTypeName) =>
        GetApplicableTypeNames().Contains(contentTypeName);

    /// <summary>
    /// Retrieves the content type names of all product types that should have a page wrapper.
    /// This is determined by finding all classes that implement IProductSchema but not IProductVariantSchema, and retrieving the value of their CONTENT_TYPE_NAME constant.
    /// </summary>
    /// <returns>A list of content type names</returns>
    /// <remarks>
    /// Make sure the content type of the page wrapper does not meet the criteria defined in this method, otherwise it will cause an infinite loop of page creation.
    /// </remarks>
    private IEnumerable<string> GetApplicableTypeNames()
    {
        // We know this class is stored in the Entities project, so we can use it to access the assembly
        var entitiesAssembly = typeof(ProductAvailableStockInfo).Assembly;
        var types = entitiesAssembly
            .GetTypes()
            .Where(type =>
                type.IsClass
                && !type.IsAbstract
                && typeof(IProductSchema).IsAssignableFrom(type)
                && !typeof(IProductVariantSchema).IsAssignableFrom(type));

        return types.Select(type =>
        {
            var field = type.GetField("CONTENT_TYPE_NAME");
            if (field is not null)
            {
                // The field is a constant, so we don't need an instance to retrieve its value - we can use null instead
                return field.GetValue(null) as string;
            }
            // If there is no CONTENT_TYPE_NAME constant, we skip this type
            return null;
        })
        // Filter out the classes with no CONTENT_TYPE_NAME constant
        .Where(name => name is not null)!;
    }

    /// <summary>
    /// Creates Page Builder JSON for the product wrapper page, with a product widget configured to display the current product 
    /// </summary>
    /// <returns>JSON string representing the product page's Page Builder content</returns>
    private string GetProductWidgetsConfiguration()
    {
        var config = new EditableAreasConfiguration
        {
            EditableAreas =
            {
                new EditableAreaConfiguration
                {
                    Identifier = "areaMain",
                    Sections =
                    {
                        new SectionConfiguration
                        {
                            Identifier = Guid.NewGuid(),
                            TypeIdentifier = ComponentIdentifiers.Sections.GENERAL,
                            Properties = new GeneralSectionProperties
                            {
                                ColorScheme = ColorSchemeOption.TransparentDark.ToString(),
                                CornerStyle = CornerStyleOption.Round.ToString(),
                                ColumnLayout = ColumnLayoutOption.OneColumn.ToString(),
                            },
                            Zones =
                            {
                                new ZoneConfiguration
                                {
                                    Identifier = Guid.NewGuid(),
                                    Name = "zoneMain",
                                    Widgets =
                                    {
                                        new WidgetConfiguration
                                        {
                                            Identifier = Guid.NewGuid(),
                                            TypeIdentifier = ComponentIdentifiers.Widgets.PRODUCT,
                                            Variants =
                                            {
                                                new WidgetVariantConfiguration
                                                {
                                                    Identifier = Guid.NewGuid(),
                                                    Properties = new ProductWidgetProperties
                                                    {
                                                        DisplayCurrentPage = true,
                                                        ShowVariantSelection = true,
                                                        ShowVariantDetails = true,
                                                        ShowCallToAction = false,
                                                        CallToActionText = "View Product",
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        return JsonConvert.SerializeObject(config, Formatting.None, GetSerializerSettings());
    }

    /// <summary>
    /// Creates Page Builder JSON for the parent page of the product wrapper, with a product listing widget configured to display products that exist beneath it in the tree
    /// </summary>
    /// <returns>JSON string representing the parent page's Page Builder content</returns>
    private string GetParentWidgetsConfiguration()
    {
        var config = new EditableAreasConfiguration
        {
            EditableAreas =
            {
                new EditableAreaConfiguration
                {
                    Identifier = "areaMain",
                    Sections =
                    {
                        new SectionConfiguration
                        {
                            Identifier = Guid.NewGuid(),
                            TypeIdentifier = ComponentIdentifiers.Sections.GENERAL,
                            Properties = new GeneralSectionProperties
                            {
                                ColorScheme = ColorSchemeOption.TransparentDark.ToString(),
                                CornerStyle = CornerStyleOption.Round.ToString(),
                                ColumnLayout = ColumnLayoutOption.OneColumn.ToString(),
                            },
                            Zones =
                            {
                                new ZoneConfiguration
                                {
                                    Identifier = Guid.NewGuid(),
                                    Name = "zoneMain",
                                    Widgets =
                                    {
                                        new WidgetConfiguration
                                        {
                                            Identifier = Guid.NewGuid(),
                                            TypeIdentifier = ComponentIdentifiers.Widgets.PRODUCT_LISTING,
                                            Variants =
                                            {
                                                new WidgetVariantConfiguration
                                                {
                                                    Identifier = Guid.NewGuid(),
                                                    Properties = new ProductListingWidgetProperties
                                                    {
                                                        SecuredItemsDisplayMode = SecuredOption.PromptForLogin.ToString(),
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        return JsonConvert.SerializeObject(config, Formatting.None, GetSerializerSettings());
    }

    /// <summary>
    /// Creates JSON serializer settings for dealing with Page Builder JSON
    /// </summary>
    /// <returns>JSON serializer settings</returns>
    private JsonSerializerSettings GetSerializerSettings() =>
        new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Include,
        };

    /// <summary>
    /// Creates page template configuration JSON for the product wrapper page and its parent page, using the General template with default properties
    /// </summary>
    /// <returns>JSON string representing the page template configuration</returns>
    private string GetPageTemplateConfiguration() =>
        "{\"identifier\":\"TrainingGuides.GeneralPageTemplate\",\"properties\":null,\"fieldIdentifiers\":null}";

    /// <summary>
    /// Retrieves all product pages that reference the specified product.
    /// </summary>
    /// <param name="guid">GUID of the product item being referenced</param>
    /// <param name="languageName">Language version to check</param>
    /// <param name="languageId">Language ID to check (must match the language name)</param>
    /// <returns>A collection of product pages that reference the specified product</returns>
    private async Task<IEnumerable<IWebPageFieldsSource>> GetProductPagesForProduct(Guid guid, string? languageName, int? languageId)
    {
        Func<ContentQueryParameters, ContentQueryParameters> whereFilter = config => config
            // Normally we would use the .Linking(...) method in the customContentTypeQueryParameters function.
            // However, during the delete event, the linking data is already removed,
            // so we must manually check for the GUID in ProductPageProducts.
            .Where(where => where.WhereContains(nameof(ProductPage.ProductPageProducts), guid.ToString()));

        var customContentQueryParameters = languageId is int intLanguageId
            ? (config => whereFilter(config
                // Filter by language ID to ensure we only get pages in the specific language, without falling back to other languages
                .Where(where => where.WhereEquals(
                    nameof(ProductPage.SystemFields.ContentItemCommonDataContentLanguageID), intLanguageId))))
            : whereFilter;

        return await contentItemRetrieverService
            .RetrieveWebPageChildrenByPathWithoutContext(
                contentTypeNames: [ProductPage.CONTENT_TYPE_NAME],
                parentPagePath: STORE_PATH,
                customContentTypeQueryParameters: query => query,
                customContentQueryParameters: customContentQueryParameters,
                forPreview: true,
                includeSecuredItems: true,
                depth: 0,
                languageName: languageName,
                channelName: CHANNEL_NAME);
    }

    /// <summary>
    /// Ensures a parent page exists for the specified product content type in the current language
    /// </summary>
    /// <param name="contentTypeId">The ID of the product content type</param>
    /// <param name="languageName">The language in which to ensure the parent page exists</param>
    /// <param name="languageId">The ID of the language</param>
    /// <returns>The WebPageItemID of the parent page</returns>
    private async Task<int> EnsureParentPageInLanguage(int contentTypeId, string languageName, int languageId)
    {
        var contentType = DataClassInfoProvider.GetDataClassInfo(contentTypeId);
        var contentTypeGuid = contentType?.ClassGUID ?? Guid.Empty;

        // Get any language version of the parent page
        var existingParentPages = await contentItemRetrieverService
            .RetrieveWebPageChildrenByPathWithoutContext(
                contentTypeNames: [StoreSection.CONTENT_TYPE_NAME],
                parentPagePath: STORE_PATH,
                customContentTypeQueryParameters: query => query,
                customContentQueryParameters: config => config
                    // Filter store section pages that are assigned to the product's content type
                    .Where(where => where.WhereContains(nameof(StoreSection.StoreSectionContentTypes), contentTypeGuid.ToString()))
                    .TopN(1),
                forPreview: true,
                includeSecuredItems: true,
                depth: 0,
                languageName: null,
                channelName: CHANNEL_NAME);

        if (!existingParentPages.Any())
        {
            // If there is no parent page for the product type, create one and return its ID
            return await CreateParentPage(
                displayName: contentType?.ClassDisplayName ?? "Store section",
                languageName: languageName,
                contentTypeGuid: contentTypeGuid
            );
        }
        else
        {
            // If there are no language variants in the specified language, create one
            if (!existingParentPages
                .Where(page => page.SystemFields.ContentItemCommonDataContentLanguageID == languageId)
                .Any())
            {
                await CreateParentPageLanguageVariant(
                    displayName: contentType?.ClassDisplayName ?? "Store section",
                    languageName: languageName,
                    contentTypeGuid: contentTypeGuid,
                    webPageItemID: existingParentPages.First().SystemFields.WebPageItemID);
            }

            // Return the existing parent page ID
            return existingParentPages.First().SystemFields.WebPageItemID;
        }
    }

    /// <summary>
    /// Creates a parent page for a given product content type and language
    /// </summary>
    /// <param name="displayName">The display name of the parent page</param>
    /// <param name="languageName">The language in which to create the parent page</param>
    /// <param name="contentTypeGuid">The GUID of the product content type</param>
    /// <returns>The WebPageItemID of the created parent page</returns>
    private async Task<int> CreateParentPage(string displayName, string languageName, Guid contentTypeGuid)
    {
        var parentData = new ContentItemData(new Dictionary<string, object>
        {
            { nameof(StoreSection.StoreSectionContentTypes), new List<Guid>(){ contentTypeGuid } },
        });

        var parentContentItemParameters = new ContentItemParameters(StoreSection.CONTENT_TYPE_NAME, parentData);

        var createParentPageParameters = new CreateWebPageParameters(displayName,
            languageName,
            parentContentItemParameters)
        {
            ParentWebPageItemID = await GetStorePageId(languageName) ?? 0
        };

        createParentPageParameters.SetPageBuilderConfiguration(GetParentWidgetsConfiguration(), GetPageTemplateConfiguration());

        return await webPageManager.Create(createParentPageParameters);
    }

    /// <summary>
    /// Creates a language variant for an existing parent page.
    /// </summary>
    /// <param name="displayName">The display name of the parent page</param>
    /// <param name="languageName">The language in which to create the language variant</param>
    /// <param name="contentTypeGuid">The GUID of the product content type</param>
    /// <param name="webPageItemID">The WebPageItemID of the parent page</param>
    private async Task CreateParentPageLanguageVariant(string displayName, string languageName, Guid contentTypeGuid, int webPageItemID)
    {
        var parentData = new ContentItemData(new Dictionary<string, object>
        {
            { nameof(StoreSection.StoreSectionContentTypes), new List<Guid>(){ contentTypeGuid } },
        });
        var createLanguageVariantParameters =
            new CMS.Websites.CreateLanguageVariantParameters(webPageItemID,
                                                             languageName,
                                                             displayName,
                                                             parentData);

        createLanguageVariantParameters.SetPageBuilderConfiguration(GetParentWidgetsConfiguration(), GetPageTemplateConfiguration());

        if (!await webPageManager.TryCreateLanguageVariant(createLanguageVariantParameters))
        {
            logger.LogError(EventIds.ProductParentPageLanguageVariantCreateFailed,
                "Parent page language variant creation failed for product content type with GUID {ContentTypeGuid} in language {LanguageName}.",
                contentTypeGuid,
                languageName);
        }
    }

    /// <summary>
    /// Retrieves the page ID of the store's main page
    /// </summary>
    /// <param name="languageName">The language to query</param>
    /// <returns>The WebPageItemID of the store's main page, or null if not found</returns>
    private async Task<int?> GetStorePageId(string languageName)
    {
        var page = await contentItemRetrieverService.RetrieveWebPageByPathWithoutContext<EmptyPage>(
            pathToMatch: STORE_PATH,
            includeSecuredItems: true,
            languageName: languageName,
            channelName: CHANNEL_NAME,
            forPreview: true);

        return page?.SystemFields.WebPageItemID;
    }

    /// <summary>
    /// Creates a language variant of a product's page wrapper in the specified language
    /// </summary>
    /// <param name="displayName">The display name of the content item to apply to the page wrapper</param>
    /// <param name="languageName">The language in which to create the variant</param>
    /// <param name="contentItemGuid">The GUID of the content item to be referenced by the page wrapper</param>
    /// <param name="existingPageId">The WebPageItemID of the existing page</param>
    private async Task CreatePageWrapperLanguageVariantForProduct(string displayName, string languageName, Guid contentItemGuid, int existingPageId)
    {
        var itemData = new ContentItemData(new Dictionary<string, object>
        {
            { nameof(ProductPage.ProductPageProducts), new List<ContentItemReference>()
                { new() { Identifier = contentItemGuid } } },
        });

        var createLanguageVariantParameters =
            new CMS.Websites.CreateLanguageVariantParameters(existingPageId,
                                                             languageName,
                                                             displayName,
                                                             itemData);

        createLanguageVariantParameters.SetPageBuilderConfiguration(GetProductWidgetsConfiguration(), GetPageTemplateConfiguration());

        if (!await webPageManager.TryCreateLanguageVariant(createLanguageVariantParameters))
        {
            logger.LogError(EventIds.ProductWrapperLanguageVariantCreateFailed,
                "Page wrapper language variant creation failed for product content item with GUID {ContentItemGuid} in language {LanguageName}.",
                contentItemGuid,
                languageName);
        }
    }
}