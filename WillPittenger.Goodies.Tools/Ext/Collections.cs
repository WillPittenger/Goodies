// Ignore Spelling: Util Sep

using System.Linq;

namespace WillPittenger.Goodies.Tools.Ext;

/// <summary>
/// Set of extension methods for use on any collection
/// </summary>
public static class Collections
{
	/// <summary>
	/// Tests if the collection is empty by using the <see cref="System.Collections.Generic.IReadOnlyCollection{T}.Count"/> property.  This overload is for read
	/// only collection instances.
	/// </summary>
	/// <typeparam name="ElementType">The type of the collection</typeparam>
	/// <param name="collection">The collection to test</param>
	/// <returns><see langword="true"/> if the collection is empty and <see langword="false"/> otherwise</returns>
	public static bool IsEmpty<ElementType>(this System.Collections.Generic.IReadOnlyCollection<ElementType>? collection)
		=> collection == null || collection.Count == 0;

	/// <summary>
	/// Tests if the list is empty by using the <see cref="System.Collections.Generic.ICollection{T}.Count"/> property.  This overload is for any list, read only
	/// or otherwise.
	/// </summary>
	/// <typeparam name="ElementType">The type of the collection</typeparam>
	/// <param name="collection">The list to test</param>
	/// <returns><see langword="true"/> if the list is empty and <see langword="false"/> otherwise</returns>
	public static bool IsEmpty<ElementType>(this System.Collections.Generic.IList<ElementType>? collection)
		=> collection == null || collection.Count == 0;

	/// <summary>
	/// Tests if the collection is empty or not by using the <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}.Count"/> property.  This overload is
	/// for use with object model read only collections.
	/// </summary>
	/// <typeparam name="ElementType">The type of the collection</typeparam>
	/// <param name="collection">The collection to test</param>
	/// <returns><see langword="true"/> if the collection is empty and <see langword="false"/> otherwise</returns>
	public static bool IsEmpty<ElementType>(this System.Collections.ObjectModel.ReadOnlyCollection<ElementType>? collection)
		=> collection == null || collection.Count == 0;

	/// <summary>
	/// Tests if the list is empty by using the <see cref="System.Collections.ICollection.Count"/> property.  This overload is for any non-generic list.
	/// </summary>
	/// <param name="collection">The collection to test</param>
	/// <returns><see langword="true"/> if the collection is empty and <see langword="false"/> otherwise</returns>
	public static bool IsEmpty(this System.Collections.IList? collection)
		=> collection == null || collection.Count == 0;

	/// <summary>
	/// Tests if the sorted list is empty or not using <see cref="System.Collections.Generic.SortedList{TKey, TValue}.Count"/>.
	/// </summary>
	/// <typeparam name="KeyType">The type of the key for the sorted list</typeparam>
	/// <typeparam name="ValueType">The value type for the sorted list</typeparam>
	/// <param name="collection">The list to test</param>
	/// <returns><see langword="true"/> if the list is empty and <see langword="false"/> otherwise</returns>
	public static bool IsEmpty<KeyType, ValueType>(this System.Collections.Generic.SortedList<KeyType, ValueType>? collection)
		where KeyType : notnull
		=> collection == null || collection.Count == 0;

	/// <summary>
	/// Tests if the linked list is empty or not using <see cref="System.Collections.Generic.LinkedList{T}.Count"/>.
	/// </summary>
	/// <typeparam name="EntryType">The value type for the list</typeparam>
	/// <param name="collection">The linked list to test</param>
	/// <returns><see langword="true"/> if the linked list is empty and <see langword="false"/> otherwise.</returns>
	public static bool IsEmpty<EntryType>(this System.Collections.Generic.LinkedList<EntryType>? collection)
		=> collection == null || collection.Count == 0;

	/// <summary>
	/// Tests if the set is empty or not using <see cref="System.Collections.Generic.ICollection{T}.Count"/>.
	/// </summary>
	/// <typeparam name="SetEntryType">The element type for the set</typeparam>
	/// <param name="set">The set to test</param>
	/// <returns><see langword="true"/> if the set is empty and <see langword="false"/> otherwise</returns>
	public static bool IsEmpty<SetEntryType>(this System.Collections.Generic.ISet<SetEntryType>? set)
		=> set == null || set.Count == 0;

	/// <summary>
	/// Takes any enumerable collection and joins their string representations into a single string.  The value in <paramref name="strSep"/> is used as the
	/// separator.  Without this extension method, you'd need to call a static method in <see cref="string"/>, namely <see cref="string.Join(string?, System
	/// .Collections.Generic.IEnumerable{string?})"/>.
	/// </summary>
	/// <typeparam name="t">The type of the element</typeparam>
	/// <param name="objects">The collection to join</param>
	/// <param name="strSep">The separator value to use.  The default is an empty string.</param>
	/// <returns>The array joined into a string</returns>
	/// <seealso cref="Join(System.Collections.IEnumerable, char)"/>
	public static string Join<t>(this System.Collections.Generic.IEnumerable<t> objects, string strSep = "")
		=> string.Join(strSep, objects);

	/// <summary>
	/// Takes any enumerable collection and joins their string representations into a single string.  The value in <paramref name="chSep"/> is used as a separator.
	///  Without this method, you'd need to call a method in <see cref="string"/>, namely <see cref="string.Join{T}(char, System.Collections.Generic
	/// .IEnumerable{T})"/>.
	/// </summary>
	/// <typeparam name="t">The type of the element</typeparam>
	/// <param name="objects">The enumerable collection to join</param>
	/// <param name="chSep">The separator to use</param>
	/// <returns>The combined string representation for the collection</returns>
	/// <seealso cref="Join(System.Collections.IEnumerable, string)"/>
	public static string Join<t>(this System.Collections.Generic.IEnumerable<t> objects, char chSep)
		=> string.Join(chSep, objects.Select(curObj => curObj?.ToString() ?? ""));
}