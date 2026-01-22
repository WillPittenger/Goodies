namespace WillPittenger.Goodies.Data;

[System.ComponentModel.ImmutableObject(true)]
public abstract record GenericMediaFileMetaData
(
	in System.IO.FileInfo fileSrc,
	in MetaData mdInput
) : GenericFileMetaData(fileSrc, mdInput)
{
	public readonly string strAttr = mdInput[FieldNames.strAttr];
	public readonly string strComputer = mdInput[FieldNames.strComputer];
	public readonly string strItemType = mdInput[FieldNames.strItemType];
	public readonly string strLinkStatus = mdInput[FieldNames.strLinkStatus];
	public readonly string strOwner = mdInput[FieldNames.strOwner];
	public readonly string strPerceivedType = mdInput[FieldNames.strPerceivedType];
	public readonly string strProtected = mdInput[FieldNames.strProtected];
	public readonly bool bShared = mdInput[FieldNames.strShared].ToLower().Equals(@"yes");
	public readonly string strType = mdInput[FieldNames.strType];

	public new static class FieldNames
	{
		public const string strAttr = @"Attributes";
		public const string strComputer = @"Computer";
		public const string strItemType = @"Item type";
		public const string strLinkStatus = @"Link status";
		public const string strOwner = @"Owner";
		public const string strPerceivedType = @"Perceived type";
		public const string strProtected = @"Protected";
		public const string strShared = @"Shared";
		public const string strType = @"Type";
	}
}