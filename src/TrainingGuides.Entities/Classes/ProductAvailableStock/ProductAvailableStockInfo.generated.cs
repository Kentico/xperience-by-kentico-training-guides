using System;
using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using TrainingGuides.ProductStock;

[assembly: RegisterObjectType(typeof(ProductAvailableStockInfo), ProductAvailableStockInfo.OBJECT_TYPE)]

namespace TrainingGuides.ProductStock
{
    /// <summary>
    /// Data container class for <see cref="ProductAvailableStockInfo"/>.
    /// </summary>
    public partial class ProductAvailableStockInfo : AbstractInfo<ProductAvailableStockInfo, IInfoProvider<ProductAvailableStockInfo>>, IInfoWithId
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "trainingguidees.productavailablestock";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(IInfoProvider<ProductAvailableStockInfo>), OBJECT_TYPE, "TrainingGuidees.ProductAvailableStock", "ProductAvailableStockID", null, null, null, null, null, null, null)
        {
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Product available stock ID.
        /// </summary>
        [DatabaseField]
        public virtual int ProductAvailableStockID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(ProductAvailableStockID)), 0);
            set => SetValue(nameof(ProductAvailableStockID), value);
        }


        /// <summary>
        /// Product available stock content item ID.
        /// </summary>
        [DatabaseField]
        public virtual int ProductAvailableStockContentItemID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(ProductAvailableStockContentItemID)), 0);
            set => SetValue(nameof(ProductAvailableStockContentItemID), value);
        }


        /// <summary>
        /// Product available stock value.
        /// </summary>
        [DatabaseField]
        public virtual decimal ProductAvailableStockValue
        {
            get => ValidationHelper.GetDecimal(GetValue(nameof(ProductAvailableStockValue)), 0m);
            set => SetValue(nameof(ProductAvailableStockValue), value);
        }


        /// <summary>
        /// Product stock SKU code.
        /// </summary>
        [DatabaseField]
        public virtual string ProductStockSKUCode
        {
            get => ValidationHelper.GetString(GetValue(nameof(ProductStockSKUCode)), String.Empty);
            set => SetValue(nameof(ProductStockSKUCode), value);
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            Provider.Delete(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            Provider.Set(this);
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="ProductAvailableStockInfo"/> class.
        /// </summary>
        public ProductAvailableStockInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="ProductAvailableStockInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public ProductAvailableStockInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}