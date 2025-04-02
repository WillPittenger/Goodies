// Ignore Spelling: evt proj psd powershell nnwp uri globbing Ret Nav ioo

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

public class InputOrOutput : Obj<InputOrOutput>
{
	public InputOrOutput(in CmdLetDef owner, in bool bIsInput, in string strTypeName = "", in string strComment = "")
	{
		this.owner = owner;

		this.bIsInput = bIsInput;

		this.strTypeName = strTypeName;
		this.strComment = strComment;

		InitMAML();
	}

	internal InputOrOutput(in CmdLetDef owner, in bool bIsInput, in DTO.InputOrOutputDTO dto)
	{
		this.owner = owner;

		this.bIsInput = bIsInput;

		strTypeName = dto.TypeName;
		strComment = dto.Comment;

		InitMAML();
	}

	protected InputOrOutput(in InputOrOutput iooWhatToCopy)
	{
		owner = iooWhatToCopy.owner;

		bIsInput = iooWhatToCopy.bIsInput;

		strTypeName = iooWhatToCopy.TypeName;
		strComment = iooWhatToCopy.Comment;

		InitMAML();
	}


	public event DFieldChanged<string>? evtTypeNameChanged;

	public event DFieldChanged<string>? evtCommentChanged;


	private static class Const
	{
		public static class Elements
		{
			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpInputType = new(@"inputType", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpRetVal = new(@"returnValue", ProjDef.Const.NameSpaces.cmd);


			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpDevType = new(@"type", ProjDef.Const.NameSpaces.dev);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpName = new(@"name", ProjDef.Const.NameSpaces.maml);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpDesc = new(@"description", ProjDef.Const.NameSpaces.maml);
		}
	}


	public readonly CmdLetDef owner;

	public readonly bool bIsInput;

	private string strTypeName;

	private string strComment;

	private System.Xml.XmlElement? xeUs;

	private System.Xml.XmlText? xtTypeName;

	private System.Xml.XmlText? xtComment;


	public bool IsValid
		=> strTypeName != string.Empty;

	public string TypeName
	{
		get
			=> strTypeName;

		protected set
		{
			if(strTypeName != value)
			{
				string strOldTypeName = strTypeName;

				strTypeName = value;

				MakeDirty();

				FireTypeNameChanged(strOldTypeName);

				if(xtTypeName is not null)
					xtTypeName.InnerText = value;
			}
		}
	}

	public string Comment
	{
		get
			=> strComment;

		protected set
		{
			if(strComment != value)
			{
				string strOldComment = strComment;

				strComment = value;

				MakeDirty();

				FireCommentChanged(strOldComment);

				if(xtComment is not null)
					xtComment.InnerText = value;
			}
		}
	}

	public System.Xml.XmlElement OurRoot
		=> xeUs ?? throw new System.InvalidProgramException(@"This InputOrOutput instance isn't properly initialized.");



	private void InitMAML()
	{
		xeUs = owner.CreateMamlElement(
			bIsInput
				? Const.Elements.nnwpInputType
				: Const.Elements.nnwpRetVal
		);

		{
			System.Xml.XmlElement xeDevType = owner.CreateMamlElement(Const.Elements.nnwpDevType);
			xeUs.AppendChild(xeDevType);

			{
				System.Xml.XmlElement xeName = owner.CreateMamlElement(Const.Elements.nnwpName);
				xeDevType.AppendChild(xeName);

				xtTypeName = owner.projParent.OurMAML.CreateTextNode(strTypeName);
				xeName.AppendChild(xtTypeName);
			}

			{
				System.Xml.XmlElement xeDesc = owner.CreateMamlElement(Const.Elements.nnwpDesc);
				xeUs.AppendChild(xeDesc);

				{
					System.Xml.XmlElement xePara = owner.CreateMamlElement(ProjDef.Const.Elements.nnwpPara);
					xeDesc.AppendChild(xePara);

					xtComment = owner.projParent.OurMAML.CreateTextNode(strComment);
					xePara.AppendChild(xtComment);
				}
			}
		}
	}

	private void FireTypeNameChanged(in string strOldTypeName)
	{
		FirePropChanged(nameof(TypeName));

		evtTypeNameChanged?.Invoke(this, strOldTypeName, strTypeName);
	}

	private void FireCommentChanged(in string strOldComment)
	{
		FirePropChanged(nameof(Comment));

		evtCommentChanged?.Invoke(this, strOldComment, strComment);
	}

	public Editables.InputOrOutputEditable MakeEditable()
		=> new(this);

	internal void SaveFromEditable(in Editables.InputOrOutputEditable iooeWhatToSave)
	{
		TypeName = iooeWhatToSave.TypeName;
		Comment = iooeWhatToSave.Comment;
	}
}