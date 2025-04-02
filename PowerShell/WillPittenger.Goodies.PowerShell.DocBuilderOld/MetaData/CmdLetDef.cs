// Ignore Spelling: evt proj psd powershell nnwp uri globbing Ret Nav

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

using Tools.Ext;

[System.ComponentModel.ImmutableObject(true)]
public partial class CmdLetDef : Obj<CmdLetDef>, System.Collections.Specialized.INotifyCollectionChanged
{
	public CmdLetDef(in ProjDef projParent, in System.Type typeForCmdLet)
	{
		this.projParent = projParent;


		if(!typeForCmdLet.IsDerivedFrom(typeof(System.Management.Automation.Cmdlet)))
			throw new System.InvalidOperationException($@"{typeForCmdLet.FullName} isn’t a Cmdlet");

		if(typeForCmdLet.IsAbstract)
			throw new System.InvalidOperationException($@"{typeForCmdLet.FullName} is abstract");

		this.typeForCmdLet = typeForCmdLet;


		foreach(System.Reflection.PropertyInfo pi in typeForCmdLet.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
			mapParamsForCmdLet[pi.Name] = new(this, pi);


		foreach(object objAttr in typeForCmdLet.GetCustomAttributes(true))
		{
			if(objAttr is System.Management.Automation.CmdletAttribute cmdletattr)
			{
				Verb ??= cmdletattr.VerbName;
				Noun ??= cmdletattr.NounName;
				DefParamSet ??= mapParamSetByName[cmdletattr.DefaultParameterSetName];
			}
			else if(objAttr is System.Management.Automation.OutputTypeAttribute otattr)
				foreach(System.Management.Automation.PSTypeName? typeCur in otattr.Type)
				{
					string strTypeFullName = typeCur.Type.FullName ?? throw new System.InvalidOperationException(@"Somehow we have a type without a name");

					InputOrOutput outputNew =  new(this, false);

					mapAllOutputTypeDescByOutputTypeName[strTypeFullName] = outputNew;

					outputNew.evtTypeNameChanged += OnTypeNameOfOutputTypeChanged;
				}
		}

		JsonDocFile = new(System.IO.Path.Combine(projParent.DocsDir.FullName, $@"{Verb}-{Noun}.json"));


		InitMAML();
	}

	internal CmdLetDef(in ProjDef projParent, in DTO.CmdLetDTO dto)
	{
		this.projParent = projParent;
		typeForCmdLet = dto.TypeForCmdLet;

		Verb = dto.Verb;
		Noun = dto.Noun;
		strSynopsis = dto.Synopsis;
		strDesc = dto.Desc;

		foreach(DTO.ParamSetDTO dparamdCur in dto.AllParamSets)
			mapParamSetByName[dparamdCur.Name] = new(this, dparamdCur);

		foreach(DTO.ParamDTO dparamCur in dto.AllParams)
			mapParamsForCmdLet[dparamCur.Name] = new(this, dparamCur);

		foreach(DTO.InputOrOutputDTO dinputCur in dto.Inputs)
		{
			InputOrOutput inputNew = new(this, true, dinputCur);

			mapAllInputTypeDescByInputTypeName[dinputCur.TypeName] = inputNew;

			inputNew.evtTypeNameChanged += OnTypeNameOfInputChanged;
		}

		foreach(DTO.InputOrOutputDTO doutputCur in dto.Outputs)
		{
			InputOrOutput outputNew = new(this, false, doutputCur);

			mapAllOutputTypeDescByOutputTypeName[doutputCur.TypeName] = outputNew;

			outputNew.evtTypeNameChanged += OnTypeNameOfOutputTypeChanged;
		}

		setAllRelatedLinks.UnionWith(dto.AllRelatedLinks);


		JsonDocFile = new(System.IO.Path.Combine(projParent.DocsDir.FullName, $@"{Verb}-{Noun}.json"));


		InitMAML();
	}


	private static class Const
	{
		public static class Elements
		{
			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpCmd = new(@"command", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpDetails = new(@"details", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpName = new(@"name", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpVerb = new(@"verb", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpNoun = new(@"noun", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpSynopsis = new(@"description", ProjDef.Const.NameSpaces.maml);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpDesc = new(@"description", ProjDef.Const.NameSpaces.maml);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpSyntax = new(@"syntax", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpParams = new(@"parameters", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpInputTypes = new(@"inputTypes", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpOutputTypes = new(@"returnValues", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpAlertSet = new(@"alertSet", ProjDef.Const.NameSpaces.maml);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpOneAlert = new(@"alert", ProjDef.Const.NameSpaces.maml);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpExamples = new(@"examples", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpRelatedLinks = new(@"relatedLinks", ProjDef.Const.NameSpaces.cmd);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpNavLink = new(@"navigationLink", ProjDef.Const.NameSpaces.maml);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpLinkText = new(@"linkText", ProjDef.Const.NameSpaces.maml);

			public static readonly ProjDef.Const.NodeNameWithPrefix nnwpURI = new(@"uri", ProjDef.Const.NameSpaces.maml);
		}
	}


	public event System.Collections.Specialized.NotifyCollectionChangedEventHandler? CollectionChanged;


	public event DMapFieldChanged<System.Collections.Generic.IReadOnlyDictionary<string, InputOrOutput>, string, InputOrOutput>? evtInputTypesChanged;

	public event DMapFieldChanged<System.Collections.Generic.IReadOnlyDictionary<string, InputOrOutput>, string, InputOrOutput>? evtOutputTypesChanged;

	public event DFieldChanged<string>? evtSynopsisChanged;

	public event DFieldChanged<string>? evtDescChanged;

	public event DMapFieldChanged<System.Collections.Generic.IReadOnlyDictionary<string, Example>, string, Example>? evtExamplesChanged;

	public event DFieldChanged<string>? evtNotesChanged;


	public readonly ProjDef projParent;

	private readonly System.Type typeForCmdLet;

	private readonly System.Collections.Generic.SortedDictionary<string, ParamDef> mapParamsForCmdLet = [];

	private readonly System.Collections.Generic.SortedDictionary<string, ParamSetDef> mapParamSetByName = [];

	private readonly System.Collections.Generic.SortedDictionary<string, InputOrOutput> mapAllInputTypeDescByInputTypeName = [];

	private readonly System.Collections.Generic.SortedDictionary<string, InputOrOutput> mapAllOutputTypeDescByOutputTypeName = [];

	private string strSynopsis = string.Empty;

	private string strDesc = string.Empty;

	private readonly System.Collections.Generic.SortedList<string, Example> listAllExamplesByName = [];

	private string strNotes = string.Empty;

	private System.Collections.Generic.SortedSet<string> setAllRelatedLinks = [];
	
	private System.Xml.XmlElement? xeUs;

	private System.Xml.XmlText? xtSynopsis;

	private System.Xml.XmlText? xtDesc;

	private System.Xml.XmlText? xtNote;

	private System.Xml.XmlElement? xeRelatedLinks;

	public System.Xml.XmlElement OurRoot
		=> xeUs ?? throw new System.InvalidProgramException(@"The root element of a cmdlet isn't initialized");

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

	public ParamSetDef? DefParamSet
	{
		get;

		private set;
	}

	public System.Collections.Generic.IReadOnlyDictionary<string, ParamSetDef> AllParamSetDetails
		=> mapParamSetByName;

	public System.Collections.Generic.IReadOnlyDictionary<string, InputOrOutput> AllInputTypeDescByTypeName
		=> mapAllInputTypeDescByInputTypeName;

	public System.Collections.Generic.IReadOnlyDictionary<string, InputOrOutput> AllOutputTypeDescByOutputTypeName
		=> mapAllOutputTypeDescByOutputTypeName;


	public System.IO.FileInfo JsonDocFile
	{
		get;

		private init;
	}

	public string Synopsis
	{
		get
			=> strSynopsis;

		protected set
		{
			if(strSynopsis != value)
			{
				string strOldDoc = strSynopsis;

				strSynopsis = value;

				MakeDirty();

				FireSynopsisChanged(strOldDoc);

				if(xtSynopsis is not null)
					xtSynopsis.InnerText = value;
			}
		}
	}

	public string Desc
	{
		get
			=> strDesc;

		protected set
		{
			if(strDesc != value)
			{
				string strOldDesc = strDesc;

				strDesc = value;

				MakeDirty();

				FireDescChanged(strOldDesc);

				if(xtDesc is not null)
					xtDesc.InnerText = value;
			}
		}
	}

	public System.Collections.Generic.IReadOnlyDictionary<string, Example> AllExamples
		=> listAllExamplesByName;

	public string Notes
	{
		get
			=> strNotes;

		protected set
		{
			if(strNotes != value)
			{
				string strOldNotes = strNotes;

				strNotes = value;

				MakeDirty();

				FireNotesChanged(strOldNotes);

				if(xtNote is not null)
					xtNote.InnerText = value;
			}
		}
	}

	public System.Collections.Generic.IReadOnlySet<string> AllRelatedLinks
		=> setAllRelatedLinks;


	private void InitMAML()
	{
		xeUs = CreateMamlElement(Const.Elements.nnwpCmd);

		{
			System.Xml.XmlElement xeDetails = CreateMamlElement(Const.Elements.nnwpDetails);
			xeUs.AppendChild(xeDetails);

			{
				System.Xml.XmlElement xeName = CreateMamlElement(Const.Elements.nnwpName);
				xeDetails.AppendChild(xeName);

				xeDetails.AppendChild(projParent.OurMAML.CreateTextNode(Name));
			}

			{
				System.Xml.XmlElement xeVerb = CreateMamlElement(Const.Elements.nnwpVerb);
				xeDetails.AppendChild(xeVerb);

				xeVerb.AppendChild(projParent.OurMAML.CreateTextNode(Verb));
			}

			{
				System.Xml.XmlElement xeNoun = CreateMamlElement(Const.Elements.nnwpNoun);
				xeDetails.AppendChild(xeNoun);

				xeNoun.AppendChild(projParent.OurMAML.CreateTextNode(Noun));
			}

			{
				System.Xml.XmlElement xeSynopsis = CreateMamlElement(Const.Elements.nnwpSynopsis);
				xeDetails.AppendChild(xeSynopsis);

				{
					System.Xml.XmlElement xeSynopsisPara = CreateMamlElement(ProjDef.Const.Elements.nnwpPara);
					xeSynopsis.AppendChild(xeSynopsisPara);

					xtSynopsis = projParent.OurMAML.CreateTextNode(strSynopsis);
					xeSynopsisPara.AppendChild(xtSynopsis);
				}
			}
		}

		{
			System.Xml.XmlElement xeDesc = CreateMamlElement(Const.Elements.nnwpDetails);
			xeUs.AppendChild(xeDesc);

			{
				System.Xml.XmlElement xeDescPara = CreateMamlElement(ProjDef.Const.Elements.nnwpPara);
				xeDesc.AppendChild(xeDescPara);

				xtDesc = projParent.OurMAML.CreateTextNode(strDesc);
				xeDescPara.AppendChild(xtDesc);
			}
		}

		{
			System.Xml.XmlElement xeSyntax = CreateMamlElement(Const.Elements.nnwpSyntax);
			xeUs.AppendChild(xeSyntax);

			foreach(ParamSetDef paramsdCur in mapParamSetByName.Values)
				xeSyntax.AppendChild(paramsdCur.OurRoot);
		}

		{
			System.Xml.XmlElement xeCmdParams = CreateMamlElement(Const.Elements.nnwpParams);
			xeUs.AppendChild(xeCmdParams);

			foreach(ParamDef paramdCur in mapParamsForCmdLet.Values)
				xeCmdParams.AppendChild(paramdCur.OurRoot);
		}

		{
			System.Xml.XmlElement xeExamples = CreateMamlElement(Const.Elements.nnwpExamples);
			xeUs.AppendChild(xeExamples);

			foreach(Example exampleCur in listAllExamplesByName.Values)
				xeExamples.AppendChild(exampleCur.OurRoot);
		}

		{
			System.Xml.XmlElement xeInputTypes = CreateMamlElement(Const.Elements.nnwpInputTypes);
			xeUs.AppendChild(xeInputTypes);

			foreach(InputOrOutput inputCur in mapAllInputTypeDescByInputTypeName.Values)
				xeInputTypes.AppendChild(inputCur.OurRoot);
		}

		{
			System.Xml.XmlElement xeOutputTypes = CreateMamlElement(Const.Elements.nnwpInputTypes);
			xeUs.AppendChild(xeOutputTypes);

			foreach(InputOrOutput outputCur in mapAllOutputTypeDescByOutputTypeName.Values)
				xeOutputTypes.AppendChild(outputCur.OurRoot);
		}


		System.Xml.XmlElement xeNote = CreateMamlElement(Const.Elements.nnwpAlertSet);
		xeUs.AppendChild(xeNote);

		{
			System.Xml.XmlElement xeNoteAlert = CreateMamlElement(Const.Elements.nnwpOneAlert);
			xeNote.AppendChild(xeNoteAlert);

			{
				System.Xml.XmlElement xeNoteAlertPara = CreateMamlElement(ProjDef.Const.Elements.nnwpPara);
				xeNoteAlert.AppendChild(xeNoteAlertPara);

				xtNote = projParent.OurMAML.CreateTextNode(strNotes);
				xeNoteAlertPara.AppendChild(xtNote);
			}
		}

		{
			xeRelatedLinks = CreateMamlElement(Const.Elements.nnwpRelatedLinks);
			xeUs.AppendChild(xeRelatedLinks);

			RebuildRelatedLinksMAML();
		}
	}

	private void RebuildRelatedLinksMAML()
	{
		if(xeRelatedLinks is null)
			throw new System.InvalidProgramException(@"This CmdLetDef isn't ready");

		xeRelatedLinks.InnerText = string.Empty;

		foreach(string strCurRelatedItem in setAllRelatedLinks)
		{
			System.Xml.XmlElement xeNavLink = CreateMamlElement(Const.Elements.nnwpNavLink);
			xeRelatedLinks.AppendChild(xeNavLink);

			{
				System.Xml.XmlElement xeLinkText = CreateMamlElement(Const.Elements.nnwpLinkText);
				xeNavLink.AppendChild(xeLinkText);

				xeLinkText.AppendChild(projParent.OurMAML.CreateTextNode(strCurRelatedItem));
			}

			{
				System.Xml.XmlElement xeURI = CreateMamlElement(Const.Elements.nnwpURI);
				xeNavLink.AppendChild(xeURI);
			}
		}
	}

	internal void RegisterParameSetDetails(in ParamDetailsDef psd)
	{
		if(psd.owner.owner != this)
			throw new System.InvalidProgramException(@"Somehow we ended up with a parameter set details trying to be added to a Cmdlet's Parameter sets, but that cmdlet doesn't have the specified parameter.");

		ParamSetDef pcsd = mapParamSetByName.TryGetValue(psd.strNameOfAssociateParamSet, out ParamSetDef? value)
			? value
			: (mapParamSetByName[psd.strNameOfAssociateParamSet] = new(this, psd.strNameOfAssociateParamSet));

		pcsd.AddParameter(psd);
	}

	private void FireInputsChanged(in System.Collections.Generic.IEnumerable<string>? eNewKeys = null, in System.Collections.Generic.IEnumerable<System.Tuple<string, InputOrOutput>>? eRemovedKeys = null, in System.Collections.Generic.IEnumerable<System.Tuple<string, string, InputOrOutput>>? eChangedKeys = null)
	{
		FirePropChanged(nameof(AllInputTypeDescByTypeName));

		evtInputTypesChanged?.Invoke(this, mapAllInputTypeDescByInputTypeName, eNewKeys, eRemovedKeys, eChangedKeys);
	}

	private void FireOutputsChanged(in System.Collections.Generic.IEnumerable<string>? eNewKeys = null, in System.Collections.Generic.IEnumerable<System.Tuple<string, InputOrOutput>>? eRemovedKeys = null, in System.Collections.Generic.IEnumerable<System.Tuple<string, string, InputOrOutput>>? eChangedKeys = null)
	{
		FirePropChanged(nameof(AllOutputTypeDescByOutputTypeName));

		evtInputTypesChanged?.Invoke(this, mapAllOutputTypeDescByOutputTypeName, eNewKeys, eRemovedKeys, eChangedKeys);
	}

	private void FireSynopsisChanged(in string strOldSynopsis)
	{
		FirePropChanged(nameof(Synopsis));

		evtSynopsisChanged?.Invoke(this, strOldSynopsis, strSynopsis);
	}

	private void FireDescChanged(in string strOldDesc)
	{
		FirePropChanged(nameof(Desc));

		evtDescChanged?.Invoke(this, strOldDesc, strDesc);
	}

	private void FireNotesChanged(in string strOldNotes)
	{
		FirePropChanged(nameof(Notes));

		evtNotesChanged?.Invoke(this, strOldNotes, strNotes);
	}

	internal DTO.CmdLetDTO ToDTO()
		=> new(
			typeForCmdLet,
			Verb ?? string.Empty,
			Noun ?? string.Empty,
			guid,
			strSynopsis,
			strDesc,
			[..from ParamDef paramCur in mapParamsForCmdLet.Values
				 select paramCur.ToDTO()],
			[..from ParamSetDef paramsCur in mapParamSetByName.Values
				 select paramsCur.ToDTO()],
			[..from System.Collections.Generic.KeyValuePair<string, string?> kvCur in mapAllInputTypeDescByInputTypeName
				 select new DTO.InputOrOutputDTO(
					 kvCur.Key,
					 kvCur.Value
					)],
			[..from System.Collections.Generic.KeyValuePair<string, string?> kvCur in mapAllOutputTypeDescByOutputTypeName
				 select new DTO.InputOrOutputDTO(
					 kvCur.Key,
					 kvCur.Value
					)],
			[..from Example exampleCur in listAllExamplesByName.Values
				 select exampleCur.ToDTO()],
			[..setAllRelatedLinks]
		);

	internal System.Xml.XmlElement CreateMamlElement(ProjDef.Const.NodeNameWithPrefix nnwpCreateWhat)
		=> projParent.OurMAML.CreateElement(nnwpCreateWhat.Name, nnwpCreateWhat.NameSpace.uri.AbsolutePath);

	protected void AddInputType(in InputOrOutput inputNew)
	{
		if(mapAllInputTypeDescByInputTypeName.ContainsKey(inputNew.TypeName))
			throw new System.Exception($@"The cmdlet {Name} already contains an input named {inputNew.TypeName}");

		mapAllInputTypeDescByInputTypeName[inputNew.TypeName] = inputNew;

		MakeDirty();

		FireInputsChanged(eNewKeys: [inputNew.TypeName]);
	}

	protected void AddOutputType(in InputOrOutput outputNew)
	{
		if(mapAllOutputTypeDescByOutputTypeName.ContainsKey(outputNew.TypeName))
			throw new System.Exception($@"The cmdlet {Name} already contains an output named {outputNew.TypeName}");

		mapAllOutputTypeDescByOutputTypeName[outputNew.TypeName] = outputNew;

		MakeDirty();

		FireOutputsChanged(eNewKeys: [outputNew.TypeName]);
	}

	protected void RemoveInput(in InputOrOutput inputRemoveThis)
	{
		if(!mapAllInputTypeDescByInputTypeName.ContainsKey(inputRemoveThis.TypeName))
			throw new System.InvalidOperationException($@"The CmdletDef {Name} doesn't contain {inputRemoveThis.TypeName}.");

		mapAllInputTypeDescByInputTypeName.Remove(inputRemoveThis.TypeName);

		MakeDirty();

		FireInputsChanged(eRemovedKeys:
			[
				new(inputRemoveThis.TypeName, inputRemoveThis),
			]);
	}

	protected void RemoveOutput(in InputOrOutput OutputRemoveThis)
	{
		if(!mapAllOutputTypeDescByOutputTypeName.ContainsKey(OutputRemoveThis.TypeName))
			throw new System.InvalidOperationException($@"The CmdletDef {Name} doesn't contain {OutputRemoveThis.TypeName}.");

		mapAllOutputTypeDescByOutputTypeName.Remove(OutputRemoveThis.TypeName);

		MakeDirty();

		FireOutputsChanged(eRemovedKeys:
			[
				new(OutputRemoveThis.TypeName, OutputRemoveThis),
			]
		);
	}


	private void OnTypeNameOfInputChanged(in InputOrOutput inputSender, in string strOldVal, in string strNewVal)
	{
		mapAllInputTypeDescByInputTypeName.Remove(strOldVal);

		mapAllInputTypeDescByInputTypeName[inputSender.TypeName] = inputSender;


		MakeDirty();


		FireInputsChanged(eChangedKeys:
			[
				new(strOldVal, strNewVal, inputSender),
			]
		);
	}

	private void OnTypeNameOfOutputTypeChanged(in InputOrOutput outputSender, in string strOldVal, in string strNewVal)
	{
		mapAllOutputTypeDescByOutputTypeName.Remove(strOldVal);

		mapAllOutputTypeDescByOutputTypeName[outputSender.TypeName] = outputSender;


		MakeDirty();


		FireOutputsChanged(eChangedKeys:
			[
				new(strOldVal, strNewVal, outputSender),
			]
		);
	}
}