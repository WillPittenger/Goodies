// Ignore Spelling: Org JSON jsone

namespace WillPittenger.Goodies.JSON;

/// <summary>
/// Placeholder for any JSON object.
/// </summary>
public abstract class ObjBase
{
	/// <summary>
	/// Ensures that derived classes can create the base class, but others can't.
	/// </summary>
	protected ObjBase()
	{
	}

	/// <summary>
	/// Builds a <see cref="ObjBase"/> based on what's in <paramref name="jsoneVal"/>.
	/// </summary>
	/// <param name="jsoneVal">The JSON value to convert.</param>
	/// <returns>The new value</returns>
	/// <exception cref="Tools.Exceptions.UnknownOrInvalidEnumException{System.Text.Json.JsonValueKind}">The code couldn't figure out what to do with the value in
	/// <paramref name="jsoneVal"/>.  It could be the JSON undefined value.</exception>
	public static ObjBase Make(System.Text.Json.JsonElement jsoneVal)
	{
		return jsoneVal.ValueKind switch
		{
			System.Text.Json.JsonValueKind.Object
				=> new Obj(jsoneVal),

			System.Text.Json.JsonValueKind.Array
				=> new Array(jsoneVal),

			System.Text.Json.JsonValueKind.String or
			System.Text.Json.JsonValueKind.Number or
			System.Text.Json.JsonValueKind.True or
			System.Text.Json.JsonValueKind.False or
			System.Text.Json.JsonValueKind.Null
				=> new Val(jsoneVal),

			_
				=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<System.Text.Json.JsonValueKind>(jsoneVal.ValueKind, "while converting JSON into " +
					"internal structure"),
		};
	}
}