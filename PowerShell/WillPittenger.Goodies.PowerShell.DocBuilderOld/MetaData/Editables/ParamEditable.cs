// Ignore Spelling: Editables paramd

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData.Editables;

public class ParamEditable : ParamDef
{
	public ParamEditable(in ParamDef paramdOriginal)
		: base(paramdOriginal)
		=> this.paramdOriginal = paramdOriginal;


	public readonly ParamDef paramdOriginal;


	public new string HelpText
	{
		get
			=> base.HelpText;

		set
		{
			if(base.HelpText != value)
			{
				base.HelpText = value;

				WereChangesMade = true;
			}
		}
	}

	public bool WereChangesMade
	{
		get;

		private set;
	} = false;
}