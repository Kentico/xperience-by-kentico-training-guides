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
	/// Defines a contract for content types with the <see cref="IArticleSchema"/> reusable schema assigned.
	/// </summary>
	public interface IArticleSchema
	{
		/// <summary>
		/// Code name of the reusable field schema.
		/// </summary>
		public const string REUSABLE_FIELD_SCHEMA_NAME = "ArticleSchema";


		/// <summary>
		/// ArticleSchemaTitle.
		/// </summary>
		public string ArticleSchemaTitle { get; set; }


		/// <summary>
		/// ArticleSchemaTeaser.
		/// </summary>
		public IEnumerable<Asset> ArticleSchemaTeaser { get; set; }


		/// <summary>
		/// ArticleSchemaSummary.
		/// </summary>
		public string ArticleSchemaSummary { get; set; }


		/// <summary>
		/// ArticleSchemaText.
		/// </summary>
		public string ArticleSchemaText { get; set; }


		/// <summary>
		/// ArticleSchemaRelatedArticles.
		/// </summary>
		public IEnumerable<Article> ArticleSchemaRelatedArticles { get; set; }


		/// <summary>
		/// ArticleSchemaTaxonomy.
		/// </summary>
		public IEnumerable<TagReference> ArticleSchemaTaxonomy { get; set; }
	}
}