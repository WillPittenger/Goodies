// Ignore Spelling: Yt Dlp Vid gvi gifyd

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

[System.Management.Automation.OutputType(typeof(Goodies.YtDlpWrapper.Chan), typeof(Goodies.YtDlpWrapper.Vid), typeof(Goodies.YtDlpWrapper.PlayList))]
[System.Management.Automation.Alias(@"gvi", @"gifyd", @"Get-VidInfo", @"Get-PlayListInfo", @"Get-ChanInfo")]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"InfoFromYtDlp")]
public class GetInfoFromYtDlp : BaseVidCmdLet
{
	[System.Management.Automation.Parameter(HelpMessage = @"This can be any mix of ID values and URLs.  Other types aren't allowed.", Mandatory = true, Position =
		1, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
	public System.Collections.Generic.IEnumerable<object>? WhatToObtain
	{
		get;

		set;
	}

	[System.Management.Automation.Parameter(HelpMessage = @"If specified on a playlist or channel (which behaves like a playlist of playlists), the first level " +
		@"will also be downloaded.  Alternatively, you can call the Update member in the object you get.")]
	public System.Management.Automation.SwitchParameter LoadEntriesToo
	{
		get;

		set;
	}

	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<object> AllURLs
		=> lliststrWhatToDownload;

	/// <inheritdoc/>
	protected override bool YtDlpCalledByDerivedClass
		=> true;


	private readonly System.Collections.Generic.LinkedList<string> lliststrWhatToDownload = [];

	private ulong ulCurParam = 1;


	protected override void ProcessRecord()
	{
		base.ProcessRecord();

		if(WhatToObtain is not null)
			foreach(object objCurInput in WhatToObtain)
			{
				if(objCurInput is string strCurInput)
					lliststrWhatToDownload.AddLast(strCurInput);
				else if(objCurInput is System.Uri uriCurInput)
					lliststrWhatToDownload.AddLast(uriCurInput.AbsolutePath);
				else
					WriteWarning(@$"Unable to interpret parameter {ulCurParam}: “{objCurInput}”");

				ulCurParam++;
			}
	}

	protected override void EndProcessing()
	{
		if(IsVerboseOn)
			lliststrWhatToDownload.AddLast(@"--verbose");

		System.Text.Json.JsonElement jsone = Goodies.YtDlpWrapper.YtDlpWrapper.InvokeYtDlpForJSON(LoadEntriesToo, TaskCompletionSound,
			[..lliststrWhatToDownload, ..Opts.AllOpt]).RootOfData ?? throw new System.Exception(@"No data returned by yt-dlp");

		JSON.ObjBase jobData = JSON.ObjBase.Make(jsone);

		if(jobData is JSON.Array jaData)
			foreach(JSON.ObjBase jobjCurElement in jaData.Elements)
				WriteObj(jobjCurElement);
		else if(jobData is JSON.Obj joCurElement)
			WriteObj(joCurElement);
		else
			WriteWarning(@"yt-dlp returned data in an expected JSON object type.");
	}
}