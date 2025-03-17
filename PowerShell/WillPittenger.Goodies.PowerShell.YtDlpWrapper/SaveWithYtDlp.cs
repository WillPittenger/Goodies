// Ignore Spelling: yt Dlp swytdlp

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

[System.Management.Automation.Alias("swytdlp")]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsData.Save, "WithYtDlp")]
public class SaveWithYtDlp : BaseCmdLet
{
	[System.Management.Automation.Parameter(HelpMessage = @"This can be any mix of ID values, URLs, plus anything returned by Get-GetInfoFromYtDlp.  Other types " +
		@"aren't allowed.", Mandatory = true, Position = 1, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
	public System.Collections.Generic.IEnumerable<object> WhatToDownload
	{
		get;

		set;
	}

	[System.Management.Automation.PSDefaultValue(Value = PartsOfUrlsThatCanBeDownloaded.both)]
	[System.Management.Automation.Parameter(HelpMessage = @"If some of the URLs specify both a video and a playlist, use this to control what you get.")]
	public PartsOfUrlsThatCanBeDownloaded DownloadWhatInSpecifiedURLs
	{
		get;

		set;
	} = PartsOfUrlsThatCanBeDownloaded.both;

	[System.Management.Automation.PSDefaultValue(Value = null)]
	[System.Management.Automation.Parameter(HelpMessage = @"Use to specify how yt-dlp generates file names.  See the instructions on how to do that from " +
		@"https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template.")]
	public string? FileNameFmt
	{
		get;

		set;
	} = null;
}