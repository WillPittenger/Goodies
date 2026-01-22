namespace WillPittenger.Goodies.PowerShell.DocBuilder;

public partial class TextEditorWithToolBarCtrl : Avalonia.Controls.UserControl
{
	public TextEditorWithToolBarCtrl()
		=> InitializeComponent();


	public static readonly Avalonia.DirectProperty<TextEditorWithToolBarCtrl, string> ValProperty =
		Avalonia.DirectProperty<TextEditorWithToolBarCtrl, string>.RegisterDirect<TextEditorWithToolBarCtrl, string>
		(
			nameof(Val),
			(ctrl) => ctrl.Val,
			(ctrl, valNew) => ctrl.Val = valNew
		);


	public string Val
	{
		get;

		set;
	} = string.Empty;
}