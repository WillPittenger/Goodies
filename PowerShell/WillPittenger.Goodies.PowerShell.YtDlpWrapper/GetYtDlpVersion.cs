// Ignore Spelling: yt Dlp gydv

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// Obtains the version of yt-dlp that’s indicated by <see cref="Goodies.YtDlpWrapper.YtDlpWrapper.YtDlpExe"/>.  <see cref="GetYtDlpVersion"/> has no paramters.
/// </summary>
[System.Management.Automation.Alias(@"gydv")]
[System.Management.Automation.OutputType(typeof(string))]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"YtDlpVersion")]
public class GetYtDlpVersion : BaseCmdLet
{
	/// <summary>
	/// This is where we call yt-dlp.  We get notified of the result via <see cref="OnStdOutDataReceived(object, System.Diagnostics.DataReceivedEventArgs)"/>.  If yt-dlp writes to the Standard Error stream, that results in a call to <see cref="OnStdErrDataReceived(object, System.Diagnostics.DataReceivedEventArgs)"/> which writes that data to the console.
	/// </summary>
	protected override void EndProcessing()
	{
		base.EndProcessing();

		(YtWrapper ?? new(Goodies.YtDlpWrapper.YtDlpWrapper.WhatToInit.exe)).InvokeYtDlp(null, OnStdOutDataReceived, OnStdErrDataReceived, @"--version", @"--no-cookies-from-browser", @"--no-cookies");
	}


	/// <summary>
	/// Called when data is sent via the Standard Output stream.
	/// </summary>
	/// <param name="objSender">The sender of the data.  Ignored.</param>
	/// <param name="e">Should be the version of yt-dlp</param>
	private void OnStdOutDataReceived(object objSender, System.Diagnostics.DataReceivedEventArgs e)
	{
		if(e.Data is string strYtDlpVersion && strYtDlpVersion.Length > 0)
			WriteObject(strYtDlpVersion);
	}

	/// <summary>
	/// Called when data is sent via the Standard Error stream.  We just write that data to the console.
	/// </summary>
	/// <param name="objSender">The sender of the data.  Ignored.</param>
	/// <param name="e">Should be the version of yt-dlp</param>
	private void OnStdErrDataReceived(object objSender, System.Diagnostics.DataReceivedEventArgs e)
	{
		if(e.Data is string strData)
			System.Console.WriteLine(strData);
	}
}