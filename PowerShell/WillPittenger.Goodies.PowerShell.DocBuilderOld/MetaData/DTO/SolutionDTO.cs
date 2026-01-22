namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.DTO;

[System.ComponentModel.ImmutableObject(true)]
internal record SolutionDTO
{
	public SolutionDTO()
	{
	}

	public SolutionDTO(in ModuleDTO[]? aModules = null, in string[]? astrRequestedLangs = null)
	{
		Modules = aModules ?? [];
		RequestedLangs = astrRequestedLangs ?? [];
	}


	public ModuleDTO[] Modules
	{
		get;

		private set;
	} = [];

	public string[] RequestedLangs
	{
		get;

		private set;
	} = [];
}