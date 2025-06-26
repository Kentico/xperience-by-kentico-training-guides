using Kentico.EmailBuilder.Web.Mvc;
using Microsoft.AspNetCore.Components;
using Moq;
using TrainingGuides.Web.Features.Newsletters.NatureSpotlight;
using TrainingGuides.Web.Features.Shared.Services;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Newsletters.NatureSpotlight;

public class NatureSpotlightEmailServiceTests
{
    private const string Topic = "Frogs";
    private const string Text = "Frogs are fascinating amphibians.";
    private const string Country = "TestCountry";

    [Fact]
    public async Task GetNatureSpotlightEmailModel_ReturnsExpectedModel()
    {
        // Arrange
        var email = new NatureSpotlightEmail
        {
            NatureSpotlightTopic = Topic,
            NatureSpotlightText = Text,
            NatureSpotlightCountries = [Guid.NewGuid()],
            NatureSpotlightRelatedArticles = [],
            NatureSpotlightImages = []
        };

        var emailContextMock = new Mock<IEmailContextAccessor>();
        var emailContext = new Mock<EmailContext>();
        emailContext.Setup(x => x.GetEmail<NatureSpotlightEmail>(default)).ReturnsAsync(email);
        emailContextMock.Setup(x => x.GetContext()).Returns(emailContext.Object);

        var countryServiceMock = new Mock<ICountryService>();
        countryServiceMock.Setup(x => x.GetCountriesByGuids(It.IsAny<IEnumerable<Guid>>()))
            .Returns([Country]);

        var service = new NatureSpotlightEmailService(emailContextMock.Object, countryServiceMock.Object);

        // Act
        var result = await service.GetNatureSpotlightEmailModel();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Topic, result.Topic);
        Assert.Equal(new MarkupString(Text), result.Text);
    }
}
