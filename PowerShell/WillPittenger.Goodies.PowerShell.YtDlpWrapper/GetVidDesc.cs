// Ignore Spelling: yt Dlp gvd vid liststr

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// Specialized cmdlet to retrieve description of items.  While it references videos in the name, it actually can get accept channels and playlists too.  Unlike <see cref="GetInfoFromYtDlp" />, <see cref="GetVidDesc" /> gets data sooner as yt-dlp can send the description up the pipeline immediately rather than waiting for the JSON to be built.  That doesn’t happen until yt-dlp has completely processed that parameter.  So for a playlist, each entry in the playlist can be sent up the pipeline immediately while <see cref="GetInfoFromYtDlp" /> would need to wait for the playlist to be complete.  If you need the description for a playlist or channel, use <see cref="GetInfoFromYtDlp" />.  <see cref="GetVidDesc" /> returns only descriptions for the <em><strong>items inside</strong></em> those playlists or channels.
/// </summary>
[System.Management.Automation.Alias(@"gvd", @"Get-VideoDescription")]
[System.Management.Automation.OutputType(typeof(string))]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"VidDesc")]
public class GetVidDesc : BaseVidCmdLet
{
	/// <summary>
	/// We need a constructor so the base class knows about hour standard out handler.
	/// </summary>
	public GetVidDesc()
		=> funcStdOutDataReceivedHandler = OnStdOutDataReceived;


	/// <summary>
	/// Describes what you want downloaded.  This can be any mix of ID values, URLs, plus any objects derived from <see cref="Goodies.YtDlpWrapper.BaseObj"/>.  Other types aren't allowed.
	/// </summary>
	[System.Management.Automation.Parameter(HelpMessage = @"Describes what you want downloaded.  This can be any mix of ID values, URLs, plus any objects derived from WillPittenger.Goodies.YtDlpWrapper.BaseObj.  Other types aren't allowed.", Mandatory = true, Position = 1, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
	public System.Collections.Generic.IEnumerable<object>? WhatToObtain
	{
		get;

		set;
	}

	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<object> WhatToDownLoadInternal
		=> WhatToObtain ?? [];

	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<string> AdditionalYtDlpParams
		=> [@"--get-description"];

	protected override System.Collections.Generic.IReadOnlyDictionary<string, object>? PythonParams
		=> new System.Collections.Generic.Dictionary<string, object>()
			{
				[@"forcedescription"] = true,
				[@"noprogress"] = true,
				[@"quiet"] = true,
				[@"simulate"] = true,
			};

	/// <summary>
	/// Writes the content received from the standard output stream to the pipeline.  yt-dlp only uses standard output for data it returns, in this case, the descriptions of videos.  It's hoped it will be one event per item.
	/// </summary>
	/// <param name="objSender">The sender of the event.  Ignored.</param>
	/// <param name="e">The data needed, found in <see cref="System.Diagnostics.DataReceivedEventArgs.Data"/></param>
	private void OnStdOutDataReceived(object objSender, System.Diagnostics.DataReceivedEventArgs e)
		=> WriteObject(e.Data);
}