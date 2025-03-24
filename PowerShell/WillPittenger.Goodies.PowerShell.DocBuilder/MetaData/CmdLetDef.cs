// Ignore Spelling: evt

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

using System.Linq;
using Tools.Ext;

[System.ComponentModel.ImmutableObject(true)]
public class CmdLetDef
{
	public CmdLetDef(in System.Type typeForCmdLet)
	{
		if(!typeForCmdLet.IsDerivedFrom(typeof(System.Management.Automation.Cmdlet)))
			throw new System.InvalidOperationException($@"{typeForCmdLet.FullName} isn’t a Cmdlet");

		if(typeForCmdLet.IsAbstract)
			throw new System.InvalidOperationException($@"{typeForCmdLet.FullName} is abstract");

		this.typeForCmdLet = typeForCmdLet;


		foreach(System.Reflection.PropertyInfo pi in typeForCmdLet.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
			mapParamsForCmdLet[pi.Name] = pi;


		foreach(object objAttr in typeForCmdLet.GetCustomAttributes(true))
		{
			if(objAttr is System.Management.Automation.CmdletAttribute cmdletattr)
			{
				Verb ??= cmdletattr.VerbName;
				Noun ??= cmdletattr.NounName;
				DefParamSet ??= mapParamSetToDetailsForEachParam[cmdletattr.DefaultParameterSetName];
			}
		}
	}


	public class ParamCollectionSetDef
	{
		internal ParamCollectionSetDef(in string strName)
			=> Name = strName;


		public string Name
		{
			get;

			private init;
		}


		private System.Collections.Generic.SortedDictionary<string, ParamDef.ParamSetDef> mapAllContainedParamDetailsByParamName = [];



		public System.Collections.Generic.IReadOnlyDictionary<string, ParamDef.ParamSetDef> AllContainedParamDetailsByParamName
			=> mapAllContainedParamDetailsByParamName;

		public void AddParameter(in ParamDef.ParamSetDef psd)
			=> mapAllContainedParamDetailsByParamName[psd.owner.Name] = psd;
	}

	public class ParamDef
	{
		public ParamDef(in CmdLetDef owner, in System.Reflection.PropertyInfo pi)
		{
			this.owner = owner;
			this.pi = pi;

			foreach(object objAttr in pi.GetCustomAttributes(true))
			{
				if(objAttr is System.Management.Automation.AliasAttribute aliasattr)
					foreach(string strCurAlias in aliasattr.AliasNames)
						setAliases.Add(strCurAlias);
				else if(objAttr is System.Management.Automation.PSDefaultValueAttribute dvattr)
					DefaultVal ??= dvattr.Value;
				else if(objAttr is System.Management.Automation.ParameterAttribute pattr)
					mapAllParamSetsByName[pattr.ParameterSetName] = new(this, pattr);
			}
		}


		public class ParamSetDef
		{
			public ParamSetDef(in ParamDef owner, in System.Management.Automation.ParameterAttribute pattr)
			{
				this.owner = owner;

				strNameOfAssociateParamSet = pattr.ParameterSetName;

				strHelpText = pattr.HelpMessage;
				IsMandatory = pattr.Mandatory;
				IsValueFromPipeline = pattr.ValueFromPipeline;
				IsValueFromRemainingArgs = pattr.ValueFromRemainingArguments;

				owner.owner.RegisterParameSetDetails(this);
			}


			public delegate void DHelpTextChanged(in ParamSetDef dpsSender, in string? strOldHelpText, in string? strNewHelpText);


			public event DHelpTextChanged? evtHelpTextChanged;


			public readonly ParamDef owner;

			public readonly string strNameOfAssociateParamSet;

			private string? strHelpText;

			public string? HelpText
			{
				get
					=> strHelpText;

				set
				{
					if(strHelpText != value)
					{
						string? strOldHelpText = strHelpText;

						strHelpText = value;

						evtHelpTextChanged?.Invoke(this, strOldHelpText, strHelpText);
					}
				}
			}

			public bool IsMandatory
			{
				get;

				private init;
			}

			public bool IsValueFromPipeline
			{
				get;

				private init;
			}

			public bool IsValueFromRemainingArgs
			{
				get;

				private init;
			}

			public int Pos
			{
				get;

				private init;
			}
		}


		public readonly CmdLetDef owner;

		public readonly System.Reflection.PropertyInfo pi;


		private readonly System.Collections.Generic.SortedSet<string> setAliases = [];

		private readonly System.Collections.Generic.SortedDictionary<string, ParamSetDef> mapAllParamSetsByName = [];


		public string Name
			=> pi.Name;

		public System.Collections.Generic.IEnumerable<string> AllAliases
			=> setAliases;

		public object? DefaultVal
		{
			get;

			private init;
		}

		public System.Type Type
			=> pi.PropertyType;

		public System.Collections.Generic.IReadOnlyDictionary<string, ParamSetDef> AllParamSets
			=> mapAllParamSetsByName;
	}


	private readonly System.Type typeForCmdLet;

	private readonly System.Collections.Generic.SortedDictionary<string, System.Reflection.PropertyInfo> mapParamsForCmdLet = [];

	private readonly System.Collections.Generic.SortedDictionary<string, ParamCollectionSetDef> mapParamSetToDetailsForEachParam = [];


	public string Name
		=> typeForCmdLet.Name;

	public string? FullyQualifiedName
		=> typeForCmdLet.FullName;

	public string? Verb
	{
		get;

		private set;
	}

	public string? Noun
	{
		get;

		private set;
	}

	public ParamCollectionSetDef? DefParamSet
	{
		get;

		private set;
	}

	public System.Collections.Generic.IReadOnlyDictionary<string, ParamCollectionSetDef> AllParamSetDetails
		=> mapParamSetToDetailsForEachParam;


	internal void RegisterParameSetDetails(in ParamDef.ParamSetDef psd)
	{
		if(psd.owner.owner != this)
			throw new System.InvalidProgramException(@"Somehow we ended up with a parameter set details trying to be added to a Cmdlet's Parameter sets, but that cmdlet doesn't have the specified parameter.");

		ParamCollectionSetDef pcsd = mapParamSetToDetailsForEachParam.TryGetValue(psd.strNameOfAssociateParamSet, out ParamCollectionSetDef? value)
			? value
			: (mapParamSetToDetailsForEachParam[psd.strNameOfAssociateParamSet] = new(psd.strNameOfAssociateParamSet));

		pcsd.AddParameter(psd);
	}
}