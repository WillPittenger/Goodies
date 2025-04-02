// Ignore Spelling: DTO

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.DTO;

[System.Xml.Serialization.XmlType(TypeName = @"Example", AnonymousType = false, IncludeInSchema = true)]
internal record ExampleDTO : AbstractDTO
{
	public ExampleDTO()
	{
	}

	public ExampleDTO(in string strTitle, in string? strRemarks = null, in string? strCode = null)
	{
		Title = strTitle;
		Remarks = strRemarks ?? string.Empty;
		Code = strCode ?? string.Empty;
	}


	[System.Xml.Serialization.XmlAttribute]
	public string Title
	{
		get;

		set;
	} = string.Empty;

	[System.Xml.Serialization.XmlIgnore]
	public string Remarks
	{
		get;

		set;
	} = string.Empty;

	[System.Xml.Serialization.XmlIgnore]
	public string Code
	{
		get;

		set;
	} = string.Empty;


	[System.Xml.Serialization.XmlElement(@"Desc")]
	public System.Xml.XmlCDataSection DescAsCData
	{
		get
			=> xmldocCDataCreator.CreateCDataSection(Remarks);

		set
			=> Remarks = value.Value ?? string.Empty;
	}

	[System.Xml.Serialization.XmlElement(@"Code")]
	public System.Xml.XmlCDataSection CodeAsCData
	{
		get
			=> xmldocCDataCreator.CreateCDataSection(Code);

		set
			=> Code = value.Value ?? string.Empty;
	}
}