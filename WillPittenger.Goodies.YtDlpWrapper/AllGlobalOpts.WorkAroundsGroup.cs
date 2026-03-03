// Ignore Spelling: Dlp Vals uri Sel wget Xattr Hls Sys Arounds Vid Fmts Pwd Executables Evt Exe Evts Loc Locs Json Dest Hdrs Langs

namespace WillPittenger.Goodies.YtDlpWrapper;

using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Groups various options related to workarounds.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#workarounds">Workarounds section of yt-dlp’s readme.md</a>.
	/// </summary>
	public sealed class WorkAroundsGroup
	{
		public WorkAroundsGroup()
		{
		}

		public WorkAroundsGroup(in WorkAroundsGroup copyThis)
		{
			Encoding = new(copyThis.Encoding);
			ExtraHdrs = new(copyThis.ExtraHdrs);
			BIDI = new(copyThis.BIDI);
			SleepRequests = new(copyThis.SleepRequests);
			SleepIntervals = new(copyThis.SleepIntervals);
			SubsSleep = new(copyThis.SubsSleep);
			UseStdSleep = new(copyThis.UseStdSleep);
		}


		/// <summary>
		/// Force the specified encoding (experimental)
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strWorkAroundsEncoding), @"Force the specified encoding (experimental)", typeof(WorkAroundsGroup), @"--encoding")]
		public OneOpt<System.Text.Encoding?> Encoding
		{
			get;
		} = new(null, @"--encoding")
		{
			ValTextLookUp =
				curVal
					=> curVal?.WebName,

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is System.Text.Encoding
						? new DictionaryToOpt()
						{
							[@"encoding"] = opt.CurVal.WebName,
						}
						: [],
		};

		/// <summary>
		/// For each entry, causes yt-dlp to add headers to each download.  Each key will be a header key with the value for that key being added to the header key.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strWorkAroundsExtraHdrs), @"For each entry, causes yt-dlp to add headers to each download.  Each key will be a header key with the value for that key being added to the header key.", typeof(WorkAroundsGroup), @"--add-headers")]
		public OneOpt<System.Collections.Generic.IDictionary<string, string>> ExtraHdrs
		{
			get;
		} = new(new System.Collections.Generic.SortedList<string, string>(), @"--add-headers")
		{
			HowToCombineVals = OneOpt<System.Collections.Generic.IDictionary<string, string>>.ValCombinationRules.paramReuse,

			HowToMakeKeyValPairStrings = OneOpt<System.Collections.Generic.IDictionary<string, string>>.KeyValCombinationRules.colon,

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal.Count > 0
						? new DictionaryToOpt()
						{
							[@"http_headers"] = opt.CurVal,
						}
						: [],
		};

		/// <summary>
		/// Work around terminals that lack bidirectional text support.  Requires bidiv or fribidi executable in PATH
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strWorkAroundsBIDI), @"Work around terminals that lack bidirectional text support.  Requires bidiv or fribidi executable in PATH", typeof(WorkAroundsGroup), @"--bidi-workaround")]
		public OneOpt<bool> BIDI
		{
			get;
		} = new(false, @"--bidi-workaround")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
						{
							[@"bidi_workaround"] = true,
						}
						: [],
		};

		/// <summary>
		/// Number of seconds to sleep between requests during data extraction
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strWorkAroundsSleepRequests), @"Number of seconds to sleep between requests during data extraction", typeof(WorkAroundsGroup), @"--sleep-requests")]
		public OneOpt<double?> SleepRequests
		{
			get;
		} = new(null, @"--sleep-requests")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is null || opt.CurVal < 0
						? []
						: new DictionaryToOpt()
						{
							[@"sleep_interval_requests"] = opt.CurVal,
						},
		};

		/// <summary>
		/// Controls how long yt-dlp sleeps for before each download.  Set <see cref="UShortRangeOpt.Min"/> to a non-<see langword="null"/> value to specify a minimum interval.  Set <see cref="UShortRangeOpt.Max"/> to a non-<see langword="null"/> value to specify a maximum interval.  Leave one or both to allow those values to come from any config file specifying any of the following: --sleep-interval, --min-sleep-interval, --max-sleep-interval.  Normally, yt-dlp doesn't sleep between downloads.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strWorkAroundsSleepIntervals), @"Controls how long yt-dlp sleeps for before each download.  Set SleepIntervals.Min to a non-null value to specify a minimum interval.  Set SleepIntervals.Max to a non-null value to specify a maximum interval.  Leave one or both to allow those values to come from any config file specifying any of the following: --sleep-interval, --min-sleep-interval, --max-sleep-interval.  Normally, yt-dlp doesn't sleep between downloads.", typeof(WorkAroundsGroup), @"--min-sleep-interval", @"--max-sleep-interval")]
		public UShortRangeOpt SleepIntervals
		{
			get;
		} = new(@"--min-sleep-interval", @"--max-sleep-interval", @"sleep_interval", @"max_sleep_interval")
		{
			MaxValidByItself = false,
		};

		/// <summary>
		/// Number of seconds to sleep before each subtitle download
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strWorkAroundsSubsSleep), @"Number of seconds to sleep before each subtitle download", typeof(WorkAroundsGroup), @"--sleep-subtitles")]
		public OneOpt<double?> SubsSleep
		{
			get;
		} = new(null, @"--sleep-subtitles", @"sleep_interval_subtitles");

		public OneOpt<bool> UseStdSleep
		{
			get;
		} = new(true, @"-t")
		{
			ParamListGenerator =
				opt
					=> opt.CurVal
						?[
							@"-t",
							@"sleep"
							]
						: [],

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
							{
								[@"max_sleep_intervals"] = 20.0,
								[@"sleep_interval"] = 10.0,
								[@"sleep_interval_requests"] = .75,
								[@"sleep_interval_subtitles"] = 5.0,
							}
						: [],
		};


		/// <summary>
		/// Lists all options in a <see cref="WorkArounds"/> instance.  The owner of this instance will use <see cref="AllOpt"/> to generate its own list.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				Encoding,
				ExtraHdrs,
				BIDI,
				SleepRequests,
				SleepIntervals,
				SubsSleep,
				UseStdSleep,
			];
	}
}