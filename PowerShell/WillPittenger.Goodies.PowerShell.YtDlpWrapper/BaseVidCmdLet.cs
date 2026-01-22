// Ignore Spelling: Cnt Codec Vid Dlp Loc Ip Sel httpie avconv wget rstp rtmp mms Json Fmts Dest Arounds Hdrs bidi avi mkv mov langs Pwd strset llist Xattrs Concat Exe Vals Evts Remuxer Evt api Hls holodex Locs dlp's dateafter datebefore geo xff filesize bcats hlsnative avcov axel xattr xattribute mpegts ftp username firefox vivaldi basictext gnomekeyring kwallet na mtime postprocess bidiv fribidi multistreams lrc srt vtt ja twofactor netrc yt resx postprocessor fixup infojson aac alac flac, vorbis wav postprocessing keyframes jpg png webp selfpromo mpd webloc filepath aiff mka whatif liststr jobj

using System.Linq;

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

using Tools.Ext;

// ******************************************************************************************************************************************************
// ******************************************************************************************************************************************************
//
// Important: Some strings are provided as raw strings when possible in this file.  These are either yt-dlp options—in which case they must exactly match what
// yt-dlp is looking for—or the string is a default description of an option.  In the later case, the string is represented in Rsrcs.resx and any translation
// should occur there.  If corrections need to be made to the command line strings, consult with Will Pittenger to ensure the change is correct and won’t break
// anything.  If a default description needs to be changed, be sure to also change the en-US resx as that's the default translation.
//
// ******************************************************************************************************************************************************
// ******************************************************************************************************************************************************

public abstract class BaseVidCmdLet : BaseCmdLet
{
	protected BaseVidCmdLet()
		=> funcStdErrDataReceivedHandler = OnStdErrDataReceived;


	/// <summary>
	/// When a URL has both a video ID and a playlist ID, which should it process?  <see cref="PartsOfUrlsThatCanBeDownloaded"/> lets you tell yt-dlp what you want data for and what it can ignore.
	/// </summary>
	public enum PartsOfUrlsThatCanBeDownloaded
	{
		/// <summary>
		/// Process only the video.  If the URL contains a playlist ID, that will be ignored.  Playlist fields will be blank and possible <see langword="null"/> including the playlist channel and ID.
		/// </summary>
		vid,

		/// <summary>
		/// Process only the playlist.  If a video ID is in the URL, information on that video will still be downloaded, but only as an entry in the video.
		/// </summary>
		playlist,

		/// <summary>
		/// Like <see cref="vid"/>, but the playlist fields will now be filled out if a playlist ID is present.
		/// </summary>
		both,
	}

	/// <summary>
	/// If some of the URLs specify both a video and a playlist, use <see cref="DownloadWhatInSpecifiedURLs"/> to control what you get.
	/// </summary>
	[System.Management.Automation.PSDefaultValue(Value = PartsOfUrlsThatCanBeDownloaded.both)]
	[System.Management.Automation.Parameter(HelpMessage = @"If some of the URLs specify both a video and a playlist, use -DownloadWhatInSpecifiedURLs to control what you get.")]
	public PartsOfUrlsThatCanBeDownloaded DownloadWhatInSpecifiedURLs
	{
		get;

		set;
	} = PartsOfUrlsThatCanBeDownloaded.both;

	/// <summary>
	/// Use to specify how yt-dlp generates file names.  See the instructions on how to do that from https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template.  Note: Anything returned by <see cref="GetInfoFromYtDlp"/> will contain a file field that shows what the file name with this template would be.  However, if you later save it with a different template, the name could be very different.
	/// </summary>
	[System.Management.Automation.PSDefaultValue(Value = null)]
	[System.Management.Automation.Parameter(HelpMessage = @"Use to specify how yt-dlp generates file names.  See the instructions on how to do that from https://github.com/yt-dlp/yt-dlp/blob/master/README.md#output-template.  Note: Anything returned by Get-InfoFromYtDlp will contain a file field that shows what the file name with this template would be.  However, if you later save it with a different template, the name could be very different.")]
	public string? FileNameFmt
	{
		get;

		set;
	} = null;

	/// <summary>
	/// If specified, results in yt-dlp ignoring videos in a playlist with indices less than the specified amount.
	/// </summary>
	[System.Management.Automation.Parameter(ParameterSetName = @"Range", HelpMessage = @"If specified, results in yt-dlp ignoring videos in a playlist with indices less than the specified amount.")]
	public ulong? FirstItem
	{
		get;

		set;
	} = null;

	/// <summary>
	/// If specified, results in yt-dlp ignoring videos in a playlist with indices greater than the specified amount.
	/// </summary>
	[System.Management.Automation.Parameter(ParameterSetName = @"Range", HelpMessage = @"If specified, results in yt-dlp ignoring videos in a playlist with indices greater than the specified amount.")]
	public ulong? LastItem
	{
		get;

		set;
	} = null;

	/// <summary>
	/// If specified, results in yt-dlp downloading only the items whose index is explicitly listed by the value you pass.
	/// </summary>
	[System.Management.Automation.Parameter(ParameterSetName = @"SpecificItems", Mandatory = true, HelpMessage = @"If specified, results in yt-dlp downloading only the items whose index is explicitly listed by the value you pass.")]
	public System.Collections.Generic.IEnumerable<ulong>? AllItems
	{
		get;

		set;
	} = null;

	/// <summary>
	/// When implemented by a derived class, specifies the list of URLs to download.
	/// </summary>
	protected abstract System.Collections.Generic.IEnumerable<object>? WhatToDownLoadInternal
	{
		get;
	}

	public Goodies.YtDlpWrapper.AllGlobalOpts AllOpts
	{
		get;

		set;
	} = new();

	[System.Management.Automation.Parameter(HelpMessage = @"If specified, file sizes will be given in old style units based on the power of 2.  If omitted, SI units based on the power of 10 will be used.")]
	public System.Management.Automation.SwitchParameter UseNonSiUnits
	{
		get;

		set;
	}


	/// <summary>
	/// Override when you want to be notified of new data in the standard output from yt-dlp.  The main known use case is to immediately send objects further up the pipeline.  The base version returns <see langword="null"/>.
	/// </summary>
	protected System.Diagnostics.DataReceivedEventHandler? funcStdOutDataReceivedHandler = null;

	/// <summary>
	/// Override when you want to be notified of new data in the standard error from yt-dlp.  As of when this documentation was written, a use case hasn't been found, but support is available anyway.  The base version returns <see langword="null"/>.
	/// </summary>
	protected System.Diagnostics.DataReceivedEventHandler? funcStdErrDataReceivedHandler;

	protected virtual System.Collections.Generic.IReadOnlyDictionary<string, object>? PythonParams
		=> null;


	private readonly System.Collections.Generic.LinkedList<string> lliststrWhatToDownload = [];

	private ulong ulCurParam = 1;


	/// <summary>
	/// This version of <see cref="BeginProcessing"/> verifies the user or caller shouldn’t be using a <see cref="Goodies.YtDlpWrapper.AllGlobalOpts.FileSysGroup.TemplatesForOutputFilesGroup"/> member instead of <see cref="FileNameFmt"/>.
	/// </summary>
	/// <exception cref="System.ArgumentException">A prefix was found in <see cref="FileNameFmt"/></exception>
	/// <inheritdoc/>
	protected override void BeginProcessing()
	{
		if(FileNameFmt is string strFileNameFmt && strFileNameFmt.Length > 0 && strFileNameFmt.IndexOf(':') > 1)
			throw new System.ArgumentException(@$"Prefix detected in -FileNameFmt.  Use Opts.FileSys.TemplatesForOutputFiles.* instead.", @"-FileNameFmt");

		YtWrapper ??= new(Goodies.YtDlpWrapper.YtDlpWrapper.WhatToInit.both);

		base.BeginProcessing();
	}

	/// <summary>
	/// This override of <see cref="ProcessRecord"/> examines the input in <see cref="WhatToDownLoadInternal"/> in order to categorize items by type.  If it recognizes an item, that item is sent to yt-dlp.  Otherwise, items are rejected unless the derived class implements <see cref="OnInputOfUnknownTypeReceived(object)"/> and handles it.  If <see cref="OnInputOfUnknownTypeReceived(object)"/> returns <see langword="true"/>, this override of <see cref="ProcessRecord"/> assumes the derived class handled the item already and it doesn't need to be passed to yt-dlp.  If item was a <see cref="string"/> <see cref="OnInputOfUnknownTypeReceived(object)"/> returns <see langword="false"/>, the item assumed to be something yt-dlp understands.  If the item isn’t a string and <see cref="OnInputOfUnknownTypeReceived(object)"/> returns <see langword="false"/>, a warning will be issued the item wasn’t understood.  Note: The default version of <see cref="OnInputOfUnknownTypeReceived(object)"/> always returns false.  Derived classes that can recognize other types should override it.
	/// </summary>
	/// <remarks>
	/// <para>Below is a complete list of supported types and how it’s interpreted.</para>
	/// <list type="bullet">
	///		<listheader>
	///			<term>Type</term>
	///			<description>What happens</description>
	///		</listheader>
	///		<item>
	///			<term><see cref="string"/></term>
	///			<description>If the item appears to be a HTTP/HTTPS URL, it’s passed to yt-dlp.  If it doesn't look like a HTTP/HTTPS URL, <see cref="OnInputOfUnknownTypeReceived(object)"/> gets called.  If it returns <see langword="false"/>, the item is sent to yt-dlp.  If <see cref="OnInputOfUnknownTypeReceived(object)"/> returns <see langword="true"/>, the item is ignored.</description>
	///		</item>
	///		<item>
	///			<term><see cref="System.Uri"/></term>
	///			<description>The value in <see cref="System.Uri.AbsolutePath"/> gets sent to yt-dlp</description>
	///		</item>
	///		<item>
	///			<term>Anything derived from <see cref="Goodies.YtDlpWrapper.BaseObj"/></term>
	///			<description>The value in <see cref="Goodies.YtDlpWrapper.BaseObj.OriginalURL"/> is sent to yt-dlp.  Note: If you only need to update one <see cref="Goodies.YtDlpWrapper.BaseObj"/>, it can be simpler to call its <see cref="Goodies.YtDlpWrapper.BaseObj.UpdateFields(in JSON.ObjBase, in bool)"/> with <see langword="null"/> for the first parameter.  However, that must call yt-dlp once per item whereas the cmdlets can call yt-dlp just once for all inputs.  Do note this implementation of <see cref="ProcessRecord"/> assumes the contents of the object are out of date.</description>
	///		</item>
	///		<item>
	///			<term>Anything else</term>
	///			<description>What happens depends on the return value from <see cref="OnInputOfUnknownTypeReceived(object)"/>.  If it returns <see langword="false"/>, a warning would be issued to the user.  If <see cref="OnInputOfUnknownTypeReceived(object)"/> returns <see langword="true"/>, it’s assumed the derived class already wrote the result to the pipeline.</description>
	///		</item>
	/// </list>
	/// <para>Because of the way <see cref="ProcessRecord"/> override works and the need to call yt-dlp only once, results may be sent to the pipeline in a different order from the matching input.  Specifically, anything returned by the derived class is written first.  Then yt-dlp is called and its results returned.</para>
	/// </remarks>
	/// <inheritdoc/>
	protected override void ProcessRecord()
	{
		base.ProcessRecord();

		if(WhatToDownLoadInternal is not null)
			foreach(object objCurInput in WhatToDownLoadInternal)
			{
				if(objCurInput is string strCurInput)
				{
					if(strCurInput.StartsWith(@"http://") || strCurInput.StartsWith(@"https://"))
					{
						lliststrWhatToDownload.AddLast(strCurInput);

						WriteVerbose($@"Input# {ulCurParam} “{strCurInput}” will be sent to yt-dlp.");
					}
					else if(!OnInputOfUnknownTypeReceived(objCurInput))
					{
						lliststrWhatToDownload.AddLast(strCurInput);

						WriteVerbose($@"Input# {ulCurParam} “{strCurInput}” will be sent to yt-dlp verbatim.  It may or may not be the ID for a video.  Hopefully, yt-dlp knows what to do with it.");
					}
				}
				else if(objCurInput is System.Uri uriCurInput)
				{
					lliststrWhatToDownload.AddLast(uriCurInput.AbsolutePath);

					WriteVerbose($@"Input# {ulCurParam} “{uriCurInput.AbsolutePath}” will be sent to yt-dlp.");
				}
				else if(objCurInput is Goodies.YtDlpWrapper.BaseObj bobjCurInput)
				{
					lliststrWhatToDownload.AddLast(bobjCurInput.strID);

					WriteVerbose($@"Input# {ulCurParam} “{bobjCurInput.OriginalURL}” will be sent to yt-dlp.");
				}
				else if(!OnInputOfUnknownTypeReceived(objCurInput))
					WriteWarning(@$"Unable to interpret input# {ulCurParam}: “{objCurInput}”");

				ulCurParam++;
			}
	}

	/// <summary>
	/// Called by <see cref="ProcessRecord"/> when it encounters a object in <see cref="WhatToDownLoadInternal"/> it doesn’t know how to handle.  The default version just returns <see langword="false"/>.
	/// </summary>
	/// <param name="objCurUnknownInput">The object that <see cref="ProcessRecord"/> needs help with</param>
	/// <returns><see langword="true"/> if the item could be handled by <see cref="OnInputOfUnknownTypeReceived(object)"/> and <see langword="false"/> otherwise</returns>
	/// <remarks>
	/// <para>If the object is a <see cref="string"/> and <see cref="OnInputOfUnknownTypeReceived(object)"/> returns <see langword="false"/>, <see cref="ProcessRecord"/> assumes yt-dlp knows how to interpret the string.  In all other cases in which <see cref="OnInputOfUnknownTypeReceived(object)"/> returns <see langword="false"/>, <see cref="ProcessRecord"/> issues a warning and skips the object.  Note: The default implementation <em>always</em> returns <see langword="false"/>.</para>
	/// </remarks>
	protected virtual bool OnInputOfUnknownTypeReceived(object objCurUnknownInput)
		=> false;

	/// <summary>
	/// This override of <see cref="EndProcessing"/> provides a automatic means to invoke yt-dlp for most derived classes.  Override it if your derived class needs a specific means of invoking yt-dlp such as the <see cref="o:Goodies.YtDlpWrapper.YtDlpWrapper.InvokeYtDlpForJSON()"/> overloads.  This override handles all the parameters declared by <see cref="BaseVidCmdLet"/> for you.
	/// </summary>
	/// <inheritdoc/>
	protected override void EndProcessing()
	{
		if(lliststrWhatToDownload.Count == 0)
		{
			WriteWarning(@"Nothing to download");

			return;
		}

		System.Collections.Generic.List<string> liststrAllURLs =
			[
				..AdditionalYtDlpParams,
				..lliststrWhatToDownload,
			];

		Goodies.YtDlpWrapper.AllGlobalOpts allOpts = new(AllOpts);

		if(FirstItem is ulong ulFirstItem)
			liststrAllURLs.AddRange([@"--playlist-start", ulFirstItem.ToString()]);
		if(LastItem is ulong ulLastItem)
			liststrAllURLs.AddRange([@"--playlist-end", ulLastItem.ToString()]);
		if(AllItems is System.Collections.Generic.IEnumerable<ulong> eulAllItems)
			liststrAllURLs.AddRange([@"--playlist-items", eulAllItems.Join(',')]);

		if(FileNameFmt is string strFileNameFmt && strFileNameFmt.Length > 0)
			liststrAllURLs.AddRange([@"--output", strFileNameFmt]);

		if(IsVerboseOn)
			liststrAllURLs.Add(@"--verbose");
		if(IsWhatIfOn)
			AllOpts.VerbosityAndSimulation.SimMode.Val = true;

		WriteVerbose($@"Calling yt-dlp with these parameters: {liststrAllURLs.Select(strCurURL => $@"“{strCurURL}”").Join(' ')}");

		Goodies.YtDlpWrapper.YtDlpWrapper ytWrapper = YtWrapper ?? new(Goodies.YtDlpWrapper.YtDlpWrapper.WhatToInit.both);

		if(ytWrapper.IsPythonReady)
		{
			int iParamNum = 0;

			foreach(string strCurURI in lliststrWhatToDownload)
			{
				iParamNum++;

				System.Console.WriteLine(Rsrcs.strParamInfoProgressMsg.Fmt(@"Get-InfoFromYtDlp", iParamNum, strCurURI));

				JSON.ObjBase job = ytWrapper.ExtractDataWithYtDlpViaPython(new(strCurURI), AllOpts, false, Looger, OnProgressUpdateFromYtDlp, OnPostProcessorUpdateFromYtDlp, mapFieldOverrides: PythonParams);
				if(job is JSON.Array jaItems)
					foreach(JSON.ObjBase jobCurChild in jaItems)
						if(jobCurChild is JSON.Obj joCurChild)
							WriteObject(Goodies.YtDlpWrapper.BaseObj.FromJSON(joCurChild));
			}

			TaskCompletionSound?.Play();
		}
		else if(ytWrapper.IsExeReady)
			ytWrapper.InvokeYtDlp
				(
					TaskCompletionSound,
					funcStdOutDataReceivedHandler,
					funcStdErrDataReceivedHandler,
					[
						..liststrAllURLs,
						..AllOpts.AllOpt
					]
				);
	}

	/// <summary>
	/// Specifies more parameters that you want to pass to yt-dlp that aren’t listed in <see cref="Opts"/> and you don't need to override <see cref="EndProcessing"/> for other reasons.
	/// </summary>
	/// <returns></returns>
	protected virtual System.Collections.Generic.IEnumerable<string> AdditionalYtDlpParams
		=> [];


	protected virtual object ConvertOutputFromYtDlp(in string strFromYtDlp)
		=> strFromYtDlp;


	/// <summary>
	/// Used to connect yt-dlp to our console.  The default value of <see cref="funcStdErrDataReceivedHandler"/> is <see cref="OnStdErrDataReceived(object, System.Diagnostics.DataReceivedEventArgs)"/>.  <see cref="BaseVidCmdLet.EndProcessing"/> passes <see cref="funcStdErrDataReceivedHandler"/> to <see cref="o:Goodies.YtDlpWrapper.YtDlpWrapper.InvokeYtDlpAsElevatedProcess()"/>.
	/// </summary>
	/// <param name="objSender">The sender of the event.  Ignored.</param>
	/// <param name="e">The information.  What is needed is in <see cref="System.Diagnostics.DataReceivedEventArgs.Data"/>.</param>
	private void OnStdErrDataReceived(object objSender, System.Diagnostics.DataReceivedEventArgs e)
		=> System.Console.WriteLine(e.Data ?? "");


	protected virtual void OnProgressUpdateFromYtDlp(in System.IO.FileInfo? fileCur, in Goodies.YtDlpWrapper.YtDlpWrapper.ProgressStatuses status, in System.IO.FileInfo fileCurTemp, in long lDownLoadedBytes, in long? lTotalBytes, in long? lTotalEstimatedBytes, in System.TimeSpan? tsETA, in double? dblSpeed, in System.TimeSpan? tsElapsed, in long lFragmentIndex, in long lFragmentCnt, in JSON.Obj jobjOtherDataFields, in JSON.Obj? jobjInfoDict)
	{
	}

	protected virtual void OnPostProcessorUpdateFromYtDlp(in string strPostProcessorName, in System.IO.FileInfo? fileCur, in Goodies.YtDlpWrapper.YtDlpWrapper.ProgressStatuses status, in JSON.Obj jobjOtherDataFields, in JSON.Obj jobjInfoDict)
	{
	}
}