// Ignore Spelling: Cnt Codec Vid Dlp Loc Ip Sel httpie avconv wget rstp rtmp mms Json Fmts Dest Arounds Hdrs bidi avi mkv mov langs Pwd strset llist Xattrs Concat Exe Vals Evts Remuxer Evt api Hls holodex Locs dlp's dateafter datebefore geo xff filesize bcats hlsnative avcov axel xattr xattribute mpegts ftp username firefox vivaldi basictext gnomekeyring kwallet na mtime postprocess bidiv fribidi multistreams lrc srt vtt ja twofactor netrc yt resx postprocessor fixup infojson aac alac flac, vorbis wav postprocessing keyframes jpg png webp selfpromo mpd webloc filepath aiff mka whatif

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// All CmdLets in this module except Get-YtDlpVersion should derive from here.  <see cref="BaseCmdLet"/> simplifies checking if the user wants Verbose or WhatIf
/// and simplifies writing some times of objects either derived from <see cref="JSON.Obj"/> and the object represents a <see
/// cref="Goodies.YtDlpWrapper.BaseObj"/> value or the object is another type derived from <see cref="JSON.ObjBase"/>.
/// </summary>
public abstract class BaseCmdLet : System.Management.Automation.PSCmdlet
{
	/// <summary>
	/// Tests to see if the user activated Verbose.
	/// </summary>
	protected bool IsVerboseOn
		=> MyInvocation.BoundParameters[@"verbose"] is true;

	/// <summary>
	/// Tests if the user activated WhatIf.
	/// </summary>
	protected bool IsWhatIfOn
		=> MyInvocation.BoundParameters[@"whatif"] is true;

	/// <summary>
	/// Specifies the sound to be played when the task completes.
	/// </summary>
	/// <inheritdoc/>
	protected Sounds.ISound? TaskCompletionSound
		=> SessionState.PSVariable.Get(@"TaskCompletionSound").Value as Sounds.ISound;


	/// <summary>
	/// Writes the specified object to the pipeline using <see cref="System.Management.Automation.Cmdlet.WriteObject(object)"/>, but attempting to convert such objects as needed into different formats that make more sense.  For example, if <see cref="WriteObj(JSON.ObjBase)"/> detects <paramref name="jobDataToWrite"/> contains a Channel that could be represented by <see cref="Goodies.YtDlpWrapper.Chan"/>, it converts it.
	/// </summary>
	/// <param name="jobDataToWrite">The object to write</param>
	/// <exception cref="System.Exception">Found an object that’s a yt-dlp data structure, but item type was <see langword="null"/>.</exception>
	/// <exception cref="System.InvalidOperationException"><see cref="WriteObj(JSON.ObjBase)"/> was unable to interpret data sent back from yt-dlp.</exception>
	protected void WriteObj(JSON.ObjBase jobDataToWrite)
	{
		if(jobDataToWrite is JSON.Obj joDataToWrite)
		{
			object objValEncountered = joDataToWrite.Values[Goodies.YtDlpWrapper.YtDlpWrapper.KnownYtDlpFields.field_ItemType.strName].val.objVal
				?? throw new System.Exception(@"Unexpected null value in JSON data for the item type field");

			if(objValEncountered is string strValEncountered)
				switch(System.Enum.Parse<Goodies.YtDlpWrapper.YtDlpWrapper.ItemTypes>(strValEncountered))
				{
					case Goodies.YtDlpWrapper.YtDlpWrapper.ItemTypes.chan:
						WriteObject(new Goodies.YtDlpWrapper.Chan(joDataToWrite));

						break;

					case Goodies.YtDlpWrapper.YtDlpWrapper.ItemTypes.playList:
						WriteObject(new Goodies.YtDlpWrapper.PlayList(joDataToWrite));

						break;

					case Goodies.YtDlpWrapper.YtDlpWrapper.ItemTypes.vid:
						WriteObject(new Goodies.YtDlpWrapper.Vid(joDataToWrite));

						break;

					default:
						throw new System.InvalidOperationException(@"Unable to interpret entry sent back from yt-dlp.");
				}
			else
				WriteObject(jobDataToWrite);
		}
		else if(jobDataToWrite is JSON.Array jaDataToWrite)
			foreach(JSON.ObjBase jobCurEntry in jaDataToWrite.Elements)
				WriteObj(jobCurEntry);
		else if(jobDataToWrite is JSON.Val jvDataToWrite)
			WriteObject(jvDataToWrite);
		else
			WriteObject(jobDataToWrite);
	}
}