namespace WillPittenger.Goodies.Data;

[System.ComponentModel.ImmutableObject(true)]
public record VidFileMetaData
(
	in System.IO.FileInfo fileSrc,
	in Data.MetaData mdInput
) : AudioOrVidFileMetaData(fileSrc, mdInput)
{
	public new const string strKind = "Video";


	public new static class FieldNames
	{
	}

	public static explicit operator VidFileMetaData(MetaData mdInput)
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