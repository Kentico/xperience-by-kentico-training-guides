//--------------------------------------------------------------------------------------------------
// <auto-generated>
//
//     This code was generated by code generator tool.
//
//     To customize the code use your own partial class. For more info about how to use and customize
//     the generated code see the documentation at https://docs.xperience.io/.
//
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CMS.ContentEngine;

namespace TrainingGuides
{
	/// <summary>
	/// Represents a content item of type <see cref="Product"/>.
	/// </summary>
	public partial class Product : IContentItemFieldsSource
	{
		/// <summary>
		/// Code name of the content type.
		/// </summary>
		public const string CONTENT_TYPE_NAME = "TrainingGuides.Product";


		/// <summary>
		/// Represents system properties for a content item.
		/// </summary>
		public ContentItemFields SystemFields { get; set; }


		/// <summary>
		/// ProductName.
		/// </summary>
		public string ProductName { get; set; }


		/// <summary>
		/// ProductShortDescription.
		/// </summary>
		public string ProductShortDescription { get; set; }


		/// <summary>
		/// ProductMedia.
		/// </summary>
		public IEnumerable<Asset> ProductMedia { get; set; }


		/// <summary>
		/// ProductDescription.
		/// </summary>
		public string ProductDescription { get; set; }


		/// <summary>
		/// ProductBenefits.
		/// </summary>
		public IEnumerable<Benefit> ProductBenefits { get; set; }


		/// <summary>
		/// ProductFeatures.
		/// </summary>
		public IEnumerable<ProductFeature> ProductFeatures { get; set; }


		/// <summary>
		/// ProductPrice.
		/// </summary>
		public decimal ProductPrice { get; set; }
	}
}