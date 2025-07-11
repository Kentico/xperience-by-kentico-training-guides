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
using CMS.EmailLibrary;

namespace TrainingGuides
{
	/// <summary>
	/// Represents an email of type <see cref="NatureSpotlightEmail"/>.
	/// </summary>
	[RegisterContentTypeMapping(CONTENT_TYPE_NAME)]
	public partial class NatureSpotlightEmail : IEmailFieldsSource
	{
		/// <summary>
		/// Code name of the content type.
		/// </summary>
		public const string CONTENT_TYPE_NAME = "TrainingGuides.NatureSpotlightEmail";


		/// <summary>
		/// Represents system properties for an email item.
		/// </summary>
		[SystemField]
		public EmailFields SystemFields { get; set; }


		/// <summary>
		/// EmailSubject.
		/// </summary>
		public string EmailSubject { get; set; }


		/// <summary>
		/// EmailPreviewText.
		/// </summary>
		public string EmailPreviewText { get; set; }


		/// <summary>
		/// NatureSpotlightTopic.
		/// </summary>
		public string NatureSpotlightTopic { get; set; }


		/// <summary>
		/// NatureSpotlightText.
		/// </summary>
		public string NatureSpotlightText { get; set; }


		/// <summary>
		/// NatureSpotlightCountries.
		/// </summary>
		public IEnumerable<Guid> NatureSpotlightCountries { get; set; }


		/// <summary>
		/// NatureSpotlightRelatedArticles.
		/// </summary>
		public IEnumerable<ArticlePage> NatureSpotlightRelatedArticles { get; set; }


		/// <summary>
		/// NatureSpotlightImages.
		/// </summary>
		public IEnumerable<Asset> NatureSpotlightImages { get; set; }
	}
}