// Ignore Spelling: Cnt Codec Vid Dlp Loc Ip Sel httpie avconv wget rstp rtmp mms Json Fmts Dest Arounds Hdrs bidi avi mkv mov langs Pwd strset llist Xattrs Concat Exe Vals Evts Remuxer Evt api Hls holodex Locs dlp's dateafter datebefore geo xff filesize bcats hlsnative avcov axel xattr xattribute mpegts ftp username firefox vivaldi basictext gnomekeyring kwallet na mtime postprocess bidiv fribidi multistreams lrc srt vtt ja twofactor netrc yt resx postprocessor fixup infojson aac alac flac, vorbis wav postprocessing keyframes jpg png webp selfpromo mpd webloc filepath aiff mka whatif

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

using Tools.Ext;

// ******************************************************************************************************************************************************
// ******************************************************************************************************************************************************
//
// Important: Some strings are provided as raw strings when possible in this file.  These are either yt-dlp options—in which case they must exactly match what
// yt-dlp is looking for—or the string is a default description of an option.  In the later case, the string is represented in Rsrcs.resx and any translation
// should occur there.  If corrections need to be made to the command line strings, consult with Will Pittenger to ensure the change is correct and won’t break
// anything.  If a default description needs to be changed, be sure to also change the en-US resx as that's the default translation.
//
// ******************************************************************************************************************************************************
// ******************************************************************************************************************************************************

public abstract class BaseVidCmdLet : BaseCmdLet
{
	/// <summary>
	/// When a URL has both a video ID and a playlist ID, which should it process?  <see cref="PartsOfUrlsThatCanBeDownloaded"/> lets you tell yt-dlp what you want data for and what it can ignore.
	/// </summary>
	public enum PartsOfUrlsThatCanBeDownloaded
	{
		/// <summary>
		/// Process only the video.  If the URL contains a playlist ID, that will be ignored.  Playlist fields will be blank and possible <see langword="null"/> including the playlist channel and ID.
		/// </summary>
		vid,

		/// <summary>
		/// Process only the playlist.  If a video ID is in the URL, information on that video will still be downloaded, but only as an entry in the video.
		/// </summary>
		playlist,

		/// <summary>
		/// Like <see cref="vid"/>, but the playlist fields will now be filled out if a playlist ID is present.
		/// </summary>
		both,
	}

	/// <summary>
	/// This structure defines almost all options.  The exceptions are in the remarks.
	/// </summary>
	/// <remarks>
	/// <para>There are a few yt-dlp options that aren’t covered by <see cref="AllGlobalOpts"/>.  Those are as follows:</para>
	/// <list type="bullet">
	///		<item>
	///			<description>Options that should be in your config file such as <c>--ffmpeg-location</c></description>
	///		</item>
	///		<item>
	///			<description>Options that are listed as obsolete by yt-dlp such as anything related to SponsKrub</description>
	///		</item>
	///		<item>
	///			<description>Options that most relevant derived CmdLets will expose as individual parameters such as <c>--playlist-items</c></description>
	///		</item>
	///		<item>
	///			<description>Some authentication options due to the edge case in which they’re needed</description>
	///		</item>
	/// </list>
	/// <para>Some options listed separately in the yt-dlp read me page are merged here.  For example, --use-postprocessor and --postprocessor-args are treated as one <see cref="IOneOpt"/> instance.  On the other hand, most options that take a prefix, such as --paths, become multiple <see cref="IOneOpt"/> instances, one for each possible prefix.</para>
	/// </remarks>
	[System.ComponentModel.ImmutableObject(true)]
	public struct AllGlobalOpts
	{
		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		public AllGlobalOpts()
		{
		}


		/// <summary>
		/// Represents one option.  Allows for arrays of options regardless of type.  All options implement <see cref="IOneOpt"/>.  Don’t confuse it with <see cref="IOneOptVal"/>.  The arrays of options allow for the property <see cref="AllOpt"/> to exist.  It transforms a list of options into a list we can pass to yt-dlp.
		/// </summary>
		public interface IOneOpt
		{
			/// <summary>
			/// Returns a list of strings to be passed to yt-dlp.
			/// </summary>
			System.Collections.Generic.IEnumerable<string> Params
			{
				get;
			}
		}

		/// <summary>
		/// Represents the value of one option.  Don’t confuse <see cref="IOneOptVal"/> with <see cref="IOneOpt"/>.  <see cref="IOneOptVal"/> allows instances of <see cref="IOneOpt"/> to hold any type and get the value returned as a <c><see langword="string"/>?</c>.
		/// </summary>
		/// <remarks>
		/// <para>Use a specialized option type implementing <see cref="IOneOpt"/> if multiple strings need to be returned.</para>
		/// </remarks>
		public interface IOneOptVal
		{
			/// <summary>
			/// Gets the value text.
			/// </summary>
			string? ValText
			{
				get;
			}
		}

		/// <summary>
		/// The majority of options are described with a <see cref="OneOpt{Type}"/>.  It can hold any one value that can be described as a single nullable string plus arrays and dictionaries (maps).
		/// </summary>
		/// <typeparam name="Type">The type of the value.  Don’t use <see cref="OneOpt{Type}"/> or anything derived from it with <c><see langword="bool"/>?</c>.  Instead, use <see cref="ThreeWayOpt"/>.</typeparam>
		/// <param name="typeDef">Specifies the default value for this option.  It’s used by <see cref="IsDefaulted"/>.</param>
		/// <param name="strParamName">Specifies the parameter name.  The text is used verbatim.  Use a raw string.</param>
		/// <seealso cref="IOneOpt" />
		public class OneOpt<Type>(in Type typeDef, in string strParamName)
			: IOneOpt
		{
			/// <summary>
			/// Specifies how to combine a dictionary/map key with the corresponding value.  This helps us match the syntax that yt-dlp is looking for.
			/// </summary>
			public enum KeyValCombinationRules
			{
				/// <summary>
				/// Separate them with semicolons "key;value"
				/// </summary>
				semiColon,

				/// <summary>
				/// Separate them with colons "key:value"
				/// </summary>
				colon,

				/// <summary>
				/// Use equal signs "key=value"
				/// </summary>
				equalSign,
			}

			/// <summary>
			/// Describes how to combine multiple values from a list.  Note: With dictionaries/maps that contain lists for each entry, the key/value pairs are listed
			/// one after the other with semicolons regards of how <see cref="HowToCombineVals"/> is set.  <see cref="HowToCombineVals"/> would then specify how to
			/// combine values within the list for each entry.
			/// </summary>
			public enum ValCombinationRules
			{
				/// <summary>
				/// Causes the parameter name to repeat for each value in the list.  So if <see cref="strParamName"/> is <c>"--option"</c>, you get <c>["--option", "value1", "--option", "value2"]</c>.
				/// </summary>
				paramReuse,

				/// <summary>
				/// Causes the various values to be delimited with semicolons.  So you might see <c>["--option", "value1;value2"]</c>.
				/// </summary>
				semiColon,

				/// <summary>
				/// Causes the various values to be delimited with colons.  So you might see <c>["--option", "value1:value2"]</c>.
				/// </summary>
				colon,

				/// <summary>
				/// Causes the various values to be delimited with slashes.  So you might see <c>["--option", "value1/value2"]</c>.
				/// </summary>
				slash,
			}


			/// <summary>
			/// Static constructor that ensures <c><see langword="bool"/>?</c> is never used.
			/// </summary>
			/// <exception cref="System.InvalidProgramException">Thrown if <typeparamref name="Type"/> is <c><see langword="bool"/>?</c></exception>
			static OneOpt()
			{
				if(typeof(Type) == typeof(bool?))
					throw new System.InvalidProgramException($"Instead of building a OneOpt around a nullable bool, use {nameof(ThreeWayOpt)}.");
			}


			/// <summary>
			/// Stores the default value so that <see cref="IsDefaulted" /> works.
			/// </summary>
			public readonly Type typeDef = typeDef;

			/// <summary>
			/// Stores the current value.  Non-members should use <see cref="CurVal"/> to change <see cref="typeCurVal"/>.
			/// </summary>
			/// <seealso cref="CurVal"/>
			private Type typeCurVal = typeDef;

			/// <summary>
			/// Stores the parameter name such as <c>"--option"</c>
			/// </summary>
			public readonly string strParamName = strParamName;


			/// <summary>
			/// Gets or sets how to make key value pair strings.
			/// </summary>
			public KeyValCombinationRules HowToMakeKeyValPairStrings
			{
				get;

				internal set;
			} = KeyValCombinationRules.colon;

			/// <summary>Gets or sets how to combine values.</summary>
			/// <remarks>
			/// <para>Note: If the value is dictionary/map with a list for each entry, <see cref="HowToCombineVals"/> applies to the values inside the entries.  The dictionary’s key/entry pairs will always be combined with semicolons in such dictionaries/maps.</para>
			/// </remarks>
			public ValCombinationRules HowToCombineVals
			{
				get;

				internal set;
			} = ValCombinationRules.paramReuse;

			/// <summary>
			/// Gets or sets the current value.  Implemented as a property in case we ever need to do some sanity checking on the new value.
			/// </summary>
			public Type CurVal
			{
				get
					=> typeCurVal;

				set
					=> typeCurVal = value;
			}

			/// <summary>
			/// Returns <see langword="true"/> if the value in <see cref="CurVal" /> matches the value in <see cref="typeDef" /> and <see langword="false"/> otherwise.
			/// </summary>
			public bool IsDefaulted
				=> typeCurVal?.Equals(typeDef) ?? false;

			/// <summary>
			/// Use to specify a method that returns the value instead of this <see cref="OneOpt{Type}"/> instance calling <see cref="object.ToString()"/> on the
			/// value.  This can be useful for enum types.
			/// </summary>
			public System.Func<Type, string?>? ValTextLookUp
			{
				get;

				internal set;
			} = null;

			/// <summary>
			/// Unlike <see cref="ValTextLookUp"/>, <see cref="MultipleValTextLookUp"/> returns an array.  Use it rather than <see cref="ValTextLookUp"/> when you set <see cref="HowToCombineVals"/> to <see cref="ValCombinationRules.paramReuse"/>.  This is important as each value might need to be converted to strings.
			/// </summary>
			public System.Func<Type, System.Collections.Generic.IEnumerable<string>?>? MultipleValTextLookUp
			{
				get;

				set;
			} = null;

			/// <summary>
			/// Provides a means of code reuse for converting values into a string.  Rather than repeating the needed code, <see cref="Params"/> calls <see cref="ValText"/>.
			/// </summary>
			public string? ValText
					=> ValTextLookUp is null
						? typeCurVal is System.Collections.Generic.IReadOnlyDictionary<string, Type> map
							? HowToCombineVals switch
								{
									ValCombinationRules.paramReuse
										=> null,

									ValCombinationRules.colon
											=> map.Select(
												curKey
													=> curKey.Key == null || curKey.Value == null
														? System.Array.Empty<string>()
														: HowToMakeKeyValPairStrings switch
															{
																KeyValCombinationRules.colon
																	=> [ParamToUse, $"{curKey.Key}:{curKey.Value}"],

																KeyValCombinationRules.equalSign
																	=>  [ParamToUse, $"{curKey.Key}={curKey.Value}"],
																_
																	=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<KeyValCombinationRules>(HowToMakeKeyValPairStrings,
																		@"While combining a key and value into a single string")
															}
												).Join(':'),

									ValCombinationRules.semiColon
											=> map.Select(
												curKey
													=> curKey.Key == null || curKey.Value == null
														? System.Array.Empty<string>()
														: HowToMakeKeyValPairStrings switch
															{
																KeyValCombinationRules.semiColon
																	=> [ParamToUse, $"{curKey.Key};{curKey.Value}"],

																KeyValCombinationRules.equalSign
																	=>  [ParamToUse, $"{curKey.Key}={curKey.Value}"],
																_
																	=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<KeyValCombinationRules>(HowToMakeKeyValPairStrings,
																		@"While combining a key and value into a single string")
															}
												).Join(';'),

										_
											=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<ValCombinationRules>(HowToCombineVals, @"While selecting how to combine " +
												@"values")
									}
								: typeCurVal is System.Collections.Generic.IEnumerable<Type> enumerable
									? HowToCombineVals switch
										{
											ValCombinationRules.paramReuse
												=> null,

											ValCombinationRules.semiColon
												=> enumerable.Join(';'),

											ValCombinationRules.colon
												=> enumerable.Join(':'),

											ValCombinationRules.slash
												=> enumerable.Join('/'),

											_
												=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<ValCombinationRules>(HowToCombineVals, @"While formatting a string for a " +
													@"multiple value parameter"),
								}
							: typeCurVal?.ToString()
						: ValTextLookUp(typeCurVal);

			/// <summary>
			/// Use to specify a method that returns one or more parameter names in a list.
			/// </summary>
			public System.Func<Type, string>? CustomParamNameLookUp
			{
				get;

				internal set;
			}

			/// <summary>
			/// Computes the parameter to be used.  This is a shortcut to prevent duplication of the needed code.
			/// </summary>
			public string ParamToUse
				=> strParamName == string.Empty
					? CustomParamNameLookUp == null
						? throw new System.InvalidOperationException("No parameter name available.  Set in the constructor or with CustomParamNameLookUp.")
						: CustomParamNameLookUp(typeCurVal)
					: strParamName;

			/// <summary>Returns a list of the final parameters to pass to yt-dlp for this option.  In some cases, this can be an empty array as noted in the remarks.</summary>
			/// <remarks>
			/// <para><see cref="Params" /> returns an empty array in the following situations:</para>
			/// <list type="bullet">
			///		<item>If <typeparamref name="Type"/> is <see langword="bool"/> and <see cref="CurVal" /> is <see langword="false"/></item>
			///		<item>Any empty list</item>
			///		<item>If <typeparamref name="Type"/> is <see langword="string"/> and <see cref="CurVal" /> matches <see cref="string.Empty" /></item>
			///	</list>
			///	<para>Note: <see cref="Params"/> also ignores any items in a list that are <see langword="null"/>.  If <typeparamref name="Type"/> is a dictionary/map, those rules also applies to the values of any lists in the dictionary's entries.  If the result is the list or dictionary has no remaining entries, the return value will be an empty array.</para>
			/// </remarks>
			/// <seealso cref="HowToCombineVals"/>
			/// <seealso cref="HowToMakeKeyValPairStrings"/>
			/// <seealso cref="ValTextLookUp"/>
			/// <seealso cref="CustomParamNameLookUp"/>
			/// <seealso cref="ParamListGenerator"/>
			public virtual System.Collections.Generic.IEnumerable<string> Params
			{
				get
				{
					if(typeCurVal is bool bVal)
						return bVal
							? [ParamToUse] 
							: System.Array.Empty<string>();

					if(typeCurVal == null)
						return [];

					if(typeCurVal is System.Collections.Generic.IReadOnlyDictionary<string, Type> map)
						return HowToCombineVals switch
							{
								ValCombinationRules.paramReuse
									=> map.Select(
										curKey
											=> curKey.Key == null || curKey.Value == null
												? System.Array.Empty<string>()
												: HowToMakeKeyValPairStrings switch
													{
														KeyValCombinationRules.semiColon
															=> [ParamToUse, $"{curKey.Key};{curKey.Value}"],

														KeyValCombinationRules.colon
															=> [ParamToUse, $"{curKey.Key}:{curKey.Value}"],

														KeyValCombinationRules.equalSign
															=>  [ParamToUse, $"{curKey.Key}={curKey.Value}"],
														_
															=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<KeyValCombinationRules>(HowToMakeKeyValPairStrings,
																@"While combining a key and value into a single string"),
													}
										).SelectMany(
											curParamList
												=> curParamList
										),

								ValCombinationRules.colon or ValCombinationRules.semiColon
									=> [],

									_
										=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<ValCombinationRules>(HowToCombineVals, @"While selecting how to combine " +
											@"values")
								};
					if(typeCurVal is System.Collections.Generic.IEnumerable<Type> enumerable)
						return HowToCombineVals switch
							{
								ValCombinationRules.paramReuse
									=> enumerable.Select(
											curVal
												=> curVal == null
													? []
													: new string[]{ParamToUse, curVal.ToString() ?? ""}
										).SelectMany(
											curParamList
											=> curParamList
										),

								ValCombinationRules.semiColon or ValCombinationRules.colon or ValCombinationRules.slash
									=> [],

								_
									=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<ValCombinationRules>(HowToCombineVals, @"While formatting a string for a multiple"
										+ " value parameter"),
							};

					if(MultipleValTextLookUp != null)
					{
						System.Collections.Generic.IEnumerable<string>? textValList = MultipleValTextLookUp(typeCurVal);

						return textValList == null
							? []
							: [ParamToUse, ..textValList];
					}

					string strValText = "";
					if(IsDefaulted || typeCurVal is IOneOptVal)
						strValText = ((IOneOptVal)typeCurVal).ValText ?? string.Empty;

					if(ValTextLookUp != null)
						strValText = ValTextLookUp(typeCurVal) ?? string.Empty;

					if(strValText == string.Empty && ValTextLookUp != null)
						strValText = ValTextLookUp(typeCurVal) ?? string.Empty;

					return strValText == string.Empty ? [] : [ParamToUse, strValText];
				}
			}

			/// <summary>
			/// Lets you specify a method that generates the strings to use.  It should return either an empty list or <see langword="null"/> if no parameters are needed.  This is commonly needed if multiple values are needed.
			/// </summary>
			public System.Func<OneOpt<Type>, System.Collections.Generic.IEnumerable<string>>? ParamListGenerator
			{
				get;

				internal set;
			} = null;
		}

		/// <summary>
		/// This is the same as <see cref="OneOpt{Type}"/>, but adds a prefix field.
		/// </summary>
		/// <typeparam name="Type">The type of the value.  Don’t use <c><see langword="bool"/>?</c>.  Instead, use <see cref="ThreeWayOpt"/>.</typeparam>
		/// <param name="typeDef">Specifies the default value for this option.  It’s used by <see cref="OneOpt{Type}.IsDefaulted"/>.</param>
		/// <param name="strParamName">Specifies the parameter name.  The text is used verbatim.  Use a raw string.</param>
		/// <param name="strValPrefix">Specifies the prefix used.  Use a raw string.</param>
		/// <remarks>
		///		<para>Some yt-dlp options such as --paths can be repeated with different prefixes.  So on a normal command line you might have "--paths home:%(title)s.%(ext)s --paths temp:c:\users\username\AppData\Roaming\yt-dlp\Temp".  In that string, <c>"temp"</c> and <c>"home"</c> are prefixes.</para>
		///		<para>In <see cref="AllGlobalOpts"/>, those will be exposed as separate instances of <see cref="OneOptWithPrefix{Type}"/>.</para>
		/// </remarks>
		public class OneOptWithPrefix<Type>(in Type typeDef, in string strParamName, in string strValPrefix)
			: OneOpt<Type>(typeDef, strParamName)
		{
			/// <summary>
			/// The prefix needed for this instance of <see cref="OneOptWithPrefix{Type}"/>.  Options might be duplicated for each option so the prefixes might be
			/// different.
			/// </summary>
			public readonly string strValPrefix = strValPrefix;

			/// <inheritdoc/>
			public override System.Collections.Generic.IEnumerable<string> Params
				=> IsDefaulted
					? []
					: [strParamName, $"{strValPrefix}:{CurVal}"];
		}

		/// <summary>
		/// Specialized <see cref="IOneOpt"/> implementation for use when an option has three states: On, Off, and Defaulted.  It’s implemented as a <c><see langword="bool"/>?</c> such that <see langword="true"/> represents On, <see langword="false"/> represents Off, and <see langword="null"/> represents the state in which we don't pass parameters.  The default value is always <see langword="null"/>.
		/// </summary>
		/// <param name="strTrueParamName">The parameter name to use when <see cref="Val"/> is <see langword="true"/>.  Use a raw string.  No sanity checks are performed and the value is used verbatim.</param>
		/// <param name="strFalseParamName">The parameter name to use when <see cref="Val"/> is <see langword="false"/>.  Use a raw string.  No sanity checks are performed and the value is used verbatim.</param>
		public class ThreeWayOpt(in string strTrueParamName, string strFalseParamName)
			: IOneOpt
		{
			/// <summary>
			/// Stores the value in this option.  Non-members should access it with <see cref="Val"/>.
			/// </summary>
			private bool? bVal = null;

			/// <summary>
			/// The name of the parameter used if the value is <see langword="true"/>.
			/// </summary>
			public readonly string strTrueParamName = strTrueParamName;

			/// <summary>
			/// The name of the parameter used if the value is <see langword="false"/>.
			/// </summary>
			public readonly string strFalseParamName = strFalseParamName;


			/// <summary>
			/// Gets or sets the value.  Implemented as a property in case we need to add some sanity checks.
			/// </summary>
			public bool? Val
			{
				get
					=> bVal;

				set
					=> bVal = value;
			}

			/// <summary>
			/// Returns <see langword="true"/> if the value is <see langword="null"/> and <see langword="false"/> otherwise.
			/// </summary>
			public bool IsDefaulted
				=> bVal == null;

			/// <summary>
			/// Returns a list of parameters that are needed.  If <see cref="IsDefaulted"/> is <see langword="true"/>, this will always be an empty array.  Otherwise, an array containing just either <see cref="strFalseParamName"/> or <see cref="strTrueParamName"/> will be returned.
			/// </summary>
			public System.Collections.Generic.IEnumerable<string> Params
				=> bVal switch
					{
						true
							=> [strTrueParamName],

						false
						 => [strFalseParamName],

						null
							=> [],
					};
		}

		/// <summary>
		/// Implementation of <see cref="IOneOptVal"/> that represents two lengths of time, both from the start of a video.  THe first is the start of a range.  The second represents the end of the range and is optional.  All ranges in yt-dlp must have a start.  yt-dlp interprets a range without an end as the rest of the video.  The range is always output in the form "hours:minutes[-hours:minutes]".  So you’ll always get strings from <see cref="ValText"/> looking like either "2:30" or "2:30-3:11".
		/// </summary>
		public record TimeRangeVal 
			: IOneOptVal
		{
			/// <summary>
			/// Stores the starting index.  If <see cref="tsStart"/> is <see langword="null"/>, <see cref="ValText"/> will return <see langword="null"/> preventing the associated option from emitting any parameters.  Always set this with <see cref="Start"/>.
			/// </summary>
			private System.TimeSpan? tsStart = null;

			/// <summary>
			/// Stores the optional ending index.  If <see cref="tsEnd"/> is <see langword="null"/>, <see cref="ValText"/> might still return a string containing the starting index.  Always set this with <see cref="End"/>.
			/// </summary>
			private System.TimeSpan? tsEnd = null;

			/// <summary>
			/// Gets or sets the starting index of the range.  If <see cref="Start"/> is <see langword="null"/>, <see cref="ValText"/> will return <see langword="null"/> preventing the associated option from emitting parameters.  Setting <see cref="Start"/> to <see langword="null"/> also sets <see cref="End"/> to <see langword="null"/>.  If <see cref="End"/> is less than the value you set <see cref="Start"/> to, the values will be swapped.
			/// </summary>
			public System.TimeSpan? Start
			{
				get
					=> tsStart;

				set
				{
					if(value == null && tsEnd != null)
						tsEnd = null;
					else if(tsEnd >= value)
						tsStart = value;
					else
					{
						tsStart = tsEnd;
						tsEnd = value;
					}
				}
			}

			/// <summary>
			/// Gets or sets the ending index of the range.  If <see cref="End"/> is <see langword="null"/>, <see cref="ValText"/> may still return a string if <see cref="Start"/> has a value.  Setting <see cref="Start"/> to <see langword="null"/> also sets <see cref="End"/> to <see langword="null"/>.  If <see cref="Start"/> is greater than the value you set <see cref="End"/> to, the values will be swapped.
			/// </summary>
			public System.TimeSpan? End
			{
				get
					=> tsEnd;

				set
				{
					if(value != null && tsStart == null)
						throw new System.InvalidOperationException("If Start is null, so must End.");

					if(value == null)
						tsEnd = null;
					else if(tsStart <= value)
						tsEnd = value;
					else
					{
						tsEnd = tsStart;
						tsStart = value;
					}
				}
			}

			/// <summary>
			/// Converts the range into a string as expected by yt-dlp.
			/// </summary>
			public string? ValText
				=> tsStart == null
					? null
					: tsEnd == null
						? ((System.TimeSpan)tsStart).ToString("h:m")
						: $"{(System.TimeSpan)tsStart:h:m}-{(System.TimeSpan)tsEnd:h:m}";


			/// <summary>
			/// Specifies a <see cref="TimeRangeVal"/> that is invalid.  It’s treated as a signal to switch parameters to disable any time range from the config file.
			/// </summary>
			public static readonly TimeRangeVal invalid = new();
		}

		/// <summary>
		/// Abstract class describing how long to sleep between retries.
		/// </summary>
		/// <param name="strPrefix">The prefix to be used.  This works like <see cref="OneOptWithPrefix{Type}.strValPrefix"/>.</param>
		public abstract record BaseRetrySleepSpecVal(in string strPrefix)
			: IOneOptVal
		{
			/// <summary>
			/// Stores the prefix for this <see cref="BaseRetrySleepSpecVal"/> instance.
			/// </summary>
			public readonly string strPrefix = strPrefix;


			/// <summary>
			/// Returns the value of <see cref="ValTextCore"/> with the prefix in front.
			/// </summary>
			public string? ValText
				=> ValTextCore is null
					? null
					: $"{strPrefix}:{ValTextCore}";


			/// <summary>
			/// In derived classes, returns the core value as text.
			/// </summary>
			protected abstract string ValTextCore
			{
				get;
			}
		}

		/// <summary>
		/// Implementation of <see cref="BaseRetrySleepSpecVal"/> that stores how long to sleep as a simple number.
		/// </summary>
		/// <param name="strPrefix">The prefix to use</param>
		public record RetrySleepNumSpecVal(in string strPrefix)
			: BaseRetrySleepSpecVal(strPrefix)
		{
			/// <summary>
			/// The value to store.
			/// </summary>
			public uint uiVal = 1;


			/// <inheritdoc/>
			protected override string ValTextCore
				=> uiVal.ToString();
		}

		/// <summary>
		/// Implementation of <see cref="BaseRetrySleepSpecVal"/> that describes how long to sleep such that the time in which yt-dlp sleep increases linearly.
		/// </summary>
		/// <param name="strPrefix">The prefix to use.</param>
		public record RetrySleepLinearSpecVal(in string strPrefix)
			: BaseRetrySleepSpecVal(strPrefix)
		{
			/// <summary>
			/// Set to the desired start point.  This is mandatory and defaults to <c>1</c>.
			/// </summary>
			public uint uiStart = 1;

			/// <summary>
			/// Set to the desired end point.  <see cref="uiEnd"/> is optional.  If you leave this at <see langword="null"/>, the sleep time will increase forever.
			/// </summary>
			public uint? uiEnd = null;

			/// <summary>
			/// Set to the desired step.  Note: The default value of <see langword="null"/> may cause the sleep time to behave as though you specified a <see cref="RetrySleepNumSpecVal"/> rather than a <see cref="RetrySleepLinearSpecVal"/>.
			/// </summary>
			public uint? uiStep = null;


			/// <inheritdoc/>
			protected override string ValTextCore
				=> uiEnd == null && uiStep == null
					? $"linear={uiStart}"
					: uiStep == null
						? $"linear={uiStart}:{uiEnd}"
						: uiEnd == null
							? $"linear={uiStart}::{uiStep}"
							: $"linear={uiStart}:{uiEnd}:{uiStep}";
		}

		/// <summary>
		/// Implementation of <see cref="BaseRetrySleepSpecVal"/> that describes a sleep that increases in an exponential manner.
		/// </summary>
		/// <param name="strPrefix">The prefix to use</param>
		public record RetrySleepExpSpecVal(in string strPrefix)
			: BaseRetrySleepSpecVal(strPrefix)
		{
			/// <summary>
			/// The starting value.  This is required.
			/// </summary>
			public uint uiStart = 1;

			/// <summary>
			/// When to stop increasing the sleep time.  The default value of <see langword="null"/> means the time never stops increasing.
			/// </summary>
			public uint? uiEnd = null;

			/// <summary>
			/// Controls how fast the sleep time increases.  A higher base results in a faster exponent.  If you leave this at the default value of <see langword="null"/>, yt-dlp will select a value for you.
			/// </summary>
			public uint? uiBase = null;


			/// <inheritdoc/>
			protected override string ValTextCore
				=> uiEnd == null && uiBase == null
					? $"exp={uiStart}"
					: uiBase == null
						? $"exp={uiStart}:{uiEnd}"
						: uiEnd == null
							? $"exp={uiStart}::{uiBase}"
							: $"exp={uiStart}:{uiEnd}:{uiBase}";
		}

		/// <summary>
		/// Declares a list of Internet link file choices.  These are all incompatible standards.
		/// </summary>
		public enum InternetLinkFileTypeChoices
		{
			/// <summary>
			/// Rely on any config files or yt-dlp's default of never.
			/// </summary>
			@default,

			/// <summary>
			/// Choose a file type based on the OS that yt-dlp is running on
			/// </summary>
			native,

			/// <summary>
			/// A URL file would be written.
			/// </summary>
			windows,

			/// <summary>
			/// File type supported by MacOS
			/// </summary>
			webloc,

			/// <summary>
			/// File type supported by Linux
			/// </summary>
			desktop,
		}

		/// <summary>
		/// Specialized <see cref="IOneOpt"/> implementation that describes a range of integers.  Implemented around <see langword="ushort"/>.
		/// </summary>
		/// <param name="strMinParam">The name of the parameter corresponding to the minimum value.</param>
		/// <param name="strMaxParam">The name of the parameter corresponding to the maximum value.</param>
		/// <remarks>
		/// <para>If you set <see cref="Min"/> to a value greater than <see cref="Max"/>, it will swap those values.  Ditto if you set <see cref="Max"/> to a value greater than <see cref="Min"/>.  You can set <see cref="Min"/> and/or <see cref="Max"/> while leaving the other <see langword="null"/>.</para>
		/// </remarks>
		public record UShortRangeOpt(in string strMinParam, in string strMaxParam)
			: IOneOpt
		{
			/// <summary>
			/// Stores the minimum.  Set this with <see cref="Min"/>.
			/// </summary>
			private ushort? usMin = null;

			/// <summary>
			/// Stores the maximum.  Set this with <see cref="Max"/>.
			/// </summary>
			private ushort? usMax = null;


			/// <summary>
			/// The parameter to use when <see cref="Min"/> is non-<see langword="null"/>.
			/// </summary>
			public readonly string strMinParam = strMinParam;

			/// <summary>
			/// The parameter to use when <see cref="Max"/> is non-<see langword="null"/>.
			/// </summary>
			public readonly string strMaxParam = strMaxParam;


			/// <summary>
			/// The minimum value.  The minimum and <see cref="Max"/> will be swapped if needed.
			/// </summary>
			public ushort? Min
			{
				get
					=> usMin;

				set
				{
					if(value == null && usMax != null)
						usMax = null;
					else if(usMax >= value)
						usMin = value;
					else
					{
						usMin = usMax;
						usMax = value;
					}
				}
			}

			/// <summary>
			/// The maximum value.  The maximum and <see cref="Min"/> will be swapped if needed.
			/// </summary>
			public ushort? Max
			{
				get
					=> usMax;

				set
				{
					if(value != null && usMin == null)
						throw new System.InvalidOperationException("If Start is null, so must End.");

					if(value == null)
						usMax = null;
					else if(usMin <= value)
						usMax = value;
					else
					{
						usMax = usMin;
						usMin = value;
					}
				}
			}

			/// <inheritdoc/>
			public System.Collections.Generic.IEnumerable<string> Params
				=> usMin == null && usMax == null
					? []
					: usMin != null && usMax == null
						? [strMinParam, ((ushort)usMin).ToString()]
						: usMin == null && usMax != null
							? [strMaxParam, ((ushort)usMax).ToString()]
							: usMin != null && usMax != null
								? [strMinParam, ((ushort)usMin).ToString(), strMaxParam, ((ushort)usMax).ToString()]
								: [];
		}


		/// <summary>
		/// Used as an additional value similar to <see langword="null"/>.  Some options may output this as <c>'-'</c>.
		/// </summary>
		public static readonly System.IO.FileInfo fileInvalid = new("invalid");

		/// <summary>
		/// Used as an additional value similar to <see langword="null"/>.  Some options may output this as <c>'-'</c>.
		/// </summary>
		public static readonly System.IO.DirectoryInfo dirInvalid = new("invalid");

		/// <summary>
		/// Used as an additional value similar to <see langword="null"/>.  Some options may output this as <c>'-'</c>.
		/// </summary>
		public static readonly System.Text.RegularExpressions.Regex regexInvalid = new(string.Empty);


		/// <summary>
		/// Stores all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#general-options">General options section of yt-dlp’s readme.md</a>.
		/// </summary>
		[System.ComponentModel.ImmutableObject(true)]
		public sealed class GeneralGroup
		{
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
			} = new(false, @"--ignore-errors");

			/// <summary>
			/// If <see langword="false"/>, Continue with next video on download errors; e.g. to skip unavailable videos in a playlist.  If <see langword="true"/>, abort downloading of further videos if an error occurs.  If <see langword="null"/>, the system acts as though you used <see langword="false"/>, but no parameter will be passed to yt-dlp.  However, the default value of <see langword="null"/> might allow a config file might cause yt-dlp to act as though this were <see langword="true"/>.  A <see langword="true"/> or <see langword="false"/> value overrides the config file while <see langword="null"/> doesn't override the config file.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strGeneralAbortOnError), @"If false, Continue with next video on download errors; e.g. to skip unavailable videos in a playlist; If true, abort downloading of further videos if an error occurs; If null, the system acts as though you used false, but no parameter will be passed to yt-dlp.  However, the default value of null might allow a config file might cause yt-dlp to act as though this were true.  A true or false value overrides the config file while null doesn't override the config file.", typeof(GeneralGroup), @"--abort-on-error",
				@"--no-abort-on-error")]
			public ThreeWayOpt AbortOnError
			{
				get;
			} = new(@"--abort-on-error", @"--no-abort-on-error");

			/// <summary>
			/// Extractor names to use separated by commas.  You can also use regular expressions, “all”, “default”, “end” (end URL matching) like <c>"holodex.*,end,youtube"</c>.  Prefix the name with a “-” to exclude it.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strGeneralUseExtractors), @"Extractor names to use separated by commas.  You can also use regular expressions, “all”,"
				+ "“default”, “end” (end URL matching) like \"holodex.*,end,youtube\".  Prefix the name with a “-” to exclude it.", typeof(GeneralGroup),
				@"--use-extractors")]
			public OneOpt<System.Collections.Generic.IEnumerable<string>> UseExtractors
			{
				get;
			} = new([], @"--use-extractors");

			/// <summary>
			/// Use this prefix for unqualified URLs. E.g. <c>"gvsearch2:python"</c> downloads two videos from Google videos for the search term “python”.  Use the value “auto” to let yt-dlp guess.  To emit a warning when yt-dlp guesses, use “auto_warning”.  “error” just throws an error.  “fixup_error” repairs broken URLs, but emits an error if this is not possible instead of searching.  If DefSearchPrefix is empty and no config file specifies --default-search, yt-dlp acts as though DefSearchPrefix contains “fixup_error”.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strGeneralDefSearchPrefix), @"Use this prefix for unqualified URLs. E.g. ""gvsearch2:python"" downloads two videos from Google videos for the search term “python”.  Use the value “auto” to let yt-dlp guess.  To emit a warning when yt-dlp guesses, use “auto_warning”.  “error” just throws an error.  “fixup_error” repairs broken URLs, but emits an error if this is not possible instead of searching.  If DefSearchPrefix is empty and no config file specifies --default-search, yt-dlp acts as though DefSearchPrefix contains “fixup_error”.",
				typeof(GeneralGroup), @"--default-search")]
			public OneOpt<string> DefSearchPrefix
			{
				get;
			} = new("", @"--default-search");

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
			} = new(@"--flat-playlist", @"--no-flat-playlist");

			/// <summary>
			/// Download live streams from the start.  Currently only supported for YouTube (Experimental).  Use <see langword="true"/> to turn this on.  Use <see langword="false"/> to force it to be off.  Use <see langword="null"/> to use the default value (<see langword="false"/>) unless your config file specifies it in which case, that value will be used.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strGeneralLiveFromStart), @"Download live streams from the start.  Currently only supported for YouTube (Experimental).  Use true to turn this on.  Use false to force it to be off.  Use null to use the default value (false) unless your config file specifies it in which case, that value will be used.", typeof(GeneralGroup), @"--live-from-start", @"--no-live-from-start")]
			public ThreeWayOpt LiveFromStart
			{
				get;
			} = new(@"--live-from-start", @"--no-live-from-start");

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
								: throw new System.InvalidProgramException(@"How did a non-time range get in here?")
			};

			/// <summary>
			/// Mark videos watched (even in simulate mode).  Use <see langword="null"/> to get the default value which acts like a <see langword="false"/> value unless this is set in a config file.  Use <see langword="true"/> to force it on and <see langword="false"/> to force it off.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strGeneralMarkWatched), @"Mark videos watched (even in simulate mode).  Use null to get the default value which acts like a false value unless this is set in a config file.  Use true to force it on and false to force it off.", typeof(GeneralGroup),
				@"--mark-watched", @"--no-mark-watched")]
			public ThreeWayOpt MarkWatched
			{
				get;
			} = new(@"--mark-watched", @"--no-mark-watched");

			/// <summary>
			/// Whether to emit color codes in the output stream stdout.  Can be one of <see cref="ColorPoliciesForStreams.always"/>, <see cref="ColorPoliciesForStreams.auto"/> (default), <see cref="ColorPoliciesForStreams.never"/>, or <see cref="ColorPoliciesForStreams.noColor"/> (use non color terminal sequences).  Use <see cref="ColorPoliciesForStreams.autoTTY"/> or <see cref="ColorPoliciesForStreams.noColorTTY"/> to decide based on terminal support only.  For the stderr equivalent, see <see cref="ColorWhenInStdErr"/>.  <see cref="ColorPoliciesForStreams.@default"/> prevents a parameter from being emitted allowing any value from a config file or yt-dlp’s default to take effect.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strGeneralColorWhenStdOut), @"Whether to emit color codes in the output stream stdout.  Can be one  of ColorPoliciesForStreams.always, ColorPoliciesForStreams.auto (default), ColorPoliciesForStreams.never, or ColorPoliciesForStreams.noColor (use non color terminal sequences).  Use ColorPoliciesForStreams.autoTTY or ColorPoliciesForStreams.noColorTTY to decide based on terminal support only.  For the stderr equivalent, see ColorWhenInStdErr.  ColorPoliciesForStreams.@default prevents a parameter from being emitted allowing any value from a config file or yt-dlp’s default to take effect.", typeof(GeneralGroup), @"--color")]
			public OneOptWithPrefix<ColorPoliciesForStreams> ColorWhenInStdOut
			{
				get;
			} = new(ColorPoliciesForStreams.@default, @"--color", @"stdout");

			/// <summary>
			/// Whether to emit color codes in the output stream stderr.  Can be one of <see cref="ColorPoliciesForStreams.always"/>, <see cref="ColorPoliciesForStreams.auto"/> (default), <see cref="ColorPoliciesForStreams.never"/>, or <see cref="ColorPoliciesForStreams.noColor"/> (use non color terminal sequences).  Use <see cref="ColorPoliciesForStreams.autoTTY"/> or <see cref="ColorPoliciesForStreams.noColorTTY"/> to decide based on terminal support only.  For the stdout equivalent, see <see cref="ColorWhenInStdOut"/>.  <see cref="ColorPoliciesForStreams.@default"/> prevents a parameter from being emitted allowing any value from a config file or yt-dlp’s default to take effect.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strGeneralColorWhenStdErr), @"Whether to emit color codes in the output stream stderr.  Can be one  of ColorPoliciesForStreams.always, ColorPoliciesForStreams.auto (default), ColorPoliciesForStreams.never, or ColorPoliciesForStreams.noColor (use non color terminal sequences).  Use ColorPoliciesForStreams.autoTTY or ColorPoliciesForStreams.noColorTTY to decide based on terminal support only.  For the stdout equivalent, see ColorWhenInStdOut.  ColorPoliciesForStreams.@default prevents a parameter from being emitted allowing any value from a config file or yt-dlp's default to take effect.", typeof(GeneralGroup), @"--color")]
			public OneOptWithPrefix<ColorPoliciesForStreams> ColorWhenInStdErr
			{
				get;
			} = new(ColorPoliciesForStreams.@default, @"--color", @"stderr");


			/// <summary>
			/// Stores a list of all options for <see cref="GeneralGroup"/>/<see cref="General"/>.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
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

		/// <summary>
		/// Groups all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#network-options">Network Options section of yt-dlp’s readme.md</a>.
		/// </summary>
		[System.ComponentModel.ImmutableObject(true)]
		public sealed class NetGroup
		{
			/// <summary>
			/// Lists your options for which type of IP address to use.  The only valid values are static readonly members of <see cref="IpAddrModeVal"/>.
			/// </summary>
			[System.ComponentModel.ImmutableObject(true)]
			public sealed class IpAddrModeVal
			{
				/// <summary>
				/// Constructs new instances.
				/// </summary>
				/// <param name="strParamName">The name of the associated parameter.</param>
				private IpAddrModeVal(in string? strParamName)
					=> ParamName = strParamName;


				/// <summary>
				/// Stores the name of the parameter
				/// </summary>
				public string? ParamName
				{
					get;
				}


				/// <summary>
				/// Force yt-dlp to use IPv4.
				/// </summary>
				public static readonly IpAddrModeVal v4Only = new("-4");

				/// <summary>
				/// Force yt-dlp to use IPv6
				/// </summary>
				public static readonly IpAddrModeVal v6Only = new("-6");

				/// <summary>
				/// Let yt-dlp choose.
				/// </summary>
				public static readonly IpAddrModeVal anyIpVersion = new(null);
			}


			/// <summary>
			/// Specifies a URI that's invalid so <see cref="ProxyURL"/> can have multiple values that are special.  See the documentation on it for more information.
			/// </summary>
			public static readonly System.Uri uriInvalid = new(@"http://invalid");


			/// <summary>
			/// Use the specified HTTP/HTTPS/SOCKS proxy.  To enable SOCKS proxy, specify a proper scheme, e.g. <c>"socks5://user:pass@127.0.0.1:1080/"</c>.   <see langword="null"/> prevents a parameter from being emitted.  The value in <see cref="uriInvalid"/> gives you a direct connection.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strNetProxyURL), @"Use the specified HTTP/HTTPS/SOCKS proxy.  To enable SOCKS proxy, specify a proper scheme,  e.g. socks5://user:pass@127.0.0.1:1080/.  Null prevents a parameter from being emitted.  The value in uriInvalid gives you a direct connection", typeof(NetGroup), @"--proxy")]
			public OneOpt<System.Uri?> ProxyURL
			{
				get;
			} = new(null, @"--proxy")
			{
				ParamListGenerator =
					opt
						=>
							opt.CurVal == null
								? []
								: opt.CurVal == uriInvalid
									? [string.Empty]
									: [opt.CurVal.AbsoluteUri],
			};

			/// <summary>
			/// Time to wait before giving up.  The value null prevents this parameter from being emitted.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strNetSocketTimeOut), @"Time to wait before giving up.  The value null prevents this parameter from being emitted.", typeof(NetGroup), @"--socket-timeout")]
			public OneOpt<System.TimeSpan?> SocketTimeOut
			{
				get;
			} = new(null, @"--socket-timeout")
			{
				ValTextLookUp =
					curVal
						=> curVal?.TotalSeconds.ToString(),
			};

			/// <summary>
			/// Client-side IP address to bind to.  The value null prevents this parameter from being emitted.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strNetIpAddr), @"Client-side IP address to bind to.  The value null prevents this parameter from being emitted.", typeof(NetGroup), @"--socket-address")]
			public OneOpt<System.Net.IPAddress?> SocketIpAddr
			{
				get;
			} = new(null, @"--socket-address")
			{
				CustomParamNameLookUp =
					curVal
						=> curVal?.ToString() ?? "",
			};

			/// <summary>
			/// Use to select between IP v4 and IP v6.  Get values from <see cref="IpAddrModeVal"/>.  Use <see cref="IpAddrModeVal.v4Only"/> to force IP version 4. Use <see cref="IpAddrModeVal.v6Only"/> to force IP version 6.  Use  <see cref="IpAddrModeVal.anyIpVersion"/> to let yt-dlp select a mode.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strNetIPAddrMode), @"Use to select between IP v4 and IP v6.  Get values from IpAddressModeOpt.  Use IpAddressModeOpt.v4 to force IP version 4.  Use IpAddressModeOpt.v6 to force IP version 6.  Use IpAddressModeOpt.anyIpVersion to let yt-dlp select a mode.", typeof(NetGroup), @"--force-ipv4", @"--force-ipv6")]
			public OneOpt<IpAddrModeVal> IpAddrMode
			{
				get;
			} = new(IpAddrModeVal.anyIpVersion, string.Empty)
			{
				CustomParamNameLookUp
					= typeCurVal
						=> typeCurVal?.ParamName ?? "",
			};


			/// <summary>
			/// Lists all options inside <see cref="NetGroup"/>.  Parents of this instance use it to merge their list of options into theirs.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					ProxyURL,
					SocketTimeOut,
					SocketIpAddr,
					IpAddrMode,
				];
		}

		/// <summary>
		/// Groups all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#geo-restriction">Geo-Restrictions Options section of yt-dlp’s readme.md</a>.
		/// </summary>
		[System.ComponentModel.ImmutableObject(true)]
		public sealed class GeoRestrictionGroup
		{
			/// <summary>
			/// Use this proxy to verify the IP address for some geo-restricted sites.  The default proxy specified by <see cref="NetGroup.ProxyURL"/> (or none, if the option is  <see langword="null"/>) is used for the actual downloading.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strGeoRestrictionVerificationProxy), @"Use this proxy to verify the IP address for some geo-restricted sites.  The default proxy specified by Net.ProxyURL (or none, if the option is null) is used for the actual downloading.", typeof(GeoRestrictionGroup),  @"--geo-verification-proxy")]
			public OneOpt<System.Uri?> VerificationProxy
			{
				get;

				set;
			} = new(null, @"--geo-verification-proxy");

			/// <summary>
			/// How to fake X-Forwarded-For HTTP header to try bypassing geographic restriction.  One of “default” (only when known to be useful), “never”, an IP block in CIDR notation, or a two-letter ISO 3166-2 country code.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strGeoRestrictionXFF), @"How to fake X-Forwarded-For HTTP header to try bypassing geographic restriction.  One of “default” (only when known to be useful), “never”, an IP block in CIDR notation, or a two-letter ISO 3166-2 country code.", typeof(GeoRestrictionGroup),
				@"--xff")]
			public OneOpt<string> XFF
			{
				get;
			} = new(string.Empty, @"--xff");


			/// <summary>
			/// Lists all options inside <see cref="GeoRestrictionGroup"/>.  The parents of this instance will use <see cref="AllOpt"/> to build their own list of all options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					VerificationProxy,
					XFF,
				];
		}

		/// <summary>
		///  Groups all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#video-selection">Video Selection Options section of yt-dlp’s readme.md</a>.
		/// </summary>
		[System.ComponentModel.ImmutableObject(true)]
		public sealed class VidSelGroup
		{
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
			}

			/// <summary>
			/// Implementation of <see cref="DateWrapperValCore"/> that stores dates in relative form.  If you need to specify an exact date, use <see cref="DateOnlyWrapperValCore"/>.  You need to specify one of several starting points or origins with an instance from <see cref="Origins"/>, a distance, and a scale for that distance with a value form <see cref="DistanceScales"/>.  You can end up with sometime resembling "2 weeks ago yesterday".
			/// </summary>
			/// <param name="Origin">The starting point.  Several values are defined as <see langword="public"/> <see langword="static"/> <see langword="readonly"/> members.</param>
			/// <param name="Distance">How far before the origin specified in <paramref name="Origin"/>.  How yt-dlp interprets <paramref name="Distance"/> is specified by <paramref name="Scale"/>.</param>
			/// <param name="Scale">Specifies how far to go back in time.  If <paramref name="Scale"/> is <see cref="DistanceScales.week"/> and <paramref name="Distance"/> is <c>6</c>, you'll get 6 weeks before the value in <paramref name="Origin"/>.</param>
			public class DateRelativeValWrapperValCore(DateRelativeValWrapperValCore.Origins Origin, ulong Distance, DateRelativeValWrapperValCore.DistanceScales
					Scale)
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
				public ulong Distance
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
			public OneOpt<string?> MinFileSize
			{
				get;
			} = new(null, @"--min-filesize");

			/// <summary>
			/// Abort download if the file size is larger than specified size, e.g. 50k or 44.6M.  A  <see langword="null"/> value causes the value from any config file or the default (no maximum) to be used.  Be careful combining this with <see cref="MinFileSize"/>.  If <see cref="MinFileSize"/> is larger than <see cref="MaxFileSize"/>, no data will be downloaded!
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidSelMaxFileSize), @"Abort download if the file size is larger than specified size, e.g. 50k or 44.6M.  A null value causes the value from any config file or the default (no maximum) to be used.  Be careful combining this with MinFileSize.  If MinFileSize is larger than MaxFileSize, no data will be downloaded!", typeof(VidSelGroup), @"--msc-filesize")]
			public OneOpt<string?> MaxFileSize
			{
				get;
			} = new(null, @"--msc-filesize");

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
			};

			/// <summary>
			/// Generic video filter.  Any <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template field">Output Template Field</a> can be compared with a number or a string using the operators defined in <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#filtering-formats.">Filtering Formats</a>.  You can also simply specify a field to match if the field is present, use “!field” to check if the field is not present, and “&amp;” to check multiple conditions.  Use a “\” to escape “&amp;” or quotes if needed.  If multiple elements are present, the filter matches if at least one of the conditions is met. E.g. <c>["!is_live", "like_count>?100 &amp; description~='(?i)\bcats \&amp; dogs\\b'"]</c> matches only videos that are not live OR those that have a like count more than 100 (or the like field is not available) and also has a description that contains the phrase “cats &amp; dogs” (caseless).  Use <c>[<see langword="null"/>]</c> to interactively ask whether to download each video.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidSelMatchFilter), @"Generic video filter.  Any https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template field can be compared with a number or a string using the operators defined in https://github.com/yt-dlp/yt-dlp/blob/master/README.md#filtering-formats.  You can also simply specify a field to match if the field is present, use “!field” to check if the field is not present, and “&” to check multiple conditions.  Use a “\\” to escape “&” or quotes if needed.  If multiple elements are present, the filter matches if at least one of the conditions is met. E.g. [""!is_live"", ""like_count>?100 & description~='(?i)\bcats \& dogs\b'""] matches only videos that are not live OR those that have a like count more than 100 (or the like field is not available) and also has a description that contains the phrase “cats & dogs” (caseless).  Use [null] to interactively ask whether to download each video.", typeof(VidSelGroup), @"--match-filters")]
			public OneOpt<System.Collections.Generic.IEnumerable<System.Text.RegularExpressions.Regex?>> MatchFilters
			{
				get;
			} = new([], @"--match-filters")
			{
				ParamListGenerator =
					opt
						=> (
								from System.Text.RegularExpressions.Regex? regexCur in opt.CurVal
								select
									new string[]{
										@"--match-filters",
										regexCur is null
											? @"-"
											: regexCur.ToString()
									}
								).SelectMany(
									curParamList
										=> curParamList
							),
				HowToCombineVals = OneOpt<System.Collections.Generic.IEnumerable<System.Text.RegularExpressions.Regex?>>.ValCombinationRules.paramReuse,
			};

			/// <summary>
			/// Generic video filter that stops download when a video is rejected.  Any <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template field">Output Template Field</a> can be compared with a number or a string using the operators defined in <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#filtering-formats.">Filtering Formats</a>.  You can also simply specify a field to match if the field is present, use “!field” to check if the field is not present, and “&amp;” to check multiple conditions.  Use a “\” to escape “&amp;” or quotes if needed.  If multiple elements are present, the filter matches if at least one of the conditions is met. E.g. <c>["!is_live", "like_count>?100 &amp; description~='(?i)\bcats \&amp; dogs\\b'"]</c> matches only videos that are not live OR those that have a like count more than 100 (or the like field is not available) and also has a description that contains the phrase “cats &amp; dogs” (caseless).  Use <c>[<see langword="null"/>]</c> to interactively ask whether to download each video.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidSelBreakMatchFilter), @"Generic video filter that stops downloading when a video is rejected.  Any https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template field can be compared with a number or a string using the operators defined in https://github.com/yt-dlp/yt-dlp/blob/master/README.md#filtering-formats.  You can also simply specify a field to match if the field is present, use “!field” to check if the field is not present, and “&” to check multiple conditions.  Use a “\” to escape “&” or quotes if needed.  If multiple elements are present, the filter matches if at least one of the conditions is met. E.g. [""!is_live"", ""like_count>?100 & description~='(?i)\bcats \& dogs\b'""] matches only videos that are not live OR those that have a like count more than 100 (or the like field is not"
				+ @"available) and also has a description that contains the phrase “cats & dogs” (caseless).  Use [null] to interactively ask whether to download each "
				+ "video.", typeof(VidSelGroup), @"--break-match-filters")]
			public OneOpt<System.Collections.Generic.IEnumerable<System.Text.RegularExpressions.Regex?>> BreakMatchFilters
			{
				get;
			} = new([], @"--break-match-filters")
			{
				ParamListGenerator =
					opt
						=> (
								from System.Text.RegularExpressions.Regex? regexCur in opt.CurVal
								select
									new string[]
									{
										@"--break-match-filters",
										regexCur is null
											? @"-"
											: regexCur.ToString()
									}
								).SelectMany(
									curParamList
										=> curParamList
							),
				HowToCombineVals = OneOpt<System.Collections.Generic.IEnumerable<System.Text.RegularExpressions.Regex?>>.ValCombinationRules.paramReuse,
			};

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
							}
			};

			/// <summary>
			/// Download only videos suitable for the given age.  The default value  <see langword="null"/> to prevents this parameter from being emitted.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidSelAgeLimit), @"Download only videos suitable for the given age.  The default value null to prevents this parameter from being emitted.", typeof(VidSelGroup), @"--age-limit")]
			public OneOpt<int?> AgeLimit
			{
				get;

				private init;
			} = new(null, @"--age-limit");

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
					=> opt.CurVal == null
						? []
						: opt.CurVal == fileInvalid
							? [@"--no-download-archive"]
							: [opt.strParamName, opt.CurVal.FullName],
			};

			/// <summary>
			/// If <see langword="true"/>, stops the download process when encountering a file that is in the archive supplied with <see cref="DownLoadArchive"/>.  If <see langword="false"/>, the download continues.  Use the default value of  <see langword="null"/> to get the value from the config.  That will prevent a parameter from being emitted.  If yt-dlp still doesn't find a value, it acts as though you specified <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidSelBreakOnExisting), @"If true, stops the download process when encountering a file that is in the archive supplied with DownloadArchive.  If false, the download continues.  Use the default value of null to get the value from the config.  That will prevent a parameter from being emitted.  If yt-dlp still doesn't find a value, it acts as though you specified false.", typeof(VidSelGroup),
				@"--break-on-existing", @"--no-break-on-existing")]
			public ThreeWayOpt BreakOnExisting
			{
				get;
			} = new(@"--break-on-existing", @"--no-break-on-existing");

			/// <summary>
			/// If <see langword="true"/>, alters <see cref="DownLoadsGroup.MaxDownLoads"/>, <see cref="BreakOnExisting"/>, <see cref="BreakMatchFilters"/>, and auto-number in file templates to reset per input URL.  If <see langword="false"/>, those behave normally.  The default value of <see langword="null"/> causes yt-dlp to check the config files.  If no value is found there, it acts like you used <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidSelBreakPerInput), @"If true, alters Downloads.MaxDownloads, BreakOnExisting, BreakMatchFilters, and auto-number in file templates to reset per input URL.  If false, those behave normally.  The default value of null causes yt-dlp to check the config files.  If no value is found there, it acts like you used false.", typeof(VidSelGroup), @"--break-per-input", @"--no-break-per-input")]
			public ThreeWayOpt BreakPerInput
			{
				get;
			} = new(@"--break-per-input", @"--no-break-per-input");

			/// <summary>
			/// Specifies the number of allowed failures until the rest of the playlist is skipped.  Use the default value of <see langword="null"/> to have yt-dlp check the config file for this value.  By default, it continues to download the playlist even with errors.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidSelSkipPlayListAfterErrors), @"Specifies the number of allowed failures until the rest of the playlist is skipped.  Use the default value of null to have yt-dlp check the config file for this value.  By default, it continues to download the playlist even with errors.", typeof(VidSelGroup), @"--skip-playlist-after-errors")]
			public OneOpt<uint?> SkipPlayListAfterErrors
			{
				get;
			} = new(null, @"--skip-playlist-after-errors");


			/// <summary>
			/// Returns a list of all options inside <see cref="VidSelGroup"/>.  The parent of this instance will use them to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					MinFileSize,
					MaxFileSize,
					ByDate,
					MatchFilters,
					BreakMatchFilters,
					PlayListOrVidSel,
					AgeLimit,
					DownLoadArchive,
					BreakOnExisting,
					BreakPerInput,
					SkipPlayListAfterErrors,
				];
		}

		/// <summary>
		/// Groups all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#download-options">Downloads Options section of yt-dlp’s readme.md</a>.
		/// </summary>
		[System.ComponentModel.ImmutableObject(true)]
		public sealed class DownLoadsGroup
		{
			/// <summary>
			/// Lists all the downloaders that yt-dlp supports as of March 7th, 2025.  You must choose one of the <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c>.
			/// </summary>
			public sealed class SupportedDownLoaders : IOneOptVal
			{
				/// <summary>
				/// Constructs a new instance
				/// </summary>
				/// <param name="strName">The name of the downloader</param>
				private SupportedDownLoaders(in string strName)
					=> this.strName = strName;


				/// <summary>
				/// The name of the downloader
				/// </summary>
				public readonly string strName;


				/// <summary>
				/// Use the aria2c downloader
				/// </summary>
				public static readonly SupportedDownLoaders aria2c = new(@"aria2c");

				/// <summary>
				/// Use the avconv downloader
				/// </summary>
				public static readonly SupportedDownLoaders avconv = new(@"avcov");

				/// <summary>
				/// Use the axel downloader
				/// </summary>
				public static readonly SupportedDownLoaders axel = new(@"axel");

				/// <summary>
				/// Use the curl downloader
				/// </summary>
				public static readonly SupportedDownLoaders curl = new(@"curl");

				/// <summary>
				/// Use ffmpeg for downloading
				/// </summary>
				public static readonly SupportedDownLoaders ffmpeg = new(@"ffmpeg");

				/// <summary>
				/// Use the httpie downloader
				/// </summary>
				public static readonly SupportedDownLoaders httpie = new(@"httpie");

				/// <summary>
				/// Use the wget downloader
				/// </summary>
				public static readonly SupportedDownLoaders wget = new(@"wget");

				/// <summary>
				/// Use the native downloader
				/// </summary>
				public static readonly SupportedDownLoaders native = new(@"native");


				/// <summary>
				/// Returns the value as a <see cref="string"/>.
				/// </summary>
				public string? ValText
					=> strName;
			}

			/// <summary>
			/// Groups some download options that involve retries
			/// </summary>
			public sealed class RetryMaxGroup
			{
				/// <summary>
				/// Maximum number of retries allowed before yt-dlp gives up.  If it doesn't have any value, either from here or a config file, yt-dlp assumes 10.  If Retries is left at the default value of <see langword="null"/> and no config file specifies --max-retries, yt-dlp will retry the file 10 times.  Use <see cref="uint.MaxValue"/> to specify that yt-dlp should retry forever.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadMaxRetries), @"Maximum number of retries allowed before yt-dlp gives up.  If it doesn't have any value, either from here or a config file, yt-dlp assumes 10.  If Retries is left at the default value of null and no config file specifies --max-retries, yt-dlp will retry the file 10 times.  Use uint.MaxValue to specify that yt-dlp should retry forever.", typeof(DownLoadsGroup),
					@"--retries")]
				public OneOpt<uint?> Retries
				{
					get;
				} = new(null, @"--retries")
				{
					ValTextLookUp =
						uiCurVal
							=> uiCurVal switch
							{
								null
									=> null,

								uint.MaxValue
									=> "infinite",

								_
									=> uiCurVal.ToString(),
							},
				};

				/// <summary>
				/// Number of times to retry in the event of file access errors.  If you leave FileAccess set to <see langword="null"/> and no config file uses --file-access-retries, yt-dlp will act as though FileAccess is 3.  If FileAccess is set to <see cref="uint.MaxValue"/>, yt-dlp will wait for the file to become available.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadMaxFileAccessRetries), @"Number of times to retry in the event of file access errors.  If you leave FileAccess set to null and no config file uses --file-access-retries, yt-dlp will act as though FileAccess is 3.  If FileAccess is set to uint.MaxValue, yt-dlp will wait for the file to become available.", typeof(DownLoadsGroup), @"--file-access-retries")]
				public OneOpt<uint?> FileAccess
				{
					get;
				} = new(null, @"--file-access-retries")
				{
					ValTextLookUp =
						uiCurVal
							=> uiCurVal switch
							{
								null
									=> null,

								uint.MaxValue
									=> "infinite",

								_
									=> uiCurVal.ToString(),
							},
				};

				/// <summary>
				/// Number of times to retry in the event of HTTP errors involving fragments.  If you leave Fragment set to <see langword="null"/> and no config file uses --fragment-retries, yt-dlp will act as though Fragment is 10.  If FragmentRetries is set to <see cref="uint.MaxValue"/>, yt-dlp will wait for the file to become available.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadMaxFragmentRetries), @"Number of times to retry in the event of HTTP errors involving fragments.  If you leave Fragment set to null and no config file uses --fragment-retries, yt-dlp will act as though Fragment is 10.  If FragmentRetries is set to uint.MaxValue, yt-dlp will wait for the file to become available.", typeof(DownLoadsGroup), @"--fragment-retries")]
				public OneOpt<uint?> Fragment
				{
					get;
				} = new(null, @"--fragment-retries")
				{
					ValTextLookUp =
						uiCurVal
							=> uiCurVal switch
							{
								null
									=> null,

								uint.MaxValue
									=> "infinite",

								_
									=> uiCurVal.ToString(),
							},
				};


				/// <summary>
				/// Lists all options inside <see cref="RetryMaxGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to build it’s own list of options.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
					=> [
						Retries,
						FileAccess,
						Fragment,
					];
			}

			/// <summary>
			/// Groups some options that involve sleep.
			/// </summary>
			public sealed class SleepBeforeRetryGroup
			{
				/// <summary>
				/// Time to sleep in seconds between retries on a http resource.  This can be a <see cref="RetrySleepNumSpecVal"/>, a <see cref="RetrySleepLinearSpecVal"/>, or a <see cref="RetrySleepExpSpecVal"/>.  The default value of <see langword="null"/> allows yt-dlp to sleep as needed unless a config file specifies a http sleep.  Linear (<see cref="RetrySleepLinearSpecVal"/>) and Exponential (<see cref="RetrySleepExpSpecVal"/>) structures provide more options.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadHttpRetrySleep), @"Time to sleep in seconds between retries on a http resource.  This can be a RetrySleepNumSpecVal, a RetrySleepLinearSpecVal, or a RetrySleepExpSpecVal.  The default value of null allows yt-dlp to sleep as needed unless a config file specifies a http sleep.  Linear (RetrySleepLinearSpecVal) and Exponential (RetrySleepExpSpecVal) structures provide more options.",
					typeof(DownLoadsGroup), @"--retry-sleep")]
				public OneOptWithPrefix<BaseRetrySleepSpecVal?> HTTP
				{
					get;
				} = new(null, @"--retry-sleep", @"http");

				/// <summary>
				/// Time to sleep in seconds between retries on a fragment resource.  This can be a <see cref="RetrySleepNumSpecVal"/>, a <see cref="RetrySleepLinearSpecVal"/>, or a <see cref="RetrySleepExpSpecVal"/>.  The default value of <see langword="null"/> allows yt-dlp to sleep as needed unless a config file specifies a fragment sleep.  Linear (<see cref="RetrySleepLinearSpecVal"/>) and Exponential (<see cref="RetrySleepExpSpecVal"/>) structures provide more options.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadFragmentRetrySleep), @"Time to sleep in seconds between retries on a fragment resource.  This can be a RetrySleepNumSpecVal, a RetrySleepLinearSpecVal, or a RetrySleepExpSpecVal.  The default value of null allows yt-dlp to sleep as needed unless a config file specifies a fragment sleep.  Linear (RetrySleepLinearSpecVal) and Exponential (RetrySleepExpSpecVal) structures provide more options.", typeof(DownLoadsGroup), @"--retry-sleep")]
				public OneOptWithPrefix<BaseRetrySleepSpecVal?> Fragment
				{
					get;
				} = new(null, @"--retry-sleep", @"fragment");

				/// <summary>
				/// Time to sleep in seconds between retries on a file resource.  This can be a <see cref="RetrySleepNumSpecVal"/>, a <see cref="RetrySleepLinearSpecVal"/>, or a <see cref="RetrySleepExpSpecVal"/>.  The default value of <see langword="null"/> allows yt-dlp to sleep as needed unless a config file specifies a file sleep.  Linear (<see cref="RetrySleepLinearSpecVal"/>) and Exponential (<see cref="RetrySleepExpSpecVal"/>) structures provide more options.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadFileAccessRetrySleep), @"Time to sleep in seconds between retries on a file resource.  This can be a RetrySleepNumSpecVal, a RetrySleepLinearSpecVal, or a RetrySleepExpSpecVal.  The default value of null allows yt-dlp to sleep as needed unless a config file specifies a file access sleep.  Linear (RetrySleepLinearSpecVal) and Exponential (RetrySleepExpSpecVal) structures provide more options.", typeof(DownLoadsGroup), @"--retry-sleep")]
				public OneOptWithPrefix<BaseRetrySleepSpecVal?> FileAccess
				{
					get;
				} = new(null, @"--retry-sleep", @"file_access");

				/// <summary>
				/// Time to sleep in seconds between retries on a extractor resource.  This can be a <see cref="RetrySleepNumSpecVal"/>, a <see cref="RetrySleepLinearSpecVal"/>, or a <see cref="RetrySleepExpSpecVal"/>.  The default value of <see langword="null"/> allows yt-dlp to sleep as needed unless a config file specifies a extractor sleep.  Linear (<see cref="RetrySleepLinearSpecVal"/>) and Exponential (<see cref="RetrySleepExpSpecVal"/>) structures provide more options.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadExtractorRetrySleep), @"Time to sleep in seconds between retries on a extractor resource.  This can be a RetrySleepNumSpecVal, a RetrySleepLinearSpecVal, or a RetrySleepExpSpecVal.  The default value of null allows yt-dlp to sleep as needed unless a config file specifies a extractor sleep.  Linear (RetrySleepLinearSpecVal) and Exponential (RetrySleepExpSpecVal) structures provide more options.", typeof(DownLoadsGroup), @"--retry-sleep")]
				public OneOptWithPrefix<BaseRetrySleepSpecVal?> Extractor
				{
					get;
				} = new(null, @"--retry-sleep", @"extractor");


				/// <summary>
				/// Lists all the options inside <see cref="SleepBeforeRetryGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
					=> [
						HTTP,
						Fragment,
						FileAccess,
						Extractor,
					];
			}

			/// <summary>
			/// Lists the ways in which a playlist can be ordered.  yt-dlp uses separate command line options for <see cref="reversed"/> and <see cref="random"/>, but by using an <see langword="enum"/>, we can group them into a single <see cref="OneOpt{PlayListOrderChoices}"/> instance.
			/// </summary>
			public enum PlayListOrderChoices
			{
				/// <summary>
				/// Specifies the default order.  If a config file specifies --playlist-random, --playlist-reverse, or something like "-I ::-1", that order will take override yt-dlp's default.  Otherwise, yt-dlp will treat this value as <see cref="normal"/>.
				/// </summary>
				@default,

				/// <summary>
				/// Forces yt-dlp to ignore the config files and download playlist entries in the order listed in the playlist.
				/// </summary>
				normal,

				/// <summary>
				/// Download the entries in reverse order.
				/// </summary>
				reversed,

				/// <summary>
				/// Download the entries in random order.
				/// </summary>
				random,
			}

			/// <summary>
			/// Groups all options regarding which downloader to use for what.
			/// </summary>
			public sealed class ChoicesForDownLoadingGroup
			{
				/// <summary>
				/// Name of a downloader to use for HTTP.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForHTTP), @"Name of a downloader to use for HTTP.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
				public OneOptWithPrefix<SupportedDownLoaders?> HTTP
				{
					get;
				} = new(null, @"--downloader", @"http");

				/// <summary>
				/// Name of a downloader to use for FTP.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForFTP), @"Name of a downloader to use for FTP.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
				public OneOptWithPrefix<SupportedDownLoaders?> FTP
				{
					get;
				} = new(null, @"--downloader", @"ftp");

				/// <summary>
				/// Name of a downloader to use for m3u8 files.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForM3U8), @"Name of a downloader to use for m3u8.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
				public OneOptWithPrefix<SupportedDownLoaders?> M3U8
				{
					get;
				} = new(null, @"--downloader", @"m3u8");

				/// <summary>
				/// Name of a downloader to use for DASH.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForDASH), @"Name of a downloader to use for DASH.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
				public OneOptWithPrefix<SupportedDownLoaders?> DASH
				{
					get;
				} = new(null, @"--downloader", @"dash");

				/// <summary>
				/// Name of a downloader to use for RSTP.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForRSTP), @"Name of a downloader to use for RSTP.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
				public OneOptWithPrefix<SupportedDownLoaders?> RSTP
				{
					get;
				} = new(null, @"--downloader", @"rstp");

				/// <summary>
				/// Name of a downloader to use for RTMP.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForRTMP), @"Name of a downloader to use for RTMP.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
				public OneOptWithPrefix<SupportedDownLoaders?> RTMP
				{
					get;
				} = new(null, @"--downloader", @"rtmp");

				/// <summary>
				/// Name of a downloader to use for MMS.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForMMS), @"Name of a downloader to use for MMS.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
				public OneOptWithPrefix<SupportedDownLoaders?> MMS
				{
					get;
				} = new(null, @"--downloader", @"mms");


				/// <summary>
				/// Lists all options inside <see cref="ChoicesForDownLoadingGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
					=> [
						HTTP,
						FTP,
						M3U8,
						DASH,
						RSTP,
						RTMP,
						MMS,
					];
			}

			/// <summary>
			/// Groups all options that send yt-dlp arguments for a downloader.
			/// </summary>
			public sealed class ArgsForDownLoaderGroup
			{
				/// <summary>
				/// Give these arguments to the native downloader.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsNative), @"Give these arguments to the native downloader.", typeof(DownLoadsGroup), @"--downloader-args")]
				public OneOptWithPrefix<string> Native
				{
					get;
				} = new(string.Empty, @"--downloader-args", @"native");

				/// <summary>
				/// Give these arguments to the aria2c downloader.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsAria2c), @"Give these arguments to the aria2c downloader.", typeof(DownLoadsGroup),
					@"--downloader-args")]
				public OneOptWithPrefix<string> Aria2c
				{
					get;
				} = new(string.Empty, @"--downloader-args", @"aria2c");

				/// <summary>
				/// Give these arguments to the avconv downloader.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsAvconv), @"Give these arguments to the avconv downloader.", typeof(DownLoadsGroup),
					@"--downloader-args")]
				public OneOptWithPrefix<string> Avconv
				{
					get;
				} = new(string.Empty, @"--downloader-args", @"avconv");

				/// <summary>
				/// Give these arguments to the axel downloader.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsAxel), @"Give these arguments to the axel downloader.", typeof(DownLoadsGroup),
					@"--downloader-args")]
				public OneOptWithPrefix<string> Axel
				{
					get;
				} = new(string.Empty, @"--downloader-args", @"axel");

				/// <summary>
				/// Give these arguments to the curl downloader.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsCurl), @"Give these arguments to the curl downloader.", typeof(DownLoadsGroup),
					@"--downloader-args")]
				public OneOptWithPrefix<string> Curl
				{
					get;
				} = new(string.Empty, @"--downloader-args", @"curl");

				/// <summary>
				/// Give these arguments to the ffmpeg downloader.  For this downloader, you can use the same syntax as for <see cref="PostProcessingGroup.PostProcessorArgs"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsFFMPEG), @"Give these arguments to the ffmpeg downloader.  For this downloader, you can use the same " +
					@"syntax as for PostProcessing.PostProcessingArgs.", typeof(DownLoadsGroup), @"--downloader-args")]
				public OneOptWithPrefix<string> FFMPEG
				{
					get;
				} = new(string.Empty, @"--downloader-args", @"ffmpeg");

				/// <summary>
				/// Give these arguments to the httpie downloader.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsHTTPIE), @"Give these arguments to the httpie downloader.", typeof(DownLoadsGroup),
					@"--downloader-args")]
				public OneOptWithPrefix<string> HTTPIE
				{
					get;
				} = new(string.Empty, @"--downloader-args", @"httpie");

				/// <summary>
				/// Give these arguments to the wget downloader.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsWget), @"Give these arguments to the wget downloader.", typeof(DownLoadsGroup),
					@"--downloader-args")]
				public OneOptWithPrefix<string> Wget
				{
					get;
				} = new(string.Empty, @"--downloader-args", @"wget");


				/// <summary>
				/// Lists all options inside <see cref="ArgsForDownLoaderGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build their own list.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
					=> [
						Native,
						Aria2c,
						Avconv,
						Axel,
						Curl,
						FFMPEG,
						HTTPIE,
						Wget,
					];
			}


			/// <summary>
			/// Number of fragments of a dash/hlsnative video that should be downloaded concurrently.  The default value of <see langword="null"/> allows yt-dlp to use its default which is <c>1</c> unless a config file overrides that with --concurrent-fragments.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadConcurrentFragments), @"Number of fragments of a dash/hlsnative video that should be downloaded concurrently.  The default value of null allows yt-dlp to use its default which is 1 unless a config file overrides that.", typeof(DownLoadsGroup),
				@"--concurrent-fragments")]
			public OneOpt<uint?> ConcurrentFragments
			{
				get;
			} = new(null, @"--concurrent-fragments");

			/// <summary>
			/// Maximum download rate in bytes per second.  The default value of <see langword="null"/> allows yt-dlp to use unlimited bandwidth unless --limit-rate is specified in a config file.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadLimitRate), @"Maximum download rate in bytes per second.  The default value of null allows yt-dlp to use unlimited bandwidth unless --limit-rate is specified in a config file.", typeof(DownLoadsGroup), @"--limit-rate")]
			public OneOpt<ulong?> LimitRate
			{
				get;
			} = new(null, @"--limit-rate");

			/// <summary>
			/// Minimum download rate in bytes per second below which throttling is assumed and the video data is re-extracted.  The default value of <see langword="null"/> allows yt-dlp to have no minimum bandwidth unless --throttled-rate is specified in a config file.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadThrottledRate), @"Minimum download rate in bytes per second below which throttling is assumed and the video data is re-extracted.  The default value of null allows yt-dlp to have no minimum bandwidth unless --throttled-rate is specified in a config file.", typeof(DownLoadsGroup), @"--throttled-rate")]
			public OneOpt<ulong?> ThrottledRate
			{
				get;
			} = new(null, @"--throttled-rate");

			/// <summary>
			/// Abort after downloading the specified number of files.  The default value of <see langword="null"/> causes yt-dlp to download everything unless a config file specifies --max-downloads.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadsMax), @"Abort after downloading the specified number of files.  The default value of null causes yt-dlp to download everything unless a config file specifies --max-downloads.", typeof(DownLoadsGroup), @"--max-downloads")]
			public OneOpt<ulong?> MaxDownLoads
			{
				get;
			} = new(null, @"--max-downloads");

			/// <summary>
			/// Just a group of options that involve retrying some form of resource.
			/// </summary>
			public RetryMaxGroup MaxRetries
			{
				get;
			} = new();

			/// <summary>
			/// Groups a series of options involving how long to sleep before a retry.  All involve the command line option --retry-sleep.
			/// </summary>
			public SleepBeforeRetryGroup RetrySleep
			{
				get;
			} = new();

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will skip any fragments that aren’t available.  If <see langword="false"/>, it will retry them or abort depending on the value in <see cref="RetryMaxGroup.Fragment"/>.  If you leave this at the default value of <see langword="null"/>, yt-dlp will look at the config files for a value.  If none is found, it's as though <see cref="SkipUnavailableFragments"/> is true.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadSkipUnavailableFragments), @"If true, yt-dlp will skip any fragments that aren't available.  If false, it will retry them or abort depending on the value in RetryMaxGroup.Fragment.  If you leave this at the default value of null, yt-dlp will look at the config files for a value.  If none is found, it's as though SkipUnavailableFragments is true.", typeof(DownLoadsGroup),
				@"--skip-unavailable-fragments", @"--no-skip-unavailable-fragments")]
			public ThreeWayOpt SkipUnavailableFragments
			{
				get;
			} = new(@"--skip-unavailable-fragments", @"--no-skip-unavailable-fragments");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will leave fragment files after the full file is downloaded.  If <see langword="false"/>, it will delete them.  The default value of <see langword="null"/> is treated by yt-dlp as though you said <see langword="false"/> unless the config file species --keep-fragments or --no-keep-fragments.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadKeepFragments), @"If true, yt-dlp will leave fragment files after the full file is downloaded.  If false, it will delete them.  The default value of null is treated by yt-dlp as though you said false unless the config file species --keep-fragments or --no-keep-fragments.", typeof(DownLoadsGroup), @"--keep-fragments", @"--no-keep-fragments")]
			public ThreeWayOpt KeepFragments
			{
				get;
			} = new(@"--keep-fragments", @"--no-keep-fragments");

			/// <summary>
			/// Size of the download buffer in bytes.  The default value of <see langword="null"/> allows yt-dlp to choose this unless a config file specifies --buffer-size.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadBufferSize), @"Size of the download buffer in bytes.  The default value of null allows yt-dlp to choose this unless a config file specifies --buffer-size.", typeof(DownLoadsGroup), @"--buffer-size")]
			public OneOpt<ulong?> BufferSize
			{
				get;
			} = new(null, @"--buffer-size");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will resize the buffer as needed.  If <see langword="false"/>, yt-dlp will leave the buffer size alone.  The default of <see langword="null"/> acts like <see langword="true"/> unless a config file specifies either --resize-buffer or --no-resize-buffer.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadResizeBuffer), @"If true, yt-dlp will resize the buffer as needed.  If false, yt-dlp will leave the buffer size alone.  The default of null acts like true unless a config file specifies either --resize-buffer or --no-resize-buffer.", typeof(DownLoadsGroup), @"--resize-buffer", @"--no-resize-buffer")]
			public ThreeWayOpt ResizeBuffer
			{
				get;
			} = new(@"--resize-buffer", @"--no-resize-buffer");

			/// <summary>
			/// Size of a chunk for chunk-based HTTP downloading.  May be useful for bypassing bandwidth throttling imposed by a web server (experimental)
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadHttpChunkSize), @"Size of a chunk for chunk-based HTTP downloading.  May be useful for bypassing bandwidth throttling imposed by a web server (experimental)", typeof(DownLoadsGroup), @"--http-chunk-size")]
			public OneOpt<ulong?> HttpChunkSize
			{
				get;
			} = new(null, @"--http-chunk-size");

			/// <summary>
			/// Controls the order in which videos will be downloaded in.  <see cref="PlayListOrderChoices.normal"/> causes the playlist entries to be downloaded in the order listed by the playlist.  <see cref="PlayListOrderChoices.reversed"/> reverses that order.  <see cref="PlayListOrderChoices.random"/> causes yt-dlp to randomly choose the next video.  If you use the default value of <see cref="PlayListOrderChoices.@default"/> and no config file changes the order with --playlist-random, --playlist-reverse, or something like "-I ::-1", yt-dlp will act like <see cref="PlayListOrder"/> is set to <see cref="PlayListOrderChoices.normal"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadPlayListOrder), @"Controls the order in which videos will be downloaded in.  PlayListOrderChoices.normal causes the playlist entries to be downloaded in the order listed by the playlist.  PlayListOrderChoices.reverse reverses that order.  PlayListOrderChoices.random causes yt-dlp to randomly choose the next video.  If you use the default value of PlayListOrderChoices.@default and no config file changes the order with --playlist-random, --playlist-reverse, or something like ""-I ::-1"", yt-dlp will act like PlayListOrder is set to PlayListOrderChoices.normal.", typeof(DownLoadsGroup),
				@"--playlist-random", @"--playlist-items")]
			public OneOpt<PlayListOrderChoices> PlayListOrder
			{
				get;
			} = new(PlayListOrderChoices.@default, string.Empty)
			{
				ParamListGenerator =
					opt
						=> opt.CurVal switch
							{
								PlayListOrderChoices.@default
									=> [],

								PlayListOrderChoices.normal
									=> [@"--playlist-items", @"::1"],

								PlayListOrderChoices.reversed
									=> [@"--playlist-items", @"::-1"],

								PlayListOrderChoices.random
									=> [@"--playlist-random"],

								_
									=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<PlayListOrderChoices>(opt.CurVal, @"While selecting parameters for the playlist "
										+ @"order"),
							}
			};

			/// <summary>
			/// If <see langword="true"/>, playlist entries won’t be processed until their information is needed.  If <see langword="false"/>, yt-dlp will download all playlist entries immediately.  If you use the default value of <see langword="null"/>, yt-dlp will act as though you specified <see langword="false"/> unless a config file specifies either --lazy-playlist or --no-lazy-playlist.  Note: The value of <see langword="true"/> disables <see cref="PlayListOrder"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadLazyPlayList), @"If true, playlist entries won’t be processed until their information is needed.  If false, yt-dlp will download all playlist entries immediately.  If you use the default value of null, yt-dlp will act as though you specified false unless a config file specifies either --lazy-playlist or --no-lazy-playlist.  Note: The value of true disables PlayListRandomOrder and PlayListReverseOrder.", typeof(DownLoadsGroup), @"--lazy-playlist", @"--no-lazy-playlist")]
			public ThreeWayOpt LazyPlayList
			{
				get;
			} = new(@"--lazy-playlist", @"--no-lazy-playlist");

			/// <summary>
			/// Sets file xattribute ytdl.filesize with expected file size
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadSetExpectedFileSizeXATTR), @"Sets file xattribute ytdl.filesize with expected file size", typeof(DownLoadsGroup), @"--xattr-set-filesize")]
			public OneOpt<bool> XattrSetFileSizeAttr
			{
				get;
			} = new(false, @"--xattr-set-filesize");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp uses the MPEG TS format to store videos in.  This improves the ability for some players to play the video while it’s still downloading.  If <see langword="false"/>, the video will be saved straight to the final format prior to any needed post processing that might change the file format.  If you use the default value of <see langword="null"/>, yt-dlp will act as though you used <see langword="true"/> for a live stream and <see langword="false"/> for other videos.  Both defaults though are if a config file specifies either --hls-use-mpegts or --no-hls-use-mpegts.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadHlsUseMPEGTS), @"If true, yt-dlp uses the MPEG TS format to store videos in.  This improves the ability for some players to play the video while it's still downloading.  If false, the video will be saved straight to the final format prior to any needed post processing that might change the file format.  If you use the default value of null, yt-dlp will act as though you used true for a live stream and false for other videos.  Both defaults though are overridden unless the config file specifies either --hls-use-mpegts or --no-hls-use-mpegts.", typeof(DownLoadsGroup), @"--hls-use-mpegts", @"--no-hls-use-mpegts")]
			public ThreeWayOpt HlsUseMPEGTS
			{
				get;
			} = new(@"--hls-use-mpegts", @"--no-hls-use-mpegts");

			/// <summary>
			/// Download only chapters that match the regular expression.  A “*” prefix denotes time-range instead of chapter.  Negative timestamps are calculated from the end.  “*from-url” can be used to download between the “start_time” and “end_time” extracted from the URL.  Needs ffmpeg.  If you need to download multiple sections, use an array, e.g. <c>[""*10:15-inf"", ""intro""]</c>.  No assistance is provided here beyond the array because of the edge case where this is needed.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadSections), @"Download only chapters that match the regular expression.  A “*” prefix denotes time-range instead of chapter.  Negative timestamps are calculated from the end.  “*from-url” can be used to download between the “start_time” and “end_time” extracted from the URL.  Needs ffmpeg.  If you need to download multiple sections, use an array, e.g. [""*10:15-inf"", ""intro""].  No assistance is provided here beyond the array because of the edge case where this is needed.", typeof(DownLoadsGroup), @"--download-sections")]
			public OneOpt<System.Collections.Generic.IEnumerable<string>> Sections
			{
				get;
			} = new([], @"--download-sections");

			/// <summary>
			/// Groups all choices about which downloader to use
			/// </summary>
			public ChoicesForDownLoadingGroup WhatToUse
			{
				get;
			} = new();

			/// <summary>
			/// All these options specify what to pass to each downloader
			/// </summary>
			public ArgsForDownLoaderGroup Args
			{
				get;
			} = new();


			/// <summary>
			/// Returns a list of all options inside <see cref="DownLoadsGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					ConcurrentFragments,
					LimitRate,
					ThrottledRate,
					MaxDownLoads,
					..MaxRetries.AllOpt,
					..RetrySleep.AllOpt,
					SkipUnavailableFragments,
					KeepFragments,
					BufferSize,
					ResizeBuffer,
					LazyPlayList,
					XattrSetFileSizeAttr,
					HlsUseMPEGTS,
					Sections,
					..WhatToUse.AllOpt,
					..Args.AllOpt,
				];
		}

		/// <summary>
		/// Groups all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#filesystem-options">File System Options section of yt-dlp’s readme.md</a>.
		/// </summary>
		[System.ComponentModel.ImmutableObject(true)]
		public sealed class FileSysGroup
		{
			/// <summary>
			/// Provides selections on if yt-dlp should overwrite files or not.
			/// </summary>
			[System.ComponentModel.ImmutableObject(true)]
			public sealed class OverWriteChoices
			{
				/// <summary>
				/// Constructs a new instance
				/// </summary>
				/// <param name="strName">The name to use.  This is mainly for documentation.</param>
				/// <param name="strParamToPass">The parameter to emit to yt-dlp</param>
				private OverWriteChoices(in string strName, in string strParamToPass)
				{
					this.strName = strName;
					this.strParamToPass = strParamToPass;
				}


				/// <summary>
				/// The name to use.  This is mainly for documentation.
				/// </summary>
				public readonly string strName;

				/// <summary>
				/// The parameter to emit to yt-dlp
				/// </summary>
				public readonly string strParamToPass;


				/// <summary>
				/// Disallows overwriting files
				/// </summary>
				public static readonly OverWriteChoices disallow = new(@"disallow", @"--no-overwrites");

				/// <summary>
				/// Forces files that exist to be overwritten
				/// </summary>
				public static readonly OverWriteChoices force = new(@"force", @"--force-overwrites");

				/// <summary>
				/// Overwrites related files, but not the actual target
				/// </summary>
				public static readonly OverWriteChoices allowForRelatedFilesOnly = new(@"allowed for related files only",
					@"--no-force-overwrites");
			}

			/// <summary>
			/// Specifies which browser to get cookies from.  Optionally also specifies the profile and, on operating systems other than Windows, a keyring.
			/// </summary>
			/// <param name="Browser">A value from <see cref="SupportedBrowsers"/> that specifies the browser to use.</param>
			/// <remarks>
			///		<para>Add the profile with <see cref="Profile"/>.  Add the keyring with <see cref="KeyRing"/>.  Those are properties, so use the property initialization syntax.</para>
			/// </remarks>
			public class CookiesFromBrowserSpecOpt(CookiesFromBrowserSpecOpt.SupportedBrowsers Browser) : IOneOptVal
			{
				/// <summary>
				/// A value from <see cref="SupportedBrowsers"/> that specifies the browser to use.
				/// </summary>
				public SupportedBrowsers Browser
				{
					get;

					set;
				} = Browser;

				/// <summary>
				/// Which KeyRing to use.  Must be a value from <see cref="SupportedKeyRings"/>.
				/// </summary>
				public SupportedKeyRings? KeyRing
				{
					get;

					set;
				} = null;

				/// <summary>
				/// Gets or sets the profile.  Specify a profile name or path.
				/// </summary>
				public string Profile
				{
					get;

					set;
				} = string.Empty;

				/// <summary>
				/// Container name (if Firefox) ("none" for no container)
				/// </summary>
				public string Container
				{
					get;

					set;
				} = string.Empty;


				/// <summary>
				/// Lists all browsers that yt-dlp supports.  These are all <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members of <see cref="SupportedBrowsers"/>.  You aren’t allowed to create your own instances of <see cref="SupportedBrowsers"/> as that would require yt-dlp to support new browsers.
				/// </summary>
				public sealed class SupportedBrowsers
				{
					/// <summary>
					/// Constructs a new instance
					/// </summary>
					/// <param name="strName">The name of the browser as expected by yt-dlp.  Be sure to pass a raw string.</param>
					private SupportedBrowsers(in string strName)
						=> this.strName = strName;


					/// <summary>
					/// The exact name yt-dlp is looking for
					/// </summary>
					public readonly string strName;


					/// <summary>
					/// Specifies the Brave browser.
					/// </summary>
					public static readonly SupportedBrowsers brave = new(@"brave");

					/// <summary>
					/// Specifies the Google Chrome browser.  For Chromium, use <see cref="chromium"/>.
					/// </summary>
					public static readonly SupportedBrowsers chrome = new(@"chrome");

					/// <summary>
					/// Specifies the Chromium browser.  If you want Google Chrome, use <see cref="chrome"/>.
					/// </summary>
					public static readonly SupportedBrowsers chromium = new(@"chromium");

					/// <summary>
					/// Specifies the Microsoft Edge browser.
					/// </summary>
					public static readonly SupportedBrowsers edge = new(@"edge");

					/// <summary>
					/// Specifies Mozilla Firefox.
					/// </summary>
					public static readonly SupportedBrowsers firefox = new(@"firefox");

					/// <summary>
					/// Specifies the Opera browser
					/// </summary>
					public static readonly SupportedBrowsers opera = new(@"opera");

					/// <summary>
					/// Specifies the Safari browser
					/// </summary>
					public static readonly SupportedBrowsers safari = new(@"safari");

					/// <summary>
					/// Specifies the Vivaldi browser
					/// </summary>
					public static readonly SupportedBrowsers vivaldi = new(@"vivaldi");

					/// <summary>
					/// Specifies the Whale browser
					/// </summary>
					public static readonly SupportedBrowsers whale = new(@"whale");


					/// <summary>
					/// Disables the cookies.
					/// </summary>
					public static readonly SupportedBrowsers none = new(string.Empty);
				}

				/// <summary>
				/// Lists choices for which key ring you want to use.  These are all <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members of <see cref="SupportedKeyRings"/>.  You aren’t allowed to create your own instances of <see cref="SupportedKeyRings"/> as that would require yt-dlp to support new browsers.
				/// </summary>
				public sealed class SupportedKeyRings
				{
					/// <summary>
					/// Constructs a new instance.
					/// </summary>
					/// <param name="strName">The exact text yt-dlp is looking for.  Use a raw string.</param>
					private SupportedKeyRings(in string strName)
						=> this.strName = strName;


					/// <summary>
					/// The exact text yt-dlp is looking for.
					/// </summary>
					public readonly string strName;


					/// <summary>
					/// Specifies the key ring should be basic text.  This might be unencrypted.
					/// </summary>
					public static readonly SupportedKeyRings basicText = new(@"basictext");

					/// <summary>
					/// Specifies the Gnome keyring.
					/// </summary>
					public static readonly SupportedKeyRings gnome = new(@"gnomekeyring");

					/// <summary>
					/// Specifies a KWallet keyring.  The version isn’t specified.
					/// </summary>
					public static readonly SupportedKeyRings kwallet = new(@"kwallet");

					/// <summary>
					/// Specifies a KWallet 5 keyring.
					/// </summary>
					public static readonly SupportedKeyRings kwallet5 = new(@"kwallet5");

					/// <summary>
					/// Specifies a KWallet 6 keyring.
					/// </summary>
					public static readonly SupportedKeyRings kwallet6 = new(@"kwallet6");
				}


				/// <summary>
				/// Generates the text that yt-dlp is looking for.  The output depends on which fields are <see langword="null"/> or non-<see langword="null"/>.
				/// </summary>
				public string? ValText
				{
					get
					{
						string? strKeyRing = KeyRing?.strName;

						return strKeyRing is null && Profile == "" && Container == ""
										? Browser.strName
										: strKeyRing is null && Profile == "" && Container != ""
											? $"{Browser.strName}::{Container}"
											: strKeyRing is null && Profile != "" && Container == ""
												? $"{Browser.strName}:{Profile}"
												: strKeyRing is null && Profile != "" && Container != ""
													? $"{Browser.strName}:{Profile}::{Container}"
													: strKeyRing is not null && Profile == "" && Container == ""
														? $"{Browser.strName}+{strKeyRing}"
														: strKeyRing is not null && Profile == "" && Container != ""
															? $"{Browser.strName}+{strKeyRing}::{Container}"
															: strKeyRing is not null && Profile != "" && Container == ""
																? $"{Browser.strName}+{strKeyRing}:{Profile}"
																: $"{Browser.strName}+{strKeyRing}:{Profile}::{Container}";
					}
				}

				/// <summary>
				/// Generates a <see cref="CookiesFromBrowserSpecOpt"/> from a <see cref="SupportedBrowsers"/> instance.
				/// </summary>
				/// <param name="browser"></param>
				public static implicit operator CookiesFromBrowserSpecOpt(in SupportedBrowsers browser)
					=> new(browser);
			}

			/// <summary>
			/// Groups all options that emit --paths.
			/// </summary>
			public sealed class PathsGroup
			{
				/// <summary>
				/// All post-processed files will be downloaded to this path if it isn't null.  Ignored by yt-dlp if --output without a prefix specifies an absolute path.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsHome), @"All post-processed files will be downloaded to this path if it isn't null.  Ignored by yt-dlp if --output without a prefix specifies an absolute path.", typeof(FileSysGroup), @"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> Home
				{
					get;
				} = new(null, @"--paths", @"home");

				/// <summary>
				/// This path will be used while files are being downloaded if you change it from the default value of <see langword="null"/>.  Once post-processing is complete, the file will be moved to the home path.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsTemp), @"This path will be used while files are being downloaded if you change it from the default value of null.  Once post-processing is complete, the file will be moved to the home path.", typeof(FileSysGroup), @"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> Temp
				{
					get;
				} = new(null, @"--paths", @"temp");

				/// <summary>
				/// All subtitle files will be saved to this path if it isn't <see langword="null"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsSubs), @"All subtitle files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> Subs
				{
					get;
				} = new(null, @"--paths", @"subtitle");

				/// <summary>
				/// All thumbnail files will be saved to this path if it isn't <see langword="null"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsThumbs), @"All thumbnail files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> Thumbs
				{
					get;
				} = new(null, @"--paths", @"thumbnail");

				/// <summary>
				/// All description files for anything that isn’t a playlist will be saved here if this isn’t <see langword="null"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsDesc), @"All description files for anything that isn't a playlist will be saved here if this isn’t null.", typeof(FileSysGroup), @"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> Desc
				{
					get;
				} = new(null, @"--paths", "description");

				/// <summary>
				/// All .info.json files will be saved to this path if it isn’t <see langword="null"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsInfoJSON), @"All .info.json files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> InfoJSON
				{
					get;
				} = new(null, @"--paths", @"infojson");

				/// <summary>
				/// All link files will be saved to this path if it isn’t <see langword="null"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsLinks), @"All link files will be saved to this path if it isn't null.", typeof(FileSysGroup),
					@"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> Links
				{
					get;
				} = new(null, @"--paths", @"link");

				/// <summary>
				/// All playlist thumbnail files will be saved to this path if it isn’t <see langword="null"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsPlayListThumbs), @"All playlist thumbnail files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> PlayListThumb
				{
					get;
				} = new(null, @"--paths", @"pl_thumbnail");

				/// <summary>
				/// All playlist description files will be saved to this path if it isn’t <see langword="null"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsPlayListDesc), @"All playlist description files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> PlayListDesc
				{
					get;
				} = new(null, @"--paths", @"pl_description");

				/// <summary>
				/// All playlist .info.json files will be saved to this path if it isn’t <see langword="null"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsPlayListInfoJSON), @"All playlist .info.json files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> PlayListInfoJSON
				{
					get;
				} = new(null, @"--paths", @"pl_infojson");

				/// <summary>
				/// All chapter files will be saved to this path if it isn’t <see langword="null"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsChapter), @"All chapter files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> Chapters
				{
					get;
				} = new(null, @"--paths", @"chapter");

				/// <summary>
				/// All playlist video files will be saved to this path if it isn’t <see langword="null"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsPlayListEntries), @"All playlist video files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
				public OneOptWithPrefix<System.IO.DirectoryInfo?> PlayListEntries
				{
					get;
				} = new(null, @"pl_video", @"playlist");


				/// <summary>
				/// Lists all the options inside <see cref="FileSysGroup"/>.  THe parent of this instance will use <see cref="AllOpt"/> to build its own list of options.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
					=> [
						Home,
						Temp,
						Subs,
						Thumbs,
						Desc,
						InfoJSON,
						Links,
						PlayListThumb,
						PlayListDesc,
						PlayListInfoJSON,
						Chapters,
						PlayListEntries,
					];
			}

			/// <summary>
			/// Groups various options that all use --output.  These just use different prefixes.  Note: You won’t find the no-prefix version of --output here as all relevant Cmdlets in <see cref="YtDlpWrapper"/> declare that directly with <see cref="BaseVidCmdLet.FileNameFmt"/> and that would conflict with anything in here for that.  You also won’t find anything for the annotation template as that’s deprecated.
			/// </summary>
			public sealed class TemplatesForOutputFilesGroup
			{
				/// <summary>
				/// Applies to all subtitle files
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatesSubs), @"Applies to all subtitle files", typeof(FileSysGroup),
					@"--output")]
				public OneOptWithPrefix<string> Subs
				{
					get;
				} = new(string.Empty, @"--output", @"subtitle");

				/// <summary>
				/// Applies to all thumbnail files
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatesThumbs), @"Applies to all thumbnail files", typeof(FileSysGroup),
					@"--output")]
				public OneOptWithPrefix<string> Thumbs
				{
					get;
				} = new(string.Empty, @"--output", @"infojson");

				/// <summary>
				/// Applies to all description files
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatesDesc), @"Applies to all description files", typeof(FileSysGroup),
					@"--output")]
				public OneOptWithPrefix<string> Desc
				{
					get;
				} = new(string.Empty, @"--output", @"link");

				/// <summary>
				/// Applies to all .info.json files
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatesInfoJSON), @"Applies to all .info.json files", typeof(FileSysGroup),
					@"--output")]
				public OneOptWithPrefix<string> InfoJSON
				{
					get;
				} = new(string.Empty, @"--output", @"infojson");

				/// <summary>
				/// Applies to all link files
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatesLinks), @"Applies to all link files", typeof(FileSysGroup), @"--output")]
				public OneOptWithPrefix<string> Links
				{
					get;
				} = new(string.Empty, @"--output", @"link");

				/// <summary>
				/// Applies to all playlist thumbnail files
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatePlayListThumbs), @"Applies to all playlist thumbnail files", typeof(FileSysGroup), @"--output")]
				public OneOptWithPrefix<string> PlayListThumbs
				{
					get;
				} = new(string.Empty, @"--output", @"pl_thumbnail");

				/// <summary>
				/// Applies to all playlist description files
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatePlayListDesc), @"Applies to all playlist description files", typeof(FileSysGroup), @"--output")]
				public OneOptWithPrefix<string> PlayListDesc
				{
					get;
				} = new(string.Empty, @"--output", @"pl_description");

				/// <summary>
				/// Applies to all playlist .info.json files
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatePlayListInfoJSON), @"Applies to all playlist .info.json files", typeof(FileSysGroup), @"--output")]
				public OneOptWithPrefix<string> PlayListInfoJSON
				{
					get;
				} = new(string.Empty, @"--output", @"pl_infojson");

				/// <summary>
				/// Applies to all chapter files
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplateChapters), @"Applies to all chapter files", typeof(FileSysGroup),
					@"--output")]
				public OneOptWithPrefix<string> Chapters
				{
					get;
				} = new(string.Empty, @"--output", @"chapter");

				/// <summary>
				/// Applies to all playlist video files
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatePlayListEntries), @"Applies to all playlist video files", typeof(FileSysGroup), @"--output")]
				public OneOptWithPrefix<string> PlayListEntries
				{
					get;
				} = new(string.Empty, @"--output", @"pl_video");


				/// <summary>
				/// Lists all options inside <see cref="TemplatesForOutputFilesGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to generate it’s own list.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
					=> [
						Subs,
						Thumbs,
						Desc,
						InfoJSON,
						Links,
						PlayListThumbs,
						PlayListDesc,
						PlayListInfoJSON,
						Chapters,
						PlayListEntries,
					];
			}


			/// <summary>
			/// Lets you access options inside <see cref="PathsGroup"/>.
			/// </summary>
			public PathsGroup Paths
			{
				get;

				private init;
			} = new();

			/// <summary>
			/// Provides access to <see cref="TemplatesForOutputFilesGroup"/>.
			/// </summary>
			public TemplatesForOutputFilesGroup OutputTemplates
			{
				get;

				private init;
			} = new();

			/// <summary>
			/// Placeholder for unavailable fields.  If you leave this at the default, yt-dlp uses “NA” unless a config file provides another value with
			/// --output-na-placeholder.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPlaceHolderTextInGeneratedFileNames), @"Placeholder for unavailable fields.  If you leave this at the default, " +
				@"yt-dlp uses “NA” unless a config file provides another value with --output-na-placeholder.", typeof(FileSysGroup),
				@"--output-na-placeholder")]
			public OneOpt<string> PlaceHolderTextInGeneratedFileNames
			{
				get;

				private init;
			} = new(string.Empty, @"--output-na-placeholder");

			/// <summary>
			/// If <see langword="true"/>, file names are limited to ASCII characters with no spaces or ampersands (‘&amp;’).  If <see langword="false"/>, no such
			/// restrictions are in place.  If you use the default value of <see langword="null"/> and no config file specifies a value, yt-dlp will act as though
			/// you set RestrictFileNames to <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFIleSysRestrictFileNames), @"If true, file names are limited to ASCII characters with no spaces or ampersands (‘&’).  " +
				@"If false, no such restrictions are in place.  If you use the default value of null and no config file specifies a value, yt-dlp will act as though " +
				@"you set restrictFileNames to false.", typeof(FileSysGroup), @"--restrict-filenames", @"--no-restrict-filenames")]
			public ThreeWayOpt RestrictFileNames
			{
				get;

				private init;
			} = new( @"--restrict-filenames", @"--no-restrict-filenames");

			/// <summary>
			/// If <see langword="true"/>, all file names must be Windows compatible even if yt-dlp is run on a non-Windows OS.  If <see langword="false"/>, only
			/// minimal sanitation is done.  If you use the default value of <see langword="null"/> and --windows-filenames isn’t set by a config file, yt-dlp’s
			/// default changes based on if you’re running it in Windows or something else.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysUseWindowsNames), @"If true, all file names must be Windows compatible even if yt-dlp is run on a non-Windows " +
				@"OS.  If false, only minimal sanitation is done.  If you use the default value of null and this isn’t set by a config file, yt-dlp’s default changes "
				+ @"based on if you’re running it in Windows or something else.", typeof(FileSysGroup), @"--windows-filenames",
				@"--no-windows-filenames")]
			public ThreeWayOpt UseWindowsFileNames
			{
				get;

				private init;
			} = new(@"--windows-filenames", @"--no-windows-filenames");

			/// <summary>
			/// Limit the filename length (excluding extension) to the specified number of characters
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysMaxFileNameLength), @"Limit the filename length (excluding extension) to the specified number of " +
				@"characters", typeof(FileSysGroup), @"--trim-filenames")]
			public OneOpt<ushort?> MaxFileNameLength
			{
				get;

				private init;
			} = new(null, @"--trim-filenames");

			/// <summary>
			/// Controls how and when yt-dlp overwrites files.  The default value of <see langword="null"/> means that unless a config file species --no-overwrites,
			/// --force-overwrites, or --no-force-overwrites, yt-dlp acts as though you chose OverWriteChoices.allowForRelatedFilesOnly.  Your choices are
			/// OverWriteChoices.disallow (never overwrite), OverWriteChoices.force (always overwrite), and OverWriteChoices.allowForRelatedFilesOnly.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysWhenToAllowOverWrites), @"Controls how and when yt-dlp overwrites files.  The default value of null means that " +
				@"unless a config file species --no-overwrites, --force-overwrites, or --no-force-overwrites, yt-dlp acts as though you chose OverWriteChoices" +
				@".allowForRelatedFilesOnly.  Your choices are OverWriteChoices.disallow (never overwrite), OverWriteChoices.force (always overwrite), and " +
				@"OverWriteChoices.allowForRelatedFilesOnly.", typeof(FileSysGroup), @"--no-overwrites", @"--force-overwrites", @"--no-force-overwrites")]
			public OneOpt<OverWriteChoices?> WhenToAllowOverWrites
			{
				get;

				private init;
			} = new(null, string.Empty)
			{
				ParamListGenerator =
					opt
						=>
							opt.CurVal == null
								? []
								: opt.CurVal == OverWriteChoices.disallow
									? [@"--no-overwrites"]
									: opt.CurVal == OverWriteChoices.force
										? [@"--force-overwrites"]
										: opt.CurVal == OverWriteChoices.allowForRelatedFilesOnly
											? [@"--no-force-overwrites"]
											: throw new Tools.Exceptions.UnknownOrInvalidEnumException<OverWriteChoices>(opt.CurVal, "While mapping an OverWriteChoices "
												+ @"value to a parameter"),
			};

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will attempt to restart any interrupted downloads from a previous yt-dlp call.  If <see langword="false"/>, yt-dlp
			/// will always restart such downloads.  The default value of <see langword="null"/> acts like <see langword="true"/> unless a config file forces another
			/// behavior with -c, --continue, or --no-continue.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysResumePartialDownLoads), @"If true, yt-dlp will attempt to restart any interrupted downloads from a previous " +
				@"yt-dlp call.  If false, yt-dlp will always restart such downloads.  The default value of null acts like true unless a config file forces another " +
				@"behavior with -c, --continue, or --no-continue.", typeof(FileSysGroup), @"--continue", @"--no-continue")]
			public ThreeWayOpt ResumePartialDownLoads
			{
				get;

				private init;
			} = new(@"--continue", @"--no-continue");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will write into part files instead of the main file.  If <see langword="false"/>, it will write instead into the
			/// main file.  The default value of <see langword="null"/> acts like true unless a config file forces another value with either --part or --no-part.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysUsePartFiles), @"If true, yt-dlp will write into part files instead of the main file.  If false, it will write " +
				@"instead into the main file.  The default value of null acts like true unless a config file forces another value with either --part or --no-part.",
				typeof(FileSysGroup), @"--part", @"--no-part")]
			public ThreeWayOpt UsePartFiles
			{
				get;

				private init;
			} = new(@"--part", @"--no-part");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will use the Last-Modified header to set the file modified time.  If <see langword="false"/>, it won’t.  The default
			/// value of <see langword="null"/>, if <see cref="MTime"/> isn’t set by a config file, causes yt-dlp to act as though <see cref="MTime"/> is
			/// <see langword="true"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysMTime), @"If true, yt-dlp will use the Last-Modified header to set the file modified time.  If false, it won’t.  " +
				@"The default value of null, if MTime isn’t set by a config file, causes yt-dlp to act as though MTime is true.", typeof(FileSysGroup),
				@"--mtime", @"--no-mtime")]
			public ThreeWayOpt MTime
			{
				get;

				private init;
			} = new(@"--mtime", @"--no-mtime");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will create a .description file for each video.  If <see langword="false"/>, no such file will be created.  The
			/// default value of <see langword="null"/>, if no config file specifies one, causes yt-dlp to act as though you specified <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysDescShouldBeWritten), @"If true, yt-dlp will create a .description file for each video.  If false, no such file " +
				@"will be created.  The default value of null, if no config file specifies one, causes yt-dlp to act as though you specified false.",
				typeof(FileSysGroup), @"--write-description", @"--no-write-description")]
			public ThreeWayOpt DescShouldBeWritten
			{
				get;

				private init;
			} = new(@"--write-description", @"--no-write-description");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will create a .info.json file for each video.  If <see langword="false"/>, no such file will be created.  The
			/// default value of <see langword="null"/>, if no config file specifies one, causes yt-dlp to act as though you specified <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysInfoJsonShouldBeWritten), @"If true, yt-dlp will create a .info.json file for each video.  If false, no such " +
				@"file will be created.  The default value of null, if no config file specifies one, causes yt-dlp to act as though you specified false.",
				typeof(FileSysGroup), @"--write-infojson", @"--no-write-infojson")]
			public ThreeWayOpt InfoJsonShouldBeWritten
			{
				get;

				private init;
			} = new(@"--write-infojson", @"--no-write-infojson");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will write a playlist metadata file for each video.  If <see langword="false"/>, no such file will be created.  The
			/// default value of <see langword="null"/>, if no config file specifies one, causes yt-dlp to act as though you specified <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPlayListMetaDataShouldBeWritten), @"If true, yt-dlp will write a playlist metadata file for each video.  If false, " +
				@"no such file will be created.  The default value of null, if no config file specifies one, causes yt-dlp to act as though you specified false.",
				typeof(FileSysGroup), @"--write-playlist-metafiles", @"--no-write-playlist-metafiles")]
			public ThreeWayOpt PlayListMetaFilesShouldBeWritten
			{
				get;

				private init;
			} = new(@"--write-playlist-metafiles", @"--no-write-playlist-metafiles");

			/// <summary>
			/// If <see langword="true"/>, comments will be included in any .info.json files.  If <see langword="false"/>, comments won’t be included unless the
			/// extraction is known to be quick.  The default value of <see langword="null"/>, if no value is in a config file, causes yt-dlp to act as though you
			/// used value is <see langword="false"/>.  Ignored by yt-dlp unless you also specify set <see cref="InfoJsonShouldBeWritten"/> to a
			/// non-<see langword="null"/> value or a config file specifies --write-comments.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysCommentsShouldBeWritten), @"If true, comments will be included in any .info.json files.  If false, comments " +
				@"won’t be included unless the extraction is known to be quick.  The default value of null, if no value is in a config file, causes yt-dlp to act as " +
				@"though you used value is false.  Ignored by yt-dlp unless you also specify set InfoJsonShouldBeWritten to a non-null value or a config file specifies"
				+ @" --write-comments.", typeof(FileSysGroup), @"--write-comments", @"--no-write-comments")]
			public ThreeWayOpt CommentsShouldBeWritten
			{
				get;

				private init;
			} = new(@"--write-comments", @"--no-write-comments");

			/// <summary>
			/// If you specify a value other than <see langword="null"/> or <see cref="fileInvalid"/>, this must be a Netscape formatted file to read cookies from and
			/// dump cookie jar into.  If you use <see cref="fileInvalid"/>, it will disable any value from a config file.  If you use the default value of
			/// <see langword="null"/>, yt-dlp will not use a cookies file unless you set <see cref="CookiesFromBrowser"/> or a config file specifies a cookie file
			/// with --cookies.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysCookieFile), @"If you specify a value other than null or fileInvalid, this must be a Netscape formatted file to " +
				@"read cookies from and dump cookie jar into.  If you use fileInvalid, it will disable any value from a config file.  If you use the default value of " +
				@"null, yt-dlp will not use a cookies file unless you set CookiesFromBrowser or a config file specifies a cookie file with --cookies.", typeof(FileSysGroup),
				@"--cookies", @"--no-cookies")]
			public OneOpt<System.IO.FileInfo?> CookiesFromFile
			{
				get;

				private init;
			} = new(null, @"--cookies")
			{
				ParamListGenerator =
					opt
						=> opt.CurVal == null
							? []
							: opt.CurVal == fileInvalid
								? [@"--no-cookies"]
								: [@"--cookies", opt.CurVal.FullName],
			};

			/// <summary>
			/// If you specify a non-<see langword="null"/> value with a browser other than <see cref="CookiesFromBrowserSpecOpt.SupportedBrowsers.none"/>, yt-dlp
			/// will attempt to get cookies from the specified browser.  The default value of <see langword="null"/> doesn't cause cookies to be loaded from a browser
			/// though yt-dlp might do so anyway if a config file specifies --cookies-from-browser or you use <see cref="CookiesFromFile"/>.  Specify a
			/// non-<see langword="null"/> value and set <see cref="CookiesFromBrowserSpecOpt.Browser"/> to <see cref="CookiesFromBrowserSpecOpt.SupportedBrowsers
			/// .none"/> if you want to disable any value from a config file.  If you don’t need to specify a profile or keyring, just pass the <see
			/// cref="CookiesFromBrowserSpecOpt.SupportedBrowsers"/> instance.  It will be implicitly typecast to the needed type.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysCookiesFromBrowser), @"If you specify a non-null value with a browser other than CookiesFromBrowserSpecOpt" +
				@".SupportedBrowsers.none, yt-dlp will attempt to get cookies from the specified browser.  The default value of null doesn't cause cookies to be loaded "
				+ @"from a browser though yt-dlp might do so anyway if a config file specifies --cookies-from-browser or you use CookiesFromFile.  Specify a non-null " +
				@"value and set browser to CookiesFromBrowserSpecOpt.SupportedBrowsers.none if you want to disable any value from a config file.  If you don’t need to "
				+ @"specify a profile or keyring, just pass the CookiesFromBrowserSpecOpt.SupportedBrowsers instance.  It will be implicitly typecast to the needed " +
				@"type.", typeof(FileSysGroup), @"--cookies-from-browser", @"--no-cookies-from-browser")]
			public OneOpt<CookiesFromBrowserSpecOpt?> CookiesFromBrowser
			{
				get;

				private init;
			} = new(null, string.Empty)
			{
				ParamListGenerator =
				opt
					=>
					{
						if(opt is null)
							return [];

						CookiesFromBrowserSpecOpt? curVal = opt.CurVal;
						if(curVal is null)
							return [];
						
						string? strTextForSel = curVal.ValText;
						return curVal.Browser == CookiesFromBrowserSpecOpt.SupportedBrowsers.none
							? [@"--no-cookies-from-browser"]
							: strTextForSel is not null
								? [@"--cookies-from-browser", strTextForSel]
								: [];
					},
			};

			/// <summary>
			/// Location in the file system where yt-dlp can store some downloaded information (such as client IDs "and signatures) permanently.  The default value of
			/// <see langword="null"/>, unless --cache-dir or --no-cache-dir is in a config file, causes yt-dlp to look for an environment variable called
			/// XDG_CACHE_HOME.  If it exists and contains a valid path, yt-dlp creates a subfolder named “yt-dlp” inside there and uses that.  If you set <see
			/// cref="CacheDir"/> to dirInvalid, yt-dlp will disable caching.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysCacheDir), @"Location in the file system where yt-dlp can store some downloaded information (such as client IDs " +
				@"and signatures) permanently.  The default value of null, unless --cache-dir or --no-cache-dir is in a config file, causes yt-dlp to look for an environment " +
				@"variable called XDG_CACHE_HOME.  If it exists and contains a valid path, yt-dlp creates a subfolder named “yt-dlp” inside there and uses that.  If you"
				+ @" set CacheDir to dirInvalid, yt-dlp will disable caching.", typeof(FileSysGroup), @"--cache-dir", @"--no-cache-dir")]
			public OneOpt<System.IO.DirectoryInfo?> CacheDir
			{
				get;

				private init;
			} = new(null, string.Empty)
			{
				ParamListGenerator =
					opt
						=> opt.CurVal == null
							? []
							: opt.CurVal == dirInvalid
								? [@"--no-cache-dir"]
								: [@"--cache-dir", opt.CurVal.FullName],
			};


			/// <summary>
			/// Lists all options inside <see cref="FileSysGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					..Paths.AllOpt,
					..OutputTemplates.AllOpt,
					PlaceHolderTextInGeneratedFileNames,
					RestrictFileNames,
					UseWindowsFileNames,
					MaxFileNameLength,
					WhenToAllowOverWrites,
					ResumePartialDownLoads,
					UsePartFiles,
					MTime,
					DescShouldBeWritten,
					InfoJsonShouldBeWritten,
					PlayListMetaFilesShouldBeWritten,
					CommentsShouldBeWritten,
					CookiesFromFile,
					CookiesFromBrowser,
					CacheDir,
				];
		}

		/// <summary>
		/// Groups a series of options related to thumbnails.  All are listed under <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#thumbnail-options">Thumbnail Options section of yt-dlp’s readme.md</a>.
		/// </summary>
		public sealed class ThumbGroup
		{
			/// <summary>
			/// yt-dlp defines several different options for what thumbnails it writes out.  <see cref="WriteChoices"/> lists those.
			/// </summary>
			public enum WriteChoices
			{
				/// <summary>
				/// Causes yt-dlp to write video thumbnails.
				/// </summary>
				yes,

				/// <summary>
				/// Prevents yt-dlp from ever writing thumbnails even if a config file sets either --write-thumbnail or --write-all-thumbnails.
				/// </summary>
				never,

				/// <summary>
				/// Causes yt-dlp to write all thumbnails out.
				/// </summary>
				all,

				/// <summary>
				/// If you use <see cref="@default"/>, yt-dlp will act like you actually specified <see cref="never"/> unless a config file sets --write-thumbnail, --no-write-thumbnail, or --write-all-thumbnails.
				/// </summary>
				@default,
			}

			/// <summary>
			/// If you set this to <see cref="WriteChoices.yes"/>, yt-dlp will write thumbnails for videos.  If you use <see cref="WriteChoices.never"/>, it will never write thumbnails for videos.  If you use <see cref="WriteChoices.all"/>, it will write all thumbnails.  <see cref="WriteChoices.@default"/> causes yt-dlp to act as though you specified <see cref="WriteChoices.never"/> unless a config file specifies either --write-thumbnail, --no-write-thumbnail, or --write-all-thumbnails.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strThumbWrite), @"If you set this to WriteChoices.yes, yt-dlp will write thumbnails for videos.  If you use WriteChoices.never, it will never write thumbnails for videos.  If you use WriteChoices.all, it will write all thumbnails.  WriteChoices.@default causes yt-dlp to act as though you specified WriteChoices.never unless a config file specifies either --write-thumbnail, --no-write-thumbnail, or --write-all-thumbnails.", typeof(ThumbGroup), @"--write-thumbnail", @"--no-write-thumbnail", @"--write-all-thumbnails")]
			public OneOpt<WriteChoices> Write
			{
				get;
			} = new(WriteChoices.@default, string.Empty)
			{
				ParamListGenerator =
				opt
					=> opt.CurVal switch
						{
							WriteChoices.@default
								=> [],

							WriteChoices.yes
								=> [@"--write-thumbnail"],

							WriteChoices.never
								=> [@"--no-write-thumbnail"],

							WriteChoices.all
								=> [@"--write-all-thumbnails"],

							_
								=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<WriteChoices>(opt.CurVal, @"While converting a WriteChoices value into a"
									+ @"yt-dlp parameter"),
						}
			};

			/// <summary>
			/// If true, causes yt-dlp to list available thumbnails of each video.  Note: Thumbnail information is also available via other methods such as getting the JSON or using the types in the namespace <see cref="Goodies.YtDlpWrapper"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strThumbList), @"If true, causes yt-dlp to list available thumbnails of each video.  Note: Thumbnail information is also available via other methods such as getting the JSON or using the types in the namespace WillPittenger.Goodies.YtDlpWrapper.", typeof(ThumbGroup), @"--list-thumbnails")]
			public OneOpt<bool> List
			{
				get;
			} = new(false, @"--list-thumbnails");


			/// <summary>
			/// Lists all options inside <see cref="ThumbGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to generate its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					Write,
					List,
				];
		}

		/// <summary>
		/// Groups various options related to verbosity and simulation.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#verbosity-and-simulation-options">Verbosity and Simulation Options section of yt-dlp’s readme.md</a>.
		/// </summary>
		public sealed class VerbosityAndSimulationGroup
		{
			/// <summary>
			/// Groups all options that invoke the yt-dlp option --print.
			/// </summary>
			public sealed class PrintTemplatesGroup
			{
				/// <summary>
				/// Field name or output template to print to screen during pre-processing.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplatePreProcess), @"Field name or output template to print to screen during pre-processing.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate. is used.", typeof(VerbosityAndSimulationGroup), @"--print")]
				public OneOptWithPrefix<string> PreProcess
				{
					get;
				} = new(string.Empty, @"--print", @"pre_process");

				/// <summary>
				/// Field name or output template to print to screen once an item passes filtering.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplateAfterFilter), @"Field name or output template to print to screen once an item passes filtering.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print")]
				public OneOptWithPrefix<string> AfterFilter
				{
					get;
				} = new(string.Empty, @"--print", @"after_filter");

				/// <summary>
				/// Field name or output template to print to screen once an item is ready to download.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplateVid), @"Field name or output template to print to screen once an item is ready to download.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print")]
				public OneOptWithPrefix<string> Vid
				{
					get;
				} = new(string.Empty, @"--print", @"video");

				/// <summary>
				/// Field name or output template to print to screen just before a download.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplateBeforeDownLoad), @"Field name or output template to print to screen just before a download.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print")]
				public OneOptWithPrefix<string> BeforeDownLoad
				{
					get;
				} = new(string.Empty, @"--print", @"before_dl");

				/// <summary>
				/// Field name or output template to print to screen just before post-processing.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplatePostProcess), @"Field name or output template to print to screen just before post-processing.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print")]
				public OneOptWithPrefix<string> PostProcess
				{
					get;
				} = new(string.Empty, @"--print", @"post_process");

				/// <summary>
				/// Field name or output template to print to screen after moving the file.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplateAfterMove), @"Field name or output template to print to screen after moving the file.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print")]
				public OneOptWithPrefix<string> AfterMove
				{
					get;
				} = new(string.Empty, @"--print", @"after_move");

				/// <summary>
				/// Field name or output template to print to screen after a video is completely processed.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplateAfterVid), @"Field name or output template to print to screen after a video is completely processed.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimModeMode=false/--no-simulate.",
					typeof(VerbosityAndSimulationGroup), @"--print")]
				public OneOptWithPrefix<string> AfterVid
				{
					get;
				} = new(string.Empty, @"--print", @"after_video");

				/// <summary>
				/// Field name or output template to print to screen after all videos in a playlist are done.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplatePlayList), @"Field name or output template to print to screen after all videos in a playlist are done.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimModeMode=false/--no-simulate.",
					typeof(VerbosityAndSimulationGroup), @"--print")]
				public OneOptWithPrefix<string> PlayList
				{
					get;
				} = new(string.Empty, @"--print", @"playlist");


				/// <summary>
				/// Lists all options inside <see cref="PrintTemplatesGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to build its own list of options.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
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
			/// Groups all options that invoke the yt-dlp option --print-to-file.
			/// </summary>
			public sealed class PrintToFileGroup
			{
				/// <summary>
				/// Field name or output template to print to screen during pre-processing.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFilePreProcess), @"Field name or output template to print to screen during pre-processing.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print-to-file")]
				public PrintAndTemplateFileOpt PreProcess
				{
					get;
				} = new(@"pre_process");

				/// <summary>
				/// Field name or output template to print to screen once an item passes filtering.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFileAfterFilter), @"Field name or output template to print to screen once an item passes filtering.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print-to-file")]
				public PrintAndTemplateFileOpt AfterFilter
				{
					get;
				} = new(@"after_filter");

				/// <summary>
				/// Field name or output template to print to screen once an item is ready to download.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFileVid), @"Field name or output template to print to screen once an item is ready to " +
					@"download.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print-to-file")]
				public PrintAndTemplateFileOpt Vid
				{
					get;
				} = new(@"video");

				/// <summary>
				/// Field name or output template to print to screen just before a download.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFileBeforeDownLoad), @"Field name or output template to print to screen just before a download.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print-to-file")]
				public PrintAndTemplateFileOpt BeforeDownLoad
				{
					get;
				} = new(@"before_dl");

				/// <summary>
				/// Field name or output template to print to screen just before post-processing.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFilePostProcess), @"Field name or output template to print to screen just before post-processing.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print-to-file")]
				public PrintAndTemplateFileOpt PostProcess
				{
					get;
				} = new(@"post_process");

				/// <summary>
				/// Field name or output template to print to screen after moving the file.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFileAfterMove), @"Field name or output template to print to screen after moving the file.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print-to-file")]
				public PrintAndTemplateFileOpt AfterMove
				{
					get;
				} = new(@"after_move");

				/// <summary>
				/// Field name or output template to print to screen after a video is completely processed.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFileAfterVid), @"Field name or output template to print to screen after a video is completely processed.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print-to-file")]
				public PrintAndTemplateFileOpt AfterVid
				{
					get;
				} = new(@"after_video");

				/// <summary>
				/// Field name or output template to print to screen after all videos in a playlist are done.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFilePlayList), @"Field name or output template to print to screen after all videos in a playlist are done.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimulationGroup), @"--print-to-file")]
				public PrintAndTemplateFileOpt PlayList
				{
					get;
				} = new(@"playlist");


				/// <summary>
				/// Lists all options in <see cref="PrintToFileGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options. </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
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
			/// Provides a way to specify all the parameters defined by yt-dlp's --print-to-file option.  You need to pass in a file and specify a template before anything will be emitted to yt-dlp.
			/// </summary>
			/// <param name="strPrefix">One of the prefix strings that yt-dlp is looking for.  Use a raw string.</param>
			public sealed class PrintAndTemplateFileOpt(in string strPrefix) : IOneOpt
			{
				/// <summary>
				/// The file that you want.  The file doesn’t need to already exist, but the file name does need to be valid.
				/// </summary>
				public System.IO.FileInfo? fileDest = null;

				/// <summary>
				/// The template of what you want printed.  Note: Until this reaches yt-dlp, no verification of the syntax is done.
				/// </summary>
				public string strTemplate = string.Empty;

				/// <summary>
				/// The exact prefix text yt-dlp is looking for
				/// </summary>
				public readonly string strPrefix = strPrefix;


				/// <summary>
				/// Generates the value text in the syntax that yt-dlp is looking for.
				/// </summary>
				/// <remarks>
				///		<para>Nothing will be emitted if any of the following is true:</para>
				///		<list type="bullet">
				///			<item><see cref="strTemplate"/> is <see cref="string.Empty"/></item>
				///			<item><see cref="fileDest"/> is <see langword="null"/></item>
				///			<item><see cref="fileDest"/> is <see cref="fileInvalid"/></item>
				///		</list>
				/// </remarks>
				public System.Collections.Generic.IEnumerable<string> Params
					=> strTemplate == string.Empty || fileDest == null || fileDest == fileInvalid
						? []
						: [@"--print-to-file", $"{strPrefix}:{strTemplate}", fileDest.FullName];
			}

			/// <summary>
			/// Lists various modes that can be used to display progress.
			/// </summary>
			public enum ProgressModes
			{
				/// <summary>
				/// Allows yt-dlp to select a build-dependent method of displaying the progress of a download unless a config file uses --newline, --no-progress, or
				/// --progress.
				/// </summary>
				@default,

				/// <summary>
				/// Disables display of progress indicators.
				/// </summary>
				none,

				/// <summary>
				/// Causes yt-dlp to start a new line in the console whenever it updates the progress of a download.
				/// </summary>
				newLines,

				/// <summary>
				/// Show the progress bar even in quiet mode (<see cref="QuietMode"/>).
				/// </summary>
				progressBarEvenInQuietMode,
			}


			/// <summary>
			/// If <see langword="true"/>, quiet mode will be on.  If you turn on VerboseMode or a config file uses -v/--verbose, the log written to stderr.  If <see langword="false"/>, quiet mode will be disabled.  The default value of <see langword="null"/> allows the config file value to take precedence if one specifies --quiet or --no-quiet.  Otherwise, yt-dlp will act as though QuietMode is <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationQuietMode), @"If true, quiet mode will be on.  If you turn on VerboseMode or a config file uses -v/--verbose, the log written to stderr.  If false, quiet mode will be disabled.  The default value of null allows the config file value to take precedence if one specifies --quiet or --no-quiet.  Otherwise, yt-dlp will act as though QuietMode is false.", typeof(VerbosityAndSimulationGroup),
				@"--quiet", @"--no-quiet")]
			public ThreeWayOpt QuietMode
			{
				get;
			} = new(@"--quiet", @"--no-quiet");

			/// <summary>
			/// Causes warnings to be suppressed if true.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationNoWarnings), @"Causes warnings to be suppressed if true.", typeof(VerbosityAndSimulationGroup), @"--no-warnings")]
			public OneOpt<bool> NoWarnings
			{
				get;
			} = new(false, @"--no-warnings");

			/// <summary>
			/// If <see langword="true"/>, everything will be simulated except as noted below.  If <see langword="false"/>, everything happens for real.  If you use the default value of <see langword="null"/>, the value form the config file takes precedence.  If yt-dlp still doesn’t have a value, it acts as though <see cref="SimMode"/> is <see langword="false"/>.  Note: If <see cref="GeneralGroup.MarkWatched"/> is true or a config file specifies --mark-watched, videos will be marked as watched even in simulate mode unless you set <see cref="GeneralGroup.MarkWatched"/> to <see langword="false"/>!!!
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationSimMode), @"If true, everything will be simulated except as noted below.  If false, everything happens for real.  If you use the default value of null, the value form the config file takes precedence.  If yt-dlp still doesn't have a value, it acts as though SimMode is false.  Note: If General.MarkWatched is true or a config file specifies --mark-watched, videos will be marked as watched even in simulate mode unless you set General.MarkWatched to false!!!", typeof(VerbosityAndSimulationGroup), @"--simulate",
				@"--no-simulate")]
			public ThreeWayOpt SimMode
			{
				get;
			} = new(@"--simulate", @"--no-simulate");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will continue to process items that have no available formats.  If <see langword="false"/>, it will immediately fail.  If you use the default value of <see langword="null"/>, yt-dlp will act as though <see cref="IgnoreNoFmtsError"/> is <see langword="false"/> unless a config file species either --ignore-no-formats-error or --no-ignore-no-formats-error.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationIgnoreNoFmtsError), @"If true, yt-dlp will continue to process items that have no available formats.  If false, it will immediately fail.  If you use the default value of null, yt-dlp will act as though IgnoreNoFmtsError is false unless a config file species either --ignore-no-formats-error or --no-ignore-no-formats-error.", typeof(VerbosityAndSimulationGroup),
				@"--ignore-no-formats-error", @"--no-ignore-no-formats-error")]
			public ThreeWayOpt IgnoreNoFmtsError
			{
				get;
			} = new(@"--ignore-no-formats-error", @"--no-ignore-no-formats-error");

			/// <summary>
			/// Do not download the video but write all related files.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationSkipDownLoad), @"Do not download the video but write all related files.", typeof(VerbosityAndSimulationGroup), @"--skip-download")]
			public OneOpt<bool> SkipDownLoad
			{
				get;
			} = new(false, @"--skip-download");

			/// <summary>
			/// Provides a <see cref="PrintTemplatesGroup"/> instance.
			/// </summary>
			public PrintTemplatesGroup PrintTemplates
			{
				get;
			} = new();

			/// <summary>
			/// Provides a <see cref="PrintAndTemplateFileOpt"/> instance.
			/// </summary>
			public PrintToFileGroup PrintToFile
			{
				get;
			} = new();

			/// <summary>
			/// If you set this to <see cref="ProgressModes.@default"/>, yt-dlp uses the mode set in that build unless a config file changes it.  If you use <see cref="ProgressModes.none"/>, yt-dlp won't display the progress.  If you use <see cref="ProgressModes.newLines"/>, yt-dlp will print a new line every time it updates the progress.  If you use <see cref="ProgressModes.progressBarEvenInQuietMode"/>, quiet mode won't disable the progress bar.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationProgressMode), @"If you set this to ProgressModes.@default, yt-dlp uses the mode set in that build unless a config file changes it.  If you use ProgressModes.none, yt-dlp won't display the progress.  If you use ProgressModes.newLine, yt-dlp will print a new line every time it updates the progress.  If you use ProgressModes.ProgressEvenInQuietMode, quiet mode won't disable the progress bar.", typeof(VerbosityAndSimulationGroup), @"--no-progress", @"--newline", @"--progress")]
			public OneOpt<ProgressModes> ProgressMode
			{
				get;
			} = new(ProgressModes.@default, string.Empty)
			{
				ParamListGenerator =
					opt
						=> opt.CurVal switch
							{
								ProgressModes.@default
									=> [],

								ProgressModes.none
									=> [@"--no-progress"],

								ProgressModes.newLines
									=> [@"--newline"],

								ProgressModes.progressBarEvenInQuietMode
									=> [@"--progress"],

								_
									=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<ProgressModes>(opt.CurVal, @"While selecting a parameter based on the progress " +
										@"bar mode"),
							},
			};

			/// <summary>
			/// Display progress in the console title bar.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationDisplayProgressInConsoleTitle), @"Display progress in the console title bar.", typeof(VerbosityAndSimulationGroup), @"--console-title")]
			public OneOpt<bool> DisplayProgressInConsoleTitle
			{
				get;
			} = new(false, @"--console-title");

			/// <summary>
			/// Groups options that specify a template string for displaying progress.
			/// </summary>
			public sealed class ProgressTemplateGroup
			{
				/// <summary>
				/// Specifies the template for displaying the progress of a download.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. <c><see cref="DownLoad"/>="%(info.id)s-%(progress.eta)s"</c>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationProgressTemplateDownLoad), @"Specifies the template for displaying the progress of a download.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. DownLoad=""%(info.id)s-%(progress.eta)s"".", typeof(VerbosityAndSimulationGroup), @"--progress-template")]
				public OneOptWithPrefix<string> DownLoad
				{
					get;
				} = new(string.Empty, @"--progress-template", @"download");

				/// <summary>
				/// Specifies the template for displaying the progress of a download in the titlebar.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. <c><see cref="DisplayProgressInConsoleTitle"/>=<see langword="true"/>; <see cref="DownLoadTitle"/>="%(info.id)s-%(progress.eta)s"</c>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationProgressTemplateDownLoadTitle), @"Specifies the template for displaying the progress of a download in the titlebar.  The video’s fields are accessible under the “info” key and  the progress attributes are accessible under “progress” key.  E.g. DisplayProgressInConsoleTitle=true; DownLoadTitle=""%(info.id)s-%(progress.eta)s"".", typeof(VerbosityAndSimulationGroup),
					@"--progress-template")]
				public OneOptWithPrefix<string> DownLoadTitle
				{
					get;
				} = new(string.Empty, @"--progress-template", @"download-title");

				/// <summary>
				/// Specifies the template for displaying the progress of post-processing.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. <c><see cref="PostProcess"/>=""%(info.id)s-%(progress.eta)s"</c>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationProgressTemplatePostProcess), @"Specifies the template for displaying the progress of post-processing.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. PostProcess=""%(info.id)s-%(progress.eta)s"".", typeof(VerbosityAndSimulationGroup), @"--progress-template")]
				public OneOptWithPrefix<string> PostProcess
				{
					get;
				} = new(string.Empty, @"--progress-template", @"postprocess");

				/// <summary>
				/// Specifies the template for displaying the progress of post-processing in the title bar.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. <c><see cref="DisplayProgressInConsoleTitle"/>=<see langword="true"/>; <see cref="PostProcessTitle"/>="%(info.id)s-%(progress.eta)s"</c>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationProgressTemplatePostProcessTitle), @"Specifies the template for displaying the progress of post-processing in the title bar.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. DisplayProgressInConsoleTitle=true; PostProcessTitle=""%(info.id)s-%(progress.eta)s"".", typeof(VerbosityAndSimulationGroup), @"--progress-template")]
				public OneOptWithPrefix<string>  PostProcessTitle
				{
					get;
				} = new(string.Empty, @"postprocess-title", @"post_process");


				/// <summary>
				/// Lists all options in <see cref="ProgressTemplateGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
					=> [
						DownLoad,
						DownLoadTitle,
						PostProcess,
						PostProcessTitle,
					];
			}

			/// <summary>
			/// Provides an instance of <see cref="ProgressTemplateGroup"/>
			/// </summary>
			public ProgressTemplateGroup ProgressTemplates
			{
				get;
			} = new();

			/// <summary>
			/// Force download archive entries to be written as far as no errors occur, even if <see cref="SimMode"/>/-s/--simulate or another simulation option is used.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationForceWriteArchive), @"Force download archive entries to be written as far as no errors occur, even if SimMode/-s/--simulate or another simulation option is used.", typeof(VerbosityAndSimulationGroup), @"--force-write-archive")]
			public OneOpt<bool> ForceWriteArchive
			{
				get;
			} = new(false, @"--force-write-archive");

			/// <summary>
			/// Time between progress output.  The default lets yt-dlp choose unless a config file uses --progress-delta.  Use <c><see cref="DeltaOfProgressUpdates"/>=0</c> to force yt-dlp to use its own default.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationDeltaOfProgressUpdates), @"Time between progress output.  The default lets yt-dlp choose unless a config file uses --progress-delta.  Use DeltaOfProgressUpdates=0 to force yt-dlp to use its own default.", typeof(VerbosityAndSimulationGroup), @"--progress-delta")]
			public OneOpt<ushort?> DeltaOfProgressUpdates
			{
				get;
			} = new(null, @"--progress-delta");

			/// <summary>
			/// Print downloaded pages encoded using base64 to debug problems (very verbose)
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationDumpPages), @"Print downloaded pages encoded using base64 to debug problems (very verbose)", typeof(VerbosityAndSimulationGroup), @"--dump-pages")]
			public OneOpt<bool> DumpPages
			{
				get;
			} = new(false, @"--dump-pages");

			/// <summary>
			/// Write downloaded intermediary pages to files in the current directory to debug problems
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationWritePages), @"Write downloaded intermediary pages to files in the current directory to debug problems", typeof(VerbosityAndSimulationGroup), @"--write-pages")]
			public OneOpt<bool> WritePages
			{
				get;
			} = new(false, @"--write-pages");

			/// <summary>
			/// Display sent and read HTTP traffic
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTraffic), @"Display sent and read HTTP traffic", typeof(VerbosityAndSimulationGroup), @"--print-traffic")]
			public OneOpt<bool> PrintTraffic
			{
				get;
			} = new(false, @"--print-traffic");


			/// <summary>
			/// Lists all options in a <see cref="VerbosityAndSimulationGroup"/> instance.  The parent of this instance will use <see cref="AllOpt"/> to build its own list.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					QuietMode,
					NoWarnings,
					SimMode,
					IgnoreNoFmtsError,
					SkipDownLoad,
					..PrintTemplates.AllOpt,
					..PrintToFile.AllOpt,
					ProgressMode,
					DisplayProgressInConsoleTitle,
					..ProgressTemplates.AllOpt,
					ForceWriteArchive,
					DeltaOfProgressUpdates,
					DumpPages,
					WritePages,
					PrintTraffic,
				];
		}

		/// <summary>
		/// Groups various options related to workarounds.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#workarounds">Workarounds section of yt-dlp’s readme.md</a>.
		/// </summary>
		public sealed class WorkAroundsGroup
		{
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
			};

			/// <summary>
			/// Work around terminals that lack bidirectional text support.  Requires bidiv or fribidi executable in PATH
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strWorkAroundsBIDI), @"Work around terminals that lack bidirectional text support.  Requires bidiv or fribidi executable in PATH", typeof(WorkAroundsGroup), @"--bidi-workaround")]
			public OneOpt<bool> BIDI
			{
				get;
			} = new(false, @"--bidi-workaround");

			/// <summary>
			/// Number of seconds to sleep between requests during data extraction
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strWorkAroundsSleepRequests), @"Number of seconds to sleep between requests during data extraction", typeof(WorkAroundsGroup), @"--sleep-requests")]
			public OneOpt<ushort?> SleepRequests
			{
				get;
			} = new(null, @"--sleep-requests");

			/// <summary>
			/// Controls how long yt-dlp sleeps for before each download.  Set <see cref="UShortRangeOpt.Min"/> to a non-<see langword="null"/> value to specify a minimum interval.  Set <see cref="UShortRangeOpt.Max"/> to a non-<see langword="null"/> value to specify a maximum interval.  Leave one or both to allow those values to come from any config file specifying any of the following: --sleep-interval, --min-sleep-interval, --max-sleep-interval.  Normally, yt-dlp doesn't sleep between downloads.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strWorkAroundsSleepIntervals), @"Controls how long yt-dlp sleeps for before each download.  Set SleepIntervals.Min to a non-null value to specify a minimum interval.  Set SleepIntervals.Max to a non-null value to specify a maximum interval.  Leave one or both to allow those values to come from any config file specifying any of the following: --sleep-interval, --min-sleep-interval, --max-sleep-interval.  Normally, yt-dlp doesn't sleep between downloads.", typeof(WorkAroundsGroup), @"--min-sleep-interval", @"--max-sleep-interval")]
			public UShortRangeOpt SleepIntervals
			{
				get;
			} = new(@"--min-sleep-interval", @"--max-sleep-interval");

			/// <summary>
			/// Number of seconds to sleep before each subtitle download
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strWorkAroundsSubsSleep), @"Number of seconds to sleep before each subtitle download", typeof(WorkAroundsGroup), @"--sleep-subtitles")]
			public OneOpt<bool> SubsSleep
			{
				get;
			} = new(false, @"--sleep-subtitles");


			/// <summary>
			/// Lists all options in a <see cref="WorkArounds"/> instance.  The owner of this instance will use <see cref="AllOpt"/> to generate its own list.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					Encoding,
					ExtraHdrs,
					BIDI,
					SleepRequests,
					SleepIntervals,
					SubsSleep,
				];
		}

		/// <summary>
		/// Groups various options related to video formats.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#video-format-options">Video Format Options section of yt-dlp’s readme.md</a>.
		/// </summary>
		public sealed class VidFmtGroup
		{
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
			public OneOpt<string> Sorting
			{
				get;
			} = new(string.Empty, @"--format-sort");

			/// <summary>
			/// If <see langword="true"/>, user-specified fields have precedence over all other fields.  If <see langword="false"/>, some fields will have precedence over those specified by the user.  For what user-specified fields were specified, see <see cref="Sorting"/>/--format-sort.  If you leave this at the default value of  <see langword="null"/>, a config file may set --format-sort-force or --no-format-sort-force.  By default, if <see cref="ForceSorting"/> is <see langword="null"/> and no config file sets a value, yt-dlp will act as though <see cref="ForceSorting"/> is <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidFmtForceSorting), @"If true, user-specified fields have precedence over all other fields.  If false, some fields will have precedence over those specified by the user.  For what user-specified fields were specified, see fmtSorting/--format-sort.  If you leave this at the default value of null, a config file may set --format-sort-force or --no-format-sort-force.  By default, if forceFmtSorting is null and no config file sets a value, yt-dlp will act as though forceFmtSorting is false.", typeof(VidFmtGroup), @"--format-sort-force",
				@"--no-format-sort-force")]
			public ThreeWayOpt ForceSorting
			{
				get;
			} = new(@"--format-sort-force", @"--no-format-sort-force");

			/// <summary>
			/// If <see langword="true"/>, multiple video streams can be merged into one file.  If <see langword="false"/>, that won’t be allowed.  If you use the default value of <see langword="null"/>, yt-dlp will act as though <see cref="VidMultiStreamsAllowed"/> is <see langword="false"/> unless a config file sets either --video-multistreams or --no-video-multistreams.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidFmtAllowVidMultiStreams), @"If true, multiple video streams can be merged into one file.  If false, that won't be allowed.  If you use the default value of null, yt-dlp will act as though VidMultiStreamsAlloweed is false unless a config file sets either --video-multistreams or --no-video-multistreams.", typeof(VidFmtGroup), @"--video-multistreams", @"--no-video-multistreams")]
			public ThreeWayOpt VidMultiStreamsAllowed
			{
				get;
			} = new(@"--video-multistreams", @"--no-video-multistreams");

			/// <summary>
			/// If <see langword="true"/>, multiple audio streams can be merged into one file.  If <see langword="false"/>, that won’t be allowed.  If you use the default value of <see langword="null"/>, yt-dlp will act as though <see cref="AudioMultiStreamsAllowed"/> is <see langword="false"/> unless a config file sets either --audio-multistreams or --no-audio-multistreams.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidFmtAllowAudioMultiStreams), @"If true, multiple audio streams can be merged into one file.  If false, that won't be allowed.  If you use the default value of null, yt-dlp will act as though AudioMultiStreamsAlloweed is false unless a config file sets either --audio-multistreams or --no-audio-multistreams.", typeof(VidFmtGroup), @"--audio-multistreams", @"--no-audio-multistreams")]
			public ThreeWayOpt AudioMultiStreamsAllowed
			{
				get;
			} = new(@"--audio-multistreams", @"--no-audio-multistreams");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will prefer formats with free containers over non-free containers with the same quality.  Use with <c><see cref="Sorting"/>="ext"</c> to prefer free formats regardless of quality.  If <see langword="false"/>, yt-dlp won’t give any special preferences to free formats.  If you use the default value of <see langword="null"/> and no config file specifies either --prefer-free-formats or --no-prefer-free-formats, yt-dlp will act as though PreferFree is <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidFmtPreferFree), @"If true, yt-dlp will prefer formats with free containers over non-free containers with the same quality.  Use with fmtSorting=""ext"" to prefer free formats regardless of quality.  If false, yt-dlp won't give any special preferences to free formats.  If you use the default value of null and no config file specifies either --prefer-free-formats or --no-prefer-free-formats, yt-dlp will act as though PreferFree is false.", typeof(VidFmtGroup), @"--prefer-free-formats", @"--no-prefer-free-formats")]
			public ThreeWayOpt PreferFree
			{
				get;
			} = new(@"--prefer-free-formats", @"--no-prefer-free-formats");

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
						}
			};

			/// <summary>
			/// Containers that may be used when merging formats, separated by ‘/’, e.g. <c><see cref="MergeOutput"/>="mp4/mkv"</c>.  Ignored if no merge is required.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVidFmtMergeOutput), @"Containers that may be used when merging formats, separated by ‘/’, e.g. MergeOutput=""mp4/mkv"".  Ignored if no merge is required.", typeof(VidFmtGroup), @"--merge-output-format")]
			public OneOpt<System.Collections.Generic.IEnumerable<SupportedMergeOutputFmts>> MergeOutput
			{
				get;
			} = new([], @"--merge-output-format")
			{
				HowToCombineVals = OneOpt<System.Collections.Generic.IEnumerable<SupportedMergeOutputFmts>>.ValCombinationRules.slash,
			};


			/// <summary>
			/// Lists all options in <see cref="VidFmtGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
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

		/// <summary>
		/// Groups various options related to video formats.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#subtitle-options">Subtitle Options section of yt-dlp’s readme.md</a>.
		/// </summary>
		public sealed class SubsGroup
		{
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
			} = new(@"--write-subs", @"--no-write-subs");

			/// <summary>
			/// If <see langword="true"/>, automatically generated subtitles will be written to disk.  If <see langword="false"/>, they won't be.  If you leave <see cref="WriteAutoFile"/> at the default value of <see langword="null"/>, yt-dlp will act as though <see cref="WriteAutoFile"/> is <see langword="false"/> unless a config file specifies either --write-auto-subs or --no-write-auto-subs.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strSubsWriteAutoFile), @"If true, automatically generated subtitles will be written to disk.  If false, they won't be.  If you leave WriteAutoFile at the default value of null, yt-dlp will act as though WriteAutoFile is false unless a config file specifies either --write-auto-subs or --no-write-auto-subs.", typeof(SubsGroup), @"--write-auto-subs", @"--no-write-auto-subs")]
			public ThreeWayOpt WriteAutoFile
			{
				get;
			} = new(@"--write-auto-subs", @"--no-write-auto-subs");

			/// <summary>
			/// Subtitle format; accepts formats preference separated by ‘/’, e.g. <c>"srt"</c> or <c>"ass/srt/best"</c>
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strSubsFmtForFiles), @"Subtitle format; accepts formats preference separated by ‘/’, e.g. ""srt"" or ""ass/srt/best""", typeof(SubsGroup), @"--sub-format")]
			public OneOpt<string> FmtForFiles
			{
				get;
			} = new(string.Empty, @"--sub-format");

			/// <summary>
			/// Languages of the subtitles to download (can be regex) or “all” separated by commas, e.g. <c><see cref="LangsToDownLoad"/>="en.*,ja"</c> (where “en.*” is a regex pattern that matches “en” followed by 0 or more of any character).  You can prefix the language code with a ‘-’ to exclude it from the requested languages, e.g. <c><see cref="LangsToDownLoad"/>="all,-live_chat"</c>.  Use Get-LangsForVid or <see cref="Goodies.YtDlpWrapper.Vid.Subs"/> to get a list of available languages.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strSubsLangsToDownLoad), @"Languages of the subtitles to download (can be regex) or “all” separated by commas, e.g. LangsToDownLoad=""en.*,ja"" (where “en.*” is a regex pattern that matches “en” followed by 0 or more of any character).  You can prefix the language code with a ‘-’ to exclude it from the requested languages, e.g. LangsToDownLoad=""all,-live_chat"".  Use Get-LangsForVid or WillPittenger.Goodies.YtDlpWrapper.Vid.Subs to get a list of available languages.", typeof(SubsGroup), @"--sub-langs")]
			public OneOpt<string> LangsToDownLoad
			{
				get;
			} = new(string.Empty, @"--sub-langs");


			/// <summary>
			/// Lists all options in <see cref="SubsGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					WriteFile,
					WriteAutoFile,
					FmtForFiles,
					LangsToDownLoad,
				];
		}

		/// <summary>
		/// Groups various options related to authentication.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#authentication-options">Authentication Options section of yt-dlp’s readme.md</a>.
		/// </summary>
		public sealed class AuthenticationGroup
		{
			/// <summary>
			/// Represents a user name and password combination in a single value.  Note: This value is ignored if either <see cref="UserName"/> or <see cref="Pwd"/> are empty.
			/// </summary>
			public sealed class UserNameAndPwdOpt() : IOneOpt
			{
				/// <summary>
				/// The user name
				/// </summary>
				public string UserName
				{
					get;

					set;
				} = string.Empty;

				/// <summary>
				/// The password
				/// </summary>
				public string Pwd
				{
					get;

					set;
				} = string.Empty;

				/// <inheritdoc/>
				public System.Collections.Generic.IEnumerable<string> Params
				{
					get
					{
						if(UserName == string.Empty && Pwd != string.Empty)
							throw new System.InvalidOperationException("If you specify a password, you must specify a user name.");

						#pragma warning disable IDE0046 // Convert to conditional expression
							if(Pwd == string.Empty && UserName != string.Empty)
								throw new System.InvalidOperationException("If you specify a user name, you must specify a password.");
						#pragma warning restore IDE0046 // Convert to conditional expression

						return [@"--username", UserName, @"--password", Pwd];
					}
				}
			}


			/// <summary>
			/// Specifies how to log in with user name and password.  Note: If you use a user name, you must provide a password and vice-versa.  Support depends on the website.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strAuthenticationUserNameAndPwd), @"Specifies how to log in with user name and password.  Note: If you use a user name, you must provide a password and vice-versa.  Support depends on the website.", typeof(AuthenticationGroup), @"--username",
				@"--password")]
			public UserNameAndPwdOpt UserNameAndPwd
			{
				get;
			} = new();

			/// <summary>
			/// Two-factor authentication code
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strAuthenticationTwoFactor), @"Two-factor authentication code", typeof(AuthenticationGroup),
				@"--twofactor")]
			public OneOpt<string> TwoFactor
			{
				get;
			} = new("", @"--twofactor");

			/// <summary>
			/// Use .netrc authentication data
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strAuthenticationNetRC), @"Use .netrc authentication data", typeof(AuthenticationGroup),
				@"--netrc")]
			public OneOpt<bool> NetRC
			{
				get;
			} = new(false, @"--netrc");

			/// <summary>
			/// Video-specific password
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strAuthenticationVidSpecificPwd), @"Video-specific password", typeof(AuthenticationGroup),
				@"--video-password")]
			public OneOpt<string> VidSpecificPwd
			{
				get;
			} = new("", @"--video-password");


			/// <summary>
			/// Lists all options in <see cref="AuthenticationGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					UserNameAndPwd,
					TwoFactor,
					NetRC,
					VidSpecificPwd,
				];
		}

		/// <summary>
		/// Groups various options related to post-processing.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#post-processing-options">Post-Processing Options section of yt-dlp’s readme.md</a>.
		/// </summary>
		public sealed class PostProcessingGroup
		{
			/// <summary>
			/// Describes how to parse metadata.If either <see cref="strFrom"/> or <see cref="strTo"/> are empty strings, nothing will be emitted.
			/// </summary>
			public sealed record ParseMetaDataInstructOpt : IOneOptVal
			{
				/// <summary>
				/// The literal text to send to yt-dlp before the colon.  Represents the source of data.  Can be a field name or a Python format string.
				/// </summary>
				public string strFrom = string.Empty;

				/// <summary>
				/// The literal text to send to yt-dlp after the colon.  Represents the destination of the data.  Can be a field name or a Python format string.
				/// </summary>
				public string strTo = string.Empty;


				/// <summary>
				/// The combination of <see cref="strFrom"/> and <see cref="strTo"/> in the format specified by yt-dlp.
				/// </summary>
				public string? ValText
					=> strFrom == string.Empty || strTo == string.Empty
						? null
						: $"{strFrom}:{strTo}";
			}

			/// <summary>
			/// Describes how to replace metadata.  All fields must be specified.
			/// </summary>
			/// <param name="strPrefix">Specifies the stage at which replacement will happen.  This must be the exact string expected by yt-dlp.  Use a raw string and not something from the RESX.</param>
			public sealed class ReplaceInMetaDataInstructOpt(in string strPrefix) : IOneOpt
			{
				/// <summary>
				/// The stage at which replacement will happen.
				/// </summary>
				public readonly string strPrefix = strPrefix;

				/// <summary>
				/// A set of field names or Python formatting strings.  Use one entry per field.
				/// </summary>
				public readonly System.Collections.Generic.SortedSet<string> strsetFields = [];

				/// <summary>
				/// The regular expression to send to yt-dlp.
				/// </summary>
				public System.Text.RegularExpressions.Regex? regex = null;

				/// <summary>
				/// The new text.
				/// </summary>
				public string strReplace = string.Empty;


				/// <summary>
				/// Generates a list in the format expected by yt-dlp.  Nothing happens if either <see cref="strsetFields"/> is empty, <see cref="regex"/> is <see langword="null"/>, or <see cref="strReplace"/> is empty.
				/// </summary>
				public System.Collections.Generic.IEnumerable<string> Params
					=> strsetFields.Count <= 0 || regex == null || strReplace == string.Empty
						? []
						: [@"--replace-in-metadata", $"{strPrefix}:{strsetFields.Join(",")}", regex.ToString(), strReplace];
			}

			/// <summary>
			/// Provides a series of options for how to fix problems.  All allowed values are <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members.
			/// </summary>
			public sealed class FixUpPolicyChoices
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
			/// Provides choices for what stage of post-processing a set of instructions should be used  The only allowed values are all <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members.
			/// </summary>
			public sealed class WhenValsForPostProcessing
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
			}

			/// <summary>
			/// Provides choices for what post-processing event a set of instructions should be used  The only allowed values are all <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members.
			/// </summary>
			public sealed class PreDefinedProcessorEvts
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
			}

			/// <summary>
			/// Provides choices for what stage of post-processing a set of instructions should be used  The only allowed values are all <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members if either <see cref="SupportedProcessorExecutables"/> or <see cref="SupportedFfmpegProcessorExecutables"/>.
			/// </summary>
			public class SupportedProcessorExecutables
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
									=> $"{kvCur.Key}={kvCur.Value}"
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
			public sealed record PostProcessingRecordForKnownProcessorVal(in PreDefinedProcessorEvts? WhichEvt = null, in SupportedProcessorExecutables?
				WhichProcessorExecutable = null, in System.Collections.Generic.IReadOnlyDictionary<string, string>? Args = null, in WhenValsForPostProcessing? When =
				null) : BasePostProcessorArgsVal(Args, When)
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
							? $"{WhichEvt.strTextName}:{base.ValText}"
							: WhichEvt == null && WhichProcessorExecutable != null
								? $"{WhichProcessorExecutable.strName}:{base.ValText}"
								: $"{WhichEvt}+{WhichProcessorExecutable}:{base.ValText}";


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

			/// <summary>
			/// Describes a post-processing record for any executable that yt-dlp does NOT know about.  You specify the executable with a <see cref="System.IO.FileInfo"/> instance.  Note: Always use the full path with <see cref="PostProcessorArgsForExeVal"/> instances.  If you’re specifying an executable from your system path, use <see cref="NamedExeProcessorArgsVal"/> instead.  This is because <see cref="ValText"/> uses <see cref="System.IO.FileSystemInfo.FullName"/> which assumes the path is relative to the current working directory.
			/// </summary>
			/// <param name="fileExe">A <see cref="System.IO.FileInfo"/> with the full path of the file</param>
			/// <param name="Args">The arguments to send</param>
			/// <param name="When">When to trigger the processor.  Use a value from <see cref="WhenValsForPostProcessing"/>.  You can pass <see langword="null"/> here.  That’s treated as <see cref="WhenValsForPostProcessing.all"/>.</param>
			public sealed record PostProcessorArgsForExeVal(in System.IO.FileInfo fileExe, in System.Collections.Generic.IReadOnlyDictionary<string, string>? Args =
				null, in WhenValsForPostProcessing? When = null) : BasePostProcessorArgsVal(Args, When)
			{
				/// <summary>
				/// A <see cref="System.IO.FileInfo"/> with the full path of the file
				/// </summary>
				public System.IO.FileInfo fileExe = fileExe;


				/// <summary>
				/// The value text in the syntax expected by yt-dlp
				/// </summary>
				public override string? ValText
					=> (Args == null || Args.Count == 0) && When == WhenValsForPostProcessing.all
						? fileExe.FullName
						: (Args == null || Args.Count == 0) && When != WhenValsForPostProcessing.all
							? $"{fileExe.FullName}:when={When.strText}"
							: $"{fileExe.FullName}:{base.ValText}";


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
			public sealed record NamedExeProcessorArgsVal(in string strExeFileNameWithOutPath, in System.Collections.Generic.IReadOnlyDictionary<string, string>? Args
				= null, in WhenValsForPostProcessing? When = null) : BasePostProcessorArgsVal(Args, When)
			{
				/// <summary>
				/// The name of the file to use
				/// </summary>
				public string strExeFileNameWithOutPath = strExeFileNameWithOutPath;


				/// <summary>
				/// Returns the value text in the syntax expected by yt-dlp
				/// </summary>
				public override string? ValText
					=> (Args == null || Args.Count == 0) && When == WhenValsForPostProcessing.all
						? strExeFileNameWithOutPath
						: (Args == null || Args.Count == 0) && When != WhenValsForPostProcessing.all
							? $"{strExeFileNameWithOutPath}:{base.ValText}"
							: $"{strExeFileNameWithOutPath}:{base.ValText}";


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
				/// <summary>
				/// If <see langword="true"/>, yt-dlp will embed subtitle information into the video file.  If <see langword="false"/>, it won’t.  Ignored if the final video file isn’t mp4, webm, or mkv.  If you leave <see cref="Subs"/> set to the default value of <see langword="null"/> and no config file specifies either --embed-subs or --no-embed-subs, yt-dlp will act as though <see cref="Subs"/> was set to <see langword="false"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingEmbedSubs), @"If true, yt-dlp will embed subtitle information into the video file.  If false, it won’t.  Ignored if the final video file isn’t mp4, webm, or mkv.  If you leave Subs set to the default value of null and no config file specifies either --embed-subs or --no-embed-subs, yt-dlp will act as though Subs was set to false.", typeof(PostProcessingGroup), @"--embed-subs", @"--no-embed-subs")]
				public ThreeWayOpt Subs
				{
					get;
				} = new(@"--embed-subs", @"--no-embed-subs");

				/// <summary>
				/// If <see langword="true"/>, yt-dlp will embed a thumbnail into the post-processed file as cover art.  If <see langword="false"/>, it won’t do that.  If you leave <see cref="Thumb"/> set to the default value of <see langword="null"/> and no config file specifies either --embed-thumbnail or --no-embed-thumbnail, yt-dlp will act as though <see cref="Thumb"/> were set to <see langword="false"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingEmbedThumb), @"If true, yt-dlp will embed a thumbnail into the post-processed file as cover art.  If false, it won’t do that.  If you leave Thumb set to the default value of null and no config file specifies either --embed-thumbnail or --no-embed-thumbnail, yt-dlp will act as though Thumb were set to false.", typeof(PostProcessingGroup), @"--embed-thumbnail", @"--no-embed-thumbnail")]
				public ThreeWayOpt Thumb
				{
					get;
				} = new(@"--embed-thumbnail", @"--no-embed-thumbnail");

				/// <summary>
				/// If <see langword="true"/>, yt-dlp will embed metadata information into the post-processed file.  If <see langword="false"/>, it won’t.  If you leave <see cref="MetaData"/> set to the default value of <see langword="null"/> and no config file specifies either --embed-metadata or --no-embed-metadata, yt-dlp will act as though <see cref="MetaData"/> is set to <see langword="false"/>.  Note: If <see cref="MetaData"/> is <see langword="true"/>, the defaults for embedding chapter and info json data changes to reflect the <see cref="MetaData"/> value.  With both types of data, setting <see cref="MetaData"/> to <see langword="true"/> will cause yt-dlp to default to embedding both chapter and info json data as well.  If you don’t want that, set <see cref="Chapters"/> and/or <see cref="InfoJSON"/> to <see langword="false"/>.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingEmbedMetaData), @"If true, yt-dlp will embed metadata information into the post-processed file.  If false, it won’t.  If you leave it set to the default value of null and no config file specifies either --embed-metadata or --no-embed-metadata, yt-dlp will act as though MetaData is set to false.  Note: If MetaData is true, the defaults for embedding chapter and info json data changes to reflect the MetaData value.  With both types of data, setting MetaData to true will cause yt-dlp to default to embedding both chapter and info json data as well.  If you don't want that, set Chapters and/or EmbedInfoJSON to false.", typeof(PostProcessingGroup), @"--embed-metadata",
					@"--no-embed-metadata")]
				public ThreeWayOpt MetaData
				{
					get;
				} = new(@"--embed-metadata", @"--no-embed-metadata");

				/// <summary>
				/// If <see langword="true"/>, yt-dlp will embed chapters into the post-processed file.  If <see langword="false"/>, it won’t do that.  If you leave <see cref="Chapters"/> set to the default value of <see langword="null"/> and no config file specifies --embed-chapters or --no-embed-chapters, yt-dlp will use a value that depends on if metadata is being embedded.  Unless told otherwise by either <see cref="Chapters"/> or a config file with --embed-chapters, yt-dlp will embed chapters if metadata is being embedded and won’t embed them if meta data isn’t being embedded.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingEmbedChapters), @"If true, yt-dlp will embed chapters into the post-processed file.  If false, it won’t do that.  If you leave EmbedChapters set to the default value of null and no config file specifies --embed-chapters or --no-embed-chapters, yt-dlp will use a value that depends on if metadata is being embedded.  Unless told otherwise by either Chapters or a config file with --embed-chapters, yt-dlp will embed chapters if metadata is being embedded and won’t embed them if meta data isn't being embedded.", typeof(PostProcessingGroup), @"--embed-chapters", @"--no-embed-chapters")]
				public ThreeWayOpt Chapters
				{
					get;
				} = new(@"--embed-chapters", @"--no-embed-chapters");

				/// <summary>
				/// If <see langword="true"/>, yt-dlp will embed info JSON data into the post-processed file.  If <see langword="false"/>, it won’t do that.  If you leave <see cref="InfoJSON"/> set to the default value of <see langword="null"/> and no config file specifies --embed-infojson or --no-embed-infojson, yt-dlp will use a value that depends on if metadata is being embedded.  Unless told otherwise by either <see cref="InfoJSON"/> or a config file, yt-dlp will embed info JSON data if metadata is being embedded and won’t embed it if meta data isn’t being embedded.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingEmbedInfoJSON), @"If true, yt-dlp will embed info JSON data into the post-processed file.  If false, it won’t do that.  If you leave InfoJSON set to the default value of null and no config file specifies --embed-infojson or --no-embed-infojson, yt-dlp will use a value that depends on if metadata is being embedded.  Unless told otherwise by either InfoJSON or a config file, yt-dlp will embed info JSON data if metadata is being embedded and won’t embed it if meta data isn’t being embedded.", typeof(PostProcessingGroup),
					@"--embed-infojson", @"--no-embed-infojson")]
				public ThreeWayOpt InfoJSON
				{
					get;
				} = new(@"--embed-infojson", @"--no-embed-infojson");


				/// <summary>
				/// Lists all options in <see cref="EmbedGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
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
			public sealed class ParseMetaDataInstructGroup
			{
				/// <summary>
				/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataPreProcess), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won't be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
				public OneOptWithPrefix<ParseMetaDataInstructOpt> PreProcess
				{
					get;
				} = new(new(), @"--parse-metadata", @"pre_process");

				/// <summary>
				/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataAfterFilter), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
				public OneOptWithPrefix<ParseMetaDataInstructOpt> AfterFilter
				{
					get;
				} = new(new(), @"--parse-metadata", @"after_filter");

				/// <summary>
				/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataVid), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
				public OneOptWithPrefix<ParseMetaDataInstructOpt> Vid
				{
					get;
				} = new(new(), @"--parse-metadata", @"video");

				/// <summary>
				/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataBeforeDownLoad), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
				public OneOptWithPrefix<ParseMetaDataInstructOpt> BeforeDownLoad
				{
					get;
				} = new(new(), @"--parse-metadata", @"before_dl");

				/// <summary>
				/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataPostProcess), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
				public OneOptWithPrefix<ParseMetaDataInstructOpt> PostProcess
				{
					get;
				} = new(new(), @"--parse-metadata", @"post_process");

				/// <summary>
				/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataAfterMove), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
				public OneOptWithPrefix<ParseMetaDataInstructOpt> AfterMove
				{
					get;
				} = new(new(), @"--parse-metadata", @"after_move");

				/// <summary>
				/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataAfterVid), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
				public OneOptWithPrefix<ParseMetaDataInstructOpt> AfterVid
				{
					get;
				} = new(new(), @"--parse-metadata", @"after_video");

				/// <summary>
				/// Parse additional metadata like title/artist from other fields; see <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata">MODIFYING METADATA</a> for details.  Note: This parameter won’t be emitted unless you set both fields.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingParseMetaDataPlayList), @"Parse additional metadata like title/artist from other fields; see https://github.com/yt-dlp/yt-dlp/blob/master/README.md#modifying-metadata for details.  Note: This parameter won’t be emitted unless you set both fields.", typeof(PostProcessingGroup), @"--parse-metadata")]
				public OneOptWithPrefix<ParseMetaDataInstructOpt> PlayList
				{
					get;
				} = new(new(), @"--parse-metadata", @"playlist");


				/// <summary>
				/// Lists all options in <see cref="ParseMetaDataInstructGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to build its own list of options.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
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
			/// Groups all of the options for replacing metadata.  There’s one option for each value from <see cref="PreDefinedProcessorEvts"/>.  All use --replace-in-metadata.
			/// </summary>
			public sealed class ReplaceInMetaDataInstructGroup
			{
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


				/// <summary>
				/// Lists all options in <see cref="ReplaceInMetaDataInstructGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
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
			/// Groups all options for running an executable at the specified time.  Contains one option per value in <see cref="PreDefinedProcessorEvts"/>.  Each option results in a --exec.
			/// </summary>
			public sealed class ExecCmdGroup
			{
				/// <summary>
				/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdPreProcess), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
				public OneOptWithPrefix<string> PreProcess
				{
					get;
				} = new(string.Empty, @"--exec", @"pre_process");

				/// <summary>
				/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdAfterFilter), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
				public OneOptWithPrefix<string> AfterFilter
				{
					get;
				} = new(string.Empty, @"--exec", @"after_filter");

				/// <summary>
				/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdVid), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
				public OneOptWithPrefix<string> Vid
				{
					get;
				} = new(string.Empty, @"--exec", @"video");

				/// <summary>
				/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdBeforeDownLoad), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
				public OneOptWithPrefix<string> BeforeDownLoad
				{
					get;
				} = new(string.Empty, @"--exec", @"before_dl");

				/// <summary>
				/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdBeforeDownLoad), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
				public OneOptWithPrefix<string> PostProcess
				{
					get;
				} = new(string.Empty, @"--exec", @"post_process");

				/// <summary>
				/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdAfterMove), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
				public OneOptWithPrefix<string> AfterMove
				{
					get;
				} = new(string.Empty, @"--exec", @"after_move");

				/// <summary>
				/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdAfterVid), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
				public OneOptWithPrefix<string> AfterVid
				{
					get;
				} = new(string.Empty, @"--exec", @"after_video");

				/// <summary>
				/// Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdPlayList), @"Executes a command in your shell.  On Windows, this will be a cmd.exe shell.  The same syntax as the output template can be used to pass any field as arguments to the command.  If no fields are passed, “%(filepath,_filename|)q” is appended to the end of the command.", typeof(PostProcessingGroup), @"--exec")]
				public OneOptWithPrefix<string> PlayList
				{
					get;

					private init;
				} = new(string.Empty, @"--exec", @"playlist");

				/// <summary>
				/// Remove any previously defined execution command including any set using --exec in a config file.
				/// </summary>
				[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingExecCmdNone), @"Remove any previously defined execution command including any set using --exec in a config file.", typeof(PostProcessingGroup), @"--no-exec")]
				public OneOpt<bool> None
				{
					get;
				} = new(false, @"--no-exec");


				/// <summary>
				/// Lists all options in <see cref="ExecCmdGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
				/// </summary>
				public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
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
			public OneOpt<string> ExtractAudioToFmt
			{
				get;
			} = new(string.Empty, string.Empty)
			{
				ParamListGenerator =
					opt
						=> opt.CurVal == string.Empty
						? []
						: [@"--extract-audio", @"--audio-format", opt.CurVal],
			};

			/// <summary>
			/// Specify ffmpeg audio quality to use when converting the audio.  Insert a value between <c>"0"</c> (best) and <c>"10"</c> (worst) for VBR or a specific bitrate like <c>"128K"</c> (default <c>"5"</c>)
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingAudioQuality), @"Specify ffmpeg audio quality to use when converting the audio.  Insert a value between 0 (best) and 10 (worst) for VBR or a specific bitrate like 128K (default 5)", typeof(PostProcessingGroup), @"--audio-quality")]
			public OneOpt<string> AudioQuality
			{
				get;
			} = new(string.Empty, @"--audio-quality");

			/// <summary>
			/// Remux the video into another container if necessary (currently supported: avi, flv, gif, mkv, mov, mp4, webm, aac, aiff, alac, flac, m4a, mka, mp3, ogg, opus, vorbis, wav).  If the target container doesn’t support the video/audio codec, remuxing will fail.  You can specify multiple rules; e.g. <c><see cref="RemuxVid"/>="aac>m4a/mov>mp4/mkv"</c> will remux aac to m4a, mov to mp4 and anything else to mkv.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingRemuxVid), @"Remux the video into another container if necessary (currently supported: avi, flv, gif, mkv, mov, mp4, webm, aac, aiff, alac, flac, m4a, mka, mp3, ogg, opus, vorbis, wav).  If the target container doesn’t support the video/audio codec, remuxing will fail.  You can specify multiple rules; e.g. RemuxVid=""aac>m4a/mov>mp4/mkv"" will remux aac to m4a, mov to mp4 and anything else to mkv.",
				typeof(PostProcessingGroup), @"--remux-video")]
			public OneOpt<string> RemuxVid
			{
				get;
			} = new(string.Empty, @"--remux-video");

			/// <summary>
			/// Remux the video into another container if necessary (currently supported: avi, flv, gif, mkv, mov, mp4, webm, aac, aiff, alac, flac, m4a, mka, mp3, ogg, opus, vorbis, wav).  You can specify multiple rules; e.g. <c><see cref="RecodeVid"/>="aac>m4a/mov>mp4/mkv"</c> will remux aac to m4a, mov to mp4 and anything else to mkv.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostPocessingRecodeVid), @"Remux the video into another container if necessary (currently supported: avi, flv, gif, mkv, mov, mp4, webm, aac, aiff, alac, flac, m4a, mka, mp3, ogg, opus, vorbis, wav).  You can specify multiple rules; e.g. RecodeVid=""aac>m4a/mov>mp4/mkv"" will remux aac to m4a, mov to mp4 and anything else to mkv.", typeof(PostProcessingGroup), @"--recode-video")]
			public OneOpt<string> RecodeVid
			{
				get;
			} = new(string.Empty, @"--recode-video");

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

							System.Collections.Generic.IEnumerable<string[]> groupArgs = from BasePostProcessorArgsVal curArg in opt.CurVal
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
			};

			/// <summary>
			/// If <see langword="true"/>, yt-dlp won’t delete the video after post-processing.  If <see langword="false"/>, it would.  If you leave <see cref="KeepVid"/> set to the default value of <see langword="null"/> and a config file doesn't specify either --keep-video or --no-keep-video, yt-dlp will act as though <see cref="KeepVid"/> is <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingKeepVid), @"If true, yt-dlp won't delete the video after post-processing.  If false, it would.  If you leave KeepVid set to the default value of null and a config file doesn't specify either --keep-video or --no-keep-video, yt-dlp will act as though KeepVid is false.", typeof(PostProcessingGroup), @"--keep-video", @"--no-keep-video")]
			public ThreeWayOpt KeepVid
			{
				get;
			} = new(@"--keep-video", @"--no-keep-video");

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
			} = new(false, @"--xattrs");

			/// <summary>
			/// Automatically correct known faults of the file.  One of <see cref="FixUpPolicyChoices.never"/> (do nothing), <see cref="FixUpPolicyChoices.warn"/> (only emit a warning), <see cref="FixUpPolicyChoices.detectOrWarn"/> (fix the file if we can, warn otherwise), <see cref="FixUpPolicyChoices.force"/> (try fixing even if the file already exists).  If you leave <see cref="FixUpPolicy"/> set to the default value of <see cref="FixUpPolicyChoices.@default"/> and no config file sets --fixup, yt-dlp will act as though <see cref="FixUpPolicy"/> was set to <see cref="FixUpPolicyChoices.detectOrWarn"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingFixUpPolicy), @"Automatically correct known faults of the file.  One of FixUpPolicyChoices.never (do nothing), FixUpPolicyChoices.warn (only emit a warning), FixUpPolicyChoices.detectOrWarn (fix the file if we can, warn otherwise), FixUpPolicyChoices.force (try fixing even if the file already exists).  If you leave FixUpPolicy set to the default value of FixUpPolicyChoices.@default and no config file sets --fixup, yt-dlp will act as though FixUpPolicy was set to FixUpPolicyChoices.detectOrWarn.", typeof(PostProcessingGroup),
				@"--fixup")]
			public OneOpt<FixUpPolicyChoices> FixUpPolicy
			{
				get;
			} = new(FixUpPolicyChoices.@default, @"--fixup");

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
			public OneOpt<SubsGroup.SupportedFmts> ConvertSubs
			{
				get;
			} = new(SubsGroup.SupportedFmts.@default, @"--convert-subs");

			/// <summary>
			/// Convert the thumbnails to another format (currently supported: jpg, png, webp).  You can specify multiple rules; e.g. <c><see cref="ConvertThumbs"/>="jpg>webp/png"</c> will convert jpg to webp, and anything else to png.  Set <see cref="ConvertThumbs"/> to “none” to disable conversion (default).
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingConvertThumbs), @"Convert the thumbnails to another format (currently supported: jpg, png, webp).  You can specify multiple rules; e.g. ConvertThumbs=""jpg>webp/png"" will convert jpg to webp, and anything else to png.  Set ConvertThumbs to “none” to disable conversion (default).", typeof(PostProcessingGroup), @"--convert-thumbnails")]
			public OneOpt<string> ConvertThumbs
			{
				get;
			} = new(string.Empty, @"--convert-thumbnails");

			/// <summary>
			/// Splits videos into multiple videos based on chapters.  Use with <see cref="FileSysGroup.PathsGroup.Chapters"/> and <see cref="FileSysGroup.TemplatesForOutputFilesGroup.Chapters"/> to specify the file name.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingSplitChapters), @"Splits videos into multiple videos based on chapters.  Use with FileSysGroup.PathsGroup.Chapters and FileSysGroup.TemplatesForOutputFilesGroup.Chapters to specify the file name.", typeof(PostProcessingGroup),
				@"--split-chapters", @"--no-split-chapters")]
			public ThreeWayOpt SplitChapters
			{
				get;
			} = new(@"--split-chapters", @"--no-split-chapters");

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
									)
			};

			/// <summary>
			/// If <see langword="true"/>, forces key frames at cuts when downloading, splitting, or removing sections.  This is slow due to needing to recode, but the resulting video might have fewer artifacts around the cuts.  If <see cref="ForceKeyFramesAtCuts"/> is <see langword="false"/>, yt-dlp won’t force them.  If you leave <see cref="ForceKeyFramesAtCuts"/> set to the default value of <see langword="false"/> and no config file specifies --force-keyframes-at-cuts, yt-dlp will act as though <see cref="ForceKeyFramesAtCuts"/> is <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strPostProcessingForceKeyFramesAtCuts), @"If true, forces key frames at cuts when downloading, splitting, or removing sections.  This is slow due to needing to recode, but the resulting video might have fewer artifacts around the cuts.  If ForceKeyFramesAtCuts is false, yt-dlp won’t force them.  If you leave ForceKeyFramesAtCuts set to the default value of null and no config file specifies --force-keyframes-at-cuts, yt-dlp will act as though ForceKeyFramesAtCuts is false.", typeof(PostProcessingGroup), @"--force-key-frames-at-cuts",
				@"--no-force-keyframes-at-cuts")]
			public ThreeWayOpt ForceKeyFramesAtCuts
			{
				get;
			} = new(@"--force-key-frames-at-cuts", @"--no-force-keyframes-at-cuts");


			/// <summary>
			/// Lists all options in <see cref="PostProcessingGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to generate its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					ExtractAudioToFmt,
					AudioQuality,
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

		/// <summary>
		/// Groups various options related to SponsorBlock.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#sponsorblock-options">SponsorBlock Options:</a>.
		/// </summary>
		public sealed class SponsorBlockGroup
		{
			/// <summary>
			/// SponsorBlock categories to create chapters for, separated by commas.  Available categories are “sponsor”, “intro”, “outro”, “selfpromo”, “preview”, “filler”, “interaction”, “music_offtopic”, “poi_highlight”, “chapter”, “all”, and “default” (all).  You can prefix the category with a “-” to exclude it.  See https://wiki.sponsor.ajay.app/w/Segment_Categories for descriptions of the categories.  E.g. <c><see cref="MarkCats"/>="all,-preview"</c>
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strSponsorBlockMarkCats), @"SponsorBlock categories to create chapters for, separated by commas.  Available categories are “sponsor”, “intro”, “outro”, “selfpromo”, “preview”, “filler”, “interaction”, “music_offtopic”, “poi_highlight”, “chapter”, “all”, and “default” (=all).  You can prefix the category with a “-” to exclude it.  See https://wiki.sponsor.ajay.app/w/Segment_Categories for descriptions of the categories.  E.g. MarkCats=""all,-preview""", typeof(SponsorBlockGroup), @"--sponsorblock-mark")]
			public OneOpt<string> MarkCats
			{
				get;
			} = new(string.Empty, @"--sponsorblock-mark");

			/// <summary>
			/// SponsorBlock categories to be removed from the video file, separated by commas.  If a category is present in both <see cref="MarkCats"/> and <see cref="RemoveCats"/>, the <see cref="RemoveCats"/> entry takes precedence.   Available categories are “sponsor”, “intro”, “outro”, “selfpromo”, “preview”, “filler”, “interaction”, “music_offtopic”, “all” (equals “sponsor,intro,outro,selfpromo,preview,filler,interaction,music_offtopic”), and “default” (equals “all,-filler”).  You can prefix the category with a “-” to exclude it.  See https://wiki.sponsor.ajay.app/w/Segment_Categories for descriptions of the categories. E.g. <c><see cref="MarkCats"/>="all,-preview"; <see cref="RemoveCats"/>="sponsor,selfpromo,interaction"</c>
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strSponsorBlockRemoveCats), @"SponsorBlock categories to be removed from the video file, separated by commas.  If a category is present in both MarkCats and RemoveCats, the RemoveCats entry takes precedence.   Available categories are “sponsor”, “intro”, “outro”, “selfpromo”, “preview”, “filler”, “interaction”, “music_offtopic”, “all” (equals “sponsor,intro,outro,selfpromo,preview,filler,interaction,music_offtopic”), and “default” (equals “all,-filler”).  You can prefix the category with a “-” to exclude it.  See https://wiki.sponsor.ajay.app/w/Segment_Categories for descriptions of the categories. E.g.MarkCats=""all,-preview""; RemoveCats=""sponsor,selfpromo,interaction""", typeof(SponsorBlockGroup), @"--sponsorblock-remove")]
			public OneOpt<string> RemoveCats
			{
				get;
			} = new(string.Empty, @"--sponsorblock-remove");

			/// <summary>
			/// An output template for the title of the SponsorBlock chapters created by <see cref="MarkCats"/>/--sponsorblock-mark.  The only available fields are start_time, end_time, category, categories, name, category_names. Defaults to “[SponsorBlock]: %(category_names)l”.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strSponsorBlockChapterTitle), @"An output template for the title of the SponsorBlock chapters created by MarkCats/--sponsorblock-mark.  The only available fields are start_time, end_time, category, categories, name, category_names. Defaults to “[SponsorBlock]: %(category_names)l”.", typeof(SponsorBlockGroup), @"--sponsorblock-chapter-title")]
			public OneOpt<string> ChapterTitle
			{
				get;
			} = new(string.Empty, @"--sponsorblock-chapter-title");

			/// <summary>
			/// Disables both <see cref="MarkCats"/>/--sponsorblock-mark and <see cref="RemoveCats"/>/--sponsorblock-remove.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strSponsorBlockDisable), @"Disables both MarkCats/--sponsorblock-mark and RemoveCats/--sponsorblock-remove.", typeof(SponsorBlockGroup), @"--no-sponsorblock")]
			public OneOpt<bool> Disable
			{
				get;
			} = new(false, @"--no-sponsorblock");

			/// <summary>
			/// SponsorBlock API location, defaults to https://sponsor.ajay.app
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strSponsorBlockApiURL), @"SponsorBlock API location, defaults to https://sponsor.ajay.app", typeof(SponsorBlockGroup), @"--sponsorblock-api")]
			public OneOpt<System.Uri?> ApiURL
			{
				get;
			} = new(null, @"--sponsorblock-api");


			/// <summary>
			/// Lists all options in <see cref="SponsorBlockGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					MarkCats,
					RemoveCats,
					ChapterTitle,
					Disable,
					ApiURL,
				];
		}

		/// <summary>
		/// Groups various options related to calling the extractors.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#extractor-options">Extractor Options section of yt-dlp’s readme.md</a>.  Additional relevant information can be found at <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#extractor-arguments">the later Extractor Arguments section</a>.
		/// </summary>
		public sealed class ExtractorGroup
		{
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
			} = new(null, @"--extractor-retries");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp process dynamic DASH manifests.  If <see langword="false"/>. yt-dlp won’t.  If <see cref="AllowDynamicMPD"/> is left at the default value of <see langword="null"/> and no config file specifies --allow-dynamic-mpd or --no-ignore-dynamic-mpd, yt-dlp will act as though <see cref="AllowDynamicMPD"/> is <see langword="true"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strExtractorAllowDynamicMPD), @"If true, yt-dlp process dynamic DASH manifests.  If false. yt-dlp won’t.  If AllowDynamicMPD is left at the default value of null and no config file specifies --allow-dynamic-mpd or --no-ignore-dynamic-mpd, yt-dlp will act as though AllowDynamicMPD is true.", typeof(ExtractorGroup), @"--allow-dynamic-mpd", @"--ignore-dynamic-mpd")]
			public ThreeWayOpt AllowDynamicMPD
			{
				get;
			} = new(@"--allow-dynamic-mpd", @"--ignore-dynamic-mpd");

			/// <summary>
			/// If <see langword="true"/>, yt-dlp will split HLS playlists to different formats at discontinuities such as ad breaks.  If <see langword="false"/>, it won’t do that and the same formats will be used throughout.  If you leave <see cref="SplitHlsDiscontinuities"/> set to the default value of <see langword="null"/> and no config file specifies --hls-split-discontinuity, yt-dlp will act as though <see cref="SplitHlsDiscontinuities"/> equals <see langword="false"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strExtractorSplitHlsDiscontinuities), @"If true, yt-dlp will split HLS playlists to different formats at discontinuities such as ad breaks.  If false, it won’t do that and the same formats will be used throughout.  If you leave SplitHlsDiscontinuities set to the default value of null and no config file specifies --hls-split-discontinuity, yt-dlp will act as though SplitHlsDiscontinuities equals false.", typeof(ExtractorGroup), @"--hls-split-discontinuity", @"--no-hls-split-discontinuity")]
			public ThreeWayOpt SplitHlsDiscontinuities
			{
				get;
			} = new(@"--hls-split-discontinuity", @"--no-hls-split-discontinuity");

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
			};


			/// <summary>
			/// Lists all options in <see cref="ExtractorGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to generate its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<IOneOpt> AllOpt
				=> [
					Retries,
					AllowDynamicMPD,
					SplitHlsDiscontinuities,
					Args,
				];
		}


		/// <summary>
		/// Provides access to a <see cref="GeneralGroup"/> instance
		/// </summary>
		public readonly GeneralGroup General
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="NetGroup"/> instance
		/// </summary>
		public readonly NetGroup Net
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="GeoRestrictionGroup"/> instance.
		/// </summary>
		public readonly GeoRestrictionGroup GeoRestriction
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="VidSelGroup"/> instance
		/// </summary>
		public readonly VidSelGroup VidSel
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="DownLoadsGroup"/> instance
		/// </summary>
		public readonly DownLoadsGroup DownLoad
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="FileSysGroup"/> instance
		/// </summary>
		public readonly FileSysGroup FileSys
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="ThumbGroup"/> instance
		/// </summary>
		public readonly ThumbGroup Thumb
		{
			get;
		} = new();

		/// <summary>
		/// Controls if yt-dlp creates Internet links and which format it uses when those link files are written.  If this is <see cref="InternetLinkFileTypeChoices.@default"/>, yt-dlp won’t write any link files unless a config file specifies --write-link, --write-url-link, --write-webloc-link, or --write-desktop-link.  If you specify <see cref="InternetLinkFileTypeChoices.native"/>, yt-dlp does write a link file, but selects a file format based on the OS it’s running on.  If you specify <see cref="InternetLinkFileTypeChoices.windows"/>, you’ll get a URL file.  If you use <see cref="InternetLinkFileTypeChoices.webloc"/>, you’ll get a .webloc file suitable for use on Macs.  If you specify <see cref="InternetLinkFileTypeChoices.desktop"/> you'll get a .desktop file for use on Linux.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strWriteLinks), @"Controls if yt-dlp creates Internet links and which format it uses when those link files are written.  If this is InternetLinkFileTypeChoices.@default, yt-dlp won't write any link files unless a config file specifies --write-link, --write-url-link, --write-webloc-link, or --write-desktop-link.  If you specify InternetLinkFileTypeChoices.native, yt-dlp does write a link file, but selects a file format based on the OS it's running on.  If you specify InternetLinkFileTypeChoices.windows, you'll get a URL file.  If you use InternetLinkFileTypeChoices.webloc, you'll get a .webloc file suitable for use on Macs.  If you specify InternetLinkFileTypeChoices.desktop you'll get a .desktop file for use on Linux.", typeof(AllGlobalOpts), @"--write-link", @"--write-url-link", @"--write-webloc-link",
			@"--write-desktop-link")]
		public OneOpt<InternetLinkFileTypeChoices> WriteLinks
		{
			get;
		} = new(InternetLinkFileTypeChoices.@default, string.Empty)
		{
			ParamListGenerator =
				opt
					=> opt.CurVal switch
						{
							InternetLinkFileTypeChoices.@default
								=> [],

							InternetLinkFileTypeChoices.native
								=> [@"--write-link"],

							InternetLinkFileTypeChoices.windows
								=> [@"--write-url-link"],

							InternetLinkFileTypeChoices.webloc
								=> [@"--write-webloc-link"],

							InternetLinkFileTypeChoices.desktop
								=> [@"--write-desktop-link"],


								_
									=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<InternetLinkFileTypeChoices>(opt.CurVal, @"While converting a " +
										@"InternetLinkFileTypeChoices value into a yt-dlp parameter"),
						},
		};

		/// <summary>
		/// Provides access to a <see cref="VerbosityAndSimulationGroup"/> instance
		/// </summary>
		public readonly VerbosityAndSimulationGroup VerbosityAndSimulation
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="WorkAroundsGroup"/> instance
		/// </summary>
		public readonly WorkAroundsGroup WorkArounds
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="VidFmtGroup"/> instance
		/// </summary>
		public readonly VidFmtGroup VidFmt
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="SubsGroup"/> instance
		/// </summary>
		public readonly SubsGroup Subs
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="AuthenticationGroup"/> instance
		/// </summary>
		public readonly AuthenticationGroup Authentication
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="PostProcessingGroup"/> instance
		/// </summary>
		public readonly PostProcessingGroup PostProcessing
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="SponsorBlockGroup"/> instance
		/// </summary>
		public readonly SponsorBlockGroup SponsorBlock
		{
			get;
		} = new();

		/// <summary>
		/// Provides access to a <see cref="ExtractorGroup"/> instance
		/// </summary>
		public readonly ExtractorGroup Extractor
		{
			get;
		} = new();


		/// <summary>
		/// Caches the list of options generated by <see cref="AllOpt"/> so we don’t need to keep rebuilding it.
		/// </summary>
		private System.Collections.Generic.IEnumerable<IOneOpt>? allOpts = null;

		/// <summary>
		/// Generates a list of parameters to emit to yt-dlp.  To do that, it goes to each child and obtains a list of options from properties like <see cref="GeneralGroup.AllOpt"/>.  The list of options is then cached in <see cref="allOpts"/>.  <see cref="AllOpt"/> then goes to each option and obtains the parameter list to emit to yt-dlp as an enumerable.  That list is then combined into a single enumerable.
		/// </summary>
		public System.Collections.Generic.IEnumerable<string> AllOpt
		{
			get
			{
				allOpts ??= General.AllOpt.Concat(Net.AllOpt).Concat(GeoRestriction.AllOpt).Concat(VidSel.AllOpt).Concat(DownLoad.AllOpt).Concat(FileSys.AllOpt)
					.Concat(Thumb.AllOpt).Append(WriteLinks).Concat(VerbosityAndSimulation.AllOpt).Concat(WorkArounds.AllOpt).Concat(VidFmt.AllOpt).Concat(Subs.AllOpt)
					.Concat(Authentication.AllOpt).Concat(PostProcessing.AllOpt).Concat(SponsorBlock.AllOpt).Concat(Extractor.AllOpt);
				
				return allOpts.Select
					(
						curOpt
							=> curOpt.Params
					).SelectMany
					(
						curParamSet
							=> curParamSet
					);
			}
		}
	}


	/// <summary>
	/// Provides access to the <see cref="AllGlobalOpts"/> instance.  If the derived type doesn't expose a yt-dlp directly, it might be in here.
	/// </summary>
	public AllGlobalOpts Opts
	{
		get;
	} = new();

	/// <summary>
	/// If some of the URLs specify both a video and a playlist, use <see cref="DownloadWhatInSpecifiedURLs"/> to control what you get.
	/// </summary>
	[System.Management.Automation.PSDefaultValue(Value = PartsOfUrlsThatCanBeDownloaded.both)]
	[System.Management.Automation.Parameter(HelpMessage = @"If some of the URLs specify both a video and a playlist, use -DownloadWhatInSpecifiedURLs to control what you get.")]
	public PartsOfUrlsThatCanBeDownloaded DownloadWhatInSpecifiedURLs
	{
		get;

		set;
	} = PartsOfUrlsThatCanBeDownloaded.both;

	/// <summary>
	/// Use to specify how yt-dlp generates file names.  See the instructions on how to do that from https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template.  Note: Anything returned by <see cref="GetInfoFromYtDlp"/> will contain a file field that shows what the file name with this template would be.  However, if you later save it with a different template, the name could be very different.
	/// </summary>
	[System.Management.Automation.PSDefaultValue(Value = null)]
	[System.Management.Automation.Parameter(HelpMessage = @"Use to specify how yt-dlp generates file names.  See the instructions on how to do that from https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template.  Note: Anything returned by Get-InfoFromYtDlp will contain a file field that shows what the file name with this template would be.  However, if you later save it with a different template, the name could be very different.")]
	public string? FileNameFmt
	{
		get;

		set;
	} = null;

	/// <summary>
	/// If specified, results in yt-dlp ignoring videos in a playlist with indices less than the specified amount.
	/// </summary>
	[System.Management.Automation.Parameter(ParameterSetName = @"Range", HelpMessage = @"If specified, results in yt-dlp ignoring videos in a playlist with indices less than the specified amount.")]
	public ulong? FirstItem
	{
		get;

		set;
	} = null;

	/// <summary>
	/// If specified, results in yt-dlp ignoring videos in a playlist with indices greater than the specified amount.
	/// </summary>
	[System.Management.Automation.Parameter(ParameterSetName = @"Range", HelpMessage = @"If specified, results in yt-dlp ignoring videos in a playlist with indices greater than the specified amount.")]
	public ulong? LastItem
	{
		get;

		set;
	} = null;

	/// <summary>
	/// If specified, results in yt-dlp downloading only the items whose index is explicitly listed by the value you pass.
	/// </summary>
	[System.Management.Automation.Parameter(ParameterSetName = @"SpecificItems", Mandatory = true, HelpMessage = @"If specified, results in yt-dlp downloading only the items whose index is explicitly listed by the value you pass.")]
	public System.Collections.Generic.IEnumerable<ulong>? AllItems
	{
		get;

		set;
	} = null;

	/// <summary>
	/// When implemented by a derived class, specifies the list of URLs to download.  <see cref="BaseVidCmdLet"/> takes care of translating the objects into the format it needs.  Warnings are emitted as needed.  Invalid values are discarded.  Most derived classes will provide an identical property, but change the name based on their needs.
	/// </summary>
	protected abstract System.Collections.Generic.IEnumerable<object> AllURLs
	{
		get;
	}

	/// <summary>
	/// Specifies if the derived class will call yt-dlp or if <see cref="BaseVidCmdLet"/> should do it in <see cref="EndProcessing"/>.  The default version returns <see langword="true"/>.  Override <see cref="YtDlpCalledByDerivedClass"/> and return <see langword="true"/> if you need to yt-dlp invoked differently such as getting JSON.  If <see cref="YtDlpCalledByDerivedClass"/> is set to <see langword="true"/>, <see cref="AdditionalYtDLpParams"/> won’t be called and overriding it would have no effect.
	/// </summary>
	protected virtual bool YtDlpCalledByDerivedClass
		=> false;

	/// <summary>
	/// Override when you want to be notified of new data in the standard output from yt-dlp.  The main known use case is to immediately send objects further up the pipeline.  The base version returns <see langword="null"/>.
	/// </summary>
	protected virtual System.Diagnostics.DataReceivedEventHandler? StdOutDataReceivedHandler
		=> null;

	/// <summary>
	/// Override when you want to be notified of new data in the standard error from yt-dlp.  As of when this documentation was written, a use case hasn't been found, but support is available anyway.  The base version returns <see langword="null"/>.
	/// </summary>
	protected virtual System.Diagnostics.DataReceivedEventHandler? StdErrDataReceivedHandler
		=> null;


	/// <summary>
	/// This version of <see cref="BeginProcessing"/> verifies the user or caller shouldn’t be using a <see cref="AllGlobalOpts.FileSysGroup.TemplatesForOutputFilesGroup"/> member instead of <see cref="FileNameFmt"/>.
	/// </summary>
	/// <exception cref="System.ArgumentException">A prefix was found in <see cref="FileNameFmt"/></exception>
	/// <inheritdoc/>
	protected override void BeginProcessing()
	{
		if(FileNameFmt is string strFileNameFmt && strFileNameFmt.Length > 0 && strFileNameFmt.IndexOf(':') > 1)
			throw new System.ArgumentException(@$"Prefix detected in -FileNameFmt.  Use Opts.FileSys.TemplatesForOutputFiles.* instead.", @"-FileNameFmt");

		base.BeginProcessing();
	}

	/// <summary>
	/// This override of <see cref="EndProcessing"/> provides a automatic means to invoke yt-dlp for most derived classes.  Override it if your derived class needs a specific means of invoking yt-dlp such as the <see cref="o:Goodies.YtDlpWrapper.YtDlpWrapper.InvokeYtDlpForJSON()"/> overloads.  This override handles all the parameters declared by <see cref="BaseVidCmdLet"/> for you.
	/// </summary>
	/// <inheritdoc/>
	protected override void EndProcessing()
	{
		if(YtDlpCalledByDerivedClass)
			return;

		System.Collections.Generic.List<string> liststrAllURLs =
			[
				..AdditionalYtDLpParams,
				..from object objCurURL in AllURLs
					select objCurURL is string strCurURL
						? strCurURL
						: objCurURL is System.Uri uriCur
							 ? uriCur.AbsolutePath
							 : throw new System.InvalidOperationException(@$"Unexpected type in list of URLs to send to yt-dlp: {objCurURL.GetType()}"),
			];

		if(FirstItem is ulong ulFirstItem)
			liststrAllURLs.AddRange([@"--playlist-start", ulFirstItem.ToString()]);
		if(LastItem is ulong ulLastItem)
			liststrAllURLs.AddRange([@"--playlist-end", ulLastItem.ToString()]);
		if(AllItems is System.Collections.Generic.IEnumerable<ulong> eulAllItems)
			liststrAllURLs.AddRange([@"--playlist-items", eulAllItems.Join(',')]);

		if(FileNameFmt is string strFileNameFmt && strFileNameFmt.Length > 0)
			liststrAllURLs.AddRange([@"--output", strFileNameFmt]);

		if(IsVerboseOn)
			liststrAllURLs.Add(@"--verbose");
		if(IsWhatIfOn)
			Opts.VerbosityAndSimulation.SimMode.Val = true;

		if(liststrAllURLs.Count > 0)
			Goodies.YtDlpWrapper.YtDlpWrapper.InvokeYtDlp(TaskCompletionSound, StdOutDataReceivedHandler, StdErrDataReceivedHandler, [..liststrAllURLs, ..Opts
				.AllOpt]);
	}

	/// <summary>
	/// Specifies more parameters that you want to pass to yt-dlp that aren’t listed in <see cref="Opts"/> and you don't need to override <see cref="EndProcessing"/> for other reasons.
	/// </summary>
	/// <returns></returns>
	protected virtual System.Collections.Generic.IEnumerable<string> AdditionalYtDLpParams
		=> [];
}