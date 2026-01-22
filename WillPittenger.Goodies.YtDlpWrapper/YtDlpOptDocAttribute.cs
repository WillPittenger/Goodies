// Ignore Spelling: yt Dlp Rsrc astr

namespace WillPittenger.Goodies.YtDlpWrapper;

using Tools.Ext;

/// <summary>
/// Used to store metadata on a yt-dlp as implemented by <see cref="BaseVidCmdLet.AllGlobalOpts"/>.  Each option in there should have <see cref="YtDlpOptDocAttribute"/> applied.
/// </summary>
/// <remarks>
/// <see cref="YtDlpOptDocAttribute"/> tries to get the data from the Resources file (an RESX) as specified in the constructor.  If it can’t, it uses the default description.  Also stored are the parameters that might be emitted to yt-dlp.
/// </remarks>
[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
public class YtDlpOptDocAttribute : System.Attribute
{
	/// <summary>
	/// Constructs a new instance.
	/// </summary>
	/// <param name="strKey">The key in the specified RESX file</param>
	/// <param name="strDefaultDesc">A default description.  This will typically be the neutral language translation.</param>
	/// <param name="typeToUseToFindResources">Specify a type to use to locate the RESX.  It must be a type in the same assembly.</param>
	/// <param name="astrAssociatedParams">One or more parameters the option might emit to yt-dlp.</param>
	public YtDlpOptDocAttribute(string strKey, string strDefaultDesc, System.Type typeToUseToFindResources, params string[] astrAssociatedParams)
	{
		System.Resources.ResourceManager rm = new(typeToUseToFindResources);

		string strFoundVal = rm.GetString(strKey) ?? strDefaultDesc;

		Description = strFoundVal.IsEmpty() ? strDefaultDesc : strFoundVal;


		AssociatedParams = astrAssociatedParams ?? [];
	}


	/// <summary>
	/// The description of the option.
	/// </summary>
	public string Description
	{
		get;

		private init;
	}

	/// <summary>
	/// The parameters that might be emitted.
	/// </summary>
	public string[] AssociatedParams
	{
		get;

		private init;
	}
}