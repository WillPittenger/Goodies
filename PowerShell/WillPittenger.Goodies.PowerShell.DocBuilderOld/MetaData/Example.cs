// Ignore Spelling: evt proj psd powershell nnwp uri globbing Ret Nav

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

using Tools.Ext;

public class Example : Obj<Example>
{
	public Example(in CmdLetDef cmdletParent, in string strTitle, in string strRemarks, in string strCode)
	{
		this.cmdletParent = cmdletParent;
		this.strTitle = strTitle;
		this.strRemarks = strRemarks;
		this.strCode = strCode;


		InitMAML();
	}

	internal Example(in CmdLetDef cmdletParent, in DTO.ExampleDTO dto)
	{
		this.cmdletParent = cmdletParent;
		strTitle = dto.Title;
		strRemarks = dto.Remarks;
		strCode = dto.Code;


		InitMAML();
	}

	protected Example(in Example exampleCopyThis)
	{
		cmdletParent = exampleCopyThis.cmdletParent;
		strTitle =exampleCopyThis.Title;
		strRemarks = exampleCopyThis.Remarks;
		strCode = exampleCopyThis.Code;


		InitMAML();
	}


	public event DFieldChanged<string>? evtTitleChanged;

	public event DFieldChanged<string>? evtRemarksChanged;

	public event DFieldChanged<string>? evtCodeChanged;


	internal static class Const
	{
		public static readonly string strTitleFmt = $@"{'-'.RepeatChar(26)} Example {{0}} {'-'.RepeatChar(26)}";


		public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpExample = new(@"example", ModuleDef.Const.NameSpaces.cmd);


		public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpTitle = new(@"title", ModuleDef.Const.NameSpaces.maml);

		public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpCode = new(@"code", ModuleDef.Const.NameSpaces.dev);

		public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpRemarks = new(@"remarks", ModuleDef.Const.NameSpaces.dev);
	}


	public readonly CmdLetDef cmdletParent;

	public string strTitle;

	private string strRemarks;

	private string strCode;

	private System.Xml.XmlElement? xeUs;

	private System.Xml.XmlText? xtTitle;

	private System.Xml.XmlText? xtCode;

	private System.Xml.XmlText? xtRemarks;


	public string Title
	{
		get
			=> strTitle;

		protected set
		{
			if(strTitle != value)
			{
				string strOldTitle = strTitle;

				strTitle = value;

				MakeDirty();

				FireTitleChanged(strOldTitle);

				if(xtTitle is not null)
					xtTitle.InnerText = value;
			}
		}
	}

	public string Remarks
	{
		get
			=> strRemarks;

		protected set
		{
			if(strRemarks != value)
			{
				string strOldDesc = strRemarks;

				strRemarks = value;

				MakeDirty();

				FireRemarksChanged(strOldDesc);

				if(xtRemarks is not null)
					xtRemarks.InnerText = value;
			}
		}
	}

	public string Code
	{
		get
			=> strCode;

		protected set
		{
			if(strCode != value)
			{
				string strOldCode = strCode;

				strCode = value;

				MakeDirty();

				FireCodeChanged(strOldCode);

				if(xtCode is not null)
					xtCode.InnerText = value;
			}
		}
	}

	internal System.Xml.XmlElement OurRoot
		=> xeUs ?? throw new System.InvalidProgramException(@"Some how the root element for an example didn't get initialized");


	private void InitMAML()
	{
		xeUs = cmdletParent.CreateMamlElement(Const.nnwpExample);

		{
			System.Xml.XmlElement xeTitle = cmdletParent.CreateMamlElement(Const.nnwpTitle);
			xeUs.AppendChild(xeTitle);

			xtTitle = cmdletParent.moduleParent.OurMAML.CreateTextNode(Title);
			xeTitle.AppendChild(xtTitle);
		}

		{
			System.Xml.XmlElement xeCode = cmdletParent.CreateMamlElement(Const.nnwpCode);
			xeUs.AppendChild(xeCode);

			xtCode = cmdletParent.moduleParent.OurMAML.CreateTextNode(strCode);
			xeCode.AppendChild(xtCode);
		}

		{
			System.Xml.XmlElement xeRemarks = cmdletParent.CreateMamlElement(Const.nnwpRemarks);
			xeUs.AppendChild(xeRemarks);

			{
				System.Xml.XmlElement xeRemarksPara = cmdletParent.CreateMamlElement(ModuleDef.Const.Elements.nnwpPara);
				xeRemarks.AppendChild(xeRemarksPara);

				xtRemarks = cmdletParent.moduleParent.OurMAML.CreateTextNode(strRemarks);
				xeRemarksPara.AppendChild(xtRemarks);
			}
		}
	}

	private void FireTitleChanged(in string strOldTitle)
	{
		FirePropChanged(nameof(Title));

		evtTitleChanged?.Invoke(this, strOldTitle, strTitle);
	}

	private void FireRemarksChanged(in string strOldDesc)
	{
		FirePropChanged(nameof(Remarks));

		evtRemarksChanged?.Invoke(this, strOldDesc, Remarks);
	}

	private void FireCodeChanged(in string strOldCode)
	{
		FirePropChanged(nameof(Code));

		evtCodeChanged?.Invoke(this, strOldCode, Code);
	}

	internal DTO.ExampleDTO ToDTO()
		=> new
		(
			Title,
			Remarks,
			Code
		);

	public Editables.ExampleEditable MakeEditable()
		=> new(this);

	internal void SaveFromEditable(in Editables.ExampleEditable eexampleWhatToSave)
	{
		Title = eexampleWhatToSave.Title;
		Remarks = eexampleWhatToSave.Remarks;
		Code = eexampleWhatToSave.Code;
	}
}