// Ignore Spelling: yt Dlp gvts vid

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// Specialized cmdlet to retrieve durations of items.
/// </summary>
/// <remarks>
/// <para>While it references videos in the name, it actually can accept channels and playlists too.  Unlike <see cref="GetInfoFromYtDlp" />, <see cref="GetVidLength" /> gets data sooner as yt-dlp can send the filename up the pipeline immediately rather than waiting for the JSON to be built.  That doesn’t happen until yt-dlp has completely processed that parameter.  So for a playlist, each entry in the playlist can be sent up the pipeline immediately while <see cref="GetInfoFromYtDlp" /> would need to wait for the playlist to be complete.  <see cref="GetVidLength"/> returns all results as instances of <see cref="System.TimeSpan"/>.  The items can be of type <see cref="string"/>, <see cref="System.Uri"/>, or <see cref="System.IO.FileInfo"/>.  If a item is a <see cref="string"/>, its checked to see if it looks like a URL of the HTTP/HTTPS protocol.  If so, that URL is passed to yt-dlp and the duration obtained from there.  Otherwise, the string is checked to see if it’s an existing file on the local file system with <see cref="System.IO.File.Exists(string?)"/>.  In that case, yt-dlp won’t be used to obtain the length of the video.  Instead, <see cref="GetVidLength"/> tries to use COM to get the video length.</para>
/// </remarks>
[System.Management.Automation.Alias(@"gvts", @"--Get-VideoDuration")]
[System.Management.Automation.OutputType(typeof(System.TimeSpan))]
[System.Management.Automation.Cmdlet(System.Management.Automation.VerbsCommon.Get, @"VidLength")]
public class GetVidLength : BaseVidCmdLet
{
	/// <summary>
	/// We need a constructor so the base class knows about hour standard out handler.
	/// </summary>
	public GetVidLength()
		=> funcStdOutDataReceivedHandler = OnStdOutDataReceived;


	/// <summary>
	/// Describes what you want downloaded.  This can be any mix of ID values, URLs, plus any objects derived from <see cref="Goodies.YtDlpWrapper.BaseObj"/>.  Other types aren't allowed.
	/// </summary>
	[System.Management.Automation.Parameter(HelpMessage = @"Describes what you want downloaded.  This can be any mix of ID values, URLs, plus any objects derived from WillPittenger.Goodies.YtDlpWrapper.BaseObj.  Other types aren't allowed.", Mandatory = true, Position =
		1, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
	public System.Collections.Generic.IEnumerable<object>? WhatToObtain
	{
		get;

		set;
	}


	/// <summary>
	/// Used to access the shell.
	/// </summary>
	private static readonly Windows.Win32.UI.Shell.IShellDispatch2 shell = (Windows.Win32.UI.Shell.IShellDispatch2)new Windows.Win32.UI.Shell.Shell();

	/// <summary>
	/// Used to cache folder objects
	/// </summary>
	private static readonly System.Collections.Generic.Dictionary<System.IO.DirectoryInfo, Windows.Win32.UI.Shell.Folder> mapFolderToShellFolder = [];


	/// <summary>
	/// As defined by COM, this is the index for the length of a video
	/// </summary>
	private const int iIndexForVideoLengthInShellObjDetails = 27;


	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<object> WhatToDownLoadInternal
		=> WhatToObtain ?? [];

	/// <inheritdoc/>
	protected override System.Collections.Generic.IEnumerable<string> AdditionalYtDlpParams
		=> [@"--get-duration"];

	protected override System.Collections.Generic.IReadOnlyDictionary<string, object>? PythonParams
		=> new System.Collections.Generic.Dictionary<string, object>()
		{
			[@"forceduration"] = true,
			[@"noprogress"] = true,
			[@"quiet"] = true,
			[@"simulate"] = true,
		};


	/// <summary>
	/// Called by <see cref="BaseVidCmdLet.ProcessRecord"/> when it encounters a object in <see cref="WhatToDownLoadInternal"/> it doesn’t know how to handle.  The default version just returns <see langword="false"/>.
	/// </summary>
	/// <param name="objCurUnknownInput">The object that <see cref="BaseVidCmdLet.ProcessRecord"/> needs help with</param>
	/// <returns><see langword="true"/> if the item could be handled by <see cref="OnInputOfUnknownTypeReceived(object)"/> and <see langword="false"/> otherwise</returns>
	/// <remarks>
	/// <para>If the object is a <see cref="string"/> and <see cref="OnInputOfUnknownTypeReceived(object)"/> returns <see langword="false"/>, <see cref="BaseVidCmdLet.ProcessRecord"/> assumes yt-dlp knows how to interpret the string.  In all other cases in which <see cref="OnInputOfUnknownTypeReceived(object)"/> returns <see langword="false"/>, <see cref="BaseVidCmdLet.ProcessRecord"/> issues a warning and skips the object.  Note: The default implementation <em>always</em> returns <see langword="false"/>.</para>
	/// <para>The <see cref="GetVidLength"/> override of <see cref="OnInputOfUnknownTypeReceived(object)"/> handles <see cref="System.IO.FileInfo"/> instances if <see cref="System.IO.FileInfo.Exists"/> returns <see langword="true"/>.  If so, it uses COM to get the video length.  Otherwise, it returns <see langword="false"/> which would cause the base class to issue a warning rather than send the entry to yt-dlp.</para>
	/// <para>If the entry is a <see cref="string"/>, <see cref="OnInputOfUnknownTypeReceived(object)"/> uses <see cref="System.IO.File.Exists(string?)"/> to see if the value in the string is the name of an existing file.  If so, it calls itself with the string converted to a <see cref="System.IO.FileInfo"/> and takes the branch mentioned in the previous paragraph.  If not, <see cref="OnInputOfUnknownTypeReceived(object)"/> returns <see langword="false"/>.</para>
	/// </remarks>
	protected override bool OnInputOfUnknownTypeReceived(object objCurUnknownInput)
	{
		if(objCurUnknownInput is System.IO.FileInfo fileCurInput && fileCurInput.Exists)
		{
			WriteObject(Goodies.Data.MetaData.GetMetaDataForFile(fileCurInput));

			return true;
		}
		return objCurUnknownInput is string strCurUnknownInput && System.IO.File.Exists(strCurUnknownInput) && OnInputOfUnknownTypeReceived(new System.IO.FileInfo(strCurUnknownInput));
	}

	/// <summary>
	/// Writes the content received from the standard output stream to the pipeline.  yt-dlp only uses standard output for data it returns, in this case, the lengths of videos.  It's hoped it will be one event per item.
	/// </summary>
	/// <param name="objSender">The sender of the event.  Ignored.</param>
	/// <param name="e">The data needed, found in <see cref="System.Diagnostics.DataReceivedEventArgs.Data"/></param>
	private void OnStdOutDataReceived(object objSender, System.Diagnostics.DataReceivedEventArgs e)
	{
		if(e.Data is string strData && ulong.TryParse(strData, out ulong ulDurationInMilliSeconds))
			WriteObject(System.TimeSpan.FromMilliseconds(ulDurationInMilliSeconds));
		else
			WriteObject(null);
	}

	protected override object ConvertOutputFromYtDlp(in string strFromYtDlp)
		=> System.TimeSpan.FromSeconds(double.Parse(strFromYtDlp));
}