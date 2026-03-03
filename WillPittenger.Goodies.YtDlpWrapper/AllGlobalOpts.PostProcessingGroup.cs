// Ignore Spelling: Dlp Vals uri Sel wget Xattr Hls Sys Arounds Vid Fmts Pwd Executables Evt Exe Evts Loc Locs Json Dest Hdrs Langs

namespace WillPittenger.Goodies.YtDlpWrapper;

using System.Linq;

using Tools.Ext;

using IReadOnlyDictionaryToOptLists = System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<object>?>;
using IReadOnlyDictionaryToOpt = System.Collections.Generic.IReadOnlyDictionary<string, object?>;
using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;
using System.Security.Cryptography.X509Certificates;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Groups various options related to post-processing.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#post-processing-options">Post-Processing Options section of yt-dlp’s readme.md</a>.
	/// </summary>
	public sealed partial class PostProcessingGroup
	{
		public PostProcessingGroup()
		{
		}

		public PostProcessingGroup(in PostProcessingGroup copyThis)
		{
			ExtractAudioToFmt = new(copyThis.ExtractAudioToFmt);
			RemuxVid = new(copyThis.RemuxVid);
			RecodeVid = new(copyThis.RecodeVid);
			PostProcessorArgs = new(copyThis.PostProcessorArgs);
			KeepVid = new(copyThis.KeepVid);
			OverWrites = new(copyThis.OverWrites);
			Embed = new(copyThis.Embed);
			ParseMetaDataInstruct = new(copyThis.ParseMetaDataInstruct);
			ReplaceInMetaDataInstruct = new(copyThis.ReplaceInMetaDataInstruct);
			WriteMetaDataToXATTR = new(copyThis.WriteMetaDataToXATTR);
			FixUpPolicy = new(copyThis.FixUpPolicy);
			ExecCmd = new(copyThis.ExecCmd);
			ConvertSubs = new(copyThis.ConvertSubs);
			ConvertThumbs = new(copyThis.ConvertThumbs);
			RemoveChapters = new(copyThis.RemoveChapters);
			ForceKeyFramesAtCuts = new(copyThis.ForceKeyFramesAtCuts);
		}


		/// <summary>
		/// Describes how to parse metadata.If either <see cref="strFrom"/> or <see cref="strTo"/> are empty strings, nothing will be emitted.
		/// </summary>
		public sealed class ParseMetaDataInstructOpt : BaseOneOpt<ParseMetaDataInstructOpt>
		{
			/// <summary>
			/// The stage at which replacement will happen.
			/// </summary>
			public readonly string strPrefix;

			public sealed record OneInstruct
			(
				in System.Collections.Generic.IEnumerable<string>? estrFromFields = null,
				in string? strTo = null
			)
			{
				/// <summary>
				/// The literal text to send to yt-dlp before the colon.  Represents the source of data.  Can be a field name or a Python format string.
				/// </summary>
				public System.Collections.Generic.IEnumerable<string> estrFromFields = estrFromFields ?? [];

				/// <summary>
				/// The literal text to send to yt-dlp after the colon.  Represents the destination of the data.  Can be a field name or a Python format string.
				/// </summary>
				public string strTo = strTo ?? string.Empty;
			}

			private readonly System.Collections.Generic.LinkedList<OneInstruct> llistInstructions = [];


			public ParseMetaDataInstructOpt(in ParseMetaDataInstructOpt copyThis)
			{
				PythonParamsGenerator = GetPythonParams;

				strPrefix = copyThis.strPrefix;
			}

			public ParseMetaDataInstructOpt(in string strPrefix)
			{
				PythonParamsGenerator = GetPythonParams;

				this.strPrefix = strPrefix;
			}

			public System.Collections.Generic.IReadOnlyCollection<OneInstruct> Instructions
				=> llistInstructions;

			/// <summary>
			/// The combination of <see cref="strFrom"/> and <see cref="strTo"/> in the format specified by yt-dlp.
			/// </summary>
			public override System.Collections.Generic.IEnumerable<string> Params
				=> llistInstructions.IsEmpty()
					? []
					: (
							from oiCurInstruct in llistInstructions
							select
								new string[]
								{
									@"--replace-in-metadata",
									$"{strPrefix}:{oiCurInstruct.estrFromFields.Join(',')}:{oiCurInstruct.strTo}"
								}
						).SelectMany
						(
							astrCurInstruct
								=> astrCurInstruct
						);

			internal override DictionaryToOpt? PythonParams
				=> PythonParamsGenerator?.Invoke(this);

			private DictionaryToOpt? GetPythonParams(in ParseMetaDataInstructOpt opt)
				=> YtDlpWrapper.yt is not null
					? (llistInstructions.IsEmpty()
						? []
						: new DictionaryToOpt()
							{
								[@"postprocessors"] =
									(
										from oiCurInstruct in llistInstructions
										select new System.Collections.Generic.Dictionary<string, object>()
										{
											[@"key"] = @"MetaDataParser",
											[@"when"] = strPrefix,
											[@"actions"] = new System.Collections.Generic.List<object>()
											{
												new System.Collections.Generic.List<object>()
												{
													new System.Collections.Generic.List<object>()
													{
														YtDlpWrapper.yt.postprocessor.metadataparser.MetadataParserPP.interpretter,
														oiCurInstruct.estrFromFields.Join(','),
														oiCurInstruct.strTo,
													}
												}
											}
										}
									).SelectMany
									(
										dtoCur
											=> dtoCur
									),
							}
						)
					: throw new Exceptions.PythonException(Exceptions.PythonException.Reasons.ytDlpNotReady);


			public void AddFirst(in System.Collections.Generic.IEnumerable<string> estrFromFields, in string strTo)
				=> AddFirst(new(estrFromFields, strTo));

			public void AddFirst(in OneInstruct oiNew)
				=> llistInstructions.AddFirst(oiNew);

			public void AddLast(in System.Collections.Generic.IEnumerable<string> estrFromFields, in string strTo)
				=> AddLast(new(estrFromFields, strTo));

			public void AddLast(in OneInstruct oiNew)
				=> llistInstructions.AddLast(oiNew);

			public void AddBefore(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in System.Collections.Generic.IEnumerable<string> estrFromFields, in string strTo)
				=> AddBefore(llnBeforeThisInstruct, new(estrFromFields, strTo));

			public void AddBefore(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in OneInstruct oiNew)
				=> llistInstructions.AddBefore(llnBeforeThisInstruct, oiNew);

			public void AddAfter(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in System.Collections.Generic.IEnumerable<string> estrFromFields, in string strTo)
				=> AddAfter(llnBeforeThisInstruct, new(estrFromFields, strTo));

			public void AddAfter(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in OneInstruct oiNew)
				=> llistInstructions.AddAfter(llnBeforeThisInstruct, oiNew);

			public void AddRangeFirst(in System.Collections.Generic.IEnumerable<OneInstruct> eoiNew)
				=> llistInstructions.AddRangeFirst(eoiNew);

			public void AddRangeLast(in System.Collections.Generic.IEnumerable<OneInstruct> eoiNew)
				=> llistInstructions.AddRangeLast(eoiNew);

			public void AddRangeBefore(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in System.Collections.Generic.IEnumerable<OneInstruct> eoiNew)
				=> llistInstructions.AddRangeBefore(llnBeforeThisInstruct, eoiNew);

			public void AddRangeAfter(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in System.Collections.Generic.IEnumerable<OneInstruct> eoiNew)
				=> llistInstructions.AddRangeAfter(llnBeforeThisInstruct, eoiNew);

			public bool RemoveInstruct(in OneInstruct oiWhatToRemove)
				=> llistInstructions.Remove(oiWhatToRemove);

			public void RemoveInstruct(in System.Collections.Generic.LinkedListNode<OneInstruct> llnoiWhatToRemove)
				=> llistInstructions.Remove(llnoiWhatToRemove);

			public void RemoveFirst()
				=> llistInstructions.RemoveFirst();

			public void RemoveLast()
				=> llistInstructions.RemoveLast();
		}

		/// <summary>
		/// Describes how to replace metadata.  All fields must be specified.
		/// </summary>
		public sealed class ReplaceInMetaDataInstructOpt : BaseOneOpt<ReplaceInMetaDataInstructOpt>
		{
			/// <summary>
			/// The stage at which replacement will happen.
			/// </summary>
			public readonly string strPrefix;

			[System.ComponentModel.ImmutableObject(true)]
			public sealed record OneInstruct
			(
				in System.Collections.Generic.IEnumerable<string> estrInFields,
				in System.Text.RegularExpressions.Regex regex,
				in string strReplacement
			)
			{
				public readonly System.Collections.Generic.IReadOnlySet<string> setstrInFields = new System.Collections.Generic.HashSet<string>(estrInFields);

				public readonly System.Text.RegularExpressions.Regex regex = regex;

				public readonly string strReplacement = strReplacement;
			}

			private readonly System.Collections.Generic.LinkedList<OneInstruct> llistInstructions = [];


			public ReplaceInMetaDataInstructOpt(in ReplaceInMetaDataInstructOpt copyThis)
			{
				PythonParamsGenerator = GetPythonParams;

				strPrefix = copyThis.strPrefix;
			}

			/// <param name="strPrefix">Specifies the stage at which replacement will happen.  This must be the exact string expected by yt-dlp.  Use a raw string and not something from the RESX.</param>
			public ReplaceInMetaDataInstructOpt(in string strPrefix)
			{
				PythonParamsGenerator = GetPythonParams;

				this.strPrefix = strPrefix;
			}

			public System.Collections.Generic.IReadOnlyCollection<OneInstruct> Instructions
				=> llistInstructions;

			/// <summary>
			/// Generates a list in the format expected by yt-dlp.  Nothing happens if either <see cref="strsetFields"/> is empty, <see cref="regex"/> is <see langword="null"/>, or <see cref="strReplace"/> is empty.
			/// </summary>
			public override System.Collections.Generic.IEnumerable<string> Params
				=> llistInstructions.IsEmpty()
					? []
					: (
							from oiCurInstruct in llistInstructions
							select
								new System.Collections.Generic.List<string>()
								{
									@"--replace-in-metadata",
									$@"{strPrefix}:{oiCurInstruct.setstrInFields.Join(",")}",
									oiCurInstruct.regex.ToString(),
									oiCurInstruct.strReplacement,
								}
						).SelectMany
						(
							list
								=> list
						);

			internal override DictionaryToOpt? PythonParams
				=> PythonParamsGenerator?.Invoke(this);

			private DictionaryToOpt? GetPythonParams(in ReplaceInMetaDataInstructOpt opt)
				=>
					opt.Instructions.IsEmpty()
						? []
						: new DictionaryToOpt()
						{
							[@"postprocessor"] =
									new System.Collections.Generic.List<DictionaryToOpt>()
									{
										new()
										{
											[@"actions"] =
												(
													from oiCurInstruct in llistInstructions
													select new System.Collections.Generic.List<object>()
													{
														YtDlpWrapper.yt?.postprocessor.metadataparser.MetadataParserPP.replacer,
														oiCurInstruct.estrInFields.Join(','),
														oiCurInstruct.regex.ToString(),
														oiCurInstruct.strReplacement,
													}
												).SelectMany
												(
													listItems
														=> listItems
												),
											[@"key"] = @"MetadataParser",
											[@"when"] = opt.strPrefix,
										},
									},
						};

			public void AddFirst(in System.Collections.Generic.IEnumerable<string> estrInFields, in System.Text.RegularExpressions.Regex regex, in string strReplacement)
				=> AddFirst(new(estrInFields, regex, strReplacement));

			public void AddFirst(in OneInstruct oiNew)
				=> llistInstructions.AddFirst(oiNew);

			public void AddLast(in System.Collections.Generic.IEnumerable<string> estrInFields, in System.Text.RegularExpressions.Regex regex, in string strReplacement)
				=> AddLast(new(estrInFields, regex, strReplacement));

			public void AddLast(in OneInstruct oiNew)
				=> llistInstructions.AddLast(oiNew);

			public void AddBefore(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in System.Collections.Generic.IEnumerable<string> estrInFields, in System.Text.RegularExpressions.Regex regex, in string strReplacement)
				=> AddBefore(llnBeforeThisInstruct, new(estrInFields, regex, strReplacement));

			public void AddBefore(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in OneInstruct oiNew)
				=> llistInstructions.AddBefore(llnBeforeThisInstruct, oiNew);

			public void AddAfter(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in System.Collections.Generic.IEnumerable<string> estrInFields, in System.Text.RegularExpressions.Regex regex, in string strReplacements)
				=> AddAfter(llnBeforeThisInstruct, new(estrInFields, regex, strReplacements));

			public void AddAfter(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in OneInstruct oiNew)
				=> llistInstructions.AddAfter(llnBeforeThisInstruct, oiNew);

			public void AddRangeFirst(in System.Collections.Generic.IEnumerable<OneInstruct> eoiNew)
				=> llistInstructions.AddRangeFirst(eoiNew);

			public void AddRangeLast(in System.Collections.Generic.IEnumerable<OneInstruct> eoiNew)
				=> llistInstructions.AddRangeLast(eoiNew);

			public void AddRangeBefore(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in System.Collections.Generic.IEnumerable<OneInstruct> eoiNew)
				=> llistInstructions.AddRangeBefore(llnBeforeThisInstruct, eoiNew);

			public void AddRangeAfter(in System.Collections.Generic.LinkedListNode<OneInstruct> llnBeforeThisInstruct, in System.Collections.Generic.IEnumerable<OneInstruct> eoiNew)
				=> llistInstructions.AddRangeAfter(llnBeforeThisInstruct, eoiNew);

			public bool RemoveInstruct(in OneInstruct oiWhatToRemove)
				=> llistInstructions.Remove(oiWhatToRemove);

			public void RemoveInstruct(in System.Collections.Generic.LinkedListNode<OneInstruct> llnoiWhatToRemove)
				=> llistInstructions.Remove(llnoiWhatToRemove);

			public void RemoveFirst()
				=> llistInstructions.RemoveFirst();

			public void RemoveLast()
				=> llistInstructions.RemoveLast();
		}

		public sealed class AudioExtractionOpt : BaseOneOpt<AudioExtractionOpt>
		{
			public AudioExtractionOpt()
			{
				PythonParamsGenerator = GetPythonParams;

				PreferredFmt = string.Empty;
				PreferredQuality = null;
			}

			public AudioExtractionOpt(in string? strPreferredFmt = null, in string? strPreferredQuality = null)
			{
				PythonParamsGenerator = GetPythonParams;

				PreferredFmt = strPreferredFmt ?? string.Empty;
				PreferredQuality = strPreferredQuality;
			}

			public AudioExtractionOpt(in byte bytePreferredQuality, in string? strPreferredFmt = null)
			{
				PythonParamsGenerator = GetPythonParams;

				PreferredFmt = strPreferredFmt ?? string.Empty;
				PreferredQaulityAsByte = bytePreferredQuality;
			}

			public AudioExtractionOpt(in AudioExtractionOpt copyThis)
			{
				PythonParamsGenerator = GetPythonParams;

				PreferredFmt = copyThis.PreferredFmt;
				PreferredQuality = copyThis.PreferredQuality;
			}


			public string PreferredFmt
			{
				get;

				set;
			}

			public object? PreferredQuality
			{
				get;

				private set;
			}

			public string PreferredQaulityAsStr
			{
				set
					=> PreferredQuality = value;
			}

			public byte PreferredQaulityAsByte
			{
				set
				{
					Tools.Exceptions.AssertOrThrow.TestIt(value <= 10, () => throw new System.IndexOutOfRangeException(@"Integer audio qualities are limited to 0 through 10."));

					PreferredQuality = value;
				}
			}

			public override System.Collections.Generic.IEnumerable<string>? Params
				=> PreferredFmt == string.Empty || PreferredQuality is null
					? []
					: new System.Collections.Generic.List<string>()
						{
							@"--extract-audio",
							@"--audio-format",
							PreferredFmt,
							PreferredQuality.ToString() ?? throw new System.InvalidProgramException(@"ToString on the quality returned null"),
						};

			internal override DictionaryToOpt? PythonParams
				=> PythonParamsGenerator?.Invoke(this);

			private DictionaryToOpt? GetPythonParams(in AudioExtractionOpt opt)
				=> opt.PreferredFmt == string.Empty || opt.PreferredQuality == null
					? []
					: new DictionaryToOpt()
					{
						[@"format"] = @"bestaudio/best",
						[@"postprocessors"] = new System.Collections.Generic.List<DictionaryToOpt>()
							{
								new()
								{
									[@"key"] = @"FFmpegExtractAudio",
									[@"nopostoverwrites"] = false,
									[@"preferredcodec"] = opt.PreferredFmt,
									[@"preferredquality"] = opt.PreferredQuality,
								},
							},
					};
		}

		public sealed class FmtConversionOpt<FmtListType>
			: BaseOneOpt<FmtConversionOpt<FmtListType>>
			where FmtListType : class, FmtConversionOpt<FmtListType>.IFmtListType
		{
			internal FmtConversionOpt(in string strCmdLineOp, in string strPythonOpKey)
			{
				PythonParamsGenerator = GetPythonParams;

				this.strCmdLineOp = strCmdLineOp;
				this.strPythonOpKey = strPythonOpKey;
			}

			internal FmtConversionOpt(in FmtConversionOpt<FmtListType> copyThis)
			{
				PythonParamsGenerator = GetPythonParams;

				strCmdLineOp = copyThis.strCmdLineOp;
				strPythonOpKey = copyThis.strPythonOpKey;
			}


			public interface IFmtListType
			{
				string Name
				{
					get;
				}
			}

			public class Val
			{
				public Val()
				{
				}

				public Val(in FmtListType defDestFmt)
					=> DefDestFmt = defDestFmt;

				public Val(in System.Collections.Generic.OrderedDictionary<FmtListType, FmtListType> specifiedConversions, in FmtListType? defDestFmt = null)
				{
					SpecifiedConversions = specifiedConversions;
					DefDestFmt = defDestFmt;
				}


				public System.Collections.Generic.OrderedDictionary<FmtListType, FmtListType> SpecifiedConversions
				{
					get;

					set
					{
						Tools.Exceptions.AssertOrThrow.TestIt(SpecifiedConversions.Keys.All(curKeyFmt => curKeyFmt != value[curKeyFmt]), () => throw new System.InvalidOperationException(@"An entry in the list would result in a no operation"));

						field = value;
					}
				} = [];

				public FmtListType? DefDestFmt
				{
					get;

					set;
				}
			}


			public readonly string strCmdLineOp;
			public readonly string strPythonOpKey;


			public Val CurVal
			{
				get;

				set;
			} = new();

			private string FmtStr
				=> CurVal.SpecifiedConversions.Count == 0 && CurVal.DefDestFmt is not null
					? CurVal.DefDestFmt.Name
					: CurVal.SpecifiedConversions.Count > 0 && CurVal.DefDestFmt is null
						? CurVal.SpecifiedConversions.Select(
								kvpCur
									=> $@"{kvpCur.Key}>{kvpCur.Value}"
							).Join("/")
						: CurVal.SpecifiedConversions.Count > 0 && CurVal.DefDestFmt is not null
							? $@"{CurVal.SpecifiedConversions.Select(
								kvpCur
									=> $@"{kvpCur.Key}>{kvpCur.Value}"
							).Join("/")}/{CurVal.DefDestFmt}"
							: throw new System.InvalidProgramException(@"Somehow despite all our earlier checks, we got an invalid specification");

			public override System.Collections.Generic.IEnumerable<string>? Params
				=> CurVal.SpecifiedConversions.Count == 0 && CurVal.DefDestFmt is null
					? []
					: new System.Collections.Generic.List<string>()
						{
							@"--remux-video",
							FmtStr,
						};

			internal override DictionaryToOpt? PythonParams
				=> PythonParamsGenerator?.Invoke(this);


			private DictionaryToOpt? GetPythonParams(in FmtConversionOpt<FmtListType> opt)
				=> opt.CurVal.SpecifiedConversions.Count == 0 && opt.CurVal.DefDestFmt is null
					? []
					: new DictionaryToOpt()
					{
						[@"postprocessors"] = new System.Collections.Generic.List<DictionaryToOpt>()
									{
										new()
										{
											[@"key"] = strPythonOpKey,
											[@"preferedformat"] = FmtStr,
										}
									},
					};
		}

		[System.ComponentModel.ImmutableObject(true)]
		public sealed class SupportedVidFmts : FmtConversionOpt<SupportedVidFmts>.IFmtListType
		{
			private SupportedVidFmts(in string strName)
				=> this.strName = strName;


			public readonly string strName;


			public static readonly SupportedVidFmts avi = new(@"avi");

			public static readonly SupportedVidFmts flv = new(@"flv");

			public static readonly SupportedVidFmts gif = new(@"gif");

			public static readonly SupportedVidFmts mkv = new(@"mkv");

			public static readonly SupportedVidFmts mov = new(@"mov");

			public static readonly SupportedVidFmts mp4 = new(@"mp4");

			public static readonly SupportedVidFmts webm = new(@"webm");

			public static readonly SupportedVidFmts aac = new(@"aac");

			public static readonly SupportedVidFmts aiff = new(@"aiff");

			public static readonly SupportedVidFmts alac = new(@"alac");

			public static readonly SupportedVidFmts flac = new(@"flac");

			public static readonly SupportedVidFmts m4a = new(@"m4a");

			public static readonly SupportedVidFmts mka = new(@"mka");

			public static readonly SupportedVidFmts mp3 = new(@"mk3");

			public static readonly SupportedVidFmts ogg = new(@"ogg");

			public static readonly SupportedVidFmts opus = new(@"opus");

			public static readonly SupportedVidFmts vorbis = new(@"vorbis");

			public static readonly SupportedVidFmts wav = new(@"wav");


			public string Name
				=> strName;


			public static explicit operator string(in SupportedVidFmts fmtToConvert)
				=> fmtToConvert.strName;
		}

		[System.ComponentModel.ImmutableObject(true)]
		public sealed class SupportedSubFmts
		{
			private SupportedSubFmts(in string strName)
				=> this.strName = strName;


			public readonly string strName;

			public static readonly SupportedSubFmts ass = new(@"ass");

			public static readonly SupportedSubFmts lrc = new(@"lrc");

			public static readonly SupportedSubFmts srt = new(@"srt");

			public static readonly SupportedSubFmts vtt = new(@"vtt");


			public string Name
				=> strName;


			public static explicit operator string(in SupportedSubFmts fmtToConvert)
				=> fmtToConvert.strName;
		}

		[System.ComponentModel.ImmutableObject(true)]
		public sealed class SupportedThumbFmts
			: FmtConversionOpt<SupportedThumbFmts>.IFmtListType
		{
			private SupportedThumbFmts(in string strName)
				=> this.strName = strName;


			public readonly string strName;

			public static readonly SupportedThumbFmts jpg = new(@"jpg");

			public static readonly SupportedThumbFmts jpeg = jpg;

			public static readonly SupportedThumbFmts png = new(@"png");

			public static readonly SupportedThumbFmts webp = new(@"webp");


			public string Name
				=> strName;


			public static explicit operator string(in SupportedThumbFmts fmtToConvert)
				=> fmtToConvert.strName;
		}

		/// <summary>
		/// Describes a value for a post-processor.  You must provide a name.  If you don’t provide a name, nothing will be emitted.  The arguments though can be an empty string.
		/// </summary>
		/// <param name="strName">The parameter name</param>
		/// <param name="strArgs">The list of arguments</param>
		public sealed class PostProcessorArgsVal(in string strName, in string? strArgs = null) : IOneOptVal
		{
			/// <summary>
			/// The parameter name
			/// </summary>
			public string strName = strName;

			/// <summary>
			/// The arguments to send
			/// </summary>
			public string strArgs = strArgs ?? string.Empty;


			/// <summary>
			/// The name and arguments in the format expected by yt-dlp
			/// </summary>
			public string? ValText
				=> strName == string.Empty
					? null
					: strArgs == string.Empty
						? strName
						: $"{strName}:{strArgs}";
		}


		/// <summary>
		/// Provides a series of options for how to fix problems.  All allowed values are <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members.
		/// </summary>
		public sealed class FixUpPolicyChoices : IOneOptVal
		{
			/// <summary>
			/// Constructs a new instance.  This is the only way to make a new <see cref="FixUpPolicyChoices"/> by design.
			/// </summary>
			/// <param name="strText">The exact text that yt-dlp is looking for.  Use a raw string and not something from the RESX.</param>
			private FixUpPolicyChoices(in string strText)
				=> this.strText = strText;


			/// <summary>
			/// The exact text that yt-dlp is looking for.
			/// </summary>
			public readonly string strText;


			/// <summary>
			/// Causes yt-dlp to use the value from the config file if a config file specifies a value.  Otherwise, yt-dlp will act as though you used <see cref="detectOrWarn"/>.
			/// </summary>
			public static readonly FixUpPolicyChoices @default = new(string.Empty);

			/// <summary>
			/// Never fix problems
			/// </summary>
			public static readonly FixUpPolicyChoices never = new(@"never");

			/// <summary>
			/// Only warn about issues
			/// </summary>
			public static readonly FixUpPolicyChoices warn = new(@"warn");

			/// <summary>
			/// Attempts to fix issues if possible.  Otherwise, this option will act like <see cref="warn"/>.
			/// </summary>
			public static readonly FixUpPolicyChoices detectOrWarn = new(@"detect_or_warn");

			/// <summary>
			/// Force fixes even if the file exists.
			/// </summary>
			public static readonly FixUpPolicyChoices force = new(@"force");


			public string ValText
				=> strText;
		}


		/// <summary>
		/// Provides choices for what stage of post-processing a set of instructions should be used  The only allowed values are all <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members.
		/// </summary>
		public sealed class WhenValsForPostProcessing : IOneOptVal
		{
			/// <summary>
			/// Constructs a new instance.  This is the only constructor by design.
			/// </summary>
			/// <param name="strText">The exact text that yt-dlp is looking for.  Use a raw string and not a value from the RESX.</param>
			private WhenValsForPostProcessing(in string strText)
				=> this.strText = strText;


			/// <summary>
			/// The exact text that yt-dlp is looking for.
			/// </summary>
			public readonly string strText;


			/// <summary>
			/// Specifies the pre-processing stage
			/// </summary>
			public static readonly WhenValsForPostProcessing preProcess = new(@"pre_process");

			/// <summary>
			/// After the item has passed the filters applied
			/// </summary>
			public static readonly WhenValsForPostProcessing afterFilter = new(@"after_filter");

			/// <summary>
			/// The video format has been chosen, but print and output instructions haven’t been run.
			/// </summary>
			public static readonly WhenValsForPostProcessing vid = new(@"video");

			/// <summary>
			/// Just before downloading starts
			/// </summary>
			public static readonly WhenValsForPostProcessing beforeDownLoad = new(@"before_dl");

			/// <summary>
			/// Post-processing is starting
			/// </summary>
			public static readonly WhenValsForPostProcessing postProcess = new(@"post_process");

			/// <summary>
			/// The final file has been moved to its final location
			/// </summary>
			public static readonly WhenValsForPostProcessing afterMove = new(@"after_move");

			/// <summary>
			/// The video is completely processed
			/// </summary>
			public static readonly WhenValsForPostProcessing afterVid = new(@"after_video");

			/// <summary>
			/// The playlist is complete
			/// </summary>
			public static readonly WhenValsForPostProcessing playList = new(@"playlist");


			/// <summary>
			/// Perform the step at all stages
			/// </summary>
			public static readonly WhenValsForPostProcessing all = new(string.Empty);


			public string ValText
				=> strText;
		}

		/// <summary>
		/// Provides choices for what post-processing event a set of instructions should be used  The only allowed values are all <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members.
		/// </summary>
		public sealed class PreDefinedProcessorEvts : IOneOptVal
		{
			/// <summary>
			/// Constructs a new instance.  This is the only constructor by design.
			/// </summary>
			/// <param name="strTextName">The exact text that yt-dlp is looking for.  Use a raw string and not a value from the RESX file.</param>
			private PreDefinedProcessorEvts(in string strTextName)
				=> this.strTextName = strTextName;


			/// <summary>
			/// The exact text that yt-dlp is looking for
			/// </summary>
			public readonly string strTextName;


			/// <summary>
			/// Run the instructions while merging files
			/// </summary>
			public static readonly PreDefinedProcessorEvts merger = new(@"Merger");

			/// <summary>
			/// Run the instructions while modifying chapters
			/// </summary>
			public static readonly PreDefinedProcessorEvts modChapters = new(@"ModifyChapters");

			/// <summary>
			/// Run the instructions while splitting chapters
			/// </summary>
			public static readonly PreDefinedProcessorEvts splitChapters = new(@"SplitChapters");

			/// <summary>
			/// Run the instructions while extracting audio
			/// </summary>
			public static readonly PreDefinedProcessorEvts extractAudio = new(@"ExtractAudio");

			/// <summary>
			/// Run the instructions while remuxing videos
			/// </summary>
			public static readonly PreDefinedProcessorEvts vidRemuxer = new(@"VideoRemuxer");

			/// <summary>
			/// Run the instructions while converting videos
			/// </summary>
			public static readonly PreDefinedProcessorEvts vidConverter = new(@"VideoConvertor");

			/// <summary>
			/// Run the instructions while processing metadata
			/// </summary>
			public static readonly PreDefinedProcessorEvts metaData = new(@"Metadata");

			/// <summary>
			/// Run the instructions while embedding subtitles
			/// </summary>
			public static readonly PreDefinedProcessorEvts embedSubtitle = new(@"EmbedSubtitle");

			/// <summary>
			/// Run the instructions while embedding thumbnails
			/// </summary>
			public static readonly PreDefinedProcessorEvts embedThumb = new(@"EmbedThumbnail");

			/// <summary>
			/// Run the instructions while converting subtitles
			/// </summary>
			public static readonly PreDefinedProcessorEvts subtitlesConverter = new(@"SubtitlesConveror");

			/// <summary>
			/// Run the instructions while converting thumbnails
			/// </summary>
			public static readonly PreDefinedProcessorEvts thumbConverter = new(@"ThumbnailConvertor");

			/// <summary>
			/// Run the instructions while fixing up stretched
			/// </summary>
			public static readonly PreDefinedProcessorEvts fixUpStretched = new(@"FixupStretched");

			/// <summary>
			/// Run the instructions while fixing up m4a files
			/// </summary>
			public static readonly PreDefinedProcessorEvts fixUpM4A = new(@"FixupM4a");

			/// <summary>
			/// Run the instructions while fixing up m3u8 files
			/// </summary>
			public static readonly PreDefinedProcessorEvts fixUpM3U8 = new(@"FixupM3u8");

			/// <summary>
			/// Run the instructions while fixing up time stamps
			/// </summary>
			public static readonly PreDefinedProcessorEvts fixUpTimeStamp = new(@"FixupTimestamp");

			/// <summary>
			/// Run the instructions while fixing durations
			/// </summary>
			public static readonly PreDefinedProcessorEvts fixUpDuration = new(@"FixupDuration");


			public string ValText
				=> strTextName;
		}

		/// <summary>
		/// Provides choices for what stage of post-processing a set of instructions should be used  The only allowed values are all <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members if either <see cref="SupportedProcessorExecutables"/> or <see cref="SupportedFfmpegProcessorExecutables"/>.
		/// </summary>
		public class SupportedProcessorExecutables : IOneOptVal
		{
			/// <summary>
			/// Constructs a new instance.  This is the only constructor by design.
			/// </summary>
			/// <param name="strName">The exact text expected by yt-dlp.  Use a raw string and not something from the RESX.</param>
			protected internal SupportedProcessorExecutables(in string strName)
				=> this.strName = strName;


			/// <summary>
			/// The exact text expected by yt-dlp
			/// </summary>
			public readonly string strName;


			/// <summary>
			/// Use Atomic Parsley.
			/// </summary>
			public static readonly SupportedProcessorExecutables atomicParsley = new("AtomicParsley");


			public string ValText
				=> strName;


			public static implicit operator string(in SupportedProcessorExecutables speConvertThis)
				=> speConvertThis.strName;
		}

		/// <summary>
		/// Lists additional options, but specifying an FFMPEG executable along with a stream.  You can use <see cref="GetFfmpegForInputStream(in uint)"/>, <see cref="GetFfmpegForOutputStream(in uint)"/>, <see cref="GetFfprobeForInputStream(in uint)"/>, or <see cref="GetFfprobeForOutputStream(in uint)"/> to get a value for a specific stream.
		/// </summary>
		public sealed class SupportedFfmpegProcessorExecutables : SupportedProcessorExecutables
		{
			/// <summary>
			/// Constructs a new instance.  This is the only constructor by design.
			/// </summary>
			/// <param name="strName">The exact text expected by yt-dlp.  Use a raw string and not something from the RESX file.</param>
			private SupportedFfmpegProcessorExecutables(in string strName) : base(strName)
			{
			}


			/// <summary>
			/// Specifies the ffmpeg executable, but no stream
			/// </summary>
			public static readonly SupportedFfmpegProcessorExecutables ffmpeg = new(@"FFmpeg");

			/// <summary>
			/// Specifies the ffprobe executable, but no stream
			/// </summary>
			public static readonly SupportedFfmpegProcessorExecutables ffprobe = new(@"FFprobe");



			/// <summary>
			/// Constructs a new <see cref="SupportedFfmpegProcessorExecutables"/> instance for the specified input stream and ffmpeg.
			/// </summary>
			/// <param name="uiStreamID">The input stream identifier</param>
			/// <returns>A <see cref="SupportedFfmpegProcessorExecutables"/> instance for ffmpeg and the specified input stream</returns>
			public static SupportedFfmpegProcessorExecutables GetFfmpegForInputStream(in uint uiStreamID)
				=> new($"{ffmpeg.strName}_i{uiStreamID}");

			/// <summary>
			/// Constructs a new <see cref="SupportedFfmpegProcessorExecutables"/> instance for the specified output stream and ffmpeg.
			/// </summary>
			/// <param name="uiStreamID">The output stream identifier</param>
			/// <returns>A <see cref="SupportedFfmpegProcessorExecutables"/> instance for ffmpeg and the specified output stream</returns>
			public static SupportedFfmpegProcessorExecutables GetFfmpegForOutputStream(in uint uiStreamID)
				=> new($"{ffmpeg.strName}_o{uiStreamID}");


			/// <summary>
			/// Constructs a new <see cref="SupportedFfmpegProcessorExecutables"/> instance for the specified input stream and ffprobe.
			/// </summary>
			/// <param name="uiStreamID">The input stream identifier</param>
			/// <returns>A <see cref="SupportedFfmpegProcessorExecutables"/> instance for ffprobe and the specified input stream</returns>
			public static SupportedFfmpegProcessorExecutables GetFfprobeForInputStream(in uint uiStreamID)
				=> new($"{ffprobe.strName}_i{uiStreamID}");

			/// <summary>
			/// Constructs a new <see cref="SupportedFfmpegProcessorExecutables"/> instance for the specified output stream and ffprobe.
			/// </summary>
			/// <param name="uiStreamID">The output stream identifier</param>
			/// <returns>A <see cref="SupportedFfmpegProcessorExecutables"/> instance for ffprobe and the specified output stream</returns>
			public static SupportedFfmpegProcessorExecutables GetFfprobeForOutputStream(in uint uiStreamID)
				=> new($"{ffprobe.strName}_o{uiStreamID}");


			public static implicit operator string(in SupportedFfmpegProcessorExecutables sfpeConvertThis)
				=> sfpeConvertThis.strName;
		}

		/// <summary>
		/// Abstract representation of instructions to send to a post-processor.  Use <see cref="PostProcessingRecordForKnownProcessorVal"/> for post-processors that yt-dlp knows about.  Use <see cref="PostProcessorArgsForExeVal"/> for all other post-processors.
		/// </summary>
		/// <param name="Args">The arguments to send</param>
		/// <param name="When">When to trigger the processor.  Use a value from <see cref="WhenValsForPostProcessing"/>.  You can pass <see langword="null"/> here.  That’s treated as <see cref="WhenValsForPostProcessing.all"/>.</param>
		public abstract record BasePostProcessorArgsVal(in System.Collections.Generic.IReadOnlyDictionary<string, string>? Args = null, in
			WhenValsForPostProcessing? When = null) : IOneOptVal
		{
			/// <summary>
			/// If non-<see langword="null"/>, specifies a list of arguments to send.  The key in the dictionary is the parameter name.  The value for each key is the parameter value.
			/// </summary>
			public System.Collections.Generic.IReadOnlyDictionary<string, string>? Args
			{
				get;

				set;
			} = Args;

			/// <summary>
			/// Specifies when to run the processor with these arguments
			/// </summary>
			public WhenValsForPostProcessing When
			{
				get;

				set;
			} = When ?? WhenValsForPostProcessing.all;

			/// <summary>
			/// Returns a list of the arguments combined into a single string.  Mainly used by <see cref="ValText"/> so we don't have to call it several times.
			/// </summary>
			public string? CombinedArgs
				=> Args == null || Args.Count == 0
					? null
					: Args.Select(
							kvCur
								=> $@"{kvCur.Key}={kvCur.Value}"
						).Join(';');


			/// <summary>
			/// Constructs the string in the syntax that yt-dlp expects.  Cases exist for if any property is defaulted.
			/// </summary>
			public virtual string? ValText
				=> (Args == null || Args.Count == 0) && When == WhenValsForPostProcessing.all
					? null
					: (Args == null || Args.Count == 0) && When != WhenValsForPostProcessing.all
						? $"when={When.strText}"
						: When == WhenValsForPostProcessing.all
							? CombinedArgs
							: $"when={When.strText}:{CombinedArgs}";
		}

		/// <summary>
		/// Describes a post-processing record for any of the post-processors listed in <see cref="SupportedProcessorExecutables"/> or <see cref="SupportedFfmpegProcessorExecutables"/>
		/// </summary>
		/// <param name="WhichEvt">Which event to run the post processor on.  <paramref name="WhichEvt"/> lets you specify arguments for when a post-processor is being run at the selected stage.  This must be a value from <see cref="PreDefinedProcessorEvts"/>.</param>
		/// <param name="WhichProcessorExecutable">Which post-processor to send the instructions to.  This must be a predefined value from <see cref="SupportedProcessorExecutables"/> <see cref="SupportedFfmpegProcessorExecutables"/> unless you get a value from <see cref="SupportedFfmpegProcessorExecutables.GetFfmpegForInputStream(in uint)"/>, <see cref="SupportedFfmpegProcessorExecutables.GetFfmpegForOutputStream(in uint)"/>, <see cref="SupportedFfmpegProcessorExecutables.GetFfprobeForInputStream(in uint)"/>, or <see cref="SupportedFfmpegProcessorExecutables.GetFfprobeForOutputStream(in uint)"/>.</param>
		/// <param name="Args">The arguments to send</param>
		/// <param name="When">When to trigger the processor.  Use a value from <see cref="WhenValsForPostProcessing"/>.  You can pass <see langword="null"/> here.  That’s treated as <see cref="WhenValsForPostProcessing.all"/>.</param>
		public sealed record PostProcessingRecordForKnownProcessorVal
		(
			in PreDefinedProcessorEvts? WhichEvt = null,
			in SupportedProcessorExecutables? WhichProcessorExecutable = null,
			in System.Collections.Generic.IReadOnlyDictionary<string, string>? Args = null,
			in WhenValsForPostProcessing? When = null
		) : BasePostProcessorArgsVal(Args, When)
		{
			/// <summary>
			/// Which event to run the post processor on.  <see cref="WhichEvt"/> lets you specify arguments for when a post-processor is being run at the selected stage.  This must be a value from <see cref="PreDefinedProcessorEvts"/>.
			/// </summary>
			public PreDefinedProcessorEvts? WhichEvt
			{
				get;

				set;
			} = WhichEvt;

			/// <summary>
			/// Which post-processor to send the instructions to.  This must be a predefined value from <see cref="SupportedProcessorExecutables"/> <see cref="SupportedFfmpegProcessorExecutables"/> unless you get a value from <see cref="SupportedFfmpegProcessorExecutables.GetFfmpegForInputStream(in uint)"/>, <see cref="SupportedFfmpegProcessorExecutables.GetFfmpegForOutputStream(in uint)"/>, <see cref="SupportedFfmpegProcessorExecutables.GetFfprobeForInputStream(in uint)"/>, or <see cref="SupportedFfmpegProcessorExecutables.GetFfprobeForOutputStream(in uint)"/>.
			/// </summary>
			public SupportedProcessorExecutables? WhichProcessorExecutable
			{
				get;

				set;
			} = WhichProcessorExecutable;


			/// <summary>
			/// Constructs the value text using the syntax that yt-dlp expects.  Cases exist for several possibilities.
			/// </summary>
			public override string? ValText
				=> WhichEvt == null && WhichProcessorExecutable == null
					? null
					: WhichEvt != null && WhichProcessorExecutable == null
						? $@"{WhichEvt.strTextName}:{base.ValText}"
						: WhichEvt == null && WhichProcessorExecutable != null
							? $@"{WhichProcessorExecutable.strName}:{base.ValText}"
							: $@"{WhichEvt}+{WhichProcessorExecutable}:{base.ValText}";


			/// <summary>
			/// Constructs a <see cref="PostProcessingRecordForKnownProcessorVal"/> instance from a <see cref="PreDefinedProcessorEvts"/> value.
			/// </summary>
			/// <param name="whichEvt">The value to wrap</param>
			public static implicit operator PostProcessingRecordForKnownProcessorVal(in PreDefinedProcessorEvts whichEvt)
				=> new(whichEvt);

			/// <summary>
			/// Constructs an instance of <see cref="PostProcessingRecordForKnownProcessorVal"/> for a <see cref="SupportedProcessorExecutables"/> or <see
			/// cref="SupportedFfmpegProcessorExecutables"/> value
			/// </summary>
			/// <param name="whichProcessorExecutable"></param>
			public static implicit operator PostProcessingRecordForKnownProcessorVal(in SupportedProcessorExecutables whichProcessorExecutable)
				=> new(whichProcessorExecutable);
		}

		public abstract record BasePostProcessorArgsForExeVal
		(
			in System.Collections.Generic.IReadOnlyDictionary<string, string>? Args = null,
			in WhenValsForPostProcessing? When = null
		) : BasePostProcessorArgsVal(Args, When)
		{
			public abstract string FullExePath
			{
				get;
			}


			/// <summary>
			/// The value text in the syntax expected by yt-dlp
			/// </summary>
			public override string? ValText
				=> (Args == null || Args.Count == 0) && When == WhenValsForPostProcessing.all
					? FullExePath
					: (Args == null || Args.Count == 0) && When != WhenValsForPostProcessing.all
						? $@"{FullExePath}:when={When.strText}"
						: Args.IsEmpty() && When != WhenValsForPostProcessing.all
							? $@"{FullExePath}:when={When.strText};{base.CombinedArgs}"
							: $@"{FullExePath}:{CombinedArgs}";
		}

		/// <summary>
		/// Describes a post-processing record for any executable that yt-dlp does NOT know about.  You specify the executable with a <see cref="System.IO.FileInfo"/> instance.  Note: Always use the full path with <see cref="PostProcessorArgsForExeVal"/> instances.  If you’re specifying an executable from your system path, use <see cref="NamedExeProcessorArgsVal"/> instead.  This is because <see cref="ValText"/> uses <see cref="System.IO.FileSystemInfo.FullName"/> which assumes the path is relative to the current working directory.
		/// </summary>
		/// <param name="fileExe">A <see cref="System.IO.FileInfo"/> with the full path of the file</param>
		/// <param name="Args">The arguments to send</param>
		/// <param name="When">When to trigger the processor.  Use a value from <see cref="WhenValsForPostProcessing"/>.  You can pass <see langword="null"/> here.  That’s treated as <see cref="WhenValsForPostProcessing.all"/>.</param>
		public sealed record PostProcessorArgsForExeVal
		(
			in System.IO.FileInfo fileExe,
			in System.Collections.Generic.IReadOnlyDictionary<string, string>? Args = null,
			in WhenValsForPostProcessing? When = null
		) : BasePostProcessorArgsForExeVal(Args, When)
		{
			/// <summary>
			/// A <see cref="System.IO.FileInfo"/> with the full path of the file
			/// </summary>
			public System.IO.FileInfo fileExe = fileExe;


			public override string FullExePath
				=> fileExe.FullName;


			/// <summary>
			/// Constructs a new <see cref="PostProcessorArgsForExeVal"/> instance for the given <see cref="System.IO.FileInfo"/>
			/// </summary>
			/// <param name="fileExe">>A <see cref="System.IO.FileInfo"/> with the full path of the file.  Do not use a <see cref="System.IO.FileInfo"/> with only a file name as that will result in the wrong directory.</param>
			public static implicit operator PostProcessorArgsForExeVal(in System.IO.FileInfo fileExe)
				=> new(fileExe);
		}

		/// <summary>
		/// Describes a post-processing record for any executable that yt-dlp doesn’t know about which can be found with the system path
		/// </summary>
		/// <param name="strExeFileNameWithOutPath">The file name of the executable, but only the file name.  Omit the path.</param>
		/// <param name="Args"></param>
		/// <param name="When">When to trigger the processor.  Use a value from <see cref="WhenValsForPostProcessing"/>.  You can pass <see langword="null"/> here.  That’s treated as <see cref="WhenValsForPostProcessing.all"/>.</param>
		public sealed record NamedExeProcessorArgsVal
		(
			in string strExeFileNameWithOutPath,
			in System.Collections.Generic.IReadOnlyDictionary<string, string>? Args = null,
			in WhenValsForPostProcessing? When = null
		) : BasePostProcessorArgsForExeVal(Args, When)
		{
			/// <summary>
			/// The name of the file to use
			/// </summary>
			public string strExeFileNameWithOutPath = strExeFileNameWithOutPath;


			public override string FullExePath
				=> strExeFileNameWithOutPath;


			/// <summary>
			/// Constructs a new <see cref="NamedExeProcessorArgsVal"/> from a <see cref="string"/> containing the name of the executable.
			/// </summary>
			/// <param name="strExeFileNameWithOutPath">The executable’s file name</param>
			public static implicit operator NamedExeProcessorArgsVal(in string strExeFileNameWithOutPath)
				=> new(strExeFileNameWithOutPath);
		}


		/// <summary>
		/// Groups items related to embedding items.
		/// </summary>
		public sealed class EmbedGroup
		{
			public EmbedGroup()
			{
			}

			public EmbedGroup(in EmbedGroup copyThis)
			{
				Subs = copyThis.Subs;
				Thumb = copyThis.Thumb;
				MetaData = copyThis.MetaData;
				Chapters = copyThis.Chapters;
				InfoJSON = copyThis.InfoJSON;
			}


			/// <summary>
			/// If <see langword="true"/>, yt-dlp will embed subtitle information into the video file.  If <see langword="false"/>, it won’t.  Ignored if the final video file isn’t mp4, webm, or mkv.  If you leave <see cref="Subs"/> set to the default value of <see langword="null"/> and no config file specifies either --embed-subs or --no-embed-subs, yt-dlp will act as though <see cref="Subs"/> was set to <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingEmbedSubs), @"If true, yt-dlp will embed subtitle information into the video file.  If false, it won’t.  Ignored if the final video file isn’t mp4, webm, or mkv.  If you leave Subs set to the default value of null and no config file specifies either --embed-subs or --no-embed-subs, yt-dlp will act as though Subs was set to false.", typeof(PostProcessingGroup), @"--embed-subs", @"--no-embed-subs")]
			public ThreeWayOpt Subs
			{
				get;
			} = new(@"--embed-subs", @"--no-embed-subs")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.Val == true
							? new DictionaryToOpt()
							{
								[@"postprocessors"] = new DictionaryToOpt()
								{
									[@"already_have_subtitles"] = false,
									[@"key"] = @"FFmpegEmbedSubtitle",
								},
								[@"writesubtitles"] = true,
							}
							: [],
			};

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will embed a thumbnail into the post-processed file as cover art.  If <see langword="false"/>, it won’t do that.  If you leave <see cref="Thumb"/> set to the default value of <see langword="null"/> and no config file specifies either --embed-thumbnail or --no-embed-thumbnail, yt-dlp will act as though <see cref="Thumb"/> were set to <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingEmbedThumb), @"If true, yt-dlp will embed a thumbnail into the post-processed file as cover art.  If false, it won’t do that.  If you leave Thumb set to the default value of null and no config file specifies either --embed-thumbnail or --no-embed-thumbnail, yt-dlp will act as though Thumb were set to false.", typeof(PostProcessingGroup), @"--embed-thumbnail", @"--no-embed-thumbnail")]
			public ThreeWayOpt Thumb
			{
				get;
			} = new(@"--embed-thumbnail", @"--no-embed-thumbnail")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.Val == true
							? new DictionaryToOpt()
							{
								[@"outtpl"] = new DictionaryToOpt()
								{
									[@"pl_thumbnail"] = string.Empty,
								},
								[@"postprocessors"] = new System.Collections.Generic.List<DictionaryToOpt>()
										{
											new()
											{
												[@"already_have_thumbnail"] = false,
												[@"key"] = @"EmbedThumbnail",
											},
										},
								[@"writethumbnail"] = true,
							}
							: [],
			};

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will embed metadata information into the post-processed file.  If <see langword="false"/>, it won’t.  If you leave <see cref="MetaData"/> set to the default value of <see langword="null"/> and no config file specifies either --embed-metadata or --no-embed-metadata, yt-dlp will act as though <see cref="MetaData"/> is set to <see langword="false"/>.  Note: If <see cref="MetaData"/> is <see langword="true"/>, the defaults for embedding chapter and info json data changes to reflect the <see cref="MetaData"/> value.  With both types of data, setting <see cref="MetaData"/> to <see langword="true"/> will cause yt-dlp to default to embedding both chapter and info json data as well.  If you don’t want that, set <see cref="Chapters"/> and/or <see cref="InfoJSON"/> to <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingEmbedMetaData), @"If true, yt-dlp will embed metadata information into the post-processed file.  If false, it won’t.  If you leave it set to the default value of null and no config file specifies either --embed-metadata or --no-embed-metadata, yt-dlp will act as though MetaData is set to false.  Note: If MetaData is true, the defaults for embedding chapter and info json data changes to reflect the MetaData value.  With both types of data, setting MetaData to true will cause yt-dlp to default to embedding both chapter and info json data as well.  If you don't want that, set Chapters and/or EmbedInfoJSON to false.", typeof(PostProcessingGroup), @"--embed-metadata",
				@"--no-embed-metadata")]
			public ThreeWayOpt MetaData
			{
				get;
			} = new(@"--embed-metadata", @"--no-embed-metadata")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.Val == true
							? new DictionaryToOpt()
							{
								[@"postprocessors"] = new System.Collections.Generic.List<DictionaryToOpt>()
										{
											new()
											{
												[@"add_chapters"] = true,
												[@"add_infojson"] = @"if_exists",
												[@"add_metadata"] = true,
												[@"key"] = @"FFmpegMetaData",
											}
										},
							}
							: [],
			};

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will embed chapters into the post-processed file.  If <see langword="false"/>, it won’t do that.  If you leave <see cref="Chapters"/> set to the default value of <see langword="null"/> and no config file specifies --embed-chapters or --no-embed-chapters, yt-dlp will use a value that depends on if metadata is being embedded.  Unless told otherwise by either <see cref="Chapters"/> or a config file with --embed-chapters, yt-dlp will embed chapters if metadata is being embedded and won’t embed them if meta data isn’t being embedded.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingEmbedChapters), @"If true, yt-dlp will embed chapters into the post-processed file.  If false, it won’t do that.  If you leave EmbedChapters set to the default value of null and no config file specifies --embed-chapters or --no-embed-chapters, yt-dlp will use a value that depends on if metadata is being embedded.  Unless told otherwise by either Chapters or a config file with --embed-chapters, yt-dlp will embed chapters if metadata is being embedded and won’t embed them if meta data isn't being embedded.", typeof(PostProcessingGroup), @"--embed-chapters", @"--no-embed-chapters")]
			public ThreeWayOpt Chapters
			{
				get;
			} = new(@"--embed-chapters", @"--no-embed-chapters")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.Val == true
							? new DictionaryToOpt()
							{
								[@"postprocessors"] = new System.Collections.Generic.List<DictionaryToOpt>()
										{
											new()
											{
												[@"add_chapters"] = true,
												[@"add_infojson"] = null,
												[@"add_metadata"] = false,
												[@"key"] = @"FFmpegMetaData",
											}
										},
							}
							: [],
			};

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will embed info JSON data into the post-processed file.  If <see langword="false"/>, it won’t do that.  If you leave <see cref="InfoJSON"/> set to the default value of <see langword="null"/> and no config file specifies --embed-infojson or --no-embed-infojson, yt-dlp will use a value that depends on if metadata is being embedded.  Unless told otherwise by either <see cref="InfoJSON"/> or a config file, yt-dlp will embed info JSON data if metadata is being embedded and won’t embed it if meta data isn’t being embedded.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingEmbedInfoJSON), @"If true, yt-dlp will embed info JSON data into the post-processed file.  If false, it won’t do that.  If you leave InfoJSON set to the default value of null and no config file specifies --embed-infojson or --no-embed-infojson, yt-dlp will use a value that depends on if metadata is being embedded.  Unless told otherwise by either InfoJSON or a config file, yt-dlp will embed info JSON data if metadata is being embedded and won’t embed it if meta data isn’t being embedded.", typeof(PostProcessingGroup),
				@"--embed-infojson", @"--no-embed-infojson")]
			public ThreeWayOpt InfoJSON
			{
				get;
			} = new(@"--embed-info-json", @"--no-embed-info-json")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.Val == true
							? new DictionaryToOpt()
							{
								[@"postprocessors"] = new System.Collections.Generic.List<DictionaryToOpt>()
										{
											new()
											{
												[@"add_chapters"] = null,
												[@"add_infojson"] = true,
												[@"add_metadata"] = false,
												[@"key"] = @"FFmpegMetaData",
											}
										},
							}
							: [],
			};


			/// <summary>
			/// Lists all options in <see cref="EmbedGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					Subs,
					Thumb,
					MetaData,
					Chapters,
					InfoJSON,
				];
		}

		/// <summary>
		/// Groups all of the options for parsing metadata.  There’s one option for each value from <see cref="PreDefinedProcessorEvts"/>.  All use --parse-metadata.
		/// </summary>
		public sealed partial class ParseMetaDataInstructGroup
		{
			public ParseMetaDataInstructGroup()
			{
			}

			public ParseMetaDataInstructGroup(in ParseMetaDataInstructGroup copyThis)
			{
				PreProcess = new(copyThis.PreProcess);
				AfterFilter = new(copyThis.AfterFilter);
				Vid = new(copyThis.Vid);
				BeforeDownLoad = new(copyThis.BeforeDownLoad);
				PostProcess = new(copyThis.PostProcess);
				AfterMove = new(copyThis.PreProcess);
				AfterVid = new(copyThis.AfterVid);
				PlayList = new(copyThis.PlayList);
				PoplateSeasonAndEpisodeFieldsFromTitle = new(copyThis.PoplateSeasonAndEpisodeFieldsFromTitle);
			}


			private static readonly System.Text.RegularExpressions.Regex regexSeasonNumExtractor = GetSeasonNumExtractor();
			private static readonly System.Text.RegularExpressions.Regex regexEpisodeNumExtractor = GetEpisodeNumExtractor();
			private static readonly System.Text.RegularExpressions.Regex regexCombinedSeasonEpisodeNumExtractor = GetCombinedSeasonEpisodeNumExtractor();


			/// <summary>
			/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataPreProcess), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won't be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
			public ParseMetaDataInstructOpt PreProcess
			{
				get;
			} = new(@"pre_process");

			/// <summary>
			/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataAfterFilter), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
			public ParseMetaDataInstructOpt AfterFilter
			{
				get;
			} = new(@"after_filter");

			/// <summary>
			/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataVid), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
			public ParseMetaDataInstructOpt Vid
			{
				get;
			} = new(@"video");

			/// <summary>
			/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataBeforeDownLoad), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
			public ParseMetaDataInstructOpt BeforeDownLoad
			{
				get;
			} = new(@"before_dl");

			/// <summary>
			/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataPostProcess), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
			public ParseMetaDataInstructOpt PostProcess
			{
				get;
			} = new(@"post_process");

			/// <summary>
			/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataAfterMove), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
			public ParseMetaDataInstructOpt AfterMove
			{
				get;
			} = new(@"after_move");

			/// <summary>
			/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataAfterVid), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
			public ParseMetaDataInstructOpt AfterVid
			{
				get;
			} = new(@"after_video");

			/// <summary>
			/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataPlayList), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
			public ParseMetaDataInstructOpt PlayList
			{
				get;
			} = new(@"playlist");

			public OneOpt<bool> PoplateSeasonAndEpisodeFieldsFromTitle
			{
				get;
			} = new(false, "--parse-metadata")
			{
				ParamListGenerator =
					opt
						=> opt.CurVal
							? new System.Collections.Generic.List<string>()
								{
									@"--parse-metadata",
									$@"title:{regexSeasonNumExtractor}",
									@"--parse-metadata",
									$@"title:{regexEpisodeNumExtractor}",
									@"--parse-metadata",
									$@"title:{regexCombinedSeasonEpisodeNumExtractor}",
								}
							: [],
				PythonParamsGenerator =
					static (in opt)
						=> opt.CurVal
							? new DictionaryToOpt()
							{
								[@"postprocessors"] = new System.Collections.Generic.List<object>()
											{
												new System.Collections.Generic.List<DictionaryToOpt>()
												{
													new()
													{
														[@"key"] = @"MetadataParser",
														[@"when"] = @"pre_process",
														[@"actions"] = new System.Collections.Generic.List<object>()
														{
															new System.Collections.Generic.List<object>()
															{
																new System.Collections.Generic.List<object>()
																{
																	YtDlpWrapper.yt?.postprocessor.metadataparser.MetadataParserPP.interpretter,
																	@"title",
																	regexSeasonNumExtractor.ToString(),
																},
																new System.Collections.Generic.List<object>()
																{
																	YtDlpWrapper.yt?.postprocessor.metadataparser.MetadataParserPP.interpretter,
																	@"title",
																	regexEpisodeNumExtractor.ToString(),
																},
																new System.Collections.Generic.List<object>()
																{
																	YtDlpWrapper.yt?.postprocessor.metadataparser.MetadataParserPP.interpretter,
																	@"title",
																	regexCombinedSeasonEpisodeNumExtractor.ToString(),
																},
															}
														}
													},
												},
										},
							}
							: [],
			};


			/// <summary>
			/// Lists all options in <see cref="ParseMetaDataInstructGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					PreProcess,
					AfterFilter,
					Vid,
					BeforeDownLoad,
					PostProcess,
					AfterMove,
					AfterVid,
					PlayList,
					PoplateSeasonAndEpisodeFieldsFromTitle,
				];


			[System.Text.RegularExpressions.GeneratedRegexAttribute(@"(?i)Season\s*(?<season_number>\d+)", System.Text.RegularExpressions.RegexOptions.None, "en-US")]
			private static partial System.Text.RegularExpressions.Regex GetSeasonNumExtractor();

			[System.Text.RegularExpressions.GeneratedRegexAttribute(@"(?i)Episode\s*(?<episode_number>\d+)", System.Text.RegularExpressions.RegexOptions.None, "en-US")]
			private static partial System.Text.RegularExpressions.Regex GetEpisodeNumExtractor();

			[System.Text.RegularExpressions.GeneratedRegexAttribute(@"(?i)S\s*(?<season_number>\d+)\s*E\s*(?<episode_number>\d+)", System.Text.RegularExpressions.RegexOptions.None, "en-US")]
			private static partial System.Text.RegularExpressions.Regex GetCombinedSeasonEpisodeNumExtractor();
		}

		/// <summary>
		/// Groups all of the options for replacing metadata.  There’s one option for each value from <see cref="PreDefinedProcessorEvts"/>.  All use --replace-in-metadata.
		/// </summary>
		public sealed partial class ReplaceInMetaDataInstructGroup
		{
			public ReplaceInMetaDataInstructGroup()
			{
			}

			public ReplaceInMetaDataInstructGroup(in ReplaceInMetaDataInstructGroup copyThis)
			{
				PreProcess = new(copyThis.PreProcess);
				AfterFilter = new(copyThis.AfterFilter);
				Vid = new(copyThis.Vid);
				BeforeDownLoad = new(copyThis.BeforeDownLoad);
				PostProcess = new(copyThis.PostProcess);
				AfterMove = new(copyThis.AfterMove);
				AfterVid = new(copyThis.AfterVid);
				PlayList = new(copyThis.PlayList);
				TrimTrailingPeriodInTitle = new(copyThis.TrimTrailingPeriodInTitle);
				ReplaceEllipsisInTitle = new(copyThis.ReplaceEllipsisInTitle);
				ReplaceMDashInTitle = new(copyThis.ReplaceMDashInTitle);
			}


			private static readonly System.Text.RegularExpressions.Regex regexTrailingPeriodMatcher = GetTrailingPeriodMatcher();
			private static readonly System.Text.RegularExpressions.Regex regexEllipsisMatcher = GetEllipsisMatcher();
			private static readonly System.Text.RegularExpressions.Regex regexMDashMatcher = GetMDashMatcher();


			/// <summary>
			/// Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingReplaceInMetaDataPreProcess), @"Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.", typeof(PostProcessingGroup), @"--replace-in-metadata")]
			public ReplaceInMetaDataInstructOpt PreProcess
			{
				get;
			} = new(@"pre_process");

			/// <summary>
			/// Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingReplaceInMetaDataAfterFilter), @"Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.", typeof(PostProcessingGroup), @"--replace-in-metadata")]
			public ReplaceInMetaDataInstructOpt AfterFilter
			{
				get;
			} = new(@"after_filter");

			/// <summary>
			/// Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingReplaceInMetaDataVid), @"Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.", typeof(PostProcessingGroup), @"--replace-in-metadata")]
			public ReplaceInMetaDataInstructOpt Vid
			{
				get;
			} = new(@"video");

			/// <summary>
			/// Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingReplaceInMetaDataBeforeDownLoad), @"Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.", typeof(PostProcessingGroup), @"--replace-in-metadata")]
			public ReplaceInMetaDataInstructOpt BeforeDownLoad
			{
				get;
			} = new(@"before_dl");

			/// <summary>
			/// Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingReplaceInMetaDataPostProcess), @"Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.", typeof(PostProcessingGroup), @"--replace-in-metadata")]
			public ReplaceInMetaDataInstructOpt PostProcess
			{
				get;
			} = new(@"post_process");

			/// <summary>
			/// Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingReplaceInMetaDataAfterMove), @"Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.", typeof(PostProcessingGroup), @"--replace-in-metadata")]
			public ReplaceInMetaDataInstructOpt AfterMove
			{
				get;
			} = new(@"after_move");

			/// <summary>
			/// Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingReplaceInMetaDataAfterVid), @"Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.", typeof(PostProcessingGroup), @"--replace-in-metadata")]
			public ReplaceInMetaDataInstructOpt AfterVid
			{
				get;
			} = new(@"after_video");

			/// <summary>
			/// Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingReplaceInMetaDataPlayList), @"Replace text in a metadata field using the given regex.  Note: This parameter won’t be emitted unless you set all three fields.", typeof(PostProcessingGroup), @"--replace-in-metadata")]
			public ReplaceInMetaDataInstructOpt PlayList
			{
				get;
			} = new(@"playlist");

			public OneOpt<bool> TrimTrailingPeriodInTitle
			{
				get;
			} = new(false, @"--replace-in-metadata")
			{
				ParamListGenerator =
					opt
						=> opt.CurVal
							? new System.Collections.Generic.List<string>()
								{
									@"--replace-in-metadata",
									@"pre_process:title",
									regexTrailingPeriodMatcher.ToString(),
									string.Empty,
								}
							: [],
				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal
							? new DictionaryToOpt()
							{
								[@"postprocessor"] = new System.Collections.Generic.List<DictionaryToOpt>()
										{
											new()
											{
												[@"actions"] = new System.Collections.Generic.List<object>()
													{
														new System.Collections.Generic.List<object>()
														{
															YtDlpWrapper.yt?.postprocessor.metadataparser.MetadataParserPP.replacer,
															@"title",
															regexTrailingPeriodMatcher.ToString(),
															string.Empty,
														},
													},
												[@"key"] = @"MetadataParser",
												[@"when"] = @"pre_process",
											},
										},
							}
							: [],
			};

			public OneOpt<bool> ReplaceEllipsisInTitle
			{
				get;
			} = new(false, @"--replace-in-metadata")
			{
				ParamListGenerator =
					opt
						=> opt.CurVal
							? new System.Collections.Generic.List<string>()
								{
									@"--replace-in-metadata",
									@"pre_process:title",
									regexEllipsisMatcher.ToString(),
									@"…",
								}
							: [],
				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal
							? new DictionaryToOpt()
							{
								[@"postprocessor"] = new System.Collections.Generic.List<DictionaryToOpt>()
										{
											new()
											{
												[@"actions"] = new System.Collections.Generic.List<object>()
													{
														new System.Collections.Generic.List<object>()
														{
															YtDlpWrapper.yt?.postprocessor.metadataparser.MetadataParserPP.replacer,
															@"title",
															regexEllipsisMatcher.ToString(),
															@"…",
														},
													},
												[@"key"] = @"MetadataParser",
												[@"when"] = @"pre_process",
											},
										},
							}
							: [],
			};

			public OneOpt<bool> ReplaceMDashInTitle
			{
				get;
			} = new(false, @"--replace-in-metadata")
			{
				ParamListGenerator =
					opt
						=> opt.CurVal
							? new System.Collections.Generic.List<string>()
								{
									@"--replace-in-metadata",
									@"pre_process:title",
									regexMDashMatcher.ToString(),
									@"—",
								}
							: [],
				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal
							? new DictionaryToOpt()
							{
								[@"postprocessor"] = new System.Collections.Generic.List<DictionaryToOpt>()
										{
											new()
											{
												[@"actions"] = new System.Collections.Generic.List<object>()
													{
														new System.Collections.Generic.List<object>()
														{
															YtDlpWrapper.yt?.postprocessor.metadataparser.MetadataParserPP.replacer,
															@"title",
															regexMDashMatcher.ToString(),
															@"—",
														},
													},
												[@"key"] = @"MetadataParser",
												[@"when"] = @"pre_process",
											},
										},
							}
							: [],
			};


			/// <summary>
			/// Lists all options in <see cref="ReplaceInMetaDataInstructGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					PreProcess,
					AfterFilter,
					Vid,
					BeforeDownLoad,
					PostProcess,
					AfterMove,
					AfterVid,
					PlayList,
					TrimTrailingPeriodInTitle,
					ReplaceEllipsisInTitle,
					ReplaceMDashInTitle,
				];


			[System.Text.RegularExpressions.GeneratedRegex(@"(?<![^\.])\.$")]
			private static partial System.Text.RegularExpressions.Regex GetTrailingPeriodMatcher();

			[System.Text.RegularExpressions.GeneratedRegex(@"\.{2,}")]
			private static partial System.Text.RegularExpressions.Regex GetEllipsisMatcher();

			[System.Text.RegularExpressions.GeneratedRegex(@"\s*(-{2,}|—)\s*")]
			private static partial System.Text.RegularExpressions.Regex GetMDashMatcher();
		}

		/// <summary>
		/// Groups all options for running an executable at the specified time.  Contains one option per value in <see cref="PreDefinedProcessorEvts"/>.  Each option results in a --exec.
		/// </summary>
		public sealed class ExecCmdGroup
		{
			public ExecCmdGroup()
			{
			}

			public ExecCmdGroup(in ExecCmdGroup copyThis)
			{
				PreProcess = new(copyThis.PreProcess);
				AfterFilter = new(copyThis.AfterFilter);
				Vid = new(copyThis.Vid);
				BeforeDownLoad = new(copyThis.BeforeDownLoad);
				PostProcess = new(copyThis.PostProcess);
				AfterMove = new(copyThis.AfterMove);
				AfterVid = new(copyThis.AfterVid);
				PlayList = new(copyThis.PlayList);
				None = new(copyThis.None);
			}


			public class CmdOpt : BaseOneOpt<CmdOpt>
			{
				public CmdOpt(in string strPrefix)
				{
					PythonParamsGenerator = GetPythonParams;

					this.strPrefix = strPrefix;

					lliststrCmds = [];
				}

				public CmdOpt(in string strPrefix, in string strOneCmdToRun)
				{
					PythonParamsGenerator = GetPythonParams;

					this.strPrefix = strPrefix;

					lliststrCmds = new([strOneCmdToRun]);
				}

				public CmdOpt(in CmdOpt copyThis)
				{
					PythonParamsGenerator = GetPythonParams;

					strPrefix = copyThis.strPrefix;

					lliststrCmds = new(copyThis.lliststrCmds);
				}


				public readonly string strPrefix;

				private readonly System.Collections.Generic.LinkedList<string> lliststrCmds;


				public override System.Collections.Generic.IEnumerable<string>? Params
					=> lliststrCmds.IsEmpty()
						? []
						: (
								from strCurCmd in lliststrCmds
								select new System.Collections.Generic.List<string>()
								{
									@"--exec",
									$@"{strPrefix}:{strCurCmd}",
								}
							).SelectMany
							(
								list
									=> list
							);

				internal override DictionaryToOpt? PythonParams
					=> PythonParamsGenerator?.Invoke(this);

				private DictionaryToOpt? GetPythonParams(in CmdOpt opt)
					=> opt.lliststrCmds.IsEmpty()
						? []
						: new DictionaryToOpt()
						{
							[@"postprocessors"] = new System.Collections.Generic.List<DictionaryToOpt>()
									{
										new()
										{
											[@"exec_cmd"] = opt.lliststrCmds,
											[@"key"] = @"exec",
											[@"when"] = opt.strPrefix,
										}
									},
						};
			}


			/// <summary>
			/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdPreProcess), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
			public CmdOpt PreProcess
			{
				get;
			} = new(@"pre_process");

			/// <summary>
			/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdAfterFilter), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
			public CmdOpt AfterFilter
			{
				get;
			} = new(@"after_filter");

			/// <summary>
			/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdVid), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
			public CmdOpt Vid
			{
				get;
			} = new(@"video");

			/// <summary>
			/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdBeforeDownLoad), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
			public CmdOpt BeforeDownLoad
			{
				get;
			} = new(@"before_dl");

			/// <summary>
			/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdBeforeDownLoad), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
			public CmdOpt PostProcess
			{
				get;
			} = new(@"post_process");

			/// <summary>
			/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdAfterMove), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
			public CmdOpt AfterMove
			{
				get;
			} = new(@"after_move");

			/// <summary>
			/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdAfterVid), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
			public CmdOpt AfterVid
			{
				get;
			} = new(@"after_video");

			/// <summary>
			/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdPlayList), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
			public CmdOpt PlayList
			{
				get;
			} = new(@"playlist");

			/// <summary>
			/// Remove any previously defined execution command including any set using --exec in a config file.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdNone), @"Remove any previously defined execution command including any set using --exec in a config file.", typeof(PostProcessingGroup), @"--no-exec")]
			public OneOpt<bool> None
			{
				get;
			} = new(false, @"--no-exec")
			{
				PythonParamsGenerator =
					(in opt)
						=> new DictionaryToOpt() { },
			};


			/// <summary>
			/// Lists all options in <see cref="ExecCmdGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					PreProcess,
					AfterFilter,
					Vid,
					BeforeDownLoad,
					PostProcess,
					AfterMove,
					AfterVid,
					PlayList,
				];
		}


		/// <summary>
		/// Extracts audio and specifies a format to convert the audio to.  Currently supported: best (default), aac, alac, flac, m4a, mp3, opus, vorbis, wav).  You can specify multiple rules; e.g. <c><see cref="ExtractAudioToFmt"/>="aac>alac/mp3>flac/opus"</c> will recode aac to alac, mp3 to flac and anything else to opus.  If <see cref="ExtractAudioToFmt"/> is an empty string, no extraction will be done unless a config file specifies -x or --extract-audio.  <see cref="ExtractAudioToFmt"/> combines --extract-audio and --audio-format.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExtractAudioToFmt), @"Extracts audio and specifies a format to convert the audio to.  Currently supported: best (default), aac, alac, flac, m4a, mp3, opus, vorbis, wav).  You can specify multiple rules; e.g. ExtractAudioToFmt=""aac>alac/mp3>flac/opus"" will recode aac to alac, mp3 to flac and anything else to opus.  If extractAudioToFmt is an empty string, no extraction will be done unless a config file specifies -x or --extract-audio.  ExtractAudioToFmt combines --extract-audio and --audio-format.", typeof(PostProcessingGroup),
			@"--extract-audio", @"--audio-format")]
		public AudioExtractionOpt ExtractAudioToFmt
		{
			get;
		} = new(5);

		/// <summary>
		/// Remux the video into another container if necessary (currently supported: avi, flv, gif, mkv, mov, mp4, webm, aac, aiff, alac, flac, m4a, mka, mp3, ogg, opus, vorbis, wav).  If the target container doesn’t support the video/audio codec, remuxing will fail.  You can specify multiple rules; e.g. <c><see cref="RemuxVid"/>="aac>m4a/mov>mp4/mkv"</c> will remux aac to m4a, mov to mp4 and anything else to mkv.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingRemuxVid), @"Remux the video into another container if necessary (currently supported: avi, flv, gif, mkv, mov, mp4, webm, aac, aiff, alac, flac, m4a, mka, mp3, ogg, opus, vorbis, wav).  If the target container doesn’t support the video/audio codec, remuxing will fail.  You can specify multiple rules; e.g. RemuxVid=""aac>m4a/mov>mp4/mkv"" will remux aac to m4a, mov to mp4 and anything else to mkv.",
			typeof(PostProcessingGroup), @"--remux-video")]
		public FmtConversionOpt<SupportedVidFmts> RemuxVid
		{
			get;
		} = new(@"--remux-video", @"FFmpegVideoRemuxer");

		/// <summary>
		/// Remux the video into another container if necessary (currently supported: avi, flv, gif, mkv, mov, mp4, webm, aac, aiff, alac, flac, m4a, mka, mp3, ogg, opus, vorbis, wav).  You can specify multiple rules; e.g. <c><see cref="RecodeVid"/>="aac>m4a/mov>mp4/mkv"</c> will remux aac to m4a, mov to mp4 and anything else to mkv.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostPocessingRecodeVid), @"Remux the video into another container if necessary (currently supported: avi, flv, gif, mkv, mov, mp4, webm, aac, aiff, alac, flac, m4a, mka, mp3, ogg, opus, vorbis, wav).  You can specify multiple rules; e.g. RecodeVid=""aac>m4a/mov>mp4/mkv"" will remux aac to m4a, mov to mp4 and anything else to mkv.", typeof(PostProcessingGroup), @"--recode-video")]
		public FmtConversionOpt<SupportedVidFmts> RecodeVid
		{
			get;
		} = new(@"--recode-video", @"FfmpegVideoConverter");

		/// <summary>
		/// Give these arguments to the post-processors.  For built in post-processors, place instances of <see cref="PostProcessingRecordForKnownProcessorVal"/> into the list.  Implicit typecasts exist for that type from <see cref="PreDefinedProcessorEvts"/> and <see cref="SupportedProcessorExecutables"/>, though you won’t have any When or arguments.  To specify both an event and a executable, create a new <see cref="PostProcessingRecordForKnownProcessorVal"/> using the constructor.  <see cref="PostProcessingRecordForKnownProcessorVal"/> entries will be emitted as --postprocessor-args.  If you want to run a processor that yt-dlp doesn’t know about, place a <see cref="System.IO.FileInfo"/> (constructed around a full path)  or <see cref="string"/> (with just the filename of an executable in the path) into the list to run it without arguments at the default time.  If you want to run it at a specific event or with arguments, create either an instance of either <see cref="PostProcessorArgsForExeVal"/> or <see cref="NamedExeProcessorArgsVal"/> and pass the needed fields to it.  Note: Don't use <see cref="System.IO.FileInfo"/> instances that lack a full path.  Instead, just use the name of the executable in a string.  All <see cref="System.IO.FileInfo"/> instances will be mapped to a string with <see cref="System.IO.FileSystemInfo.FullName"/> which assumes the current working directory for this program. Any instances of <see cref="PostProcessorArgsForExeVal"/> or <see cref="NamedExeProcessorArgsVal"/> will be emitted as --use-postprocessor.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingArgs), @"Give these arguments to the post-processors.  For built in post-processors, place instances of PostProcessingRecordForKnownProcessorVal into the list.  Implicit typecasts exist for that type from PreDefinedProcessorEvts and SupportedProcessorExecutables, though you won't have any When or arguments.  To specify both an event and a executable, create a new PostProcessingRecordForKnownProcessorVal using the constructor.  PostProcessingRecordForKnownProcessorVal entries will be emitted as --postprocessor-args.  If you want to run a processor that yt-dlp doesn't know about, place a System.IO.FileInfo or string into the list to run it without arguments at the default time.  If you want to run it at a specific event or with arguments, create either an instance of either PostProcessorArgsForExeVal or NamedExeProcessorArgsVal and pass the needed fields to it.  Note: Don't use System.IO.FileInfo instances that lack a full path.  Instead, just use the name of the executable in a string.  All System.IO.FileInfo instances will be mapped to a string with FullName which assumes the current working directory for this program.  Any instances of PostProcessorArgsForExeVal or NamedExeProcessorArgsVal will be emitted as --use-postprocessor.", typeof(PostProcessingGroup), @"--postprocessing-args", @"--use-postprocessor")]
		public OneOpt<System.Collections.Generic.IEnumerable<BasePostProcessorArgsVal>?> PostProcessorArgs
		{
			get;
		} = new(null, string.Empty)
		{
			ParamListGenerator =
				static opt
					=>
				{
					if(opt.CurVal == null || opt.CurVal.Count() == 0)
						return [];

					System.Collections.Generic.IEnumerable<string[]> groupArgs =
						from BasePostProcessorArgsVal curArg in opt.CurVal
						let strValText = curArg.ValText
						where strValText is not null
						select
							new string[]
							{
										curArg is PostProcessingRecordForKnownProcessorVal
											? @"--postprocessing-args"
											: @"--use-postprocessor",
										strValText,
							};

					return groupArgs.SelectMany(
						static array
							=> array
					);
				},

			PythonParamsGenerator =
				(in opt)
					=>
				{
					if(opt.CurVal is not null && opt.CurVal.Count() > 0)
					{
						DictionaryToOpt result = [];

						DictionaryToOpt dicttoKnownProcessorEntry;
						result[@"postprocessor_args"] = dicttoKnownProcessorEntry = [];

						System.Collections.Generic.List<DictionaryToOpt> listdicttoNamedProcessors;
						result[@"postprocessors"] = listdicttoNamedProcessors = [];

						foreach(BasePostProcessorArgsVal bppavCur in opt.CurVal)
							if(bppavCur is PostProcessingRecordForKnownProcessorVal pprfkpv && pprfkpv.WhichProcessorExecutable is not null)
								dicttoKnownProcessorEntry[pprfkpv.WhichProcessorExecutable] = pprfkpv.Args;
							else if(bppavCur is BasePostProcessorArgsForExeVal bppafevCur)
							{
								DictionaryToOpt dictto = new()
								{
									[@"key"] = bppafevCur.FullExePath,
									[@"when"] = bppafevCur.When.ValText,
								};
								if(bppafevCur.Args is not null)
									foreach(System.Collections.Generic.KeyValuePair<string, string> kvpCurEntry in bppafevCur.Args)
										dictto[kvpCurEntry.Key] = kvpCurEntry.Value;

								listdicttoNamedProcessors.Add(dictto);
							}
							else
								throw new System.InvalidProgramException(@"Unknown or invalid post processor arg instance");

						return result;
					}

					return new DictionaryToOpt();
				},
		};

		/// <summary>
		/// If <see langword="true"/>, yt-dlp won’t delete the video after post-processing.  If <see langword="false"/>, it would.  If you leave <see cref="KeepVid"/> set to the default value of <see langword="null"/> and a config file doesn't specify either --keep-video or --no-keep-video, yt-dlp will act as though <see cref="KeepVid"/> is <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingKeepVid), @"If true, yt-dlp won't delete the video after post-processing.  If false, it would.  If you leave KeepVid set to the default value of null and a config file doesn't specify either --keep-video or --no-keep-video, yt-dlp will act as though KeepVid is false.", typeof(PostProcessingGroup), @"--keep-video", @"--no-keep-video")]

		public ThreeWayOpt KeepVid
		{
			get;
		} = new(@"--keep-video", @"--no-keep-video", @"keepvideo", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// If this is <see langword="true"/>, yt-dlp will overwrite any existing post-processor files.  If <see langword="false"/>, it won’t do that.  If you leave <see cref="OverWrites"/> set to the default value of <see langword="null"/> and no config file specifies either --post-overwrites or --no-post-overwrites, yt-dlp will act as though you set <see cref="OverWrites"/> to <see langword="true"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingOverWrites), @"If this is true, yt-dlp will overwrite any existing post-processor files.  If false, it won’t do that.  If you leave OverWrites set to the default value of null and no config file specifies either --post-overwrites or --no-post-overwrites, yt-dlp will act as though you set OverWrites to true.", typeof(PostProcessingGroup), @"--post-overwrites",
			@"--no-post-overwrites")]
		public ThreeWayOpt OverWrites
		{
			get;
		} = new(@"--post-overwrites", @"--no-post-overwrites");

		/// <summary>
		/// Provides access to a <see cref="EmbedGroup"/> instance
		/// </summary>
		public EmbedGroup Embed
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="ParseMetaDataInstructGroup"/> instance
		/// </summary>
		public ParseMetaDataInstructGroup ParseMetaDataInstruct
		{
			get;
		} = new();

		/// <summary>
		/// Provide access to a <see cref="ReplaceInMetaDataInstructGroup"/>
		/// </summary>
		public ReplaceInMetaDataInstructGroup ReplaceInMetaDataInstruct
		{
			get;
		} = new();

		/// <summary>
		/// Write metadata to the video file’s xattrs (using Dublin Core and XDG standards)
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingWriteMetaDataToXATTR), @"Write metadata to the video file’s xattrs (using Dublin Core and XDG standards)", typeof(PostProcessingGroup), @"--xattrs")]
		public OneOpt<bool> WriteMetaDataToXATTR
		{
			get;
		} = new(false, @"--xattrs")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
						{
							[@"postprocessors"] = new System.Collections.Generic.List<DictionaryToOpt>()
									{
										new()
										{
											[@"key"] = @"XAttrMetadata",
										}
									},
						}
						: [],
		};

		/// <summary>
		/// Automatically correct known faults of the file.  One of <see cref="FixUpPolicyChoices.never"/> (do nothing), <see cref="FixUpPolicyChoices.warn"/> (only emit a warning), <see cref="FixUpPolicyChoices.detectOrWarn"/> (fix the file if we can, warn otherwise), <see cref="FixUpPolicyChoices.force"/> (try fixing even if the file already exists).  If you leave <see cref="FixUpPolicy"/> set to the default value of <see cref="FixUpPolicyChoices.@default"/> and no config file sets --fixup, yt-dlp will act as though <see cref="FixUpPolicy"/> was set to <see cref="FixUpPolicyChoices.detectOrWarn"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingFixUpPolicy), @"Automatically correct known faults of the file.  One of FixUpPolicyChoices.never (do nothing), FixUpPolicyChoices.warn (only emit a warning), FixUpPolicyChoices.detectOrWarn (fix the file if we can, warn otherwise), FixUpPolicyChoices.force (try fixing even if the file already exists).  If you leave FixUpPolicy set to the default value of FixUpPolicyChoices.@default and no config file sets --fixup, yt-dlp will act as though FixUpPolicy was set to FixUpPolicyChoices.detectOrWarn.", typeof(PostProcessingGroup), @"--fixup")]
		public OneOpt<FixUpPolicyChoices> FixUpPolicy
		{
			get;
		} = new(FixUpPolicyChoices.@default, @"--fixup")
		{
			PythonParamsGenerator =
				(in opt)
				=> opt.CurVal == FixUpPolicyChoices.@default
					? []
					: new DictionaryToOpt()
					{
						[@"fixup"] = opt.CurVal.ValText,
					},
		};

		/// <summary>
		/// Provides access to a <see cref="ExecCmdGroup"/> instance
		/// </summary>
		public ExecCmdGroup ExecCmd
		{
			get;
		} = new();

		/// <summary>
		/// Convert the subtitles to another format.  All values must come from <see cref="SubsGroup.SupportedFmts"/>.  Use <see cref="SubsGroup.SupportedFmts.doNotConvert"/> to prevent conversion even if a config file specifies --convert-subs or --convert-subtitles.  If you leave <see cref="ConvertSubs"/> set to the default value of <see cref="SubsGroup.SupportedFmts.@default"/> and no config file uses either --convert-subs or --convert-subtitles, yt-dlp will act as though <see cref="ConvertSubs"/> is set to <see cref="SubsGroup.SupportedFmts.doNotConvert"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingConvertSubs), @"Convert the subtitles to another format.  All values must come from SubsGroup.SupporteFmts. Use SubsGroup.SupportedFmts.doNotConvert to prevent conversion even if a config file specifies --convert-subs or --convert-SubsGroup.  If you leave ConvertSubs set to the default value of SubsGroup.SupportedFmts.@default and no config file uses either --convert-subs or --convert-subtitles, yt-dlp will act as though ConvertSubs is set to SubsGroup.SupportedFmts.doNotConvert.", typeof(PostProcessingGroup), @"--convert-subs")]
		public OneOpt<SubsGroup.SupportedFmts?> ConvertSubs
		{
			get;
		} = new(null, @"--convert-subs")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is null
						? []
						: new DictionaryToOpt()
						{
							[@"postprocessors"] = new System.Collections.Generic.List<DictionaryToOpt>()
									{
										new()
										{
											[@"format"] = opt.CurVal,
											[@"key"] = @"FFmpegSubtitlesConverter",
											[@"when"] = @"bwfore_dl",
										},
									},
						},
		};

		/// <summary>
		/// Convert the thumbnails to another format (currently supported: jpg, png, webp).  You can specify multiple rules; e.g. <c><see cref="ConvertThumbs"/>="jpg>webp/png"</c> will convert jpg to webp, and anything else to png.  Set <see cref="ConvertThumbs"/> to “none” to disable conversion (default).
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingConvertThumbs), @"Convert the thumbnails to another format (currently supported: jpg, png, webp).  You can specify multiple rules; e.g. ConvertThumbs=""jpg>webp/png"" will convert jpg to webp, and anything else to png.  Set ConvertThumbs to “none” to disable conversion (default).", typeof(PostProcessingGroup), @"--convert-thumbnails")]
		public FmtConversionOpt<SupportedThumbFmts> ConvertThumbs
		{
			get;
		} = new(@"--convert-thumbnails", @"FFmpegThumbnailConverter");

		/// <summary>
		/// Splits videos into multiple videos based on chapters.  Use with <see cref="FileSysGroup.PathsGroup.Chapters"/> and <see cref="FileSysGroup.TemplatesForOutputFilesGroup.Chapters"/> to specify the file name.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingSplitChapters), @"Splits videos into multiple videos based on chapters.  Use with FileSysGroup.PathsGroup.Chapters and FileSysGroup.TemplatesForOutputFilesGroup.Chapters to specify the file name.", typeof(PostProcessingGroup),
			@"--split-chapters", @"--no-split-chapters")]
		public ThreeWayOpt SplitChapters
		{
			get;
		} = new(@"--split-chapters", @"--no-split-chapters")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.Val is true
						? new DictionaryToOpt()
						{
							[@"postprocessorcs"] = new System.Collections.Generic.List<DictionaryToOpt>()
									{
										new()
										{
											[@"force_keyframes"] = false,
											[@"key"] = @"FFmpegSplitChapters",
										},
									},
						}
						: [],
		};

		/// <summary>
		/// Remove chapters whose title matches any of the regular expressions.  Use <c>[<see cref="regexInvalid"/>]</c> to prevent chapters from being removed even if a config file specifies --remove-chapters.  If <see cref="RemoveChapters"/> is either <see langword="null"/> or an empty array and no config file specifies --remove-chapters, no chapters will be removed.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingRemoveChapters), @"Remove chapters whose title matches any of the regular expressions.  Use [regexInvalid] to prevent chapters from being removed even if a config file specifies --remove-chapters.  If RemoveChapters is either null or an empty array and no config file specifies --remove-chapters, no chapters will be removed.", typeof(PostProcessingGroup), @"--remove-chapters", @"--no-remove-chapters")]
		public OneOpt<System.Collections.Generic.IReadOnlyList<System.Text.RegularExpressions.Regex>?> RemoveChapters
		{
			get;
		} = new([], string.Empty)
		{
			ParamListGenerator =
				opt
					=> opt.CurVal == null || opt.CurVal.Count == 0
						? []
						: opt.CurVal[0] == regexInvalid
							? [@"--no-remove-chapters"]
							: opt.CurVal.Select(
									curVal
										=> new string[]
											{
													@"--remove-chapters",
													curVal.ToString()
											}
								).SelectMany(
									curVal
										=> curVal
								),

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal == null || opt.CurVal.Count == 0 || opt.CurVal[0] == regexInvalid
							? []
							: new DictionaryToOpt()
							{
								[@"postprocessors"] = new System.Collections.Generic.List<DictionaryToOpt>()
										{
											new()
											{
												[@"force_keyferames"] = false,
												[@"key"] = @"ModifyChapters",
												[@"remove_chapter_patterns"] = opt.CurVal,
												[@"remove_ranges"] = System.Array.Empty<object>(),
												[@"remove_sponsor_segments"] = new System.Collections.Generic.HashSet<string>(),
												[@"sponsorblock_chapter_title"] = @"[SponsorBlock] %(category_names)l",
											},
										},
							},
		};

		/// <summary>
		/// If <see langword="true"/>, forces key frames at cuts when downloading, splitting, or removing sections.  This is slow due to needing to recode, but the resulting video might have fewer artifacts around the cuts.  If <see cref="ForceKeyFramesAtCuts"/> is <see langword="false"/>, yt-dlp won’t force them.  If you leave <see cref="ForceKeyFramesAtCuts"/> set to the default value of <see langword="false"/> and no config file specifies --force-keyframes-at-cuts, yt-dlp will act as though <see cref="ForceKeyFramesAtCuts"/> is <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingForceKeyFramesAtCuts), @"If true, forces key frames at cuts when downloading, splitting, or removing sections.  This is slow due to needing to recode, but the resulting video might have fewer artifacts around the cuts.  If ForceKeyFramesAtCuts is false, yt-dlp won’t force them.  If you leave ForceKeyFramesAtCuts set to the default value of null and no config file specifies --force-keyframes-at-cuts, yt-dlp will act as though ForceKeyFramesAtCuts is false.", typeof(PostProcessingGroup), @"--force-key-frames-at-cuts",
			@"--no-force-keyframes-at-cuts")]
		public ThreeWayOpt ForceKeyFramesAtCuts
		{
			get;
		} = new(@"--force-keyframes-at-cuts", @"--no-force-keyframes-at-cuts", @"force_keyframes_at_cuts", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);


		/// <summary>
		/// Lists all options in <see cref="PostProcessingGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to generate its own list of options.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				ExtractAudioToFmt,
				RemuxVid,
				RecodeVid,
				PostProcessorArgs,
				KeepVid,
				OverWrites,
				..Embed.AllOpt,
				..ParseMetaDataInstruct.AllOpt,
				..ReplaceInMetaDataInstruct.AllOpt,
				WriteMetaDataToXATTR,
				FixUpPolicy,
				..ExecCmd.AllOpt,
				ConvertSubs,
				ConvertThumbs,
				SplitChapters,
				RemoveChapters,
				ForceKeyFramesAtCuts,
		];
	}
}