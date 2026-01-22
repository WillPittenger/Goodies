namespace WillPittenger.Goodies.Data;

[System.ComponentModel.ImmutableObject(true)]
public record DocFileMetaData
(
	in System.IO.FileInfo fileSrc,
	in MetaData mdInput
) : GenericFileMetaData(fileSrc, mdInput)
{
	public new const string strKind = "Document";


	public new static class FieldNames
	{
	}

	public static explicit operator DocFileMetaData(MetaData mdInput)
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