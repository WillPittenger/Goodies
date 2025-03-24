// Ignore Spelling: yt Dlp uyd

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// Attempts to update yt-dlp.  Depending on where you installed yt-dlp, you might need to provide Administrator privileges.
/// </summary>
[System.Management.Automation.Alias(@"uyd")]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsData.Update, @"YtDlp", DefaultParameterSetName = @"Normal")]
public class UpdateYtDlp : BaseCmdLet
{
	/// <summary>
	/// If set, yt-dlp will be told to get the latest nightly.  Otherwise, it will use the latest version of whatever is installed.
	/// </summary>
	[System.Management.Automation.Parameter(ParameterSetName = @"Nightly", Mandatory = true, HelpMessage = @"If set, yt-dlp will be told to get the latest nightly.  Otherwise, it will use the latest version of whatever is installed.")]
	System.Management.Automation.SwitchParameter GetNightly
	{
		get;

		set;
	}

	/// <summary>
	/// Specify a name such as a version number to go to that version
	/// </summary>
	[System.Management.Automation.Parameter(ParameterSetName = @"SpecificVersion", Mandatory = true, HelpMessage = @"Specify a name such as a version number to go to that version")]
	string SpecificVersionName
	{
		get;

		set;
	} = string.Empty;

	/// <summary>
	/// Tests to see if the user has write access to the directory where yt-dlp is installed by creating and deleting a file there.  If it fails, <see cref="EndProcessing"/> will call yt-dlp with elevated privileges.
	/// </summary>
	private static bool DoesUserHaveWriteAccessToWhereYtDlpIsInstalled
	{
		get
		{
			string strYtDlpDir = Goodies.YtDlpWrapper.YtDlpWrapper.YtDlpExe.DirectoryName ?? throw new System.InvalidProgramException(@"How did yt-dlp get in a directory with no name???");

			try
			{
				using System.IO.FileStream stream = System.IO.File.Create(System.IO.Path.Combine(strYtDlpDir, @"testing for write access"), 1, System.IO.FileOptions.DeleteOnClose);
			}
			catch
			{
				return false;
			}

			return true;
		}
	}


	/// <inheritdoc/>
	protected override void EndProcessing()
	{
		base.EndProcessing();

		if(GetNightly)
			WriteVerbose(@"Checking to see if yt-dlp can be updated to the latest nightly.");
		else if(SpecificVersionName == string.Empty)
			WriteVerbose(@"Checking to see if yt-dlp can be updated to the latest version of whatever branch is currently installed.");
		else
			WriteVerbose($@"Checking if yt-dlp can be updated to the branch “{SpecificVersionName}”.");

		System.Collections.Generic.List<string> liststrArgsForYtDlp =
			GetNightly
				? [@"--update-to", @"nightly"]
				: SpecificVersionName == string.Empty
					? [@"--update"]
					: [@"--update-to", SpecificVersionName];
		liststrArgsForYtDlp.AddRange([@"--no-cookies-from-browser", @"--no-cookies"]);
		if(IsVerboseOn)
			liststrArgsForYtDlp.Add(@"--verbose");

		if(DoesUserHaveWriteAccessToWhereYtDlpIsInstalled)
			Goodies.YtDlpWrapper.YtDlpWrapper.InvokeYtDlp(TaskCompletionSound, [.. liststrArgsForYtDlp]);
		else
			Goodies.YtDlpWrapper.YtDlpWrapper.InvokeYtDlpAsElevatedProcess(TaskCompletionSound, [.. liststrArgsForYtDlp]);
	}
}