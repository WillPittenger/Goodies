// Ignore Spelling: astr Dlp

using System.Diagnostics.CodeAnalysis;

namespace WillPittenger.Goodies.YtDlpWrapper;

public class YtDlpWrapper
{
	[System.ComponentModel.ImmutableObject(true)]
	public record FieldDef(in string strName, in FieldDef.Types type)
	{
		public enum Types
		{
			str,
			@ushort,
			@int,
			@long,
			dbl,
			@bool,
			entryType,
			datetime,
			dateOnly,
			duration,
			fileLoc,
			url,
			array,
			obj,
		}

		public string strName = strName;
		public Types type = type;
	}

	public record GenFieldDef(in FieldDef src, in System.Type typeOutput)
	{
		public FieldDef src = src;
		public System.Type typeOutput = typeOutput;

		public static System.DateOnly GetDateOnlyForField(in string strVal)
			=> System.DateOnly.ParseExact(strVal, @"yyyyMMdd");

		public static System.TimeSpan GetTimeSpanFromDurationField(in long lDuration)
			=> lDuration is <= int.MaxValue and >= int.MinValue
				? new(0, 0, (int)lDuration)
				: throw new System.ArgumentOutOfRangeException(nameof(lDuration), @"Unfortunately, .NET timespans don't support durations that don't fit inside a " +
					@"standard 32-bit int.");

		public static System.IO.FileInfo GetFileInfoFromField(in string strFileLoc)
			=> new(strFileLoc);

		public static System.Uri GetUriFromField(in string strUri)
			=> new(strUri);
	}

	public static class KnownYtDlpFields
	{
		public static readonly FieldDef fieldAvailability = new("availability", FieldDef.Types.str);
		public static readonly FieldDef fieldChanName = new("channel", FieldDef.Types.str);
		public static readonly FieldDef fieldChanFollowerCnt = new("channel_follower_count", FieldDef.Types.@long);
		public static readonly FieldDef fieldChanID = new("channel_id", FieldDef.Types.str);
		public static readonly FieldDef fieldChanIsVerified = new("channel_is_verified", FieldDef.Types.@bool);
		public static readonly FieldDef fieldChanURL = new("channel_url", FieldDef.Types.url);
		public static readonly FieldDef fieldDesc = new("description", FieldDef.Types.str);
		public static readonly FieldDef fieldDispID = new("display_id", FieldDef.Types.str);
		public static readonly FieldDef fieldEpoch = new("epoch", FieldDef.Types.@long);
		public static readonly FieldDef fieldExtractor = new("extractor", FieldDef.Types.str);
		public static readonly FieldDef fieldExtractorKey = new("extractor_key", FieldDef.Types.str);
		public static readonly FieldDef fieldID = new("id", FieldDef.Types.str);
		public static readonly FieldDef fieldOriginalURL = new("original_url", FieldDef.Types.url);
		public static readonly FieldDef fieldProtocol = new("protocol", FieldDef.Types.str);
		public static readonly FieldDef fieldTags = new("tags", FieldDef.Types.array);
		public static readonly FieldDef fieldThumbList = new("thumbnails", FieldDef.Types.array);
		public static readonly FieldDef fieldTitle = new("title", FieldDef.Types.str);
		public static readonly FieldDef fieldUploader = new("uploader", FieldDef.Types.str);
		public static readonly FieldDef fieldUploaderId = new("uploader_id", FieldDef.Types.str);
		public static readonly FieldDef fieldUploaderUrl = new("uploader_url", FieldDef.Types.url);
		public static readonly FieldDef fieldViewCnt = new("view_count", FieldDef.Types.@long);
		public static readonly FieldDef fieldWebPageUrl = new("webpage_url", FieldDef.Types.url);
		public static readonly FieldDef fieldWebPageUrlBaseName = new("webpage_url_basename", FieldDef.Types.str);
		public static readonly FieldDef fieldWebPageUrlDomain = new("webpage_url_domain", FieldDef.Types.str);
		public static readonly FieldDef field_ItemType = new("_type", FieldDef.Types.entryType);
		public static readonly FieldDef field_YtDlpVersion = new("_version", FieldDef.Types.obj);
		public static readonly FieldDef fieldReleaseYear = new("release_year", FieldDef.Types.@int);
		public static readonly FieldDef field__FilesToMove = new("__files_to_move", FieldDef.Types.array);

		public static class Chan
		{
			public static readonly FieldDef fieldPlayListCnt = new("playlist_cnt", FieldDef.Types.@long);
		}

		public static class Playlists
		{
			public static readonly FieldDef fieldEntries = new("entries", FieldDef.Types.array);
			public static readonly FieldDef fieldModifiedDate = new("modified_date", FieldDef.Types.dateOnly);
			public static readonly FieldDef field__LastPlayIndex = new("__last_playlist_index", FieldDef.Types.@long);
		}

		public static class Vid
		{
			public static readonly FieldDef fieldAgeLimit = new("age_limit", FieldDef.Types.@long);
			public static readonly FieldDef fieldAspectRatio = new("aspect_ratio", FieldDef.Types.dbl);
			public static readonly FieldDef fieldAudioBitRate = new("abr", FieldDef.Types.dbl);
			public static readonly FieldDef fieldAutoCaptions = new("automatic_captions", FieldDef.Types.obj);
			public static readonly FieldDef fieldAvgRating = new("average_rating", FieldDef.Types.obj);
			public static readonly FieldDef fieldAvgVidBitRate = new("vbr", FieldDef.Types.dbl);
			public static readonly FieldDef fieldCatList = new("categories", FieldDef.Types.array);
			public static readonly FieldDef fieldChapterList = new("chapters", FieldDef.Types.array);
			public static readonly FieldDef fieldCommentCnt = new("comment_count", FieldDef.Types.@long);
			public static readonly FieldDef fieldDuration = new("duration", FieldDef.Types.duration);
			public static readonly FieldDef fieldDurationStr = new("duration_string", FieldDef.Types.str);
			public static readonly FieldDef fieldDynamicRange = new("dynamic_range", FieldDef.Types.str);
			public static readonly FieldDef fieldExt = new("ext", FieldDef.Types.str);
			public static readonly FieldDef fieldFileName = new("filename", FieldDef.Types.fileLoc);
			public static readonly FieldDef fieldFileSize = new("filesize", FieldDef.Types.@long);
			public static readonly FieldDef fieldFileSizeApprox = new("filesize_approx", FieldDef.Types.@long);
			public static readonly FieldDef fieldFmt = new("format", FieldDef.Types.str);
			public static readonly FieldDef fieldFmtID = new("format_id", FieldDef.Types.str);
			public static readonly FieldDef fieldFmtList = new("formats", FieldDef.Types.array);
			public static readonly FieldDef fieldFmtNote = new("format_note", FieldDef.Types.str);
			public static readonly FieldDef fieldFramesPerSec = new("fps", FieldDef.Types.@long);
			public static readonly FieldDef fieldFullTitle = new("fulltitle", FieldDef.Types.str);
			public static readonly FieldDef fieldHeatMap = new("heatmap", FieldDef.Types.obj);
			public static readonly FieldDef fieldHeight = new("height", FieldDef.Types.@long);
			public static readonly FieldDef fieldIsLive = new("is_live", FieldDef.Types.@bool);
			public static readonly FieldDef fieldLang = new("language", FieldDef.Types.str);
			public static readonly FieldDef fieldLikeCnt = new("like_count", FieldDef.Types.@long);
			public static readonly FieldDef fieldLiveStatus = new("live_status", FieldDef.Types.str);
			public static readonly FieldDef fieldPlayList = new("playlist", FieldDef.Types.str);
			public static readonly FieldDef fieldPlayListAutoNumber = new("playlist_autonumber", FieldDef.Types.@long);
			public static readonly FieldDef fieldPlayListChan = new("playlist_channel", FieldDef.Types.str);
			public static readonly FieldDef fieldPlayListChanId = new("playlist_channel_id", FieldDef.Types.str);
			public static readonly FieldDef fieldPlayListId = new("playlist_id", FieldDef.Types.str);
			public static readonly FieldDef fieldPlayListIndex = new("playlist_index", FieldDef.Types.@long);
			public static readonly FieldDef fieldPlayListTitle = new("playlist_title", FieldDef.Types.str);
			public static readonly FieldDef fieldPlayListUploader = new("playlist_uploader", FieldDef.Types.str);
			public static readonly FieldDef fieldPlayListUploaderID = new("playlist_uploader_id", FieldDef.Types.str);
			public static readonly FieldDef fieldPlayableInEmbed = new("playable_in_embed", FieldDef.Types.@bool);
			public static readonly FieldDef fieldProtocol = new("protocol", FieldDef.Types.str);
			public static readonly FieldDef fieldReleaseDate = new("release_date", FieldDef.Types.dateOnly);
			public static readonly FieldDef fieldReleaseTimeStamp = new("release_timestamp", FieldDef.Types.obj);
			public static readonly FieldDef fieldReleaseYear = new("release_year", FieldDef.Types.@ushort);
			public static readonly FieldDef fieldRequestedDownloads = new("requested_downloads", FieldDef.Types.array);
			public static readonly FieldDef fieldRequestedFmts = new("requested_formats", FieldDef.Types.array);
			public static readonly FieldDef fieldRequestedSubtitles = new("requested_subtitles", FieldDef.Types.array);
			public static readonly FieldDef fieldResolution = new("resolution", FieldDef.Types.str);
			public static readonly FieldDef fieldStreachedRatio = new("stretched_ratio", FieldDef.Types.obj);
			public static readonly FieldDef fieldSubtitles = new("subtitles", FieldDef.Types.obj);
			public static readonly FieldDef fieldThumb = new("thumbnail", FieldDef.Types.url);
			public static readonly FieldDef fieldTimeStamp = new("timestamp", FieldDef.Types.@long);
			public static readonly FieldDef fieldTotalBitRate = new("tbr", FieldDef.Types.dbl);
			public static readonly FieldDef fieldUploadDate = new("upload_date", FieldDef.Types.dateOnly);
			public static readonly FieldDef fieldVidCodec = new("vcodec", FieldDef.Types.str);
			public static readonly FieldDef fieldWasLive = new("was_live", FieldDef.Types.@bool);
			public static readonly FieldDef fieldWidth = new("width", FieldDef.Types.@long);
			public static readonly FieldDef field_FmtSortFields = new("_format_sort_fields", FieldDef.Types.array);
			public static readonly FieldDef field_HasDRM = new("_has_drm", FieldDef.Types.@bool);

			public static class Audio
			{
				public static readonly FieldDef fieldCodec = new("acodec", FieldDef.Types.str);
				public static readonly FieldDef fieldSampleRate = new("asr", FieldDef.Types.@long);
				public static readonly FieldDef fieldChannelCnt = new("audio_channels", FieldDef.Types.@long);
			}

			public static class Chapter
			{
				public static readonly FieldDef fieldStartTime = new("start_time", FieldDef.Types.duration);
				public static readonly FieldDef fieldTitle = new("title", FieldDef.Types.str);
				public static readonly FieldDef fieldEndTime = new("end_time", FieldDef.Types.duration);
			}

			public static class Fmt
			{
				public static readonly FieldDef fieldAudioExt = new("audio_ext", FieldDef.Types.str);
				public static readonly FieldDef fieldColumns = new("columns", FieldDef.Types.@long);
				public static readonly FieldDef fieldContainer = new("container", FieldDef.Types.str);
				public static readonly FieldDef fieldDownloaderOpt = new("downloader_options", FieldDef.Types.obj);
				public static readonly FieldDef fieldExt = new("ext", FieldDef.Types.str);
				public static readonly FieldDef fieldFragments = new("fragments", FieldDef.Types.array);
				public static readonly FieldDef fieldFullName = new("format", FieldDef.Types.str);
				public static readonly FieldDef fieldHasDrm = new("has_drm", FieldDef.Types.@bool);
				public static readonly FieldDef fieldHttpHdrs = new("http_headers", FieldDef.Types.obj);
				public static readonly FieldDef fieldID = new("format_id", FieldDef.Types.str);
				public static readonly FieldDef fieldLangPref = new("language_preference", FieldDef.Types.@long);
				public static readonly FieldDef fieldNote = new("format_note", FieldDef.Types.str);
				public static readonly FieldDef fieldPref = new("preference", FieldDef.Types.obj);
				public static readonly FieldDef fieldQuality = new("quality", FieldDef.Types.dbl);
				public static readonly FieldDef fieldRows = new("rows", FieldDef.Types.@long);
				public static readonly FieldDef fieldSrcPref = new("source_preference", FieldDef.Types.@long);
				public static readonly FieldDef fieldTotalBitRate = new("tbr", FieldDef.Types.dbl);
				public static readonly FieldDef fieldURL = new("url", FieldDef.Types.url);
				public static readonly FieldDef fieldVidExt = new("video_ext", FieldDef.Types.str);

				public static class HttpHdrs
				{
					public static readonly FieldDef fieldAccept = new("Accept", FieldDef.Types.str);
					public static readonly FieldDef fieldAcceptLang = new("Accept-Language", FieldDef.Types.str);
					public static readonly FieldDef fieldSecFetchMode = new("Sec-Fetch-Mode", FieldDef.Types.str);
					public static readonly FieldDef fieldUserAgent = new("User-Agent", FieldDef.Types.str);
				}
			}
		}
	}

	public enum ItemTypes
	{
		chan,
		playList,
		vid,
	}

	public record YtDlpInfo
	{
		internal YtDlpInfo()
		{
		}

		public string StdOutCnts
		{
			get;

			internal set;
		} = "";

		public string StdErrCnts
		{
			get;

			internal set;
		} = "";
	}

	public record YtDlpInfoWithJSON()
		: YtDlpInfo()
	{
		public System.Text.Json.JsonElement? RootOfData
		{
			get;

			internal set;
		} = null;
	}

	public static YtDlpInfo InvokeYtDlp(params string[] astrParams)
		=> InvokeYtDlp(null, null, astrParams);

	public static YtDlpInfo InvokeYtDlp(System.Diagnostics.DataReceivedEventHandler? handlerStdOut = null, System.Diagnostics.DataReceivedEventHandler?
		handlerStdErr = null, params string[] astrParams)
	{
		YtDlpInfo result = new();

		using System.Diagnostics.Process procYtlp = new()
		{
			EnableRaisingEvents = true,
			PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal,
			StartInfo = new(YtDlpExe.FullName, astrParams)
			{
				CreateNoWindow = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				WorkingDirectory = System.Environment.CurrentDirectory,
			},
		};

		if(handlerStdOut != null)
			procYtlp.OutputDataReceived += handlerStdOut;
		if(handlerStdErr != null)
			procYtlp.ErrorDataReceived += handlerStdErr;

		using System.Threading.Tasks.Task taskYtDlpRunner = new
			(
				() =>
				{
					procYtlp.Start();
					procYtlp.WaitForExit();

					result.StdOutCnts = procYtlp.StandardOutput.ReadToEnd();
					result.StdErrCnts = procYtlp.StandardError.ReadToEnd();
				}, System.Threading.Tasks.TaskCreationOptions.PreferFairness | System.Threading.Tasks.TaskCreationOptions.LongRunning
		);

		return result;
	}

	public static YtDlpInfoWithJSON InvokeYtDlpForJSON(bool bUpdatePlayListEntries, params string[] astrParams)
		=> InvokeYtDlpForJSON(bUpdatePlayListEntries, null, null, astrParams);

	public static YtDlpInfoWithJSON InvokeYtDlpForJSON(in bool bUpdatePlayListEntries, System.Diagnostics.DataReceivedEventHandler? handlerStdOut = null, System
		.Diagnostics.DataReceivedEventHandler? handlerStdErr = null, params string[] astrParams)
	{
		YtDlpInfoWithJSON result = new();

		using System.Diagnostics.Process procYtlp = new()
		{
			EnableRaisingEvents = true,
			PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal,
			StartInfo = new(YtDlpExe.FullName, [..astrParams, bUpdatePlayListEntries ? "-J" : "-j"])
			{
				CreateNoWindow = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				WorkingDirectory = System.Environment.CurrentDirectory,
			},
		};

		if(handlerStdOut != null)
			procYtlp.OutputDataReceived += handlerStdOut;
		if(handlerStdErr != null)
			procYtlp.ErrorDataReceived += handlerStdErr;

		using System.Threading.Tasks.Task taskYtDlpRunner = new
			(
				() =>
				{
					procYtlp.Start();
					procYtlp.WaitForExit();

					result.StdOutCnts = procYtlp.StandardOutput.ReadToEnd();
					result.StdErrCnts = procYtlp.StandardError.ReadToEnd();
				}, System.Threading.Tasks.TaskCreationOptions.PreferFairness | System.Threading.Tasks.TaskCreationOptions.LongRunning
		);
		taskYtDlpRunner.Wait();

		result.RootOfData = System.Text.Json.JsonDocument.Parse(result.StdErrCnts, new()
			{
				AllowTrailingCommas = true,
				CommentHandling = System.Text.Json.JsonCommentHandling.Skip,
				MaxDepth = int.MaxValue
			}).RootElement;

		return result;
	}

	public static System.IO.FileInfo YtDlpExe
	{
		get;

		set;
	} = new("yt-dlp");
}