// Ignore Spelling: jsone yt dlp

using System.Linq;

namespace WillPittenger.Goodies.YtDlpWrapper;

public class Chan : BaseObj
{
	#region Constructors & Deconstructors
		public Chan(in string strChanID, in bool bLoadEntriesNow = false)
			: this(new System.Uri($"https://www.youtube.com/${strChanID}"), bLoadEntriesNow)
			=> mapAllKnownChan[strChanID] = this;

		public Chan(in System.Uri uriWhichChan, in bool bLoadEntriesNow = false)
			: this(YtDlpWrapper.InvokeYtDlpForJSON(bLoadEntriesNow, null, astrParams: uriWhichChan.AbsolutePath).RootOfData
				?? throw new System.InvalidOperationException("Failed to get data from yt-dlp"))
			=> mapAllKnownChan[strID] = this;

		public Chan(in System.Text.Json.JsonElement jsoneChanInfo) : this((JSON.Obj)JSON.ObjBase.Make(jsoneChanInfo))
			=> mapAllKnownChan[strID] = this;

		public Chan(in JSON.Obj joChanInfo) : base(joChanInfo)
			=> mapAllKnownChan[strID] = this;

		~Chan()
			=> mapAllKnownChan.Remove(strID);
	#endregion

	#region Members
		private static readonly System.Collections.Generic.SortedDictionary<string, Chan> mapAllKnownChan = [];

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

		public static System.Collections.Generic.IReadOnlyDictionary<string, Chan> AllKnownChan
			=> mapAllKnownChan;
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