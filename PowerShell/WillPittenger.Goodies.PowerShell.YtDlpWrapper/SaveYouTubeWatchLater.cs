// Ignore Spelling: yt Dlp ytwatchlater sytwl

using WillPittenger.Goodies.Tools.Ext;

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

[System.Management.Automation.Alias(@"sytwl")]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsData.Save, @"YouTubeWatchLater")]
public class SaveYouTubeWatchLater : BaseSaveCmdLet
{
	protected override System.Collections.Generic.IEnumerable<object> AllURLs => 
		[@":ytwatchlater"];
}