// Ignore Spelling: Util

namespace WillPittenger.Goodies.Tools.Exceptions;

/// <summary>
/// Describes a condition in which a value in an <see langword="enum"/> type was unknown and/or invalid.  The field is either corrupt or a value was added to the
/// type, but a <see langword="switch"/> statement wasn't updated to consider that new value.
/// </summary>
/// <typeparam name="EnumType">The type of the enum</typeparam>
public class UnknownOrInvalidEnumException<EnumType> : System.Exception
{
	#region Constructors & Destructors
		/// <summary>
		/// Constructs a new <see cref="UnknownOrInvalidEnumException{EnumType}"/> instance.
		/// </summary>
		/// <param name="objFoundThis">The value found that is either unknown or invalid</param>
		/// <param name="strActionDesc">What the code was doing</param>
		/// <param name="eInner">Any inner exception, if any.  Defaults to <see langword="null"/> in which case, there is no inner exception.</param>
		public UnknownOrInvalidEnumException(in EnumType objFoundThis, in string strActionDesc, System.Exception? eInner = null) : base(null, eInner)
		{
			System.Diagnostics.Debug.Assert(strActionDesc != null && strActionDesc.Length > 0);

			this.objFoundThis = objFoundThis;
			this.strActionDesc = strActionDesc;
		}
	#endregion

	// Members
	#region Members
		/// <summary>
		/// The object found
		/// </summary>
		public readonly EnumType objFoundThis;

		/// <summary>
		/// A description of what the code was doing
		/// </summary>
		public readonly string strActionDesc;
	#endregion

	// Properties
	#region Properties
		/// <summary>
		/// Returns a text message describing about the error
		/// </summary>
		public override string Message
			=> $"Unknown or invalid {typeof(EnumType)} enum value while {strActionDesc}: {objFoundThis}";
	#endregion
}