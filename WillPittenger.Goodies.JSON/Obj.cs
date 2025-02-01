// Ignore Spelling: Org JSON jsone vals

using System.Linq;
using WillPittenger.Goodies.Tools.Ext;

namespace WillPittenger.Goodies.JSON;

/// <summary>
/// This corresponds to a JSON object.  It can contain <see cref="NamedVal"/> indexed by their name.  The values inside those <see cref="NamedVal"/> can be any
/// instance of a class derived from <see cref="ObjBase"/> such as <see cref="Array"/> or <see cref="Obj"/>.
/// </summary>
public class Obj : ObjBase, System.Collections.Generic.IEnumerable<NamedVal>, System.Collections.IEnumerable, System.Collections.Specialized
	.INotifyCollectionChanged
{
	/// <summary>
	/// Constructs a <see cref="Obj"/> instance using the specified parsed JSON.
	/// </summary>
	/// <param name="jsoneProperty">This is the JSON object to convert.  This is expected to be an instance of <see cref="System.Text.Json.JsonProperty"/>.
	/// </param>
	/// <exception cref="System.InvalidOperationException">The JSON element isn't an object.</exception>
	public Obj(System.Text.Json.JsonElement jsoneProperty)
	{
		System.Diagnostics.Debug.Assert(jsoneProperty.ValueKind == System.Text.Json.JsonValueKind.Object);
		if(jsoneProperty.ValueKind != System.Text.Json.JsonValueKind.Object)
			throw new System.InvalidOperationException("This JSON element isn't an object");

		foreach(System.Text.Json.JsonProperty jsonpCur in jsoneProperty.EnumerateObject())
			values[jsonpCur.Name] = new NamedVal(jsonpCur);
	}

	/// <summary>
	/// Constructs a <see cref="Obj"/> using the provided dictionary of values.  Each value should be a name/value pair.
	/// </summary>
	/// <param name="vals">The list of pairs</param>
	public Obj(System.Collections.Generic.IReadOnlyDictionary<string, object?> vals)
	{
		foreach(System.Collections.Generic.KeyValuePair<string, object?> kvpCur in vals)
			values[kvpCur.Key] = new NamedVal(kvpCur.Key, kvpCur.Value);
	}

	/// <summary>
	/// Constructs a <see cref="Obj"/> using the provided dictionary of values.  Each value should be a name/value pair.
	/// </summary>
	/// <param name="vals">The list of pairs</param>
	public Obj(System.Collections.Generic.IReadOnlyDictionary<string, Val> vals)
	{
		foreach(System.Collections.Generic.KeyValuePair<string, Val> kvpCur in vals)
			values[kvpCur.Key] = new NamedVal(kvpCur.Key, kvpCur.Value);
	}

	/// <summary>
	/// Constructs a <see cref="Obj"/> using the provided list of <see cref="NamedVal"/> instances.  Each one should already be a name/value pair.
	/// </summary>
	/// <param name="vals">The list to use</param>
	public Obj(System.Collections.Generic.IEnumerable<NamedVal> vals)
	{
		foreach(NamedVal nvCur in vals)
			values[nvCur.strName] = nvCur;
	}

	/// <summary>
	/// Internal data we update when they add elements with <see cref="Add(in NamedVal)"/>, <see cref="Add(in string, in object?)"/>, or <see
	/// cref="Add(in string, in Val)"/>.
	/// </summary>
	private readonly System.Collections.Generic.SortedDictionary<string, NamedVal> values = [];

	/// <summary>
	/// Fired when the collection changes
	/// </summary>
	public event System.Collections.Specialized.NotifyCollectionChangedEventHandler? CollectionChanged;

	/// <summary>
	/// Quick way to test if a field is present in this object
	/// </summary>
	/// <param name="strFieldName">The field name</param>
	/// <returns><see langword="true"/> if the field is present and <see langword="false"/> otherwise.</returns>
	public bool HasField(in string strFieldName)
		=> values.ContainsKey(strFieldName);

	/// <summary>
	/// Adds a field to the list using a field name and a preconverted value.
	/// </summary>
	/// <param name="strFieldName">The name of the new field.</param>
	/// <param name="val">The value to use.  If you need a null value, use <see cref="Add(in string, in object?)"/> instead.</param>
	/// <exception cref="System.InvalidOperationException">The field name isn't unique</exception>
	public void Add(in string strFieldName, in Val val)
	{
		if(values.ContainsKey(strFieldName))
			throw new System.InvalidOperationException($"The object already has a field named “${strFieldName}”.");

		values[strFieldName] = new(strFieldName, val);

		CollectionChanged?.Invoke(this, new(System.Collections.Specialized.NotifyCollectionChangedAction.Add, values[strFieldName]));
	}

	/// <summary>
	/// Adds a field to the list using a field name and a raw .NET object.
	/// </summary>
	/// <param name="strFieldName">The name of the new field</param>
	/// <param name="objNewVal">The value to use.  This can be <see langword="null"/>.</param>
	/// <exception cref="System.InvalidOperationException">The field name isn't unique.</exception>
	public void Add(in string strFieldName,  in object? objNewVal)
	{
		if(values.ContainsKey(strFieldName))
			throw new System.InvalidOperationException($"The object already has a field named “${strFieldName}”.");

		values[strFieldName] = new(strFieldName, objNewVal);

		CollectionChanged?.Invoke(this, new(System.Collections.Specialized.NotifyCollectionChangedAction.Add, values[strFieldName]));
	}

	/// <summary>
	/// Adds a new field based on an existing <see cref="NamedVal"/> instance.
	/// </summary>
	/// <param name="nvNew">The new field</param>
	/// <exception cref="System.InvalidOperationException">The field name isn't unique</exception>
	public void Add(in NamedVal nvNew)
	{
		if(values.ContainsKey(nvNew.strName))
			throw new System.InvalidOperationException($"The object already has a field named “${nvNew.strName}”.");

		values[nvNew.strName] = nvNew;

		CollectionChanged?.Invoke(this, new(System.Collections.Specialized.NotifyCollectionChangedAction.Add, nvNew));
	}

	/// <summary>
	/// Named values in the object indexed by their name.
	/// </summary>
	public System.Collections.Generic.IReadOnlyDictionary<string, NamedVal> Values
		=> values;

	/// <summary>
	/// Returns an enumerator of the values inside the object.  Note this isn't generic.
	/// </summary>
	/// <returns></returns>
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => values.Values.GetEnumerator();

	/// <summary>
	/// Returns a generic enumerator of type <see cref="NamedVal"/>.
	/// </summary>
	/// <returns></returns>
	public System.Collections.Generic.IEnumerator<NamedVal> GetEnumerator() => values.Values.GetEnumerator();

	/// <inheritdoc/>
	public override string ToString()
		=> $"{{{values.Values.Select(
			curVal
				=> curVal.ToString()
			).Join(',')}}}";
}