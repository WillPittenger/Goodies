namespace WillPittenger.Goodies.YtDlpWrapper;

using System.Linq;

using DictionaryToOptLists = System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<object>?>;
using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Stores all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#general-options">General options section of yt-dlp’s readme.md</a>.
	/// </summary>
	[System.ComponentModel.ImmutableObject(true)]
	public sealed class GeneralGroup
	{
		public GeneralGroup()
		{
		}

		public GeneralGroup(in GeneralGroup copyThis)
		{
			IgnoreErrors = new(copyThis.IgnoreErrors);
			AbortOnError = new(copyThis.AbortOnError);
			UseExtractors = new(copyThis.UseExtractors);
			DefSearchPrefix = new(copyThis.DefSearchPrefix);
			IgnoreConfigFiles = new(copyThis.IgnoreConfigFiles);
			NoConfigLoc = new(copyThis.NoConfigLoc);
			ConfigLoc = new(copyThis.ConfigLoc);
			AdditionalExtractorPlugInLocs = new(copyThis.AdditionalExtractorPlugInLocs);
			FlatPlayList = new(copyThis.FlatPlayList);
			LiveFromStart = new(copyThis.LiveFromStart);
			WaitForVid = new(copyThis.WaitForVid);
			MarkWatched = new(copyThis.MarkWatched);
			ColorWhenInStdOut = new(copyThis.ColorWhenInStdOut);
			ColorWhenInStdErr = new(copyThis.ColorWhenInStdErr);
		}


		/// <summary>
		/// Use to select a color policy.  You must use one of the static readonly members by design.  These values represent those known to yt-dlp.  <see cref="@default"/> tells the option to emit no parameter.
		/// </summary>
		public class ColorPoliciesForStreams : IOneOptVal
		{
			/// <summary>
			/// Constructs a new <see cref="ColorPoliciesForStreams"/>
			/// </summary>
			/// <param name="strRepresentation">The string to associate with this value</param>
			private ColorPoliciesForStreams(in string strRepresentation)
				=> Representation = strRepresentation;


			/// <summary>
			/// Returns the string to associate with this value
			/// </summary>
			public string? Representation
			{
				get;

				private init;
			}

			/// <summary>
			/// Returns the value as a string.
			/// </summary>
			public string? ValText
				=> Representation;



			/// <summary>
			/// Always use colors
			/// </summary>
			public static readonly ColorPoliciesForStreams always = new("always");

			/// <summary>
			/// yt-dlp should choose
			/// </summary>
			public static readonly ColorPoliciesForStreams auto = new("auto");

			/// <summary>
			/// Never use color
			/// </summary>
			public static readonly ColorPoliciesForStreams never = new("never");

			/// <summary>
			/// Use only non color terminal sequences
			/// </summary>
			public static readonly ColorPoliciesForStreams noColor = new("no_color");

			/// <summary>
			/// yt-dlp decides based on terminal support only.
			/// </summary>
			public static readonly ColorPoliciesForStreams autoTTY = new("auto-tty");

			/// <summary>
			/// Like <see cref="noColor"/>, but yt-dlp decides based on terminal support only.
			/// </summary>
			public static readonly ColorPoliciesForStreams noColorTTY = new("no_color-tty");

			/// <summary>
			/// Prevents a parameter from being emitted.  If a config file specifies a color value with --color, that would be used.  Otherwise, yt-dlp will assume
			/// you wanted <see cref="auto"/>.
			/// </summary>
			public static readonly ColorPoliciesForStreams @default = new(string.Empty);
		}

		/// <summary>
		/// Causes yt-dlp to ignore download and post-processing errors.  The download will be considered successful even if the post-processing fails.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralIgnoreErrors), @"Ignore download and post-processing errors.  The download will be considered successful even if the post-processing fails.", typeof(GeneralGroup), @"--ignore-errors")]
		public OneOpt<bool> IgnoreErrors
		{
			get;
		} = new(false, @"--ignore-errors", @"ignoreerrors");

		/// <summary>
		/// If <see langword="false"/>, Continue with next video on download errors; e.g. to skip unavailable videos in a playlist.  If <see langword="true"/>, abort downloading of further videos if an error occurs.  If <see langword="null"/>, the system acts as though you used <see langword="false"/>, but no parameter will be passed to yt-dlp.  However, the default value of <see langword="null"/> might allow a config file might cause yt-dlp to act as though this were <see langword="true"/>.  A <see langword="true"/> or <see langword="false"/> value overrides the config file while <see langword="null"/> doesn't override the config file.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralAbortOnError), @"If false, Continue with next video on download errors; e.g. to skip unavailable videos in a playlist; If true, abort downloading of further videos if an error occurs; If null, the system acts as though you used false, but no parameter will be passed to yt-dlp.  However, the default value of null might allow a config file might cause yt-dlp to act as though this were true.  A true or false value overrides the config file while null doesn't override the config file.", typeof(GeneralGroup), @"--abort-on-error",
			@"--no-abort-on-error")]
		public ThreeWayOpt AbortOnError
		{
			get;
		} = new(@"--abort-on-error", @"--no-abort-on-error")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.Val is true or null
						? []
						: new DictionaryToOpt()
						{
							[@"ignoreerrors"] = @"only_download",
						},
		};

		/// <summary>
		/// Extractor names to use separated by commas.  You can also use regular expressions, “all”, “default”, “end” (end URL matching) like <c>"holodex.*,end,youtube"</c>.  Prefix the name with a “-” to exclude it.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralUseExtractors), @"Extractor names to use separated by commas.  You can also use regular expressions, “all”,"
			+ "“default”, “end” (end URL matching) like \"holodex.*,end,youtube\".  Prefix the name with a “-” to exclude it.", typeof(GeneralGroup),
			@"--use-extractors")]
		public OneOpt<System.Collections.Generic.IReadOnlyCollection<string>> UseExtractors
		{
			get;
		} = new([], @"--use-extractors")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal.Count == 0
						? []
						: new DictionaryToOpt()
						{
							[@"allowed_extractors"] = opt.CurVal,
						},
		};

		/// <summary>
		/// Use this prefix for unqualified URLs. E.g. <c>"gvsearch2:python"</c> downloads two videos from Google videos for the search term “python”.  Use the value “auto” to let yt-dlp guess.  To emit a warning when yt-dlp guesses, use “auto_warning”.  “error” just throws an error.  “fixup_error” repairs broken URLs, but emits an error if this is not possible instead of searching.  If DefSearchPrefix is empty and no config file specifies --default-search, yt-dlp acts as though DefSearchPrefix contains “fixup_error”.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralDefSearchPrefix), @"Use this prefix for unqualified URLs. E.g. ""gvsearch2:python"" downloads two videos from Google videos for the search term “python”.  Use the value “auto” to let yt-dlp guess.  To emit a warning when yt-dlp guesses, use “auto_warning”.  “error” just throws an error.  “fixup_error” repairs broken URLs, but emits an error if this is not possible instead of searching.  If DefSearchPrefix is empty and no config file specifies --default-search, yt-dlp acts as though DefSearchPrefix contains “fixup_error”.",
			typeof(GeneralGroup), @"--default-search")]
		public OneOpt<string> DefSearchPrefix
		{
			get;
		} = new("", @"--default-search")
		{
			PythonParamsGenerator =
				(in opt)
				=> opt.CurVal.Length == 0
					? []
					: new DictionaryToOpt()
					{
						[@"default_search"] = opt.CurVal
					},
		};

		/// <summary>
		/// Don’t load any more configuration files except those given to in <see cref="ConfigLoc"/>.  For backward compatibility, if this option is found inside the system configuration file, the user configuration is not loaded.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralIgnoreConfigFiles), @"Don’t load any more configuration files except those given to in ConfigLoc.  For backward compatibility, if this option is found inside the system configuration file, the user configuration is not loaded.", typeof(GeneralGroup),
			@"--ignore-config")]
		public OneOpt<bool> IgnoreConfigFiles
		{
			get;
		} = new(false, @"--ignore-config");

		/// <summary>
		/// Do not load any custom configuration files.  Note this seems to conflict with <see cref="ConfigLoc"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralNoConfigLoc), @"Do not load any custom configuration files.  Note this seems to conflict with ConfigLoc.", typeof(GeneralGroup), @"--no-config-locations")]
		public OneOpt<bool> NoConfigLoc
		{
			get;
		} = new(false, @"--no-config-locations");

		/// <summary>
		/// Location of the main configuration file; either the path to the config or its containing directory (a <see langword="null"/> value causes yt-dlp to get a config via stdin).  This is an array.  Each entry in the array adds a entry.  Except for null values, all entries should be instances of either <see cref="System.IO.FileInfo"/> or <see cref="System.IO.DirectoryInfo"/>.  The order of entries is important, but no more details is known.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralConfigLoc), @"Location of the main configuration file; either the path to the config or its containing directory (a null value causes yt-dlp to get a config via stdin).  This is an array.  Each entry in the array adds a entry.  Except for null values,all entries should be instances of either System.IO.FileInfo or System.IO.DirectoryInfo.  The order of entries is important, but no more details is known.",
			typeof(GeneralGroup), @"--config-locations")]
		public OneOpt<System.Collections.Generic.IEnumerable<System.IO.FileSystemInfo>?> ConfigLoc
		{
			get;
		} = new(null, @"--config-locations")
		{
			HowToCombineVals = OneOpt<System.Collections.Generic.IEnumerable<System.IO.FileSystemInfo>?>.ValCombinationRules.paramReuse,
			MultipleValTextLookUp = curVal
				=> curVal == null
					? []
					: curVal.Select(
						curEntry
							=> curEntry.FullName
				),
		};

		/// <summary>
		/// Path to an additional directory to search for plugins. This option can be used multiple times to add multiple directories. Use a <see langword="null"/> entry to search the default plugin directories.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralExtraExtractorPlugInLocs), @"Path to an additional directory to search for plugins.  Use multiple entries for each directory.  Use a null entry to search the default plugin directories.", typeof(GeneralGroup), @"--plugins-dir")]
		public OneOpt<System.Collections.Generic.IEnumerable<System.IO.DirectoryInfo>?> AdditionalExtractorPlugInLocs
		{
			get;
		} = new(null, @"--plugins-dir")
		{
			HowToCombineVals = OneOpt<System.Collections.Generic.IEnumerable<System.IO.DirectoryInfo>?>.ValCombinationRules.paramReuse,
			MultipleValTextLookUp = curVal
				=> curVal is null
					? []
					: curVal.Select(
							curEntry
								=> curEntry.FullName
						),
		};

		/// <summary>
		/// Do not extract a playlist's URL result entries.  This may cause some entry metadata may be missing and downloading may be bypassed.  Use <see langword="true"/> to use the flat playlist.  Use <see langword="false"/> to not use the flat playlist.  Use <see langword="null"/> to get a default value which might come from a config file.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralFlatPlayList), @"Do not extract a playlist's URL result entries.  This may cause some entry metadata may be missing and downloading may be bypassed.  Use true to use the flat playlist.  Use false to not use the flat playlist.  Use null to get a default value which might come from a config file.", typeof(GeneralGroup), @"--flat-playlist", @"--no-flat-playlist")]
		public ThreeWayOpt FlatPlayList
		{
			get;
		} = new(@"--flat-playlist", @"--no-flat-playlist")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.Val switch
					{
						true
							=> new DictionaryToOpt()
							{
								[@"extract_flat"] = @"in_playlist",
							},

						false
							=> new DictionaryToOpt()
							{
								[@"extract_flat"] = @"discard_in_plalist",
							},

						_
							=> [],
					}
		};

		/// <summary>
		/// Download live streams from the start.  Currently only supported for YouTube (Experimental).  Use <see langword="true"/> to turn this on.  Use <see langword="false"/> to force it to be off.  Use <see langword="null"/> to use the default value (<see langword="false"/>) unless your config file specifies it in which case, that value will be used.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralLiveFromStart), @"Download live streams from the start.  Currently only supported for YouTube (Experimental).  Use true to turn this on.  Use false to force it to be off.  Use null to use the default value (false) unless your config file specifies it in which case, that value will be used.", typeof(GeneralGroup), @"--live-from-start", @"--no-live-from-start")]
		public ThreeWayOpt LiveFromStart
		{
			get;
		} = new(@"--live-from-start", @"--no-live-from-start", @"live_from_start", ThreeWayOpt.WhichValsToSendToPythonChoices.both);

		/// <summary>
		/// Wait for scheduled streams to become available.  Pass the minimum number of seconds (or range) to wait between retries.  Put the start in the <see cref="TimeRangeVal.Start"/> property and the end in the <see cref="TimeRangeVal.End"/> property.  <see cref="TimeRangeVal.End"/> isn’t valid until <see cref="TimeRangeVal.Start"/> is non-<see langword="null"/>.  That’s because yt-dlp requires a range with a start regardless.  By default, yt-dlp doesn't wait for videos, unless a config file specifies --wait-for-video.  You can specify this behavior by specifying a <see langword="null"/> value for <see cref="WaitForVid"/> or creating a <see cref="TimeRangeVal"/>, but not setting <see cref="TimeRangeVal.Start"/>.  Note: <see cref="TimeRangeVal.invalid"/> is special.  It causes --no-wait-for-video to be passed.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralWaitForVid), @"Wait for scheduled streams to become available.  Pass the minimum number of seconds (or range) to wait between retries.  Put the start in the Start property and the end in the End property.  End isn’t valid until Start is non-null.  That’s because yt-dlp requires a range with a start regardless.  By default, yt-dlp doesn't wait for videos, unless a config file specifies --wait-for-video.  You can specify this behavior by specifying a null value for WaitForVid or creating a TimeRangeVal, but not setting Start.  Note: TimeRangeVal.invalid is special.  It causes --no-wait-for-video to be passed.", typeof(GeneralGroup), @"--wait-for-video", @"--no-wait-for-video")]
		public OneOpt<TimeRangeVal?> WaitForVid
		{
			get;
		} = new(null, string.Empty)
		{
			ParamListGenerator =
				opt
					=> opt.CurVal is null
						? []
						: opt.CurVal is TimeRangeVal val
							? val == TimeRangeVal.invalid
								? [@"--no-wait-for-video"]
								: val.ValText is string strValText
									? [@"--wait-for-video", val.ValText]
									: []
							: throw new System.InvalidProgramException(@"How did a non-time range get in here?"),

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is null
						? []
						: opt.CurVal is TimeRangeVal val && opt.CurVal.Start is System.TimeSpan tsStart
							? val == TimeRangeVal.invalid
								? []
								: opt.CurVal.End is System.TimeSpan tsEnd
									? new DictionaryToOpt()
									{
										[@"wait_for_video"] =
												new System.Collections.Generic.List<double?>()
												{
													tsStart.TotalSeconds,
													tsEnd.TotalSeconds,
												},
									}
									: new DictionaryToOpt()
									{
										[@"wait_for_video"] =
												new System.Collections.Generic.List<double?>()
												{
													tsStart.TotalSeconds,
													null,
												},
									}
								: [],
		};

		/// <summary>
		/// Mark videos watched (even in simulate mode).  Use <see langword="null"/> to get the default value which acts like a <see langword="false"/> value unless this is set in a config file.  Use <see langword="true"/> to force it on and <see langword="false"/> to force it off.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralMarkWatched), @"Mark videos watched (even in simulate mode).  Use null to get the default value which acts like a false value unless this is set in a config file.  Use true to force it on and false to force it off.", typeof(GeneralGroup),
			@"--mark-watched", @"--no-mark-watched")]
		public ThreeWayOpt MarkWatched
		{
			get;
		} = new(@"--mark-watched", @"--no-mark-watched", @"mark_watched", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// Whether to emit color codes in the output stream stdout.  Can be one of <see cref="ColorPoliciesForStreams.always"/>, <see cref="ColorPoliciesForStreams.auto"/> (default), <see cref="ColorPoliciesForStreams.never"/>, or <see cref="ColorPoliciesForStreams.noColor"/> (use non color terminal sequences).  Use <see cref="ColorPoliciesForStreams.autoTTY"/> or <see cref="ColorPoliciesForStreams.noColorTTY"/> to decide based on terminal support only.  For the stderr equivalent, see <see cref="ColorWhenInStdErr"/>.  <see cref="ColorPoliciesForStreams.@default"/> prevents a parameter from being emitted allowing any value from a config file or yt-dlp’s default to take effect.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralColorWhenStdOut), @"Whether to emit color codes in the output stream stdout.  Can be one  of ColorPoliciesForStreams.always, ColorPoliciesForStreams.auto (default), ColorPoliciesForStreams.never, or ColorPoliciesForStreams.noColor (use non color terminal sequences).  Use ColorPoliciesForStreams.autoTTY or ColorPoliciesForStreams.noColorTTY to decide based on terminal support only.  For the stderr equivalent, see ColorWhenInStdErr.  ColorPoliciesForStreams.@default prevents a parameter from being emitted allowing any value from a config file or yt-dlp’s default to take effect.", typeof(GeneralGroup), @"--color")]
		public OneOptWithPrefix<ColorPoliciesForStreams> ColorWhenInStdOut
		{
			get;
		} = new(ColorPoliciesForStreams.@default, @"--color", @"stdout")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal == ColorPoliciesForStreams.@default
						? []
						: new DictionaryToOpt()
						{
							[@"color"] = new DictionaryToOpt()
							{
								[opt.strValPrefix] = opt.CurVal.Representation,
							}
						}
		};

		/// <summary>
		/// Whether to emit color codes in the output stream stderr.  Can be one of <see cref="ColorPoliciesForStreams.always"/>, <see cref="ColorPoliciesForStreams.auto"/> (default), <see cref="ColorPoliciesForStreams.never"/>, or <see cref="ColorPoliciesForStreams.noColor"/> (use non color terminal sequences).  Use <see cref="ColorPoliciesForStreams.autoTTY"/> or <see cref="ColorPoliciesForStreams.noColorTTY"/> to decide based on terminal support only.  For the stdout equivalent, see <see cref="ColorWhenInStdOut"/>.  <see cref="ColorPoliciesForStreams.@default"/> prevents a parameter from being emitted allowing any value from a config file or yt-dlp’s default to take effect.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeneralColorWhenStdErr), @"Whether to emit color codes in the output stream stderr.  Can be one  of ColorPoliciesForStreams.always, ColorPoliciesForStreams.auto (default), ColorPoliciesForStreams.never, or ColorPoliciesForStreams.noColor (use non color terminal sequences).  Use ColorPoliciesForStreams.autoTTY or ColorPoliciesForStreams.noColorTTY to decide based on terminal support only.  For the stdout equivalent, see ColorWhenInStdOut.  ColorPoliciesForStreams.@default prevents a parameter from being emitted allowing any value from a config file or yt-dlp's default to take effect.", typeof(GeneralGroup), @"--color")]
		public OneOptWithPrefix<ColorPoliciesForStreams> ColorWhenInStdErr
		{
			get;
		} = new(ColorPoliciesForStreams.@default, @"--color", @"stderr")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal == ColorPoliciesForStreams.@default
						? []
						: new DictionaryToOpt()
						{
							[@"color"] = new DictionaryToOpt()
							{
								[opt.strValPrefix] = opt.CurVal.Representation,
							}
						}
		};


		/// <summary>
		/// Stores a list of all options for <see cref="GeneralGroup"/>/<see cref="General"/>.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=>
			[
				IgnoreErrors,
				AbortOnError,
				DefSearchPrefix,
				IgnoreConfigFiles,
				NoConfigLoc,
				ConfigLoc,
				AdditionalExtractorPlugInLocs,
				FlatPlayList,
				LiveFromStart,
				WaitForVid,
				MarkWatched,
				ColorWhenInStdOut,
				ColorWhenInStdErr,
			];
	}
}