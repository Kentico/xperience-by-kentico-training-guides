using Microsoft.AspNetCore.Components;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.RazorComponents;

public partial class EmailBuilderColumns : ComponentBase
{
    private const string AREA_MAIN = "MainContent";
    private const string AREA_SECONDARY = "SecondaryContent";
    private const string AREA_TERTIARY = "TertiaryContent";

    private EmailBuilderColumnsModel Model { get; set; } = new();

    private Dictionary<string, object> AreaAttributes { get; set; } = [];

    private int NumberOfColumns => ColumnLayout switch
    {

        ColumnLayoutOption.TwoColumnEven or
        ColumnLayoutOption.TwoColumnLgSm or
        ColumnLayoutOption.TwoColumnSmLg
        => 2,
        ColumnLayoutOption.ThreeColumnEven or
        ColumnLayoutOption.ThreeColumnSmLgSm
        => 3,
        ColumnLayoutOption.OneColumn or
        _
        => 1
    };

    [Inject]
    private IComponentStyleEnumService ComponentStyleEnumService { get; set; } = null!;

    [Parameter]
    public ColumnLayoutOption ColumnLayout { get; set; }

    [Parameter]
    public CornerStyleOption CornerStyle { get; set; } = CornerStyleOption.Sharp;

    [Parameter]
    public ColorSchemeOption Column1ColorScheme { get; set; } = ColorSchemeOption.Light1;

    [Parameter]
    public ColorSchemeOption Column2ColorScheme { get; set; } = ColorSchemeOption.Light1;

    [Parameter]
    public ColorSchemeOption Column3ColorScheme { get; set; } = ColorSchemeOption.Light1;

    [Parameter]
    public IEnumerable<string> AllowedWidgets { get; set; } = Enumerable.Empty<string>();

    [Parameter]
    public IEnumerable<string> AllowedSections { get; set; } = Enumerable.Empty<string>();

    protected override void OnInitialized()
    {
        if (AllowedWidgets.Where(str => !string.IsNullOrWhiteSpace(str)).Any())
            AreaAttributes.Add(nameof(AllowedWidgets), AllowedWidgets);
        if (AllowedSections.Where(str => !string.IsNullOrWhiteSpace(str)).Any())
            AreaAttributes.Add(nameof(AllowedSections), AllowedSections);

        Model = new EmailBuilderColumnsModel
        {
            Columns = GetColumns(),
        };
    }

    private IEnumerable<EmailBuilderColumnModel> GetColumns()
    {
        var columns = new List<EmailBuilderColumnModel>();

        int numberOfColumns = NumberOfColumns;
        for (int i = 0; i < numberOfColumns; i++)
        {
            columns.Add(GetColumn(i));
        }

        return columns;
    }

    private EmailBuilderColumnModel GetColumn(int columnIndex)
    {
        string columnIdentifier;
        int width;
        var cssClasses = GetColumnCssClasses(columnIndex).ToList();
        cssClasses.AddRange(ComponentStyleEnumService.GetCornerStyleClasses(CornerStyle));
        cssClasses.Add("tg-tiny-margin");

        switch (ColumnLayout)
        {
            case ColumnLayoutOption.TwoColumnEven:
                //first column is main
                width = 49;
                columnIdentifier = columnIndex == 0
                    ? AREA_MAIN
                    : AREA_SECONDARY;
                break;
            case ColumnLayoutOption.TwoColumnLgSm:
                //first column is main
                if (columnIndex == 0)
                {
                    width = 64;
                    columnIdentifier = AREA_MAIN;
                }
                else
                {
                    width = 35;
                    columnIdentifier = AREA_SECONDARY;
                }
                break;
            case ColumnLayoutOption.TwoColumnSmLg:
                //second column is main
                if (columnIndex == 0)
                {
                    width = 35;
                    columnIdentifier = AREA_SECONDARY;
                }
                else
                {
                    width = 64;
                    columnIdentifier = AREA_MAIN;
                }
                break;
            case ColumnLayoutOption.ThreeColumnEven:
                //middle column is main
                width = 32;
                columnIdentifier = columnIndex == 1
                    ? AREA_MAIN
                    : columnIndex == 0
                        ? AREA_SECONDARY
                        : AREA_TERTIARY;
                break;
            case ColumnLayoutOption.ThreeColumnSmLgSm:
                if (columnIndex == 1)
                {
                    width = 38;
                    columnIdentifier = AREA_MAIN;
                }
                else
                {
                    width = 30;
                    columnIdentifier = columnIndex == 0
                        ? AREA_SECONDARY
                        : AREA_TERTIARY;
                }
                break;
            case ColumnLayoutOption.OneColumn:
            default:
                //sole column is main
                width = 99;
                columnIdentifier = AREA_MAIN;
                break;
        }

        return new EmailBuilderColumnModel
        {
            Width = width,
            CssClasses = cssClasses,
            Identifier = columnIdentifier,
            AllowedWidgets = AllowedWidgets,
            AllowedSections = AllowedSections
        };
    }

    private IEnumerable<string> GetColumnCssClasses(int columnIndex) => columnIndex switch
    {
        0 => ComponentStyleEnumService.GetColorSchemeClasses(Column1ColorScheme),
        1 => ComponentStyleEnumService.GetColorSchemeClasses(Column2ColorScheme),
        2 => ComponentStyleEnumService.GetColorSchemeClasses(Column3ColorScheme),
        _ => Enumerable.Empty<string>()
    };
}