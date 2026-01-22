namespace WillPittenger.Goodies.PowerShell.DocBuilder;

public partial class QueryWhatSlnDlg : Avalonia.Controls.Window
{
	public QueryWhatSlnDlg()
		=> InitializeComponent();


	private static Avalonia.Platform.Storage.FilePickerOpenOptions? fpoo;


	public System.IO.FileInfo? SelSln
	{
		get;

		private set;
	} = null;

	public bool IsReadyToClose
	{
		get;

		private set;
	} = false;


	private void OnBrowseForSlnClicked(object? objSender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		fpoo ??= new()
		{
			AllowMultiple = false,
			FileTypeFilter =
			[
				new(@"Visual Studio Solution Files")
				{
					Patterns =
					[
						@"*.sln",
					],
				},
				new(@"DocBuilder Translation Requests")
				{
					Patterns =
					[
						@"*.PowerShell DocBuilder Translation Request.XML",
					],
				},
			],
			SuggestedStartLocation = StorageProvider.TryGetFolderFromPathAsync(new(System.Environment.CurrentDirectory)).Result,
		};

		System.Collections.Generic.IReadOnlyList<Avalonia.Platform.Storage.IStorageFile> efileSolutionSelected = StorageProvider.OpenFilePickerAsync(fpoo).Result;

		if(efileSolutionSelected.Count > 0)
			SelSln = new(efileSolutionSelected[0].Path.AbsolutePath);
	}

	private void OnPrevFileClicked(object? objSender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		SelSln = e.Source as System.IO.FileInfo;

		IsReadyToClose = true;
	}

	private void OnExitAppClicked(object? objSender, Avalonia.Interactivity.RoutedEventArgs e)
		=> IsReadyToClose = true;
}