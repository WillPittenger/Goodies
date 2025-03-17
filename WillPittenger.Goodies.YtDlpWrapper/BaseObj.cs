namespace WillPittenger.Goodies.YtDlpWrapper;

public abstract class BaseObj
{
	#region Constructors & Deconstructors
		public BaseObj(in JSON.Obj joInfo)
		{
			strID = (string?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldID.strName].val.objVal ?? "";

			Update(joInfo);
		}

		public BaseObj(in string strID)
			=> this.strID = strID;

		public BaseObj(in System.Uri uriWhichObj)
			=> strID = System.Web.HttpUtility.ParseQueryString(uriWhichObj.Query).AllKeys['v'] ?? "";
	#endregion

	#region Members
		public readonly string strID;
	#endregion

	#region Properties
		protected abstract string ExpectedObjType
		{
			get;
		}

		protected abstract YtDlpWrapper.FieldDef ExpectedIdField
		{
			get;
		}

		protected abstract System.Uri UpdateURL
		{
			get;
		}

		public string? Desc
		{
			get;

			private set;
		}

		public string? Title
		{
			get;

			private set;
		}

		public System.Uri? PublishedTo
		{
			get;

			private set;
		}

		public long? FollowerCnt
		{
			get;

			private set;
		}

		public long? Epoch
		{
			get;

			private set;
		}

		public string? Extractor
		{
			get;

			private set;
		}

		public string? ExtractorKey
		{
			get;

			private set;
		}

		public System.DateOnly? ModifiedDate
		{
			get;

			private set;
		} = null;

		public System.Uri OriginalURL
		{
			get;

			private set;
		}

		public System.Collections.Generic.IReadOnlySet<string>? Tags
		{
			get;

			private set;
		}

		public string? UploaderName
		{
			get;

			private set;
		}

		public string? UploaderID
		{
			get;

			private set;
		}

		public System.Uri? UploaderURL
		{
			get;

			private set;
		}

		public long? ViewCnt
		{
			get;

			private set;
		}

		public System.Uri? WebPageURL
		{
			get;

			private set;
		}

		public string? WebPageBaseName
		{
			get;

			private set;
		}

		public string? WebPageDomain
		{
			get;

			private set;
		}
	#endregion

	#region Methods
		public void Update(JSON.Obj? joInfo = null, in bool bUpdatePlayListsEntriesToo = false)
		{
			joInfo ??= (JSON.Obj)JSON.ObjBase.Make(YtDlpWrapper.InvokeYtDlpForJSON(bUpdatePlayListsEntriesToo, astrParams: UpdateURL.AbsoluteUri).RootOfData
				?? throw new System.InvalidOperationException("Failed to get JSON data from yt-dlp"));

			if(joInfo.Values[ExpectedIdField.strName].val.objVal is string strFoundId && strFoundId != strID)
				throw new System.InvalidOperationException($"The object “{strFoundId}” doesn't match the expected “{strID}”.  Unable to update “{strID}”!");
			if(joInfo.Values[YtDlpWrapper.KnownYtDlpFields.field_ItemType.strName].val.objVal is string strItemType && strItemType != ExpectedObjType)
				throw new System.InvalidOperationException($"Unable to update “{strID}” as the data available isn't for a {ExpectedObjType}");


			Desc = (string?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldDesc.strName].val.objVal;

			Title = (string?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldTitle.strName].val.objVal;

			if(joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldChanURL.strName].val.objVal is string strChanUrl)
				PublishedTo = new(strChanUrl);

			FollowerCnt = (long?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldChanFollowerCnt.strName].val.objVal;

			Epoch = (long?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldEpoch.strName].val.objVal;

			Extractor = (string?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldExtractor.strName].val.objVal;

			ExtractorKey = (string?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldExtractorKey.strName].val.objVal;

			if(joInfo.Values[YtDlpWrapper.KnownYtDlpFields.Playlists.fieldModifiedDate.strName].val.objVal is string strModifiedDate)
				ModifiedDate = System.DateOnly.ParseExact(strModifiedDate, "yyyyMMdd");

			if(joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldOriginalURL.strName].val.objVal is string strOriginalURL)
				OriginalURL = new(strOriginalURL);

			System.Collections.Generic.SortedSet<string> setTags = [];
			if(joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldTags.strName].val.objVal is JSON.Array jaTags)
				foreach(JSON.ObjBase jobCur in jaTags)
					if(jobCur is JSON.Val jvCur && jvCur.objVal is string strCurTag)
						setTags.Add(strCurTag);
			Tags = setTags;

			UploaderName = (string?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldUploader.strName].val.objVal;

			UploaderID = (string?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldUploaderId.strName].val.objVal;

			if(joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldUploaderUrl.strName].val.objVal is string strUploaderURL)
				UploaderURL = new(strUploaderURL);

			ViewCnt = (long?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldViewCnt.strName].val.objVal;

			if(joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldWebPageUrl.strName].val.objVal is string strWebPageURL)
				WebPageURL = new(strWebPageURL);

			WebPageBaseName = (string?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldWebPageUrlBaseName.strName].val.objVal;

			WebPageDomain = (string?)joInfo.Values[YtDlpWrapper.KnownYtDlpFields.fieldWebPageUrlDomain.strName].val.objVal;

			UpdateFields(joInfo, bUpdatePlayListsEntriesToo);
		}

		protected abstract void UpdateFields(in JSON.ObjBase jobRootWithUpdatedInfo, in bool bUpdatePlayListsEntriesToo);
	#endregion
}