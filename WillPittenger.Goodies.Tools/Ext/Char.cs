namespace WillPittenger.Goodies.Tools.Ext;

public static class Char
{
	#region Tests
		public static bool IsUpper(in this char chTest)
			=> char.IsUpper(chTest);

		public static bool IsLower(in this char chTest)
			=> char.IsLower(chTest);

		public static bool IsLetter(in this char chTest)
			=> char.IsLetter(chTest);

		public static bool IsDigit(in this char chTest)
			=> char.IsDigit(chTest);

		public static bool IsLetterOrDigit(in this char chTest)
			=> char.IsLetterOrDigit(chTest);

		public static bool IsControl(in this char chTest)
			=> char.IsControl(chTest);

		public static bool IsHighSurrogate(in this char chTest)
			=> char.IsHighSurrogate(chTest);

		public static bool IsLowSurrogate(in this char chTest)
			=> char.IsLowSurrogate(chTest);

		public static bool IsNumber(in this char chTest)
			=> char.IsNumber(chTest);

		public static bool IsPunctuation(in this char chTest)
			=> char.IsPunctuation(chTest);

		public static bool IsSeparator(in this char chTest)
			=> char.IsSeparator(chTest);

		public static bool IsSurrogate(in this char chTest)
			=> char.IsSurrogate(chTest);

		public static bool IsSymbol(in this char chTest)
			=> char.IsSymbol(chTest);

		public static bool IsWhiteSpace(in this char chTest)
			=> char.IsWhiteSpace(chTest);
	#endregion

	#region Conversions
		public static char ToLower(in this char chConvertThis)
			=> char.ToLower(chConvertThis);

		public static char ToLower(in this char chConvertThis, in System.Globalization.CultureInfo culture)
			=> char.ToLower(chConvertThis, culture);

		public static char ToLowerInvariant(in this char chConvertThis)
			=> char.ToLowerInvariant(chConvertThis);

		public static char ToUpper(in this char chConvertThis)
			=> char.ToUpper(chConvertThis);

		public static char ToUpper(in this char chConvertThis, in System.Globalization.CultureInfo culture)
			=> char.ToUpper(chConvertThis, culture);

		public static char ToUpperInvariant(in this char chConvertThis) 
			=> char.ToUpperInvariant(chConvertThis);
	#endregion
}