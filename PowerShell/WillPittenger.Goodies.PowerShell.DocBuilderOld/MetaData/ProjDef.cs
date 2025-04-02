// Ignore Spelling: Proj Guids maml dev utf xmlns mshelp nnwp sln

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

[System.ComponentModel.ImmutableObject(true)]
public class ProjDef
{
	public ProjDef(System.IO.FileInfo fileProj)
	{
		string strProjDir = fileProj?.Directory?.FullName ?? throw new System.InvalidOperationException(@"How did we get a file without a parent folder?");


		ProjFile = fileProj;
		DocsDir = new(System.IO.Path.Combine(strProjDir, @".docs"));
		DocsMdDir = new(System.IO.Path.Combine(strProjDir, @".markdown"));
		DocsOutDir = new(System.IO.Path.Combine(DocsMdDir.FullName, @".out"));

		System.Xml.XmlDocument doc = new();
		doc.Load(fileProj.FullName);

		System.Xml.XmlElement elemProject = doc[@"Project"] ?? throw new System.Exception(@"No project element");
		System.Xml.XmlElement elemPropGroup = elemProject[@"PropertyGroup"] ?? throw new System.Exception(@"No property group element");

		Name = elemPropGroup[@"AssemblyName"]?.InnerText ?? System.IO.Path.GetFileNameWithoutExtension(fileProj.Name);
		GUID = elemPropGroup[@"ProjectGuid"]?.InnerText is string strGuidText
			? new(strGuidText)
			: null;
		ProjectTypeGuids = from string strCurGuid in elemPropGroup[@"ProjectTypeGuids"]?.InnerText?.Split(';') ?? []
											 select new System.Guid(strCurGuid);


		if(GUID is System.Guid guid)
			allKnownProjectsByID[guid] = allKnownProjectsByPath[fileProj.FullName] = this;


		IsCompiled = System.IO.Directory.GetDirectories(System.IO.Path.Combine(fileProj?.Directory?.FullName ?? throw new System.InvalidOperationException(@"How did we get a file without a parent directory"), @"bin"), $@"{Name}.dll", System.IO.SearchOption.AllDirectories).Length > 0;

		IsCompiledButWithoutHelp = IsCompiled && !DocsDir.Exists;


		fswBinCtnts = new(System.IO.Path.Combine(ProjFile.Directory.FullName, @"bin"), ModuleName)
		{
			EnableRaisingEvents = true,
			IncludeSubdirectories = true,
			NotifyFilter = System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.Size,
		};
		fswBinCtnts.Changed += OnBinOutPutFileChanged;
		fswBinCtnts.Created += OnBinOutPutFileCreated;
		fswBinCtnts.Deleted += OnBinOutPutFileDeleted;


		foreach(System.IO.FileInfo fileCurBinary in ProjFile.Directory.GetFiles(ModuleName, System.IO.SearchOption.AllDirectories))
			AllDetectedBinaries[fileCurBinary.FullName] = fileCurBinary;

		InitDoc();
	}

	static ProjDef()
	{
		xnt = new();
		xnm = new(xnt);

		foreach(Const.NameSpaceDef namespaceCur in Const.NameSpaceDef.AllNameSpaces)
			xnm.AddNamespace(namespaceCur.strPrefix, namespaceCur.uri.AbsolutePath);
	}


	private static readonly System.Xml.XmlNamespaceManager xnm;
	private static readonly System.Xml.NameTable xnt;

	internal static class Const
	{
		[System.ComponentModel.ImmutableObject(true)]
		public class NameSpaceDef
		{
			public NameSpaceDef(in string strPrefix, in System.Uri uri)
			{
				llistAllNameSpaces.AddLast(this);

				this.strPrefix = strPrefix;
				this.uri = uri;
			}


			public readonly string strPrefix;

			public readonly System.Uri uri;


			private static readonly System.Collections.Generic.LinkedList<NameSpaceDef> llistAllNameSpaces = [];


			public static System.Collections.Generic.IReadOnlyCollection<NameSpaceDef> AllNameSpaces
				=> llistAllNameSpaces;
		}

		internal readonly struct NodeNameWithPrefix(string Name, ProjDef.Const.NameSpaceDef NameSpace)
		{
			public string Name
				=> Name;

			public NameSpaceDef NameSpace
				=> NameSpace;
		}

		public static class NameSpaces
		{
			public static readonly NameSpaceDef @default = new(string.Empty, new(@"http://msh"));

			public static readonly NameSpaceDef maml = new(@"maml", new(@"http://schemas.microsoft.com/maml/2004/10"));
			public static readonly NameSpaceDef cmd = new(@"command", new(@"http://schemas.microsoft.com/maml/dev/command/2004/10"));
			public static readonly NameSpaceDef dev = new(@"dev", new(@"http://schemas.microsoft.com/maml/dev/2004/10"));
			public static readonly NameSpaceDef mshelp = new(@"MSHelp", new(@"http://msdn.microsoft.com/mshelp"));
		}

		public static class Elements
		{
			public static readonly NodeNameWithPrefix nnwpRoot = new(@"helpItems", NameSpaces.@default);


			public static readonly NodeNameWithPrefix nnwpPara = new(@"para", NameSpaces.maml);
		}

		public static class AttrNames
		{
			public static readonly NodeNameWithPrefix nnwpSchema = new(@"schema", NameSpaces.@default);

			public static readonly NodeNameWithPrefix nnwpXMLNS = new(@"xmlns", NameSpaces.@default);


			public const string strXMLNS = @"xmlns";


			public static string MakeXMLNS(in NameSpaceDef ns)
				=> ns.strPrefix == string.Empty
					? throw new System.InvalidOperationException(@"Don't use MakeXMLNS with the default namespace")
					: $@"{strXMLNS}:{ns}";
		}
	}


	private readonly System.IO.FileSystemWatcher fswBinCtnts;

	private readonly System.Collections.Generic.LinkedList<SolutionDef> allRegisteredSolutions = [];

	private readonly System.Collections.Generic.SortedDictionary<string, CmdLetDef> mapAllCmdLetsByName = [];

	private readonly System.Xml.XmlDocument docOurMAML = new(xnt);


	public static readonly System.Collections.Generic.SortedDictionary<System.Guid, ProjDef> allKnownProjectsByID = [];
	public static readonly System.Collections.Generic.SortedDictionary<string, ProjDef> allKnownProjectsByPath = [];


	public System.IO.FileInfo ProjFile
	{
		get;

		private set;
	}

	public System.IO.DirectoryInfo DocsDir
	{
		get;

		private set;
	}

	public System.IO.DirectoryInfo DocsMdDir
	{
		get;

		private set;
	}

	public System.IO.DirectoryInfo DocsOutDir
	{
		get;

		private set;
	}

	internal System.Xml.XmlDocument OurMAML
		=> docOurMAML;

	private System.Collections.Generic.SortedList<string, System.IO.FileInfo> AllDetectedBinaries
	{
		get;
	} = [];

	public string ModuleName
		=> $@"{Name}.dll";

	public string RelativePathToProjFile
		=> System.IO.Path.GetRelativePath(App.Sln?.SolutionFile?.FullName ?? throw new System.InvalidProgramException("The app doesn't have a parent solution?"), ProjFile.FullName);

	public string Name
	{
		get;

		private set;
	}

	public System.Guid? GUID
	{
		get;

		private set;
	}

	public System.Collections.Generic.IEnumerable<System.Guid> ProjectTypeGuids
	{
		get;

		private set;
	}

	public bool NoCmdLetsFound
		=> mapAllCmdLetsByName.Count > 0;

	public bool IsCompiled
	{
		get;

		private init;
	}

	public string SearchStatus
		=> AllDetectedBinaries.Count == 0
			? Rsrcs.strNoBinaries
			: NoCmdLetsFound
				? Rsrcs.strNoCmdLets
				: Rsrcs.strHasCmdLets;

	public bool IsCompiledButWithoutHelp
	{
		get;

		private init;
	}

	public System.Collections.Generic.IReadOnlyList<CmdLetDef> CmdLetsWithHelp
		=> [..from CmdLetDef cmdletd in mapAllCmdLetsByName.Values
			 where cmdletd.JsonDocFile.Exists
			 select cmdletd];

	public bool HasUnpackgedHelpReady
		=> CmdLetsWithHelp.Count > 0;

	public bool HasPackagedHelpReady
		=> DocsDir.GetDirectories(@"*.cab", System.IO.SearchOption.AllDirectories).Length > 0 || DocsDir.GetDirectories(@"*.zip", System.IO.SearchOption.AllDirectories).Length > 0;

	public string HelpStatus
		=> IsCompiledButWithoutHelp
			? Rsrcs.strNoHelpCreated
			: HasUnpackgedHelpReady
				? Rsrcs.strSomeUnpackagedHelpCreated
				: Rsrcs.strPackagedHelpReady;

	public bool NewBuildDetected
	{
		get;

		private set;
	} = false;

	public string AllRegisteredSolutionsAsText
		=> string.Join(',', allRegisteredSolutions.Select(slnCur => slnCur.Name));


	public System.Collections.Generic.IReadOnlyCollection<SolutionDef> AllRegisteredSolutions
		=> allRegisteredSolutions;

	public System.Collections.Generic.IReadOnlyDictionary<string, CmdLetDef> AllCmdLetsByName
		=> mapAllCmdLetsByName;


	public static ProjDef LookUpProject(System.IO.FileInfo fileProj)
		=> allKnownProjectsByPath.ContainsKey(fileProj.FullName)
			? allKnownProjectsByPath[fileProj.FullName]
			: new(fileProj);


	public void RegisterSolution(SolutionDef sln)
		=> allRegisteredSolutions.AddLast(sln);

	private void InitDoc()
	{
		docOurMAML.PrependChild(docOurMAML.CreateXmlDeclaration(@"1.0", @"utf-8", null));

		System.Xml.XmlElement xeRoot = docOurMAML.CreateElement(Const.Elements.nnwpRoot.Name);
		docOurMAML.AppendChild(xeRoot);
		xeRoot.SetAttribute(Const.AttrNames.nnwpSchema.Name, Const.AttrNames.nnwpSchema.NameSpace.uri.AbsolutePath);
		xeRoot.SetAttribute(Const.AttrNames.nnwpXMLNS.Name, Const.NameSpaces.@default.uri.AbsolutePath);
	}


	private void OnBinOutPutFileDeleted(object objSender, System.IO.FileSystemEventArgs e)
		=> AllDetectedBinaries.Remove(e.FullPath);
	private void OnBinOutPutFileCreated(object objSender, System.IO.FileSystemEventArgs e)
		=> AllDetectedBinaries[e.FullPath] = new(e.FullPath);
	private void OnBinOutPutFileChanged(object objSender, System.IO.FileSystemEventArgs e)
		=> NewBuildDetected = true;
}