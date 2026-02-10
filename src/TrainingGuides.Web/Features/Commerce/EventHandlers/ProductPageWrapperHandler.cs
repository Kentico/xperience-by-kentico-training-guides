using CMS;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;

using Kentico.PageBuilder.Web.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using TrainingGuides.ProductStock;
using TrainingGuides.Web.Commerce.EventHandlers;
using TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;
using TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductWidget;
using TrainingGuides.Web.Features.Shared.Logging;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.Sections.General;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterModule(typeof(ProductPageWrapperHandler))]

namespace TrainingGuides.Web.Commerce.EventHandlers;

public class ProductPageWrapperHandler() : Module(MODULE_NAME)
{
    private const string ADMIN = "administrator";
    private const string STORE_PATH = "/Store";
    private const string CHANNEL_NAME = "TrainingGuidesPages";
    private const string WEB_CHANNEL_GUID = "FDBA40FE-1ECE-4821-9D57-EAA1D89E13B1";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private IWebPageManagerFactory webPageManagerFactory;
    private IWebPageManager webPageManager;
    private IContentItemRetrieverService contentItemRetrieverService;
    private IInfoProvider<UserInfo> userInfoProvider;
    private IInfoProvider<WebsiteChannelInfo> websiteChannelInfoProvider;
    private ILogger<ProductPageWrapperHandler> logger;

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public const string MODULE_NAME = "Product page wrapper handlers";

    // Contains initialization code that is executed when the application starts
    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit();

        webPageManagerFactory = parameters.Services.GetRequiredService<IWebPageManagerFactory>();
        contentItemRetrieverService = parameters.Services.GetRequiredService<IContentItemRetrieverService>();
        userInfoProvider = parameters.Services.GetRequiredService<IInfoProvider<UserInfo>>();
        websiteChannelInfoProvider = parameters.Services.GetRequiredService<IInfoProvider<WebsiteChannelInfo>>();
        logger = parameters.Services.GetRequiredService<ILogger<ProductPageWrapperHandler>>();

        var user = userInfoProvider.Get()
            .WhereEquals(nameof(UserInfo.UserName), ADMIN)
            .FirstOrDefault();

        var webChannel = websiteChannelInfoProvider.Get()
            .WhereEquals(nameof(WebsiteChannelInfo.WebsiteChannelGUID), new Guid(WEB_CHANNEL_GUID))
            .FirstOrDefault();

        webPageManager = webPageManagerFactory.Create(webChannel?.WebsiteChannelID ?? 0, user?.UserID ?? 0);

        // Assigns custom handlers to events
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
        ContentItemEvents.Create.After += ContentItem_Create_After;
        ContentItemEvents.CreateLanguageVariant.After += ContentItem_CreateLanguageVariant_After;
        ContentItemEvents.Delete.Execute += ContentItem_Delete_Execute;
        ContentItemEvents.Publish.Execute += ContentItem_Publish_Execute;
        ContentItemEvents.Unpublish.Execute += ContentItem_Unpublish_Execute;
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
    }

    private void ContentItem_Create_After(object sender, CreateContentItemEventArgs e)
    {
        if (e.ID is null || !IsApplicableType(e.ContentTypeName))
            return;

        // Create a web page wrapper for the newly created product
        if (e.GUID is not null)
        {
            _ = CreatePageWrapperForProduct(e.DisplayName, e.ContentLanguageName, e.ContentLanguageID, e.ContentTypeID, (Guid)e.GUID);
        }
    }

    private void ContentItem_CreateLanguageVariant_After(object sender, CreateContentItemLanguageVariantEventArgs e)
    {
        if (!IsApplicableType(e.ContentTypeName))
            return;

        _ = EnsureProductPageWrapperForLanguage(e.DisplayName, e.ContentLanguageName, e.ContentLanguageID, e.ContentTypeID, e.Guid);
    }

    private void ContentItem_Delete_Execute(object sender, DeleteContentItemEventArgs e)
    {
        if (!IsApplicableType(e.ContentTypeName))
            return;

        // If there is no existing page in the language being deleted, we don't need to create a new one, so we will not use the ensure method.
        var productPages = GetProductPagesForProduct(e.Guid, e.ContentLanguageName, e.ContentLanguageID);

        foreach (var productPage in productPages)
        {
            webPageManager.Delete(
                    new DeleteWebPageParameters(productPage.SystemFields.WebPageItemID, e.ContentLanguageName)
                    {
                        Permanently = true,
                    }).GetAwaiter().GetResult();
        }
    }

    private void ContentItem_Publish_Execute(object sender, PublishContentItemEventArgs e)
    {
        if (!IsApplicableType(e.ContentTypeName))
            return;

        var productPages = EnsureProductPageWrapperForLanguage(e.DisplayName, e.ContentLanguageName, e.ContentLanguageID, e.ContentTypeID, e.Guid);

        foreach (var productPage in productPages)
        {
            if (!webPageManager.TryPublish(productPage.SystemFields.WebPageItemID, e.ContentLanguageName).GetAwaiter().GetResult())
            {
                logger.LogError(EventIds.ProductWrapperPublishFailed,
                "Publish failed for product page with ID {WebPageItemID} for product content item ID {ContentItemID} in language {LanguageName}.",
                productPage.SystemFields.WebPageItemID,
                e.ID,
                e.ContentLanguageName);
            }
        }
    }

    private void ContentItem_Unpublish_Execute(object sender, UnpublishContentItemEventArgs e)
    {
        if (!IsApplicableType(e.ContentTypeName))
            return;

        var productPages = GetProductPagesForProduct(e.Guid, e.ContentLanguageName, e.ContentLanguageID);

        foreach (var productPage in productPages)
        {
            if (!webPageManager.TryUnpublish(productPage.SystemFields.WebPageItemID, e.ContentLanguageName).GetAwaiter().GetResult())
            {
                logger.LogError(EventIds.ProductWrapperUnpublishFailed,
                "Unpublish failed for product page with ID {WebPageItemID} for product content item ID {ContentItemID} in language {LanguageName}.",
                productPage.SystemFields.WebPageItemID,
                e.ID,
                e.ContentLanguageName);
            }
        }
    }

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
    /// Determines whether the specified content type name is applicable for page wrapper creation.
    /// </summary>
    /// <param name="contentTypeName">Name of the content type to evaluate</param>
    /// <returns>True if a page wrapper should be created for the specified content type</returns>
    private bool IsApplicableType(string contentTypeName)
    {
        var types = GetApplicableTypeNames();
        return types.Contains(contentTypeName);
    }


    private int CreatePageWrapperForProduct(string displayName, string languageName, int languageId, int contentTypeId, Guid contentItemGuid)
    {
        var itemData = new ContentItemData(new Dictionary<string, object>
        {
            { nameof(ProductPage.ProductPageProducts), new List<ContentItemReference>()
                { new() { Identifier = contentItemGuid } } },
        });

        var contentItemParameters = new ContentItemParameters(ProductPage.CONTENT_TYPE_NAME, itemData);

        var createPageParameters = new CreateWebPageParameters(displayName, languageName, contentItemParameters)
        {
            ParentWebPageItemID = EnsureParentPageInLanguage(contentTypeId, languageName, languageId)
        };

        createPageParameters.SetPageBuilderConfiguration(GetProductWidgetsConfiguration(), GetPageTemplateConfiguration());
        int id = webPageManager.Create(createPageParameters).GetAwaiter().GetResult();

        if (id <= 0)
        {
            logger.LogError(EventIds.ProductWrapperCreateFailed,
                "Page wrapper creation failed for product content item with GUID {ContentItemGuid} in language {LanguageName}.",
                contentItemGuid,
                languageName);
        }

        return id;
    }

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

    private JsonSerializerSettings GetSerializerSettings() =>
        new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Include,
        };

    private string GetPageTemplateConfiguration() =>
        "{\"identifier\":\"TrainingGuides.GeneralPageTemplate\",\"properties\":null,\"fieldIdentifiers\":null}";

    private void CreatePageWrapperLanguageVariantForProduct(string displayName, string languageName, Guid contentItemGuid, int existingPageId)
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

        if (!webPageManager.TryCreateLanguageVariant(createLanguageVariantParameters).GetAwaiter().GetResult())
        {
            logger.LogError(EventIds.ProductWrapperLanguageVariantCreateFailed,
                "Page wrapper language variant creation failed for product content item with GUID {ContentItemGuid} in language {LanguageName}.",
                contentItemGuid,
                languageName);
        }
    }

    private int EnsureParentPageInLanguage(int contentTypeId, string languageName, int languageId)
    {
        var contentType = DataClassInfoProvider.GetDataClassInfo(contentTypeId);
        var contentTypeGuid = contentType?.ClassGUID ?? Guid.Empty;

        // Get any language version of the parent page
        var existingParentPages = contentItemRetrieverService
            .RetrieveWebPageChildrenByPathWithoutContext(
                contentTypeNames: [StoreSection.CONTENT_TYPE_NAME],
                parentPagePath: STORE_PATH,
                customContentTypeQueryParameters: query => query,
                customContentQueryParameters: config => config
                    .Where(where => where.WhereContains(nameof(StoreSection.StoreSectionContentTypes), contentTypeGuid.ToString()))
                    .TopN(1),
                forPreview: true,
                includeSecuredItems: true,
                depth: 0,
                languageName: null,
                channelName: CHANNEL_NAME)
            .GetAwaiter().GetResult();

        if (!existingParentPages.Any())
        {
            // If there is no parent page for the product type, create one and return its ID
            return CreateParentPage(
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
                CreateParentPageLanguageVariant(
                    displayName: contentType?.ClassDisplayName ?? "Store section",
                    languageName: languageName,
                    contentTypeGuid: contentTypeGuid,
                    webPageItemID: existingParentPages.First().SystemFields.WebPageItemID);
            }

            // Return the existing parent page ID
            return existingParentPages.First().SystemFields.WebPageItemID;
        }
    }

    private int CreateParentPage(string displayName, string languageName, Guid contentTypeGuid)
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
            ParentWebPageItemID = GetStorePageId(languageName) ?? 0
        };

        createParentPageParameters.SetPageBuilderConfiguration(GetParentWidgetsConfiguration(), GetPageTemplateConfiguration());

        return webPageManager.Create(createParentPageParameters).GetAwaiter().GetResult();
    }

    private void CreateParentPageLanguageVariant(string displayName, string languageName, Guid contentTypeGuid, int webPageItemID)
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

        if (!webPageManager.TryCreateLanguageVariant(createLanguageVariantParameters).GetAwaiter().GetResult())
        {
            logger.LogError(EventIds.ProductParentPageLanguageVariantCreateFailed,
                "Parent page language variant creation failed for product content type with GUID {ContentTypeGuid} in language {LanguageName}.",
                contentTypeGuid,
                languageName);
        }
    }

    private int? GetStorePageId(string languageName) =>
        contentItemRetrieverService.RetrieveWebPageByPathWithoutContext<EmptyPage>(
                pathToMatch: STORE_PATH,
                includeSecuredItems: true,
                languageName: languageName,
                channelName: CHANNEL_NAME,
                forPreview: true)
            .GetAwaiter().GetResult()?.SystemFields.WebPageItemID;

    /// <summary>
    /// Retrieves all product pages that reference the specified product.
    /// </summary>
    /// <param name="guid">Guid of the product item being referenced</param>
    /// <param name="languageName">Language version to check</param>
    /// <param name="languageId">Language ID to check (must match the language name)</param>
    /// <returns></returns>
    private IEnumerable<IWebPageFieldsSource> GetProductPagesForProduct(Guid guid, string? languageName, int? languageId)
    {
        Func<ContentQueryParameters, ContentQueryParameters> customContentQueryParameters;

        Func<ContentQueryParameters, ContentQueryParameters> whereFilter = config => config
            // Normally we would use the .Linking(...) method in the customContentTypeQueryParameters function.
            // However, during the delete event, the linking data is already removed,
            // so we must manually check for the GUID in ProductPageProducts.
            .Where(where => where.WhereContains(nameof(ProductPage.ProductPageProducts), guid.ToString()));

        customContentQueryParameters = languageId is int intLanguageId
            ? (config => whereFilter(config
                // Filter by language ID to ensure we only get pages in the specific language, without falling back to other languages
                .Where(where => where.WhereEquals(
                    nameof(ProductPage.SystemFields.ContentItemCommonDataContentLanguageID), intLanguageId))))
            : whereFilter;

        return contentItemRetrieverService
                .RetrieveWebPageChildrenByPathWithoutContext(
                    contentTypeNames: [ProductPage.CONTENT_TYPE_NAME],
                    parentPagePath: STORE_PATH,
                    customContentTypeQueryParameters: query => query,
                    customContentQueryParameters: customContentQueryParameters,
                    forPreview: true,
                    includeSecuredItems: true,
                    depth: 0,
                    languageName: languageName,
                    channelName: CHANNEL_NAME)
                .GetAwaiter().GetResult();

    }

    /// <summary>
    /// Ensures that a product page exists for the specified product and language. If it doesn't exist, it will be created. If a page exists in another language, a language variant will be created for the current language.
    /// </summary>
    /// <param name="displayName">Display name for new variant if creation required</param>
    /// <param name="languageName">Language name of variant to retrieve</param>
    /// <param name="languageId">Language ID of variant to retrieve (must match language name)</param>
    /// <param name="contentTypeId">ID of the reusable product item's content type</param>
    /// <param name="contentItemGuid">Guid of the reusable content item</param>
    /// <returns></returns>
    private IEnumerable<IWebPageFieldsSource> EnsureProductPageWrapperForLanguage(string displayName, string languageName, int languageId, int contentTypeId, Guid contentItemGuid)
    {
        var langSpecificProductPages = GetProductPagesForProduct(contentItemGuid, languageName, languageId);

        if (langSpecificProductPages.Any())
        {
            return langSpecificProductPages;
        }

        var allLanguageProductPages = GetProductPagesForProduct(contentItemGuid, null, null);

        if (allLanguageProductPages.Any())
        {
            var uniquePageIDs = allLanguageProductPages.Select(page => page.SystemFields.WebPageItemID).Distinct();

            foreach (int pageId in uniquePageIDs)
            {
                // We should still make sure the parent exists in the current language, but we don't need its ID to create a new page
                _ = EnsureParentPageInLanguage(contentTypeId, languageName, languageId);

                CreatePageWrapperLanguageVariantForProduct(displayName, languageName, contentItemGuid, pageId);
            }

            return GetProductPagesForProduct(contentItemGuid, languageName, languageId);
        }

        _ = CreatePageWrapperForProduct(displayName, languageName, languageId, contentTypeId, contentItemGuid);

        return GetProductPagesForProduct(contentItemGuid, languageName, languageId);
    }
}