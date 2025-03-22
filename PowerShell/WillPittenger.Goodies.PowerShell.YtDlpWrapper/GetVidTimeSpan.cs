// Ignore Spelling: yt Dlp gvts

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

[System.Management.Automation.Alias(@"gvts", @"--Get-VideoDuration")]
[System.Management.Automation.OutputType(typeof(System.TimeSpan))]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"VidTimeSpan")]
public class GetVidTimeSpan : BaseVidCmdLet
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
				if(e.Data is string strData && ulong.TryParse(strData, out ulong ulDurationInMilliSeconds))
					WriteObject(System.TimeSpan.FromMilliseconds(ulDurationInMilliSeconds));
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
		=> [@"--get-duration"];
}