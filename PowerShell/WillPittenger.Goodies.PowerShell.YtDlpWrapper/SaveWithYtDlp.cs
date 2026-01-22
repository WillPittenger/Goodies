// Ignore Spelling: yt Dlp swyd liststr

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// Attempts to save videos to your local file system using yt-dlp.  If you use -WhatIf, <see cref="SaveWithYtDlp"/> adds --simulate to the parameters sent to yt-dlp and prints a message for each file name.
/// </summary>
[System.Management.Automation.Alias("swyd")]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsData.Save, @"WithYtDlp")]
public class SaveWithYtDlp : BaseVidCmdLet
{
	/// <summary>
	/// We need a constructor so the base class knows about hour standard out handler.
	/// </summary>
	public SaveWithYtDlp()
		=> funcStdOutDataReceivedHandler = OnStdOutDataReceived;


	/// <summary>
	/// Describes what you want downloaded.  This can be any mix of ID values, URLs, plus any objects derived from <see cref="Goodies.YtDlpWrapper.BaseObj"/>.  Other types aren't allowed.
	/// </summary>
	[System.Management.Automation.Parameter(HelpMessage = @"Describes what you want downloaded.  This can be any mix of ID values, URLs, plus any objects derived from WillPittenger.Goodies.YtDlpWrapper.BaseObj.  Other types aren't allowed.", Mandatory = true, Position = 1, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
	public System.Collections.Generic.IEnumerable<object>? WhatToDownLoad
	{
		get;

		set;
	}

	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<object> WhatToDownLoadInternal
		=> WhatToDownLoad ?? [];

	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<string> AdditionalYtDlpParams
		=> IsWhatIfOn
			? [@"--simulate", @"--print", @"filename"]
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