// Ignore Spelling: yt Dlp swyd

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

[System.Management.Automation.Alias("swyd")]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsData.Save, @"WithYtDlp")]
public class SaveWithYtDlp : BaseVidCmdLet
{
	[System.Management.Automation.Parameter(HelpMessage = @"This can be any mix of ID values, URLs, plus anything returned by Get-InfoFromYtDlp.  Other types "
		+ @"aren't allowed.", Mandatory = true, Position = 1, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
	public System.Collections.Generic.IEnumerable<object>? WhatToDownload
	{
		get;

		set;
	}
	
	protected override System.Diagnostics.DataReceivedEventHandler? StdOutDataReceivedHandler
		=> IsWhatIfOn
			? (sender, e)
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
				}
			: null;

	protected override System.Collections.Generic.IEnumerable<object> AllURLs
		=> lliststrWhatToDownload;


	private readonly System.Collections.Generic.LinkedList<string> lliststrWhatToDownload = [];

	private ulong ulCurParam = 1;

	/// <inheritdoc/>
	protected override void ProcessRecord()
	{
		base.ProcessRecord();

		if(WhatToDownload is not null)
			foreach(object objCurInput in WhatToDownload)
			{
				if(objCurInput is string strCurInput)
					lliststrWhatToDownload.AddLast(strCurInput);
				else if(objCurInput is System.Uri uriCurInput)
					lliststrWhatToDownload.AddLast(uriCurInput.AbsolutePath);
				else if(objCurInput is Goodies.YtDlpWrapper.BaseObj bobjCurInput)
					lliststrWhatToDownload.AddLast(bobjCurInput.strID);
				else
					WriteWarning(@$"Unable to interpret parameter {ulCurParam}: “{objCurInput}”");

				ulCurParam++;
			}
	}

	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<string> AdditionalYtDLpParams
		=> IsWhatIfOn
			? [@"--print", @"filename"]
			: [];
}