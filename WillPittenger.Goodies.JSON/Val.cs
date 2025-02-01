// Ignore Spelling: Org JSON jsone

namespace WillPittenger.Goodies.JSON;

/// <summary>
/// Represents any single value in a JSON object.  This can include arrays, doubles, booleans, and <c>null</c>.  Instances of <see cref="Val"/> don't have names
/// and are effectively just wrappers around a .NET type or the <see langword="null"/>.  For a value that has a name, use <see cref="NamedVal"/> instead.
/// </summary>
/// <seealso cref="NamedVal"/>
public class Val : ObjBase
{
	/// <summary>
	/// Constructs a <see cref="Val"/> based on what it finds inside <paramref name="jsoneVal"/>.
	/// </summary>
	/// <param name="jsoneVal">The value to convert.  This should be a <see cref="System.Text.Json.JsonElement"/>.</param>
	public Val(System.Text.Json.JsonElement jsoneVal)
	{
		switch(jsoneVal.ValueKind)
		{
			case System.Text.Json.JsonValueKind.Undefined:
				objVal = System.Text.Json.JsonValueKind.Undefined;

				break;

			case System.Text.Json.JsonValueKind.Object:
				objVal = new Obj(jsoneVal);

				break;

			case System.Text.Json.JsonValueKind.Array:
				objVal = new Array(jsoneVal);

				break;

			case System.Text.Json.JsonValueKind.String:
				objVal = jsoneVal.GetString();

				break;

			case System.Text.Json.JsonValueKind.Number:
				objVal = jsoneVal.GetDouble();

				break;

			case System.Text.Json.JsonValueKind.True:
			case System.Text.Json.JsonValueKind.False:
				objVal = jsoneVal.GetBoolean();

				break;

			case System.Text.Json.JsonValueKind.Null:
				objVal = null;

				break;
		}
	}

	/// <summary>
	/// Constructs a new <see cref="Val"/> containing the value in <paramref name="objVal"/>.
	/// </summary>
	/// <param name="objVal"></param>
	public Val(object? objVal)
		=> this.objVal = objVal;

	/// <summary>
	/// The value found.
	/// </summary>
	public readonly object? objVal;

	/// <inheritdoc/>
	public override string ToString()
		=> objVal?.ToString() ?? "";
}