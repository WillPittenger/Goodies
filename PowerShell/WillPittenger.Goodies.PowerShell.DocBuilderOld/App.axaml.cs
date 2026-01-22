// Ignore Spelling: evt

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.DocBuilder;

public partial class App : Avalonia.Application
{
	public override void Initialize()
	{
		string[] args = System.Environment.GetCommandLineArgs();

		if(args.Length > 2 && System.IO.File.Exists(args[1]))
			Solution = new(args[1]);

		if(fileSolution is null)
		{
			QueryWhatSlnDlg dlg = new();

			dlg.Show();
			while(!dlg.IsReadyToClose)
				System.Threading.Thread.Sleep(100);

			if(dlg.SelSln is null)
				System.Environment.Exit(-1);
			else
				Solution = dlg.SelSln;

			dlg.Close();
		}

		foreach(string strCurSln in System.IO.File.ReadLines(filePrevSlnFiles.FullName))
			mapPrevFilesByName[strCurSln] = new(strCurSln);

		if(fileSolution is not null)
			mapPrevFilesByName[fileSolution.FullName] = fileSolution;


		Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);


		System.IO.File.WriteAllLinesAsync(filePrevSlnFiles.FullName, mapPrevFilesByName.Values.Select(fileCurSln => fileCurSln.FullName));
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if(ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
			desktop.MainWindow = new MainWnd();

		base.OnFrameworkInitializationCompleted();
	}


	public delegate void DNewSolutionLoaded(in System.IO.FileInfo fileSolution, in MetaData.SolutionDef slnNew);


	public enum Modes
	{
		full,
		importedRequest,
	}

	private Modes mode = Modes.full;


	public event DNewSolutionLoaded? evtNewSolutionLoaded;


	public static readonly System.IO.DirectoryInfo dirOurSettingsFolder = new(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Will Pittenger", "Goodies", "PowerShell", "DocBuilder"));

	public static readonly System.IO.FileInfo filePrevSlnFiles = new(System.IO.Path.Combine(dirOurSettingsFolder.FullName, "prevSln.json"));


	private System.IO.FileInfo? fileSolution = null;

	private MetaData.SolutionDef? sln = null;

	private System.Collections.Generic.SortedDictionary<string, System.IO.FileInfo> mapPrevFilesByName = [];


	public System.IO.FileInfo? Solution
	{
		get
			=> fileSolution;

		set
		{
			fileSolution = value;

			if(fileSolution is not null && fileSolution.Exists)
				sln = new(fileSolution);

			if(fileSolution is not null)
				if(fileSolution.Extension.Equals(@".sln", System.StringComparison.CurrentCultureIgnoreCase))
					mode = Modes.full;
				else if(fileSolution.Extension.Equals(@".xml", System.StringComparison.CurrentCultureIgnoreCase))
					mode = Modes.importedRequest;
		}
	}

	public static App Instance
		=> (App)(Current ?? throw new System.InvalidProgramException(@"No application instance"));

	public MetaData.SolutionDef? Sln
		=> sln;

	public System.Collections.Generic.IReadOnlyDictionary<string, System.IO.FileInfo> PrevSlnFilesByName
		=> mapPrevFilesByName;
}