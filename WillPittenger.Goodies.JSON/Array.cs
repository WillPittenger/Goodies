// Ignore Spelling: objb

using System.Linq;
using WillPittenger.Goodies.Tools.Ext;
// Ignore Spelling: jsone Org JSON

namespace WillPittenger.Goodies.JSON;

/// <summary>
/// Represents an array of JSON values.
/// </summary>
/// <seealso cref="ObjBase"/>
/// <seealso cref="Val"/>
public class Array : ObjBase, System.Collections.Generic.IEnumerable<ObjBase>, System.Collections.IEnumerable, System.Collections.Specialized
	.INotifyCollectionChanged
{
	/// <summary>
	/// Constructs a <see cref="Array"/> instance based on a <see cref="System.Text.Json.JsonElement"/> instance.
	/// </summary>
	/// <param name="jsoneProperty">The property to convert.</param>
	public Array(in System.Text.Json.JsonElement jsoneProperty)
	{
		System.Diagnostics.Debug.Assert(jsoneProperty.ValueKind == System.Text.Json.JsonValueKind.Array);
		if(jsoneProperty.ValueKind != System.Text.Json.JsonValueKind.Array)
			throw new System.InvalidOperationException("This JSON element isn't an array");

		elements.AddRange(
			from System.Text.Json.JsonElement jsoneCur in jsoneProperty.EnumerateArray()
				select Make(jsoneCur)
		);
	}

	/// <summary>
	/// Lets you construct a <see cref=" Array"/> from a list of <see cref="ObjBase"/> instances.
	/// </summary>
	/// <param name="objects">A <see cref="System.Collections.Generic.IEnumerable{ObjBase}"/> containing the new objects</param>
	public Array(in System.Collections.Generic.IEnumerable<ObjBase> objects)
		=> elements.AddRange(objects);

	/// <summary>
	/// Stores the elements found.
	/// </summary>
	private readonly System.Collections.Generic.List<ObjBase> elements = [];

	/// <summary>
	/// Fired when an item is added.
	/// </summary>
	public event System.Collections.Specialized.NotifyCollectionChangedEventHandler? CollectionChanged;

	/// <summary>
	/// Use to enumerate the items.
	/// </summary>
	public System.Collections.Generic.IReadOnlyList<ObjBase> Elements
		=> elements;

	/// <summary>
	/// Use to add one new item to the array.
	/// </summary>
	/// <param name="objbNew">The new item</param>
	public void Add(in ObjBase objbNew)
	{
		elements.Add(objbNew);

		CollectionChanged?.Invoke(this, new(System.Collections.Specialized.NotifyCollectionChangedAction.Add, objbNew));
	}

	/// <summary>
	/// Lets you add many items at once.  This is more efficient than calling <see cref="Add(ObjBase)"/> repeatedly.
	/// </summary>
	/// <param name="objects">A <see cref="System.Collections.Generic.IEnumerable{ObjBase}"/> containing the new items.</param>
	public void Add(in System.Collections.Generic.IEnumerable<ObjBase> objects)
	{
		elements.AddRange(objects);

		CollectionChanged?.Invoke(this, new(System.Collections.Specialized.NotifyCollectionChangedAction.Add,
			(System.Collections.IList)objects));
	}

	/// <summary>
	/// Use to insert a single entry into a specific position.
	/// </summary>
	/// <param name="iIndexToInsertAt">Where to put the array.  This must be greater than <c>0</c> and less than the length of the array.</param>
	/// <param name="objbNew">The new entry</param>
	/// <exception cref="System.ArgumentOutOfRangeException">You specified a value that was outside what the array could handle.</exception>
	public void Insert(in int iIndexToInsertAt, in ObjBase objbNew)
	{
		elements.Insert(iIndexToInsertAt, objbNew);

		CollectionChanged?.Invoke(this, new(System.Collections.Specialized.NotifyCollectionChangedAction.Add, objbNew, iIndexToInsertAt));
	}

	/// <summary>
	/// Allows you to enumerate the items.
	/// </summary>
	/// <returns>An enumerator</returns>
	public System.Collections.IEnumerator GetEnumerator()
		=> elements.GetEnumerator();

	/// <summary>
	/// Allows you to enumerate the items.
	/// </summary>
	/// <returns>An enumerator</returns>
	System.Collections.Generic.IEnumerator<ObjBase> System.Collections.Generic.IEnumerable<ObjBase>.GetEnumerator()
		=> elements.GetEnumerator();

	/// <inheritdoc/>
	public override string ToString()
		=> $"{{{elements.Select(
			curItem
				=> curItem.ToString()
			).Join(',')}}}";
}