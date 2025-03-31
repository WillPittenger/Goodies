// Ignore Spelling: DTO

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.DTO;

[System.Xml.Serialization.XmlType(TypeName = @"ParamDetails", AnonymousType = false, IncludeInSchema = true)]
internal record ParamDetailsDTO : AbstractDTO
{
	public ParamDetailsDTO()
	{
	}

	public ParamDetailsDTO(in string strAssociatedParamSetName, in bool bIsMandatory, in bool bIsValFromPipeline, in bool bIsValFromRemainingArgs, in string? strHelpText, in int? iPos = null)
	{
		AssociatedParamSetName = strAssociatedParamSetName;
		IsMandatory = bIsMandatory;
		IsValFromPipeline = bIsValFromPipeline;
		IsValFromRemainingArgs = bIsValFromRemainingArgs;
		HelpText = strHelpText;
		Pos = iPos;
	}


	[System.Xml.Serialization.XmlAttribute]
	public string AssociatedParamSetName
	{
		get;

		set;
	} = string.Empty;

	[System.Xml.Serialization.XmlAttribute]
	public bool IsMandatory
	{
		get;

		set;
	} = false;

	[System.Xml.Serialization.XmlAttribute]
	public bool IsValFromPipeline
	{
		get;

		set;
	} = false;

	[System.Xml.Serialization.XmlAttribute]
	public bool IsValFromRemainingArgs
	{
		get;

		set;
	} = false;

	public string? HelpText
	{
		get;

		set;
	} = null;

	[System.Xml.Serialization.XmlAttribute]
	public int? Pos
	{
		get;

		set;
	} = null;
}