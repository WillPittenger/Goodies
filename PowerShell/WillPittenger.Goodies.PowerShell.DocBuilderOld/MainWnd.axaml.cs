using System.Linq;

namespace WillPittenger.Goodies.PowerShell.DocBuilder;

using Tools.Ext;

using WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

public partial class MainWnd :Avalonia.Controls.Window
{
	public MainWnd()
	{
		App.Instance.evtNewSolutionLoaded += OnNewSolutionLoaded;

		InitializeComponent();

		UpdateTitleBar();
	}


	private static Avalonia.Platform.Storage.FilePickerOpenOptions? fpoo;


	protected override void OnInitialized()
	{
		base.OnInitialized();

		if(App.Instance.Solution is null)
		{
			fpoo ??= new()
			{
				AllowMultiple = false,
				FileTypeFilter = [
					new(@"Visual Studio Solution Files")
					{
						Patterns =
						[
							@"*.sln",
						],
					}
				],
				SuggestedStartLocation = StorageProvider.TryGetFolderFromPathAsync(new(System.Environment.CurrentDirectory)).Result,

			};

			System.Collections.Generic.IReadOnlyList<Avalonia.Platform.Storage.IStorageFile> efileSolutionSelected = StorageProvider.OpenFilePickerAsync(fpoo).Result;

			if(efileSolutionSelected.Count > 0)
				App.Instance.Solution = new(efileSolutionSelected[0].Path.AbsolutePath);
		}

		dgProjectsFound.ItemsSource = App.Instance.Sln?.AllModulesByName?.Values ?? throw new System.InvalidProgramException(@"How did we get here without a solution?");
	}

	private void UpdateTitleBar()
		=> Title = Rsrcs.strMainWndTitleFmt.Fmt(App.Instance.Solution?.FullName ?? string.Empty);


	private void OnNewSolutionLoaded(in System.IO.FileInfo arg1, in SolutionDef arg2)
		=> UpdateTitleBar();
}