// Ignore Spelling: Cnt Codec Vid Dlp Loc Ip Sel httpie avconv wget rstp rtmp mms Json Fmts Dest Arounds Hdrs bidi avi mkv mov langs Pwd strset llist Xattrs Concat Exe Vals Evts Remuxer Evt api Hls holodex Locs dlp's dateafter datebefore geo xff filesize bcats hlsnative avcov axel xattr xattribute mpegts ftp username firefox vivaldi basictext gnomekeyring kwallet na mtime postprocess bidiv fribidi multistreams lrc srt vtt ja twofactor netrc yt resx postprocessor fixup infojson aac alac flac, vorbis wav postprocessing keyframes jpg png webp selfpromo mpd webloc filepath aiff mka whatif

namespace WillPittenger.Goodies.PowerShell.YtDlpWrapper;

/// <summary>
/// All CmdLets in this module should derive from here.  It declares the <see cref="AllGlobalOpts"/> instance <see cref="Opts"/> parameter property for advanced
/// usage.
/// </summary>
public abstract class BaseCmdLet : System.Management.Automation.PSCmdlet
{
	protected bool IsVerboseOn
		=> MyInvocation.BoundParameters[@"verbose"] is true;

	protected bool IsWhatIfOn
		=> MyInvocation.BoundParameters["whatif"] is true;


	protected void WriteObj(JSON.ObjBase jobDataToWrite)
	{
		if(jobDataToWrite is JSON.Obj joDataToWrite)
		{
			object objValEncountered = joDataToWrite.Values[Goodies.YtDlpWrapper.YtDlpWrapper.KnownYtDlpFields.field_ItemType.strName].val.objVal
				?? throw new System.Exception("Unexpected null value in JSON data for the item type field");

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
						throw new System.InvalidOperationException($"Unable to interpret entry sent back from yt-dlp.");
				}
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