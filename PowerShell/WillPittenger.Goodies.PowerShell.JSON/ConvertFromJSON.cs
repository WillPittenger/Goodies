// Ignore Spelling: JSON

namespace WillPittenger.Goodies.PowerShell.JSON;

[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsData.ConvertFrom, "JSON")]
public class ConvertFromJSON : System.Management.Automation.Cmdlet
{
	[System.Management.Automation.Parameter(Position=0, Mandatory = true)]
	[System.Management.Automation.ValidateNotNullOrWhiteSpace]
	public string Input
	{
		get;

		set;
	}

	[System.Management.Automation.Parameter]
	public int MaxDepth
	{
		get;

		set;
	} = int.MaxValue;

	[System.Management.Automation.Parameter]
	public System.Management.Automation.SwitchParameter AsPowerShellObj
	{
		get;

		set;
	} = false;

	[System.Management.Automation.Parameter]
	public System.Management.Automation.ScriptBlock? AdditionalFieldsMaker
	{
		get;

		set;
	} = null;

	protected override void ProcessRecord()
	{
		base.ProcessRecord();

		try
		{
			System.Text.Json.JsonDocument jsonDoc = System.Text.Json.JsonDocument.Parse(Input, new System.Text.Json.JsonDocumentOptions()
			{
				AllowTrailingCommas = true,
				CommentHandling = System.Text.Json.JsonCommentHandling.Skip,
				MaxDepth = MaxDepth,
			});

			Goodies.JSON.ObjBase val = Goodies.JSON.ObjBase.Make(jsonDoc.RootElement);

			AdditionalFieldsMaker?.Invoke(val);

			if(AsPowerShellObj)
				WriteObject(ConvertJsonToPowerShellObj(val));
			else
				WriteObject(val);
		}
		catch(System.Text.Json.JsonException e)
		{
			ThrowTerminatingError(new System.Management.Automation.ErrorRecord(e, "parse error", System.Management.Automation.ErrorCategory
				.ParserError, null));
		}
	}

	private static System.Management.Automation.PSObject ConvertJsonToPowerShellObj(Goodies.JSON.ObjBase jsonbo)
		=> new(jsonbo switch
		{
			Goodies.JSON.Array jsona
				=> ConvertJsonToPowerShellObj(jsona),

			Goodies.JSON.Obj jsono
				=> ConvertJsonToPowerShellObj(jsono),

			Goodies.JSON.Val jsonv
				=> jsonv.objVal,

			_
				=> throw new System.NotImplementedException(),
		});

	private static System.Management.Automation.PSObject[] ConvertJsonToPowerShellObj(Goodies.JSON.Array jsona)
	{
		System.Management.Automation.PSObject[] result = [];

		foreach(Goodies.JSON.ObjBase json in jsona.elements)
			result = [..result, new System.Management.Automation.PSObject(ConvertJsonToPowerShellObj(json))];

		return result;
	}

	private static System.Management.Automation.PSObject ConvertJsonToPowerShellObj(Goodies.JSON.Obj jsono)
	{
		System.Management.Automation.PSObject result = new();

		foreach(Goodies.JSON.NamedVal jsonnv in jsono)
			result.Properties.Add(new System.Management.Automation.PSNoteProperty(jsonnv.strName, ConvertJsonToPowerShellObj(jsonnv.val)));

		return result;
	}
}