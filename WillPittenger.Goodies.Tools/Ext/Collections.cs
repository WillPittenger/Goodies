// Ignore Spelling: Util Sep

namespace WillPittenger.Goodies.Tools.Ext;

public static class Collections
{
	public static bool IsEmpty<ElementType>(this System.Collections.Generic.IReadOnlyCollection<ElementType> collection)
		=> collection.Count == 0;

	public static bool IsEmpty<ElementType>(this System.Collections.Generic.IList<ElementType> collection)
		=> collection.Count == 0;

	public static bool IsEmpty<ElementType>(this System.Collections.ObjectModel.ReadOnlyCollection<ElementType> collection)
		=> collection.Count == 0;

	public static bool IsEmpty(this System.Collections.IList collection)
		=> collection.Count == 0;

	public static bool IsEmpty<KeyType, ValueType>(this System.Collections.Generic.SortedList<KeyType, ValueType> collection)
		where KeyType : notnull
		=> collection.Count == 0;

	public static bool IsEmpty<EntryType>(this System.Collections.Generic.LinkedList<EntryType> collection)
		=> collection.Count == 0;

	public static bool IsEmpty<SetEntryType>(this System.Collections.Generic.ISet<SetEntryType>? set)
		=> set == null || set.Count == 0;

	public static string Join(this System.Collections.IEnumerable objects, string strSep)
		=> string.Join(strSep, objects);
}