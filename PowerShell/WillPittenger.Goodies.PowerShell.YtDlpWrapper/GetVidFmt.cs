// Ignore Spelling: yt Dlp gvf vid liststr

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// Specialized cmdlet to retrieve available formats of items.  While it references videos in the name, it actually can accept channels and playlists too.  Unlike <see cref="GetInfoFromYtDlp" />, <see cref="GetVidFmt" /> gets data sooner as yt-dlp can send the format information up the pipeline immediately rather than waiting for the JSON to be built.  That doesn’t happen until yt-dlp has completely processed that parameter.  So for a playlist, each entry in the playlist can be sent up the pipeline immediately while <see cref="GetInfoFromYtDlp" /> would need to wait for the playlist to be complete.  If you need the description for a playlist or channel, use <see cref="GetInfoFromYtDlp" />.  <see cref="GetVidFmt"/> uses --get-format which returns data as plain text.  If you need the data in an electronically readable format, use <see cref="GetInfoFromYtDlp"/> which returns instances of <see cref="Goodies.YtDlpWrapper.BaseObj"/>.  If the input of <see cref="GetInfoFromYtDlp"/> turns out to be a playlist or channel, you’ll get back a <see cref="Goodies.YtDlpWrapper.PlayList"/> or <see cref="Goodies.YtDlpWrapper.Chan"/> instance.  In that case, the videos will be inside as long as you set <see cref="GetInfoFromYtDlp.LoadEntriesToo"/> to <see langword="true"/>.  Each video would be a <see cref="Goodies.YtDlpWrapper.Vid"/>.  The <see cref="Goodies.YtDlpWrapper.Vid.AllFmtsById"/> member would contain the formats.
/// </summary>
[System.Management.Automation.Alias(@"gvf", @"Get-VideoFormat")]
[System.Management.Automation.OutputType(typeof(string))]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"VidFmt")]
public class GetVidFmt : BaseVidCmdLet
{
	/// <summary>
	/// We need a constructor so the base class knows about hour standard out handler.
	/// </summary>
	public GetVidFmt()
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
	protected override System.Collections.Generic.IEnumerable<string> AdditionalYtDlpParams
		=> [@"--get-format"];

	protected override System.Collections.Generic.IReadOnlyDictionary<string, object>? PythonParams
		=> new System.Collections.Generic.Dictionary<string, object>()
			{
				[@"listformats"] = true,
			};


	/// <summary>
	/// Writes the content received from the standard output stream to the pipeline.  yt-dlp only uses standard output for data it returns, in this case, the formats of videos.  It's hoped it will be one event per item.
	/// </summary>
	/// <param name="objSender">The sender of the event.  Ignored.</param>
	/// <param name="e">The data needed, found in <see cref="System.Diagnostics.DataReceivedEventArgs.Data"/></param>
	private void OnStdOutDataReceived(object objSender, System.Diagnostics.DataReceivedEventArgs e)
		=> WriteObject(e.Data);
}