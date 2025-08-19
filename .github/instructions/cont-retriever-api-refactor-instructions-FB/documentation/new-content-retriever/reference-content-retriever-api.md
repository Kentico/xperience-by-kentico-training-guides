---
    title: Reference - ContentRetriever API
    identifier: reference_content_retriever_api_xp
    order: 400
    persona: developer, architect
    license: 1

    redirect_from:
        - x/reference_content_retriever_experimental_xp
        - x/reference_content_retriever_api_xp
        - documentation/developers-and-admins/development/content-retrieval/reference-content-retriever-experimental

    toc:
        minHeadingLevel: 2
        maxHeadingLevel: 3

    related_pages:
        - content_retriever_api_xp
        - 4YbWCQ
---

## Page queries

### Retrieve the current page

**Method:** `RetrieveCurrentPage<TResult>(...)`

Retrieves the page requested by the current HTTP request. The method maps the retrieved data to the specified `TResult` model.

{% code lang=csharp title="Basic usage" %}

// Assuming 'ArticlePage' is a generated model class
// For instance, assume the request is handled by 'ArticlesController'
// so 'TResult' always matches the retrieved schema and is mapped correctly.
ArticlePage currentPage =
  await contentRetriever.RetrieveCurrentPage<ArticlePage>();

// If the current page can be of multiple types, you can use 'IWebPageFieldsSource'
// or different suitable base class together with pattern matching.
// Suitable model classes are paired with content types using
// 'RegisterContentTypeMappingAttribute' (automatically added to generated classes)
//
// Gets the current page using a shared interface
var currentPageData =
  await contentRetriever.RetrieveCurrentPage<IWebPageFieldsSource>();

// Uses pattern matching to handle the specific type
switch (currentPageData)
{
  case ArticlePage article:
    // Handles specific logic for ArticlePage
    Console.WriteLine($"Displaying Article: {article.Title}");
    // Renders article view...
    break;

  case LandingPage landing:
    // Handles specific logic for LandingPage
    Console.WriteLine($"Displaying Landing Page: {landing.Headline}");
    // Renders landing page view...
    break;

  case null:
    // Handles case where the page is not found or not retrievable
    Console.WriteLine("Current page not found.");
    // Renders 404 view...
    break;

  default:
    // Handles any other unexpected page types
    // Renders a default view...
    break;
}

{% endcode %}

{% code lang=csharp title="With parameters" %}

// Overrides method defaults with custom parameters
var parameters = new RetrieveCurrentPageParameters
{
    // Forces a language variant
    LanguageName = "spanish",
    // Includes directly linked items
    LinkedItemsMaxLevel = 1,
    // Forces preview mode data
    IsForPreview = true
};

ArticlePage page = await contentRetriever.RetrieveCurrentPage<ArticlePage>(parameters);

{% endcode %}

{% code lang=csharp title="All configuration options" %}

// Disables caching
var cacheSettings = RetrievalCacheSettings.CacheDisabled;

ArticlePage page = await contentRetriever.RetrieveCurrentPage<ArticlePage>(
    // Uses the default retrieval params
    new RetrieveCurrentPageParameters(),
    // Only gets the ArticleTitle property
    query => query.Columns(nameof(ArticlePage.ArticleTitle)),
    cacheSettings,
    // Explicitly uses default mapping
    configureModel: null 
);

{% endcode %}

Some overloads of `RetrieveCurrentPage` require an instance of `RetrieveCurrentPageParameters`. This object allows you to fine-tune how the current page is retrieved. For available parameters, see the {% inpage_link "Page query parameters" %} table.

### Retrieve pages sharing reusable field schema

Method: `RetrievePagesOfReusableSchemas<TResult>(...)`

Retrieves a collection of pages whose content types use one or more of the specified {% page_link D4_OD linkText="reusable field schemas" %}, mapping them to the `TResult` model. Allows filtering by path, language, and other criteria using the parameters described in the {% inpage_link "Page query parameters" %} table.

{% code lang=csharp title="Basic usage" %}

// Assuming 'IPageMetadata' is an interface implemented by types using these schemas
var schemaNames = new[] { IMetadataFields.REUSABLE_FIELD_SCHEMA_NAME, ISeoFields.REUSABLE_FIELD_SCHEMA_NAME };

// Gets all pages using the specified schemas with default settings
IEnumerable<IPageMetadata> pagesWithSchemas =
  await contentRetriever.RetrievePagesOfReusableSchemas<IPageMetadata>(schemaNames);

{% endcode %}

{% code lang=csharp title="With parameters" %}

var schemaNames = new[] { IMetadataFields.REUSABLE_FIELD_SCHEMA_NAME };

// Gets pages using the 'Metadata' schema under the /products path
var parameters = new RetrievePagesOfReusableSchemasParameters
{
    PathMatch = PathMatch.Children("/products")
};

IEnumerable<IPageMetadata> productPages =
  await contentRetriever.RetrievePagesOfReusableSchemas<IPageMetadata>(schemaNames, parameters);

{% endcode %}

{% code lang=csharp title="All configuration options" %}

var schemaNames = new[] { IMetadataFields.REUSABLE_FIELD_SCHEMA_NAME, ISeoFields.REUSABLE_FIELD_SCHEMA_NAME };

// Disables caching
var cacheSettings = RetrievalCacheSettings.CacheDisabled;

// Gets the top 20 pages using the schemas
IEnumerable<IPageMetadata> pageInfo =
  await contentRetriever.RetrievePagesOfReusableSchemas<IPageMetadata>(
    schemaNames,
    new RetrievePagesOfReusableSchemasParameters(),
    query => query
                // Gets the top 20 results
                .TopN(20),
    cacheSettings,
    // Uses default mapping (columnName <=> propertyName)
    configureModel: null
);

{% endcode %}

For available parameters, see the {% inpage_link "Page query parameters" %} table.

### Retrieve pages of a single content type

Method: `RetrievePages<TSource, TResult>(...)`

Retrieves a collection of pages of a specific content type, represented by the `TResult` generic. Allows filtering by path, language, and other criteria using the parameters described in the {% inpage_link "Page query parameters" %} table.

The provided `TSource` type determines the content type to retrieve. It must be a model class registered using {% page_link 5IbWCQ linkText="RegisterContentTypeMapping" anchor="RegisterContentTypeMapping attribute" %} (applied to {% page_link 5IbWCQ linkText="generated system classes" %} by default). For this method, `TSource` also serves as the intermediate mapped result provided to the `configureModel` delegate.

{% code lang=csharp title="Basic usage" %}

// Assuming 'NewsArticle' is a generated model class for your news pages
// Gets all news articles using default settings (language, preview context, etc.)
IEnumerable<NewsArticle> newsArticles =
  await contentRetriever.RetrievePages<NewsArticle>();

{% endcode %}

{% code lang=csharp title="With parameters" %}

// Gets news articles under the /news path, including one level of linked items
var parameters = new RetrievePagesParameters
{
    // Gets only children under the specified path
    PathMatch = PathMatch.Children("/news"),
    // Includes directly linked items
    LinkedItemsMaxLevel = 1
};

IEnumerable<NewsArticle> newsArticles =
  await contentRetriever.RetrievePages<NewsArticle>(parameters);

{% endcode %}

{% code lang=csharp title="All configuration options" %}

// Assuming NewsArticleSummary is a simpler model
public class NewsArticleSummary
{
    public string Title { get; set; }
    public string Summary { get; set; }
}

// Gets the top 5 latest news articles under /news, only getting specific columns
var parameters = new RetrievePagesParameters
{
    PathMatch = PathMatch.Children("/news")
};

// Disables caching
var cacheSettings = RetrievalCacheSettings.CacheDisabled;

// NewsArticle is `TSource`, avalilable in configureModel
// NewsArticleSummary is `TResult`, the final type returned by the call
IEnumerable<NewsArticle> latestNews =
  await contentRetriever.RetrievePages<NewsArticle, NewsArticleSummary>(
    parameters,
    query => query
                // Selects specific columns
                .Columns("Headline", "PublicationDate", "Summary")
                // Orders by date
                .OrderByDescending("PublicationDate")
                // Gets the top 5
                .TopN(5),
    cacheSettings,
    configureModel: async (container, source) => new NewsArticleSummary
    {
        // Source is 'NewsArticle'
        Title = $"{source.Headline} - #{source.PublicationDate.ToString("en-US")}",
        Summary = source.Summary
    }
);

{% endcode %}

For available parameters, see the {% inpage_link "Page query parameters" %} table.

### Retrieve pages of multiple content types

Method: `RetrievePagesOfContentTypes<TResult>(...)`

Description: Retrieves a collection of web pages belonging to one of the specified content type code names, mapping them to the `TResult` model. Allows filtering by path, language, and other criteria using the parameters described in the {% inpage_link "Page query parameters" %} table.

{% code lang=csharp title="Basic usage" %}

var contentTypes = new[] { ArticlePage.CONTENT_TYPE_NAME, BlogPage.CONTENT_TYPE_NAME };

// Gets all articles and blogs using default settings
// Assuming 'BaseViewModel' is a suitable base class or interface
// You can also use 'IWebPageFieldsSource' as the shared type
// Suitable model classes are paired with content types using
// 'RegisterContentTypeMappingAttribute' (automatically added to generated classes)
IEnumerable<BaseViewModel> pages =
  await contentRetriever.RetrievePagesOfContentTypes<BaseViewModel>(contentTypes);

// Uses pattern matching to handle the specific type
switch (allPages.FirstOrDefault())
{
  case ArticlePage article:
    // Specific logic for ArticlePage
    Console.WriteLine($"Displaying Article: {article.Title}");
    // Render article view...
    break;

  case BlogPage blog:
    // Specific logic for BlogPage
    Console.WriteLine($"Displaying Blog Page: {blog.Headline}");
    // Render blog page view...
    break;

  default:
    // Handle any other unexpected page types
    // Render a default view...
    break;
}

{% endcode %}

{% code lang=csharp title="With parameters" %}

var contentTypes = new[] { Article.CONTENT_TYPE_NAME, Blog.CONTENT_TYPE_NAME };

// Gets articles and blogs under the /archive path
var parameters = new RetrievePagesOfContentTypesParameters
{
    PathMatch = PathMatch.Children("/archive")
};

IEnumerable<BaseViewModel> archivePages =
  await contentRetriever.RetrievePagesOfContentTypes<BaseViewModel>(contentTypes, parameters);

{% endcode %}

{% code lang=csharp title="All configuration options" %}

var contentTypes = new[] { Article.CONTENT_TYPE_NAME, Blog.CONTENT_TYPE_NAME };

// Gets the first 10 articles or blogs under /archive, ordered by name
var parameters = new RetrievePagesOfContentTypesParameters
{
    PathMatch = PathMatch.Children("/archive")
};

// Disables caching
var cacheSettings = RetrievalCacheSettings.CacheDisabled;

// Assuming BaseViewModel is a suitable base class
IEnumerable<BaseViewModel> links = await contentRetriever.RetrievePagesOfContentTypes<BaseViewModel>(
    contentTypes,
    parameters,
    query => query
                // Orders by name
                .OrderBy("DocumentName")
                // Gets the top 10
                .TopN(10),
    cacheSettings,
    // Uses default mapping
    configureModel: null
);

{% endcode %}

For available parameters, see the {% inpage_link "Page query parameters" %} table.

### Retrieve pages by GUIDs

**Method:** `RetrievePagesByGuids<TResult>(...)`

Retrieves a collection of specific pages identified by their `WebPageItemGUID` values. The method maps the retrieved data to the specified `TResult` model.

This method is useful when you have a list of specific page identifiers, for example obtained using {% page_link 8ASiCQ linkText="page selector" anchor="Page selector" %}, and need to fetch their data efficiently.

{% note %}
**Combined content selector usage limitations**

The GUIDs returned by {% page_link 8ASiCQ linkText="combined content selector" anchor="Combined content selector" %} cannot be used with this method because they are incompatible with the page GUIDs this method expects. The combined content selector returns `ContentItemGUID` values inside its `ContentItemReference` return type, while this method expects `WebPageItemGUID` values.

Passing collections of GUIDs from the combined content selector returns an empty result. Use {% inpage_link "Retrieve content items by GUIDs" linkText="RetrieveContentByGuids" %} to work with combined selector data.
{% endnote %}

{% code lang=csharp title="Basic usage" %}

// Assuming 'ArticlePage' is a generated model class
var specificPageGuids = new[] { Guid.Parse("..."), Guid.Parse("...") };

// Gets the specified pages using default settings
IEnumerable<ArticlePage> specificPages =
  await contentRetriever.RetrievePagesByGuids<ArticlePage>(specificPageGuids);

{% endcode %}

The rest of the usage is identical to {% inpage_link "Retrieve pages of a single content type" %}.

The `RetrievePagesByGuids` method utilizes the `RetrievePagesParameters` object, which is also used by the {% inpage_link "Retrieve pages of a single content type" %} method. This object allows you to fine-tune how the pages are retrieved. For available parameters, see the {% inpage_link "Page query parameters" %} table.

**Note:** When caching is enabled via `RetrievalCacheSettings`, appropriate {% page_link xYLWCQ linkText="cache dependencies" anchor="By ID/GUID/CodeName" %} based on the provided `SystemFields.WebPageItemGuid` values are automatically included.

### Retrieve all pages

Method: `RetrieveAllPages<TSource, TResult>(...)`

Retrieves all pages (by default from the current channel), mapping them to the specified `TResult` model. Allows filtering by path, language, and other criteria using the parameters described in the {% inpage_link "Page query parameters" %} table.

{% code lang=csharp title="Basic usage" %}

// Assuming 'BasePageViewModel' is a suitable base class for all pages
// Gets all pages using default settings
IEnumerable<BasePageViewModel> allPages =
  await contentRetriever.RetrieveAllPages<BasePageViewModel>();

// Uses pattern matching to handle the specific type
switch (allPages.FirstOrDefault())
{
  case ArticlePage article:
    // Specific logic for ArticlePage
    Console.WriteLine($"Displaying Article: {article.Title}");
    // Render article view...
    break;

  case LandingPage landing:
    // Specific logic for LandingPage
    Console.WriteLine($"Displaying Landing Page: {landing.Headline}");
    // Render landing page view...
    break;

  case null:
    // Handle case where the page is not found or not retrievable
    Console.WriteLine("Current page not found.");
    // Render 404 view...
    break;

  default:
    // Handle any other unexpected page types
    // Render a default view...
    break;
}

{% endcode %}

{% code lang=csharp title="With parameters" %}

var parameters = new RetrieveAllPagesParameters() {
  ChannelName = "FrenchCuisine"
};

IEnumerable<BasePageViewModel> allLivePages =
  await contentRetriever.RetrieveAllPages<BasePageViewModel>(parameters);

{% endcode %}

{% code lang=csharp title="All configuration options" %}

// Gets all pages under '/Articles', getting only specific columns
var parameters = new RetrieveAllPagesParameters
{
  PathMatch = PathMatch.Section("/Articles")
};

// Disables caching
var cacheSettings = RetrievalCacheSettings.CacheDisabled;

// Assuming Article is a generated model class
IEnumerable<PageTitleModel> pageLinks =
  await contentRetriever.RetrieveAllPages<BasePageViewModel>(
    parameters,
    query => query
                .Columns("PageTitle")
    cacheSettings,
    configureModel: async (container, baseModel) => {
      // Gets only the title and maps to specific model
      return new PageTitleModel() {
        PageTitle = baseModel.PageTitle ?? "Untitled page"
      }
    } 
);

{% endcode %}

Some overloads of `RetrieveAllPages` require an instance of `RetrieveAllPagesParameters`. This object allows you to fine-tune how the pages are retrieved. For available parameters, see the {% inpage_link "Page query parameters" %} table.

### Page query parameters

The following table contains all parameters available for page query methods. Each parameter row specifies which methods support that parameter.

{% table %}
  {% row  header="true" %}
    {% cell %}
    Property
    {% endcell %}
    {% cell %}
    Default value
    {% endcell %}
    {% cell %}
    Applicable methods
    {% endcell %}
    {% cell %}
    Description
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    ChannelName
    {% endcell %}
    {% cell %}
    `null`
    {% endcell %}
    {% cell %}
    `RetrievePagesOfReusableSchemas`, `RetrievePages`, `RetrievePagesOfContentTypes`, `RetrievePagesByGuids`, `RetrieveAllPages`
    {% endcell %}
    {% cell %}
    Name of the website channel to fetch the pages from. If `null`, the channel from the current request context is used.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    PathMatch
    {% endcell %}
    {% cell %}
    `null`
    {% endcell %}
    {% cell %}
    `RetrievePagesOfReusableSchemas`, `RetrievePages`, `RetrievePagesOfContentTypes`, `RetrievePagesByGuids`, `RetrieveAllPages`
    {% endcell %}
    {% cell %}
    Limits results based on the {% page_link 4obWCQ linkText="content tree path" anchor="Filter pages based on content tree structure" %} (e.g., children of a path, specific path). If `null`, no path filtering is used.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    IncludeUrlPath
    {% endcell %}
    {% cell %}
    `true`
    {% endcell %}
    {% cell %}
    All page query methods
    {% endcell %}
    {% cell %}
    Specifies if page URL data should be included in the results. This data is necessary when {% page_link retrieve_page_urls_xp linkText="resolving page URLs" %} and saves a database roundtrip if included.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    LinkedItemsMaxLevel
    {% endcell %}
    {% cell %}
    `0`
    {% endcell %}
    {% cell %}
    All page query methods
    {% endcell %}
    {% cell %}
    Controls the depth of {% page_link content_items_xp linkText="linked content items" anchor="Link content items" %} to retrieve recursively along with the main page. A value of `0` means no linked items are retrieved.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    LanguageName
    {% endcell %}
    {% cell %}
    `null`
    {% endcell %}
    {% cell %}
    All page query methods
    {% endcell %}
    {% cell %}
    Allows you to override the default content {% page_link OxT_Cw linkText="language" %} determined by the {% page_link 4obWCQ linkText="current context" anchor="Access current preferred language" %}. If left `null`, the language retrieved from the current HTTP request is used.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    UseLanguageFallbacks
    {% endcell %}
    {% cell %}
    `true`
    {% endcell %}
    {% cell %}
    All page query methods
    {% endcell %}
    {% cell %}
    Determines if the system should attempt to retrieve content in {% page_link OxT_Cw linkText="fallback languages" anchor="Language fallbacks" %} if the content is not available in the primary specified language.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    IncludeSecuredItems
    {% endcell %}
    {% cell %}
    `false`
    {% endcell %}
    {% cell %}
    All page query methods
    {% endcell %}
    {% cell %}
    Specifies whether content items requiring special permissions (e.g., {% page_link 4obWCQ linkText="secured sections" anchor="Page security configuration" %}) should be included in the results.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    IsForPreview
    {% endcell %}
    {% cell %}
    `null`
    {% endcell %}
    {% cell %}
    All page query methods
    {% endcell %}
    {% cell %}
    Allows overriding the preview mode context. If left `null`, the retrieval respects the current website context (whether the request is in preview mode or live). Set to `true` to force preview data or `false` to force live data.
    {% endcell %}
  {% endrow %}
{% endtable %}

## Content item queries

### Retrieve content items sharing reusable field schema

Method: `RetrieveContentOfReusableSchemas<TResult>(...)`

Retrieves a collection of reusable content items whose content types use one or more of the specified {% page_link D4_OD linkText="reusable field schemas" %}, mapping them to the `TResult` model. Allows filtering by workspace, language, and other criteria using the parameters described in the {% inpage_link "Content item query parameters" %} table.

{% code lang=csharp title="Basic usage" %}

// Assuming 'IContactDetails' is an interface implemented by types using the 'ContactSchema'
var schemaNames = new[] { IContactSchema.REUSABLE_FIELD_SCHEMA_NAME };

// Gets all content items using the 'IContactSchema' generated schema interface
IEnumerable<IContactSchema> contacts =
  await contentRetriever.RetrieveContentOfReusableSchemas<IContactSchema>(schemaNames);

{% endcode %}

{% code lang=csharp title="With parameters" %}

var schemaNames = new[] { IAddressSchema.REUSABLE_FIELD_SCHEMA_NAME,
                          ILocationSchema.REUSABLE_FIELD_SCHEMA_NAME };

// Gets items using Address or Location schemas from the 'Locations' workspace
var parameters = new RetrieveContentOfReusableSchemasParameters
{
    WorkspaceNames = new[] { "Locations" }
};

// Uses 'IContentItemFieldsSource' as the shared type
IEnumerable<IContentItemFieldsSource> locationData =
  await contentRetriever
    .RetrieveContentOfReusableSchemas<IContentItemFieldsSource>(schemaNames, parameters);

{% endcode %}

{% code lang=csharp title="All configuration options" %}

var schemaNames = new[] { IMetadataSchema.REUSABLE_FIELD_SCHEMA_NAME };

// Disables caching
var cacheSettings = RetrievalCacheSettings.CacheDisabled;

// Assuming IMetadata is an inteface generated for 'MetadataSchema'
IEnumerable<IMetadata> oldestMetadataItems =
  await contentRetriever.RetrieveContentOfReusableSchemas<IMetadata>(
    schemaNames,
    parameters,
    query => query
                // Gets the first 5
                .TopN(5),
    cacheSettings,
    // Uses default mapping
    configureModel: null
);

{% endcode %}

For available parameters, see the {% inpage_link "Content item query parameters" %} table.

### Retrieve content items of a single content type

Method: `RetrieveContent<TSource, TResult>(...)`

Retrieves a collection of content items of a specific {% page_link gYHWCQ linkText="content type" %}, mapped to the `TResult` model. Allows filtering by workspace, language, and other criteria using the parameters described in the {% inpage_link "Content item query parameters" %} table. 

The provided `TSource` type determines the content type to retrieve. It must be a model class registered using {% page_link 5IbWCQ linkText="RegisterContentTypeMapping" anchor="RegisterContentTypeMapping attribute" %} (applied to {% page_link 5IbWCQ linkText="generated system classes" %} by default). For this method, `TSource` also serves as the intermediate mapped result provided to the `configureModel` delegate.

{% code lang=csharp title="Basic usage" %}

// Assuming 'Author' is a generated model class for your author content items
// Gets all authors using default settings (language, preview context, etc.)
IEnumerable<Author> authors =
  await contentRetriever.RetrieveContent<Author>();

{% endcode %}

{% code lang=csharp title="With parameters" %}

// Gets authors from a specific workspace
var parameters = new RetrieveContentParameters
{
    // Specifies relevant workspace
    WorkspaceNames = new[] { "BlogContent" },
};

IEnumerable<Author> blogAuthors =
  await contentRetriever.RetrieveContent<Author>(parameters);

{% endcode %}

{% code lang=csharp title="All configuration options" %}

// Gets the top 5 authors with the most articles from the 'BlogContent' workspace
var parameters = new RetrieveContentParameters
{
     WorkspaceNames = new[] { "BlogContent" }
};

// Disables caching
var cacheSettings = RetrievalCacheSettings.CacheDisabled;

// Assuming AuthorSummary is a simpler model with FullName and ArticleCount
public class AuthorSummary
{
    public string FullName { get; set; }
    public int ArticleCount { get; set; }
}

// 'TSource' set to generated model class 'Author'
// Final result `TResult` transformed and mapped to 'AuthorSummary'
IEnumerable<AuthorSummary> topAuthors = await contentRetriever.RetrieveContent<Author, AuthorSummary>(
    parameters,
    query => query
                // Selects specific columns
                .Columns("FirstName", "LastName", "ArticleCount")
                // Orders by count
                .OrderBy(new OrderByColumn("ArticleCount", OrderDirection.Descending))
                // Gets the top 5
                .TopN(5),
    cacheSettings,
    // Example of custom mapping
    async (container, source) => new AuthorSummary
    {
        // Source is 'Author'
        FullName = $"{source.FirstName} {source.LastName}",
        ArticleCount = source.ArticleCount
    }
);

{% endcode %}

For available parameters, see the {% inpage_link "Content item query parameters" %} table.

### Retrieve content items of multiple content types

Method: `RetrieveContentOfContentTypes<TResult>(...)`

Retrieves a collection of reusable content items belonging to the specified content types, mapping them to the `TResult` model. Allows filtering by workspace, language, and other criteria using the parameters described in the {% inpage_link "Content item query parameters" %} table.

{% code lang=csharp title="Basic usage" %}

var contentTypes = new[] { Author.CONTENT_TYPE_NAME, Article.CONTENT_TYPE_NAME };

// Gets all authors and articles using default settings
// Uses 'IContentItemFieldsSource' as the shared type.
// Suitable model classes are paired with content types using
// 'RegisterContentTypeMappingAttribute' (automatically added to generated classes)
IEnumerable<IContentItemFieldsSource> blogItems =
  await contentRetriever
    .RetrieveContentOfContentTypes<IContentItemFieldsSource>(contentTypes);

foreach (var item in blogItems)
{
  switch (item)
  {
    case Author author:
      // Handles Author specific logic
      Console.WriteLine($"- Found Author: {author.DisplayName}");
      // e.g., DisplayAuthorBio(author);
      break;

    case Article article:
      // Handles Article specific logic
      Console.WriteLine($"- Found Article: {article.DisplayName}");
      // e.g., DisplayArticleSummary(article);
      break;

    default:
      break;
  }
}

{% endcode %}

{% code lang=csharp title="With parameters" %}

var contentTypes = new[] { Author.CONTENT_TYPE_NAME, BookReview.CONTENT_TYPE_NAME };

// Gets authors and book reviews from the 'Reviews' workspace
var parameters = new RetrieveContentOfContentTypesParameters
{
    WorkspaceNames = new[] { "Reviews" }
};

IEnumerable<IContentItemFieldsSource> reviewContent =
  // Can use 'IContentItemFieldsSource' if no common base type
  await contentRetriever
    .RetrieveContentOfContentTypes<IContentItemFieldsSource>(contentTypes, parameters);

{% endcode %}

{% code lang=csharp title="All configuration options" %}

var contentTypes = new[] { Author.CONTENT_TYPE_NAME,
                           Article.CONTENT_TYPE_NAME,
                           Book.CONTENT_TYPE_NAME };

// Gets the 10 most recently modified authors, articles, or books
var parameters = new RetrieveContentOfContentTypesParameters
{
     // Retrieves content from multiple workspaces
     WorkspaceNames = new[] { "MainContent", "BlogContent" }
};

// Disables caching
var cacheSettings = RetrievalCacheSettings.CacheDisabled;

// Assuming IContentInfo is an interface with Name and LastModified properties
IEnumerable<IContentInfo> recentContent =
  await contentRetriever.RetrieveContentOfContentTypes<IContentInfo>(
    contentTypes,
    parameters,
    query => query
                // Selects specific columns
                .Columns("DisplayName", "ContentItemModifiedWhen")
                // Gets the top 10 results
                .TopN(10),
    cacheSettings,
    // Uses default mapping
    configureModel: null
);

{% endcode %}

For available parameters, see the {% inpage_link "Content item query parameters" %} table.

### Retrieve content items by GUIDs

**Method:** `RetrieveContentByGuids<TResult>(...)`

Retrieves a collection of specific reusable content items identified by their `ContentItemGUID` values. The method maps the retrieved data to the specified `TResult` model.

This method is useful when you have a list of specific content item identifiers (e.g., from a {% page_link RoXWCQ linkText="related items" %} field) and need to fetch their data efficiently.

{% code lang=csharp title="Basic usage when expecting a single type" %}

// Assuming 'Author' is a generated model class
var specificAuthorGuids = new[] { Guid.Parse("..."), Guid.Parse("...") };

// Gets the specified authors using default settings
IEnumerable<Author> specificAuthors =
  await contentRetriever.RetrieveContentByGuids<Author>(specificAuthorGuids);

{% endcode %}

The rest of the usage is identical to {% inpage_link "Retrieve content items of a single content type" %}.

The `RetrieveContentByGuids` method uses the `RetrieveContentParameters` object. This object allows you to fine-tune how the content items are retrieved. For available parameters, see the {% inpage_link "Content item query parameters" %} table.

**Note:** When caching is enabled via `RetrievalCacheSettings`, appropriate cache dependencies based on the provided `contentItemGuids` are automatically included.

### Content item query parameters

The following table contains all parameters available for content item query methods. Each parameter row specifies which methods support that parameter.

{% table %}
  {% row  header="true" %}
    {% cell %}
    Property
    {% endcell %}
    {% cell %}
    Default value
    {% endcell %}
    {% cell %}
    Applicable methods
    {% endcell %}
    {% cell %}
    Description
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    IncludeUrlPath
    {% endcell %}
    {% cell %}
    `true`
    {% endcell %}
    {% cell %}
    All content item query methods
    {% endcell %}
    {% cell %}
    Specifies if page URL data should be included in the results. This data is necessary when {% page_link retrieve_page_urls_xp linkText="resolving page URLs" %} and saves a database roundtrip if included.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    WorkspaceNames
    {% endcell %}
    {% cell %}
    `[]` (Empty *Enumerable*)
    {% endcell %}
    {% cell %}
    All content item query methods
    {% endcell %}
    {% cell %}
    Names of the {% page_link workspaces_xp linkText="workspaces" %} from which content should be retrieved.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    LinkedItemsMaxLevel
    {% endcell %}
    {% cell %}
    `0`
    {% endcell %}
    {% cell %}
    All content item query methods
    {% endcell %}
    {% cell %}
    Controls the depth of linked content items to retrieve recursively. `0` means no linked items are retrieved.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    LanguageName
    {% endcell %}
    {% cell %}
    `null`
    {% endcell %}
    {% cell %}
    All content item query methods
    {% endcell %}
    {% cell %}
    Allows you to override the default content {% page_link OxT_Cw linkText="language" %} determined by the {% page_link 4obWCQ linkText="current context" anchor="Access current preferred language" %}. If left `null`, the language retrieved from the current HTTP request is used.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    UseLanguageFallbacks
    {% endcell %}
    {% cell %}
    `true`
    {% endcell %}
    {% cell %}
    All content item query methods
    {% endcell %}
    {% cell %}
    Determines if the system should attempt to retrieve content in {% page_link OxT_Cw linkText="fallback languages" anchor="Language fallbacks" %} if the content is not available in the primary specified language.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    IncludeSecuredItems
    {% endcell %}
    {% cell %}
    `false`
    {% endcell %}
    {% cell %}
    All content item query methods
    {% endcell %}
    {% cell %}
    Specifies whether content items requiring special permissions (e.g., {% page_link 4obWCQ linkText="secured sections" anchor="Page security configuration" %}) should be included in the results.
    {% endcell %}
  {% endrow %}

  {% row %}
    {% cell %}
    IsForPreview
    {% endcell %}
    {% cell %}
    `null`
    {% endcell %}
    {% cell %}
    All content item query methods
    {% endcell %}
    {% cell %}
    Allows overriding the preview mode context. If left null, the retrieval respects the current website context (whether the request is in preview mode or live). Set to true to force preview data or false to force live data.
    {% endcell %}
  {% endrow %}
{% endtable %}
