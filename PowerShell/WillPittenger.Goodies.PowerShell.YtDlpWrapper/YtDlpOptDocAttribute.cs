// Ignore Spelling: Dlp Rsrc astr

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

using Tools.Ext;

[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
public class YtDlpOptDocAttribute : System.Attribute
{
	public YtDlpOptDocAttribute(string strKey, string strDefaultDesc, System.Type typeToUseToFindResources, params string[] astrAssociateParams)
	{
		System.Resources.ResourceManager rm = new(typeToUseToFindResources);

		string strFoundVal = rm.GetString(strKey) ?? strDefaultDesc;

		strTranslatedVal = strDefaultDesc.IsEmpty() ? strDefaultDesc : strFoundVal;


		astrAssociatedParams = astrAssociatedParams ?? [];
	}


	public readonly string strTranslatedVal;

	public readonly string[] astrAssociatedParams;


	public string Description
		=> strTranslatedVal;

	public string[] AssociatedParams
		=> astrAssociatedParams;
}