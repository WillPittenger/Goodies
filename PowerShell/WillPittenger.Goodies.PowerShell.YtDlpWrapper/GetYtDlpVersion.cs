// Ignore Spelling: Dlp

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"YtDlpVersion")]
public class GetYtDlpVersion : System.Management.Automation.Cmdlet
{
	protected override void EndProcessing()
	{
		base.EndProcessing();

		Goodies.YtDlpWrapper.YtDlpWrapper.InvokeYtDlp(@"--version", @"--no-cookies-from-browser", @"--no-cookies");
	}
}