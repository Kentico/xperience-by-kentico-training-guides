using System.ComponentModel;
using System.Text.RegularExpressions;
using CMS.Core;
using Moq;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Shared.OptionProviders;

public class DropdownEnumOptionProviderTests
{
    public enum SampleOption
    {
        [Description("{$Sample.Localized$}")]
        Localized = 0,

        [Description("Plain text")]
        Plain = 1,

        WithoutDescription = 2
    }

    private static readonly Dictionary<string, string> resourceStrings = new()
    {
        ["Sample.Localized"] = "Localisé"
    };

    private readonly DropdownEnumOptionProvider<SampleOption> provider;

    public DropdownEnumOptionProviderTests()
    {
        // Mirrors LocalizeString: replaces {$key$} expressions and leaves any other text alone.
        var localizationService = new Mock<ILocalizationService>();
        localizationService
            .Setup(service => service.LocalizeString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns((string text, string _, bool _, bool _) =>
                Regex.Replace(text, @"\{\$(.+?)\$\}", match => resourceStrings[match.Groups[1].Value]));

        provider = new DropdownEnumOptionProvider<SampleOption>(localizationService.Object);
    }

    [Fact]
    public async Task GetOptionItems_LocalizationExpressionInDescription_ComesBackLocalized()
    {
        var items = await provider.GetOptionItems();

        Assert.Equal("Localisé", items.Single(item => item.Value == nameof(SampleOption.Localized)).Text);
    }

    // Enums shared with other components keep plain-text descriptions, so they must come back as they are.
    [Fact]
    public async Task GetOptionItems_PlainDescriptionOrNone_ComesBackUnchanged()
    {
        var items = (await provider.GetOptionItems()).ToList();

        Assert.Equal("Plain text", items.Single(item => item.Value == nameof(SampleOption.Plain)).Text);
        Assert.Equal(
            nameof(SampleOption.WithoutDescription),
            items.Single(item => item.Value == nameof(SampleOption.WithoutDescription)).Text);
    }

    // Values are stored on components by member name, so localizing must not change them or their order.
    [Fact]
    public async Task GetOptionItems_Values_AreMemberNamesInDeclarationOrder()
    {
        var items = await provider.GetOptionItems();

        Assert.Equal(
            [nameof(SampleOption.Localized), nameof(SampleOption.Plain), nameof(SampleOption.WithoutDescription)],
            items.Select(item => item.Value));
    }
}
