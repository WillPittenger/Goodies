// Ignore Spelling: yt Dlp gydv

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

[System.Management.Automation.Alias(@"gydv")]
[System.Management.Automation.OutputType(typeof(string))]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"YtDlpVersion")]
public class GetYtDlpVersion : System.Management.Automation.Cmdlet
{
	protected override void EndProcessing()
	{
		base.EndProcessing();

		Goodies.YtDlpWrapper.YtDlpWrapper.InvokeYtDlp(null, OnStdOutDataReceived, null, @"--version", @"--no-cookies-from-browser", @"--no-cookies");
	}

	private void OnStdOutDataReceived(object objSender, System.Diagnostics.DataReceivedEventArgs e)
	{
		if(e.Data is string strYtDlpVersion)
			WriteObject(strYtDlpVersion);
	}
}