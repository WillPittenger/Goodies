// Ignore Spelling: Util

namespace WillPittenger.Goodies.Tools.Exceptions;

using Ext;

public class NotUniqueException : System.Exception
{
	#region Constructors & Deconstructors
		public NotUniqueException(in string strUniqueFieldName, in string strValueInUse, in string? strDescriptiveFieldUse = null)
		{
			System.Diagnostics.Trace.Assert(!strUniqueFieldName.IsEmpty());
			System.Diagnostics.Trace.Assert(!strValueInUse.IsEmpty());

			this.strUniqueFieldName = strUniqueFieldName ?? throw new System
				.ArgumentNullException(nameof(strUniqueFieldName), @"While constructing an exception about something not " +
					@"being unique");
			this.strValueInUse = strValueInUse ?? throw new System.ArgumentNullException(nameof(strValueInUse), @"While " +
				@"constructing an exception about something not being unique");
			this.strDescriptiveFieldUse = strDescriptiveFieldUse;
		}
	#endregion

	#region Members
		public readonly string strUniqueFieldName;
		public readonly string strValueInUse;
		public readonly string? strDescriptiveFieldUse;
	#endregion

	#region Properties
		public override string Message
			=> $"Field {strUniqueFieldName} is required to be unique.  But it isn't.  The value {strValueInUse} is used already.  {(strDescriptiveFieldUse != null ? "  " + strDescriptiveFieldUse : "")}";
	#endregion
}