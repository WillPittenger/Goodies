namespace WillPittenger.Goodies.PowerShell.Data;

[System.Management.Automation.CmdletBinding]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"FileMetaData")]
public class GetFileMetaData : System.Management.Automation.Cmdlet
{
	[System.Management.Automation.Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
	public System.Collections.Generic.IEnumerable<IWrapper> Locations
	{
		get;

		set;
	} = [];

	[System.Management.Automation.Parameter()]
	public string Mask
	{
		get;

		set;
	} = string.Empty;

	[System.Management.Automation.Parameter()]
	public System.Management.Automation.SwitchParameter Recurse
	{
		get;

		set;
	}

	public interface IWrapper;

	public class StringPathWrapper(in string strPathToWrap) : IWrapper
	{
		public readonly string strWrappedPath = strPathToWrap;

		public static implicit operator StringPathWrapper(in string strPathToWrap)
			=> new(strPathToWrap);
	}

	public class FileInfoWrapper(in System.IO.FileInfo fileWhatToWrap) : IWrapper
	{
		public readonly System.IO.FileInfo fileCtnts = fileWhatToWrap;

		public static implicit operator FileInfoWrapper(in System.IO.FileInfo fileWhatToWrap)
			=> new(fileWhatToWrap);
	}

	public class DirInfoWrapper(in System.IO.DirectoryInfo dirWhatToWrap) : IWrapper
	{
		public readonly System.IO.DirectoryInfo dirCtnts = dirWhatToWrap;

		public static implicit operator DirInfoWrapper(in System.IO.DirectoryInfo dirWhatToWrap)
			=> new(dirWhatToWrap);
	}

	protected override void ProcessRecord()
	{
		base.ProcessRecord();

		foreach(IWrapper wrapperCur in Locations)
		{
			if(Stopping)
				return;

			if(wrapperCur is StringPathWrapper spwCur)
			{
				if(System.IO.Directory.Exists(spwCur.strWrappedPath))
					WriteObject(Goodies.Data.MetaData.GetMetaDataForFiles(Mask, Recurse, new System.IO.DirectoryInfo(spwCur.strWrappedPath)));
				else if(System.IO.File.Exists(spwCur.strWrappedPath))
					WriteObject(Goodies.Data.MetaData.GetMetaDataForFiles(new System.IO.FileInfo(spwCur.strWrappedPath)));
				else
					WriteError(new(new System.InvalidOperationException($@"The value “{spwCur.strWrappedPath}” doesn't seem to be a valid path to either a file or a directory/folder"), @"Invalid string path", System.Management.Automation.ErrorCategory.InvalidData, spwCur.strWrappedPath));
			}
			else if(wrapperCur is FileInfoWrapper fiwCur)
				WriteObject(Goodies.Data.MetaData.GetMetaDataForFiles(fiwCur.fileCtnts));
			else if(wrapperCur is DirInfoWrapper diwCur)
				WriteObject(Goodies.Data.MetaData.GetMetaDataForFiles(Mask, Recurse, diwCur.dirCtnts));
			else
				WriteError(new(new System.InvalidProgramException($@"Normally, you can pass any combination of strings, FileInfo objects, and DirectoryInfo objects needed to Get-FileMetaData.  It has a system where those types and only those types can be placed in Locations via wrapper classes.  But somehow you managed to put an unknown wrapper class in Locations.  That type is {wrapperCur.GetType()}.  Your build of Get-FileMetaData might be corrupt.  Please reinstall with a fresh copy."), @"Invalid or unknown wrapper type", System.Management.Automation.ErrorCategory.InvalidType, wrapperCur));
		}
	}
}