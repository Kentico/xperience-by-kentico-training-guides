---
    title: Reference - Content item query
    persona: developer
    identifier: YRT_Cw
    order: 200
    license: 1

    redirect_from: x/YRT_Cw

    toc:
        minHeadingLevel: 2
        maxHeadingLevel: 3
---

This page provides information about the parameterization methods available for the {% page_link WhT_Cw linkText="Content item API" %}. The methods allow you to adjust queries and limit which items are retrieved or specify which columns are loaded to improve performance, for example.

## ContentItemQueryBuilder methods

### ForContentType {% anchor Builder-ForContentType %}

Retrieves all items of the specified content type. Generates a subquery that can be further configured. See {% inpage_link "Content query parameterization" linkText="Content query parameterization" %} and {% inpage_link "ForContentType parameterization" %}.

{% code lang=csharp %}
var builder = new ContentItemQueryBuilder();

// Retrieves all content items of the 'Sample.Type' type
builder.ForContentType("Sample.Type");
{% endcode %}

### ForContentTypes {% anchor Builder-ForContentTypes %}

Retrieves all content items across all content types. Use the method's {% inpage_link "ForContentTypes parameterization" linkText="inner parameterization" %} to limit the selection to a subset of items.

Does not include content type field data and web page data by default. Use {% inpage_link FCTs-WCTF linkText="WithContentTypeFields" %} or {% inpage_link FCTs-WWPD linkText="WithWebPageData" %} to include it.

{% code lang=csharp %}
var builder = new ContentItemQueryBuilder();

builder.ForContentTypes(parameters =>
{
    // Retrieves all items with the given reusable schema
    parameters.OfReusableSchema("PageMetadata");
});
{% endcode %}

### Parameters {% anchor Builder-Parameters %}

Specifies a set of parameters that apply to all items selected by individual subqueries. See {% inpage_link "Content query parameterization" linkText="Content query parameterization" %}.

{% code lang=csharp %}

var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type")
       .ForContentType("Sample.NewsRelease")
       // Sorts all records according to the 'ContentItemName' column
       .Parameters(globalParams => globalParams.OrderBy("ContentItemName"))

{% endcode %}

### InLanguage

Selects items from the specified language. Use a language code name as specified in the **Languages** application. The `InLanguage` method is applied to the result of all its preceding subqueries in a query.

{% code lang=csharp %}

var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type")
       .ForContentType("Sample.NewsRelease")
       // Selects only items from the English language
       .InLanguage("en");

{% endcode %}

More advanced scenario including linked items:

{% code lang=csharp %}

var builder = new ContentItemQueryBuilder();

builder.ForContentType("project.store", subqueryParameters =>
        {
            // Retrieves all linked items up to the maximum depth of 1
            subqueryParameters.WithLinkedItems(1);
        })
        // Selects only items from the English language
        .InLanguage("en");

{% endcode %}

You can see the order in which the query methods are evaluated in the following diagram:

{% image InLanguageWithLinkedItems.png title="InLanguage method used on ForContentType query with linked items" width=500 border=true %}

### InWorkspaces

Selects items from the specified {% page_link workspaces_xp linkText="workspaces" %}. Use workspace code names as specified in the **Workspaces** application. The `InLanguage` method is applied to the result of all its preceding subqueries in a query.

{% code lang=csharp %}

var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type")
       .ForContentType("Sample.NewsRelease")
       // Selects only items from the WonkaFactory and Acme workspaces
       .InWorkspaces("WonkaFactory", "Acme");

{% endcode %}

{% note %}

Website channel pages are not scoped under workspaces. Combining this method with {% inpage_link "ReferenceContentitemquery-ForWebsite" linkText="ForWebsite" %} in a single query leads to an empty query result.

{% endnote %}

More advanced scenario including linked items:

{% code lang=csharp %}

var builder = new ContentItemQueryBuilder();

builder.ForContentType("project.store", subqueryParameters =>
        {
            // Retrieves all linked items up to the maximum depth of 1
            subqueryParameters.WithLinkedItems(1);
        })
        // Selects only items from the WonkaFactory and Acme workspaces
        .InWorkspaces("WonkaFactory", "Acme");

{% endcode %}

You can see the order in which the query methods are evaluated in the following diagram:

{% image InWorkspacesWithLinkedItems.png title="InWorkspaces method used on ForContentType query with linked items" width=500 border=true %}

## Content query parameterization

### Columns

Limits the columns that are retrieved by the query. See {% page_link XBT_Cw linkText="Content item database structure" %}.

If not specified, the query by default includes all columns from all selected content types.

{% code lang=csharp %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    // Retrieves only the 'Title' and 'Content' columns from 'Sample.Type'
    subqueryParameters.Columns("Title", "Content");
});
{% endcode %}

If *Columns* is called multiple times for a content type, columns from all method calls are included.

The method also supports column aliasing:

{% code lang=csharp %}
builder.ForContentType("Sample.Type1", subqueryParameters =>
{
    // Aliases 'Type1Title' as 'Title'
    subqueryParameters.Columns(QueryColumn.Alias("Type1Title", "Title"));
})
ForContentType("Sample.Type2", subqueryParameters =>
{
    // Aliases 'Type2Title' as 'Title'
    subqueryParameters.Columns(QueryColumn.Alias("Type2Title", "Title"));
})
// Orders both 'Type1' and 'Type2'
.Parameters(globalParameters => globalParameters.OrderBy("Title"));
{% endcode %}

#### UrlPathColumns

A specialized extension method that adds only the columns required for {% page_link retrieve_page_urls_xp linkText="web page URL retrieval" %}. This method helps optimize performance when you need only URL-related data.

When used with {% inpage_link "ForContentType parameterization" linkText="ForContentType" %}, call the method inside the subquery parameterization action:

{% code lang=csharp title="UrlPathColumns usage" highlight=7 %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Page", subqueryParameters =>
{
    // Retrieves only columns required for URL path generation
    subqueryParameters.ForWebsite("SampleChannel")
                      .UrlPathColumns();
});
{% endcode %}

When used with {% inpage_link "ForContentTypes parameterization" linkText="ForContentTypes" %}, call the method on the global `Parameters` object:

{% code lang=csharp title="UrlPathColumns usage" highlight=7 %}
var builder = new ContentItemQueryBuilder();

builder.ForContentTypes( /* query parameters... */)
       .Parameters(p =>
          // Retrieves only columns required for URL path generation
          p.UrlPathColumns()
       );
{% endcode %}

`UrlPathColumns` must be used in conjunction with `ForWebsite` or `WithWebPageData`. It automatically includes all columns required by the system to correctly resolve page URLs.

You can combine `UrlPathColumns` with additional `Columns` calls if you need URL columns in addition to other fields:

{% code lang=csharp title="UrlPathColumns usage" highlight=7-8 %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Page", subqueryParameters =>
{
    // Retrieves URL path columns plus the Title field
    subqueryParameters.ForWebsite("SampleChannel")
                     .UrlPathColumns()
                     .Columns("Title");
});
{% endcode %}

### Offset

Offsets the records by the specified `index` (zero\-based) and takes the next `X` items specified by `fetch`.

Must be used together with {% inpage_link "ReferenceContentitemquery-OrderByParam" linkText="OrderBy" %}, otherwise the pagination is not applied (as the system cannot guarantee a deterministic ordering of the returned results).

{% code lang=csharp %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    // Takes the next 5 items starting from the 11th
    subqueryParameters.Offset(10, 5);
    subqueryParameters.OrderBy("ContentItemName");
});
{% endcode %}

### IncludeTotalCount

Ensures that every retrieved item stores the total number of items, regardless of pagination applied by *Offset*.

{% code lang=csharp %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    // Includes the total number of items
    subqueryParameters.IncludeTotalCount()
    // Takes the next 5 items starting from the 11th
    subqueryParameters.Offset(10, 5);
    subqueryParameters.OrderBy("ContentItemName");
});
{% endcode %}

After executing the query, use `GetTotalCount` on an item in the result to get the total number of items.

{% code lang=csharp %}
var items = await queryExecutor.GetResult(builder, item => item);
int? totalItemCount = items.First().GetTotalCount();
{% endcode %}

### TopN

Limits the number of records fetched from the database. 

{% code lang=csharp %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    // Takes the first 5 results from the selection
    subqueryParameters.TopN(5);
});
{% endcode %}

### OrderBy{% anchor ReferenceContentitemquery-OrderByParam %} 

Allows ordering of the results based on the value of a specified column.

{% code lang=csharp %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    // By default, items in the specified columns are sorted in ascending order
    subqueryParameters.OrderBy("ContentItemName");

    // You can parameterize the behavior by providing an instance of 'CMS.DataEngine.OrderByColumn'
    subqueryParameters.OrderBy(new OrderByColumn("ContentItemName", OrderDirection.Descending));
});
{% endcode %}

### Where

Inserts an {% external_link "https://learn.microsoft.com/en-us/sql/t-sql/queries/where-transact-sql" linkText="SQL WHERE" %} clause into the query.

{% code lang=csharp title="WhereTrue" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    // Retrieves items that have the 'ShowInBanner' property set to true
    subqueryParameters.Where(where => where.WhereTrue("ShowInBanner"));
});
{% endcode %}

{% code lang=csharp title="WhereLike" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    // Multiple where operations are implicitly joined by AND
    subqueryParameters.Where(where => where.WhereLike("ColumnA", "value")
                                        .WhereLike("ColumnB", "value"));

    // OR has to be specified explicitly
    subqueryParameters.Where(where => where.WhereLike("ColumnA", "value")
                                        .Or()
                                        .WhereLike("ColumnB", "value"));
});
{% endcode %}

{% code lang=csharp title="WhereStartsWith" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    // Retrieves items whose name starts with with 'Apple'  
    subqueryParameters.Where(where => where.WhereStartsWith("ContentItemName", "Apple"));
});
{% endcode %}

{% info %}
The set of available `Where`extensions matches the expressivity of the SQL WHERE syntax. This page provides examples of only a few of the available methods.
{% endinfo %}

### WhereContainsTags

Limits the query to content items that contain the {% page_link taxonomies_xp linkText="specified tags" %}.

{% code lang=csharp title="WhereContainsTags" %}
// A collection of tags, e.g., obtained from a Tag selector
IEnumerable<Guid> tagIdentifiers;

var builder = new ContentItemQueryBuilder()
    .ForContentType(
        "Some.Type",
        subqueryParameters =>
            // Retrieves items with the specified tags
            subqueryParameters.Where(where =>
                where.WhereContainsTags("SomeTaxonomy", tagIdentifiers))
    ).InLanguage("en");

{% endcode %}

{% code lang=csharp title="WhereContainsTags" %}
// A collection of tags, e.g., obtained from a Tag selector
IEnumerable<Guid> tagIdentifiers;
var tagCollection = await TagCollection.Create(tagIdentifiers);

var builder = new ContentItemQueryBuilder()
    .ForContentType(
        ArticlePage.CONTENT_TYPE_NAME,
        subqueryParameters =>
            // Retrieves items with the specified tags and any child tags
            subqueryParameters.Where(where =>
                where.WhereContainsTags("SomeTaxonomy", tagCollection))
    ).InLanguage("en");
{% endcode %}

## ForContentType parameterization

Methods described in this section can only be called from within subqueries generated by a `ContentItemBuilder.ForContentType` call. 

### ForWebsite {% anchor ReferenceContentitemquery-ForWebsite %}

Configures the query to {% page_link 4obWCQ linkText="retrieve pages" %} from a specified {% page_link 34HFC linkText="website channel" %}. Specify the following parameters:

- `websiteChannelName` – code name of the website channel from which the pages are retrieved.
- `pathMatch` – a parameter of type `PathMatch` used to limit the retrieved pages only to a certain section of the website's content tree.
- `includeUrlPath` – indicates if the URL path should be included in the retrieved data.

{% code lang=csharp title="ForWebsite" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    // Retrieves pages from the specified website channel
    subqueryParameters.ForWebsite(
                            websiteChannelName: "DancingGoatPages",
                            pathMatch: PathMatch.Children("/Articles"),
                            includeUrlPath: true);
});
{% endcode %}

See {% page_link 4obWCQ linkText="Retrieve page content" %} for more information.

### WithLinkedItems {% anchor ReferenceContentitemquery-WithLinkedItems %}

Configures query to include {% page_link content_items_xp anchor="Link content items" linkText="linked content items" %} recursively up to the specified depth.

{% code lang=csharp title="WithLinkedItems" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    // Retrieves all linked items up to the maximum depth of 2
    subqueryParameters.WithLinkedItems(2);
});
{% endcode %}

If the collection includes web page items and you want to use web page specific data (such as URL and tree path), specify the `IncludeWebPageData` option.

{% code lang=csharp title="IncludeWebPageData usage" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    // Retrieves all linked items up to the maximum depth of 2
    // including web page data of linked web page items
    subqueryParameters
        .WithLinkedItems(2, options => options.IncludeWebPageData());
});
{% endcode %}

To see how the query result is bound to a model class, check out {% inpage_link "ReferenceContentitemquery-WithLinkedItemsMap" linkText="WithLinkedItems mapping" %}.

### Linking {% anchor ReferenceContentitemquery-ForContentTypeLinking %}

Retrieves all content items of the type specified in `ForContentType` that reference any of the content items from the provided collection in the given field. Enables loading data on\-demand (lazily).

{% code lang=csharp title="Linking" %}
var builder = new ContentItemQueryBuilder();

// Retrieves items of the 'project.staff' content type
builder.ForContentType("project.staff", subqueryParameters =>
{
    // Retrieves all content items of 'project.staff' type that reference any
    // of the items in 'managersCollection' from their 'ManagerField' field
    subqueryParameters.Linking("ManagerField", managersCollection);
});
{% endcode %}

For more information see {% inpage_link "ReferenceContentitemquery-Linking" linkText="Linking details" %}.

### LinkedFrom {% anchor ReferenceContentitemquery-ForContentTypeLinkedFrom %}

Retrieves content items of the type specified in `ForContentType` that are linked from the given field of items in the provided collection with the given content type. Enables loading data on\-demand (lazily).

{% code lang=csharp title="LinkedFrom" %}
var builder = new ContentItemQueryBuilder();

// Retrieves items of the 'project.staff' content type
builder.ForContentType("project.staff", subqueryParameters =>
{
    // Items are retrieved for a collection of
    // 'project.store' items - 'storesCollection' - and the field 'StaffField'
    subqueryParameters.LinkedFrom("project.store", "StaffField", storesCollection);
});
{% endcode %}

For more information see {% inpage_link "ReferenceContentitemquery-LinkedFrom" linkText="LinkedFrom details" %}.

### LinkedFromSchemaField

Retrieves all content items of the type specified in `ForContentType` that are linked from a collection of items via a {% page_link D4_OD linkText="reusable schema field" %}.

{% code lang=csharp title="LinkedFromSchemaField usage example" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentType("Sample.Type", subqueryParameters =>
{
    subqueryParameters.LinkedFromSchemaField("SchemaFieldCodeName", contentItems);
});
{% endcode %}

For more information see {% inpage_link "FCT-LinkedFromSchemaField" linkText="LinkedFromSchemaField details" %}.

### SetUrlLanguageBehavior

Defines the behavior for the language slug of URL paths when web pages are retrieved in a fallback language. This method is relevant when requesting multilingual content ({% inpage_link "InLanguage" linkText="InLanguage" %}) and {% page_link retrieve_page_urls_xp linkText="retrieving page URL" %} information (e.g., through methods like {% page_link retrieve_page_urls_xp linkText="GetUrl()" %}, often with {% inpage_link ReferenceContentitemquery-ForWebsite linkText="ForWebsite" %} or {% inpage_link FCTs-WWPD linkText="WithWebPageData" %}).

It accepts a `UrlLanguageBehavior` enum value:

- `UrlLanguageBehavior.UseRequestedLanguage` -- When a page is served in a  {% page_link OxT_Cw linkText="fallback language" anchor="Language fallbacks" %}, its URL path is generated based on the language originally requested by the `InLanguage` method. For instance, if Spanish is requested and an English page is returned as a fallback, the URL path will still be structured as if it were a Spanish page (e.g., using URL prefixes as configured in the {% page_link OxT_Cw linkText="Languages" anchor="Set up a new language" %} application).
- `UrlLanguageBehavior.UseFallbackLanguage` (Default) -- When a page is served in a fallback language, its URL path is generated based on the language of the actual fallback content. In the example below, the URL path would be structured as an English page.

{% code lang=csharp title="Using SetUrlLanguageBehavior" highlight=7 %}
// Assume 'builder' is an initialized ContentItemQueryBuilder

// Within ForContentType configuration:
builder.ForContentType("Sample.PageType", config =>
{
    config.ForWebsite("MyWebsiteChannel")
          .SetUrlLanguageBehavior(UrlLanguageBehavior.UseRequestedLanguage)
})
// Requesting multilingual content
.InLanguage("spanish");

// After executing the query:
IEnumerable<SamplePageType> pages = await executor.GetMappedWebPageResult<SamplePageType>(builder);
foreach (var page in pages)
{
    // If 'page' is an English fallback for a Spanish request, 
    // its url will be in the format:
    //
    // '/<spanishLanguageName>/my-page-slug'
    //
    // due to 'UseRequestedLanguage'.
    WebPageUrl url = page.GetUrl();
}
{% endcode %}

## ForContentTypes parameterization

Methods described in this section can only be called from within subqueries generated by a `ContentItemBuilder.ForContentTypes` call.

### OfContentType

Selects items with the specified content types.

{% code lang=csharp title="Retrieve based on content type" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentTypes(parameters =>
{  
    // Retrieves items of the 'Acme.Article', 'Acme.Blog' content types
    parameters.OfContentType("Acme.Article", "Acme.Blog");
});
{% endcode %}

### OfReusableSchema

Selects items with the specified {% page_link D4_OD linkText="reusable field schema" %}.

{% code lang=csharp title="Retrieve based on reusable field schema" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentTypes(parameters =>
{  
    // Retrieves items with the 'PageMetadata' reusable field schema
    parameters.OfReusableSchema("PageMetadata");
});
{% endcode %}

### ForWebsite

Provides multiple overloads that can:

- select all pages in the system
- select pages based on their ID, GUID, or code names
- select pages from specific {% page_link 34HFC linkText="website channels" %} and paths

{% code lang=csharp title="Retrieve web page items" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentTypes(parameters =>
{
    // Calls are mutually exclusive
    // Shown to demonstrate available overloads

    // Retrieves all pages in the system
    parameters.ForWebsite();

    // Retrieves pages with the provided GUIDs
    // (e.g., from the 'Page selector' component)
    parameters.ForWebsite(webPageGuids);

    // Retrieves all pages under the 'Acme' channel and 'Articles' page path
    parameters.ForWebsite(
            websiteChannelName: "Acme",
            pathMatch: PathMatch.Children("/Articles"));
});
{% endcode %}

Each overload also provides the optional `includeUrlPath` path parameter. `true` by default, it indicates whether web page URL data should be included in the query.

If you want to retrieve pages and reusable content items in a single query, see the {% inpage_link FCTs-WWPD linkText="WithWebPageData" %} method.

### WithContentTypeFields {% anchor FCTs-WCTF %}

By default, the result returned by `ForContentTypes` contains content item metadata (*CMS_ContentItem* table {% page_link XBT_Cw anchor="Content items" %}) and {% page_link D4_OD linkText="reusable field schema" %} data. `WithContentTypeFields` also adds {% page_link gYHWCQ linkText="content type-specific fields" anchor="Add fields" %} to the result.

{% code lang=csharp title="WithContentTypeFields example" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentTypes(parameters =>
{
    // Retrieves items with the 'PageMetadata' reusable field schema
    // and includes content-type specific fields in the result
    parameters.OfReusableSchema("PageMetadata")
                .WithContentTypeFields();
});
{% endcode %}

### WithWebPageData {% anchor FCTs-WWPD %}

By default, the result returned by `ForContentTypes` contains content item metadata (*CMS_ContentItem* table {% page_link XBT_Cw anchor="Content items" %}) and {% page_link D4_OD linkText="reusable field schema" %} data. `WithWebPageData` also adds website content-specific fields (such as URL and tree path) to the result.

{% code lang=csharp title="WithWebPageData example" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentTypes(parameters =>
{
    // Retrieves items with the 'PageMetadata' reusable field schema
    // and includes web page specific fields in the result
    parameters.OfReusableSchema("PageMetadata")
                .WithWebPageData();
});
{% endcode %}

### WithLinkedItems

Configures query to include {% page_link content_items_xp anchor="Link content items" linkText="linked content items" %} recursively up to the specified depth. The parameterization behaves identically to its alternative in {% inpage_link "ReferenceContentitemquery-WithLinkedItems" linkText="ForContentType" %}.

{% code lang=csharp title="WithLinkedItems usage" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentTypes(subqueryParameters =>
{
    // Retrieves all linked items up to the maximum depth of 2
    subqueryParameters
        .OfContentType(Article.CONTENT_TYPE_NAME)
        .WithLinkedItems(2);
});
{% endcode %}

If the collection of linked items contains web page items, you need to specify the `IncludeWebPageData` option to include web page specific data (such as URL and tree path) of the web page items:

{% code lang=csharp title="IncludeWebPageData usage" %}
var builder = new ContentItemQueryBuilder();

builder.ForContentTypes(subqueryParameters =>
{
    // Retrieves all linked items up to the maximum depth of 2
    // including web page data of linked web page items
    subqueryParameters
        .OfContentType(Article.CONTENT_TYPE_NAME)
        .WithLinkedItems(2, options => options.IncludeWebPageData());
});
{% endcode %}

To see how the query result is bound to a model class, check out {% inpage_link "ReferenceContentitemquery-WithLinkedItemsMap" linkText="WithLinkedItems mapping" %}.

### Linking

The method behaves identically to {% inpage_link "ReferenceContentitemquery-ForContentTypeLinking" linkText="ForContentType.Linking" %}, with the following exceptions:

- You need to specify the name of the content type whose items you want to retrieve, in addition to the field used to link the items. This is necessary to prevent ambiguities in case the searched content types contain identical field names.
- The usage is limited to one `Linking` call for each `ForContentTypes` subquery.

{% code lang=csharp title="ForContentTypes.Linking usage" %}
var builder = new ContentItemQueryBuilder();
builder.ForContentTypes(subqueryParameters =>
{
    // Retrieves items of the 'project.staff' type that link to
    // any of the content items in 'managersCollection' via their 'ManagerField' field
    subqueryParameters.Linking("project.staff", "ManagerField", managersCollection);
});
{% endcode %}

For more information see {% inpage_link "ReferenceContentitemquery-Linking" linkText="Linking details" %}.

### LinkedFrom

The behavior is identical to {% inpage_link "ReferenceContentitemquery-ForContentTypeLinkedFrom" linkText="ForContentType.LinkedFrom" %}, with the only difference being that you cannot chain multiple calls within a single subquery.

For more information see {% inpage_link "ReferenceContentitemquery-LinkedFrom" linkText="LinkedFrom details" %}.

### LinkingSchemaField

Retrieves all content items that link to a collection of items via the specified {% page_link D4_OD linkText="reusable schema field" %}. {% inpage_link FCT-LinkedFromSchemaField linkText="LinkedFromSchemaField" %} complements this method by retrieving links from the opposite direction.

{% code lang=csharp title="LinkingSchemaField method usage" %}
var imageIdentifiers = new List<int>() { 1, 2, 3 };

var builder = new ContentItemQueryBuilder().ForContentTypes(q =>
{
    q.LinkingSchemaField("ProductImage", imageIdentifiers);
});
{% endcode %}

For more information see {% inpage_link "FCT-LinkingSchemaField" linkText="LinkingSchemaField details" %}.

### LinkedFromSchemaField

Retrieves all content items that are linked from a collection of items via a {% page_link  D4_OD linkText="reusable schema field" %}. {% inpage_link FCT-LinkingSchemaField linkText="LinkingSchemaField" %} complements this method by retrieving links from the opposite direction.

{% code lang=csharp title="LinkedFromSchemaField method usage" %}
var builder = new ContentItemQueryBuilder().ForContentTypes(q =>
{
    q.LinkedFromSchemaField("ProductImage", productIdentifiers);
});
{% endcode %}

For more information see {% inpage_link "FCT-LinkedFromSchemaField" linkText="LinkedFromSchemaField details" %}.

### InSmartFolder

Retrieves content items that fulfill the filter conditions of the specified {% page_link content_hub_folders_xp anchor="Smart folders" linkText="smart folder" %}. This allows content editors to control which items are retrieved directly in the *Content hub* UI, without needing to adjust the code.

The smart folder can be specified by its ID, GUID, or code name. You can get the smart folder GUID from a field using the {% page_link 8ASiCQ anchor="Smart folder selector" linkText="Smart folder selector" %} UI form component. You can also find the identifiers of smart folders in the *Content hub* application -- expand the menu actions of a folder and select *Properties*.

{% code lang=csharp title="InSmartFolder method usage" %}
var builder = new ContentItemQueryBuilder();

// Retrieves content items from the specified smart folder
builder.ForContentTypes(parameters => parameters.InSmartFolder(smartFolderGuid));
{% endcode %}

The `InSmartFolder` parameterization causes the query to return an **empty result** if the specified smart folder:

- Doesn't exist
- Doesn't have dynamic content delivery enabled
- Has invalid filter conditions (for example if a tag saved in the *Taxonomy* filter option was later deleted)

The following scenarios are unsupported and result in an exception:

- Using multiple `inSmartFolder` calls for a single query
- Combining `InSmartFolder` with `ForWebsite`parameterization (smart folders are only supported for reusable content items, not pages)

If you need to ensure that only items of one specific content type are retrieved (regardless of the smart folder's filter condition), add `OfContentType` to the parameterization.

{% code lang=csharp title="InSmartFolder usage for a specific content type" %}
var builder = new ContentItemQueryBuilder();

// Retrieves items of one specific content type from a smart folder
builder.ForContentTypes(parameters =>
{
    parameters.InSmartFolder(smartFolderGuid)
                .OfContentType("Sample.Type");
});
{% endcode %}

### SetUrlLanguageBehavior

Defines the behavior for the language of URL paths when web pages are retrieved in a fallback language. This method is particularly relevant when using {% inpage_link "InLanguage" linkText="InLanguage" %} and retrieving page URL information (e.g., through methods like {% page_link retrieve_page_urls_xp linkText="GetUrl()" %} after ensuring necessary columns are loaded, often with {% inpage_link ReferenceContentitemquery-ForWebsite linkText="ForWebsite" %} or {% inpage_link FCTs-WWPD linkText="WithWebPageData" %}).

It accepts a `UrlLanguageBehavior` enum value:

- `UrlLanguageBehavior.UseRequestedLanguage` -- When a page is served in a {% page_link OxT_Cw linkText="fallback language" anchor="Language fallbacks" %}, its URL path is generated based on the language originally requested by the `InLanguage` method. For instance, if Spanish is requested and an English page is returned as a fallback, the URL path will still be structured as if it were a Spanish page (e.g., using URL prefixes as configured in the {% page_link OxT_Cw anchor="Set up a new language" linkText="Languages" %} application).
- `UrlLanguageBehavior.UseFallbackLanguage` (Default) -- When a page is served in a fallback language, its URL path is generated based on the language of the actual fallback content. In the example below, the URL path would be structured as an English page.

{% code lang=csharp title="Using SetUrlLanguageBehavior" highlight=7 %}
var builder = new ContentItemQueryBuilder();

builder.ForContentTypes(parameters =>
{
    parameters.ForWebsite("MyWebsiteChannel")
              .WithWebPageData() // Ensure web page data is loaded
              .SetUrlLanguageBehavior(UrlLanguageBehavior.UseRequestedLanguage)
})
.InLanguage("spanish"); // Requesting content in Spanish

// After executing the query:
IEnumerable<SamplePageType> pages = await executor.GetMappedWebPageResult<SamplePageType>(builder);
foreach (var page in pages)
{
    // If 'page' is an English fallback for a Spanish request, 
    // its url will be in the format:
    //
    // '/<spanishLanguageName>/my-page-slug'
    //
    // due to 'UseRequestedLanguage'.
    WebPageUrl url = page.GetUrl();

}
{% endcode %}

### Where conditions

Where parameterization can be added to `ForContentTypes` queries using {% inpage_link Builder-Parameters linkText="Parameters" %}.

{% code lang=csharp title="Adding Where conditions" %}
var builder = new ContentItemQueryBuilder();
builder.ForContentTypes(parameters =>
{
    // ...
}).Parameters(parameters =>
{
    parameters.Where(where => where) //...
});
{% endcode %}

## Method details

Methods described in this section can only be called from within subqueries generated by a `ContentItemBuilder.ForContentTypes` call or a `ContentItemBuilder.ForContentType` call.

### WithLinkedItems {% anchor ReferenceContentitemquery-WithLinkedItemsMap %}

`IContentQueryExecutor.GetMappedResult`, `IContentQueryExecutor.GetMappedWebPageResult`, or using the provided `IContentQueryModelTypeMapper` automatically binds the linked content item hierarchy to the specified model.

{% code lang=csharp %}
IEnumerable<ModelClass> data =
            queryExecutor.GetMappedResult<ModelClass>(builder);
{% endcode %}

When mapping the data manually, use `GetLinkedItems` on the result to get the next level of references. This can be repeated up until the specified recursion level.

{% code lang=csharp %}
var data = queryExecutor.GetResult<ContentItemDto>(builder, resultSelector);

private ContentItemDto resultSelector(IContentQueryDataContainer itemData)
{
    var item = new ContentItemDto
    {
        Title = itemData.GetValue<string>("Title"),
        Text = itemData.GetValue<string>("Text"),

        // Retrieves first-level linked items from the 'Author' field
        AuthorName = itemData.GetLinkedItems("Author").First()
                                .GetValue<string>("Name");

        // Retrieves second-level linked items from the 'Author' field
        AuthorProfileBlurb = 
            itemData.GetLinkedItems("Author").First()
                    .GetLinkedItems("Profile").First()
                    .GetValue<string>("ProfileBlurb");
    };

    return item;
}
{% endcode %}

### Linking {% anchor ReferenceContentitemquery-Linking %}

Retrieves all content items which reference any of the content items from the provided collection in the specified field. Enables loading data on\-demand (lazily).

The following diagram illustrates the behavior on a simple content model:

{% image LinkingBasic.jpg title="Linking usage visualization" width=500 border=true %}

{% note %}

Combining this method with {% inpage_link "ReferenceContentitemquery-LinkedFrom" linkText="LinkedFrom" %} in a single subquery is not supported.

{% endnote %}

The method can also be used together with `WithLinkedItems`. For example:

{% code lang=csharp title="Linking and WithLinkedItems" %}
var builder = new ContentItemQueryBuilder();

// Retrieves items of the 'project.staff' content type
builder.ForContentType("project.staff", subqueryParameters =>
{
    // Retrieves all items of 'project.staff' type that reference any of the
    // items in 'managersCollection' from their 'ManagerField' together with
    // all second-level references for the selected 'project.staff' items
    subqueryParameters.Linking("ManagerField", managersCollection)
                    .WithLinkedItems(2);
});
{% endcode %}

{% image LinkingCombined.jpg title="Linking usage visualization" width=500 border=true %}

The collection of items for which to retrieve references must implement the `IContentItemIdentifier` interface. The interface ensures fields required by each data model that wants to leverage this method.  The fields must be bound to the model during model binding within `ContentQueryExecutor.GetResult`.

{% page_link 5IbWCQ linkText="Generated content type classes" %} already implement `IContentItemIdentifier` via the `SystemFields` property. The following approach applies for custom model classes.

{% code lang=csharp title="IContentItemIdentifier fields in custom model classes" %}
var data = queryExecutor.GetResult<ContentItemDto>(builder, resultSelector);

ManagerDto resultSelector(IContentQueryDataContainer itemData)
{
    var item = new ManagerDto
    {
        // Binds fields required by 'Linking'
        ContentItemID = itemData.ContentItemID,
        ContentItemLanguageID = itemData.ContentItemDataContentLanguageID,
        // other fields...
    };

    return item;
}

// Example custom model class binding content items of the 'Manager' content type
class ManagerDto : IContentItemIdentifier
{
    public int ContentItemID { get; }
    public int ContentLanguageID { get; }
    public IEnumerable<ContentItemReference> PictureField { get; }
    public IEnumerable<ContentItemReference> AwardsField { get; }
}
{% endcode %}

### LinkedFrom {% anchor ReferenceContentitemquery-LinkedFrom %}

Retrieves content items of a specific type linked from the given field in the provided collection. Enables loading data on\-demand (lazily).

The following diagram illustrates the behavior on a simple content model. Red arrows trace query evaluation:

{% image LinkedFromBasic.jpg title="LinkedFrom usage visualization" width=500 border=true %}

{% note %}

Combining this method with {% inpage_link "ReferenceContentitemquery-Linking" linkText="Linking" %} in a single subquery is not supported.

{% endnote %}

The method can also be used together with `WithLinkedItems`. For example:

{% code lang=csharp title="Linked from and WithLinkedItems" %}
var builder = new ContentItemQueryBuilder();

// Retrieves items of the 'project.staff' content type
builder.ForContentType("project.staff", subqueryParameters =>
{  
    // Items are retrieved for a collection of 'project.store' items and the field 'StaffField'
    // together with all first-level references for the selected 'project.staff' items
    subqueryParameters.LinkedFrom("project.store", "StaffField", storesCollection)
                        .WithLinkedItems(1);
});
{% endcode %}

{% image LinkedFromCombined.jpg title="LinkedFrom usage visualization" width=500 border=true %}

The collection of items for which to retrieve references must implement the `IContentItemIdentifier` interface. The interface ensures fields required by each data model that wants to leverage this method. The fields must be bound to the model during model binding within `ContentQueryExecutor.GetResult`.

{% page_link 5IbWCQ linkText="Generated content type classes" %} by default implement `IContentItemIdentifier` via the `SystemFields` property. The following approach applies for custom model classes.

{% code lang=csharp title="IContentItemIdentifier fields in custom model classes" %}
var data = queryExecutor.GetResult<ContentItemDto>(builder, resultSelector);

StoreDto resultSelector(IContentQueryDataContainer itemData)
{
    var item = new StoreDto
    {
        // Binds fields required by `LinkedFrom`
        ContentItemID = itemData.ContentItemID,
        ContentItemLanguageID = itemData.ContentItemDataContentLanguageID,
        // other fields...
    };

    return item;
}

// Example custom model class binding content items of the 'Store' content type
class StoreDto : IContentItemIdentifier
{
    public int ContentItemID { get; }
    public int ContentLanguageID { get; }
    public IEnumerable<ContentItemReference> StaffField { get; }
    public IEnumerable<ContentItemReference> ReferenceField { get; }
    public IEnumerable<ContentItemReference> FaqField { get; }
}
{% endcode %}

### LinkingSchemaField {% anchor FCT-LinkingSchemaField %}

This method can only be called from within subqueries generated by a `ContentItemBuilder.ForContentTypes` call.

Retrieves all content items that link to a collection of items via the specified {% page_link D4_OD linkText="reusable schema field" %}. {% inpage_link FCT-LinkedFromSchemaField linkText="LinkedFromSchemaField" %} complements this method by retrieving links from the opposite direction.

See the following diagram for an illustration of the method's behavior on a simplified content model. The query retrieves all content items that link to the images from `imageIdentifiers` via the *ProductImage* reusable schema field. Red arrows trace the query evaluation:

{% image LinkingSchemaFields.png title="LinkinkSchemaField method demonstration" width="700" border=true %}

In the diagram above, the method is called with the following parameters:

- `"ProductImage"` -- the code name of a field that belongs to a reusable field schema. Reusable schema field code names must be unique across the system, it's therefore sufficient to target fields directly.
- `imageIdentifiers` -- IDs of content items with the *Image* content type.

### LinkedFromSchemaField {% anchor FCT-LinkedFromSchemaField%}

Retrieves all content items that are linked from a collection of items via a {% page_link  D4_OD linkText="reusable schema field" %}. {% inpage_link FCT-LinkingSchemaField linkText="LinkingSchemaField" %} complements this method by retrieving links from the opposite direction.

See the following diagram for an illustration of the method's behavior on a simplified content model. The query retrieves all *Image* content items that are linked from `productIdentifiers` via the *ProductImage* reusable schema field. Red arrows trace the query evaluation:

{% image LinkedFromSchemaFields.png title="LinkedFromSchemaField method demonstration" width="700" border=true %}

In the diagram above, the method is called with the following parameters:

- `"ProductImage"` -- the code name of a field that belongs to a reusable field schema. Reusable schema field code names are unique and can be targeted directly without specifying the corresponding schema.
- `productIdentifiers` -- a collection of content items from content types using the *ProductFields* reusable field schema.

{% info icon=false %}
The collection of items for which to retrieve references must implement the `IContentItemIdentifier` interface. The interface ensures fields required by each data model that wants to leverage this method.

{% page_link 5IbWCQ linkText="Generated content type classes" %} by default implement `IContentItemIdentifier` via the `SystemFields` property.
{% endinfo %}

## IContentQueryExecutor configuration

Apart from configuring the query itself, you can also fine tune its execution. You can do so by setting the properties of the `ContentQueryExecutionOptions` attribute and providing it to the `IContentQueryExecutor` interface.

{% table %}
    {% row header=true %}
        {% cell %}
        Property
        {% endcell %}

        {% cell %}
        Description
        {% endcell %}
    {% endrow %}

    {% row %}
        {% cell %}
        `ForPreview`
        {% endcell %}

        {% cell %}
        If set to *true*, the query executor retrieves the queried items in their latest available version, regardless of their workflow state. Otherwise, the latest published version is retrieved.

        Default value: *false*.

        {% endcell %}
    {% endrow %}

    {% row %}
        {% cell %}
        `IncludeSecuredItems`
        {% endcell %}

        {% cell %}
        If set to *true*, the query executor retrieves all items according to the query, including secured items. Otherwise, only items that are not secured are included in the query.

        Default value: *false*.

        {% endcell %}
    {% endrow %}
{% endtable %}
To see how the configuration affects the data retrieval, check out the examples at {% page_link 4obWCQ linkText="Retrieve page content" %}.
