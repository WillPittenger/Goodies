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

	[System.Management.Automation.PSDefaultValue(Value = PartsOfUrlsThatCanBeDownloaded.both)]
	[System.Management.Automation.Parameter(HelpMessage = @"If some of the URLs specify both a video and a playlist, use this to control what you get.")]
	public PartsOfUrlsThatCanBeDownloaded DownloadWhatInSpecifiedURLs
	{
		get;

		set;
	} = PartsOfUrlsThatCanBeDownloaded.both;

	[System.Management.Automation.PSDefaultValue(Value = null)]
	[System.Management.Automation.Parameter(HelpMessage = @"Use to specify how yt-dlp generates file names.  See the instructions on how to do that from " +
		@"https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template.  Note: The structure will contain a file field that shows what the file name with "
		+ @"this template would be.  However, if you later save it with a different template, the name could be very different.")]
	public string? FileNameFmt
	{
		get;

		set;
	} = null;


	private readonly System.Collections.Generic.LinkedList<string> lliststrWhatToDownload = [];

	protected override void ProcessRecord()
	{
		base.ProcessRecord();

		int iCurParam = 1;

		if(WhatToObtain is not null)
			foreach(object objCurInput in WhatToObtain)
			{
				if(objCurInput is string strCurInput)
					lliststrWhatToDownload.AddLast(strCurInput);
				else if(objCurInput is System.Uri uriCurInput)
					lliststrWhatToDownload.AddLast(uriCurInput.AbsolutePath);
				else
					WriteWarning(@$"Unable to interpret parameter {iCurParam}: “{objCurInput}”");

				iCurParam++;
			}
	}

	protected override void EndProcessing()
	{
		base.EndProcessing();

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