// Ignore Spelling: Dlp Vals uri Sel wget Xattr Hls Sys Arounds Vid Fmts Pwd Executables Evt Exe Evts Loc Locs Json Dest Hdrs Langs

namespace WillPittenger.Goodies.YtDlpWrapper;

using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Groups various options related to video formats.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#subtitle-options">Subtitle Options section of yt-dlp’s readme.md</a>.
	/// </summary>
	public sealed class SubsGroup
	{
		public SubsGroup()
		{
		}

		public SubsGroup(in SubsGroup copyThis)
		{
			WriteFile = new(copyThis.WriteFile);
			WriteAutoFile = new(copyThis.WriteAutoFile);
			FmtForFiles = new(copyThis.FmtForFiles);
			LangsToDownLoad = new(copyThis.LangsToDownLoad);
		}


		/// <summary>
		/// Lists the formats that yt-dlp supports.  Each value is a <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> instance.  Only those values are allowed.
		/// </summary>
		public sealed class SupportedFmts
		{
			/// <summary>
			/// This is the only way to construct a new <see cref="SupportedFmts"/> by design.
			/// </summary>
			/// <param name="strText">The exact text that yt-dlp expects for the desired format.  Use a raw string and not one from the RESX file.</param>
			private SupportedFmts(in string strText)
				=> this.strText = strText;


			/// <summary>
			/// The text to pass to yt-dlp
			/// </summary>
			public readonly string strText;



			/// <summary>
			/// Unless a config file specifies a value, this is the same as <see cref="doNotConvert"/>.  No conversion would happen.
			/// </summary>
			public static readonly SupportedFmts @default = new(string.Empty);

			/// <summary>
			/// Prevents any conversion from happening even if a config file specifies a conversion.
			/// </summary>
			public static readonly SupportedFmts doNotConvert = new(@"none");


			/// <summary>
			/// Converts all subtitle files to the ASS format.
			/// </summary>
			public static readonly SupportedFmts ass = new(@"ass");

			/// <summary>
			/// Converts all subtitle files to the LRC format.
			/// </summary>
			public static readonly SupportedFmts lrc = new(@"lrc");

			/// <summary>
			/// Converts all subtitle files to the SRT format.
			/// </summary>
			public static readonly SupportedFmts srt = new(@"srt");

			/// <summary>
			/// Converts all subtitle files to the VTT format.
			/// </summary>
			public static readonly SupportedFmts vtt = new(@"vtt");
		}


		/// <summary>
		/// If <see langword="true"/>, subtitle files will be written out.  If <see langword="false"/>, they won’t be.  If you leave this at the default value of <see langword="null"/>, yt-dlp will act as though <see cref="WriteFile"/> is <see langword="false"/> unless a config file specifies either --write-subs or --no-write-subs.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strSubsWriteFile), @"If true, subtitle files will be written out.  If false, they won't be.  If you leave this at the default value of null, yt-dlp will act as though WriteFile is false unless a config file specifies either --write-subs or --no-write-subs.", typeof(SubsGroup), @"--write-subs", @"--no-write-subs")]
		public ThreeWayOpt WriteFile
		{
			get;
		} = new(@"--write-subs", @"--no-write-subs", @"writesubtitles", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// If <see langword="true"/>, automatically generated subtitles will be written to disk.  If <see langword="false"/>, they won't be.  If you leave <see cref="WriteAutoFile"/> at the default value of <see langword="null"/>, yt-dlp will act as though <see cref="WriteAutoFile"/> is <see langword="false"/> unless a config file specifies either --write-auto-subs or --no-write-auto-subs.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strSubsWriteAutoFile), @"If true, automatically generated subtitles will be written to disk.  If false, they won't be.  If you leave WriteAutoFile at the default value of null, yt-dlp will act as though WriteAutoFile is false unless a config file specifies either --write-auto-subs or --no-write-auto-subs.", typeof(SubsGroup), @"--write-auto-subs", @"--no-write-auto-subs")]
		public ThreeWayOpt WriteAutoFile
		{
			get;
		} = new(@"--write-auto-subs", @"--no-write-auto-subs", @"writeautomaticsub", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// Subtitle format; accepts formats preference separated by ‘/’, e.g. <c>"srt"</c> or <c>"ass/srt/best"</c>
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strSubsFmtForFiles), @"Subtitle format; accepts formats preference separated by ‘/’, e.g. ""srt"" or ""ass/srt/best""", typeof(SubsGroup), @"--sub-format")]
		public OneOpt<string> FmtForFiles
		{
			get;
		} = new(string.Empty, @"--sub-format", @"subtitlesformat");

		/// <summary>
		/// Languages of the subtitles to download (can be regex) or “all” separated by commas, e.g. <c><see cref="LangsToDownLoad"/>="en.*,ja"</c> (where “en.*” is a regex pattern that matches “en” followed by 0 or more of any character).  You can prefix the language code with a ‘-’ to exclude it from the requested languages, e.g. <c><see cref="LangsToDownLoad"/>="all,-live_chat"</c>.  Use Get-LangsForVid or <see cref="Goodies.YtDlpWrapper.Vid.Subs"/> to get a list of available languages.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strSubsLangsToDownLoad), @"Languages of the subtitles to download (can be regex) or “all” separated by commas, e.g. LangsToDownLoad=""en.*,ja"" (where “en.*” is a regex pattern that matches “en” followed by 0 or more of any character).  You can prefix the language code with a ‘-’ to exclude it from the requested languages, e.g. LangsToDownLoad=""all,-live_chat"".  Use Get-LangsForVid or WillPittenger.Goodies.YtDlpWrapper.Vid.Subs to get a list of available languages.", typeof(SubsGroup), @"--sub-langs")]
		public OneOpt<System.Collections.Generic.IReadOnlyCollection<string>> LangsToDownLoad
		{
			get;
		} = new([], @"--sub-langs")
		{
			HowToCombineVals = OneOpt<System.Collections.Generic.IReadOnlyCollection<string>>.ValCombinationRules.slash,

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal.Count > 0
						? new DictionaryToOpt()
						{
							[@"subtitleslangs"] = opt.CurVal,
						}
						: [],
		};


		/// <summary>
		/// Lists all options in <see cref="SubsGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				WriteFile,
				WriteAutoFile,
				FmtForFiles,
				LangsToDownLoad,
			];
	}
}