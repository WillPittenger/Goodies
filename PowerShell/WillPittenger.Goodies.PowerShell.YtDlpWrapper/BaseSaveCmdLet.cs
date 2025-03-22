// Ignore Spelling: yt Dlp

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

using Goodies.Tools.Ext;

public abstract class BaseSaveCmdLet : BaseVidCmdLet
{
	[System.Management.Automation.Parameter(ParameterSetName = @"Range")]
	public ulong? FirstItem
	{
		get;

		set;
	} = null;

	[System.Management.Automation.Parameter(ParameterSetName = @"Range")]
	public ulong? LastItem
	{
		get;

		set;
	} = null;

	[System.Management.Automation.Parameter(ParameterSetName = @"SpecificItems", Mandatory = true)]
	public System.Collections.Generic.IEnumerable<ulong>? AllItems
	{
		get;

		set;
	} = null;

	[System.Management.Automation.Alias("o")]
	[System.Management.Automation.Parameter()]
	public string OutputTemplate
	{
		get;

		set;
	}

	protected abstract System.Collections.Generic.IEnumerable<object> AllURLs
	{
		get;
	}


	/// <inheritdoc/>
	protected override void BeginProcessing()
	{
		if(OutputTemplate.Length > 0 && OutputTemplate.IndexOf(':') > 2)
			throw new System.ArgumentException(@$"Prefix detected in -OutputTemplate.  Use Opts.FileSys.TemplatesForOutputFiles.* instead.");

		base.BeginProcessing();
	}

	/// <inheritdoc/>
	protected override void EndProcessing()
	{
		base.EndProcessing();

		System.Collections.Generic.List<string> liststrAllURLs =
			[
				..from object objCurURL in AllURLs
					select objCurURL is string strCurURL
						? strCurURL
						: objCurURL is System.Uri uriCur
							 ? uriCur.AbsolutePath
							 : throw new System.InvalidOperationException(@$"Unexpected type in list of URLs to send to yt-dlp: {objCurURL.GetType()}"),
			];

		if(FirstItem is ulong ulFirstItem)
			liststrAllURLs.AddRange([@"--playlist-start", ulFirstItem.ToString()]);
		if(LastItem is ulong ulLastItem)
			liststrAllURLs.AddRange([@"--playlist-end", ulLastItem.ToString()]);
		if(AllItems is System.Collections.Generic.IEnumerable<ulong> eulAllItems)
			liststrAllURLs.AddRange([@"--playlist-items", eulAllItems.Join(',')]);

		if(OutputTemplate.Length > 0)
			liststrAllURLs.AddRange([@"--output", OutputTemplate]);

		if(IsVerboseOn)
			liststrAllURLs.Add(@"--verbose");
		if(IsWhatIfOn)
			Opts.VerbosityAndSimulation.SimMode.Val = true;

		if(liststrAllURLs.Count > 0)
			Goodies.YtDlpWrapper.YtDlpWrapper.InvokeYtDlp(TaskCompletionSound, [..liststrAllURLs, ..Opts.AllOpt]);
	}
}