// Ignore Spelling: Org JSON

namespace WillPittenger.Goodies.JSON;

/// <summary>
/// This corresponds to a JSON object.  It can contain <see cref="NamedVal"/> indexed by their name.  The values inside those <see cref="NamedVal"/> can be any
/// instance of a class derived from <see cref="ObjBase"/> such as <see cref="Array"/> or <see cref="Obj"/>.
/// </summary>
public class Obj : ObjBase, System.Collections.Generic.IEnumerable<NamedVal>, System.Collections.IEnumerable
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

		System.Collections.Generic.SortedDictionary<string, NamedVal> values = [];

		foreach(System.Text.Json.JsonProperty jsonpCur in jsoneProperty.EnumerateObject())
			values[jsonpCur.Name] = new NamedVal(jsonpCur);

		this.values = values;
	}

	/// <summary>
	/// Named values in the object indexed by their name.
	/// </summary>
	public readonly System.Collections.Generic.IReadOnlyDictionary<string, NamedVal> values;

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
}