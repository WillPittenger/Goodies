// Ignore Spelling: astr yt Dlp runas Exe uri

namespace WillPittenger.Goodies.YtDlpWrapper;

using System.Diagnostics;
using Python.Runtime;

public class YtDlpWrapper
{
	public YtDlpWrapper(WhatToInit whatToInit)
	{
		if(whatToInit != WhatToInit.python)
			InitForExe();

		if(whatToInit != WhatToInit.exe)
			InitForPython();
	}

	~YtDlpWrapper()
		=> PythonEngine.Shutdown();


	internal dynamic? yt = null;

	public readonly System.DateOnly doMinYtDlpVer = new(2026, 1, 1);
	public readonly System.Version verMinPython = new(3, 10);


	public enum HowToUseYtDlp
	{
		asExe,
		fromPython,
	}

	public enum WhatToInit : byte
	{
		exe,
		python,
		both,
	}


	public class PythonExeName
	{
		internal PythonExeName(in System.IO.FileInfo fileExeName, in System.IO.FileInfo fileDllName)
		{
			this.fileExeName = fileExeName;
			this.fileDllName = fileDllName;
		}


		public readonly System.IO.FileInfo fileExeName;
		public readonly System.IO.FileInfo fileDllName;


		public class GenericPythonExeName(in System.IO.FileInfo fileExeName, in System.IO.FileInfo fileDllName)
			: PythonExeName(fileExeName, fileDllName);
	}


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

	[System.ComponentModel.ImmutableObject(true)]
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

	[System.ComponentModel.ImmutableObject(true)]
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

	[System.ComponentModel.ImmutableObject(true)]
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

	[System.ComponentModel.ImmutableObject(true)]
	public record YtDlpInfoWithJSON()
		: YtDlpInfo()
	{
		public System.Text.Json.JsonElement? RootOfData
		{
			get;

			internal set;
		} = null;
	}

	public interface ILogger
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Name must match python declaration")]
		void debug(string strMsg);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Name must match python declaration")]
		void warning(string strMsg);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Name must match python declaration")]
		void error(string strMsg);
	}

	public enum ProgressStatuses : byte
	{
		finished,

		error,

		downloading,
	}

	public delegate void DProgressUpdate(in System.IO.FileInfo? fileCur, in ProgressStatuses status, in System.IO.FileInfo fileCurTemp, in long lDownLoadedBytes, in long? lTotalBytes, in long? lTotalEstimatedBytes, in System.TimeSpan? tsETA, in double? dblSpeed, in System.TimeSpan? tsElapsed, in long lFragmentIndex, in long lFragmentCnt, in JSON.Obj jobjOtherDataFields, in JSON.Obj? jobjInfoDict);

	public delegate void DPostProcessorUpdate(in string strPostProcessorName, in System.IO.FileInfo? fileCur, in ProgressStatuses status, in JSON.Obj jobjOtherDataFields, in JSON.Obj jobjInfoDict);

	[System.ComponentModel.ImmutableObject(true)]
	private class ProgressHookTranslator(DProgressUpdate handlerProgressUpdates)
	{
		public void SendNotification(dynamic poData)
		{
			using Py.GILState lockInfo = Py.GIL();

			ProgressStatuses status = poData.GetAttr(@"status")?.ToString() is string strStatus
				? System.Enum.Parse<ProgressStatuses>(strStatus)
				: ProgressStatuses.error;

			System.IO.FileInfo? fileCur = poData.GetAttr(@"filename")?.ToString() is string strFileName ? new(strFileName) : null;
			System.IO.FileInfo? fileCurTemp = poData.GetAttr(@"tmpFilename")?.ToString() is string strTempFileName ? new(strTempFileName) : null;

			System.TimeSpan? tsETA = poData.GetAttr(@"eta") is long lEtaInSeconds ? new System.TimeSpan(0, 0, (int)lEtaInSeconds) : null;
			System.TimeSpan? tsElapsed = poData.GetAttr(@"elapsed") is long lElapsedInSeconds ? new System.TimeSpan(0, 0, (int)lElapsedInSeconds) : null;


			JSON.Obj? jobjInfoDict = null;
			if(poData.HasAttr(@"info_dict"))
			{
				jobjInfoDict = ConvertPythonObjToJSON(poData.GetAttr(@"info_dict"));

				poData.DelAttr(@"info_dict");
			}

			handlerProgressUpdates(fileCur, status, fileCurTemp, (long)poData.GetAttr(@"downloaded_bytes", 0L), (long?)poData.GetAttr(@"total_bytes"), (long?)poData.GetAttr(@"total_bytes_estimate"), tsETA, (double?)poData.GetAttr(@"speed"), tsElapsed, (long)poData.GetAttr(@"fragment_index", 0L), (long)poData.GetAttr(@"fragment_count", 0L), ConvertPythonObjToJSON(poData), jobjInfoDict);
		}
	}

	private class PostProcessorHookTranslator(DPostProcessorUpdate handlerPostProcessorUpdates)
	{
		public void SendNotification(dynamic poData)
		{
			using Py.GILState lockInfo = Py.GIL();

			ProgressStatuses status = poData.GetAttr(@"status")?.ToString() is string strStatus
				? System.Enum.Parse<ProgressStatuses>(strStatus)
				: ProgressStatuses.error;

			System.IO.FileInfo? fileCur = poData.GetAttr(@"filename")?.ToString() is string strFileName ? new(strFileName) : null;

			JSON.Obj? jobjInfoDict = null;
			if(poData.HasAttr(@"info_dict"))
			{
				jobjInfoDict = ConvertPythonObjToJSON(poData.GetAttr(@"info_dict"));

				poData.DelAttr(@"info_dict");
			}

			handlerPostProcessorUpdates(poData.GetAttr(@"postprocessor"), fileCur, status, ConvertPythonObjToJSON(poData), jobjInfoDict);
		}
	}

	public YtDlpInfo InvokeYtDlp(in Sounds.ISound? soundCompletion = null, params string[] astrParams)
		=> InvokeYtDlp(soundCompletion, null, null, astrParams);

	public void InitForExe()
	{
		foreach(string strCurDir in System.Environment.GetEnvironmentVariable(@"path")?.Split(";") ?? [])
		{
			System.IO.FileInfo fileCurYtDlpPossiblePath = new(System.IO.Path.Combine
				(
					strCurDir,
					System.OperatingSystem.IsWindows()
						? @"yt-dlp.exe"
						: @"yt-dlp"
				));

			if(fileCurYtDlpPossiblePath.Exists)
			{
				YtDlpExe = fileCurYtDlpPossiblePath;

				System.Diagnostics.ProcessStartInfo psi = new()
					{
						Arguments = @"--version",
						UseShellExecute = false,
						RedirectStandardOutput = true,
						CreateNoWindow = true,
						FileName = fileCurYtDlpPossiblePath.FullName,
					};

				using(System.Diagnostics.Process? process = System.Diagnostics.Process.Start(psi))
				{
					if(process is null)
						throw new Exceptions.YtDlpException(Exceptions.YtDlpException.Reasons.missingExe, @"Unable to set the version of the YtDlp executable found");

					if(doMinYtDlpVer > System.DateOnly.ParseExact(process.StandardOutput.ReadToEnd().Replace(@"^(\d{4}\.\d{1,2}\.\d{1,2})(\.\d+)?", @"$1"), @"yyyy.MM.dd"))
						throw new Exceptions.YtDlpException(Exceptions.YtDlpException.Reasons.tooOld);
				}

					break;
			}
		}

		YtDlpExe = new(@"yt-dlp");
	}

	public void InitForPython()
	{
		foreach(string strCurDir in System.Environment.GetEnvironmentVariable(@"path")?.Split(";") ?? [])
		{
			System.IO.FileInfo fileCurPythonPossiblePath = new(System.IO.Path.Combine
				(
					strCurDir,
					System.OperatingSystem.IsWindows()
						? @"python.exe"
						: @"python"
				));

			if(!fileCurPythonPossiblePath.Exists)
				fileCurPythonPossiblePath = new(System.IO.Path.Combine
					(
						strCurDir,
						System.OperatingSystem.IsWindows()
							? @"python3.exe"
							: @"python3"
					));

			if(!fileCurPythonPossiblePath.Exists)
				throw new Exceptions.PythonException(Exceptions.PythonException.Reasons.pythonMissing);

			System.Diagnostics.ProcessStartInfo psi = new()
				{
					FileName = fileCurPythonPossiblePath.FullName,
					Arguments = @"-q -V",
					RedirectStandardOutput = true,
					UseShellExecute = false,
				};

			using(System.Diagnostics.Process? process = System.Diagnostics.Process.Start(psi))
				{
					if(process is null)
						throw new Exceptions.PythonException(Exceptions.PythonException.Reasons.failedToStartPython, @"Null process variable");

					System.Version verPython = System.Version.Parse(process.StandardOutput.ReadToEnd());

					if(verPython < verMinPython)
						throw new Exceptions.PythonException(Exceptions.PythonException.Reasons.pythonNotNewEnough);
				}

			psi.Arguments = @"-c ""import sysconfig; print(sysconfig.get_config_var('DLLLIBRARY') or sysconfig.get_config_var('LDLIBRARY'))""";

			System.IO.FileInfo? filePythonDLL = null;
			using (System.Diagnostics.Process? process = System.Diagnostics.Process.Start(psi))
				{
					if(process is null)
						throw new Exceptions.PythonException(Exceptions.PythonException.Reasons.failedToStartPython, @"Null process variable");

					string strLibName = process.StandardOutput.ReadToEnd().Trim();
					filePythonDLL = new(System.IO.Path.Combine(fileCurPythonPossiblePath.FullName, strLibName));
				}

			PythonEngineSpec = new(fileCurPythonPossiblePath, filePythonDLL);
		}
	}

	public YtDlpInfo InvokeYtDlp(Sounds.ISound? soundCompletion = null, in System.Diagnostics.DataReceivedEventHandler? handlerStdOut = null, in System
		.Diagnostics.DataReceivedEventHandler? handlerStdErr = null, params string[] astrParams)
	{
		System.IO.FileInfo fileYtDlpExe = YtDlpExe is null
			? throw new Exceptions.YtDlpException(Exceptions.YtDlpException.Reasons.missingExe)
			: YtDlpExe;

		soundCompletion ??= Sounds.PredefinedSounds.AllPredefinedSounds[Sounds.PredefinedSounds.SoundIDs.tada];

		YtDlpInfo result = new();

		using System.Diagnostics.Process procYtlp = new()
			{
				EnableRaisingEvents = true,
				PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal,
				StartInfo = new(fileYtDlpExe.FullName, astrParams)
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

		soundCompletion.Play();

		return result;
	}

	public YtDlpInfo InvokeYtDlpAsElevatedProcess(in Sounds.ISound? soundCompletion = null, params string[] astrParams)
		=>  InvokeYtDlpAsElevatedProcess(soundCompletion, null, null, astrParams);

	public YtDlpInfo InvokeYtDlpAsElevatedProcess(Sounds.ISound? soundCompletion = null, in System.Diagnostics.DataReceivedEventHandler? handlerStdOut = null, in System.Diagnostics.DataReceivedEventHandler? handlerStdErr = null, params string[] astrParams)
	{
		System.IO.FileInfo fileYtDlpExe = YtDlpExe is null
			? throw new Exceptions.YtDlpException(Exceptions.YtDlpException.Reasons.missingExe)
			: YtDlpExe;

		soundCompletion ??= Sounds.PredefinedSounds.AllPredefinedSounds[Sounds.PredefinedSounds.SoundIDs.tada];

		YtDlpInfo result = new();

		using System.Diagnostics.Process procYtlp = new()
			{
				EnableRaisingEvents = true,
				PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal,
				StartInfo = new(fileYtDlpExe.FullName, astrParams)
				{
					CreateNoWindow = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					WorkingDirectory = System.Environment.CurrentDirectory,
					Verb = @"Runas",
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

		soundCompletion.Play();

		return result;
	}

	public YtDlpInfoWithJSON InvokeYtDlpForJSON(in bool bUpdatePlayListEntries, in Sounds.ISound? soundCompletion = null, params string[] astrParams)
		=> InvokeYtDlpForJSON(bUpdatePlayListEntries, soundCompletion, null, null, astrParams);

	public YtDlpInfoWithJSON InvokeYtDlpForJSON(in bool bUpdatePlayListEntries, Sounds.ISound? soundCompletion = null, System.Diagnostics.DataReceivedEventHandler? handlerStdOut = null, System.Diagnostics.DataReceivedEventHandler? handlerStdErr = null, params string[] astrParams)
	{
		System.IO.FileInfo fileYtDlpExe = YtDlpExe is null
			? throw new Exceptions.YtDlpException(Exceptions.YtDlpException.Reasons.missingExe)
			: YtDlpExe;

		soundCompletion ??= Sounds.PredefinedSounds.AllPredefinedSounds[Sounds.PredefinedSounds.SoundIDs.tada];

		YtDlpInfoWithJSON result = new();

		using System.Diagnostics.Process procYtlp = new()
			{
				EnableRaisingEvents = true,
				PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal,
				StartInfo = new(fileYtDlpExe.FullName, [..astrParams, bUpdatePlayListEntries ? "-J" : "-j"])
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

		Sounds.PredefinedSounds.AllPredefinedSounds[Sounds.PredefinedSounds.SoundIDs.tada].Play();

		return result;
	}

	public void InstallYtDlpFromPip(in bool bUseLatest = false, in System.Diagnostics.DataReceivedEventHandler? handlerStdOut = null, in System.Diagnostics.DataReceivedEventHandler? handlerStdErr = null)
	{
		if(pythonEngineSpec is null)
			throw new Exceptions.PythonException(Exceptions.PythonException.Reasons.pythonMissing);

		string strPreParam = bUseLatest ? @"--pre" : string.Empty;

		using System.Diagnostics.Process procPip = new()
			{
				EnableRaisingEvents = true,
				PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal,
				StartInfo = new(pythonEngineSpec.fileExeName.FullName, $@"-m pip install -U ${strPreParam} ""yt-dlp[default]""")
					{
						CreateNoWindow = true,
						RedirectStandardError = true,
						RedirectStandardOutput = true,
						WorkingDirectory = System.Environment.CurrentDirectory,
					},
			};

		if(handlerStdOut != null)
			procPip.OutputDataReceived += handlerStdOut;
		if(handlerStdErr != null)
			procPip.ErrorDataReceived += handlerStdErr;
		
		using System.Threading.Tasks.Task taskYtDlpRunner = new
			(
				() =>
					{
						procPip.Start();
						procPip.WaitForExit();
					}, System.Threading.Tasks.TaskCreationOptions.PreferFairness | System.Threading.Tasks.TaskCreationOptions.LongRunning
			);
	}

	public void DownLoadWithYtDlpViaPython(System.Collections.Generic.IEnumerable<System.Uri> enumWhatToDownLoad, AllGlobalOpts? opts = null, in ILogger? logger = null, in DProgressUpdate? handlerProgressHook = null, in DPostProcessorUpdate? handlerPostProcessorHook = null)
	{
		if(yt is null)
			throw new Exceptions.PythonException(Exceptions.PythonException.Reasons.ytDlpNotReady);

		using Py.GILState lockInfo = Py.GIL();

		PyDict pdParams = (opts ?? new()).ToPythonDict(this);

		pdParams.SetItem(@"Logger", logger.ToPython());

		using dynamic ydl = yt.YouTubeDL(opts);

		using dynamic plWhatToDownLoad = new PyList();

		foreach(System.Uri uriCur  in enumWhatToDownLoad)
			plWhatToDownLoad.Append(uriCur);

		if(handlerProgressHook is not null)
			ydl.add_progress_hook(new ProgressHookTranslator(handlerProgressHook));
		if(handlerPostProcessorHook is not null)
			ydl.add_postprocessor_hook(new DPostProcessorUpdate(handlerPostProcessorHook));

		ydl.download(plWhatToDownLoad);
	}

	public JSON.ObjBase ExtractDataWithYtDlpViaPython(System.Uri uriWhatToDownLoad, AllGlobalOpts? opts = null, in bool bResolveLinks = false, in ILogger? logger = null, in DProgressUpdate? handlerProgressHook = null, in DPostProcessorUpdate? handlerPostProcessorHook = null, in System.Collections.Generic.IReadOnlyDictionary<string, object>? mapFieldOverrides = null)
	{
		if(yt is null)
			throw new Exceptions.PythonException(Exceptions.PythonException.Reasons.ytDlpNotReady);

		using Py.GILState lockInfo = Py.GIL();

		PyDict pdParams = (opts ?? new()).ToPythonDict(this);
		if(mapFieldOverrides is System.Collections.Generic.IReadOnlyDictionary<string, object> mapSafeFieldOverrides)
			foreach(System.Collections.Generic.KeyValuePair<string, object> kvpCur in mapSafeFieldOverrides)
				pdParams.SetItem(kvpCur.Key, kvpCur.Value.ToPython());

		pdParams.SetItem(@"Logger", logger.ToPython());

		using dynamic ydl = yt.YouTubeDL(opts);

		using dynamic plWhatToDownLoad = new PyList();

		if(handlerProgressHook is not null)
			ydl.add_progress_hook(new ProgressHookTranslator(handlerProgressHook));
		if(handlerPostProcessorHook is not null)
			ydl.add_postprocessor_hook(new DPostProcessorUpdate(handlerPostProcessorHook));

		return ConvertPythonObjToJSON(ydl.extract_info(uriWhatToDownLoad.AbsoluteUri, Download: false, Process: bResolveLinks));
	}

	private static JSON.ObjBase? ConvertPythonObjToJSON(PyObject? poInput)
	{
		using Py.GILState lockInfo = Py.GIL();

		if(poInput is null)
			return null;

		if(poInput is PySequence pseqInput)
		{
			JSON.Array jaOutput = [];

			foreach(dynamic curChild in pseqInput)
				jaOutput.Add(ConvertPythonObjToJSON(curChild));

			return jaOutput;
		}

		if(poInput is PyInt piInput)
			return new JSON.Val(piInput.AsManagedObject(typeof(int)));

		if(poInput is PyNumber pnInput)
			return new JSON.Val(pnInput.AsManagedObject(typeof(double)));

		if(poInput is PyString pstrInput)
			return new JSON.Val(pstrInput.AsManagedObject(typeof(string)));

		JSON.Obj jobjOutput = [];
		if(poInput is PyDict pdInput)
			foreach(dynamic curAttr in pdInput)
				jobjOutput[curAttr.Key] = ConvertPythonObjToJSON(curAttr.Value.AsManagedObject());
		else
			foreach(dynamic curAttrName in poInput.Dir())
			{
				if(curAttrName is not string strCurAttrName)
					continue;

				jobjOutput[curAttrName] = ConvertPythonObjToJSON(poInput.GetAttr(curAttrName).Value);
			}

		return jobjOutput;
	}

	private PythonExeName? pythonEngineSpec = null;


	public System.IO.FileInfo? YtDlpExe
	{
		get;

		set;
	} = null;

	public PythonExeName? PythonEngineSpec
	{
		get
			=> pythonEngineSpec;

		set
		{
			if(pythonEngineSpec is not null)
				PythonEngine.Shutdown();

			if(value is not null)
			{
				pythonEngineSpec = value;

				Runtime.PythonDLL = value.fileDllName.FullName;

				PythonEngine.Initialize();
				PythonEngine.BeginAllowThreads();

				using Py.GILState lockInfo = Py.GIL();

				try
				{
					yt = Py.Import("yt_dlp");
				}
				catch(System.Exception ex)
				{
					throw new Exceptions.PythonException(Exceptions.PythonException.Reasons.ytDlpNotFoundByPython, @"Exception caught and rethrown.", ex);
				}

				if(yt is null)
					throw new Exceptions.PythonException(Exceptions.PythonException.Reasons.ytDlpNotFoundByPython);

				if(yt.version.__verson__ is string strYtDlpVer)
				{
					if(doMinYtDlpVer > System.DateOnly.ParseExact(strYtDlpVer.Replace(@"^(\d{4}\.\d{1,2}\.\d{1,2})(\.\d+)?", @"$1"), @"yyyy.MM.dd"))
						throw new Exceptions.YtDlpException(Exceptions.YtDlpException.Reasons.tooOld);
				}
			}
		}
	}

	public HowToUseYtDlp? HowToUseYt
	{
		get;

		set;
	} = HowToUseYtDlp.fromPython;

	public bool IsExeReady
		=> YtDlpExe is not null;

	public bool IsPythonReady
		=> PythonEngineSpec is not null;
}