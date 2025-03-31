// Ignore Spelling: DTO Proj astr

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.DTO;

[System.Xml.Serialization.XmlRoot(ElementName = @"Cmdlet")]
internal record CmdLetDTO : AbstractDTO
{
	public CmdLetDTO()
	{
	}

	public CmdLetDTO(in System.Type typeForCmdLet, in string strVerb, in string strNoun, in System.Guid guid, in string strSynopsis, in string strDesc, in ParamDTO[]? allParams = null, in ParamSetDTO[]? allParamSets = null, in InputOrOutputDTO[]? inputs = null, in InputOrOutputDTO[]? outputs = null, in ExampleDTO[]? examples = null, in string[]? astrAllRelatedLinks = null)
	{
		TypeForCmdLet = typeForCmdLet;

		Verb = strVerb;
		Noun = strNoun;
		GUID = guid;
		Synopsis = strSynopsis;
		Desc = strDesc;
		AllParams = allParams ?? [];
		AllParamSets = allParamSets ?? [];
		Inputs = inputs ?? [];
		Outputs = outputs ?? [];
		Examples = examples ?? [];
		AllRelatedLinks = astrAllRelatedLinks ?? [];
	}


	[System.Xml.Serialization.XmlAttribute]
	public System.Type TypeForCmdLet
	{
		get;

		private set;
	} = typeof(System.IO.FileInfo);

	[System.Xml.Serialization.XmlAttribute]
	public string Verb
	{
		get;

		set;
	} = string.Empty;

	[System.Xml.Serialization.XmlAttribute]
	public string Noun
	{
		get;

		set;
	} = string.Empty;

	[System.Xml.Serialization.XmlAttribute]
	public System.Guid GUID
	{
		get;

		set;
	} = System.Guid.Empty;

	[System.Xml.Serialization.XmlIgnore]
	public string Synopsis
	{
		get;

		set;
	} = string.Empty;

	[System.Xml.Serialization.XmlIgnore]
	public string Desc
	{
		get;

		set;
	} = string.Empty;

	[System.Xml.Serialization.XmlArray]
	public ParamDTO[] AllParams
	{
		get;

		set;
	} = [];

	[System.Xml.Serialization.XmlArray]
	public ParamSetDTO[] AllParamSets
	{
		get;

		set;
	} = [];

	[System.Xml.Serialization.XmlArray(@"Input")]
	public InputOrOutputDTO[] Inputs
	{
		get;

		set;
	} = [];

	[System.Xml.Serialization.XmlArray(@"Output")]
	public InputOrOutputDTO[] Outputs
	{
		get;

		set;
	} = [];

	[System.Xml.Serialization.XmlArray]
	public ExampleDTO[] Examples
	{
		get;

		set;
	} = [];

	[System.Xml.Serialization.XmlArray]
	public string[] AllRelatedLinks
	{
		get;

		set;
	} = [];


	[System.Xml.Serialization.XmlElement(@"Synopsis")]
	public System.Xml.XmlCDataSection SynopsisAsCData
	{
		get
			=> xmldocCDataCreator.CreateCDataSection(Synopsis);

		set
			=> Synopsis = value.Value ?? string.Empty;
	}

	[System.Xml.Serialization.XmlElement(@"Desc")]
	public System.Xml.XmlCDataSection DescAsCData
	{
		get
			=> xmldocCDataCreator.CreateCDataSection(Desc);

		set
			=> Desc = value.Value ?? string.Empty;
	}
}