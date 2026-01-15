using CMS.DataEngine;

namespace TrainingGuides.ProductStock;

public partial class ProductAvailableStockInfo
{
    static ProductAvailableStockInfo()
    {
        TYPEINFO.DependsOn =
        [
            new ObjectDependency(nameof(ProductAvailableStockID), "cms.contentitem", ObjectDependencyEnum.Required)
        ];
    }
}