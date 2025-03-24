namespace WillPittenger.Goodies.YtDlpWrapper;

public abstract class Playable : BaseObj
{
	#region Constructors & Deconstructors
		protected Playable(in JSON.Obj joInfo) : base(joInfo)
		{
		}

		protected Playable(in string strID) : base(strID)
		{
		}

		protected Playable(in System.Uri uriWhichObj) : base(uriWhichObj)
		{
		}
	#endregion

	#region Members
		public readonly Chan? chanParent = null;
	#endregion

	#region Properties
		public Chan? ChanParent
			=> chanParent;
	#endregion
}