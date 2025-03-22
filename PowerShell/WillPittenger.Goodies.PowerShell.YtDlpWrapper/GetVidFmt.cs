// Ignore Spelling: yt Dlp gvf vid

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

[System.Management.Automation.Alias(@"gvf", @"Get-VideoFormat")]
[System.Management.Automation.OutputType(typeof(string))]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"VidFmt")]
public class GetVidFmt : BaseVidCmdLet
{
	[System.Management.Automation.Parameter(HelpMessage = @"This can be any mix of ID values and URLs.  Other types aren't allowed.", Mandatory = true, Position =
		1, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
	public System.Collections.Generic.IEnumerable<object>? WhatToObtain
	{
		get;

		set;
	}

	protected override System.Diagnostics.DataReceivedEventHandler? StdOutDataReceivedHandler
		=> (sender, e)
			=>
				WriteObject(e.Data);

	protected override System.Diagnostics.DataReceivedEventHandler? StdErrDataReceivedHandler
		=> (sender, e)
			=> 
				System.Console.Error.WriteLine(e.Data ?? "");

	protected override System.Collections.Generic.IEnumerable<object> AllURLs
		=> WhatToObtain ?? [];

	protected override System.Collections.Generic.IEnumerable<string> AdditionalYtDLpParams
		=> [@"--get-format"];
}