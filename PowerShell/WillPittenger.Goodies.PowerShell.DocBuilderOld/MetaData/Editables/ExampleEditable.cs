// Ignore Spelling: Editables

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.Editables;

public class ExampleEditable : Example
{
	internal ExampleEditable(in Example exampleOriginal)
		: base(exampleOriginal)
		=> this.exampleOriginal = exampleOriginal;


	public readonly Example exampleOriginal;


	public new string Title
	{
		get
			=> base.Title;

		set
		{
			if(base.Title != value)
			{
				base.Title = value;

				WereChangesMade = true;
			}
		}
	}

	public new string Remarks
	{
		get
			=> base.Remarks;

		set
		{
			if(base.Remarks != value)
			{
				base.Remarks = value;

				WereChangesMade = true;
			}
		}
	}

	public new string Code
	{
		get
			=> base.Code;

		set
		{
			if(base.Code != value)
			{
				base.Code = value;

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
		=> exampleOriginal.SaveFromEditable(this);
}