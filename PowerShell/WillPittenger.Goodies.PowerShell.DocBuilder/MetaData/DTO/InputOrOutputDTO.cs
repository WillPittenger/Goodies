// Ignore Spelling: DTO

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.DTO;

internal record InputOrOutputDTO : AbstractDTO
{
	public InputOrOutputDTO()
	{
	}

	public InputOrOutputDTO(in string strTypeName, in string strComment)
	{
		TypeName = strTypeName;
		Comment = strComment;
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


	[System.Xml.Serialization.XmlElement("Comment")]
	public System.Xml.XmlCDataSection CommentAsCData
	{
		get
			=> xmldocCDataCreator.CreateCDataSection(Comment);

		set
			=> Comment = value.Value ?? string.Empty;
	}
}