// Ignore Spelling: jsone

using System.Linq;

namespace WillPittenger.Goodies.YtDlpWrapper;

public class PlayList : Playable
{
	#region Constructors & Deconstructors
		public PlayList(in string strPlayListID, in bool bLoadEntries) : base(strPlayListID)
		{
			mapAllKnownPlayListsByID[strPlayListID] = this;

			if(bLoadEntries)
				Update();
		}

		public PlayList(in System.Uri uriWhichPlayList, in bool bLoadEntries) : base(uriWhichPlayList)
		{
			mapAllKnownPlayListsByID[strID] = this;

			if(bLoadEntries)
				Update();
		}

		public PlayList(in System.Text.Json.JsonElement jsonePlayListInfo) : base((JSON.Obj)JSON.ObjBase.Make(jsonePlayListInfo))
			=> mapAllKnownPlayListsByID[strID] = this;

		public PlayList(in JSON.Obj joPlayListInfo) : base(joPlayListInfo)
			=> mapAllKnownPlayListsByID[strID] = this;

		~PlayList()
			=> mapAllKnownPlayListsByID.Remove(strID);
	#endregion

	#region Members
		private readonly System.Collections.Generic.SortedDictionary<string, Playable> mapEntriesByID = [];

		private System.Collections.Generic.List<Playable> listEntriesByIndex = [];

		private static readonly System.Collections.Generic.SortedDictionary<string, PlayList> mapAllKnownPlayListsByID = [];
	#endregion

	#region Properties
		protected override string ExpectedObjType
			=> @"playlist";

		protected override YtDlpWrapper.FieldDef ExpectedIdField
			=> YtDlpWrapper.KnownYtDlpFields.fieldID;

		protected override System.Uri UpdateURL
			=> new($"https://www.youtube.com/playlist?list={strID}");

		public System.Collections.Generic.IReadOnlyDictionary<string, Playable> EntriesByID
			=> mapEntriesByID;

		public System.Collections.Generic.IReadOnlyList<Playable> EntriesInOrder
			=> listEntriesByIndex;

		public static System.Collections.Generic.IReadOnlyDictionary<string, PlayList> AllKnownPlayLists
			=> mapAllKnownPlayListsByID;
	#endregion

	#region Methods
		public static PlayList GetPlayListFromID(in string strID, in bool bLoadEntries = false)
			=> mapAllKnownPlayListsByID[strID] ?? new(strID, bLoadEntries);

		protected override void UpdateFields(in JSON.ObjBase jobRootWithUpdatedInfo, in bool bUpdatePlayListsEntriesToo)
		{
			if(bUpdatePlayListsEntriesToo && jobRootWithUpdatedInfo is JSON.Obj joRootWithUpdatedInfo && joRootWithUpdatedInfo.Values[YtDlpWrapper.KnownYtDlpFields
				.Playlists.fieldEntries.strName].val.objVal is JSON.Array jaEntries)
			{
				mapEntriesByID.Clear();

				foreach(JSON.Val jvCurEntry in jaEntries.Cast<JSON.Val>())
					if(jvCurEntry.objVal is JSON.Obj joCurEntry)
						if(joCurEntry.Values[YtDlpWrapper.KnownYtDlpFields.field_ItemType.strName].val.objVal is string strItemType && joCurEntry.Values[YtDlpWrapper
								.KnownYtDlpFields.fieldID.strName].val.objVal is string strEntryID)
							mapEntriesByID[strEntryID] = strItemType switch
								{
									@"playlist"
										=> GetPlayListFromID(strID),
								};
			}
		}
	#endregion
}