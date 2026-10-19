using CMS.Core;
using Moq;
using Xunit;

namespace TrainingGuides.Web.Tests;

/// <summary>
/// Initializes the process-wide Xperience service container once with a settings service,
/// for code such as <c>ImageHelper.IsImage</c> that reads settings through it.
/// </summary>
public sealed class ServiceContainerFixture
{
    public ServiceContainerFixture()
    {
        // CMS.Core.Service is qualified because TrainingGuides.Service is a content type
        CMS.Core.Service.Use<IAppSettingsService>(new Mock<IAppSettingsService>().Object);
        CMS.Core.Service.InitializeContainer();
    }
}

/// <summary>
/// Test classes that need the service container join this collection with
/// <c>[Collection(ServiceContainerCollection.NAME)]</c>, so they share one initialization and never run in parallel.
/// </summary>
[CollectionDefinition(NAME)]
public class ServiceContainerCollection : ICollectionFixture<ServiceContainerFixture>
{
    /// <summary>
    /// Name of the collection.
    /// </summary>
    public const string NAME = "Service container";
}
