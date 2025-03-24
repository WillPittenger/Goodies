// Ignore Spelling: Proj

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

[System.ComponentModel.ImmutableObject(true)]
public partial class SolutionDef
{
	private static readonly System.Text.RegularExpressions.Regex regexParser = ParserMaker();

	private static readonly System.Text.RegularExpressions.Regex regexLineSpliter = LineSplitterMaker();

	public SolutionDef(System.IO.FileInfo fileSolution)
	{
		SolutionFile = fileSolution;
		Name = System.IO.Path.GetFileNameWithoutExtension(fileSolution.Name);

		using System.IO.StreamReader reader = new(fileSolution.FullName);

		string[] lines = [.. regexLineSpliter.Split(reader.ReadToEnd()).Where(curLine => curLine.Trim() != string.Empty).Where(curLine => regexParser.IsMatch(curLine))];

		foreach(string strCurLine in lines)
		{
			System.Text.RegularExpressions.Match matchResult = regexParser.Match(strCurLine);

			ProjDef projCur = ProjDef.LookUpProject(new(System.IO.Path.Combine(fileSolution.DirectoryName
				?? throw new System.Exception(@"No path for solution"), matchResult.Groups[@"PathToProjFile"].Value)));

			allProjByGUID[projCur.GUID] = allProjByName[projCur.Name] = projCur;

			projCur.RegisterSolution(this);
		}
	}

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

	private readonly System.Collections.Generic.SortedDictionary<string, ProjDef> allProjByName = [];
	private readonly System.Collections.Generic.SortedDictionary<System.Guid, ProjDef> allProjByGUID = [];

	[System.Text.RegularExpressions.GeneratedRegex(@"^Project\(""\s*(?<PlatformGuid>{[A-Fa-f\d\-]+})""\)\s*=\s*""(?<Name>[A-Za-z\d\. ]+)""\s*,\s*""(?<PathToProjFile>[A-Za-z\\/\d\. ]+\.csproj)""\s*,\s*""(?<ProjectGuid>{[A-Fa-f\d\-]+})""")]
	private static partial System.Text.RegularExpressions.Regex ParserMaker();

	[System.Text.RegularExpressions.GeneratedRegex(@"(\r\n|\n\r|\r|\n)")]
	private static partial System.Text.RegularExpressions.Regex LineSplitterMaker();


	public System.Collections.Generic.IReadOnlyDictionary<string, ProjDef> AllProjByName
		=> allProjByName;

	public System.Collections.Generic.IReadOnlyDictionary<System.Guid, ProjDef> AllProjByGUID
		=> allProjByGUID;


	public string AllProjAsText
		=> string.Join(',', allProjByName.Keys);
}