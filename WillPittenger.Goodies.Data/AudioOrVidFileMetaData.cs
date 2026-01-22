namespace WillPittenger.Goodies.Data;

[System.ComponentModel.ImmutableObject(true)]
public abstract record AudioOrVidFileMetaData
(
	in System.IO.FileInfo fileSrc,
	in Data.MetaData mdInput
) : GenericMediaFileMetaData(fileSrc, mdInput)
{
	public readonly string strBitRate = mdInput[FieldNames.strBitRate];
	public readonly string strCopyright = mdInput[FieldNames.strCopyright];
	public readonly System.TimeSpan tsLength = System.TimeSpan.Parse(mdInput[FieldNames.strLength]);
	public readonly string strRating = mdInput[FieldNames.strRating];
	public readonly string strTitle = mdInput[FieldNames.strTitle];

	public new static class FieldNames
	{
		public const string strBitRate = @"Bit rate";
		public const string strCopyright = @"Copyright";
		public const string strLength = @"Length";
		public const string strRating = @"Rating";
		public const string strTitle = @"Title";
	}
}