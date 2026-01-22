// Ignore Spelling: efsi Recurse

namespace WillPittenger.Goodies.Data;

using System.Linq;

[System.ComponentModel.ImmutableObject(true)]
public record MetaData : BaseMetaData
{
	public MetaData(System.IO.FileInfo fileWhatToLookUp)
	{
		Tools.Exceptions.AssertOrThrow.TestIt
		(
			fileWhatToLookUp.Exists,
			()
				=> throw new System.InvalidOperationException($@"The file “{fileWhatToLookUp.FullName}” doesn't exist!")
		);
		Tools.Exceptions.AssertOrThrow.TestIt
		(
			fileWhatToLookUp.Directory is not null,
			()
				=> throw new System.InvalidOperationException($@"The file “{fileWhatToLookUp.FullName}” doesn't seem to have a parent folder.")
		);
		if(fileWhatToLookUp.Directory is System.IO.DirectoryInfo dirParentOfOurFile)
		{
			Tools.Exceptions.AssertOrThrow.TestIt
			(
				!dirParentOfOurFile.Exists,
				()
					=> throw new System.InvalidOperationException($@"The file specified “{fileWhatToLookUp.FullName}” is in a non-existent directory!")
			);

			OurParentDir = dirParentOfOurFile;
		}
		else
			throw new System.InvalidOperationException($@"The file specified “{fileWhatToLookUp.FullName}” doesn't seem to have a parent folder.");

		Us = fileWhatToLookUp;
		Fields = GetFieldData();
		sfOurParent = shell.NameSpace(dirParentOfOurFile.FullName);
	}

	private const ushort usHighestKnownField = 266;


	/// <summary>
	/// Used to access the shell.
	/// </summary>
	private static readonly Windows.Win32.UI.Shell.IShellDispatch2 shell = (Windows.Win32.UI.Shell.IShellDispatch2)new Windows.Win32.UI.Shell.Shell();

	private readonly Windows.Win32.UI.Shell.Folder sfOurParent;

	public System.IO.FileInfo Us
	{
		get;
	}

	public System.IO.DirectoryInfo OurParentDir
	{
		get;
	}

	public System.Collections.Generic.IReadOnlyDictionary<string, string> Fields
	{
		get;
	}

	private System.Collections.Generic.IReadOnlyDictionary<string, string> GetFieldData()
	{
		System.Collections.Generic.SortedDictionary<string, string> mapFields = [];

		using Windows.Win32.Foundation.BSTR bstrNameOfCurInputFile = OurParentDir.Name;

		System.Collections.Generic.SortedDictionary<string, ushort> mapFieldNamesToIndices = [];

		for(ushort usCurField = 0; usCurField <= usHighestKnownField; usCurField++)
		{
			object objCurFieldName = sfOurParent.GetDetailsOf(null, usCurField);

			if(objCurFieldName is string strCurFieldName)
				mapFieldNamesToIndices[strCurFieldName.Trim()] = usCurField;
		}

		foreach(string strCurFieldName in mapFieldNamesToIndices.Keys)
			mapFields[strCurFieldName] = ((string)sfOurParent.GetDetailsOf(sfOurParent.ParseName(bstrNameOfCurInputFile), mapFieldNamesToIndices[strCurFieldName])).Trim();

		return mapFields;
	}

	public static System.Collections.Generic.IEnumerable<BaseMetaData> GetMetaDataForFiles(params System.Collections.Generic.IEnumerable<System.IO.FileSystemInfo> efsiWhatToLookUp)
		=> GetMetaDataForFiles(@"*", false, efsiWhatToLookUp);

	public static System.Collections.Generic.IEnumerable<BaseMetaData> GetMetaDataForFiles(in bool bRecurse, params System.Collections.Generic.IEnumerable<System.IO.FileSystemInfo> efsiWhatToLookUp)
		=> GetMetaDataForFiles(@"*", bRecurse, efsiWhatToLookUp);

	public static System.Collections.Generic.IEnumerable<BaseMetaData> GetMetaDataForFiles(in string strMask, params System.Collections.Generic.IEnumerable<System.IO.FileSystemInfo> efsiWhatToLookUp)
		=> GetMetaDataForFiles(strMask, false, efsiWhatToLookUp);

	public static System.Collections.Generic.IEnumerable<BaseMetaData> GetMetaDataForFiles(in string strMask, in bool bRecurse, params System.Collections.Generic.IEnumerable<System.IO.FileSystemInfo> efsiWhatToLookUp)
	{
		System.Collections.Generic.List<BaseMetaData> listmdResults = [];

		foreach(System.IO.FileSystemInfo fsiCur in efsiWhatToLookUp)
			if(fsiCur is System.IO.DirectoryInfo dirCurChild)
				listmdResults.AddRange
					(
						from fileCurInDir in
							dirCurChild.EnumerateFiles
							(
								strMask,
								new System.IO.EnumerationOptions()
								{
									IgnoreInaccessible = true,
									RecurseSubdirectories = bRecurse,
								}
							)
						select new MetaData(fileCurInDir) into mdNew
						select mdNew[FieldNames.strKind] switch
						{
							MusicFileMetaData.strKind
								=> (MusicFileMetaData)mdNew,

							VidMetaData.strKind
								=> (VidMetaData)mdNew,

							GenericMediaFileMetaData.strKind
								=> (GenericMediaFileMetaData)mdNew,

							GenericFileMetaData.strKind
								=> (GenericFileMetaData)mdNew,

							_
								=> (BaseMetaData)mdNew,
						}
					);
			else if(fsiCur is System.IO.FileInfo fileCurInDir)
			{
				MetaData mdNew = new(fileCurInDir);
				listmdResults.Add
				(
					mdNew[FieldNames.strKind] switch
					{
						MusicFileMetaData.strKind
							=> (MusicFileMetaData)mdNew,

						VidMetaData.strKind
							=> (VidMetaData)mdNew,

						GenericMediaFileMetaData.strKind
							=> (GenericMediaFileMetaData)mdNew,

						GenericFileMetaData.strKind
							=> (GenericFileMetaData)mdNew,

						_
							=> mdNew,
					}
				);
			}

		return listmdResults;
	}

	public string this[string strWhichKey]
		=> Fields[strWhichKey];
}