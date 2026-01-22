namespace WillPittenger.Goodies.Data;

[System.ComponentModel.ImmutableObject(true)]
public record MusicFileMetaData
(
	in System.IO.FileInfo fileSrc,
	in Data.MetaData mdInput
) : AudioOrVidFileMetaData(fileSrc, mdInput)
{
	public new const string strKind = @"Music";

	public readonly string strAlbum = mdInput[FieldNames.strAlbum];
	public readonly string strAlbumArtist = mdInput[FieldNames.strAlbumArtist];
	public readonly string strAuthors = mdInput[FieldNames.strAuthors];
	public readonly string strContributingArtists = mdInput[FieldNames.strContributingArtists];
	public readonly string strGenre = mdInput[FieldNames.strGenre];
	public readonly string strPartOfSet = mdInput[FieldNames.strPartOfSet];
	public readonly uint uiTrackNum = uint.Parse(mdInput[FieldNames.strTrackNum]);
	public readonly string strYear = mdInput[FieldNames.strYear];

	public static explicit operator MusicFileMetaData(MetaData mdInput)
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

	public new static class FieldNames
	{
		public const string strAlbum = @"Album";
		public const string strAlbumArtist = @"Album artist";
		public const string strAuthors = @"Authors";
		public const string strContributingArtists = @"Contributing artists";
		public const string strGenre = @"Genre";
		public const string strPartOfSet = @"Part of set";
		public const string strTrackNum = @"#";
		public const string strYear = @"Year";
	}
};