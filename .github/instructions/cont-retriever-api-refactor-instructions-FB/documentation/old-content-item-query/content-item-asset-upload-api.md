---
    title: Content item asset upload API
    persona: developer
    identifier: ci_upload_api_xp
    order: 500
    license: 1

    redirect_from: x/ci_upload_api_xp

    related_pages: ['WhT_Cw']
---

The system provides a supporting API for managing content item asset binaries (for content items with *Content item asset* {% page_link RIXWCQ linkText="fields" %}). When creating or updating asset binaries, you control where the data is sourced via the `IContentItemAssetSource` interface and its implementations.

You can use the following default implementations:

1. `ContentItemAssetFileSource` – sources the binary data from a location on the file system. When constructing this source, provide:
    1. filePath – the location of the asset binary.
    2. canMove – if true, moves the file from the source location to the *~/assets* folder in the Xperience application instead of creating a copy.
2. `ContentItemAssetStreamSource` – sources the binary from a {% external_link "https://learn.microsoft.com/en-us/dotnet/api/system.io.stream" linkText="Stream" %}. When constructing this source, provide:
    1. Func\<Task\<Stream\>\> – a delegate that returns the `Stream` object.

The source is passed when constructing `ContentItemAssetMedataWithSource` objects that are used when updating or creating `ContentItemData` (the class responsible for populating content item fields) via `IContentItemManager`.

The following excerpt only demonstrates `IContentItemAssetSource` instantiation and usage. See the {% page_link 0BX_Cw collection="api" linkText="Content items" %} API examples for full code samples.

{% info %}

Use the {% page_link 4YfWCQ linkText="files API" %} that can be found in the **CMS.IO** namespace instead of the default **System.IO** library to ensure compatibility with various types of storage providers.

{% endinfo %}

{% code lang=csharp title="ContentItemAssetFileSource usage" %}

using CMS.IO;

// ...

// Gets the metadata for content item creation
CreateContentItemParameters createParams = //...

// Gets the file information from a file on the file system
var assetPath = "C:/data/images/store-logo.jpg";
var file = FileInfo.New(assetPath);

// Creates a metadata object that describes the uploaded asset
var assetMetadata = new ContentItemAssetMetadata()
{
    Extension = file.Extension,
    Identifier = Guid.NewGuid(),
    LastModified = DateTime.Now,
    Name = file.Name,
    Size = file.Length
};

// Creates a 'ContentItemAssetFileSource' object used to locate the asset and merges it with the metadata object
var fileSource = new ContentItemAssetFileSource(file.FullName, false);
var assetMetadataWithSource = new ContentItemAssetMetadataWithSource(fileSource, assetMetadata);

// Passes the metadata and the binary source to ContentItemData
ContentItemData itemData = new ContentItemData(new Dictionary<string, object>{
    { "StoreLogo", assetMetadataWithSource }
});

// Creates the content item 
await contentItemManager.Create(createParams, itemData);

{% endcode %}

### Custom implementations

You can also provide custom implementations of `IContentItemAssetSource` that retrieve the binary data according to your preference.

The interface requires the following members:

- **HasData** – a boolean property that determines whether the source carries updated binary data. If false, the asset binary is not updated in the *~/assets* folder on the file system when updating the corresponding content item.
- Task **Write**(string targetAssetFilePath, CancellationToken cancellationToken) – method that writes the binary data to `targetAssetFilePath`. Invoked by the system when processing management operations over asset binaries.

The following sample implementation sources the asset binary from a URI:

{% code lang=csharp title="ContentItemAssetUriSource.cs" %}

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using CMS.ContentEngine;
using CMS.IO;

public class ContentItemAssetUriSource : IContentItemAssetSource, IDisposable
{
    private static readonly HttpClient httpClient = new HttpClient();
    private Uri assetUri;
    private HttpContent assetContent;

    // Determines whether the asset binary should be updated.
    // Always set to true - the system assumes fresh binary data every time 
    // an instance of this class is provided.
    public bool HasData => true;

    public ContentItemAssetUriSource(string assetUri)
    {
        this.assetUri = new Uri(assetUri);
    }

    // Gets meta information about the asset
    public async Task<ContentItemAssetMetadata> GetAssetMetadata(CancellationToken cancellationToken = default)
    {
        await EnsureAssetContent(cancellationToken);

        return new ContentItemAssetMetadata()
        {
            Extension = Path.GetExtension(assetUri.LocalPath),
            Identifier = Guid.NewGuid(),
            LastModified = DateTime.UtcNow,
            Name = Path.GetFileName(assetUri.LocalPath),
            Size = assetContent.Headers.ContentLength.GetValueOrDefault()
        };
    }

    // Writes the asset binary from the Http request to the file system.
    // Invoked by Xperience from within content item management operations (IContentItemManager).
    public async Task Write(string targetAssetFilePath, CancellationToken cancellationToken = default)
    {
        await EnsureAssetContent(cancellationToken);

        // Creates the target file path if it doesn't exist
        CMS.IO.DirectoryHelper.EnsureDiskPath(targetAssetFilePath, string.Empty);

        // Copies the asset binary to the target path
        using (var targetStream = File.Open(targetAssetFilePath, FileMode.Create, FileAccess.Write))
        {
            var assetStream = await assetContent.ReadAsStreamAsync(cancellationToken);
            await assetStream.CopyToAsync(targetStream, cancellationToken);
        }
    }

    private async Task EnsureAssetContent(CancellationToken cancellationToken = default)
    {
        if (assetContent == null)
        {
            var response = await httpClient.GetAsync(assetUri, cancellationToken);

            assetContent = response.Content;
        }
    }

    public void Dispose()
    {
        assetContent?.Dispose();
    } 
}

{% endcode %}
