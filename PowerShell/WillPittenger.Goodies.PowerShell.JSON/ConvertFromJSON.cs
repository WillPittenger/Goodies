// Ignore Spelling: JSON

namespace WillPittenger.Goodies.PowerShell.JSON;

/// <summary>
/// Converts an input JSON string into .NET objects or PowerShell PSObject instances.  Unlike the standard PowerShell ConvertFrom-JSON, this implementation keeps
/// fields sorted.  Arrays aren't sorted.
/// </summary>
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsData.ConvertFrom, "JSON")]
public class ConvertFromJSON : System.Management.Automation.Cmdlet
{
	[System.Management.Automation.Parameter(Position=0, Mandatory = true, HelpMessage = "Pass any JSON string.", ValueFromPipeline = true)]
	[System.Management.Automation.ValidateNotNullOrWhiteSpace]
	public string Input
	{
		get;

		set;
	}

	/// <summary>
	/// By default, the code allows the parser to go up to <see cref="int.MaxValue"/> levels deep.  However, you can use <see cref="MaxDepth"/> to limit that.  The
	/// value will be passed to the parser.
	/// </summary>
	[System.Management.Automation.Parameter(HelpMessage = "Use to limit how deep parsing goes.  The default is infinite depth or at least as close as a signed " +
		"32-bit number can come.")]
	[System.Management.Automation.ValidateRange(System.Management.Automation.ValidateRangeKind.Positive)]
	[System.Management.Automation.PSDefaultValue(Help = "The default is infinite depth or at least as close as a signed 32-bit number can come.", Value = int
		.MaxValue)]
	public int MaxDepth
	{
		get;

		set;
	} = int.MaxValue;

	/// <summary>
	/// If omitted, the return value will be a <see cref="Goodies.JSON.ObjBase"/>.  If specified, the return value will be a <see cref="System.Management
	/// .Automation.PSObject"/>.
	/// </summary>
	[System.Management.Automation.Parameter(HelpMessage = "If omitted, the return value will be a WillPittenger.Goodies.JSON.ObjBase.  If specified, the return " +
		"value will be a PSObject.")]
	public System.Management.Automation.SwitchParameter AsPowerShellObj
	{
		get;

		set;
	} = false;

	/// <summary>
	/// Lets you add additional fields.  Really only useful from within PowerShell as you need to be able to pass a script block.
	/// </summary>
	/// <remarks>
	/// <para>Use $_ to access the existing object.  This will be an instance of WillPittenger.Goodies.JSON.ObjBase.  Pass a script that adds additional entries to
	/// the tables.  You can’t modify the existing entries.  If you also use -AsPowerShellObj, the new fields will become PowerShell Note Properties.  The main use
	/// for this parameter?  Suppose the JSON has a field that you know contains a date, but it's in a string.  By passing a script block, you can add an entry in
	/// a .NET type.  Any .NET type is supported.  Values can be <see langword="null"/>.  You can add both values to arrays and fields to objects.  See the 
	/// documentation for WillPittenger.Goodies.JSON.Array.Add, WillPittenger.Goodies.JSON.Array.Insert, and WillPittenger.Goodiea.JSON.Obj.Add for more
	/// information.  You have full access to the entire object.</para>
	/// </remarks>
	[System.Management.Automation.Parameter(HelpMessage = "Lets you add additional fields.  Use $_ to access the existing object.  This will be an instance of " +
		"WillPittenger.Goodies.JSON.ObjBase.  Pass a script that adds additional entries to the tables.  You can’t modify the existing entries.  If you also use " +
		"-AsPowerShellObj`, the new fields will become PowerShell Note Properties.  The main use for this parameter?  Suppose the JSON has a field that you know " +
		"contains a date, but it’s in a string.  By passing a script block, you can add an entry in any .NET type.  Any .NET type is supported.  Values can be " +
		"$null.  You can add both values to arrays and fields to objects.  See the documentation for WillPittenger.Goodies.JSON.Array.Add, " +
		"WillPittenger.Goodies.JSON.Array.Insert, and WillPittenger.Goodiea.JSON.Obj.Add for more information.  You have full access to the entire object.")]
	public System.Management.Automation.ScriptBlock? AdditionalFieldsMaker
	{
		get;

		set;
	} = null;

	/// <summary>
	/// Runs the command.
	/// </summary>
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
				WriteObject(System.Management.Automation.PowerShell.Create(System.Management.Automation.RunspaceMode.CurrentRunspace).AddCommand("Out-String")
					.AddParameter("InputObject", val).Invoke());
		}
		catch(System.Text.Json.JsonException e)
		{
			ThrowTerminatingError(new System.Management.Automation.ErrorRecord(e, "parse error", System.Management.Automation.ErrorCategory
				.ParserError, null));
		}
	}

	/// <summary>
	/// Used internally to convert from the instance of a type from <see cref="Goodies.JSON"/> to a <see cref="System.Management.Automation.PSObject"/> instance.
	/// </summary>
	/// <param name="jsonbo">Any <see cref="Goodies.JSON.ObjBase"/> that isn't a <see cref="Goodies.JSON.Array"/> or <see cref="Goodies.JSON.Obj"/>.</param>
	/// <returns>The same structure, but now represented as a <see cref="System.Management.Automation.PSObject"/>.</returns>
	/// <exception cref="System.NotImplementedException">The function couldn't figure out what to do with this value.</exception>
	private static System.Management.Automation.PSObject? ConvertJsonToPowerShellObj(Goodies.JSON.ObjBase jsonbo)
	{
		object? objInternalRetVal = jsonbo switch
		{
			null
				=> null,

			Goodies.JSON.Array jsona
				=> ConvertJsonToPowerShellObj(jsona),

			Goodies.JSON.Obj jsono
				=> ConvertJsonToPowerShellObj(jsono),

			Goodies.JSON.Val jsonv
				=> jsonv.objVal is Goodies.JSON.ObjBase jsonboChild ? ConvertJsonToPowerShellObj(jsonboChild) : jsonv.objVal,

			_
				=> throw new System.NotImplementedException(),
		};

		return objInternalRetVal == null ? null : new(objInternalRetVal);
	}

	/// <summary>
	/// Used internally to convert any <see cref="Goodies.JSON.Array"/> into a <see cref="System.Management.Automation.PSObject"/> instance.
	/// </summary>
	/// <param name="jsona">The array to convert</param>
	/// <returns>The same array, but now represented as a <see cref="System.Management.Automation.PSObject"/> array</returns>
	private static System.Management.Automation.PSObject?[] ConvertJsonToPowerShellObj(Goodies.JSON.Array jsona)
	{
		System.Management.Automation.PSObject?[] result = [];

		foreach(Goodies.JSON.ObjBase json in jsona.Elements)
		{
			object? objInternalVal = ConvertJsonToPowerShellObj(json);
			result = [..result, objInternalVal == null ? null : new System.Management.Automation.PSObject(objInternalVal)];
		}

		return result;
	}

	/// <summary>
	/// Used internally to convert any <see cref="Goodies.JSON.Obj"/> into a <see cref="System.Management.Automation.PSObject"/>.
	/// </summary>
	/// <param name="jsono">The object to convert</param>
	/// <returns>The same structure, but now represented as a <see cref="System.Management.Automation.PSObject"/></returns>
	private static System.Management.Automation.PSObject ConvertJsonToPowerShellObj(Goodies.JSON.Obj jsono)
	{
		System.Management.Automation.PSObject result = new();

		foreach(Goodies.JSON.NamedVal jsonnv in jsono)
			result.Properties.Add(new System.Management.Automation.PSNoteProperty(jsonnv.strName, jsonnv.val == null ? null : ConvertJsonToPowerShellObj(jsonnv
				.val)));

		return result;
	}
}