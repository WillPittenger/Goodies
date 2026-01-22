namespace WillPittenger.Goodies.Data;

[System.ComponentModel.ImmutableObject(true)]
public record VidMetaData
(
	in System.IO.FileInfo fileSrc,
	in Data.MetaData mdInput
) : GenericFileMetaData(fileSrc, mdInput)
{
	public new const string strKind = "video";

	public readonly string strBitRate = mdInput[FieldNames.strBitRate];
	public readonly string strComputer = mdInput[FieldNames.strComputer];
	public readonly string strItemType = mdInput[FieldNames.strItemType];
	public readonly System.TimeSpan tsLength = System.TimeSpan.Parse(mdInput[FieldNames.strLength]);
	public readonly string strOwner = mdInput[FieldNames.strOwner];
	public readonly string strPerceivedType = mdInput[FieldNames.strPerceivedType];
	public readonly string strRating = mdInput[FieldNames.strRating];
	public readonly string strTYpe = mdInput[FieldNames.strType];

	public new static class FieldNames
	{
		public const string strBitRate = @"Bit rate";
		public const string strComputer = @"Computer";
		public const string strItemType = @"Item type";
		public const string strLength = @"Length";
		public const string strOwner = @"Owner";
		public const string strPerceivedType = @"Perceived type";
		public const string strProtected = @"Protected";
		public const string strRating = @"Rating";
		public const string strType = @"Type";
	}

	public static explicit operator VidMetaData(MetaData mdInput)
	{
		Tools.Exceptions.AssertOrThrow.TestIt
		(
			mdInput[BaseMetaData.FieldNames.strKind].Equals(strKind, System.StringComparison.CurrentCultureIgnoreCase),
			()
				=> throw new System.InvalidOperationException($@"The file specified by “{mdInput.Us.FullName}” isn't a media file.")
		);

		return new
			(
				mdInput.Us,
				mdInput
			);
	}
}