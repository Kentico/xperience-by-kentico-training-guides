using CMS.Core;
using Moq;
using Xunit;

namespace TrainingGuides.Web.Tests;

/// <summary>
/// Initializes the process-wide Xperience service container with a mock
/// <see cref="IAppSettingsService"/> for APIs such as <c>ImageHelper.IsImage</c>.
/// </summary>
public sealed class ServiceContainerFixture
{
    public ServiceContainerFixture()
    {
        // Qualify CMS.Core.Service to avoid ambiguity with the TrainingGuides.Service content type.
        CMS.Core.Service.Use<IAppSettingsService>(new Mock<IAppSettingsService>().Object);
        CMS.Core.Service.InitializeContainer();
    }
}

/// <summary>
/// xUnit test collection that shares a single <see cref="ServiceContainerFixture"/>
/// instance across test classes.
/// </summary>
[CollectionDefinition(NAME)]
public class ServiceContainerCollection : ICollectionFixture<ServiceContainerFixture>
{
    /// <summary>
    /// Collection name used in <see cref="CollectionAttribute"/>.
    /// </summary>
    public const string NAME = "Service container";
}
