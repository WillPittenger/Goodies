namespace WillPittenger.Goodies.Data;

[System.ComponentModel.ImmutableObject(true)]
public record MusicFileMetaData
(
	in System.IO.FileInfo fileSrc,
	in Data.MetaData mdInput
) : GenericFileMetaData(fileSrc, mdInput)
{
	public new const string strKind = @"music";

	public readonly string strAlbum = mdInput[FieldNames.strAlbum];
	public readonly string strAlbumArtist = mdInput[FieldNames.strAlbumArtist];
	public readonly string strAttr = mdInput[FieldNames.strAttr];
	public readonly string strAuthors = mdInput[FieldNames.strAuthors];
	public readonly string strBitRate = mdInput[FieldNames.strBitRate];
	public readonly string strComputer = mdInput[FieldNames.strComputer];
	public readonly string strContributingArtists = mdInput[FieldNames.strContributingArtists];
	public readonly string strCopyright = mdInput[FieldNames.strCopyright];
	public readonly string strGenre = mdInput[FieldNames.strGenre];
	public readonly string strItemType = mdInput[FieldNames.strItemType];
	public readonly System.TimeSpan tsLength = System.TimeSpan.Parse(mdInput[FieldNames.strLength]);
	public readonly string strLinkStatus = mdInput[FieldNames.strLinkStatus];
	public readonly string strOwner = mdInput[FieldNames.strOwner];
	public readonly string strPartOfSet = mdInput[FieldNames.strPartOfSet];
	public readonly string strPerceivedType = mdInput[FieldNames.strPerceivedType];
	public readonly string strProtected = mdInput[FieldNames.strProtected];
	public readonly string strRating = mdInput[FieldNames.strRating];
	public readonly bool bShared = mdInput[FieldNames.strShared].ToLower().Equals(@"yes");
	public readonly string strTitle = mdInput[FieldNames.strTitle];
	public readonly uint uiTrackNum = uint.Parse(mdInput[FieldNames.strTrackNum]);
	public readonly string strType = mdInput[FieldNames.strType];
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
		public const string strAttr = @"Attributes";
		public const string strAuthors = @"Authors";
		public const string strBitRate = @"Bit rate";
		public const string strComputer = @"Computer";
		public const string strContributingArtists = @"Contributing artists";
		public const string strCopyright = @"Copyright";
		public const string strGenre = @"Genre";
		public const string strItemType = @"Item type";
		public const string strLength = @"Length";
		public const string strLinkStatus = @"Link status";
		public const string strOwner = @"Owner";
		public const string strPartOfSet = @"Part of set";
		public const string strPerceivedType = @"Perceived type";
		public const string strProtected = @"Protected";
		public const string strRating = @"Rating";
		public const string strShared = @"Shared";
		public const string strTitle = @"Title";
		public const string strTrackNum = @"#";
		public const string strType = @"Type";
		public const string strYear = @"Year";
	}
};