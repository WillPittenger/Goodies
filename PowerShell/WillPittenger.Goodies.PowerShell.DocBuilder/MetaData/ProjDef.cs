// Ignore Spelling: Proj Guids

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

[System.ComponentModel.ImmutableObject(true)]
public class ProjDef
{
	public ProjDef(System.IO.FileInfo fileProj)
	{
		ProjFile = fileProj;

		System.Xml.XmlDocument doc = new();
		doc.Load(fileProj.FullName);

		System.Xml.XmlElement elemProject = doc[@"Project"] ?? throw new System.Exception(@"No project element");
		System.Xml.XmlElement elemPropGroup = elemProject[@"PropertyGroup"] ?? throw new System.Exception(@"No property group element");

		Name = elemPropGroup[@"AssemblyName"]?.InnerText ?? throw new System.Exception(@"Missing assembly name");
		GUID = new(elemPropGroup[@"ProjectGuid"]?.InnerText ?? throw new System.Exception(@"Missing project guid"));
		ProjectTypeGuids = from string strCurGuid in elemPropGroup[@"ProjectTypeGuids"]?.InnerText?.Split(';') ?? []
											 select new System.Guid(strCurGuid);


		allKnownProjectsByID[GUID] = allKnownProjectsByPath[fileProj.FullName] = this;
	}

	public System.IO.FileInfo ProjFile
	{
		get;

		private set;
	}

	public string Name
	{
		get;

		private set;
	}

	public System.Guid GUID
	{
		get;

		private set;
	}

	public System.Collections.Generic.IEnumerable<System.Guid> ProjectTypeGuids
	{
		get;

		private set;
	}

	private readonly System.Collections.Generic.LinkedList<SolutionDef> allRegisteredSolutions = [];


	public static readonly System.Collections.Generic.SortedDictionary<System.Guid, ProjDef> allKnownProjectsByID = [];
	public static readonly System.Collections.Generic.SortedDictionary<string, ProjDef> allKnownProjectsByPath = [];


	public System.Collections.Generic.IReadOnlyCollection<SolutionDef> AllRegisteredSolutions
		=> allRegisteredSolutions;


	public static ProjDef LookUpProject(System.IO.FileInfo fileProj)
		=> allKnownProjectsByPath[fileProj.FullName] ?? new(fileProj);


	public void RegisterSolution(SolutionDef sln)
		=> allRegisteredSolutions.AddLast(sln);

	public string AllRegisteredSolutionsAsText
		=> string.Join(',', allRegisteredSolutions.Select(slnCur => slnCur.Name));
}