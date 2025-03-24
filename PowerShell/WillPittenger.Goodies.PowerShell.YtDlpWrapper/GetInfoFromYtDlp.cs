// Ignore Spelling: Yt Dlp Vid gvi gifyd

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// Retrieves the info JSON from yt-dlp for one or more videos specified by <see cref="WhatToObtain"/> and sends them up the pipeline as objects derived from <see cref="Goodies.YtDlpWrapper.BaseObj"/>.
/// </summary>
[System.Management.Automation.OutputType(typeof(Goodies.YtDlpWrapper.Chan), typeof(Goodies.YtDlpWrapper.Vid), typeof(Goodies.YtDlpWrapper.PlayList))]
[System.Management.Automation.Alias(@"gvi", @"gifyd", @"Get-VidInfo", @"Get-PlayListInfo", @"Get-ChanInfo")]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"InfoFromYtDlp")]
public class GetInfoFromYtDlp : BaseVidCmdLet
{
	/// <summary>
	/// This can be any mix of ID values (as strings) and <see cref="System.Uri"/> instances.  Other types aren’t allowed.
	/// </summary>
	[System.Management.Automation.Parameter(HelpMessage = @"This can be any mix of ID values and URLs.  Other types aren't allowed.", Mandatory = true, Position =
		1, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
	public System.Collections.Generic.IEnumerable<object>? WhatToObtain
	{
		get;

		set;
	}

	/// <summary>
	/// If specified on a playlist or channel (which behaves like a playlist of playlists), the first level will also be downloaded.  Alternatively, you can call the Update member in the objects you get.
	/// </summary>
	[System.Management.Automation.Parameter(HelpMessage = @"If specified on a playlist or channel (which behaves like a playlist of playlists), the first level will also be downloaded.  Alternatively, you can call the Update member in the objects you get.")]
	public System.Management.Automation.SwitchParameter LoadEntriesToo
	{
		get;

		set;
	}

	/// <summary>
	/// Infrastructure.  Not used by <see cref="GetInfoFromYtDlp"/> as it overrides <see cref="EndProcessing"/> further.  The base class never gets to call it.
	/// </summary>
	protected override System.Collections.Generic.IEnumerable<object> WhatToDownLoadInternal
		=> WhatToObtain ?? [];


	/// <summary>
	/// Stores the items that were processed and found to contain acceptable items we can get information on.
	/// </summary>
	private readonly System.Collections.Generic.LinkedList<string> lliststrWhatToDownload = [];


	/// <summary>
	/// Records how many items we’ve processed.  This doesn’t indicate how many items were rejected or how many will be passed to yt-dlp.
	/// </summary>
	private ulong ulCurParam = 1;


	/// <summary>
	/// Checks the values to verify we can send them to yt-dlp  Those values are stored in <see cref="lliststrWhatToDownload"/> and provided to the base class via <see cref="WhatToDownLoadInternal"/>.
	/// </summary>
	protected override void ProcessRecord()
	{
		base.ProcessRecord();

		if(WhatToObtain is not null)
			foreach(object objCurInput in WhatToObtain)
			{
				if(objCurInput is string strCurInput)
				{
					lliststrWhatToDownload.AddLast(strCurInput);

					WriteVerbose($@"Input# {ulCurParam} “{strCurInput}” will be sent to yt-dlp.");
				}
				else if(objCurInput is System.Uri uriCurInput)
				{
					lliststrWhatToDownload.AddLast(uriCurInput.AbsolutePath);

					WriteVerbose($@"Input# {ulCurParam} “{uriCurInput.AbsolutePath}” will be sent to yt-dlp.");
				}
				else if(objCurInput is Goodies.YtDlpWrapper.BaseObj bobjCurInput)
				{
					lliststrWhatToDownload.AddLast(bobjCurInput.strID);

					WriteVerbose($@"Input# {ulCurParam} “{bobjCurInput.OriginalURL}” will be sent to yt-dlp.");
				}
				else
					WriteWarning(@$"Unable to interpret input# {ulCurParam}: “{objCurInput}”");

				ulCurParam++;
			}
	}

	/// <summary>
	/// Handles the yt-dlp invoke.  For efficiency reasons, rather than running yt-dlp once per input, we run it only once.  Hence it’s here.  However, this means the info JSON can’t be converted to <see cref="Goodies.YtDlpWrapper.BaseObj"/> instances until yt-dlp returns.  At that point, the converted data will be written to the pipeline.
	/// </summary>
	/// <exception cref="System.Exception">yt-dlp didn't return any data</exception>
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