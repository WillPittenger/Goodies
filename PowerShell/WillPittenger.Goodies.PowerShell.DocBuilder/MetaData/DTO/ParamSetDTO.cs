// Ignore Spelling: DTO aparams

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.DTO;

[System.Xml.Serialization.XmlType(TypeName = @"ParamSet", AnonymousType = false, IncludeInSchema = true)]
internal record ParamSetDTO : AbstractDTO
{
	public ParamSetDTO()
	{
	}

	public ParamSetDTO(in string strName, in ParamDetailsDTO[]? aparamsParamNamesInSet = null)
	{
		Name = strName;
		ParamNamesInSet = aparamsParamNamesInSet ?? [];
	}


	[System.Xml.Serialization.XmlAttribute]
	public string Name
	{
		get;

		set;
	} = string.Empty;

	[System.Xml.Serialization.XmlArray]
	public ParamDetailsDTO[] ParamNamesInSet
	{
		get;

		set;
	} = [];
}