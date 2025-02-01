// Ignore Spelling: Org JSON

namespace WillPittenger.Goodies.JSON;

/// <summary>
/// Provides a means to store a named value.  This is like a field.
/// </summary>
/// <remarks>
/// Constructs a <see cref="NamedVal"/>.
/// </remarks>
public class NamedVal : ObjBase
{
	/// <summary>
	/// Constructs a new <see cref="NamedVal"/> based on a <see cref="System.Text.Json.JsonProperty"/> value.  This is what you'd call after loading JSON.
	/// </summary>
	/// <param name="jsonp">The value to convert</param>
	public NamedVal(System.Text.Json.JsonProperty jsonp)
	{
		strName = jsonp.Name;
		val = new(jsonp.Value);
	}

	/// <summary>
	/// Constructs a new <see cref="NamedVal"/> given a name and any .NET object including <see langword="null"/>.
	/// </summary>
	/// <param name="strName">The name of the field</param>
	/// <param name="objVal">The new value.  This can be <see langword="null"/>.</param>
	public NamedVal(string strName, object? objVal)
	{
		this.strName = strName;
		val = new(objVal);
	}

	/// <summary>
	/// Same as <see cref="NamedVal(string, object?)"/>, but assumes a value that's already wrapped.
	/// </summary>
	/// <param name="strName">The new name</param>
	/// <param name="val">THe wrapped object</param>
	public NamedVal(string strName, Val val)
	{
		this.strName = strName;
		this.val = val;
	}


	/// <summary>
	/// This the name of the value found.
	/// </summary>
	public readonly string strName;

	/// <summary>
	/// This the value found.
	/// </summary>
	public readonly Val val;

	/// <inheritdoc/>
	public override string ToString()
		=> $"{strName}: {val}";
}