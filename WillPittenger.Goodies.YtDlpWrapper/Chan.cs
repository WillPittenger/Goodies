// Ignore Spelling: jsone

using System.Linq;

namespace WillPittenger.Goodies.YtDlpWrapper;

public class Chan : BaseObj
{
	#region Constructors & Deconstructors
		public Chan(in string strChanID, in bool bLoadEntriesNow = false)
			: this(new System.Uri($"https://www.youtube.com/${strChanID}"), bLoadEntriesNow)
		{
		}

		public Chan(in System.Uri uriWhichChan, in bool bLoadEntriesNow = false)
			: this(YtDlpWrapper.InvokeYtDlpForJSON(bLoadEntriesNow, astrParams: uriWhichChan.AbsolutePath).RootOfData
				?? throw new System.InvalidOperationException("Failed to get data from yt-dlp"))
		{
		}

		public Chan(in System.Text.Json.JsonElement jsoneChanInfo) : this((JSON.Obj)JSON.ObjBase.Make(jsoneChanInfo))
		{
		}

		public Chan(in JSON.Obj joChanInfo) : base(joChanInfo)
		{
		}
	#endregion

	#region Members
		private readonly System.Collections.Generic.SortedDictionary<string, PlayList> mapPlaylistsById = [];
	#endregion

	#region Properties
		protected override string ExpectedObjType
			=> @"playlist";

		protected override YtDlpWrapper.FieldDef ExpectedIdField
			=> YtDlpWrapper.KnownYtDlpFields.fieldChanID;

		protected override System.Uri UpdateURL
			=> new($"https://www.youtube.com/${strID}");

		public string? Name
		{
			get;

			private set;
		}

		public System.Collections.Generic.IReadOnlyDictionary<string, PlayList> PlaylistsByID
		{
			get
			{
				if(mapPlaylistsById.Count == 0)
					Update(bUpdatePlayListsEntriesToo: true);

				return mapPlaylistsById;
			}
		}
	#endregion

	#region Methods
		protected override void UpdateFields(in JSON.ObjBase jobRootWithUpdatedInfo, in bool bUpdatePlayListsEntriesToo)
		{
			if(jobRootWithUpdatedInfo is not JSON.Obj joRootWithUpdate)
				throw new System.InvalidOperationException(@"Object from yt-dlp seems to be the wrong type of JSON object.  Expected an instance of JavaScript " +
					@"object.");

			Name = (string?)joRootWithUpdate.Values[YtDlpWrapper.KnownYtDlpFields.fieldChanName.strName].val.objVal;

			if(joRootWithUpdate.Values[YtDlpWrapper.KnownYtDlpFields.Playlists.fieldEntries.strName].val.objVal is JSON.Array jaEntries)
			{
				mapPlaylistsById.Clear();

				foreach(JSON.Val jvCurEntry in jaEntries.Cast<JSON.Val>())
					if(jvCurEntry.objVal is string strCurPlayListID)
						mapPlaylistsById[strCurPlayListID] = PlayList.GetPlayListFromID(strCurPlayListID);
			}
		}
	#endregion
}