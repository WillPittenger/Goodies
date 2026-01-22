// Ignore Spelling: evt proj psd powershell nnwp uri globbing Ret Nav

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

using Tools.Ext;

public partial class ParamDef : Obj<ParamDef>
{
	public ParamDef(in CmdLetDef owner, in System.Reflection.PropertyInfo pi)
	{
		this.owner = owner;
		Type = pi.PropertyType;
		Name = pi.Name;

		if(Type.IsEnum)
			setAllAllowedVals.UnionWith(Type.GetEnumNames());

		foreach(object objAttr in pi.GetCustomAttributes(true))
		{
			if(objAttr is System.Management.Automation.AliasAttribute aliasattr)
				foreach(string strCurAlias in aliasattr.AliasNames)
					setAliases.Add(strCurAlias);
			else if(objAttr is System.Management.Automation.PSDefaultValueAttribute dvattr)
			{
				DefVal ??= dvattr.Value;

				HasDefVal = true;
			}
			else if(objAttr is System.Management.Automation.ParameterAttribute pattr)
			{
				mapAllParamSetsByName[pattr.ParameterSetName] = new(this, pattr);

				strHelpText ??= pattr.HelpMessage;
			}
			else if(objAttr is System.Management.Automation.SupportsWildcardsAttribute)
				AreWildCardsSupported = true;
			else if(objAttr is System.Management.Automation.ValidateSetAttribute vsattr)
				setAllAllowedVals.UnionWith(vsattr.ValidValues);
		}


		InitMAML();
	}

	internal ParamDef(in CmdLetDef owner, in DTO.ParamDTO dto)
	{
		this.owner = owner;
		Type = dto.ParamType;
		Name = dto.Name;

		setAliases = [..dto.AllAliases];
		AreWildCardsSupported = dto.AreWildCardsSupported;
		setAllAllowedVals.UnionWith(dto.AllAllowedVals);
		DefVal = dto.DefVal;
		HasDefVal = dto.HasDefVal;
		HelpText = dto.HelpText;

		foreach(DTO.ParamDetailsDTO dparamd in dto.Details)
			mapAllParamSetsByName[dparamd.AssociatedParamSetName] = new(this, dparamd);


		InitMAML();
	}

	protected ParamDef(in ParamDef paramd)
	{
		owner = paramd.owner;
		Type = paramd.Type;
		Name = paramd.Name;

		setAllAllowedVals.UnionWith(paramd.AllAllowedVals);

		setAliases.UnionWith(paramd.AllAliases);

		DefVal = paramd.DefVal;

		HasDefVal = paramd.HasDefVal;

		HelpText = paramd.HelpText;

		foreach(ParamDetailsDef paramddCur in paramd.AllParamSets.Values)
			mapAllParamSetsByName[paramddCur.strNameOfAssociateParamSet] = new(paramddCur);

		AreWildCardsSupported = paramd.AreWildCardsSupported;


		InitMAML();
	}


	public event DFieldChanged<string>? evtHelpTextChanged;

	public event DCollectionFieldChanged<System.Collections.Generic.IReadOnlySet<string>, string>? evtAliasesCollectionChanged;

	public event DMapFieldChanged<System.Collections.Generic.IReadOnlyDictionary<string, ParamDetailsDef>, string, ParamDetailsDef>? evtParamSetChanged;


	private static class Const
	{
		public static class Elements
		{
			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpParam = new(@"parameter", ModuleDef.Const.NameSpaces.cmd);


			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpName = new(@"name", ModuleDef.Const.NameSpaces.maml);

			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpDesc = new(@"description", ModuleDef.Const.NameSpaces.maml);

			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpParamVal = new(@"parameterValue", ModuleDef.Const.NameSpaces.cmd);

			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpType = new(@"type", ModuleDef.Const.NameSpaces.dev);

			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpDefVal = new(@"defaultValue", ModuleDef.Const.NameSpaces.dev);

			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpTypeName = new(@"name", ModuleDef.Const.NameSpaces.maml);

			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpTypeURI = new(@"uri", ModuleDef.Const.NameSpaces.maml);
		}

		public static class Attr
		{
			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpRequired = new(@"required", ModuleDef.Const.NameSpaces.@default);

			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpVariableLength = new(@"variableLength", ModuleDef.Const.NameSpaces.@default);

			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpGlobbing = new(@"globbing", ModuleDef.Const.NameSpaces.@default);

			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpPipelineInput = new(@"pipelineInput", ModuleDef.Const.NameSpaces.@default);

			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpPos = new(@"position", ModuleDef.Const.NameSpaces.@default);

			public static readonly ModuleDef.Const.NodeNameWithPrefix nnwpAliases = new(@"aliases", ModuleDef.Const.NameSpaces.@default);
		}
	}


	public readonly CmdLetDef owner;


	private string? strHelpText = null;

	private readonly System.Collections.Generic.SortedSet<string> setAliases = [];

	private readonly System.Collections.Generic.SortedDictionary<string, ParamDetailsDef> mapAllParamSetsByName = [];

	private readonly System.Collections.Generic.SortedSet<string> setAllAllowedVals = [];

	private System.Xml.XmlElement? xeUs;

	private System.Xml.XmlElement? xeParamSetRoot;

	private System.Xml.XmlText? xtDesc;


	public System.Xml.XmlElement OurRoot
		=> xeUs ?? throw new System.InvalidProgramException(@"This parameter isn't properly initialized");

	public System.Xml.XmlElement OurParamSetRoot
		=> xeParamSetRoot ??= (System.Xml.XmlElement)OurRoot.Clone();

	public string Name
	{
		get;

		private init;
	}

	public string HelpText
	{
		get
			=> strHelpText ?? (mapAllParamSetsByName.Count > 0 ? mapAllParamSetsByName.Values.First() : null)?.HelpText ?? string.Empty;

		protected set
		{
			if(strHelpText != value)
			{
				string strOldHelpText = strHelpText ?? string.Empty;

				strHelpText = value;

				MakeDirty();

				FireHelpTextChanged(strOldHelpText);
			}
		}
	}

	public System.Collections.Generic.IReadOnlySet<string> AllAliases
		=> setAliases;

	public bool AreWildCardsSupported
	{
		get;

		private init;
	}

	public object? DefVal
	{
		get;

		private init;
	}

	public bool HasDefVal
	{
		get;

		private init;
	} = false;

	private string DefValAsText
		=> HasDefVal
			? @"none"
			: DefVal?.ToString()
				?? @"$null";

	public System.Type Type
	{
		get;

		private init;
	}

	public System.Collections.Generic.IReadOnlyDictionary<string, ParamDetailsDef> AllParamSets
		=> mapAllParamSetsByName;

	public string ParamSetsAsStr
		=> mapAllParamSetsByName.Count > 0
			? mapAllParamSetsByName.Values.Join(',')
			: @"(all)";

	private string PosText
	{
		get
		{
			if(mapAllParamSetsByName.Count == 0)
				return @"named";

			int iPosInFirstParamSetFound = mapAllParamSetsByName.Values.First()?.Pos ?? int.MinValue;

			return iPosInFirstParamSetFound > int.MinValue
				? iPosInFirstParamSetFound.ToString()
				: @"named";
		}
	}

	public System.Collections.Generic.IReadOnlySet<string> AllAllowedVals
		=> setAllAllowedVals;


	private void InitMAML()
	{
		xeUs = owner.CreateMamlElement(Const.Elements.nnwpParam);

		xeUs.SetAttribute(Const.Attr.nnwpRequired.Name, mapAllParamSetsByName.Values.First().IsMandatory.ToString());
		xeUs.SetAttribute(Const.Attr.nnwpVariableLength.Name, mapAllParamSetsByName.Values.First().IsValueFromRemainingArgs.ToString());
		xeUs.SetAttribute(Const.Attr.nnwpGlobbing.Name, AreWildCardsSupported.ToString());
		xeUs.SetAttribute(Const.Attr.nnwpPipelineInput.Name, mapAllParamSetsByName.Values.First().IsValueFromPipeline.ToString());
		xeUs.SetAttribute(Const.Attr.nnwpPos.Name, PosText);
		xeUs.SetAttribute(Const.Attr.nnwpAliases.Name, setAliases.Count > 0
			? setAliases.Join(',')
			: @"none"
		);


		{
			System.Xml.XmlElement xeName = owner.CreateMamlElement(Const.Elements.nnwpName);
			xeUs.AppendChild(xeName);

			xeName.InnerText = Name;
		}

		{
			System.Xml.XmlElement xeDesc = owner.CreateMamlElement(Const.Elements.nnwpDesc);
			xeUs.AppendChild(xeDesc);

			{
				System.Xml.XmlElement xeDescPara = owner.CreateMamlElement(ModuleDef.Const.Elements.nnwpPara);
				xeDesc.AppendChild(xeDescPara);

				xtDesc = owner.moduleParent.OurMAML.CreateTextNode(strHelpText);
				xeDescPara.AppendChild(xtDesc);
			}
		}

		{
			System.Xml.XmlElement xeParamVal = owner.CreateMamlElement(Const.Elements.nnwpParamVal);
			xeUs.AppendChild(xeParamVal);

			xeParamVal.SetAttribute(Const.Attr.nnwpRequired.Name, mapAllParamSetsByName.Values.First().IsMandatory.ToString());
			xeParamVal.SetAttribute(Const.Attr.nnwpVariableLength.Name, mapAllParamSetsByName.Values.First().IsValueFromRemainingArgs.ToString());

			xeParamVal.InnerText = Type.FullName ?? string.Empty;
		}

		{
			System.Xml.XmlElement xeType = owner.CreateMamlElement(Const.Elements.nnwpType);
			xeUs.AppendChild(xeType);

			{
				System.Xml.XmlElement xeTypeName = owner.CreateMamlElement(Const.Elements.nnwpTypeName);
				xeType.AppendChild(xeTypeName);

				xeTypeName.AppendChild(owner.moduleParent.OurMAML.CreateTextNode(Type.FullName));

				{
					System.Xml.XmlElement xeTypeURI = owner.CreateMamlElement(Const.Elements.nnwpTypeURI);
					xeUs.AppendChild(xeTypeURI);
				}
			}
		}

		{
			System.Xml.XmlElement xeDefVal = owner.CreateMamlElement(Const.Elements.nnwpDefVal);
			xeUs.AppendChild(xeDefVal);

			xeDefVal.AppendChild(owner.moduleParent.OurMAML.CreateTextNode(DefValAsText));
		}
	}

	private void FireHelpTextChanged(in string strOldHelpText)
	{
		FirePropChanged(nameof(HelpText));

		evtHelpTextChanged?.Invoke(this, strOldHelpText, strHelpText ?? string.Empty);
	}

	internal DTO.ParamDTO ToDTO()
		=> new
		(
			Name,
			HelpText,
			Type,
			AreWildCardsSupported,
			[..setAllAllowedVals],
			[..setAliases],
			DefVal,
			HasDefVal,
			[..mapAllParamSetsByName.Values.Select(
				paramdCur
					=> paramdCur.ToDTO()
			)]
		);

	public Editables.ParamEditable MakeEditable()
		=> new(this);

	internal void SaveFromEditable(in Editables.ParamEditable paramde)
		=> HelpText = paramde.HelpText;
};