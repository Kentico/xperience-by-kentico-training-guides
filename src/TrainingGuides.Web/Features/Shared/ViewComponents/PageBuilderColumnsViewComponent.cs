using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;

namespace TrainingGuides.Web.Features.Shared.ViewComponents;

public class PageBuilderColumnsViewComponent : ViewComponent
{
    private const string AREA_MAIN = "areaMain";
    private const string AREA_SECONDARY = "areaSecondary";
    private const string AREA_TERTIARY = "areaTertiary";

    private const string ZONE_MAIN = "zoneMain";
    private const string ZONE_SECONDARY = "zoneSecondary";
    private const string ZONE_TERTIARY = "zoneTertiary";

    private const string COL_XS = "col-md-3";
    private const string COL_S = "col-md-4";
    private const string COL_M = "col-md-6";
    private const string COL_L = "col-md-8";
    private const string COL = "col-md";

    private PageBuilderAreaType pageBuilderAreaType;

    private string MainIdentifier
    {
        get => pageBuilderAreaType == PageBuilderAreaType.EditableAreas ? AREA_MAIN : ZONE_MAIN;
        set { }
    }

    private string SecondaryIdentifier
    {
        get => pageBuilderAreaType == PageBuilderAreaType.EditableAreas ? AREA_SECONDARY : ZONE_SECONDARY;
        set { }
    }

    private string TertiaryIdentifier
    {
        get => pageBuilderAreaType == PageBuilderAreaType.EditableAreas ? AREA_TERTIARY : ZONE_TERTIARY;
        set { }
    }

    public IViewComponentResult Invoke(ColumnLayoutOption columnLayoutOption,
        PageBuilderAreaType pageBuilderAreaType,
        EditableAreaOptions editableAreaOptions)
    {
        this.pageBuilderAreaType = pageBuilderAreaType;

        int numberOfColumns = GetNumberOfColumns(columnLayoutOption);
        var columns = new List<PageBuilderColumnViewModel>();
        for (int index = 0; index < numberOfColumns; index++)
        {
            columns.Add(GetColumn(index, columnLayoutOption, editableAreaOptions));
        }

        var model = new PageBuilderColumnsViewModel
        {
            PageBuilderAreaType = pageBuilderAreaType,
            PageBuilderColumns = columns,
        };

        return View("~/Features/Shared/ViewComponents/PageBuilderColumns.cshtml", model);

    }

    private int GetNumberOfColumns(ColumnLayoutOption columnLayoutOption) => columnLayoutOption switch
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

    private PageBuilderColumnViewModel GetColumn(int columnIndex, ColumnLayoutOption columnLayoutOption, EditableAreaOptions editableAreaOptions)
    {
        string cssClass = string.Empty;
        string columnIdentifier;

        switch (columnLayoutOption)
        {
            case ColumnLayoutOption.TwoColumnEven:
                //first column is main
                cssClass = COL_M;
                columnIdentifier = columnIndex == 0 ? MainIdentifier : SecondaryIdentifier;
                break;
            case ColumnLayoutOption.TwoColumnLgSm:
                //first column is main
                if (columnIndex == 0)
                {
                    cssClass += COL_L;
                    columnIdentifier = MainIdentifier;
                }
                else
                {
                    cssClass += COL_S;
                    columnIdentifier = SecondaryIdentifier;
                }
                break;
            case ColumnLayoutOption.TwoColumnSmLg:
                //second column is main
                if (columnIndex == 0)
                {
                    cssClass += COL_S;
                    columnIdentifier = SecondaryIdentifier;
                }
                else
                {
                    cssClass += COL_L;
                    columnIdentifier = MainIdentifier;
                }
                break;
            case ColumnLayoutOption.ThreeColumnEven:
                //middle column is main
                cssClass += COL_S;
                columnIdentifier = columnIndex == 1
                    ? MainIdentifier
                    : columnIndex == 0 ?
                        SecondaryIdentifier : TertiaryIdentifier;
                break;
            case ColumnLayoutOption.ThreeColumnSmLgSm:
                //middle column is main
                if (columnIndex == 1)
                {
                    cssClass += COL_M;
                    columnIdentifier = MainIdentifier;
                }
                else
                {
                    cssClass += COL_XS;
                    columnIdentifier = columnIndex == 0 ?
                        SecondaryIdentifier : TertiaryIdentifier;
                }
                break;
            case ColumnLayoutOption.OneColumn:
            default:
                //sole column is main
                columnIdentifier = MainIdentifier;
                cssClass += COL;
                break;
        }

        return new PageBuilderColumnViewModel
        {
            CssClass = cssClass,
            Identifier = columnIdentifier,
            EditableAreaOptions = editableAreaOptions
        };
    }
}


public enum PageBuilderAreaType
{
    EditableAreas = 0,
    WidgetZones = 1,
}
