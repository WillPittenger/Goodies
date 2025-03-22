// Ignore Spelling: yt Dlp ytwatchlater sytwl

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

[System.Management.Automation.OutputType(typeof(System.IO.FileInfo))]
[System.Management.Automation.Alias(@"sytwl")]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsData.Save, @"YouTubeWatchLater")]
public class SaveYouTubeWatchLater : BaseVidCmdLet
{
	protected override System.Collections.Generic.IEnumerable<object> AllURLs => 
		[@":ytwatchlater"];

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

	protected override void BeginProcessing()
		=> FileNameFmt ??= SessionState.PSVariable.Get(@"YouTubeWatchLaterOutputTemplate").Value as string;

	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<string> AdditionalYtDLpParams
		=> IsWhatIfOn
			? [@"--print", @"filename"]
			: [];
}