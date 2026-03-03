namespace WillPittenger.Goodies.YtDlpWrapper.Ext;

public static class PyList
{
	public static System.Collections.Generic.IEnumerable<object?> ToManagedList(this Python.Runtime.PyList pylistInput)
	{
		System.Collections.Generic.LinkedList<object?> listResult = [];

		using Python.Runtime.Py.GILState lockInfo = Python.Runtime.Py.GIL();

		foreach(Python.Runtime.PyObject pyobjCur in pylistInput)
		{
			if(Python.Runtime.PyDict.IsDictType(pyobjCur))
				listResult.AddLast(new Python.Runtime.PyDict(pyobjCur).ToManagedDictionary());
			else if(Python.Runtime.PyList.IsListType(pyobjCur))
				listResult.AddLast(new Python.Runtime.PyList(pyobjCur).ToManagedList());
			else if(pyobjCur.IsNone())
				listResult.AddLast(new(null));
			else
				listResult.AddLast(pyobjCur.AsManagedObject(typeof(object)));
		}

		return listResult;
	}
}