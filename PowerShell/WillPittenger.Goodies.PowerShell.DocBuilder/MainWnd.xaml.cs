// ignore spelling: sln

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.DocBuilder;

using Tools.Ext;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWnd
	: System.Windows.Window
{
	public MainWnd()
	{
		App.evtNewSolutionLoaded += OnNewSolutionLoaded;

		InitializeComponent();

		UpdateTitleBar();
	}


	protected override void OnActivated(System.EventArgs e)
	{
		base.OnActivated(e);

		if(App.Solution is null)
		{
			Microsoft.Win32.OpenFileDialog dlg = new()
			{
				AddExtension = true,
				AddToRecent = true,
				CheckFileExists = true,
				ClientGuid = App.guidForApp,
				DefaultDirectory = System.Environment.CurrentDirectory,
				DefaultExt = @"sln",
				Filter = $@"{Rsrcs.strFileFilterDescForSlnFiles}|*.sln",
				FilterIndex = 0,
				InitialDirectory = System.Environment.CurrentDirectory,
				Multiselect = false,
				Title = Rsrcs.strSlnFileSelectorDlgTitle,
				ValidateNames = true,
			};

			if(dlg.ShowDialog(this) == true)
				App.Solution = new(dlg.FileName);

		}

		dgProjectsFound.ItemsSource = App.Sln?.AllProjByName?.Values ?? throw new System.InvalidProgramException(@"How did we get here without a solution?");
	}

	private void UpdateTitleBar()
		=> Title = Rsrcs.strMainWndTitleFmt.Fmt(App.Solution?.FullName ?? string.Empty);


	private void OnNewSolutionLoaded(in System.IO.FileInfo arg1, in MetaData.SolutionDef arg2)
		=> UpdateTitleBar();
}