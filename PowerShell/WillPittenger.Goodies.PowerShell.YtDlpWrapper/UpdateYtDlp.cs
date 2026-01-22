// Ignore Spelling: yt Dlp uyd

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// Attempts to update yt-dlp.  Depending on where you installed yt-dlp, you might need to provide Administrator privileges.
/// </summary>
[System.Management.Automation.Alias(@"uyd")]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsData.Update, @"YtDlp", DefaultParameterSetName = @"Normal")]
public class UpdateYtDlp : BaseCmdLet
{
	public enum WhichVersionToUpdate : byte
	{
		auto,

		both,

		exeOnly,

		pythonOnly,
	}


	/// <summary>
	/// If set, yt-dlp will be told to get the latest nightly.  Otherwise, it will use the latest version of whatever is installed.
	/// </summary>
	[System.Management.Automation.Parameter(ParameterSetName = @"Nightly", Mandatory = true, HelpMessage = @"If set, yt-dlp will be told to get the latest nightly.  Otherwise, it will use the latest version of whatever is installed.")]
	public System.Management.Automation.SwitchParameter GetNightly
	{
		get;

		set;
	}

	/// <summary>
	/// Specify a name such as a version number to go to that version
	/// </summary>
	[System.Management.Automation.Parameter(ParameterSetName = @"SpecificVersion", Mandatory = true, HelpMessage = @"Specify a name such as a version number to go to that version")]
	public string SpecificVersionName
	{
		get;

		set;
	} = string.Empty;

	public WhichVersionToUpdate UpdateWhichInstall
	{
		get;

		set;
	} = WhichVersionToUpdate.auto;

	/// <summary>
	/// Tests to see if the user has write access to the directory where yt-dlp is installed by creating and deleting a file there.  If it fails, <see cref="EndProcessing"/> will call yt-dlp with elevated privileges.
	/// </summary>
	private static bool DoesUserHaveWriteAccessToWhereYtDlpIsInstalled
	{
		get
		{
			string strYtDlpDir = new Goodies.YtDlpWrapper.YtDlpWrapper(Goodies.YtDlpWrapper.YtDlpWrapper.WhatToInit.exe).YtDlpExe?.DirectoryName ?? throw new System.InvalidProgramException(@"How did yt-dlp get in a directory with no name???");

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

		Goodies.YtDlpWrapper.YtDlpWrapper yt = UpdateWhichInstall switch
		{
			WhichVersionToUpdate.both
				=> new(Goodies.YtDlpWrapper.YtDlpWrapper.WhatToInit.both),

			WhichVersionToUpdate.exeOnly
				=> new(Goodies.YtDlpWrapper.YtDlpWrapper.WhatToInit.exe),

			WhichVersionToUpdate.pythonOnly
				=> new(Goodies.YtDlpWrapper.YtDlpWrapper.WhatToInit.python),

			_
				=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<WhichVersionToUpdate>(UpdateWhichInstall, @"While interpreting what to do"),
		};

		if(DoesUserHaveWriteAccessToWhereYtDlpIsInstalled)
		{
			if(UpdateWhichInstall != WhichVersionToUpdate.pythonOnly && yt.IsExeReady)
				yt.InvokeYtDlp(null, [.. liststrArgsForYtDlp]);
		}
		else
			yt.InvokeYtDlpAsElevatedProcess(null, [.. liststrArgsForYtDlp]);
		if(UpdateWhichInstall != WhichVersionToUpdate.exeOnly && yt.IsPythonReady)
			yt.InstallYtDlpFromPip(GetNightly, OnStdOutDataReceived);

		TaskCompletionSound?.Play();
	}

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