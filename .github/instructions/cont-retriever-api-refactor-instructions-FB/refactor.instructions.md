---
applyTo: '**'
---
# ContentRetriever API refactoring

## Overview

This document outlines the rules and steps to refactor the Training guides repository to replace the old Content item query API with the new ContentRetriever API. 

## Steps

The refactor will happen in two steps - do not move to the next step before getting a confirmation "Proceed with step 2". The steps are:

1. Create a copy of the `ContentItemRetrieverService.cs` and `IContentItemRetrieverService.cs` and refactor ONLY the methods in them to use the new ContentRetrieval API. Do not build the application after this step is completed as it is expected to see build errors at this point. However, when refactoring, take into consideration the context of this entire repository.

2. Upon getting instructed to "Proceed with step 2", refactor code in the whole repo to use methods from the refactored `ContentItemRetrieverService` class.

3. Upon getting instructed to "Proceed with step 3" refactor all the unit tests in the *TrainingGuides.Web.Tests* project to match the refactored code of *TrainingGuides.Web*.

## Code formatting

- abide by the rules defined in the .editorconfig file, located in the root of this project

## Rules

### For all steps
- if you find that you can extract or reuse code to avoid repetitiveness, feel free to do so. DO NOT remove unused methods.
- Remove unused variables and references.
- if you run out of your output window space, let me know and I will divide your task into batches.
- if you run out of your context window space, let me know and I will divide your task into batches.
- if you have trouble reading or understanding any of the files listed in the *Context and resources* section, notify me.
- do not build or run the solution without my consent.
- All methods of the ContentRetriever need concrete content type classes that have the RegisterContentTypeMappingAttribute, not base interfaces. Using for example:
`contentRetriever.RetrievePages<IWebPageFieldsSource>(...)` will lead to runtime errors.

### For step 1
- name the file copies of the `ContentItemRetrieverService.cs` and `IContentItemRetrieverService.cs` as `ContentItemRetrieverService.cs.backup` and `IContentItemRetrieverService.cs.backup` respectively. Make changes to the original files only, not the copies.

- if the old method is retrieving pages or content items by GUID/GUIDs, opt for RetrieveContentByGuids of the ContentRetriever API before any other methods. For example:
```
public async Task<T?> RetrieveWebPageByContentItemGuid(
    Guid contentItemGuid,
    string contentTypeName,
    int depth = 1,
    string? languageName = null)
{
    var pages = await RetrieveWebPageContentItems(
        contentTypeName,
        innerParams => innerParams.WithLinkedItems(depth),
        outerParams => outerParams
            .Where(where => where.WhereEquals(nameof(ContentItemFields.ContentItemGUID), contentItemGuid)),
        languageName: languageName);
    return pages.FirstOrDefault();
}
```
can be simplified to:

```
public async Task<T?> RetrieveWebPageByContentItemGuid(
    Guid contentItemGuid,
    int depth = 1,
    string? languageName = null)
{
    var parameters = new RetrieveContentParameters
    {
        LinkedItemsMaxLevel = depth,
        LanguageName = languageName
    };

    var pages = await contentRetriever.RetrieveContentByGuids<T>(
        [contentItemGuid],
        parameters);

    return pages.FirstOrDefault();
}
```
<!-- note because our project is using only the Combined content selector-->
- do not use the RetrievePagesByGuids method at all
- if the old method's parameters only allow to retrieve one content type, opt for RetrieveContent of the ContentRetriever API before the RetrieveContentOfContentTypes.
- if the old method's parameters only allow to retrieve one content type, opt for RetrievePages of the ContentRetriever API before the RetrievePagesOfContentTypes.
- you may decrease the number of parameters the new methods take, if not all of them are needed to call the new Content retriever API
- if the options of the new Content retrieval API do not cover a specific scenario, and you cannot cover it by using the `additionalQueryConfiguration` parameter, mark the respective method with a comment to let me know and skip the refactoring of this method
- if you see methods in a generically typed class, refactor the class to be non-generic, while keeping the methods generic.
- disable cashing by using RetrievalCacheSettings.CacheDisabled
- Avoid using empty parameters of the ContentRetriever API methods,like query => { }, if possible.

### For step 2

- refactor code in the whole repo to use methods from the refactored `ContentItemRetrieverService` class.
- When retrieving the current page in a code, similar to this:

```
var context = webPageDataContextRetriever.Retrieve();
var articlePage = await contentItemRetrieverService.RetrieveWebPageById<ArticlePage>(
	context.WebPage.WebPageItemID,
	2);
```

Utilize RetrieveCurrentPage of the ContentRetriever API instead. If the repo is using a central class that holds all the calls of ContentRetriever, avoid calling ContentRetriever.RetrieveCurrentPage in the code directly. Instead wrap the call into a method of the central class' method. For example, the code above would change into:
```
var articlePage = await contentItemRetrieverService.RetrieveCurrentPage<ArticlePage>(2);

```
- if a class contains more than one instance of IContentItemRetrieverService, consolidate them into one. E.g.: 

    Old code:
    ```
    private readonly IContentItemRetrieverService genericPageRetrieverService;
    private readonly IContentItemRetrieverService<ArticlePage> articlePageRetrieverService; 
    private readonly IContentItemRetrieverService<Foo> fooRetrieverService; 
    ```

    Should simplify into:

    `private readonly IContentItemRetrieverService contentItemRetrieverService;`

- if a class contains an instance of IContentItemRetrieverService, always name the instance `contentItemRetrieverService`.

- TODO - describe better // string language = preferredLanguageRetriever.Get();
  // string articleUrl = (await webPageUrlRetriever.Retreve(articlePage, language)).RelativePath;
        string articleUrl = articlePage.GetUrl().RelativePath;

## Context and resources

- Old Content Item query API documentation files in MD format: ./documentation/old-content-item-query
- New ContentRetrieval API documentation files in MD format: ./documentation/new-content-retriever

- Code files from the **Dancing goat** repository in the *./examples/dg* folder:
    - OLD folder contains files using the old approach
    - NEW folder contains files after they have been refactored using the new ContentRetriever approach

- Code files from the **Kickstart** repository in the *./examples/kickstart* folder:
    - OLD folder contains files using the old approach
    - NEW folder contains files after they have been refactored using the new ContentRetriever approach

- Code files from the main branch of this **Training guides** repository in the *./examples/training-guides* folder:
    - OLD folder contains files using the old approach
    - NEW folder contains files after they have been refactored using the new ContentRetriever approach
    - these are files from the "starter" branch of this educational repository. The starter branch is a starting point for people to follow along with various exercises and guides to eventually get to the current state of this repository.