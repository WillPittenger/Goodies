// Ignore Spelling: Dlp Vals uri Sel wget Xattr Hls Sys Arounds Vid Fmts Pwd Executables Evt Exe Evts Loc Locs Json Dest Hdrs Langs

namespace WillPittenger.Goodies.YtDlpWrapper;

using Python.Runtime;

using System.Linq;

using Tools.Ext;
using Ext;

using IReadOnlyDictionaryToOptLists = System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<object>?>;
using DictionaryToOptLists = System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<object>?>;
using IReadOnlyDictionaryToOpt = System.Collections.Generic.IReadOnlyDictionary<string, object?>;
using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Constructs a new instance.
	/// </summary>
	public AllGlobalOpts()
	{
	}

	public AllGlobalOpts(in AllGlobalOpts copyThis)
	{
		General = new(copyThis.General);
		Net = new(copyThis.Net);
		GeoRestriction = new(copyThis.GeoRestriction);
		VidSel = new(copyThis.VidSel);
		DownLoad = new(copyThis.DownLoad);
		FileSys = new(copyThis.FileSys);
		Thumb = new(copyThis.Thumb);
		WriteLinks = new(copyThis.WriteLinks);
		VerbosityAndSimulation = new(copyThis.VerbosityAndSimulation);
		WorkArounds = new(copyThis.WorkArounds);
		VidFmt = new(copyThis.VidFmt);
		Subs = new(copyThis.Subs);
		Authentication = new(copyThis.Authentication);
		PostProcessing = new(copyThis.PostProcessing);
		SponsorBlock = new(copyThis.SponsorBlock);
		Extractor = new(copyThis.Extractor);
	}


	public interface IPythonTypeMaker
	{
		PyObject ConvertToPythonDict();

		protected static PyObject ConvertListToPythonDict(in System.Collections.IEnumerable enumWhatToConvert)
		{
			if(enumWhatToConvert is System.Collections.IDictionary dict)
			{
				Python.Runtime.PyDict pdResult = new();

				foreach(System.Collections.DictionaryEntry kvpCur in  dict)
					if(kvpCur.Key is string strKey && kvpCur.Value is object objVal)
						pdResult[strKey] = objVal is System.Collections.IEnumerable enumChild
							? ConvertListToPythonDict(enumChild)
							: objVal.ToPython();

				return pdResult;
			}

			if(enumWhatToConvert is System.Collections.ICollection coll)
			{
				Python.Runtime.PyList plResult = new();

				foreach(object? objCurEntry in coll)
					plResult.Append(objCurEntry is System.Collections.IEnumerable enumChild
						? ConvertListToPythonDict(enumChild)
						: objCurEntry.ToPython()
					);

				return plResult;
			}

			throw new System.InvalidOperationException(@"WHat is this type????");
		}
	}

	public abstract class BaseOneOpt
	{
		/// <summary>
		/// Returns a list of strings to be passed to yt-dlp.
		/// </summary>
		public abstract System.Collections.Generic.IEnumerable<string>? Params
		{
			get;
		}

		internal abstract DictionaryToOpt? PythonParams
		{
			get;
		}

		internal abstract IReadOnlyDictionaryToOptLists? PythonDict
		{
			get;
		}
	}

	/// <summary>
	/// Represents one option.  Allows for arrays of options regardless of type.  All options implement <see cref="BaseOneOpt"/>.  Don’t confuse it with <see cref="IOneOptVal"/>.  The arrays of options allow for the property <see cref="AllOpt"/> to exist.  It transforms a list of options into a list we can pass to yt-dlp.
	/// </summary>
	public abstract class BaseOneOpt<DerivedType>
		: BaseOneOpt
		where DerivedType : BaseOneOpt<DerivedType>
	{
		internal delegate DictionaryToOpt? DGetPythonParams(in DerivedType opt);

		internal DGetPythonParams? PythonParamsGenerator
		{
			get;

			set;
		}

		internal override IReadOnlyDictionaryToOptLists? PythonDict
		{
			get
			{
				DictionaryToOptLists result = [];

				if(PythonParamsGenerator is not null && PythonParamsGenerator(this as DerivedType ?? throw new System.InvalidProgramException(@"Unable to get option")) is IReadOnlyDictionaryToOpt parameters)
					foreach(System.Collections.Generic.KeyValuePair<string, object> kvpCur in parameters)
						result[kvpCur.Key] =
							[
								kvpCur.Value,
							];

				return result;
			}
		}
	}

	/// <summary>
	/// Represents the value of one option.  Don’t confuse <see cref="IOneOptVal"/> with <see cref="BaseOneOpt"/>.  <see cref="IOneOptVal"/> allows instances of <see cref="BaseOneOpt"/> to hold any type and get the value returned as a <c><see langword="string"/>?</c>.
	/// </summary>
	/// <remarks>
	/// <para>Use a specialized option type implementing <see cref="BaseOneOpt"/> if multiple strings need to be returned.</para>
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
	/// <seealso cref="BaseOneOpt" />
	public class OneOpt<Type> : BaseOneOpt<OneOpt<Type>>
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
		public readonly Type typeDef;

		/// <summary>
		/// Stores the current value.  Non-members should use <see cref="CurVal"/> to change <see cref="typeCurVal"/>.
		/// </summary>
		/// <seealso cref="CurVal"/>
		private Type typeCurVal;

		/// <summary>
		/// Stores the parameter name such as <c>"--option"</c>
		/// </summary>
		public readonly string strParamName;

		public readonly string strPythonParamName;

		public OneOpt(in OneOpt<Type> copyThis)
		{
			typeDef = copyThis.typeDef;
			typeCurVal = copyThis.CurVal;
			strParamName = copyThis.strParamName;
			strPythonParamName = copyThis.strPythonParamName;
		}

		/// <param name="typeDef">Specifies the default value for this option.  It’s used by <see cref="IsDefaulted"/>.</param>
		/// <param name="strParamName">Specifies the parameter name.  The text is used verbatim.  Use a raw string.</param>
		public OneOpt(in Type typeDef, in string strParamName, string? strPythonParmName = null)
		{
			this.typeDef = typeDef;
			typeCurVal = typeDef;
			this.strParamName = strParamName;
			strPythonParamName = strPythonParmName ?? strParamName.Replace(@"-", string.Empty);
		}


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
																=> [ParamToUse, $"{curKey.Key}={curKey.Value}"],
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
																=> [ParamToUse, $"{curKey.Key}={curKey.Value}"],
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
		public System.Func<Type, string?>? CustomParamNameLookUp
		{
			get;

			internal set;
		}

		public System.Func<Type, string?>? CustomPythonParamNameLookUp
		{
			get;

			set;
		}

		/// <summary>
		/// Computes the parameter to be used.  This is a shortcut to prevent duplication of the needed code.
		/// </summary>
		public string ParamToUse
			=> strParamName == string.Empty
				? CustomParamNameLookUp == null
					? throw new System.InvalidOperationException("No parameter name available.  Set in the constructor or with CustomParamNameLookUp.")
					: CustomParamNameLookUp(typeCurVal) ?? string.Empty
				: strParamName;

		/// <summary>
		/// Computes the parameter to be used.  This is a shortcut to prevent duplication of the needed code.
		/// </summary>
		public string? PythonParamToUse
			=> strPythonParamName == string.Empty
				? CustomPythonParamNameLookUp == null
					? throw new System.InvalidOperationException("No parameter name available.  Set in the constructor or with CustomPythonParamNameLookUp.")
					: CustomPythonParamNameLookUp(typeCurVal)
				: strPythonParamName;

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
		public override System.Collections.Generic.IEnumerable<string> Params
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
												=> [ParamToUse, $"{curKey.Key}={curKey.Value}"],
											_
												=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<KeyValCombinationRules>(HowToMakeKeyValPairStrings, @"While combining a key and value into a single string"),
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
											: new string[] { ParamToUse, curVal.ToString() ?? "" }
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
						: [ParamToUse, .. textValList];
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

		/// <summary>Returns a list of the final parameters to pass to yt-dlp for this option.  In some cases, this can be an empty array as noted in the remarks.</summary>
		/// <remarks>
		/// <para><see cref="Params" /> returns an empty array in the following situations:</para>
		/// <list type="bullet">
		///		<item>If <typeparamref name="Type"/> is <see langword="bool"/> and <see cref="CurVal" /> is <see langword="false"/></item>
		///		<item>Any empty list</item>
		///		<item>If <typeparamref name="Type"/> is <see langword="string"/> and <see cref="CurVal" /> matches <see cref="string.Empty" /></item>
		///	</list>
		///	<para>Note: <see cref="Params"/> also ignores any items in a list that are <see langword="null"/> or an empty list.  If <typeparamref name="Type"/> is a dictionary/map, those rules also applies to the values of any lists in the dictionary's entries.  If the result is the list or dictionary has no remaining entries, the return value will be an empty array.</para>
		/// </remarks>
		/// <seealso cref="HowToCombineVals"/>
		/// <seealso cref="HowToMakeKeyValPairStrings"/>
		/// <seealso cref="ValTextLookUp"/>
		/// <seealso cref="CustomParamNameLookUp"/>
		/// <seealso cref="ParamListGenerator"/>
		public virtual IReadOnlyDictionaryToOptLists? GetPythonParamList()
		{
			if(strPythonParamName is null)
				throw new System.InvalidOperationException(@"No parameter name available.  Set in the constructor or with CustomPythonParamNameLookUp.");

			if(typeCurVal is bool bVal)
				return bVal
					? new DictionaryToOptLists()
						{
							[strPythonParamName] =
								[
								true
								]
						}
					: [];

			if(PythonParamToUse is not string strPythonParamToUse)
				throw new System.InvalidOperationException(@"No parameter name available.  Set in the constructor or with CustomPythonParamNameLookUp.");

			if(typeCurVal == null)
				return new DictionaryToOptLists(){ };

			if(typeCurVal is System.Collections.Generic.IReadOnlyDictionary<string, Type> map)
				return HowToCombineVals switch
				{
					ValCombinationRules.paramReuse
						=> new DictionaryToOptLists()
							{
								[strPythonParamToUse] =
									map.Select(curKey =>
										(string[])[
											$"{curKey.Key}:{curKey.Value}",
										]
									).SelectMany(list
										=> list)
							},

					ValCombinationRules.colon or ValCombinationRules.semiColon
						=> [],

					_
						=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<ValCombinationRules>(HowToCombineVals, @"While selecting how to combine values"),
				};

			if(typeCurVal is System.Collections.Generic.IEnumerable<Type> enumerable)
				return HowToCombineVals switch
				{
					ValCombinationRules.paramReuse
						=> new DictionaryToOptLists()
							{
								[strPythonParamName] = enumerable.Select(
								curVal
									=> curVal == null
										? []
										: new string[] { ParamToUse, curVal.ToString() ?? "" }
								).SelectMany(
									curParamList
									=> curParamList
								),
							},

					ValCombinationRules.semiColon or ValCombinationRules.colon or ValCombinationRules.slash
						=> [],

					_
						=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<ValCombinationRules>(HowToCombineVals, @"While formatting a string for a multiple value parameter"),
				};

			if(MultipleValTextLookUp != null)
			{
				System.Collections.Generic.IEnumerable<string>? textValList = MultipleValTextLookUp(typeCurVal);

				return textValList == null
					? new DictionaryToOptLists(){ }
					: new()
						{
							[strPythonParamToUse] = textValList,
						};
			}

			string strValText = "";
			if(IsDefaulted || typeCurVal is IOneOptVal)
				strValText = ((IOneOptVal)typeCurVal).ValText ?? string.Empty;

			if(ValTextLookUp != null)
				strValText = ValTextLookUp(typeCurVal) ?? string.Empty;

			if(strValText == string.Empty && ValTextLookUp != null)
				strValText = ValTextLookUp(typeCurVal) ?? string.Empty;

			return strValText == string.Empty
				? new DictionaryToOptLists(){ }
				: new()
					{
						[PythonParamToUse] =
							[
								strValText
							],
					};
		}

		internal override IReadOnlyDictionaryToOptLists? PythonDict
			=> GetPythonParamList();

		/// <summary>
		/// Lets you specify a method that generates the strings to use.  It should return either an empty list or <see langword="null"/> if no parameters are needed.  This is commonly needed if multiple values are needed.
		/// </summary>
		public System.Func<OneOpt<Type>, System.Collections.Generic.IEnumerable<string>>? ParamListGenerator
		{
			get;

			internal set;
		} = null;

		internal override DictionaryToOpt? PythonParams
			=> PythonParamsGenerator?.Invoke(this);
	}

	/// <summary>
	/// This is the same as <see cref="OneOpt{Type}"/>, but adds a prefix field.
	/// </summary>
	/// <typeparam name="Type">The type of the value.  Don’t use <c><see langword="bool"/>?</c>.  Instead, use <see cref="ThreeWayOpt"/>.</typeparam>
	/// <remarks>
	///		<para>Some yt-dlp options such as --paths can be repeated with different prefixes.  So on a normal command line you might have "--paths home:%(title)s.%(ext)s --paths temp:c:\users\username\AppData\Roaming\yt-dlp\Temp".  In that string, <c>"temp"</c> and <c>"home"</c> are prefixes.</para>
	///		<para>In <see cref="AllGlobalOpts"/>, those will be exposed as separate instances of <see cref="OneOptWithPrefix{Type}"/>.</para>
	/// </remarks>
	public class OneOptWithPrefix<Type> : BaseOneOpt<OneOptWithPrefix<Type>>
	{
		/// <summary>
		/// The prefix needed for this instance of <see cref="OneOptWithPrefix{Type}"/>.  Options might be duplicated for each option so the prefixes might be
		/// different.
		/// </summary>
		public readonly string strValPrefix;

		/// <summary>
		/// Static constructor that ensures <c><see langword="bool"/>?</c> is never used.
		/// </summary>
		/// <exception cref="System.InvalidProgramException">Thrown if <typeparamref name="Type"/> is <c><see langword="bool"/>?</c></exception>
		static OneOptWithPrefix()
		{
			if(typeof(Type) == typeof(bool?))
				throw new System.InvalidProgramException($"Instead of building a OneOpt around a nullable bool, use {nameof(ThreeWayOpt)}.");
		}

		/// <summary>
		/// Stores the default value so that <see cref="IsDefaulted" /> works.
		/// </summary>
		public readonly Type typeDef;

		/// <summary>
		/// Stores the current value.  Non-members should use <see cref="CurVal"/> to change <see cref="typeCurVal"/>.
		/// </summary>
		/// <seealso cref="CurVal"/>
		private Type typeCurVal;

		/// <summary>
		/// Stores the parameter name such as <c>"--option"</c>
		/// </summary>
		public readonly string strParamName;

		public readonly string strPythonParamName;

		public OneOptWithPrefix(in OneOptWithPrefix<Type> copyThis)
		{
			PythonParamsGenerator = GetPythonParams;

			strValPrefix = copyThis.strValPrefix;
			typeDef = copyThis.typeDef;
			typeCurVal = copyThis.typeCurVal;
			strParamName = copyThis.strParamName;
			strPythonParamName = copyThis.strPythonParamName;
		}

		/// <param name="typeDef">Specifies the default value for this option.  It’s used by <see cref="OneOpt{Type}.IsDefaulted"/>.</param>
		/// <param name="strParamName">Specifies the parameter name.  The text is used verbatim.  Use a raw string.</param>
		/// <param name="strValPrefix">Specifies the prefix used.  Use a raw string.</param>
		public OneOptWithPrefix(in Type typeDef, in string strParamName, in string strValPrefix, in string? strPythonParmName = null)
		{
			PythonParamsGenerator = GetPythonParams;

			this.strValPrefix = strValPrefix;
			this.typeDef = typeDef;
			typeCurVal = typeDef;
			this.strParamName = strParamName;
			strPythonParamName = strPythonParmName ?? strParamName.Replace(@"-", string.Empty);
		}

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

		/// <inheritdoc/>
		public override System.Collections.Generic.IEnumerable<string> Params
			=> IsDefaulted
				? []
				: [strParamName, $"{strValPrefix}:{CurVal}"];

		internal override DictionaryToOpt? PythonParams
			=> PythonParamsGenerator?.Invoke(this);

		private DictionaryToOpt GetPythonParams(in OneOptWithPrefix<Type> opt)
			=> opt.CurVal is Type val
				? new DictionaryToOpt()
					{
						[opt.strPythonParamName] = new DictionaryToOpt()
							{
								[opt.strValPrefix] = val is IOneOptVal val2
									? val2.ValText
									: val,
							},
					}
				: [];
	}

	/// <summary>
	/// Specialized <see cref="BaseOneOpt"/> implementation for use when an option has three states: On, Off, and Defaulted.  It’s implemented as a <c><see langword="bool"/>?</c> such that <see langword="true"/> represents On, <see langword="false"/> represents Off, and <see langword="null"/> represents the state in which we don't pass parameters.  The default value is always <see langword="null"/>.
	/// </summary>
	public class ThreeWayOpt : BaseOneOpt<ThreeWayOpt>
	{
		/// <summary>
		/// Stores the value in this option.  Non-members should access it with <see cref="Val"/>.
		/// </summary>
		private bool? bVal = null;

		/// <summary>
		/// The name of the parameter used if the value is <see langword="true"/>.
		/// </summary>
		public readonly string strTrueParamName;

		/// <summary>
		/// The name of the parameter used if the value is <see langword="false"/>.
		/// </summary>
		public readonly string strFalseParamName;

		/// <summary>
		/// The name of the parameter used if the value is <see langword="true"/> or <see langword="false"/>.  An empty <see cref="PyDict"/> will be returned if the value is <see langword="null"/>.
		/// </summary>
		public readonly string strPythonAttrName;

		public readonly WhichValsToSendToPythonChoices whichValsToSendToPython;

		public enum WhichValsToSendToPythonChoices : byte
		{
			trueOnly,
			falseOnly,
			both,

			trueReversedOnly,
			falseReversedOnly,
			bothReversed,
		}

		/// <summary>Constructs a new <see cref="ThreeWayOpt"/> instance</summary>
		/// <param name="strTrueParamName">The parameter name to use when <see cref="Val"/> is <see langword="true"/>.  Use a raw string.  No sanity checks are performed and the value is used verbatim.</param>
		/// <param name="strFalseParamName">The parameter name to use when <see cref="Val"/> is <see langword="false"/>.  Use a raw string.  No sanity checks are performed and the value is used verbatim.</param>
		public ThreeWayOpt(in string strTrueParamName, in string strFalseParamName, in string? strPythonAttrName = null, in WhichValsToSendToPythonChoices whichValsToSendToPython = WhichValsToSendToPythonChoices.both)
		{
			this.strTrueParamName = strTrueParamName;
			this.strFalseParamName = strFalseParamName;
			this.strPythonAttrName = strPythonAttrName ?? strTrueParamName.Replace("-", string.Empty);
			this.whichValsToSendToPython = whichValsToSendToPython;
			PythonParamsGenerator = DefPythonParamsGenerator;
		}

		public ThreeWayOpt(in ThreeWayOpt copyThis)
		{
			strTrueParamName = copyThis.strTrueParamName;
			strFalseParamName = copyThis.strFalseParamName;
			strPythonAttrName = copyThis.strPythonAttrName;
			whichValsToSendToPython = copyThis.whichValsToSendToPython;
			PythonParamsGenerator = copyThis.PythonParamsGenerator;
		}


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
		public override System.Collections.Generic.IEnumerable<string> Params
			=> bVal switch
			{
				true
						=> [strTrueParamName],

				false
					 => [strFalseParamName],

				null
						=> [],
			};

		internal override DictionaryToOpt? PythonParams
			=> PythonParamsGenerator?.Invoke(this);


		protected DictionaryToOpt DefPythonParamsGenerator(in ThreeWayOpt opt)
			=> bVal switch
				{
					true when whichValsToSendToPython == WhichValsToSendToPythonChoices.trueOnly || whichValsToSendToPython == WhichValsToSendToPythonChoices.both
						=> new DictionaryToOpt()
							{
								[strPythonAttrName] = true,
							},

					false when whichValsToSendToPython == WhichValsToSendToPythonChoices.falseOnly || whichValsToSendToPython == WhichValsToSendToPythonChoices.both
						=> new DictionaryToOpt()
							{
								[strPythonAttrName] = false,
							},

					true when whichValsToSendToPython == WhichValsToSendToPythonChoices.falseReversedOnly || whichValsToSendToPython == WhichValsToSendToPythonChoices.bothReversed
						=> new DictionaryToOpt()
							{
								[strPythonAttrName] = false,
							},

					false when whichValsToSendToPython == WhichValsToSendToPythonChoices.trueReversedOnly || whichValsToSendToPython == WhichValsToSendToPythonChoices.bothReversed
						=> new DictionaryToOpt()
							{
								[strPythonAttrName] = true,
							},

					true or false when whichValsToSendToPython == WhichValsToSendToPythonChoices.bothReversed
						=> new DictionaryToOpt()
							{
								[strPythonAttrName] = !bVal,
							},

					_
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


		internal abstract double CalcWaitTime(in uint uiInput);
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


		internal override double CalcWaitTime(in uint uiStepNum)
			=> uiVal;
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


		internal override double CalcWaitTime(in uint uiStepNum)
			=> System.Math.Min(uiStart + (uiStep is uint uiRealStep ? uiRealStep : uiStart) * uiStepNum, uiEnd ?? uint.MaxValue);
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


		internal override double CalcWaitTime(in uint uiStepNum)
			=> System.Math.Min(uiStart + System.Math.Pow(uiBase ?? 2, uiStepNum), uiEnd ?? uint.MaxValue);
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
	/// Specialized <see cref="BaseOneOpt"/> implementation that describes a range of integers.  Implemented around <see langword="ushort"/>.
	/// </summary>
	/// <remarks>
	/// <para>If you set <see cref="Min"/> to a value greater than <see cref="Max"/>, it will swap those values.  Ditto if you set <see cref="Max"/> to a value greater than <see cref="Min"/>.  You can set <see cref="Min"/> and/or <see cref="Max"/> while leaving the other <see langword="null"/>.</para>
	/// </remarks>
	public class UShortRangeOpt : BaseOneOpt<UShortRangeOpt>
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
		public readonly string strMinParam;

		/// <summary>
		/// The parameter to use when <see cref="Max"/> is non-<see langword="null"/>.
		/// </summary>
		public readonly string strMaxParam;

		/// <summary>
		/// The parameter to use when <see cref="Min"/> is non-<see langword="null"/>.
		/// </summary>
		public readonly string strMinPythonParam;

		/// <summary>
		/// The parameter to use when <see cref="Max"/> is non-<see langword="null"/>.
		/// </summary>
		public readonly string strMaxPythonParam;


		public UShortRangeOpt(in UShortRangeOpt copyThis)
		{
			PythonParamsGenerator = GetPythonParams;

			strMinParam = copyThis.strMinParam;
			strMaxParam = copyThis.strMaxParam;
			strMinPythonParam = copyThis.strMinPythonParam;
			strMaxPythonParam = copyThis.strMaxPythonParam;
		}

		/// <param name="strMinParam">The name of the parameter corresponding to the minimum value.</param>
		/// <param name="strMaxParam">The name of the parameter corresponding to the maximum value.</param>
		public UShortRangeOpt(in string strMinParam, in string strMaxParam, in string? strMinPythonParam = null, in string? strMaxPythonParam = null)
		{
			this.strMinParam = strMinParam;
			this.strMaxParam = strMaxParam;
			this.strMinPythonParam = strMinPythonParam ?? strMinParam.Replace("-", string.Empty);
			this.strMaxPythonParam = strMaxPythonParam ?? strMinParam.Replace("-", string.Empty);
		}


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

		internal bool MinValidByItself
		{
			get;

			set;
		} = true;

		internal bool MaxValidByItself
		{
			get;

			set;
		} = true;


		/// <inheritdoc/>
		public override System.Collections.Generic.IEnumerable<string>? Params
			=> usMin == null && usMax == null
				? []
				: usMin != null && usMax == null && MinValidByItself
					? [strMinParam, ((ushort)usMin).ToString()]
					: usMin == null && usMax != null && MaxValidByItself
						? [strMaxParam, ((ushort)usMax).ToString()]
						: usMin != null && usMax != null
							? [strMinParam, ((ushort)usMin).ToString(), strMaxParam, ((ushort)usMax).ToString()]
							: null;

		internal override DictionaryToOpt? PythonParams
			=> PythonParamsGenerator?.Invoke(this);


		private DictionaryToOpt? GetPythonParams(in UShortRangeOpt opt)
			=> usMin == null && usMax == null
				?  []
				: usMin is ushort usRealMin && usMax is null && MinValidByItself
					? new()
						{
							[strMinPythonParam] = usRealMin,
						}
					: usMin == null && usMax is ushort usRealMax && MaxValidByItself
						? new()
							{
								[strMaxPythonParam] = usRealMax,
							}
						: usMin is ushort usRealMin2 && usMax is ushort usRealMax2
							? new()
								{
									[strMinPythonParam] = usRealMin2,
									[strMaxPythonParam] = usRealMax2,
								}
							: null;
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
	/// Groups various options related to SponsorBlock.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#sponsorblock-options">SponsorBlock Options:</a>.
	/// </summary>
	public sealed class SponsorBlockOpt
		: BaseOneOpt<SponsorBlockOpt>, System.Collections.Generic.IReadOnlyDictionary<SponsorBlockOpt.KnownCats, SponsorBlockOpt.Statuses>
	{
		public SponsorBlockOpt()
			=> PythonParamsGenerator = GetPythonParams;


		public SponsorBlockOpt(in SponsorBlockOpt copyThis)
		{
			PythonParamsGenerator = GetPythonParams;

			dictCatStatuses = copyThis.dictCatStatuses;
			ChapterTitleTemplate = copyThis.ChapterTitleTemplate;
			Disable = copyThis.Disable;
			ApiUri = copyThis.ApiUri;
		}


		[System.ComponentModel.ImmutableObject(true)]
		public sealed record KnownCats
		{
			private KnownCats(in string strName)
				=> this.strName = strName;


			public readonly string strName;


			public static readonly KnownCats @default = new(@"default");

			public static readonly KnownCats sponsor = new(@"sponsor");
			public static readonly KnownCats intro = new(@"intro");
			public static readonly KnownCats outro = new(@"outro");
			public static readonly KnownCats selfPromo = new(@"selfpromo");
			public static readonly KnownCats preview = new(@"preview");
			public static readonly KnownCats filler = new(@"filler");
			public static readonly KnownCats interaction = new(@"interaction");
			public static readonly KnownCats musicOffTopic = new(@"music_offtopic");
			public static readonly KnownCats hook = new(@"hook");
			public static readonly KnownCats poiHighLight = new(@"poi_highlight");
			public static readonly KnownCats chapter = new("chapter");
			public static readonly KnownCats all = new(@"all");


			public static implicit operator string(in KnownCats kcConvertThis)
				=> kcConvertThis.strName;
		}

		public enum Statuses : byte
		{
			ignore,
			mark,
			remove,
		}



		private readonly System.Collections.Generic.Dictionary<KnownCats, Statuses> dictCatStatuses = new()
		{
			[KnownCats.@default] = Statuses.ignore,

			[KnownCats.sponsor] = Statuses.ignore,
			[KnownCats.intro] = Statuses.ignore,
			[KnownCats.outro] = Statuses.ignore,
			[KnownCats.selfPromo] = Statuses.ignore,
			[KnownCats.preview] = Statuses.ignore,
			[KnownCats.filler] = Statuses.ignore,
			[KnownCats.interaction] = Statuses.ignore,
			[KnownCats.musicOffTopic] = Statuses.ignore,
			[KnownCats.hook] = Statuses.ignore,
			[KnownCats.poiHighLight] = Statuses.ignore,
			[KnownCats.chapter] = Statuses.ignore,
			[KnownCats.all] = Statuses.ignore,
		};

		private static readonly System.Collections.Generic.IReadOnlySet<KnownCats> setkcInvalidOnRemove =
			(System.Collections.Generic.HashSet<KnownCats>)[
				KnownCats.poiHighLight,
				KnownCats.chapter,
			];


		public string ChapterTitleTemplate
		{
			get;

			set;
		} = string.Empty;

		public bool Disable
		{
			get;

			set;
		}

		public System.Uri? ApiUri
		{
			get;

			set;
		}

		public bool AreMarkItemsPresent
			=> dictCatStatuses.Values.Any(curStatus
				=> curStatus == Statuses.mark
			);

		public bool AreRemoveItemsPresent
			=> dictCatStatuses.Values.Any(curStatus
				=> curStatus == Statuses.remove
			);

		public System.Collections.Generic.IEnumerable<string> AllMarkItems
			=> dictCatStatuses[KnownCats.@default] == Statuses.mark || dictCatStatuses[KnownCats.all] == Statuses.mark
				? [
					@"all",
					..from kcCurCat in dictCatStatuses.Keys
						where dictCatStatuses[kcCurCat] == Statuses.mark && kcCurCat != KnownCats.@default && kcCurCat != KnownCats.all
						select kcCurCat.strName,
				]
				: from kcCurCat in dictCatStatuses.Keys
						where dictCatStatuses[kcCurCat] == Statuses.mark && kcCurCat != KnownCats.@default && kcCurCat != KnownCats.all
						select kcCurCat.strName;

		public System.Collections.Generic.IEnumerable<string> AllRemoveItems
			=> dictCatStatuses[KnownCats.@default] == Statuses.remove || dictCatStatuses[KnownCats.all] == Statuses.remove
				? [
						@"all",
						..from kcCurCat in dictCatStatuses.Keys
							where dictCatStatuses[kcCurCat] == Statuses.remove && kcCurCat != KnownCats.@default && kcCurCat != KnownCats.all
							select kcCurCat.strName
					]
				: from kcCurCat in dictCatStatuses.Keys
							where dictCatStatuses[kcCurCat] == Statuses.remove && kcCurCat != KnownCats.@default && kcCurCat != KnownCats.all
							select kcCurCat.strName;

		public override System.Collections.Generic.IEnumerable<string>? Params
		{
			get
			{
				bool bAreMarkItemsPresent = AreMarkItemsPresent;
				bool bAreRemoveItemsPresent = AreRemoveItemsPresent;

				return Disable
					? [
							@"--no-sponsorblock",
						]
					: !bAreMarkItemsPresent && !bAreRemoveItemsPresent && ApiUri is null
						? []
						: ApiUri is not null && !bAreMarkItemsPresent && !bAreRemoveItemsPresent
						? [
								@"--sponsorblock-api",
								ApiUri.AbsoluteUri,
							]
						: bAreMarkItemsPresent && !bAreRemoveItemsPresent && ApiUri is null
							? [
									@"--sponsorblock-mark",
									..AllMarkItems,
								]
							: bAreMarkItemsPresent && !bAreRemoveItemsPresent && ApiUri is not null
								? [
										@"--sponsorblock-mark",
										..AllMarkItems,
										@"--spponsorblock-api",
										ApiUri.AbsoluteUri,
									]
								: !bAreMarkItemsPresent && bAreRemoveItemsPresent && ApiUri is null
									? [
											@"--sponsorblock-remove",
											..AllRemoveItems,
										]
									: !bAreMarkItemsPresent && bAreRemoveItemsPresent && ApiUri is not null
										? [
												@"--sponsorblock-remove",
												..AllRemoveItems,
												@"--spponsorblock-api",
												ApiUri.AbsoluteUri,
											]
										: bAreMarkItemsPresent && bAreRemoveItemsPresent && ApiUri is not null
											? [
													@"--sponsorblock-mark",
													..AllMarkItems,
													@"--sponsorblock-remove",
													..AllRemoveItems,
													@"--spponsorblock-api",
													ApiUri.AbsoluteUri,
												]
											: throw new System.InvalidProgramException(@"Uknown configuration for the SponsorBlock");
			}
		}

		internal override DictionaryToOpt? PythonParams
			=> PythonParamsGenerator?.Invoke(this);

		public DictionaryToOpt? GetPythonParams(in SponsorBlockOpt opt)
		{
			bool bAreMarkItemsPresent = AreMarkItemsPresent;
			bool bAreRemoveItemsPresent = AreRemoveItemsPresent;

			return Disable || ApiUri is null || !bAreMarkItemsPresent && !bAreRemoveItemsPresent
				? []
				: ApiUri is not null && !bAreMarkItemsPresent && !bAreRemoveItemsPresent
					? new DictionaryToOpt()
						{
							[@"api"] = ApiUri.AbsoluteUri,
						}
					: new DictionaryToOpt()
						{
							[@"postprocessors"] = new System.Collections.Generic.List<DictionaryToOpt>()
							{
								new()
								{
									[@"api"] = ApiUri?.AbsoluteUri ?? @"https://sponsor.ajay.app",
									[@"categories"] = AllMarkItems.Concat(AllRemoveItems).Distinct(),
									[@"key"] = @"SponsorBlock",
									[@"when"] = @"after_filter",
								},
								new()
								{
									[@"force_keyframes"] = false,
									[@"key"] = @"ModifyChapters",
									[@"remove_chapter_patters"] = System.Array.Empty<object>(),
									[@"remove_ranges"] = System.Array.Empty<object>(),
									[@"remove_sponsor_segments"] = new System.Collections.Generic.HashSet<string>(AllRemoveItems),
									[@"sponsorblock_chapter_title"] = @"[SponsorBlock] %(category_names)l",
								},
							}
						};
				}

		public System.Collections.Generic.IEnumerable<KnownCats> Keys
			=> dictCatStatuses.Keys;

		public System.Collections.Generic.IEnumerable<Statuses> Values
			=> dictCatStatuses.Values;

		public int Count
			=> dictCatStatuses.Count;

		public Statuses this[KnownCats key]
			=> dictCatStatuses[key];

		public Statuses this[in KnownCats kcWhatToLookUp]
		{
			get
				=> dictCatStatuses[kcWhatToLookUp];

			set
				=> dictCatStatuses[kcWhatToLookUp] = dictCatStatuses.ContainsKey(kcWhatToLookUp) && (value != Statuses.remove || !setkcInvalidOnRemove.Contains(kcWhatToLookUp))
					? value
					: throw new System.InvalidOperationException(@"Only existing keys can be changed");
		}


		public bool ContainsKey(KnownCats key)
			=> dictCatStatuses.ContainsKey(key);

		public bool TryGetValue(KnownCats key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out Statuses value)
			=> dictCatStatuses.TryGetValue(key, out value);

		public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<KnownCats, Statuses>> GetEnumerator()
			=> dictCatStatuses.GetEnumerator();

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			=> dictCatStatuses.GetEnumerator();
	}

	/// <summary>
	/// Provides access to a <see cref="GeneralGroup"/> instance
	/// </summary>
	public GeneralGroup General
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="NetGroup"/> instance
	/// </summary>
	public NetGroup Net
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="GeoRestrictionGroup"/> instance.
	/// </summary>
	public GeoRestrictionGroup GeoRestriction
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="VidSelGroup"/> instance
	/// </summary>
	public VidSelGroup VidSel
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="DownLoadsGroup"/> instance
	/// </summary>
	public  DownLoadsGroup DownLoad
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="FileSysGroup"/> instance
	/// </summary>
	public FileSysGroup FileSys
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="ThumbGroup"/> instance
	/// </summary>
	public ThumbGroup Thumb
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Controls if yt-dlp creates Internet links and which format it uses when those link files are written.  If this is <see cref="InternetLinkFileTypeChoices.@default"/>, yt-dlp won’t write any link files unless a config file specifies --write-link, --write-url-link, --write-webloc-link, or --write-desktop-link.  If you specify <see cref="InternetLinkFileTypeChoices.native"/>, yt-dlp does write a link file, but selects a file format based on the OS it’s running on.  If you specify <see cref="InternetLinkFileTypeChoices.windows"/>, you’ll get a URL file.  If you use <see cref="InternetLinkFileTypeChoices.webloc"/>, you’ll get a .webloc file suitable for use on Macs.  If you specify <see cref="InternetLinkFileTypeChoices.desktop"/> you'll get a .desktop file for use on Linux.
	/// </summary>
	[YtDlpOptDoc(nameof(Rsrcs.strWriteLinks), @"Controls if yt-dlp creates Internet links and which format it uses when those link files are written.  If this is InternetLinkFileTypeChoices.@default, yt-dlp won't write any link files unless a config file specifies --write-link, --write-url-link, --write-webloc-link, or --write-desktop-link.  If you specify InternetLinkFileTypeChoices.native, yt-dlp does write a link file, but selects a file format based on the OS it's running on.  If you specify InternetLinkFileTypeChoices.windows, you'll get a URL file.  If you use InternetLinkFileTypeChoices.webloc, you'll get a .webloc file suitable for use on Macs.  If you specify InternetLinkFileTypeChoices.desktop you'll get a .desktop file for use on Linux.", typeof(AllGlobalOpts), @"--write-link", @"--write-url-link", @"--write-webloc-link", @"--write-desktop-link")]
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
	/// Provides access to a <see cref="VerbosityAndSimGroup"/> instance
	/// </summary>
	public VerbosityAndSimGroup VerbosityAndSimulation
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="WorkAroundsGroup"/> instance
	/// </summary>
	public WorkAroundsGroup WorkArounds
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="VidFmtGroup"/> instance
	/// </summary>
	public VidFmtGroup VidFmt
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="SubsGroup"/> instance
	/// </summary>
	public SubsGroup Subs
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="AuthGroup"/> instance
	/// </summary>
	public AuthGroup Authentication
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="PostProcessingGroup"/> instance
	/// </summary>
	public PostProcessingGroup PostProcessing
	{
		get;

		set;
	} = new();

	/// <summary>
	/// Provides access to a <see cref="ExtractorGroup"/> instance
	/// </summary>
	public ExtractorGroup Extractor
	{
		get;

		set;
	} = new();

	public SponsorBlockOpt SponsorBlock
	{
		get;
	} = new();


	/// <summary>
	/// Caches the list of options generated by <see cref="AllOpt"/> so we don’t need to keep rebuilding it.
	/// </summary>
	private System.Collections.Generic.IEnumerable<BaseOneOpt>? allOpts = null;

	/// <summary>
	/// Generates a list of parameters to emit to yt-dlp.  To do that, it goes to each child and obtains a list of options from properties like <see cref="GeneralGroup.AllOpt"/>.  The list of options is then cached in <see cref="allOpts"/>.  <see cref="AllOpt"/> then goes to each option and obtains the parameter list to emit to yt-dlp as an enumerable.  That list is then combined into a single enumerable.
	/// </summary>
	public System.Collections.Generic.IEnumerable<string> AllOpt
	{
		get
		{
			allOpts ??= General.AllOpt.Concat(Net.AllOpt).Concat(GeoRestriction.AllOpt).Concat(VidSel.AllOpt).Concat(DownLoad.AllOpt).Concat(FileSys.AllOpt)
				.Concat(Thumb.AllOpt).Append(WriteLinks).Concat(VerbosityAndSimulation.AllOpt).Concat(WorkArounds.AllOpt).Concat(VidFmt.AllOpt).Concat(Subs.AllOpt)
				.Concat(Authentication.AllOpt).Concat(PostProcessing.AllOpt).Concat([SponsorBlock]).Concat(Extractor.AllOpt);

			return allOpts.Select
				(
					curOpt
						=> curOpt.Params
				).SelectMany
				(
					curParamSet
						=> curParamSet ?? throw new System.InvalidProgramException(@"We're missing an option group somehow")
				);
		}
	}


	public static System.Collections.ArrayList MergeLists(in System.Collections.IList listLeft, in System.Collections.IList listRight)
		=>
			[
				..listLeft,
				..listRight,
			];

	public static System.Collections.ArrayList MergeLists(in object? objLeft, in System.Collections.IList listRight)
		=>
			[
				objLeft,
				..listRight,
			];

	private static System.Collections.ArrayList MergeLists(in System.Collections.IList listLeft, in object? objRight)
		=>
			[
				listLeft,
				objRight,
			];

	private static DictionaryToOpt MergeDictionaries(in DictionaryToOpt mapLeft, in DictionaryToOpt? mapRight)
	{
		if(mapLeft.Count == 0 && (mapRight is null || mapRight.Count == 0))
			return [];

		if(mapRight is null || mapRight.Count == 0)
			return mapLeft;

		DictionaryToOpt mapRetVal = [];
		foreach(string strCurKey in mapRight.Keys)
			if(!mapLeft.ContainsKey(strCurKey))
				mapRetVal[strCurKey] = mapRight[strCurKey];
			else if(mapLeft[strCurKey] is bool bLeftVal && mapRight[strCurKey] is bool bRightVal)
				mapRetVal[strCurKey] = bLeftVal || bRightVal;
			else if(mapLeft[strCurKey] is DictionaryToOpt mapLeftVal && mapRight[strCurKey] is DictionaryToOpt mapRightVal)
				mapRetVal[strCurKey] = MergeDictionaries(mapLeftVal, mapRightVal);
			else if(mapLeft[strCurKey] is System.Collections.IList listLeftVal && mapRight[strCurKey] is System.Collections.IList listRightVal)
				mapRetVal[strCurKey] = MergeLists(listLeftVal, listRightVal);
			else if(mapLeft[strCurKey] is System.Collections.IList listLeftVal2 && mapRight[strCurKey] is not System.Collections.IList)
				mapRetVal[strCurKey] = MergeLists(listLeftVal2, mapRight[strCurKey]);
			else if(mapLeft[strCurKey] is not System.Collections.IList && mapRight[strCurKey] is System.Collections.IList listRight2)
				mapRetVal[strCurKey] = MergeLists(mapLeft[strCurKey], listRight2);
			else
				mapRetVal[strCurKey] = new System.Collections.ArrayList()
					{
						mapLeft[strCurKey],
						mapRight[strCurKey],
					};

		return mapRetVal;
	}

	private abstract class BaseOptWrapper();

	private class OneOptWrapper(in BaseOneOpt opt)
		: BaseOptWrapper
	{
		public readonly BaseOneOpt opt = opt;


		public static implicit operator OneOptWrapper(in BaseOneOpt optToWrap)
			=> new(optToWrap);
	}

	private class ManyOptWrapper(in BaseOneOpt[] optionsToWrap)
		: BaseOptWrapper
	{
		public readonly BaseOneOpt[] optionsToWrap = optionsToWrap;


		public static implicit operator ManyOptWrapper(in BaseOneOpt[] optionsToWrap)
			=> new(optionsToWrap);

		public static ManyOptWrapper FromEnumerable(in System.Collections.Generic.IEnumerable<BaseOneOpt> objects)
			=> new([.. objects]);
	}

	private static DictionaryToOpt MergePythonDictionaries(params BaseOptWrapper[] input)
	{
		DictionaryToOpt mapResult = [];

		foreach(BaseOptWrapper inputCur in input)
			if(inputCur is OneOptWrapper oow)
			{
				if(oow.opt.PythonParams is not null)
					mapResult = MergeDictionaries(mapResult, oow.opt.PythonParams);
			}
			else if(inputCur is ManyOptWrapper mow)
				foreach(BaseOneOpt optCur in mow.optionsToWrap)
					mapResult = MergeDictionaries(mapResult, optCur.PythonParams);
			else
				throw new System.InvalidOperationException(@"Unknown or invalid input to MergePythonDictionaries");

		return mapResult;
	}

	internal Python.Runtime.PyDict ToPythonDict()
	{
		if(allOpts is System.Collections.Generic.IEnumerable<BaseOneOpt> allRealOpts)
		{
			using Py.GILState lockInfo = Py.GIL();

			Python.Runtime.PyDict pdResult = new();

			DictionaryToOpt mapMerged = MergePythonDictionaries
				(
					ManyOptWrapper.FromEnumerable(General.AllOpt),
					ManyOptWrapper.FromEnumerable(Net.AllOpt),
					ManyOptWrapper.FromEnumerable(GeoRestriction.AllOpt),
					ManyOptWrapper.FromEnumerable(VidSel.AllOpt),
					ManyOptWrapper.FromEnumerable(DownLoad.AllOpt),
					ManyOptWrapper.FromEnumerable(FileSys.AllOpt),
					ManyOptWrapper.FromEnumerable(Thumb.AllOpt),
					(OneOptWrapper)WriteLinks,
					ManyOptWrapper.FromEnumerable(VerbosityAndSimulation.AllOpt),
					ManyOptWrapper.FromEnumerable(WorkArounds.AllOpt),
					ManyOptWrapper.FromEnumerable(VidFmt.AllOpt),
					ManyOptWrapper.FromEnumerable(Subs.AllOpt),
					ManyOptWrapper.FromEnumerable(Authentication.AllOpt),
					ManyOptWrapper.FromEnumerable(PostProcessing.AllOpt),
					(OneOptWrapper)SponsorBlock,
					ManyOptWrapper.FromEnumerable(Extractor.AllOpt)
				);

			foreach(System.Collections.Generic.KeyValuePair<string, object?> kvpCur in mapMerged)
				pdResult.SetItem
					(
						kvpCur.Key, kvpCur.Value is IPythonTypeMaker maker
							? maker.ConvertToPythonDict()
							: kvpCur.Value.ToPython()
					);

			return pdResult;
		}

		throw new System.InvalidOperationException("No known options!!!!!");
	}
}