namespace WillPittenger.Goodies.YtDlpWrapper;

using Python.Runtime;

using System.Linq;

using Tools.Ext;
using Ext;

using IOptList = System.Collections.Generic.IEnumerable<object>;
using IReadOnlyDictionaryToOptLists = System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<object>?>;
using IReadOnlyDictionaryToOpt = System.Collections.Generic.IReadOnlyDictionary<string, object?>;
using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	///  Groups all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#video-selection">Video Selection Options section of yt-dlp’s readme.md</a>.
	/// </summary>
	[System.ComponentModel.ImmutableObject(true)]
	public sealed partial class VidSelGroup
	{
		public VidSelGroup()
		{
		}

		public VidSelGroup(in VidSelGroup copyThis)
		{
			MinFileSize = new(copyThis.MinFileSize);
			MaxFileSize = new(copyThis.MaxFileSize);
			ByDate = new(copyThis.ByDate);
			Filters = new(copyThis.Filters);
			PlayListOrVidSel = new(copyThis.PlayListOrVidSel);
			AgeLimit = new(copyThis.AgeLimit);
			DownLoadArchive = new(copyThis.DownLoadArchive);
			BreakOnExisting = new(copyThis.BreakOnExisting);
			BreakPerInput = new(copyThis.BreakPerInput);
			SkipPlayListAfterErrors = new(copyThis.SkipPlayListAfterErrors);
		}


		[System.ComponentModel.ImmutableObject(true)]
		public partial class FileSizeVal(double dblSize, FileSizeVal.UnitsOfMeasurment? uom = null) : IOneOptVal
		{
			public readonly double dblSize = dblSize;

			public UnitsOfMeasurment uom = uom ?? UnitsOfMeasurment.bytes;

			[System.ComponentModel.ImmutableObject(true)]
			public class UnitsOfMeasurment
			{
				private UnitsOfMeasurment(in string strDescName, in string strShortName, in string strSendAs, in decimal decBytesInOneUnit)
				{
					this.strDescName = strDescName;
					this.strShortName = strShortName;
					this.strSendAs = strSendAs;
					this.decBytesInOneUnit = decBytesInOneUnit;

					mapNameToUOM[strDescName] = mapNameToUOM[strShortName] = mapNameToUOM[strSendAs] = this;
				}

				public readonly string strDescName;
				public readonly string strShortName;
				public readonly string strSendAs;
				public readonly decimal decBytesInOneUnit;

				private static readonly System.Collections.Generic.SortedDictionary<string, UnitsOfMeasurment> mapNameToUOM = new();

				public static readonly UnitsOfMeasurment bytes = new(@"bytes", @"b", string.Empty, 1m);
				public static readonly UnitsOfMeasurment kilobytes = new(@"kilobytes", @"kb", @"k", 1028m);
				public static readonly UnitsOfMeasurment megabytes = new(@"megabyes", @"mb", @"m", 1028m * 1028m);
				public static readonly UnitsOfMeasurment gigabytes = new(@"gigabytes", @"gb", @"g", 1028m * 1028m * 1028m);
				public static readonly UnitsOfMeasurment terabytes = new(@"terabytes", @"tb", @"t", 1028m * 1028m * 1028m * 1028m);

				public static UnitsOfMeasurment GetUomFromText(in string strNameToLookFor)
					=> mapNameToUOM[strNameToLookFor];
			}

			/// <inheritdoc/>
			public string? ValText
				=> $@"{dblSize:G5}{uom.strSendAs}";

			public decimal ValInBytes
				=> (decimal)dblSize * uom.decBytesInOneUnit;

			private static readonly System.Text.RegularExpressions.Regex regexFileSizeParser = MyRegex();

			public static implicit operator FileSizeVal?(in string strParseThis)
			{
				System.Text.RegularExpressions.Match matchParsingResult = regexFileSizeParser.Match(strParseThis);
				return matchParsingResult.Success
					? new(double.Parse(matchParsingResult.Groups[@"rawSize"].Value), UnitsOfMeasurment.GetUomFromText(matchParsingResult.Groups[@"uom"].Value))
					: null;
			}

			[System.Text.RegularExpressions.GeneratedRegexAttribute(@"(?i)(?<rawSize>\d+(\.\d+)?)(<?<uom>[bytes|kilobytes|megabytes|gigabytes|terabytes|b|kb|mb|gb|tb|k|m|g|t]?)", System.Text.RegularExpressions.RegexOptions.None, "en-US")]
			private static partial System.Text.RegularExpressions.Regex MyRegex();
		}

		/// <summary>
		/// Abstract class to represent a single date value in several different forms.
		/// </summary>
		public abstract class DateWrapperValCore : IOneOptVal
		{
			/// <inheritdoc/>
			public abstract string? ValText
			{
				get;
			}

			public abstract System.DateOnly? ActualDate
			{
				get;
			}
		}

		/// <summary>
		/// Implementation of <see cref="DateWrapperValCore"/> that stores an exact date without a time.  For a relative time, use <see cref="DateRelativeValWrapperValCore"/>.  An implicit typecast operator is provided from <see cref="System.DateOnly"/> to <see cref="DateOnlyWrapperValCore"/>.
		/// </summary>
		/// <param name="Val">The desired value</param>
		public class DateOnlyWrapperValCore(System.DateOnly Val) : DateWrapperValCore
		{
			/// <summary>
			/// The desired value
			/// </summary>
			public System.DateOnly Val
			{
				get;

				set;
			} = Val;


			/// <summary>
			/// Returns the value in <see cref="Val"/> as a <see cref="string"/> in the format needed by yt-dlp.
			/// </summary>
			public override string? ValText
				=> Val.ToString("yyyyMMdd");


			/// <summary>
			/// Converts an <see cref="System.DateOnly"/> value into a <see cref="DateOnlyWrapperValCore"/> instance.
			/// </summary>
			/// <param name="doVal">The value to place in the new <see cref="DateOnlyWrapperValCore"/></param>
			public static implicit operator DateOnlyWrapperValCore(in System.DateOnly doVal)
				=> new(doVal);

			public override System.DateOnly? ActualDate
				=> Val;
		}

		/// <summary>
		/// Implementation of <see cref="DateWrapperValCore"/> that stores dates in relative form.  If you need to specify an exact date, use <see cref="DateOnlyWrapperValCore"/>.  You need to specify one of several starting points or origins with an instance from <see cref="Origins"/>, a distance, and a scale for that distance with a value form <see cref="DistanceScales"/>.  You can end up with sometime resembling "2 weeks ago yesterday".
		/// </summary>
		/// <param name="Origin">The starting point.  Several values are defined as <see langword="public"/> <see langword="static"/> <see langword="readonly"/> members.</param>
		/// <param name="Distance">How far before the origin specified in <paramref name="Origin"/>.  How yt-dlp interprets <paramref name="Distance"/> is specified by <paramref name="Scale"/>.</param>
		/// <param name="Scale">Specifies how far to go back in time.  If <paramref name="Scale"/> is <see cref="DistanceScales.week"/> and <paramref name="Distance"/> is <c>6</c>, you'll get 6 weeks before the value in <paramref name="Origin"/>.</param>
		public class DateRelativeValWrapperValCore(DateRelativeValWrapperValCore.Origins Origin, long Distance, DateRelativeValWrapperValCore.DistanceScales Scale)
		: DateWrapperValCore
		{
			/// <summary>
			/// The starting point.  Several values are defined as <see langword="public"/> <see langword="static"/> <see langword="readonly"/> members.
			/// </summary>
			public Origins Origin
			{
				get;
			} = Origin;

			/// <summary>
			/// How far before the origin specified in <see cref="Origin"/>.  How yt-dlp interprets <see cref="Distance"/> is specified by <see cref="Scale"/>.
			/// </summary>
			public long Distance
			{
				get;
			} = Distance;

			/// <summary>
			/// Specifies how far to go back in time.  If <see cref="Scale"/> is <see cref="DistanceScales.week"/> and <see cref="Distance"/> is <c>6</c>, you’ll get 6 weeks before the value in <see cref="Origin"/>.
			/// </summary>
			public DistanceScales Scale
			{
				get;
			} = Scale;


			/// <summary>
			/// Lists valid origins.  These are the only origins that yt-dlp recognizes.
			/// </summary>
			[System.ComponentModel.ImmutableObject(true)]
			public sealed class Origins
			{
				/// <summary>
				/// Constructs a new instance.
				/// </summary>
				/// <param name="strText">The required text for this origin</param>
				private Origins(in string strText)
					=> this.strText = strText;


				/// <summary>
				/// The required text for this origin.
				/// </summary>
				public readonly string strText;


				/// <summary>
				/// An origin that represents "now".  This might behave identically to <see cref="today"/>.
				/// </summary>
				public static readonly Origins now = new("now");

				/// <summary>
				/// An origin that represents "today".  This might behave identically to <see cref="now"/>.
				/// </summary>
				public static readonly Origins today = new("today");

				/// <summary>
				/// An origin that represents "yesterday".
				/// </summary>
				public static readonly Origins yesterday = new("yesterday");
			}

			/// <summary>
			/// Lists valid scales.  These are the only values that yt-dlp recognizes.
			/// </summary>
			[System.ComponentModel.ImmutableObject(true)]
			public sealed class DistanceScales
			{
				/// <summary>
				/// Constructs a new instance.
				/// </summary>
				/// <param name="strText">The required text for this scale</param>
				private DistanceScales(in string strText)
					=> this.strText = strText;


				/// <summary>
				/// The required text for this scale.
				/// </summary>
				public readonly string strText;


				/// <summary>
				/// A scale that specifies the distance in <paramref cref="Distance"/> should be treated as that many days.
				/// </summary>
				public static readonly DistanceScales day = new("day");

				/// <summary>
				/// A scale that specifies the distance in <paramref cref="Distance"/> should be treated as that many weeks.
				/// </summary>
				public static readonly DistanceScales week = new("week");

				/// <summary>
				/// A scale that specifies the distance in <paramref cref="Distance"/> should be treated as that many months.
				/// </summary>
				public static readonly DistanceScales month = new("month");

				/// <summary>
				/// A scale that specifies the distance in <paramref cref="Distance"/> should be treated as that many years.
				/// </summary>
				public static readonly DistanceScales year = new("year");
			}


			/// <summary>
			/// Returns the value as text in the format expected by yt-dlp.
			/// </summary>
			public override string? ValText
				=> $"{Origin.strText}-{Distance}{Scale}";

			public override System.DateOnly? ActualDate
			{
				get
				{
					System.DateTime dtOrigin = Origin == Origins.now || Origin == Origins.today
						? System.DateTime.Today
						: Origin == Origins.yesterday
							? (System.DateTime.Today - System.TimeSpan.FromDays(-1))
							: throw new System.InvalidProgramException(@"Unknown value for an Origin");

					return (dtOrigin +
						(
							Scale == DistanceScales.year
								? System.TimeSpan.FromDays(365 * Distance)
								: Scale == DistanceScales.month
									? System.TimeSpan.FromDays(30 * Distance)
									: Scale == DistanceScales.week
										? System.TimeSpan.FromDays(7 * Distance)
										: Scale == DistanceScales.day
											? System.TimeSpan.FromSeconds(Distance)
											: throw new System.InvalidProgramException(@"Unknown value for a DistanceScale")
						)).AsDateOnly();
				}
			}
		}

		/// <summary>
		/// Stores either a pair of <see cref="DateWrapperValCore"/> instances or a single one.  If you need a pair, use <see cref="DatePairVal"/>.  It allows for both relative and exact dates or even a mix.  If you only need one date, use <see cref="DateSelVal"/>.  It also allows for both relative and exact dates, but only one value.  Exact dates should be specified as <see cref="DateOnlyWrapperValCore"/> instances.  Relative dates should be specified as instances of <see cref="DateRelativeValWrapperValCore"/>.
		/// </summary>
		public abstract class BaseDateWrapperVal
		{
			/// <summary>
			/// When implemented in a derived class, returns an array of <see cref="string"/> values, one for each value needing to be passed.  This can be several.
			/// </summary>
			public abstract System.Collections.Generic.IEnumerable<string> Params
			{
				get;
			}

			public abstract DictionaryToOpt? GetPythonParams(in OneOpt<BaseDateWrapperVal?> opt);
		}

		/// <summary>
		/// Represents a pair of dates.  This can be two relative dates, two exact dates, or one of each.
		/// </summary>
		/// <param name="Before">The starting date</param>
		/// <param name="After">The ending date</param>
		public class DatePairVal(in DateWrapperValCore? Before, in DateWrapperValCore? After)
			: BaseDateWrapperVal
		{
			/// <summary>
			/// The starting date
			/// </summary>
			public DateWrapperValCore? Before
			{
				get;

				set;
			} = Before;

			/// <summary>
			/// The ending date
			/// </summary>
			public DateWrapperValCore? After
			{
				get;

				set;
			} = After;


			/// <inheritdoc/>
			public override System.Collections.Generic.IEnumerable<string> Params
			{
				get
				{
					string? strBeforeVal = Before?.ValText;
					string? strAfterVal = After?.ValText;

					return strBeforeVal is null && strAfterVal is null
						? []
						: strBeforeVal is null && strAfterVal is not null
							? [@"--dateafter", strAfterVal]
							: strBeforeVal is not null && strAfterVal is null
								? [@"--datebefore", strBeforeVal]
								: strBeforeVal is not null && strAfterVal is not null
									? [@"--datebefore", strBeforeVal, @"--dateafter", strAfterVal]
									: [];
				}
			}

			public override DictionaryToOpt? GetPythonParams(in OneOpt<BaseDateWrapperVal?> opt)
			{
				System.DateOnly? doBefore = Before?.ActualDate;
				System.DateOnly? doAfter = After?.ActualDate;

				if(doBefore is null && doAfter is null)
					return [];

				using Py.GILState lockInfo = Py.GIL();
				YtDlpWrapper ytWrapper = new(YtDlpWrapper.WhatToInit.python);
				using dynamic? ytDateRangeClass = YtDlpWrapper.yt?.utils?.__utils?.DateRange;
				return ytDateRangeClass is not null
					? new DictionaryToOpt()
					{
						[@"daterange"] = ytDateRangeClass((doAfter ?? System.DateTime.Today.AsDateOnly()).ToString(@"yyyy-MM-dd"), doBefore ?? System.DateTime.Today.AsDateOnly()).ToString(@"yyyy-MM-dd"),
					}
					: throw new System.InvalidOperationException(@"Unable to get access to yt-dlp's Python interface");
			}
		}

		/// <summary>
		/// Represents a single date.  If you need separate before and after values, use <see cref="DatePairVal"/>.
		/// </summary>
		/// <param name="On">The desired date.</param>
		public class DateSelVal(DateWrapperValCore On) : BaseDateWrapperVal
		{
			/// <summary>
			/// Returns a list of the needed parameters.
			/// </summary>
			public override System.Collections.Generic.IEnumerable<string> Params
			{
				get
				{
					string strValText = On?.ValText ?? string.Empty;

					return strValText == string.Empty
						? []
						: [@"--date", strValText];
				}
			}


			/// <summary>
			/// Wraps the specified value in a new <see cref="DateSelVal"/> instance
			/// </summary>
			/// <param name="val">The value to wrap</param>
			public static implicit operator DateSelVal(in DateWrapperValCore val)
				=> new(val);

			public override DictionaryToOpt? GetPythonParams(in OneOpt<BaseDateWrapperVal?> opt)
			{
				if(On.ActualDate is System.DateOnly doToUse)
				{
					using Py.GILState lockInfo = Py.GIL();
					YtDlpWrapper ytWrapper = new(YtDlpWrapper.WhatToInit.python);
					using dynamic? ytDateRangeClass = YtDlpWrapper.yt?.utils?.__utils?.DateRange;

					string strFormattedDateToUse = doToUse.ToString(@"yyyy-MM-dd");

					return ytDateRangeClass is not null
						? new DictionaryToOpt()
						{
							[@"daterange"] = ytDateRangeClass(strFormattedDateToUse, strFormattedDateToUse),
						}
						: throw new System.InvalidOperationException(@"Unable to get access to yt-dlp's Python interface");
				}

				return [];
			}
		}

		public sealed class FilterOpt : BaseOneOpt<FilterOpt>
		{
			public class WrapperBase
			{
				protected WrapperBase()
				{
				}


				public readonly static WrapperBase interactive = new();


				public static WrapperBase FromExisting(in WrapperBase copyThis)
					=> copyThis is ConditionWrapper cw
						? new ConditionWrapper(cw)
						: copyThis is TestFuncWrapper tfw
							? new TestFuncWrapper(tfw)
							: throw new System.InvalidOperationException($@"Unknown class derived from {typeof(TestFuncWrapper).FullName}: {copyThis.GetType().FullName}.  Can't duplicate it if we don't know what it is.");
			}

			[System.ComponentModel.ImmutableObject(true)]
			public class ConditionWrapper : WrapperBase
			{
				public readonly System.Collections.Generic.IEnumerable<string> estrAcceptFilters;

				public readonly System.Collections.Generic.IEnumerable<string> estrRejectFilters;


				public ConditionWrapper(in string? strMatchCondition = null, in string? strRejectCondition = null)
				{
					estrAcceptFilters = strMatchCondition is null
										? []
										: [strMatchCondition];
					estrRejectFilters = strRejectCondition is null
						? []
						: [strRejectCondition];
				}

				public ConditionWrapper(in System.Collections.Generic.IEnumerable<string>? estrMatchConditions)
				{
					this.estrAcceptFilters = estrMatchConditions ?? [];
					this.estrRejectFilters = estrRejectFilters ?? [];
				}

				public ConditionWrapper(in ConditionWrapper copyThis)
				{
					estrAcceptFilters = copyThis.estrAcceptFilters;
					estrRejectFilters = copyThis.estrRejectFilters;
				}
			}

			[System.ComponentModel.ImmutableObject(true)]
			public class TestFuncWrapper : WrapperBase
			{
				public delegate string? DTester(in DictionaryToOpt infoOnObj, in IOptList? eobj);

				public class StopDownloadingException(in string strMsg)
					: System.Exception(strMsg);


				public const string strNoDef = "NoDef";


				public readonly DTester funcToCall;

				public readonly IOptList? eobj;


				public TestFuncWrapper(in TestFuncWrapper copyThis)
				{
					funcToCall = copyThis.funcToCall;
					this.eobj = copyThis.eobj;
				}

				public TestFuncWrapper(in DTester funcToCall, in IOptList? eobj = null)
				{
					this.funcToCall = funcToCall;
					this.eobj = eobj;
				}


				public static implicit operator TestFuncWrapper(in DTester funcToCall)
					=> new(funcToCall);


				internal PyObject? CallTestFunc(Python.Runtime.PyDict dict, bool bIncomplete)
				{
					using Py.GILState lockInfo = Py.GIL();

					try
					{
						string? strRetVal = funcToCall(dict.ToManagedDictionary(), eobj);

						return strRetVal is not null
							? strRetVal.ToPython()
							: YtDlpWrapper.yt?.util.__util.NO_DEFAULT;
					}
					catch(StopDownloadingException e)
					{
						throw new PythonException(YtDlpWrapper.yt?.util.__util.RejectedVideoReached, YtDlpWrapper.yt?.util.__util.RejectedVideoReached(e.Message), null);
					}
				}
			}


			private WrapperBase? filters;


			public FilterOpt(in FilterOpt copyThis)
			{
				PythonParamsGenerator = GetPythonParams;

				filters = copyThis.filters is null
					? null
					: WrapperBase.FromExisting(copyThis.filters);
			}

			public FilterOpt(in WrapperBase? filters = null)
			{
				PythonParamsGenerator = GetPythonParams;

				this.filters = filters;
			}


			public WrapperBase? Filters
			{
				get
					=> filters;

				set
					=> filters = value;
			}


			public override System.Collections.Generic.IEnumerable<string>? Params
			{
				get
				{
					if(filters is TestFuncWrapper)
						throw new System.InvalidProgramException($@"Unable to send {filters} to the exe.");

					System.Collections.Generic.List<string> liststrParams = [];

					if(filters == WrapperBase.interactive)
						liststrParams.AddRange([@"--match-filters", @"-"]);
					else if(filters is ConditionWrapper conditions)
					{
						if(conditions.estrAcceptFilters is not null)
							liststrParams.AddRange
								(
									conditions.estrAcceptFilters.Select
										(
											strCurFilter
												=> new string[] { @"--match-filters", strCurFilter }
										).SelectMany
										(
											astr
												=> astr
										)
								);

						if(conditions.estrRejectFilters is ConditionWrapper)
							liststrParams.AddRange
								(
									conditions.estrRejectFilters.Select
										(
											strCurFilter
												=> new string[] { @"--break-match-filters", strCurFilter }
										).SelectMany(
											astr
												=> astr
										)
								);
					}

					return liststrParams;
				}
			}

			internal override DictionaryToOpt? PythonParams
				=> PythonParamsGenerator?.Invoke(this);


			private DictionaryToOpt? GetPythonParams(in FilterOpt opt)
			{
				using Py.GILState lockInfo = Py.GIL();

				if(opt.filters is TestFuncWrapper fw)
					return new DictionaryToOpt()
					{
						[@"match_filter"] = ((object)fw.CallTestFunc).ToPython(),
					};

				using dynamic? ytUtils = YtDlpWrapper.yt?.utils?.__utils;
				if(ytUtils is null)
					throw new System.InvalidOperationException(@"yt-dlp utility Python functions are unavailable");

				if(opt.filters == WrapperBase.interactive)
					return new DictionaryToOpt()
					{
						[@"match_filter"] = ytUtils.match_filter_func(@"-", null)
					};

				if(opt.filters is ConditionWrapper conditions)
				{
					Python.Runtime.PyList pyliststrAcceptFilters = new();
					Python.Runtime.PyList pyliststrRejectFilters = new();

					foreach(string strCurAcceptFilter in conditions.estrAcceptFilters)
						pyliststrAcceptFilters.Append(strCurAcceptFilter.ToPython());
					foreach(string strCurRejectFitler in conditions.estrRejectFilters)
						pyliststrRejectFilters.Append(strCurRejectFitler.ToPython());

					return new DictionaryToOpt()
					{
						[@"match_filter"] = ytUtils.match_filter_func(pyliststrAcceptFilters, pyliststrRejectFilters),
					};
				}

				return null;
			}


			public static implicit operator FilterOpt(WrapperBase wrapper)
				=> new(wrapper);
		}


		/// <summary>
		/// Lists choices how yt-dlp should interpret URLs that contain both playlist and video ID values.
		/// </summary>
		public enum PlayListOrVidTypes
		{
			/// <summary>
			/// Ignore the video and process the playlist.  The video will still be processed, but as just an entry in the list.
			/// </summary>
			playList,

			/// <summary>
			/// Ignore the playlist and process only the video.  Any playlist fields, including who create the playlist will be blank or  <see langword="null"/>.
			/// </summary>
			vid,

			/// <summary>
			/// yt-dlp should process the video, but including information on the playlist.  This is like <see cref="vid"/>, but includes more fields.
			/// </summary>
			@default,
		}


		/// <summary>
		/// Abort download if the file size is smaller than specified size, e.g. 50k or 44.6M.  A  <see langword="null"/> value causes the value from any config file or the default (no minimum) to be used.  Be careful combining this with <see cref="MaxFileSize"/>.  If <see cref="MinFileSize"/> is larger than <see cref="MaxFileSize"/>, no data will be downloaded!
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidSelMinFileSize), @"Abort download if the file size is smaller than specified size, e.g. 50k or 44.6M.  A null value causes the value from any config file or the default (no minimum) to be used.  Be careful combining this with MaxFileSize.  If MinFileSize is larger than MaxFileSize, no data will be downloaded!", typeof(VidSelGroup), @"--min-filesize")]
		public OneOpt<FileSizeVal?> MinFileSize
		{
			get;
		} = new(null, @"--min-filesize")
		{
			PythonParamsGenerator = (in opt)
				=> opt.CurVal is null
					? []
					: new DictionaryToOpt()
					{
						[@"min_filesize"] = opt.CurVal.ValInBytes,
					}
		};

		/// <summary>
		/// Abort download if the file size is larger than specified size, e.g. 50k or 44.6M.  A  <see langword="null"/> value causes the value from any config file or the default (no maximum) to be used.  Be careful combining this with <see cref="MinFileSize"/>.  If <see cref="MinFileSize"/> is larger than <see cref="MaxFileSize"/>, no data will be downloaded!
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidSelMaxFileSize), @"Abort download if the file size is larger than specified size, e.g. 50k or 44.6M.  A null value causes the value from any config file or the default (no maximum) to be used.  Be careful combining this with MinFileSize.  If MinFileSize is larger than MaxFileSize, no data will be downloaded!", typeof(VidSelGroup), @"--max-filesize")]
		public OneOpt<FileSizeVal?> MaxFileSize
		{
			get;
		} = new(null, @"--max-filesize")
		{
			PythonParamsGenerator = (in opt)
				=> opt.CurVal is null
					? []
					: new DictionaryToOpt()
					{
						[@"max_filesize"] = opt.CurVal.ValInBytes,
					}
		};

		/// <summary>
		/// Lets you filter videos based on the date.  You can specify just one date with a <see cref="DateSelVal"/> instance or a start and end with <see cref="DatePairVal"/>.  The former is essentially the latter with the two values set to the same value.  Regardless, any date can be specified as a <see cref="System.DateOnly"/> or a relative value.  With relative values, you specify an origin from <see cref="DateRelativeValWrapperValCore.Origins"/>, a distance from that origin in a scale specified separately, and a <see cref="DateRelativeValWrapperValCore.DistanceScales"/>.  The origins include <see cref="DateRelativeValWrapperValCore.Origins.now"/>, <see cref="DateRelativeValWrapperValCore.Origins.today"/>, and <see cref="DateRelativeValWrapperValCore.Origins.yesterday"/>.  The scales include <see cref="DateRelativeValWrapperValCore.DistanceScales.day"/>, <see cref="DateRelativeValWrapperValCore.DistanceScales.week"/>, <see cref="DateRelativeValWrapperValCore.DistanceScales.month"/>, and <see cref="DateRelativeValWrapperValCore.DistanceScales.year"/>.  So with <see cref="DateRelativeValWrapperValCore"/>, you specify 3 days before yesterday.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidSelByDate), @"Lets you filter videos based on the date.  You can specify just one date with a DateSelVal instance or a start and end with DatePairVal.  The former is essentially the latter with the two values set to the same value.  Regardless, any date can be specified as a System.DateOnly or a relative value.  With relative values, you specify an origin from DateRelativeValWrapperValCore.Origins, a distance from that origin in a scale specified separately, DateRelativeValWrapperValCore.DistanceScales.  The origins include Now, Today, and Yesterday.  The scales include day, week, month, and year.  So DateRelativeValWrapperValCore lets you specify 3 days before yesterday.",
			typeof(VidSelGroup))]
		public OneOpt<BaseDateWrapperVal?> ByDate
		{
			get;
		} = new(null, string.Empty)
		{
			ParamListGenerator =
				opt
					=> opt.CurVal == null
						? []
						: opt.CurVal.Params,

			PythonParamsGenerator = (in opt)
				=> opt.CurVal is null
					? []
					: opt.CurVal.GetPythonParams(opt),
		};

		/// <summary>
		/// Generic video filter.  Any <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template field">Output Template Field</a> can be compared with a number or a string using the operators defined in <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#filtering-formats.">Filtering Formats</a>.  You can also simply specify a field to match if the field is present, use “!field” to check if the field is not present, and “&amp;” to check multiple conditions.  Use a “\” to escape “&amp;” or quotes if needed.  If multiple elements are present, the filter matches if at least one of the conditions is met. E.g. <c>["!is_live", "like_count>?100 &amp; description~='(?i)\bcats \&amp; dogs\\b'"]</c> matches only videos that are not live OR those that have a like count more than 100 (or the like field is not available) and also has a description that contains the phrase “cats &amp; dogs” (caseless).  If you set this to just a string or string collection, those will be treated as match filters by creating a <see cref="FilterOpt.ConditionWrapper"/>.  If you want the conditions to reject videos, create a <see cref="FilterOpt.ConditionWrapper"/> manually.  If you want yt-dlp to ask on the command line if the video should be downloaded, set this to <see cref="FilterOpt.WrapperBase.interactive"/>.  If you're using the Python option, you can set this to a function that matches <see cref="FilterOpt.TestFuncWrapper.DTester"/>.  With that function, returning <see langword='null'/> causes yt-dlp to download the video.  Returning any string causes yt-dlp to reject the video and display the string.  Throw a <see cref="FilterOpt.TestFuncWrapper.StopDownloadingException"/> to tell yt-dlp to stop downloading.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidSelFilter), @"Generic video filter.  Any https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template field can be compared with a number or a string using the operators defined in https://github.com/yt-dlp/yt-dlp/blob/master/README.md#filtering-formats.  You can also simply specify a field to match if the field is present, use “!field” to check if the field is not present, and “&” to check multiple conditions.  Use a “\\” to escape “&” or quotes if needed.  If multiple elements are present, the filter matches if at least one of the conditions is met. E.g. [""!is_live"", ""like_count>?100 & description~='(?i)\bcats \& dogs\b'""] matches only videos that are not live OR those that have a like count more than 100 (or the like field is not available) and also has a description that contains the phrase “cats & dogs” (caseless).  If you set this to just a string or string collection, those will be treated as match filters by creating a ConditionWrapper.  If you want the conditions to reject videos, create a ConditionWrapper manually.  If you want yt-dlp to ask on the command line if the video should be downloaded, set this to WrapperBase.interactive.  If you're using the Python option, you can set this to a function that matches TestFuncWrapper.DTester.  With that function, returning [null] causes yt-dlp to download the video.  Returning any string causes yt-dlp to reject the video and display the string.  Throw a TestFuncWrapper.StopDownloadingException to tell yt-dlp to stop downloading.", typeof(VidSelGroup), @"--match-filters")]
		public FilterOpt Filters
		{
			get;
		} = new(new FilterOpt.ConditionWrapper());

		/// <summary>
		/// Some URLs refer to both a playlist and a video.  With such URLs, the default value for this option causes yt-dlp to process the playlist and the video unless the config file specifies otherwise.  Specify <see cref="PlayListOrVidTypes.playList"/> to download only the playlist.  Specify <see cref="PlayListOrVidTypes.vid"/> to download only the video.  The default value, <see cref="PlayListOrVidTypes.@default"/> causes yt-dlp to act like <see cref="PlayListOrVidSel"/> was set to <see cref="PlayListOrVidTypes.vid"/>, but it also fills in playlist fields for that video.  With <see cref="PlayListOrVidTypes.vid"/>, those would be empty strings or  <see langword="null"/>.  With <see cref="PlayListOrVidTypes.playList"/>, any video mentioned in the URL would be retrieved if it happens to be in the playlist and then only as just another entry.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidSelPlayListOrVidSel), @"Some URLs refer to both a playlist and a video.  With such URLs, the default value for this option causes yt-dlp to process the playlist and the video unless the config file specifies otherwise.  Specify playList to download only the playlist.  Specify vid to download only the video.", typeof(VidSelGroup), @"--yes-playlist", @"--no-playlist")]
		public OneOpt<PlayListOrVidTypes> PlayListOrVidSel
		{
			get;
		} = new(PlayListOrVidTypes.@default, string.Empty)
		{
			CustomParamNameLookUp =
				static curVal
					=> curVal switch
					{
						PlayListOrVidTypes.playList
							=> @"--yes-playlist",

						PlayListOrVidTypes.vid
							=> @"--no-playlist",

						PlayListOrVidTypes.@default
							=> string.Empty,

						_
							=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<PlayListOrVidTypes>(curVal, @"While selecting a parameter to pass"),
					},
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal switch
					{
						PlayListOrVidTypes.vid
							=> new DictionaryToOpt
							{
								[@"noplaylist"] = true.ToPython(),
							},

						PlayListOrVidTypes.@default or PlayListOrVidTypes.playList or _
							=> [],
					},
		};

		/// <summary>
		/// Download only videos suitable for the given age.  The default value  <see langword="null"/> to prevents this parameter from being emitted.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidSelAgeLimit), @"Download only videos suitable for the given age.  The default value null to prevents this parameter from being emitted.", typeof(VidSelGroup), @"--age-limit")]
		public OneOpt<int?> AgeLimit
		{
			get;

			private init;
		} = new(null, @"--age-limit", @"age_limit");

		/// <summary>
		/// Download only videos not listed in the archive file.  Record the IDs of all downloaded videos in it.  The default value of  <see langword="null"/> prevents this parameter from being emitted.  Use <see cref="fileInvalid"/> to disable the download archive even if it's in the config file.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidSelDownLoadArchive), @"Download only videos not listed in the archive file.  Record the IDs of all downloaded videos in it.  The default value of null prevents this parameter from being emitted.  Use fileInvalid to disable the download archive even if it's in the config file.", typeof(VidSelGroup), @"--download-archive", @"--no-download-archive")]
		public OneOpt<System.IO.FileInfo?> DownLoadArchive
		{
			get;
		} = new(null, string.Empty)
		{
			ParamListGenerator =
				opt
					=> opt.CurVal is null
						? []
						: opt.CurVal == fileInvalid
							? [@"--no-download-archive"]
							: [opt.strParamName, opt.CurVal.FullName],

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is null || opt.CurVal == fileInvalid
						? []
						: new DictionaryToOpt()
						{
							[@"download_archive"] = opt.CurVal.ToPython(),
						}
		};

		/// <summary>
		/// If <see langword="true"/>, stops the download process when encountering a file that is in the archive supplied with <see cref="DownLoadArchive"/>.  If <see langword="false"/>, the download continues.  Use the default value of  <see langword="null"/> to get the value from the config.  That will prevent a parameter from being emitted.  If yt-dlp still doesn't find a value, it acts as though you specified <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidSelBreakOnExisting), @"If true, stops the download process when encountering a file that is in the archive supplied with DownloadArchive.  If false, the download continues.  Use the default value of null to get the value from the config.  That will prevent a parameter from being emitted.  If yt-dlp still doesn't find a value, it acts as though you specified false.", typeof(VidSelGroup),
			@"--break-on-existing", @"--no-break-on-existing")]
		public ThreeWayOpt BreakOnExisting
		{
			get;
		} = new(@"--break-on-existing", @"--no-break-on-existing", @"break_on_existing", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// If <see langword="true"/>, alters <see cref="DownLoadsGroup.MaxDownLoads"/>, <see cref="BreakOnExisting"/>, <see cref="BreakMatchFilters"/>, and auto-number in file templates to reset per input URL.  If <see langword="false"/>, those behave normally.  The default value of <see langword="null"/> causes yt-dlp to check the config files.  If no value is found there, it acts like you used <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidSelBreakPerInput), @"If true, alters Downloads.MaxDownloads, BreakOnExisting, BreakMatchFilters, and auto-number in file templates to reset per input URL.  If false, those behave normally.  The default value of null causes yt-dlp to check the config files.  If no value is found there, it acts like you used false.", typeof(VidSelGroup), @"--break-per-input", @"--no-break-per-input")]
		public ThreeWayOpt BreakPerInput
		{
			get;
		} = new(@"--break-per-input", @"--no-break-per-input", @"break_per_url", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// Specifies the number of allowed failures until the rest of the playlist is skipped.  Use the default value of <see langword="null"/> to have yt-dlp check the config file for this value.  By default, it continues to download the playlist even with errors.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVidSelSkipPlayListAfterErrors), @"Specifies the number of allowed failures until the rest of the playlist is skipped.  Use the default value of null to have yt-dlp check the config file for this value.  By default, it continues to download the playlist even with errors.", typeof(VidSelGroup), @"--skip-playlist-after-errors")]
		public OneOpt<uint?> SkipPlayListAfterErrors
		{
			get;
		} = new(null, @"--skip-playlist-after-errors", @"skip_playlist_after_errors");


		/// <summary>
		/// Returns a list of all options inside <see cref="VidSelGroup"/>.  The parent of this instance will use them to build its own list of options.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				MinFileSize,
				MaxFileSize,
				ByDate,
				Filters,
				PlayListOrVidSel,
				AgeLimit,
				DownLoadArchive,
				BreakOnExisting,
				BreakPerInput,
				SkipPlayListAfterErrors,
			];
	}
}