// Ignore Spelling: evt proj psd powershell nnwp uri globbing Ret Nav

namespace WillPittenger.Goodies.PowerShell.DocBuilder.MetaData;

public class ParamDetailsDef : Obj<ParamDetailsDef>
{
	public ParamDetailsDef(in ParamDef owner, in System.Management.Automation.ParameterAttribute pattr)
	{
		this.owner = owner;

		strNameOfAssociateParamSet = pattr.ParameterSetName;

		IsMandatory = pattr.Mandatory;
		IsValueFromPipeline = pattr.ValueFromPipeline;
		IsValueFromRemainingArgs = pattr.ValueFromRemainingArguments;
		HelpText = pattr.HelpMessage;
		Pos = pattr.Position == int.MinValue
			? null
			: pattr.Position;

		owner.owner.RegisterParamSetDetails(this);
	}

	internal ParamDetailsDef(in ParamDef owner, in DTO.ParamDetailsDTO dto)
	{
		this.owner = owner;

		strNameOfAssociateParamSet = dto.AssociatedParamSetName;
		IsValueFromPipeline = dto.IsValFromPipeline;
		IsValueFromRemainingArgs = dto.IsValFromRemainingArgs;
		HelpText = dto.HelpText;
		Pos = dto.Pos;

		owner.owner.RegisterParamSetDetails(this);
	}

	internal ParamDetailsDef(in ParamDetailsDef paramddCopyThis)
	{
		owner = paramddCopyThis.owner;

		strNameOfAssociateParamSet = paramddCopyThis.strNameOfAssociateParamSet;

		IsMandatory = paramddCopyThis.IsMandatory;
		IsValueFromPipeline= paramddCopyThis.IsValueFromPipeline;
		IsValueFromRemainingArgs = paramddCopyThis.IsValueFromRemainingArgs;
		HelpText = paramddCopyThis.HelpText;
		Pos = paramddCopyThis.Pos;
	}


	public event DFieldChanged<string>? evtHelpTextChanged;


	public readonly ParamDef owner;

	public readonly string strNameOfAssociateParamSet;


	public string? HelpText
	{
		get;

		private init;
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

	public int? Pos
	{
		get;

		private init;
	}


	internal DTO.ParamDetailsDTO ToDTO()
		=> new
		(
			strNameOfAssociateParamSet,
			IsMandatory,
			IsValueFromPipeline,
			IsValueFromRemainingArgs,
			HelpText,
			Pos
		);
}