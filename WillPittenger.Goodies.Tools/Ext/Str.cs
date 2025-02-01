namespace WillPittenger.Goodies.Tools.Ext;

/// <summary>
/// Collection of extension methods that operate on strings.
/// </summary>
public static class Str
{
	/// <summary>
	/// Formats the string specified by <paramref name="strFmtThis"/> using the values in <paramref name="args"/>.  Unlike <see cref="o:string.Format"/>, the
	/// format string is the <see langword="this"/> value.  While <see cref="Fmt(string, object[])"/> has been partially made obsolete by interpolated strings,
	/// there are still some use cases.
	/// </summary>
	/// <param name="strFmtThis">The string use as the format</param>
	/// <param name="args">Any list of values you want to reference in the format string</param>
	/// <returns>The formatted string</returns>
	/// <example>
	/// Typical usage
	/// <code>
	/// "this is {0} string".Fmt('a');
	/// </code>
	/// </example>
	public static string Fmt(this string strFmtThis, params object[] args)
		=> string.Format(strFmtThis, args);

	/// <summary>
	/// Tests if the string is empty or null.
	/// </summary>
	/// <param name="strTestThis">The value to test</param>
	/// <returns><see langword="true"/> if the string is empty and <see langword="false"/> otherwise</returns>
	/// <seealso cref="o:Collections.IsEmpty"/>
	public static bool IsEmpty(this string? strTestThis)
		=> strTestThis == null || strTestThis.Length == 0;

	/// <summary>
	/// Compares two strings and returns the string with the smaller value.
	/// </summary>
	/// <param name="strLeft">The first candidate</param>
	/// <param name="strRight">The second candidate</param>
	/// <returns>The string with the smaller value</returns>
	/// <seealso cref="Max(string, string)"/>
	public static string Min(string strLeft, string strRight)
		=> strLeft.Equals(strRight)
			? strRight
			: string.Compare(strLeft, strRight, System.StringComparison.Ordinal) < 0
				? strLeft
				: strRight;

	/// <summary>
	/// Compares two strings and returns the string with the bigger value.
	/// </summary>
	/// <param name="strLeft">The first candidate</param>
	/// <param name="strRight">The second candidate</param>
	/// <returns>The string with the larger value</returns>
	/// <seealso cref="Min(string, string)"/>
	public static string Max(string strLeft, string strRight)
		=> strLeft.Equals(strRight)
			? strRight
			: string.Compare(strLeft, strRight, System.StringComparison.Ordinal) > 0
				? strLeft
				: strRight;
}