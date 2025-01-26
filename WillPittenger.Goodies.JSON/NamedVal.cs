// Ignore Spelling: Org JSON

namespace WillPittenger.Goodies.JSON;

/// <summary>
/// Provides a means to store a named value.  This is like a field.
/// </summary>
/// <remarks>
/// Constructs a <see cref="NamedVal"/>.
/// </remarks>
/// <param name="jsonp">This is the parsed JSON.  A <see cref="System.Text.Json.JsonProperty"/> is expected.</param>
public class NamedVal(System.Text.Json.JsonProperty jsonp) : ObjBase
{
	/// <summary>
	/// This the name of the value found.
	/// </summary>
	public readonly string strName = jsonp.Name;

	/// <summary>
	/// This the value found.
	/// </summary>
	public readonly Val val = new(jsonp.Value);
}