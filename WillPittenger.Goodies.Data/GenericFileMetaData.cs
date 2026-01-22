namespace WillPittenger.Goodies.Data;

[System.ComponentModel.ImmutableObject(true)]
public abstract record GenericFileMetaData
(
	in System.IO.FileInfo fileSrc,
	in MetaData mdInput
) : BaseMetaData
{
	public readonly System.IO.FileInfo fileSrc = fileSrc;
	public readonly string strPath = mdInput[FieldNames.strPath];
	public readonly string strSpaceFree = mdInput[FieldNames.strSpaceFree];
	public readonly string strSpaceUsed = mdInput[FieldNames.strSpaceUsed];
	public readonly string strTotalSize = mdInput[FieldNames.strTotalSize];

	public new static class FieldNames
	{
		public const string strPath = @"Path";
		public const string strSpaceFree = @"Space free";
		public const string strSpaceUsed = @"Space used";
		public const string strTotalSize = @"Total size";
	}
}