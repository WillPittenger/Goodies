namespace WillPittenger.Goodies.PowerShell.Tools.Attr;

using Goodies.Tools.Ext;

[System.AttributeUsage(System.AttributeTargets.Property)]
public class ValidateWithDelegateAttribute : System.Management.Automation.ValidateEnumeratedArgumentsAttribute
{
	public ValidateWithDelegateAttribute(System.Type typeOfValidator)
	{
		if(!typeOfValidator.IsDerivedFrom(typeof(IInputValidator))
			throw new System.InvalidProgramException($@"All validation types passed to ValidateWithDelegateAttribute must implement ValidateWithDelegateAttribute.IInputValidator.");

		validator = (IInputValidator)(typeOfValidator.GetConstructor([])?.Invoke([]) ?? throw new System.InvalidProgramException($@"The type {typeOfValidator.FullName} can't be used by ValidateWithDelegateAttribute as it lacks a default constructor"));
	}

	public interface IInputValidator
	{
		string? Validate(System.Collections.Generic.IEnumerable<object> eobjInputsToValidate);
	}

	private readonly IInputValidator validator;

	protected override void ValidateElement(object objElement)
	{
		if(objElement is System.Collections.Generic.IEnumerable<object> eobjInputsToValidate && validator.Validate(eobjInputsToValidate) is string strErrorMsg)
			throw new System.Management.Automation.ValidationMetadataException(strErrorMsg);
	}
}