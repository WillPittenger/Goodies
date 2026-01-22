namespace WillPittenger.Goodies.Data;

[System.ComponentModel.ImmutableObject(true)]
public record ImgFileMetaData
(
	in System.IO.FileInfo fileSrc,
	in Data.MetaData mdInput
) : AudioOrVidFileMetaData(fileSrc, mdInput)
{
	public new const string strKind = @"Picture";

	public readonly ulong ulHeight = ulong.Parse(mdInput[FieldNames.strHeight]);
	public readonly string strHorzRes = mdInput[FieldNames.strHorzRes];
	public readonly string strVertRes = mdInput[FieldNames.strVertRes];
	public readonly ulong ulWidth = ulong.Parse(mdInput[FieldNames.strWidth]);

	public new static class FieldNames
	{
		public const string strHeight = @"Height";
		public const string strHorzRes = @"Horizontal resolution";
		public const string strVertRes = @"Vertical resolution";
		public const string strWidth = "Width";
	}

	public static explicit operator ImgFileMetaData(MetaData mdInput)
	{
		Tools.Exceptions.AssertOrThrow.TestIt
		(
			mdInput[BaseMetaData.FieldNames.strKind].Equals(strKind, System.StringComparison.CurrentCultureIgnoreCase),
			()
				=> throw new System.InvalidOperationException($@"The file specified by “{mdInput.Us.FullName}” isn't a music file.")
		);

		return new
			(
				mdInput.Us,
				mdInput
			);
	}
}