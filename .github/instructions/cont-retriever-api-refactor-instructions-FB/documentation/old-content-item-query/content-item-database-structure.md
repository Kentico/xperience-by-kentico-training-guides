---
    title: Content item database structure
    persona: developer
    identifier: XBT_Cw
    order: 600
    license: 1

    redirect_from: x/XBT_Cw
---

This page introduces the database structure of various system entities used to manage content in Xperience.

## Content items

Content items, managed via the **Content Hub** application and the `IContentItemManager` {% page_link kAQcCQ collection="api" linkText="management API" %}, are composed of the following entities:

{% table %}

{% row header=true %}

{% cell %}
Database table
{% endcell %}

{% cell %}
Description
{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_ContentItem  

{% endcell %}

{% cell %}
Stores system information about the content item.

- The item's content type is stored by the **ContentTypeId** column.
- The **ContentItemIsReusable** flag determines whether the item can be linked across multiple channels (i.e., is a reusable content item managed via the *Content Hub* application) or is tied to a specific channel (i.e., a {% inpage_link "Pages" linkText="page" %} or an {% inpage_link "Emails" linkText="email" %}).

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_ContentItemCommonData
{% endcell %}

{% cell %}
Stores content item properties and data of Page Builder widgets (for page content items) and {% page_link D4_OD linkText="reusable field schema" %} fields.

The table stores a separate record for each {% page_link OxT_Cw linkText="language variant" %} of an item.

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_ContentItemLanguageMetadata
{% endcell %}

{% cell %}
Contains data about the item displayed within the admin UI, such as the display name, creator, and last updated/modified date.

The table stores a separate record for each language variant of an item.

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
*ContentItemData* tables

{% endcell %}

{% cell %}
Store {% page_link gYHWCQ linkText="content type fields" %}. Created together with the corresponding content types.

The tables use the following naming structure: **\<ContentTypeNamespace\>.\<ContentTypeName\>**

For example, a *DancingGoat.Coffee* content type has a table named *DancingGoat\_Coffee*. The table's columns correspond with the {% page_link RIXWCQ linkText="fields" %} defined for the content type.

- Records in the data tables are bound to content items via the **CMS\_ContentItemCommonData** table.
- The table stores a separate record for each language variant of an item.

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_ContentItemReference

{% endcell %}

{% cell %}
Stores references between items, enabling {% page_link f4HWCQ collection="guides" linkText="content composition" %}.

{% endcell %}

{% endrow %}
{% endtable %}
When managing content items in code, the API abstracts work with all these entities behind a conventional CRUD manager (`IContentItemManager`).

Similarly, when consuming the data – to display on a website, for example – the content item query API merges all entities into a single record from which all data and meta information can be extracted.

The following diagram illustrates the relationship between entities composing a single content item. Only the most important columns are highlighted.

{% image CiStructure.png title="Content item database composition" width=650 border=true %}

## Pages

Pages, managed via website channel applications and the `IWebPageManager` {% page_link kAQcCQ collection="api" linkText="API" %}, extend the {% inpage_link "Content items" linkText="content item structure" %} with additional entities that provide web\-specific, contextual data and metadata.

{% table %}

{% row header=true %}

{% cell %}
Database table
{% endcell %}

{% cell %}
Description
{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_Channel
{% endcell %}

{% cell %}
Unlike standalone content items, pages must be tied directly to website channels.

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_WebsiteChannel
{% endcell %}

{% cell %}
Stores data about the {% page_link 34HFC linkText="website channel" %} – main domain, primary language, default {% page_link 1YB1CQ linkText="cookie level" %}, etc.

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_WebPageItem

{% endcell %}

{% cell %}
Stores metadata about pages, such as the location and order in the page content tree (**TreePath** and **Order** columns).

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_WebPageUrlPath

{% endcell %}

{% cell %}
Stores the relative URL path to pages.

The table stores a separate record for each language variant of a page.

{% endcell %}

{% endrow %}

{% row  %}
{% cell %}
CMS_WebPageAcl
{% endcell %}

{% cell %}
Stores information about content tree sections to which a specific set of {% page_link permissions_pagelevel_xp linkText="page permissions" %} is applied.
{% endcell %}
{% endrow %}

{% row  %}
{% cell %}
CMS_WebPageAclRole
{% endcell %}

{% cell %}
Stores information about which roles are present in which sets of page permissions.
{% endcell %}
{% endrow %}

{% row  %}
{% cell %}
CMS_WebPageAclRolePermission
{% endcell %}

{% cell %}
Stores specific page permissions granted to roles.
{% endcell %}
{% endrow %}

{% endtable %}
The following diagram illustrates the relationship between entities composing a single page. Only the most important columns are highlighted.

{% image PageStructure.png title="Page database entity composition" width=650 border=true %}

## Emails

Emails, managed via email channel applications, extend the {% inpage_link "Content items" linkText="content item structure" %} with additional entities that provide email\-specific, contextual data and metadata.

{% table %}

{% row header=true %}

{% cell %}
Database table
{% endcell %}

{% cell %}
Description
{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_Channel
{% endcell %}

{% cell %}
Unlike standalone content items, emails must be tied directly to email channels.

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
EmailLibrary\_EmailChannel
{% endcell %}

{% cell %}
Stores data about the {% page_link eBT_Cw linkText="email channel" %} – sending domain, language, etc.

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
EmailLibrary\_EmailConfiguration

{% endcell %}

{% cell %}
Stores the configuration of individual emails (email name, assigned template, etc.).

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
EmailLibrary\_SendConfiguration

{% endcell %}

{% cell %}
Stores the sending configuration of individual regular emails (scheduled send date, etc.).

{% endcell %}

{% endrow %}
{% endtable %}
The following diagram illustrates the relationship between entities composing a single email. Only the most important columns are highlighted.

{% image EmailStructure.png title="Email content item database entity composition" width=650 border=true %}

## Headless items

Headless items, managed via headless channel applications, extend the {% inpage_link "Content items" linkText="content item structure" %} with additional entities that provide headless\-specific, contextual data and metadata.

{% table %}

{% row header=true %}

{% cell %}
Database table
{% endcell %}

{% cell %}
Description
{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_Channel
{% endcell %}

{% cell %}
Unlike standalone content items, headless items must be tied directly to headless channels.

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_HeadlessChannel
{% endcell %}

{% cell %}
Stores data about the {% page_link nYWOD linkText="headless channel" %} – preview URL, primary language, etc.

{% endcell %}

{% endrow %}

{% row %}
{% cell %}
CMS\_HeadlessItem

{% endcell %}

{% cell %}
Stores the configuration of individual headless items (headless item name, its headless channel ID, etc.).

{% endcell %}

{% endrow %}

{% endtable %}
The following diagram illustrates the relationship between entities composing a single headless item. Only the most important columns are highlighted.

{% image HeadlessStructure.png title="Headless content item database entity composition" width="650" border=true %}
