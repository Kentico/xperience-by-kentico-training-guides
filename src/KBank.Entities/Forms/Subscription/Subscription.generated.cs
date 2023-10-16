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
using CMS;
using CMS.Base;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.OnlineForms;
using CMS.OnlineForms.Types;

[assembly: RegisterBizForm(SubscriptionItem.CLASS_NAME, typeof(SubscriptionItem))]

namespace CMS.OnlineForms.Types
{
	/// <summary>
	/// Represents a content item of type SubscriptionItem.
	/// </summary>
	public partial class SubscriptionItem : BizFormItem
	{
		#region "Constants and variables"

		/// <summary>
		/// The name of the data class.
		/// </summary>
		public const string CLASS_NAME = "BizForm.Subscription";


		/// <summary>
		/// The instance of the class that provides extended API for working with SubscriptionItem fields.
		/// </summary>
		private readonly SubscriptionItemFields mFields;

		#endregion


		#region "Properties"

		/// <summary>
		/// FirstName.
		/// </summary>
		[DatabaseField]
		public string FirstName
		{
			get => ValidationHelper.GetString(GetValue(nameof(FirstName)), @"");
			set => SetValue(nameof(FirstName), value);
		}


		/// <summary>
		/// Email.
		/// </summary>
		[DatabaseField]
		public string Email
		{
			get => ValidationHelper.GetString(GetValue(nameof(Email)), @"");
			set => SetValue(nameof(Email), value);
		}


		/// <summary>
		/// Gets an object that provides extended API for working with SubscriptionItem fields.
		/// </summary>
		[RegisterProperty]
		public SubscriptionItemFields Fields
		{
			get => mFields;
		}


		/// <summary>
		/// Provides extended API for working with SubscriptionItem fields.
		/// </summary>
		[RegisterAllProperties]
		public partial class SubscriptionItemFields : AbstractHierarchicalObject<SubscriptionItemFields>
		{
			/// <summary>
			/// The content item of type SubscriptionItem that is a target of the extended API.
			/// </summary>
			private readonly SubscriptionItem mInstance;


			/// <summary>
			/// Initializes a new instance of the <see cref="SubscriptionItemFields" /> class with the specified content item of type SubscriptionItem.
			/// </summary>
			/// <param name="instance">The content item of type SubscriptionItem that is a target of the extended API.</param>
			public SubscriptionItemFields(SubscriptionItem instance)
			{
				mInstance = instance;
			}


			/// <summary>
			/// FirstName.
			/// </summary>
			public string FirstName
			{
				get => mInstance.FirstName;
				set => mInstance.FirstName = value;
			}


			/// <summary>
			/// Email.
			/// </summary>
			public string Email
			{
				get => mInstance.Email;
				set => mInstance.Email = value;
			}
		}

		#endregion


		#region "Constructors"

		/// <summary>
		/// Initializes a new instance of the <see cref="SubscriptionItem" /> class.
		/// </summary>
		public SubscriptionItem() : base(CLASS_NAME)
		{
			mFields = new SubscriptionItemFields(this);
		}

		#endregion
	}
}