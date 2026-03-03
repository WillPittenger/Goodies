// Ignore Spelling: Dlp Vals uri Sel wget Xattr Hls Sys Arounds Vid Fmts Pwd Executables Evt Exe Evts Loc Locs Json Dest Hdrs Langs

namespace WillPittenger.Goodies.YtDlpWrapper;

using System.Linq;

using Tools.Ext;

using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{

	/// <summary>
	/// Groups various options related to calling the extractors.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#extractor-options">Extractor Options section of yt-dlp’s readme.md</a>.  Additional relevant information can be found at <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#extractor-arguments">the later Extractor Arguments section</a>.
	/// </summary>
	public sealed class ExtractorGroup
	{
		public ExtractorGroup()
		{
		}

		public ExtractorGroup(in ExtractorGroup copyThis)
		{
			Retries = new(copyThis.Retries);
			AllowDynamicMPD = new(copyThis.AllowDynamicMPD);
			SplitHlsDiscontinuities = new(copyThis.SplitHlsDiscontinuities);
			Args = new(copyThis.Args);
		}


		/// <summary>
		/// Represents a series of arguments to send to an executable via yt-dlp's --exec.
		/// </summary>
		/// <param name="ExtractorName">The filename of the executable to run.  If the executable isn’t in your system path, be sure to fully qualify it.</param>
		/// <param name="Args">The arguments to pass to the extractor.</param>
		public sealed class ArgsVal(string ExtractorName, System.Collections.Generic.IReadOnlyCollection<OneArgVal> Args) : IOneOptVal
		{
			/// <summary>
			/// Formats the arguments in the format expected by yt-dlp.
			/// </summary>
			public string? ArgsValAsText
				=> Args == null || Args.Count == 0
					? null
					: Args.Where(
							curArg
								=> curArg.IsValid
						).Select(
							curArg
								=> curArg.ValText
						).Join(';');

			/// <summary>
			/// Returns the entire value as a string.
			/// </summary>
			public string? ValText
				=> ExtractorName == string.Empty || Args.Count == 0
					? null
					: $"{ExtractorName}:{ArgsValAsText}";
		}

		/// <summary>
		/// Stores one parameter name and it's value set.
		/// </summary>
		/// <param name="ParamName">The name of the parameter.  If this is an empty string, nothing will be emitted.</param>
		/// <param name="Values">A list of values for that parameter.  If this is empty, nothing will be emitted.</param>
		public sealed record OneArgVal(string ParamName, System.Collections.Generic.IReadOnlyCollection<string> Values)
		{
			/// <summary>
			/// Tests if both the parameter name is non-empty and the value list has at least one element.
			/// </summary>
			public bool IsValid
				=> ParamName != string.Empty && Values.Count > 0;

			/// <summary>
			/// If <see cref="IsValid"/> is <see langword="true"/>, returns the entire argument as a single string.
			/// </summary>
			public string? ValText
				=> IsValid
					? $"{ParamName}={Values.Join(',')}"
					: null;
		}


		/// <summary>
		/// The number of retries for known extractor errors.  Use <see cref="ulong.MaxValue"/> for an infinite number of retries.  The default value of <see langword="null"/> causes yt-dlp to assume <c>3</c> unless a config file specifies --extractor-retries.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strExtractorRetries), @"The number of retries for known extractor errors.  Use ulong.MaxValue for an infinite number of retries.  The default value of null causes yt-dlp to assume 3 unless a config file specifies --extractor-retries.", typeof(ExtractorGroup), @"--extractor-retries")]
		public OneOpt<ulong?> Retries
		{
			get;
		} = new(null, @"--extractor-retries", @"extractor_retries");

		/// <summary>
		/// If <see langword="true"/>, yt-dlp process dynamic DASH manifests.  If <see langword="false"/>. yt-dlp won’t.  If <see cref="AllowDynamicMPD"/> is left at the default value of <see langword="null"/> and no config file specifies --allow-dynamic-mpd or --no-ignore-dynamic-mpd, yt-dlp will act as though <see cref="AllowDynamicMPD"/> is <see langword="true"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strExtractorAllowDynamicMPD), @"If true, yt-dlp process dynamic DASH manifests.  If false. yt-dlp won’t.  If AllowDynamicMPD is left at the default value of null and no config file specifies --allow-dynamic-mpd or --no-ignore-dynamic-mpd, yt-dlp will act as though AllowDynamicMPD is true.", typeof(ExtractorGroup), @"--allow-dynamic-mpd", @"--ignore-dynamic-mpd")]
		public ThreeWayOpt AllowDynamicMPD
		{
			get;
		} = new(@"--allow-dynamic-mpd", @"--ignore-dynamic-mpd", @"dynamic_mpd", ThreeWayOpt.WhichValsToSendToPythonChoices.falseOnly);

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will split HLS playlists to different formats at discontinuities such as ad breaks.  If <see langword="false"/>, it won’t do that and the same formats will be used throughout.  If you leave <see cref="SplitHlsDiscontinuities"/> set to the default value of <see langword="null"/> and no config file specifies --hls-split-discontinuity, yt-dlp will act as though <see cref="SplitHlsDiscontinuities"/> equals <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strExtractorSplitHlsDiscontinuities), @"If true, yt-dlp will split HLS playlists to different formats at discontinuities such as ad breaks.  If false, it won’t do that and the same formats will be used throughout.  If you leave SplitHlsDiscontinuities set to the default value of null and no config file specifies --hls-split-discontinuity, yt-dlp will act as though SplitHlsDiscontinuities equals false.", typeof(ExtractorGroup), @"--hls-split-discontinuity", @"--no-hls-split-discontinuity")]
		public ThreeWayOpt SplitHlsDiscontinuities
		{
			get;
		} = new(@"--hls-split-discontinuity", @"--no-hls-split-discontinuity", @"hls_split_discontinuity", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// Pass these arguments to the specified extractor.  See <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#extractor-arguments">EXTRACTOR ARGUMENTS section of yt-dlp’s readme.md</a> for details.  Use one element for each extractor.  Each element should be an <see cref="ArgsVal"/> containing one or more <see cref="OneArgVal"/> entries.  No parameters will be emitted for <see cref="ArgsVal"/> entry that have a blank extractor name or an args list that’s empty.  <see cref="OneArgVal"/> values will be ignored if they’re missing either an argument name or the value list is null.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strExtractorArgs), @"Pass these arguments to the specified extractor.  See https://github.com/yt-dlp/yt-dlp/blob/master/README.md#extractor-arguments for details.  Use one element for each extractor.  Each element should be an ArgsVal containing one or more OneArgVal entries.  No parameters will be emitted for ArgsVal entry that have a blank extractor name or an args list that’s empty.  OneArgVal values will be ignored if they’re missing either an argument name or the value list is null.", typeof(ExtractorGroup), @"--extractor-args")]
		public OneOpt<System.Collections.Generic.IReadOnlyCollection<OneArgVal>?> Args
		{
			get;
		} = new([], @"--extractor-args")
		{
			HowToCombineVals = OneOpt<System.Collections.Generic.IReadOnlyCollection<OneArgVal>?>.ValCombinationRules.paramReuse,

			HowToMakeKeyValPairStrings = OneOpt<System.Collections.Generic.IReadOnlyCollection<OneArgVal>?>.KeyValCombinationRules.equalSign,

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal.IsEmpty()
						? []
						: new DictionaryToOpt()
						{
							[@"extractor_args"] = new DictionaryToOpt(
										from curArg in opt.CurVal
										select new System.Collections.Generic.KeyValuePair<string, object?>(curArg.ParamName, curArg.Values)
									),
						},
		};


		/// <summary>
		/// Lists all options in <see cref="ExtractorGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to generate its own list of options.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				Retries,
				AllowDynamicMPD,
				SplitHlsDiscontinuities,
				Args,
			];
	}
}