// Ignore Spelling: yt Dlp ytwatchlater sytwl liststr

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// Specialized cmdlet that downloads your YouTube Watch Later playlist.  It doesn't understand any others.  Note: You must set up cookies with one of the following: <see cref="BaseVidCmdLet.AllGlobalOpts.FileSysGroup.CookiesFromBrowser"/> or --cookies-from-browser (using your config file).  Otherwise, this command won’t work.
/// </summary>
[System.Management.Automation.OutputType(typeof(System.IO.FileInfo))]
[System.Management.Automation.Alias(@"sytwl")]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsData.Save, @"YouTubeWatchLater")]
public class SaveYouTubeWatchLater : BaseVidCmdLet
{
	/// <summary>
	/// We need a constructor so the base class knows about hour standard out handler.
	/// </summary>
	public SaveYouTubeWatchLater()
		=> funcStdOutDataReceivedHandler = OnStdOutDataReceived;


	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<object> WhatToDownLoadInternal => 
		[@":ytwatchlater"];

	/// <inheritdoc/>
	protected override void BeginProcessing()
		=> FileNameFmt ??= SessionState.PSVariable.Get(@"YouTubeWatchLaterOutputTemplate").Value as string;

	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<string> AdditionalYtDlpParams
		=> IsWhatIfOn
			? [@"--print", @"filename"]
			: [];


	/// <summary>
	/// Writes the content received from the standard output stream to the pipeline.  yt-dlp only uses standard output for data it returns, in this case, the file names of videos if the caller specified -WhatIf.  It's hoped it will be one event per item.
	/// </summary>
	/// <param name="objSender">The sender of the event.  Ignored.</param>
	/// <param name="e">The data needed, found in <see cref="System.Diagnostics.DataReceivedEventArgs.Data"/></param>
	private void OnStdOutDataReceived(object objSender, System.Diagnostics.DataReceivedEventArgs e)
	{
		if(IsWhatIfOn && e.Data is string strData && strData.Length > 0)
			System.Console.WriteLine($@"What If: yt-dlp would’ve downloaded the file {strData}.  It would’ve created any missing files or, depending on settings, overwritten what was there.");
	}
}