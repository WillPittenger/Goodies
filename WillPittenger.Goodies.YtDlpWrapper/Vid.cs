// Ignore Spelling: Vid jsone Codec

using System.Linq;
using System.Reflection.Metadata;

namespace WillPittenger.Goodies.YtDlpWrapper;

public class Vid : Playable
{
	#region Constructors & Deconstructors
		public Vid(in JSON.Obj joInfo) : base(joInfo)
			=> mapAllKnownVidsByID[strID] = this;

		public Vid(in string strID) : base(strID)
			=> mapAllKnownVidsByID[strID] = this;

		public Vid(in System.Uri uriWhichObj) : base(uriWhichObj)
			=> mapAllKnownVidsByID[strID] = this;

		public Vid(in System.Text.Json.JsonElement jsonePlayListInfo) : base((JSON.Obj)JSON.ObjBase.Make(jsonePlayListInfo))
			=> mapAllKnownVidsByID[strID] = this;

		~Vid()
			=> mapAllKnownVidsByID.Remove(strID);
	#endregion

	#region Helper Types
		[System.ComponentModel.ImmutableObject(true)]
		public record Chapter
		(
			in System.TimeSpan Start,
			in string Title,
			in System.TimeSpan End
		);

		public record FmtDef
		{
			[System.ComponentModel.ImmutableObject(true)]
			public record Fragment
			(
				in System.TimeSpan Duration,
				in System.Uri? URL
			);

			[System.ComponentModel.ImmutableObject(true)]
			public record HttpHeader
			(
				in string UserAgent,
				in string Accept,
				in string AcceptLang,
				in string SecFetchMode
			);

			private static readonly System.Collections.Generic.SortedDictionary<string, FmtDef> mapAllFmtsByID = [];

			private FmtDef(in double? dblAvgBitRate, in string? strAudioCodec, in double? dblAspectRatio, in string? strAudioExt, in long? lColumns, in string? strExt,
				in long? lFileSizeApprox, in string? strName, in string strID, in string? strNote, in double? dblFramesPerSec, in System.Collections.Generic
				.IReadOnlyList<Fragment> listFragments, in long? lHeight, in System.Collections.Generic.IReadOnlyList<HttpHeader> listHttpHeaders, in string? strProtocol,
				in string? strResolution, in long? lRows, in double? dblTotalBitRate, in System.Uri? uri, in double? dblVidBitRate, in string? strVidCodec, in string?
				strVidExt, in long? lWidth)
			{
				mapAllFmtsByID[strID] = this;

				this.dblAvgBitRate = dblAvgBitRate;
				this.strAudioCodec = strAudioCodec;
				this.dblAspectRatio = dblAspectRatio;
				this.strAudioExt = strAudioExt;
				this.lColumns = lColumns;
				this.strExt = strExt;
				this.lFileSizeApprox = lFileSizeApprox;
				this.strName = strName;
				this.strID = strID;
				this.strNote = strNote;
				this.dblFramesPerSec = dblFramesPerSec;
				this.listFragments = listFragments;
				this.lHeight = lHeight;
				this.listHttpHeaders = listHttpHeaders;
				this.strProtocol = strProtocol;
				this.strResolution = strResolution;
				this.lRows = lRows;
				this.dblTotalBitRate = dblTotalBitRate;
				this.uri = uri;
				this.dblVidBitRate = dblVidBitRate;
				this.strVidCodec = strVidCodec;
				this.strVidExt = strVidExt;
				this.lWidth = lWidth;
			}

			~FmtDef()
				=> mapAllFmtsByID.Remove(strID);

			public readonly double? dblAvgBitRate;
			public readonly string? strAudioCodec;
			public readonly double? dblAspectRatio;
			public readonly string? strAudioExt;
			public readonly long? lColumns;
			public readonly string? strExt;
			public readonly long? lFileSizeApprox;
			public readonly string? strName;
			public readonly string strID;
			public readonly string? strNote;
			public readonly double? dblFramesPerSec;
			public readonly System.Collections.Generic.IReadOnlyList<Fragment> listFragments;
			public readonly long? lHeight;
			public readonly System.Collections.Generic.IReadOnlyList<HttpHeader> listHttpHeaders;
			public readonly string? strProtocol;
			public readonly string? strResolution;
			public readonly long? lRows;
			public readonly double? dblTotalBitRate;
			public readonly System.Uri? uri;
			public readonly double? dblVidBitRate;
			public readonly string? strVidCodec;
			public readonly string? strVidExt;
			public readonly long? lWidth;

			public static FmtDef GetFmtFromJSON(in JSON.Obj joFmtInfo)
				=> joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldID.strName].val.objVal is string strFmtID
					? mapAllFmtsByID[strFmtID] ?? new
						(
							(double?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldAvgVidBitRate.strName].val.objVal,
							(string?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldFullName.strName].val.objVal,
							(double?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldAspectRatio.strName].val.objVal,
							(string?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldAudioExt.strName].val.objVal,
							(long?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldColumns.strName].val.objVal,
							(string?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldExt.strName].val.objVal,
							(long?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldFileSizeApprox.strName].val.objVal,
							(string?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldFullName.strName].val.objVal,
							(string?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldID.strName].val.objVal
								?? throw new System.InvalidOperationException("Format ID is either missing or invalid!"),
							(string?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldNote.strName].val.objVal,
							(double?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldFramesPerSec.strName].val.objVal,
							joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldDuration.strName].val.objVal is JSON.Array jaFragments
								? jaFragments.Elements.Cast<JSON.Obj>()
									.Where(joCur
										=> joCur != null)
									.Select(joCur
										=> new FmtDef.Fragment
											(
												joCur.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldDuration.strName].val.objVal is double dblDuration
													? new System.TimeSpan(0, 0, (int)dblDuration)
													: System.TimeSpan.Zero,
												joCur.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldURL.strName].val.objVal is string strURL
													? new(strURL)
													: null
											)
									).ToList()
								: [],
							(long?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldHeight.strName].val.objVal,
							joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldHttpHdrs.strName].val.objVal is JSON.Array jaHttpHeaders
								? jaHttpHeaders.Elements.Cast<JSON.Obj>().Where(joCur => joCur != null).Select(joCur => new FmtDef.HttpHeader
									(
										((string?)joCur.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.HttpHdrs.fieldUserAgent.strName].val.objVal) ?? "",
										((string?)joCur.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.HttpHdrs.fieldAccept.strName].val.objVal) ?? "",
										((string?)joCur.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.HttpHdrs.fieldAcceptLang.strName].val.objVal) ?? "",
										((string?)joCur.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.HttpHdrs.fieldSecFetchMode.strName].val.objVal) ?? ""
									)).ToList()
								: [],
							(string?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldProtocol.strName].val.objVal,
							(string?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldResolution.strName].val.objVal,
							(long?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldResolution.strName].val.objVal,
							(double?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldTotalBitRate.strName].val.objVal,
							joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldURL.strName].val.objVal is string strURL
								? new(strURL)
								: null,
							(double?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldAvgVidBitRate.strName].val.objVal,
							(string?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldVidCodec.strName].val.objVal,
							(string?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldVidExt.strName].val.objVal,
							(long?)joFmtInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldWidth.strName].val.objVal
						)
					: throw new System.InvalidOperationException($"Format is an invalid value: Can't find ID");
			}
	#endregion

	#region Members
		private static readonly System.Collections.Generic.SortedDictionary<string, Vid> mapAllKnownVidsByID = [];

		private readonly System.Collections.Generic.SortedDictionary<long, FmtDef> mapAllFmtsByID = [];

		private readonly System.Collections.Generic.SortedDictionary<long, FmtDef> mapAllRequestedFmtsByID = [];
	#endregion

	#region Properties
		protected override string ExpectedObjType
			=> @"video";

		protected override YtDlpWrapper.FieldDef ExpectedIdField
			=> YtDlpWrapper.KnownYtDlpFields.fieldID;

	protected override System.Uri UpdateURL
	{
		get
		{
			PlayList? plParent = ParentPlayList;

			return new(plParent is null
					? $"https://www.youtube.com/watch?v={strID}"
					: $"https://www.youtube.com/watch?v={strID}&list={plParent.strID}"
				);
		}
	}

	public double? AudioBitRate
		{
			get;

			private set;
		}

		public string? AudioCodec
		{
			get;

			private set;
		}

		public long? AgeLimit
		{
			get;

			private set;
		}

		public double? AspectRatio
		{
			get;

			private set;
		}

		public long? AvgSampleRate
		{
			get;

			private set;
		}

		public long? AudioChannelCnt
		{
			get;

			private set;
		}

		public object? AutomaticCaptions
		{
			get;

			private set;
		}

		public double? AvgRating
		{
			get;

			private set;
		}

		public System.Collections.Generic.IReadOnlySet<string> Cats
		{
			get;

			private set;
		}

		public System.Collections.Generic.IReadOnlyList<Chapter> Chapters
		{
			get;

			private set;
		}

		public long? CommentCnt
		{
			get;

			private set;
		}

		public System.TimeSpan? Duration
		{
			get;

			private set;
		}

		public string? DurationAsStr
		{
			get;

			private set;
		}

		public string? DynamicRange
		{
			get;

			private set;
		}

		public System.IO.FileInfo? FileLoc
		{
			get;

			private set;
		}

		public long? FileSize
		{
			get;

			private set;
		}

		public string? FmtToUse
		{
			get;

			private set;
		}

		public System.Collections.Generic.IReadOnlyDictionary<long, FmtDef> AllFmtsById
			=> mapAllFmtsByID;

		public string? FmtID
		{
			get;

			private set;
		}

		public string? FmtNote
		{
			get;

			private set;
		}

		public long? FramesPerSec
		{
			get;

			private set;
		}

		public string? FullTitle
		{
			get;

			private set;
		}

		public object? HeatMap
		{
			get;

			private set;
		}

		public ulong? Height
		{
			get;

			private set;
		}

		public bool? IsLive
		{
			get;

			private set;
		}

		public string? Lang
		{
			get;

			private set;
		}

		public long? LikeCnt
		{
			get;

			private set;
		}

		public string? LiveStatus
		{
			get;

			private set;
		}

		public bool? PlayableInEmbed
		{
			get;

			private set;
		}

		public PlayList? ParentPlayList
		{
			get;

			private set;
		}

		public ulong? IndexInPlaylist
		{
			get;

			private set;
		}

		public string? Protocol
		{
			get;

			private set;
		}

		public System.DateOnly? ReleasedOn
		{
			get;

			private set;
		}

		public long? ReleaseTimeStamp
		{
			get;

			private set;
		}

		public ushort? ReleaseYear
		{
			get;

			private set;
		}

		public System.Collections.Generic.IReadOnlyDictionary<long, FmtDef> RequestedFmtsByID
			=> mapAllRequestedFmtsByID;

		public object? RequestedSubtitles
		{
			get;

			private set;
		}

		public string? Resolution
		{
			get;

			private set;
		}

		public double? StretchedRatio
		{
			get;

			private set;
		}

		public object? Subs
		{
			get;

			private set;
		}

		public double? TotalBitRate
		{
			get;

			private set;
		}

		public long? TimeStamp
		{
			get;

			private set;
		}

		public double? VidBitRate
		{
			get;

			private set;
		}

		public string? VidCodec
		{
			get;

			private set;
		}

		public bool? WasLive
		{
			get;

			private set;
		}

		public ulong? Width
		{
			get;

			private set;
		}
	#endregion

	#region Methods
		public static Vid GetVidFromID(in string strIdToLookUp)
			=> mapAllKnownVidsByID[strIdToLookUp] ?? new(strIdToLookUp);

		protected override void UpdateFields(in JSON.ObjBase jobRootWithUpdatedInfo, in bool bUpdatePlayListsEntriesToo)
		{
			if(jobRootWithUpdatedInfo is not JSON.Obj joRootWithUpdatedInfo)
				return;


			AudioBitRate = (double?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldAudioBitRate.strName].val.objVal;

			AudioCodec = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Audio.fieldCodec.strName].val.objVal;

			AgeLimit = (long?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldAgeLimit.strName].val.objVal;

			AspectRatio = (double?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldAspectRatio.strName].val.objVal;

			AvgSampleRate = (long?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Audio.fieldSampleRate.strName].val.objVal;

			AudioChannelCnt = (long?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Audio.fieldChannelCnt.strName].val.objVal;

			AutomaticCaptions = joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldAutoCaptions.strName].val.objVal;

			AvgRating = (double?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldAvgRating.strName].val.objVal;

			if(joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldCatList.strName].val.objVal is JSON.Array jaCatList)
				Cats = new System.Collections.Generic.SortedSet<string>(jaCatList.Elements.Cast<JSON.Val>()
					.Where(jvCurCat
						=> jvCurCat.objVal is string)
					.Select(jvCurCat
						=> (string)(jvCurCat.objVal ?? ""))
				);

			if(joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldChapterList.strName].val.objVal is JSON.Array jaChapterList)
				Chapters = new System.Collections.Generic.List<Chapter>(jaChapterList.Elements.Cast<JSON.Obj>()
					.Where(joCurChapter
						=> joCurChapter != null)
					.Select(joCurChapter
						=> new Chapter
						(
							new(0, 0, (int)((long?)joCurChapter.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Chapter.fieldStartTime.strName].val.objVal ?? 0)),
							(string?)joCurChapter.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Chapter.fieldTitle.strName].val.objVal ?? "",
							new(0, 0, (int)((long?)joCurChapter.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Chapter.fieldEndTime.strName].val.objVal ?? 0))
						)
					)
				);
			
			CommentCnt = (long?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldCommentCnt.strName].val.objVal;

			Duration = joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldDuration.strName].val.objVal is long lDuration
				? new(0, 0, (int)lDuration)
				: null;

			DurationAsStr = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldDurationStr.strName].val.objVal;

			DynamicRange = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldDynamicRange.strName].val.objVal;

			FileLoc = joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldFileName.strName].val.objVal is string strFileName
				? new(strFileName)
				: null;

			FileSize = (long?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldFileSize.strName].val.objVal;

			FmtToUse = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldFmt.strName].val.objVal;

			mapAllFmtsByID.Clear();
			if(joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldFmtList.strName].val.objVal is JSON.Array jaFmtList)
				foreach(JSON.Obj joCurFmt in jaFmtList.Cast<JSON.Obj>())
					if(joCurFmt.Values[YtDlpWrapper.KnownYtDlpFields.Vid.Fmt.fieldID.strName].val.objVal is long lFmtId)
						mapAllFmtsByID[lFmtId] = FmtDef.GetFmtFromJSON(joCurFmt);

			FmtID = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldFmtID.strName].val.objVal;

			FmtNote = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldFmtNote.strName].val.objVal;

			FramesPerSec = (long?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldFramesPerSec.strName].val.objVal;

			FullTitle = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldFullTitle.strName].val.objVal;

			HeatMap = joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldHeatMap.strName].val.objVal;

			Height = (ulong?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldHeight.strName].val.objVal;

			IsLive = (bool?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldIsLive.strName].val.objVal;

			Lang = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldLang.strName].val.objVal;

			LikeCnt = (long?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldLikeCnt.strName].val.objVal;

			LiveStatus = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldLiveStatus.strName].val.objVal;

			PlayableInEmbed = (bool?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldPlayableInEmbed.strName].val.objVal;

			// Never update the playlist entries as that might be lengthy.  User or owner of the new playlist can call Update if needed.
			ParentPlayList = joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldPlayList.strName].val.objVal is string strPlayListID
				? PlayList.GetPlayListFromID(strPlayListID)
				: null;

			IndexInPlaylist = (ulong?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldPlayListIndex.strName].val.objVal;

			Protocol = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldProtocol.strName].val.objVal;

			ReleasedOn = joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldReleaseDate.strName].val.objVal is string strReleaseOnDate
				? System.DateOnly.ParseExact(strReleaseOnDate, @"yyyyMMdd")
				: null;

			ReleaseTimeStamp = (long?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldReleaseTimeStamp.strName].val.objVal;

			ReleaseYear = (ushort?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldReleaseYear.strName].val.objVal;

			mapAllRequestedFmtsByID.Clear();
			if(joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldRequestedFmts.strName].val.objVal is JSON.Array jaRequestedFmts)
				foreach(JSON.Obj joCurRequestedFmt in jaRequestedFmts.Elements.Cast<JSON.Obj>())
					mapAllRequestedFmtsByID[((long?)joCurRequestedFmt.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldRequestedFmts.strName].val.objVal) ?? throw new
						System.InvalidOperationException("The format ID field doesn't seem to be a long for this format.")] = FmtDef
						.GetFmtFromJSON(joCurRequestedFmt);

			RequestedSubtitles = joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldRequestedSubtitles.strName].val.objVal;

			Resolution = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldResolution.strName].val.objVal;

			StretchedRatio = (double?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldStreachedRatio.strName].val.objVal;

			Subs = joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldSubtitles.strName].val.objVal;

			TotalBitRate = (double?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldTotalBitRate.strName].val.objVal;

			TimeStamp = (long?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldTimeStamp.strName].val.objVal;

			VidBitRate = (double?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldAvgVidBitRate.strName].val.objVal;

			VidCodec = (string?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldVidCodec.strName].val.objVal;

			WasLive = (bool?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldWasLive.strName].val.objVal;

			Width = (ulong?)joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields.Vid.fieldWidth.strName].val.objVal;
		}
	#endregion
}