// Ignore Spelling: Util

namespace WillPittenger.Goodies.Tools.Exceptions;

using Ext;

/// <summary>
/// Throw when a key isn't unique.
/// </summary>
public class NotUniqueException : System.Exception
{
	#region Constructors & Deconstructors
		/// <summary>
		/// Constructs a new <see cref="NotUniqueException"/>.
		/// </summary>
		/// <param name="strUniqueFieldName">The field name that must be unique</param>
		/// <param name="strValueInUse">The value that's already taken</param>
		/// <param name="strDescriptiveFieldUse">A description of the field specified by <paramref name="strUniqueFieldName"/></param>
		/// <exception cref="System.ArgumentNullException">Either <paramref name="strDescriptiveFieldUse"/> or <paramref name="strUniqueFieldName"/> were <see
		/// langword="null"</exception>
		public NotUniqueException(in string strUniqueFieldName, in string strValueInUse, in string? strDescriptiveFieldUse = null)
		{
			System.Diagnostics.Trace.Assert(!strUniqueFieldName.IsEmpty());
			System.Diagnostics.Trace.Assert(!strValueInUse.IsEmpty());

			this.strUniqueFieldName = strUniqueFieldName ?? throw new System.ArgumentNullException(nameof(strUniqueFieldName), @"While constructing an exception about"
				+ @"something not being unique");
			this.strValueInUse = strValueInUse ?? throw new System.ArgumentNullException(nameof(strValueInUse), @"While constructing an exception about something not"
				+ @" being unique");
			this.strDescriptiveFieldUse = strDescriptiveFieldUse;
		}
	#endregion

	#region Members
		/// <summary>
		/// The name of the field that must be unique
		/// </summary>
		public readonly string strUniqueFieldName;

		/// <summary>
		/// The value that's already taken
		/// </summary>
		public readonly string strValueInUse;

		/// <summary>
		/// A description of the field specified by <see cref="strUniqueFieldName"/>
		/// </summary>
		public readonly string? strDescriptiveFieldUse;
	#endregion

	#region Properties
		/// <summary>
		/// Returns a description about the error.
		/// </summary>
		public override string Message
			=> $"Field {strUniqueFieldName} is required to be unique.  But it isn't.  The value {strValueInUse} is used already.  {(strDescriptiveFieldUse != null ?
				"  " + strDescriptiveFieldUse : "")}";
	#endregion
}