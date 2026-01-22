namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.DTO;

[System.ComponentModel.ImmutableObject(true)]
[System.Xml.Serialization.XmlRoot(@"Module")]
internal record ModuleDTO
{
	public ModuleDTO()
	{
	}

	public ModuleDTO(in string strName, in System.Guid guid, in CmdLetDTO[]? aCmdLets = null)
	{
		Name = strName;
		GUID = guid;
		CmdLets = aCmdLets ?? [];
	}


	public string Name
	{
		get;

		private set;
	} = string.Empty;

	public System.Guid GUID
	{
		get;

		private set;
	}

	public CmdLetDTO[] CmdLets
	{
		get;

		private set;
	} = [];
}