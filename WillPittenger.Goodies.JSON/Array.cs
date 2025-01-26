// Ignore Spelling: jsone Org JSON

namespace WillPittenger.Goodies.JSON;

/// <summary>
/// Represents an array of JSON values.
/// </summary>
/// <seealso cref="ObjBase"/>
/// <seealso cref="Val"/>
public class Array : ObjBase, System.Collections.Generic.IEnumerable<ObjBase>, System.Collections.IEnumerable
{
	/// <summary>
	/// Constructs a <see cref="Array"/> instance based on a <see cref="System.Text.Json.JsonElement"/> instance.
	/// </summary>
	/// <param name="jsoneProperty">The property to convert.</param>
	public Array(System.Text.Json.JsonElement jsoneProperty)
	{
		System.Diagnostics.Debug.Assert(jsoneProperty.ValueKind == System.Text.Json.JsonValueKind.Array);
		if(jsoneProperty.ValueKind != System.Text.Json.JsonValueKind.Array)
			throw new System.InvalidOperationException("This JSON element isn't an array");

		System.Collections.Generic.List<ObjBase> elements = new System.Collections.Generic.List<ObjBase>();

		foreach(System.Text.Json.JsonElement jsoneCur in jsoneProperty.EnumerateArray())
			elements.Add(new Val(jsoneCur));

		this.elements = elements;
	}

	/// <summary>
	/// Stores the elements found.
	/// </summary>
	public readonly System.Collections.Generic.IReadOnlyList<ObjBase> elements;

	/// <summary>
	/// Allows you to enumerate the items.
	/// </summary>
	/// <returns>An enumerator</returns>
	public System.Collections.IEnumerator GetEnumerator() => elements.GetEnumerator();

	/// <summary>
	/// Allows you to enumerate the items.
	/// </summary>
	/// <returns>An enumerator</returns>
	System.Collections.Generic.IEnumerator<ObjBase> System.Collections.Generic.IEnumerable<ObjBase>.GetEnumerator() => elements.GetEnumerator();
}