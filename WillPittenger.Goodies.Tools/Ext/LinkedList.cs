namespace WillPittenger.Goodies.Tools.Ext;

public static class LinkedList
{
	public static void AddRangeFirst<T>(this System.Collections.Generic.LinkedList<T> llistAddTo, in System.Collections.Generic.IEnumerable<T> eWhatToAdd)
	{
		foreach(T curNewEntry in eWhatToAdd)
			llistAddTo.AddFirst(curNewEntry);
	}

	public static void AddRangeLast<T>(this System.Collections.Generic.LinkedList<T> llistAddTo, in System.Collections.Generic.IEnumerable<T> eWhatToAdd)
	{
		foreach(T curNewEntry in eWhatToAdd)
			llistAddTo.AddLast(curNewEntry);
	}

	public static void AddRangeBefore<T>(this System.Collections.Generic.LinkedList<T> llistAddTo, in System.Collections.Generic.LinkedListNode<T> llnBeforeThis, in System.Collections.Generic.IEnumerable<T> eWhatToAdd)
	{
		System.Collections.Generic.LinkedListNode<T> llnCurBefore = llnBeforeThis;

		foreach(T curNewEntry in eWhatToAdd)
			llnCurBefore = llistAddTo.AddBefore(llnCurBefore, curNewEntry);
	}

	public static void AddRangeAfter<T>(this System.Collections.Generic.LinkedList<T> llistAddTo, in System.Collections.Generic.LinkedListNode<T> llnBeforeThis, in System.Collections.Generic.IEnumerable<T> eWhatToAdd)
	{
		System.Collections.Generic.LinkedListNode<T> llnCurBefore = llnBeforeThis;

		foreach(T curNewEntry in eWhatToAdd)
			llnCurBefore = llistAddTo.AddAfter(llnCurBefore, curNewEntry);
	}
}