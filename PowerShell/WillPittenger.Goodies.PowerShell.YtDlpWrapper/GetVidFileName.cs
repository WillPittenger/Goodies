// Ignore Spelling: yt Dlp gvfn

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

[System.Management.Automation.Alias(@"gvfn", @"Get-VideoFileName")]
[System.Management.Automation.OutputType(typeof(System.IO.FileInfo))]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"VidFileName")]
public class GetVidFileName : BaseVidCmdLet
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
			{
				if(e.Data is string strData && strData.Length > 0)
				{
					try
					{
						WriteObject(new System.IO.FileInfo(strData ?? ""));
					}
					catch(System.Exception ex)
					{
						WriteWarning(@$"“{strData}” doesn't look like a filename: {ex.Message}");

						WriteObject(null);
					}
				}
				else
					WriteObject(null);
			};

	protected override System.Diagnostics.DataReceivedEventHandler? StdErrDataReceivedHandler
		=> (sender, e)
			=> 
				System.Console.Error.WriteLine(e.Data ?? "");

	protected override System.Collections.Generic.IEnumerable<object> AllURLs
		=> WhatToObtain ?? [];

	protected override System.Collections.Generic.IEnumerable<string> AdditionalYtDLpParams
		=> [@"--get-filename"];
}