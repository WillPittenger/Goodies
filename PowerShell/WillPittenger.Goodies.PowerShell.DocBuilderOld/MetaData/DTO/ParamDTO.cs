// Ignore Spelling: DTO astr Vals

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.DTO;

[System.Xml.Serialization.XmlType(TypeName = @"Param", AnonymousType = false, IncludeInSchema = true)]
internal record ParamDTO : AbstractDTO
{
	public ParamDTO()
	{
	}

	public ParamDTO(in string strName, in string strHelpText, in System.Type typeParam, in bool bAreWildCardsSupported, in string[] astrAllAllowedVals, in string[]? astrAllAliases = null, in object? objDefVal = null, in bool bHasDefVal = false, in ParamDetailsDTO[]? details = null)
	{
		Name = strName;
		HelpText = strHelpText;
		ParamType = typeParam;
		AreWildCardsSupported = bAreWildCardsSupported;
		AllAllowedVals = astrAllAllowedVals;
		AllAliases = astrAllAliases ?? [];
		DefVal = objDefVal;
		HasDefVal = bHasDefVal;
		DefVal = details ?? [];
	}


	[System.Xml.Serialization.XmlAttribute]
	public string Name
	{
		get;
		
		set;
	} = string.Empty;

	[System.Xml.Serialization.XmlIgnore]
	public string HelpText
	{
		get;

		set;
	} = string.Empty;

	[System.Xml.Serialization.XmlAttribute]
	public System.Type ParamType
	{
		get;

		set;
	} = typeof(ParamDTO);

	[System.Xml.Serialization.XmlAttribute]
	public bool AreWildCardsSupported
	{
		get;

		set;
	} = false;

	[System.Xml.Serialization.XmlArray]
	public string[] AllAllowedVals
	{
		get;

		set;
	} = [];

	[System.Xml.Serialization.XmlArray(ElementName = @"Alias")]
	public string[] AllAliases
	{
		get;

		set;
	} = [];

	[System.Xml.Serialization.XmlAttribute]
	public object? DefVal
	{
		get;

		set;
	} = null;

	public bool HasDefVal
	{
		get;

		set;
	} = false;

	[System.Xml.Serialization.XmlArray]
	public ParamDetailsDTO[] Details
	{
		get;

		set;
	} = [];


	[System.Xml.Serialization.XmlElement(@"HelpText")]
	public System.Xml.XmlCDataSection HelpTextAsCData
	{
		get
			=> xmldocCDataCreator.CreateCDataSection(HelpText);

		set
			=> HelpText = value.Value ?? string.Empty;
	}
}