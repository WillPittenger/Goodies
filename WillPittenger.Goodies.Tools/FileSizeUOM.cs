namespace WillPittenger.Goodies.Tools;

public static class FileSizeUOM
{
	[System.ComponentModel.ImmutableObject(true)]
	public sealed class Sizes
	{
		private Sizes(in long lRatioToBytes, in string strPluralName, in string strSingularName, in string strAbbrev)
		{
			this.lRatioToBytes = lRatioToBytes;
			this.strPluralName = strPluralName;
			this.strSingularName = strSingularName;
			this.strAbbrev = strAbbrev;
		}

		public readonly long lRatioToBytes;
		public readonly string strPluralName;
		public readonly string strSingularName;
		public readonly string strAbbrev;

		public static Sizes @byte = new(1, Rsrcs.strUomBytes, Rsrcs.strUomByte, Rsrcs.strUomBytesAbbrev);

		public static Sizes kilo = new(1000, Rsrcs.strUomKiloBytes, Rsrcs.strUomKiloByte, Rsrcs.strUomKiloBytesSiAbbrev);
		public static Sizes mega = new(1000000, Rsrcs.strUomMegaBytes, Rsrcs.strUomMegaByte, Rsrcs.strUomMegaBytesSiAbbrev);
		public static Sizes giga = new(1000000000, Rsrcs.strUomGigaBytes, Rsrcs.strUomGigaByte, Rsrcs.strUomGigaBytesSiAbbrev);
		public static Sizes tera = new(1000000000000, Rsrcs.strUomTeraBytes, Rsrcs.strUomTeraByte, Rsrcs.strUomTeraBytesSiAbbrev);

		public static Sizes oldKilo = new(0x100, Rsrcs.strUomKiloBytes, Rsrcs.strUomKiloByte, Rsrcs.strUomKiloBytesAbbrev);
		public static Sizes oldMega = new(0x10000, Rsrcs.strUomMegaBytes, Rsrcs.strUomMegaByte, Rsrcs.strUomMegaBytesAbbrev);
		public static Sizes oldGiga = new(0x1000000, Rsrcs.strUomGigaBytes, Rsrcs.strUomGigaByte, Rsrcs.strUomGigaBytesAbbrev);
		public static Sizes oldTera = new(0x100000000, Rsrcs.strUomTeraBytes, Rsrcs.strUomTeraBytes, Rsrcs.strUomTeraBytesAbbrev);
	}

	public static long Convert(in long lConvertThis, in Sizes from, in Sizes to)
		=> from == to ? lConvertThis : (long)(lConvertThis * (decimal)from.lRatioToBytes / to.lRatioToBytes);

	public static double Convert(in double dblConvertThis, in Sizes from, in Sizes to)
		=> from == to ? dblConvertThis : (double)(dblConvertThis * from.lRatioToBytes / to.lRatioToBytes);

	public static Sizes SuggestUOM(in long lAmt, in bool bUseSI)
		=> bUseSI
			? lAmt < Sizes.kilo.lRatioToBytes
				? Sizes.kilo
				: lAmt > Sizes.mega.lRatioToBytes
					? Sizes.mega
					: lAmt > Sizes.giga.lRatioToBytes
						? Sizes.giga
						: Sizes.tera
			: lAmt < Sizes.oldKilo.lRatioToBytes
				? Sizes.oldKilo
				: lAmt < Sizes.oldMega.lRatioToBytes
					? Sizes.oldMega
					: lAmt < Sizes.oldGiga.lRatioToBytes
						? Sizes.oldGiga
						: Sizes.oldTera;

	public static Sizes SuggestUOM(in double dblAmt, in bool bUseSI)
		=> bUseSI
			? dblAmt < Sizes.kilo.lRatioToBytes
				? Sizes.kilo
				: dblAmt > Sizes.mega.lRatioToBytes
					? Sizes.mega
					: dblAmt > Sizes.giga.lRatioToBytes
						? Sizes.giga
						: Sizes.tera
			: dblAmt < Sizes.oldKilo.lRatioToBytes
				? Sizes.oldKilo
				: dblAmt < Sizes.oldMega.lRatioToBytes
					? Sizes.oldMega
					: dblAmt < Sizes.oldGiga.lRatioToBytes
						? Sizes.oldGiga
						: Sizes.oldTera;

	public static Sizes SuggestUOM(in long lAmt, in bool bUseSI, out Sizes result)
		=> result = SuggestUOM(lAmt, bUseSI);

	public static Sizes SuggestUOM(in double dblAmt, in bool bUseSI, out Sizes result)
		=> result = SuggestUOM(dblAmt, bUseSI);
}