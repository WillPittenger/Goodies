// Ignore Spelling: Dlp Vals uri Sel wget Xattr Hls Sys Arounds Vid Fmts Pwd Executables Evt Exe Evts Loc Locs Json Dest Hdrs Langs

namespace WillPittenger.Goodies.YtDlpWrapper;

using System.Linq;

using Tools.Ext;

using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Groups various options related to video formats.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#video-format-options">Video Format Options section of yt-dlp’s readme.md</a>.
	/// </summary>
	public sealed class VidFmtGroup
	{
		public VidFmtGroup()
		{
		}

		public VidFmtGroup(in VidFmtGroup copyThis)
		{
			Sorting = new(copyThis.Sorting);
			ForceSorting = new(copyThis.ForceSorting);
			VidMultiStreamsAllowed = new(copyThis.VidMultiStreamsAllowed);
			AudioMultiStreamsAllowed = new(copyThis.AudioMultiStreamsAllowed);
			PreferFree = new(copyThis.PreferFree);
			Check = new(copyThis.Check);
			MergeOutput = MergeOutput;
		}


		/// <summary>
		/// Lists choices for if yt-dlp should check for what formats are available.
		/// </summary>
		public enum FmtCheckChoices
		{
			/// <summary>
			/// yt-dlp should use what’s specified in a config file or its default if no config file specifies one.
			/// </summary>
			@default,

			/// <summary>
			/// Check the format to be downloaded, but only that one.
			/// </summary>
			yes,

			/// <summary>
			/// Don’t perform any checks
			/// </summary>
			none,

			/// <summary>
			/// Check all formats even if they won’t be downloaded
			/// </summary>
			all,
		}

		/// <summary>
		/// Lists all formats that yt-dlp can merge outputs to.  Only these members are allowed.  Each value is a <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> member.
		/// </summary>
		[System.ComponentModel.ImmutableObject(true)]
		public sealed class SupportedMergeOutputFmts : IOneOptVal
		{
			/// <summary>
			/// Constructs a new instance
			/// </summary>
			/// <param name="strName">The exact string that yt-dlp expects for this format.  Use a raw string.  Don’t translate these names or put them in the RESX.</param>
			private SupportedMergeOutputFmts(in string strName)
				=> this.strName = strName;


			/// <summary>
			/// The name in question.
			/// </summary>
			public readonly string strName;


			/// <summary>
			/// yt-dlp should merge into the AVI format.
			/// </summary>
			public static readonly SupportedMergeOutputFmts avi = new(@"avi");

			/// <summary>
			/// yt-dlp should merge into the FLV format.
			/// </summary>
			public static readonly SupportedMergeOutputFmts flv = new(@"flv");

			/// <summary>
			/// yt-dlp should merge into the MKV format.
			/// </summary>
			public static readonly SupportedMergeOutputFmts mkv = new(@"mkv");

			/// <summary>
			/// yt-dlp should merge into the MOV format.
			/// </summary>
			public static readonly SupportedMergeOutputFmts mov = new(@"mov");

			/// <summary>
			/// yt-dlp should merge into the MP4 format.
			/// </summary>
			public static readonly SupportedMergeOutputFmts mp4 = new(@"mp4");

			/// <summary>
			/// yt-dlp should merge into the WEBM format.
			/// </summary>
			public static readonly SupportedMergeOutputFmts webm = new(@"webm");


			/// <inheritdoc/>
			public string? ValText
				=> strName;
		}


		/// <summary>
		/// Sort the formats by the fields given, see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#sorting-formats">Sorting Formats</a>.  This utility can’t help you set up the string needed due to the complexity.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidFmtSorting), @"Sort the formats by the fields given, see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#sorting-formats for more details.  This utility can't help you set up the string needed due to the complexity.", typeof(VidFmtGroup), @"--format-sort")]
		public OneOpt<System.Collections.Generic.IReadOnlyCollection<string>> Sorting
		{
			get;
		} = new([], @"--format-sort")
		{
			HowToCombineVals = OneOpt<System.Collections.Generic.IReadOnlyCollection<string>>.ValCombinationRules.paramReuse,

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal.Count == 0
						? []
						: new DictionaryToOpt()
						{
							[@"format_sort"] = opt.CurVal,
						},
		};

		/// <summary>
		/// If <see langword="true"/>, user-specified fields have precedence over all other fields.  If <see langword="false"/>, some fields will have precedence over those specified by the user.  For what user-specified fields were specified, see <see cref="Sorting"/>/--format-sort.  If you leave this at the default value of  <see langword="null"/>, a config file may set --format-sort-force or --no-format-sort-force.  By default, if <see cref="ForceSorting"/> is <see langword="null"/> and no config file sets a value, yt-dlp will act as though <see cref="ForceSorting"/> is <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidFmtForceSorting), @"If true, user-specified fields have precedence over all other fields.  If false, some fields will have precedence over those specified by the user.  For what user-specified fields were specified, see fmtSorting/--format-sort.  If you leave this at the default value of null, a config file may set --format-sort-force or --no-format-sort-force.  By default, if forceFmtSorting is null and no config file sets a value, yt-dlp will act as though forceFmtSorting is false.", typeof(VidFmtGroup), @"--format-sort-force",
			@"--no-format-sort-force")]
		public ThreeWayOpt ForceSorting
		{
			get;
		} = new(@"--format-sort-force", @"--no-format-sort-force", @"format_sort_force", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// If <see langword="true"/>, multiple video streams can be merged into one file.  If <see langword="false"/>, that won’t be allowed.  If you use the default value of <see langword="null"/>, yt-dlp will act as though <see cref="VidMultiStreamsAllowed"/> is <see langword="false"/> unless a config file sets either --video-multistreams or --no-video-multistreams.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidFmtAllowVidMultiStreams), @"If true, multiple video streams can be merged into one file.  If false, that won't be allowed.  If you use the default value of null, yt-dlp will act as though VidMultiStreamsAlloweed is false unless a config file sets either --video-multistreams or --no-video-multistreams.", typeof(VidFmtGroup), @"--video-multistreams", @"--no-video-multistreams")]
		public ThreeWayOpt VidMultiStreamsAllowed
		{
			get;
		} = new(@"--video-multistreams", @"--no-video-multistreams", @"allow_multiple_video_streams", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// If <see langword="true"/>, multiple audio streams can be merged into one file.  If <see langword="false"/>, that won’t be allowed.  If you use the default value of <see langword="null"/>, yt-dlp will act as though <see cref="AudioMultiStreamsAllowed"/> is <see langword="false"/> unless a config file sets either --audio-multistreams or --no-audio-multistreams.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidFmtAllowAudioMultiStreams), @"If true, multiple audio streams can be merged into one file.  If false, that won't be allowed.  If you use the default value of null, yt-dlp will act as though AudioMultiStreamsAlloweed is false unless a config file sets either --audio-multistreams or --no-audio-multistreams.", typeof(VidFmtGroup), @"--audio-multistreams", @"--no-audio-multistreams")]
		public ThreeWayOpt AudioMultiStreamsAllowed
		{
			get;
		} = new(@"--audio-multistreams", @"--no-audio-multistreams", @"allow_multiple_audio_streams", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will prefer formats with free containers over non-free containers with the same quality.  Use with <c><see cref="Sorting"/>="ext"</c> to prefer free formats regardless of quality.  If <see langword="false"/>, yt-dlp won’t give any special preferences to free formats.  If you use the default value of <see langword="null"/> and no config file specifies either --prefer-free-formats or --no-prefer-free-formats, yt-dlp will act as though PreferFree is <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidFmtPreferFree), @"If true, yt-dlp will prefer formats with free containers over non-free containers with the same quality.  Use with fmtSorting=""ext"" to prefer free formats regardless of quality.  If false, yt-dlp won't give any special preferences to free formats.  If you use the default value of null and no config file specifies either --prefer-free-formats or --no-prefer-free-formats, yt-dlp will act as though PreferFree is false.", typeof(VidFmtGroup), @"--prefer-free-formats", @"--no-prefer-free-formats")]
		public ThreeWayOpt PreferFree
		{
			get;
		} = new(@"--prefer-free-formats", @"--no-prefer-free-formats", @"prefer_free_formats", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// Controls if yt-dlp should check if a format can be actually downloaded.  If this is <see cref="FmtCheckChoices.@default"/> and no config file sets --check-formats, --check-all-formats, or --no-check-formats, yt-dlp will act as though Check is set to <see cref="FmtCheckChoices.none"/>.  Use <see cref="FmtCheckChoices.yes"/> to check selected files that they can be downloaded.  Use <see cref="FmtCheckChoices.all"/> to check all files that they can be downloaded.  Use <see cref="FmtCheckChoices.none"/> to never check.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidFmtCheck), @"Controls if yt-dlp should check if a format can be actually downloaded.  If this is FmtCheckChoices.@default and no config file sets --check-formats, --check-all-formats, or --no-check-formats, yt-dlp will act as though Check is set to FmtCheckChoices.never.  Use FmtCheckChoices.yes to check selected files that they can be downloaded.  Use FmtCheckChoices.all to check all files that they can be downloaded.  Use FmtCheckChoices.none to never check.", typeof(VidFmtGroup), @"--check-formats", @"--check-all-formats",
			@"--no-check-formats")]
		public OneOpt<FmtCheckChoices> Check
		{
			get;
		} = new(FmtCheckChoices.@default, string.Empty)
		{
			CustomParamNameLookUp =
				curVal
					=> curVal switch
					{
						FmtCheckChoices.@default
							=> string.Empty,

						FmtCheckChoices.yes
							=> @"--check-formats",

						FmtCheckChoices.all
							=> @"--check-all-formats",

						FmtCheckChoices.none
							=> @"--no-check-formats",

						_
							=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<FmtCheckChoices>(curVal, @"While selecting a parameter based on a format check choice"),
					},

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal switch
					{
						FmtCheckChoices.@default
							=> [],

						FmtCheckChoices.yes
							=> new DictionaryToOpt()
							{
								[@"check_formats"] = @"selected",
							},

						FmtCheckChoices.all
							=> new DictionaryToOpt()
							{
								[@"check_formats"] = true,
							},

						FmtCheckChoices.none
							=> new DictionaryToOpt()
							{
								[@"check_formats"] = false,
							},

						_
							=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<FmtCheckChoices>(opt.CurVal, @"While selecting a parameter based on a format check choice"),
					},
		};

		/// <summary>
		/// Containers that may be used when merging formats, separated by ‘/’, e.g. <c><see cref="MergeOutput"/>="mp4/mkv"</c>.  Ignored if no merge is required.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidFmtMergeOutput), @"Containers that may be used when merging formats, separated by ‘/’, e.g. MergeOutput=""mp4/mkv"".  Ignored if no merge is required.", typeof(VidFmtGroup), @"--merge-output-format")]
		public OneOpt<System.Collections.Generic.IReadOnlyCollection<SupportedMergeOutputFmts>> MergeOutput
		{
			get;
		} = new([], @"--merge-output-format")
		{
			HowToCombineVals = OneOpt<System.Collections.Generic.IReadOnlyCollection<SupportedMergeOutputFmts>>.ValCombinationRules.slash,

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal.Count > 0
						? new DictionaryToOpt()
						{
							[@"merge_output_format"] = opt.CurVal.Join(','),
						}
						: [],
		};


		/// <summary>
		/// Lists all options in <see cref="VidFmtGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				Sorting,
				ForceSorting,
				VidMultiStreamsAllowed,
				AudioMultiStreamsAllowed,
				PreferFree,
				Check,
				MergeOutput,
			];
	}
}