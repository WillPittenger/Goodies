// Ignore Spelling: evt proj psd powershell nnwp uri globbing Ret Nav

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

public class ParamSetDef
{
	internal ParamSetDef(in CmdLetDef cmdletParent, in string strName)
	{
		this.cmdletParent = cmdletParent;

		Name = strName;


		InitMAML();
	}

	internal ParamSetDef(in CmdLetDef cmdletParent, in DTO.ParamSetDTO dto)
	{
		this.cmdletParent = cmdletParent;

		Name = dto.Name;


		InitMAML();
	}


	private static class Const
	{
		public static class Elements
		{
			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpSyntaxItem = new(@"syntaxItem", ProjDef.Const.NameSpaces.cmd);


			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpName = new(@"name", ProjDef.Const.NameSpaces.maml);
		}
	}


	public readonly CmdLetDef cmdletParent;

	private readonly System.Collections.Generic.SortedDictionary<string, ParamDetailsDef> mapAllContainedParamDetailsByParamName = [];

	public System.Xml.XmlElement? xeUs;

	public System.Xml.XmlElement? xeName;


	public System.Xml.XmlElement OurRoot
		=> xeUs ?? throw new System.InvalidProgramException(@"This parameter set isn't initialized yet.");

	public string Name
	{
		get;

		private init;
	}

	public System.Collections.Generic.IReadOnlyDictionary<string, ParamDetailsDef> AllContainedParamDetailsByParamName
		=> mapAllContainedParamDetailsByParamName;


	private void InitMAML()
	{
		xeUs = cmdletParent.CreateMamlElement(Const.Elements.nnwpSyntaxItem);


		xeName = cmdletParent.CreateMamlElement(Const.Elements.nnwpName);
		xeUs.AppendChild(xeName);

		xeName.AppendChild(cmdletParent.projParent.OurMAML.CreateTextNode(cmdletParent.Name));


		foreach(ParamDetailsDef paramddCur in mapAllContainedParamDetailsByParamName.Values)
			xeUs.AppendChild(paramddCur.owner.OurRoot);
	}

	internal void AddParameter(in ParamDetailsDef psd)
		=> mapAllContainedParamDetailsByParamName[psd.owner.Name] = psd;

	internal DTO.ParamSetDTO ToDTO()
		=> new
		(
			Name,
			[
				..from ParamDetailsDef paramdCur in mapAllContainedParamDetailsByParamName.Values
				select paramdCur.ToDTO()
			]
		);
}