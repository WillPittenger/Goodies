// Ignore Spelling: DTO

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.DTO;

internal record InputOrOutputDTO : AbstractDTO
{
	public InputOrOutputDTO()
	{
	}

	public InputOrOutputDTO(in string strTypeName, in string strComment, in bool bIsFromAttr)
	{
		TypeName = strTypeName;
		Comment = strComment;
		IsFromAttr = bIsFromAttr;
	}


	[System.Xml.Serialization.XmlAttribute]
	public string TypeName
	{
		get;

		set;
	} = string.Empty;

	[System.Xml.Serialization.XmlIgnore]
	public string Comment
	{
		get;

		set;
	} = string.Empty;

	public bool IsFromAttr
	{
		get;

		set;
	} = false;


	[System.Xml.Serialization.XmlElement("Comment")]
	public System.Xml.XmlCDataSection CommentAsCData
	{
		get
			=> xmldocCDataCreator.CreateCDataSection(Comment);

		set
			=> Comment = value.Value ?? string.Empty;
	}
}