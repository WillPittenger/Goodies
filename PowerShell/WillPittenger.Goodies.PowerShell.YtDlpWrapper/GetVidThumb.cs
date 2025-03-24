// Ignore Spelling: yt Dlp gvth

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// Specialized cmdlet to retrieve URLs of thumbnails of items.  While it references videos in the name, it actually can accept channels and playlists too.  Unlike <see cref="GetInfoFromYtDlp" />, <see cref="GetVidThumb" /> gets data sooner as yt-dlp can send the thumbnail URL up the pipeline immediately rather than waiting for the JSON to be built.  That doesn’t happen until yt-dlp has completely processed that parameter.  So for a playlist, each entry in the playlist can be sent up the pipeline immediately while <see cref="GetInfoFromYtDlp" /> would need to wait for the playlist to be complete.  If you need the thumbnail for a playlist or channel, use <see cref="GetInfoFromYtDlp" />.  <see cref="GetVidThumb" /> returns only thumbnails for the <em><strong>items inside</strong></em> those playlists or channels.  <see cref="GetVidThumb"/> returns all results as instances of <see cref="System.Uri"/>.
/// </summary>
[System.Management.Automation.Alias(@"gvth", @"Get-VideoThumb", @"Get-VideoThumbNail")]
[System.Management.Automation.OutputType(typeof(System.Uri))]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"VidThumb")]
public class GetVidThumb : BaseVidCmdLet
{
	/// <summary>
	/// We need a constructor so the base class knows about hour standard out handler.
	/// </summary>
	public GetVidThumb()
		=> funcStdOutDataReceivedHandler = OnStdOutDataReceived;


	/// <summary>
	/// Describes what you want downloaded.  This can be any mix of ID values, URLs, plus any objects derived from <see cref="Goodies.YtDlpWrapper.BaseObj"/>.  Other types aren't allowed.
	/// </summary>
	[System.Management.Automation.Parameter(HelpMessage = @"Describes what you want downloaded.  This can be any mix of ID values, URLs, plus any objects derived from WillPittenger.Goodies.YtDlpWrapper.BaseObj.  Other types aren't allowed.", Mandatory = true, Position =
		1, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
	public System.Collections.Generic.IEnumerable<object>? WhatToObtain
	{
		get;

		set;
	}

	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<object> WhatToDownLoadInternal
		=> WhatToObtain ?? [];

	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<string> AdditionalYtDLpParams
		=> [@"--get-thumbnail"];


	/// <summary>
	/// Writes the content received from the standard output stream to the pipeline.  yt-dlp only uses standard output for data it returns, in this case, the URLs of thumbnails for the videos.  It's hoped it will be one event per item.
	/// </summary>
	/// <param name="objSender">The sender of the event.  Ignored.</param>
	/// <param name="e">The data needed, found in <see cref="System.Diagnostics.DataReceivedEventArgs.Data"/></param>
	private void OnStdOutDataReceived(object objSender, System.Diagnostics.DataReceivedEventArgs e)
	{
		if(e.Data is string strData && strData.Length > 0)
		{
			try
			{
				WriteObject(new System.Uri(strData ?? ""));
			}
			catch(System.Exception ex)
			{
				WriteWarning(@$"“{strData}” doesn't look like a url: {ex.Message}");

				WriteObject(null);
			}
		}
		else
			WriteObject(null);
	}
}