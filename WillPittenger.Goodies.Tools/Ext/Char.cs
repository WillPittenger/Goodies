namespace WillPittenger.Goodies.Tools.Ext;

/// <summary>
/// Extension class for <see cref="char"/> that mostly allows you to treat some of the static methods as instance methods.
/// </summary>
/// <remarks>
/// <para>All these methods are extension methods.  Use <c><see langword="using"/> Pittenger.Goodies.Tools.Ext</c> to access them.</para>
/// </remarks>
public static class Char
{
	#region Tests
		/// <summary>
		/// Like <see cref="char.IsUpper(char)"/>, checks if the value in <paramref name="chTest"/> is upper case.
		/// </summary>
		/// <param name="chTest"></param>
		/// <returns><see langword="true"/> if the character is upper case and <see langword="false"/> otherwise.</returns>
		public static bool IsUpper(in this char chTest)
			=> char.IsUpper(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsLower(char)"/>.
		/// </summary>
		/// <param name="chTest">The character to test</param>
		/// <returns>><see langword="true"/> if the character is lower case and <see langword="false"/> otherwise.</returns>
		public static bool IsLower(in this char chTest)
			=> char.IsLower(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsLetter(char)"/>.
		/// </summary>
		/// <param name="chTest">The character to test</param>
		/// <returns>><see langword="true"/> if the character is a letter and <see langword="false"/> otherwise.</returns>
		public static bool IsLetter(in this char chTest)
			=> char.IsLetter(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsDigit(char)"/>.
		/// </summary>
		/// <param name="chTest">The value to test</param>
		/// <returns>><see langword="true"/> if the character is a digit and <see langword="false"/> otherwise.</returns>
		public static bool IsDigit(in this char chTest)
			=> char.IsDigit(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsLetterOrDigit(char)"/>.
		/// </summary>
		/// <param name="chTest">The value to test</param>
		/// <returns>><see langword="true"/> if the character is a letter <b>or</b> a digit and <see langword="false"/> otherwise.</returns>
		public static bool IsLetterOrDigit(in this char chTest)
			=> char.IsLetterOrDigit(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsControl(char)"/>.
		/// </summary>
		/// <param name="chTest">The value to test</param>
		/// <returns>><see langword="true"/> if the character is a control character and <see langword="false"/> otherwise.</returns>
		public static bool IsControl(in this char chTest)
			=> char.IsControl(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsHighSurrogate(char)"/>.
		/// </summary>
		/// <param name="chTest">The character to test</param>
		/// <returns>><see langword="true"/> if the character is a high surrogate and <see langword="false"/> otherwise.</returns>
		public static bool IsHighSurrogate(in this char chTest)
			=> char.IsHighSurrogate(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsLowSurrogate(char)"/>.
		/// </summary>
		/// <param name="chTest">The value to test</param>
		/// <returns>><see langword="true"/> if the character is a low surrogate and <see langword="false"/> otherwise.</returns>
		public static bool IsLowSurrogate(in this char chTest)
			=> char.IsLowSurrogate(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsNumber(char)"/>.
		/// </summary>
		/// <param name="chTest">The value to test</param>
		/// <returns>><see langword="true"/> if the character is a number and <see langword="false"/> otherwise.</returns>
		public static bool IsNumber(in this char chTest)
			=> char.IsNumber(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsPunctuation(char)"/>.
		/// </summary>
		/// <param name="chTest">The value to test</param>
		/// <returns>><see langword="true"/> if the character is punctuation and <see langword="false"/> otherwise.</returns>
		public static bool IsPunctuation(in this char chTest)
			=> char.IsPunctuation(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsSeparator(char)"/>.
		/// </summary>
		/// <param name="chTest">The value to test</param>
		/// <returns>><see langword="true"/> if the character is a separator and <see langword="false"/> otherwise.</returns>
		public static bool IsSeparator(in this char chTest)
			=> char.IsSeparator(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsSurrogate(char)"/>.
		/// </summary>
		/// <param name="chTest">The value to test</param>
		/// <returns>><see langword="true"/> if the character is a surrogate and <see langword="false"/> otherwise.</returns>
		public static bool IsSurrogate(in this char chTest)
			=> char.IsSurrogate(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsSymbol(char)"/>.
		/// </summary>
		/// <param name="chTest">The value to test</param>
		/// <returns>><see langword="true"/> if the character is a symbol and <see langword="false"/> otherwise.</returns>
		public static bool IsSymbol(in this char chTest)
			=> char.IsSymbol(chTest);

		/// <summary>
		/// More convenient way to call <see cref="char.IsWhiteSpace(char)"/>.
		/// </summary>
		/// <param name="chTest">The value to test</param>
		/// <returns>><see langword="true"/> if the character is whitespace and <see langword="false"/> otherwise.</returns>
		public static bool IsWhiteSpace(in this char chTest)
			=> char.IsWhiteSpace(chTest);
	#endregion

	#region Conversions
		/// <summary>
		/// Like <see cref="char.ToLower(char)"/>, but you call it like an instance method rather than a static method.  It also converts a string to lower case, but
		/// ignoring the culture.  If you need to include the culture, call <see cref="ToLower(in char, in System.Globalization.CultureInfo)"/> instead.
		/// </summary>
		/// <param name="chConvertThis">The value to convert</param>
		/// <returns>The value in <paramref name="chConvertThis"/> after conversion.</returns>
		/// <seealso cref="ToLower(in char, in System.Globalization.CultureInfo)"/>
		/// <seealso cref="ToLowerInvariant(in char)"/>
		public static char ToLower(in this char chConvertThis)
			=> char.ToLower(chConvertThis);

		/// <summary>
		/// Like <see cref="char.ToLower(char, System.Globalization.CultureInfo)"/>, but you call it like an instance method instead of a static method.  This
		/// overload takes into account the culture specified by <paramref name="culture"/>.
		/// </summary>
		/// <param name="chConvertThis">The value to convert</param>
		/// <param name="culture">The culture to use during the conversion</param>
		/// <returns>The value in <paramref name="chConvertThis"/>, but converted to lower case according to the culture specified by <paramref name="culture"/>
		/// </returns>
		/// <seealso cref="ToLowerInvariant(in char)"/>
		/// <seealso cref="ToLower(in char)"/>
		public static char ToLower(in this char chConvertThis, in System.Globalization.CultureInfo culture)
			=> char.ToLower(chConvertThis, culture);

		/// <summary>
		/// Like <see cref="char.ToLowerInvariant(char)"/>, but you call it like an instance method rather than a static method.  It converts the value in <paramref
		/// name="chConvertThis"/> to lower case like <see cref="ToLower(in char)"/>, but in an invariant manner.
		/// </summary>
		/// <param name="chConvertThis">The value to convert</param>
		/// <returns>The character in <paramref name="chConvertThis"/> after conversion</returns>
		/// <seealso cref="ToLower(in char)"/>
		/// <seealso cref="ToLower(in char, in System.Globalization.CultureInfo)"/>
		public static char ToLowerInvariant(in this char chConvertThis)
			=> char.ToLowerInvariant(chConvertThis);

		/// <summary>
		/// Like <see cref="char.ToUpper(char)"/>, but you call it like an instance method instead of a static method.  It converts the value in <paramref
		/// name="chConvertThis"/> to upper case.  This overload doesn't take the culture into account.  If you need that, call <see cref="ToLower(in char, in
		/// System.Globalization.CultureInfo)"/> instead.
		/// </summary>
		/// <param name="chConvertThis">The value to convert</param>
		/// <returns>The value in <paramref name="chConvertThis"/> after conversion</returns>
		/// <seealso cref="ToUpperInvariant(in char)"/>
		/// <seealso cref="ToUpper(in char, in System.Globalization.CultureInfo)"/>
		public static char ToUpper(in this char chConvertThis)
			=> char.ToUpper(chConvertThis);

		/// <summary>
		/// Like <see cref="char.ToUpper(char, System.Globalization.CultureInfo)"/>, but you call it like an instance method instead of a static method.  This
		/// overload takes into account the culture specified by <paramref name="culture"/>.  If you don't need it, use <see cref="ToUpperInvariant(in char)"/> or
		/// <see cref="ToUpper(in char)"/> instead.
		/// </summary>
		/// <param name="chConvertThis">The value to convert</param>
		/// <param name="culture">The culture to use</param>
		/// <returns>The value from <paramref name="chConvertThis"/> after conversion</returns>
		/// <seealso cref="ToUpper(in char)"/>
		/// <seealso cref="ToUpperInvariant(in char)"/>
		public static char ToUpper(in this char chConvertThis, in System.Globalization.CultureInfo culture)
			=> char.ToUpper(chConvertThis, culture);

		/// <summary>
		/// Like <see cref="char.ToLowerInvariant(char)"/>, but you call it like an instance method instead of a static method.  This method converts the value in
		/// <paramref name="chConvertThis"/> in an invariant manner to upper case.
		/// </summary>
		/// <param name="chConvertThis">The value to convert</param>
		/// <returns>The value from <paramref name="chConvertThis"/> after conversion</returns>
		/// <see cref="ToUpper(in char, in System.Globalization.CultureInfo)"/>
		/// <see cref="ToUpper(in char)"/>
		public static char ToUpperInvariant(in this char chConvertThis) 
			=> char.ToUpperInvariant(chConvertThis);
	#endregion
}