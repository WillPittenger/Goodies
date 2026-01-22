// Ignore Spelling: Proj

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

[System.ComponentModel.ImmutableObject(true)]
public partial class SolutionDef
	: System.Collections.Specialized.INotifyCollectionChanged
{
	public SolutionDef(System.IO.FileInfo fileSolution)
	{
		SolutionFile = fileSolution;
		Name = System.IO.Path.GetFileNameWithoutExtension(fileSolution.Name);
		if(fileSolution.Extension.Equals(@".sln", System.StringComparison.CurrentCultureIgnoreCase))
		{
			using System.IO.StreamReader reader = new(fileSolution.FullName);

			string[] lines =
			[
				.. regexLineSplitter.Split(reader.ReadToEnd())
					.Where(curLine => curLine.Trim() != string.Empty)
					.Where(curLine => regexParser.IsMatch(curLine)),
			];

			foreach(string strCurLine in lines)
			{
				System.Text.RegularExpressions.Match matchResult = regexParser.Match(strCurLine);

				ModuleDef xeModuleCur = ModuleDef.LookUpProject(
					new(
						System.IO.Path.Combine(
							fileSolution.DirectoryName
							?? throw new System.Exception(@"No path for solution"), matchResult.Groups[@"PathToProjFile"].Value)));

				if(xeModuleCur.GUID is System.Guid guid)
					allModulesByGUID[guid] = xeModuleCur;
				allModulesByName[xeModuleCur.Name] = xeModuleCur;

				xeModuleCur.RegisterSolution(this);


				CollectionChanged?.Invoke(this, new(System.Collections.Specialized.NotifyCollectionChangedAction.Add, xeModuleCur));
			}
		}
		else if(fileSolution.Extension.Equals(@".xml", System.StringComparison.CurrentCultureIgnoreCase))
		{
			using System.Xml.XmlReader reader = System.Xml.XmlReader.Create(fileSolution.FullName);

			DTO.SolutionDTO dto = (DTO.SolutionDTO)(new System.Xml.Serialization.XmlSerializer(typeof(DTO.SolutionDTO)).Deserialize(reader) ?? throw new System.Exception(@"Unable to load translation request"));

			foreach(DTO.ModuleDTO dmoduleCur in dto.Modules)
				allModulesByGUID[dmoduleCur.GUID] = allModulesByName[dmoduleCur.Name] = new(dmoduleCur);

			RequestedLangs =
			[
				..dto.RequestedLangs.Select(strCurRequestedLang
					=> new System.Globalization.CultureInfo(strCurRequestedLang)),
			];
		}
	}


	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Stuck with name from an interface")]
	public event System.Collections.Specialized.NotifyCollectionChangedEventHandler? CollectionChanged;


	private static readonly System.Text.RegularExpressions.Regex regexParser = ParserMaker();

	private static readonly System.Text.RegularExpressions.Regex regexLineSplitter = LineSplitterMaker();


	public System.IO.FileInfo SolutionFile
	{
		get;

		private set;
	}

	public string Name
	{
		get;

		private set;
	}

	private readonly System.Collections.Generic.SortedDictionary<string, ModuleDef> allModulesByName = [];
	private readonly System.Collections.Generic.SortedDictionary<System.Guid, ModuleDef> allModulesByGUID = [];

	public System.Collections.Generic.IEnumerable<System.Globalization.CultureInfo> RequestedLangs
	{
		get;

		private set;
	} = [];


	[System.Text.RegularExpressions.GeneratedRegex(@"^Project\(""\s*(?<PlatformGuid>{[a-fA-F\d\-]+})""\)\s*=\s*""(?<Name>[a-zA-Z\d\. ]+)""\s*,\s*""(?<PathToProjFile>[a-zA-Z\\/\d\. ]+\.csproj)""\s*,\s*""(?<ProjectGuid>{[A-Fa-f\d\-]+})""")]
	private static partial System.Text.RegularExpressions.Regex ParserMaker();

	[System.Text.RegularExpressions.GeneratedRegex(@"(\r\n|\n\r|\r|\n)")]
	private static partial System.Text.RegularExpressions.Regex LineSplitterMaker();


	public System.Collections.Generic.IReadOnlyDictionary<string, ModuleDef> AllModulesByName
		=> allModulesByName;

	public System.Collections.Generic.IReadOnlyDictionary<System.Guid, ModuleDef> AllModulesByGuid
		=> allModulesByGUID;


	public string AllProjAsText
		=> string.Join(',', allModulesByName.Keys);

	internal void SaveRequestFile(
		System.Collections.Generic.IEnumerable<System.Globalization.CultureInfo> culturesToRequest,
		System.IO.FileInfo fileSaveTo)
	{
		DTO.SolutionDTO dto = new(
			[
				..allModulesByGUID.Values.Select(clCur
					=> clCur.ToDTO()),
			],
			[
				..culturesToRequest.Select(cultureCur
					=> cultureCur.Name),
			]
		);

		using System.IO.Stream wstream = new System.IO.FileStream(
			fileSaveTo.FullName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read);

		new System.Xml.Serialization.XmlSerializer(typeof(DTO.ModuleDTO)).Serialize(wstream, dto);
	}
}