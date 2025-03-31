// Ignore Spelling: evt

namespace WillPittenger.Goodies.PowerShell.DocBuilder;

public partial class App : Avalonia.Application
{
	public override void Initialize()
		=> Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);

	public override void OnFrameworkInitializationCompleted()
	{
		if(ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
			desktop.MainWindow = new MainWnd();

		base.OnFrameworkInitializationCompleted();
	}


	public delegate void DNewSolutionLoaded(in System.IO.FileInfo fileSolution, in MetaData.SolutionDef slnNew);


	public static event DNewSolutionLoaded? evtNewSolutionLoaded;


	private static System.IO.FileInfo? fileSolution = null;

	private static MetaData.SolutionDef? sln = null;


	public static System.IO.FileInfo? Solution
	{
		get
			=> fileSolution;

		set
		{
			fileSolution = value;

			if(fileSolution is not null && fileSolution.Exists)
				sln = new(fileSolution);
		}
	}

	public static MetaData.SolutionDef? Sln
		=> sln;


	private static void Main(in string[] args)
	{
		if(args.Length > 0 && System.IO.File.Exists(args[0]))
			Solution = new(args[0]);
	}
}