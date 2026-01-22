namespace WillPittenger.Goodies.PowerShell.DocBuilder;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
	: System.Windows.Application
{
	public delegate void DNewSolutionLoaded(in System.IO.FileInfo fileSolution, in MetaData.SolutionDef slnNew);


	public static event DNewSolutionLoaded? evtNewSolutionLoaded;


	private static System.IO.FileInfo? fileSolution = null;

	private static MetaData.SolutionDef? sln = null;


	public static readonly System.Guid guidForApp = new(@"b862c9c3-5c28-4f12-afd6-cde56cc86f1d");


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



	/// <inheritdoc/>
	protected override void OnStartup(System.Windows.StartupEventArgs e)
	{
		base.OnStartup(e);

		if(e.Args.Length > 0 && System.IO.File.Exists(e.Args[0]))
			Solution = new(e.Args[0]);
	}
}