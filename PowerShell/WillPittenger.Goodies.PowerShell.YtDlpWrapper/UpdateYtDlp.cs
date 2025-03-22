// Ignore Spelling: yt Dlp uyd

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

[System.Management.Automation.Alias(@"uyd")]
public class UpdateYtDlp : BaseCmdLet
{
	[System.Management.Automation.Parameter(ParameterSetName = @"Nightly", HelpMessage = @"If set, yt-dlp will be told to get the latest nightly.  Otherwise, it "
		+ @"will use the latest version of whatever is installed.")]
	System.Management.Automation.SwitchParameter GetNightly
	{
		get;

		set;
	}

	[System.Management.Automation.Parameter(ParameterSetName = @"SpecificVersion", HelpMessage = @"Specify a name such as a version number to go to that version")]
	string SpecificVersionName
	{
		get;

		set;
	} = string.Empty;

	/// <inheritdoc/>
	protected override void EndProcessing()
	{
		base.EndProcessing();

		System.Collections.Generic.List<string> liststrArgsForYtDlp =
			GetNightly
				? [@"--update-to", @"nightly"]
				: SpecificVersionName == string.Empty
					? [@"--update-to", SpecificVersionName]
					: [@"-U"];
		liststrArgsForYtDlp.AddRange([@"--no-cookies-from-browser", @"--no-cookies"]);
		if(IsVerboseOn)
			liststrArgsForYtDlp.Add(@"--verbose");

		Goodies.YtDlpWrapper.YtDlpWrapper.InvokeYtDlpAsElevatedProcess(TaskCompletionSound, [..liststrArgsForYtDlp]);
	}
}