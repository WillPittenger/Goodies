namespace WillPittenger.Goodies.YtDlpWrapper.Ext;

public static class PyDict
{
	public static System.Collections.Generic.Dictionary<string, object?> ToManagedDictionary(this Python.Runtime.PyDict dictInput)
	{
		System.Collections.Generic.Dictionary<string, object?> mapResult = [];

		foreach(Python.Runtime.PyObject pyobjCurKey in dictInput.Keys())
		{
			string strCurKey = pyobjCurKey.ToString() ?? throw new System.InvalidProgramException($@"Unable to convert {pyobjCurKey} to a string");

			Python.Runtime.PyObject pyobjEntry = dictInput[pyobjCurKey];

			if(Python.Runtime.PyDict.IsDictType(pyobjCurKey))
				mapResult[strCurKey] = new Python.Runtime.PyDict(pyobjCurKey).ToManagedDictionary();
			else if(Python.Runtime.PyList.IsListType(pyobjCurKey))
				mapResult[strCurKey] = new Python.Runtime.PyList(pyobjEntry).ToManagedList();
			else if(pyobjEntry.IsNone())
				mapResult[strCurKey] = null;
			else
				mapResult[strCurKey] = pyobjEntry.AsManagedObject(typeof(object));
		}

		return mapResult;
	}
}