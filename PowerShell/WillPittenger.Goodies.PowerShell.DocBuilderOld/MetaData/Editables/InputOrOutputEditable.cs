// Ignore Spelling: Editables

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.Editables;

public class InputOrOutputEditable : InputOrOutput
{
	internal InputOrOutputEditable(in InputOrOutput iooOrignal)
		: base(iooOrignal)
		=> this.iooOrignal = iooOrignal;


	public readonly InputOrOutput iooOrignal;


	public new string TypeName
	{
		get
			=> base.TypeName;

		set
		{
			if(base.TypeName != value)
			{
				base.TypeName = value;

				WereChangesMade = true;
			}
		}
	}

	public new string Comment
	{
		get
			=> base.Comment;

		set
		{
			if(base.Comment != value)
			{
				base.Comment = value;

				WereChangesMade = true;
			}
		}
	}

	public bool WereChangesMade
	{
		get;

		private set;
	} = false;


	public void SaveToOriginal()
		=> iooOrignal.SaveFromEditable(this);
}