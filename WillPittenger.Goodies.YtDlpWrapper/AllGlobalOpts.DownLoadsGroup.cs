// Ignore Spelling: Dlp Vals uri Sel wget Xattr Hls Sys Arounds Vid Fmts Pwd Executables Evt Exe Evts Loc Locs Json Dest Hdrs Langs

namespace WillPittenger.Goodies.YtDlpWrapper;

using Python.Runtime;

using System.Linq;

using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Groups all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#download-options">Downloads Options section of yt-dlp’s readme.md</a>.
	/// </summary>
	[System.ComponentModel.ImmutableObject(true)]
	public sealed class DownLoadsGroup
	{
		public DownLoadsGroup()
		{
		}

		public DownLoadsGroup(in DownLoadsGroup copyThis)
		{
			ConcurrentFragments = new(copyThis.ConcurrentFragments);
			LimitRate = new(copyThis.LimitRate);
			ThrottledRate = new(copyThis.ThrottledRate);
			MaxDownLoads = new(copyThis.MaxDownLoads);
			MaxRetries = new(copyThis.MaxRetries);
			RetrySleep = new(copyThis.RetrySleep);
			SkipUnavailableFragments = new(copyThis.SkipUnavailableFragments);
			KeepFragments = new(copyThis.KeepFragments);
			BufferSize = new(copyThis.BufferSize);
			ResizeBuffer = new(copyThis.ResizeBuffer);
			HttpChunkSize = new(copyThis.HttpChunkSize);
			PlayListOrder = new(copyThis.PlayListOrder);
			LazyPlayList = new(copyThis.LazyPlayList);
			HlsUseMPEGTS = new(copyThis.HlsUseMPEGTS);
			Sections = new(copyThis.Sections);
			WhatToUse = new(copyThis.WhatToUse);
			Args = new(copyThis.Args);
		}


		/// <summary>
		/// Lists all the downloaders that yt-dlp supports as of March 7th, 2025.  You must choose one of the <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c>.
		/// </summary>
		public sealed class SupportedDownLoaders : IOneOptVal
		{
			/// <summary>
			/// Constructs a new instance
			/// </summary>
			/// <param name="strName">The name of the downloader</param>
			private SupportedDownLoaders(in string strName)
				=> this.strName = strName;


			/// <summary>
			/// The name of the downloader
			/// </summary>
			public readonly string strName;


			/// <summary>
			/// Use the aria2c downloader
			/// </summary>
			public static readonly SupportedDownLoaders aria2c = new(@"aria2c");

			/// <summary>
			/// Use the avconv downloader
			/// </summary>
			public static readonly SupportedDownLoaders avconv = new(@"avconv");

			/// <summary>
			/// Use the axel downloader
			/// </summary>
			public static readonly SupportedDownLoaders axel = new(@"axel");

			/// <summary>
			/// Use the curl downloader
			/// </summary>
			public static readonly SupportedDownLoaders curl = new(@"curl");

			/// <summary>
			/// Use ffmpeg for downloading
			/// </summary>
			public static readonly SupportedDownLoaders ffmpeg = new(@"ffmpeg");

			/// <summary>
			/// Use the httpie downloader
			/// </summary>
			public static readonly SupportedDownLoaders httpie = new(@"httpie");

			/// <summary>
			/// Use the wget downloader
			/// </summary>
			public static readonly SupportedDownLoaders wget = new(@"wget");

			/// <summary>
			/// Use the native downloader
			/// </summary>
			public static readonly SupportedDownLoaders native = new(@"native");


			/// <summary>
			/// Returns the value as a <see cref="string"/>.
			/// </summary>
			public string? ValText
				=> strName;
		}

		/// <summary>
		/// Lists the ways in which a playlist can be ordered.  yt-dlp uses separate command line options for <see cref="reversed"/> and <see cref="random"/>, but by using an <see langword="enum"/>, we can group them into a single <see cref="OneOpt{PlayListOrderChoices}"/> instance.
		/// </summary>
		public enum PlayListOrderChoices
		{
			/// <summary>
			/// Specifies the default order.  If a config file specifies --playlist-random, --playlist-reverse, or something like "-I ::-1", that order will take override yt-dlp's default.  Otherwise, yt-dlp will treat this value as <see cref="normal"/>.
			/// </summary>
			@default,

			/// <summary>
			/// Forces yt-dlp to ignore the config files and download playlist entries in the order listed in the playlist.
			/// </summary>
			normal,

			/// <summary>
			/// Download the entries in reverse order.
			/// </summary>
			reversed,

			/// <summary>
			/// Download the entries in random order.
			/// </summary>
			random,
		}

		/// <summary>
		/// Groups some download options that involve retries
		/// </summary>
		public sealed class RetryMaxGroup
		{
			public RetryMaxGroup()
			{
			}

			public RetryMaxGroup(in RetryMaxGroup copyThis)
			{
				Retries = copyThis.Retries;
				FileAccess = copyThis.FileAccess;
				Fragment = copyThis.Fragment;
			}


			/// <summary>
			/// Maximum number of retries allowed before yt-dlp gives up.  If it doesn't have any value, either from here or a config file, yt-dlp assumes 10.  If Retries is left at the default value of <see langword="null"/> and no config file specifies --max-retries, yt-dlp will retry the file 10 times.  Use <see cref="uint.MaxValue"/> to specify that yt-dlp should retry forever.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadMaxRetries), @"Maximum number of retries allowed before yt-dlp gives up.  If it doesn't have any value, either from here or a config file, yt-dlp assumes 10.  If Retries is left at the default value of null and no config file specifies --max-retries, yt-dlp will retry the file 10 times.  Use uint.MaxValue to specify that yt-dlp should retry forever.", typeof(DownLoadsGroup),
				@"--retries")]
			public OneOpt<uint?> Retries
			{
				get;
			} = new(null, @"--retries")
			{
				ValTextLookUp =
					uiCurVal
						=> uiCurVal switch
						{
							null
								=> null,

							uint.MaxValue
								=> "infinite",

							_
								=> uiCurVal.ToString(),
						},

				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal switch
						{
							null
								=> [],

							uint.MaxValue
								=> new DictionaryToOpt()
								{
									[@"retries"] = @"infinite",
								},

							_
								=> new DictionaryToOpt()
								{
									[@"retries"] = opt.CurVal,
								},
						},
			};

			/// <summary>
			/// Number of times to retry in the event of file access errors.  If you leave FileAccess set to <see langword="null"/> and no config file uses --file-access-retries, yt-dlp will act as though FileAccess is 3.  If FileAccess is set to <see cref="uint.MaxValue"/>, yt-dlp will wait for the file to become available.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadMaxFileAccessRetries), @"Number of times to retry in the event of file access errors.  If you leave FileAccess set to null and no config file uses --file-access-retries, yt-dlp will act as though FileAccess is 3.  If FileAccess is set to uint.MaxValue, yt-dlp will wait for the file to become available.", typeof(DownLoadsGroup), @"--file-access-retries")]
			public OneOpt<uint?> FileAccess
			{
				get;
			} = new(null, @"--file-access-retries")
			{
				ValTextLookUp =
					uiCurVal
						=> uiCurVal switch
						{
							null
								=> null,

							uint.MaxValue
								=> "infinite",

							_
								=> uiCurVal.ToString(),
						},

				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal switch
						{
							null
								=> [],

							uint.MaxValue
								=> new DictionaryToOpt()
								{
									[@"file_access_retries"] = @"infinite",
								},

							_
								=> new DictionaryToOpt()
								{
									[@"file_access_retries"] = opt.CurVal,
								},
						},
			};

			/// <summary>
			/// Number of times to retry in the event of HTTP errors involving fragments.  If you leave Fragment set to <see langword="null"/> and no config file uses --fragment-retries, yt-dlp will act as though Fragment is 10.  If FragmentRetries is set to <see cref="uint.MaxValue"/>, yt-dlp will wait for the file to become available.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadMaxFragmentRetries), @"Number of times to retry in the event of HTTP errors involving fragments.  If you leave Fragment set to null and no config file uses --fragment-retries, yt-dlp will act as though Fragment is 10.  If FragmentRetries is set to uint.MaxValue, yt-dlp will wait for the file to become available.", typeof(DownLoadsGroup), @"--fragment-retries")]
			public OneOpt<uint?> Fragment
			{
				get;
			} = new(null, @"--fragment-retries")
			{
				ValTextLookUp =
					uiCurVal
						=> uiCurVal switch
						{
							null
								=> null,

							uint.MaxValue
								=> "infinite",

							_
								=> uiCurVal.ToString(),
						},

				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal switch
						{
							null
								=> [],

							uint.MaxValue
								=> new DictionaryToOpt()
								{
									[@"fragment_retries"] = @"infinite",
								},

							_
								=> new DictionaryToOpt()
								{
									[@"fragment_retries"] = opt.CurVal,
								},
						},
			};


			/// <summary>
			/// Lists all options inside <see cref="RetryMaxGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to build it’s own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					Retries,
					FileAccess,
					Fragment,
				];
		}

		/// <summary>
		/// Groups some options that involve sleep.
		/// </summary>
		public sealed class SleepBeforeRetryGroup
		{
			public SleepBeforeRetryGroup()
			{
			}

			public SleepBeforeRetryGroup(in SleepBeforeRetryGroup copyThis)
			{
				HTTP = copyThis.HTTP;
				Fragment = copyThis.HTTP;
				FileAccess = copyThis.FileAccess;
				Extractor = copyThis.Extractor;
			}


			/// <summary>
			/// Time to sleep in seconds between retries on a http resource.  This can be a <see cref="RetrySleepNumSpecVal"/>, a <see cref="RetrySleepLinearSpecVal"/>, or a <see cref="RetrySleepExpSpecVal"/>.  The default value of <see langword="null"/> allows yt-dlp to sleep as needed unless a config file specifies a http sleep.  Linear (<see cref="RetrySleepLinearSpecVal"/>) and Exponential (<see cref="RetrySleepExpSpecVal"/>) structures provide more options.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadHttpRetrySleep), @"Time to sleep in seconds between retries on a http resource.  This can be a RetrySleepNumSpecVal, a RetrySleepLinearSpecVal, or a RetrySleepExpSpecVal.  The default value of null allows yt-dlp to sleep as needed unless a config file specifies a http sleep.  Linear (RetrySleepLinearSpecVal) and Exponential (RetrySleepExpSpecVal) structures provide more options.",
				typeof(DownLoadsGroup), @"--retry-sleep")]
			public OneOptWithPrefix<BaseRetrySleepSpecVal?> HTTP
			{
				get;
			} = new(null, @"--retry-sleep", @"http")
			{
				PythonParamsGenerator
					= (in opt)
						=> opt.CurVal is BaseRetrySleepSpecVal spec
							? new DictionaryToOpt()
							{
								[@"retry_sleep_functions"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = (object)spec.CalcWaitTime,
								}
							}
							: [],
			};

			/// <summary>
			/// Time to sleep in seconds between retries on a fragment resource.  This can be a <see cref="RetrySleepNumSpecVal"/>, a <see cref="RetrySleepLinearSpecVal"/>, or a <see cref="RetrySleepExpSpecVal"/>.  The default value of <see langword="null"/> allows yt-dlp to sleep as needed unless a config file specifies a fragment sleep.  Linear (<see cref="RetrySleepLinearSpecVal"/>) and Exponential (<see cref="RetrySleepExpSpecVal"/>) structures provide more options.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadFragmentRetrySleep), @"Time to sleep in seconds between retries on a fragment resource.  This can be a RetrySleepNumSpecVal, a RetrySleepLinearSpecVal, or a RetrySleepExpSpecVal.  The default value of null allows yt-dlp to sleep as needed unless a config file specifies a fragment sleep.  Linear (RetrySleepLinearSpecVal) and Exponential (RetrySleepExpSpecVal) structures provide more options.", typeof(DownLoadsGroup), @"--retry-sleep")]
			public OneOptWithPrefix<BaseRetrySleepSpecVal?> Fragment
			{
				get;
			} = new(null, @"--retry-sleep", @"fragment")
			{
				PythonParamsGenerator
					= (in opt)
						=> opt.CurVal is BaseRetrySleepSpecVal spec
							? new DictionaryToOpt()
							{
								[@"retry_sleep_functions"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = (object)spec.CalcWaitTime,
								}
							}
							: [],
			};

			/// <summary>
			/// Time to sleep in seconds between retries on a file resource.  This can be a <see cref="RetrySleepNumSpecVal"/>, a <see cref="RetrySleepLinearSpecVal"/>, or a <see cref="RetrySleepExpSpecVal"/>.  The default value of <see langword="null"/> allows yt-dlp to sleep as needed unless a config file specifies a file sleep.  Linear (<see cref="RetrySleepLinearSpecVal"/>) and Exponential (<see cref="RetrySleepExpSpecVal"/>) structures provide more options.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadFileAccessRetrySleep), @"Time to sleep in seconds between retries on a file resource.  This can be a RetrySleepNumSpecVal, a RetrySleepLinearSpecVal, or a RetrySleepExpSpecVal.  The default value of null allows yt-dlp to sleep as needed unless a config file specifies a file access sleep.  Linear (RetrySleepLinearSpecVal) and Exponential (RetrySleepExpSpecVal) structures provide more options.", typeof(DownLoadsGroup), @"--retry-sleep")]
			public OneOptWithPrefix<BaseRetrySleepSpecVal?> FileAccess
			{
				get;
			} = new(null, @"--retry-sleep", @"file_access")
			{
				PythonParamsGenerator
					= (in opt)
						=> opt.CurVal is BaseRetrySleepSpecVal spec
							? new DictionaryToOpt()
							{
								[@"retry_sleep_functions"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = (object)spec.CalcWaitTime,
								}
							}
							: [],
			};

			/// <summary>
			/// Time to sleep in seconds between retries on a extractor resource.  This can be a <see cref="RetrySleepNumSpecVal"/>, a <see cref="RetrySleepLinearSpecVal"/>, or a <see cref="RetrySleepExpSpecVal"/>.  The default value of <see langword="null"/> allows yt-dlp to sleep as needed unless a config file specifies a extractor sleep.  Linear (<see cref="RetrySleepLinearSpecVal"/>) and Exponential (<see cref="RetrySleepExpSpecVal"/>) structures provide more options.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadExtractorRetrySleep), @"Time to sleep in seconds between retries on a extractor resource.  This can be a RetrySleepNumSpecVal, a RetrySleepLinearSpecVal, or a RetrySleepExpSpecVal.  The default value of null allows yt-dlp to sleep as needed unless a config file specifies a extractor sleep.  Linear (RetrySleepLinearSpecVal) and Exponential (RetrySleepExpSpecVal) structures provide more options.", typeof(DownLoadsGroup), @"--retry-sleep")]
			public OneOptWithPrefix<BaseRetrySleepSpecVal?> Extractor
			{
				get;
			} = new(null, @"--retry-sleep", @"extractor")
			{
				PythonParamsGenerator
					= (in opt)
						=> opt.CurVal is BaseRetrySleepSpecVal spec
							? new DictionaryToOpt()
							{
								[@"retry_sleep_functions"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = (object)spec.CalcWaitTime,
								}
							}
							: [],
			};


			/// <summary>
			/// Lists all the options inside <see cref="SleepBeforeRetryGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					HTTP,
					Fragment,
					FileAccess,
					Extractor,
				];
		}

		/// <summary>
		/// Groups all options regarding which downloader to use for what.
		/// </summary>
		public sealed class ChoicesForDownLoadingGroup
		{
			public ChoicesForDownLoadingGroup()
			{
			}

			public ChoicesForDownLoadingGroup(in ChoicesForDownLoadingGroup copyThis)
			{
				HTTP = copyThis.HTTP;
				FTP = copyThis.FTP;
				M3U8 = copyThis.M3U8;
				DASH = copyThis.DASH;
				RSTP = copyThis.RSTP;
				RTMP = copyThis.RTMP;
				MMS = copyThis.MMS;
			}


			/// <summary>
			/// Name of a downloader to use for HTTP.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForHTTP), @"Name of a downloader to use for HTTP.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
			public OneOptWithPrefix<SupportedDownLoaders?> HTTP
			{
				get;
			} = new(null, @"--downloader", @"http", @"external_downloader");

			/// <summary>
			/// Name of a downloader to use for FTP.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForFTP), @"Name of a downloader to use for FTP.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
			public OneOptWithPrefix<SupportedDownLoaders?> FTP
			{
				get;
			} = new(null, @"--downloader", @"ftp", @"external_downloader");

			/// <summary>
			/// Name of a downloader to use for m3u8 files.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForM3U8), @"Name of a downloader to use for m3u8.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
			public OneOptWithPrefix<SupportedDownLoaders?> M3U8
			{
				get;
			} = new(null, @"--downloader", @"m3u8", @"external_downloader");

			/// <summary>
			/// Name of a downloader to use for DASH.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForDASH), @"Name of a downloader to use for DASH.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
			public OneOptWithPrefix<SupportedDownLoaders?> DASH
			{
				get;
			} = new(null, @"--downloader", @"dash", @"external_downloader");

			/// <summary>
			/// Name of a downloader to use for RSTP.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForRSTP), @"Name of a downloader to use for RSTP.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
			public OneOptWithPrefix<SupportedDownLoaders?> RSTP
			{
				get;
			} = new(null, @"--downloader", @"rstp", @"external_downloader");

			/// <summary>
			/// Name of a downloader to use for RTMP.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForRTMP), @"Name of a downloader to use for RTMP.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
			public OneOptWithPrefix<SupportedDownLoaders?> RTMP
			{
				get;
			} = new(null, @"--downloader", @"rtmp", @"external_downloader");

			/// <summary>
			/// Name of a downloader to use for MMS.  You can get a list of known supported values from the <see cref="SupportedDownLoaders"/> class.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadWhatToUseForMMS), @"Name of a downloader to use for MMS.  You can get a list of known supported values from the SupportedDownLoaders class.", typeof(DownLoadsGroup), @"--downloader")]
			public OneOptWithPrefix<SupportedDownLoaders?> MMS
			{
				get;
			} = new(null, @"--downloader", @"mms", @"external_downloader");


			/// <summary>
			/// Lists all options inside <see cref="ChoicesForDownLoadingGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					HTTP,
					FTP,
					M3U8,
					DASH,
					RSTP,
					RTMP,
					MMS,
				];
		}

		/// <summary>
		/// Groups all options that send yt-dlp arguments for a downloader.
		/// </summary>
		public sealed class ArgsForDownLoaderGroup
		{
			public ArgsForDownLoaderGroup()
			{
			}

			public ArgsForDownLoaderGroup(in ArgsForDownLoaderGroup copyThis)
			{
				Def = copyThis.Def;
				Native = copyThis.Native;
				Aria2c = copyThis.Aria2c;
				Avconv = copyThis.Avconv;
				Axel = copyThis.Axel;
				Curl = copyThis.Curl;
				FFMPEG = copyThis.FFMPEG;
				HTTPIE = copyThis.HTTPIE;
				Wget = copyThis.Wget;
			}


			/// <summary>
			/// Give these arguments to all downloaders.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoaderArgsDef), @"Give these arguments to all downloaders.", typeof(DownLoadsGroup), @"--downloader-args")]
			public OneOptWithPrefix<string> Def
			{
				get;
			} = new(string.Empty, @"--downloader-args", string.Empty, @"external_downloader_args");

			/// <summary>
			/// Give these arguments to the native downloader.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsNative), @"Give these arguments to the native downloader.", typeof(DownLoadsGroup), @"--downloader-args")]
			public OneOptWithPrefix<string> Native
			{
				get;
			} = new(string.Empty, @"--downloader-args", @"native", @"external_downloader_args");

			/// <summary>
			/// Give these arguments to the aria2c downloader.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsAria2c), @"Give these arguments to the aria2c downloader.", typeof(DownLoadsGroup),
				@"--downloader-args")]
			public OneOptWithPrefix<string> Aria2c
			{
				get;
			} = new(string.Empty, @"--downloader-args", @"aria2c", @"external_downloader_args");

			/// <summary>
			/// Give these arguments to the avconv downloader.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsAvconv), @"Give these arguments to the avconv downloader.", typeof(DownLoadsGroup),
				@"--downloader-args")]
			public OneOptWithPrefix<string> Avconv
			{
				get;
			} = new(string.Empty, @"--downloader-args", @"avconv", @"external_downloader_args");

			/// <summary>
			/// Give these arguments to the axel downloader.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsAxel), @"Give these arguments to the axel downloader.", typeof(DownLoadsGroup),
				@"--downloader-args")]
			public OneOptWithPrefix<string> Axel
			{
				get;
			} = new(string.Empty, @"--downloader-args", @"axel", @"external_downloader_args");

			/// <summary>
			/// Give these arguments to the curl downloader.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsCurl), @"Give these arguments to the curl downloader.", typeof(DownLoadsGroup),
				@"--downloader-args")]
			public OneOptWithPrefix<string> Curl
			{
				get;
			} = new(string.Empty, @"--downloader-args", @"curl", @"external_downloader_args");

			/// <summary>
			/// Give these arguments to the ffmpeg downloader.  For this downloader, you can use the same syntax as for <see cref="PostProcessingGroup.PostProcessorArgs"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsFFMPEG), @"Give these arguments to the ffmpeg downloader.  For this downloader, you can use the same " +
				@"syntax as for PostProcessing.PostProcessingArgs.", typeof(DownLoadsGroup), @"--downloader-args")]
			public OneOptWithPrefix<string> FFMPEG
			{
				get;
			} = new(string.Empty, @"--downloader-args", @"ffmpeg", @"external_downloader_args");

			/// <summary>
			/// Give these arguments to the httpie downloader.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsHTTPIE), @"Give these arguments to the httpie downloader.", typeof(DownLoadsGroup),
				@"--downloader-args")]
			public OneOptWithPrefix<string> HTTPIE
			{
				get;
			} = new(string.Empty, @"--downloader-args", @"httpie", @"external_downloader_args");

			/// <summary>
			/// Give these arguments to the wget downloader.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strDownLoadArgsWget), @"Give these arguments to the wget downloader.", typeof(DownLoadsGroup),
				@"--downloader-args")]
			public OneOptWithPrefix<string> Wget
			{
				get;
			} = new(string.Empty, @"--downloader-args", @"wget", @"external_downloader_args");


			/// <summary>
			/// Lists all options inside <see cref="ArgsForDownLoaderGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build their own list.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					Def,
					Native,
					Aria2c,
					Avconv,
					Axel,
					Curl,
					FFMPEG,
					HTTPIE,
					Wget,
				];
		}


		/// <summary>
		/// Number of fragments of a dash/hlsnative video that should be downloaded concurrently.  The default value of <see langword="null"/> allows yt-dlp to use its default which is <c>1</c> unless a config file overrides that with --concurrent-fragments.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadConcurrentFragments), @"Number of fragments of a dash/hlsnative video that should be downloaded concurrently.  The default value of null allows yt-dlp to use its default which is 1 unless a config file overrides that.", typeof(DownLoadsGroup),
			@"--concurrent-fragments")]
		public OneOpt<uint?> ConcurrentFragments
		{
			get;
		} = new(null, @"--concurrent-fragments", @"concurrent_fragment_downloads");

		/// <summary>
		/// Maximum download rate in bytes per second.  The default value of <see langword="null"/> allows yt-dlp to use unlimited bandwidth unless --limit-rate is specified in a config file.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadLimitRate), @"Maximum download rate in bytes per second.  The default value of null allows yt-dlp to use unlimited bandwidth unless --limit-rate is specified in a config file.", typeof(DownLoadsGroup), @"--limit-rate")]
		public OneOpt<ulong?> LimitRate
		{
			get;
		} = new(null, @"--limit-rate", @"ratelimit");

		/// <summary>
		/// Minimum download rate in bytes per second below which throttling is assumed and the video data is re-extracted.  The default value of <see langword="null"/> allows yt-dlp to have no minimum bandwidth unless --throttled-rate is specified in a config file.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadThrottledRate), @"Minimum download rate in bytes per second below which throttling is assumed and the video data is re-extracted.  The default value of null allows yt-dlp to have no minimum bandwidth unless --throttled-rate is specified in a config file.", typeof(DownLoadsGroup), @"--throttled-rate")]
		public OneOpt<ulong?> ThrottledRate
		{
			get;
		} = new(null, @"--throttled-rate", @"throttledratelimit");

		/// <summary>
		/// Abort after downloading the specified number of files.  The default value of <see langword="null"/> causes yt-dlp to download everything unless a config file specifies --max-downloads.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadsMax), @"Abort after downloading the specified number of files.  The default value of null causes yt-dlp to download everything unless a config file specifies --max-downloads.", typeof(DownLoadsGroup), @"--max-downloads")]
		public OneOpt<ulong?> MaxDownLoads
		{
			get;
		} = new(null, @"--max-downloads", @"max_downloads");

		/// <summary>
		/// Just a group of options that involve retrying some form of resource.
		/// </summary>
		public RetryMaxGroup MaxRetries
		{
			get;
		} = new();

		/// <summary>
		/// Groups a series of options involving how long to sleep before a retry.  All involve the command line option --retry-sleep.
		/// </summary>
		public SleepBeforeRetryGroup RetrySleep
		{
			get;
		} = new();

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will skip any fragments that aren’t available.  If <see langword="false"/>, it will retry them or abort depending on the value in <see cref="RetryMaxGroup.Fragment"/>.  If you leave this at the default value of <see langword="null"/>, yt-dlp will look at the config files for a value.  If none is found, it's as though <see cref="SkipUnavailableFragments"/> is true.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadSkipUnavailableFragments), @"If true, yt-dlp will skip any fragments that aren't available.  If false, it will retry them or abort depending on the value in RetryMaxGroup.Fragment.  If you leave this at the default value of null, yt-dlp will look at the config files for a value.  If none is found, it's as though SkipUnavailableFragments is true.", typeof(DownLoadsGroup),
			@"--skip-unavailable-fragments", @"--no-skip-unavailable-fragments")]
		public ThreeWayOpt SkipUnavailableFragments
		{
			get;
		} = new(@"--skip-unavailable-fragments", @"--no-skip-unavailable-fragments", @"skip_unavailable_fragments", ThreeWayOpt.WhichValsToSendToPythonChoices.falseOnly);

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will leave fragment files after the full file is downloaded.  If <see langword="false"/>, it will delete them.  The default value of <see langword="null"/> is treated by yt-dlp as though you said <see langword="false"/> unless the config file species --keep-fragments or --no-keep-fragments.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadKeepFragments), @"If true, yt-dlp will leave fragment files after the full file is downloaded.  If false, it will delete them.  The default value of null is treated by yt-dlp as though you said false unless the config file species --keep-fragments or --no-keep-fragments.", typeof(DownLoadsGroup), @"--keep-fragments", @"--no-keep-fragments")]
		public ThreeWayOpt KeepFragments
		{
			get;
		} = new(@"--keep-fragments", @"--no-keep-fragments", @"keep_fragments", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// Size of the download buffer in bytes.  The default value of <see langword="null"/> allows yt-dlp to choose this unless a config file specifies --buffer-size.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadBufferSize), @"Size of the download buffer in bytes.  The default value of null allows yt-dlp to choose this unless a config file specifies --buffer-size.", typeof(DownLoadsGroup), @"--buffer-size")]
		public OneOpt<ulong?> BufferSize
		{
			get;
		} = new(null, @"--buffer-size", @"buffersize");

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will resize the buffer as needed.  If <see langword="false"/>, yt-dlp will leave the buffer size alone.  The default of <see langword="null"/> acts like <see langword="true"/> unless a config file specifies either --resize-buffer or --no-resize-buffer.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadResizeBuffer), @"If true, yt-dlp will resize the buffer as needed.  If false, yt-dlp will leave the buffer size alone.  The default of null acts like true unless a config file specifies either --resize-buffer or --no-resize-buffer.", typeof(DownLoadsGroup), @"--resize-buffer", @"--no-resize-buffer")]
		public ThreeWayOpt ResizeBuffer
		{
			get;
		} = new(@"--resize-buffer", @"--no-resize-buffer", @"noresizebuffer", ThreeWayOpt.WhichValsToSendToPythonChoices.falseReversedOnly);

		/// <summary>
		/// Size of a chunk for chunk-based HTTP downloading.  May be useful for bypassing bandwidth throttling imposed by a web server (experimental)
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadHttpChunkSize), @"Size of a chunk for chunk-based HTTP downloading.  May be useful for bypassing bandwidth throttling imposed by a web server (experimental)", typeof(DownLoadsGroup), @"--http-chunk-size")]
		public OneOpt<ulong?> HttpChunkSize
		{
			get;
		} = new(null, @"--http-chunk-size", @"http_chunk_size");

		/// <summary>
		/// Controls the order in which videos will be downloaded in.  <see cref="PlayListOrderChoices.normal"/> causes the playlist entries to be downloaded in the order listed by the playlist.  <see cref="PlayListOrderChoices.reversed"/> reverses that order.  <see cref="PlayListOrderChoices.random"/> causes yt-dlp to randomly choose the next video.  If you use the default value of <see cref="PlayListOrderChoices.@default"/> and no config file changes the order with --playlist-random, --playlist-reverse, or something like "-I ::-1", yt-dlp will act like <see cref="PlayListOrder"/> is set to <see cref="PlayListOrderChoices.normal"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadPlayListOrder), @"Controls the order in which videos will be downloaded in.  PlayListOrderChoices.normal causes the playlist entries to be downloaded in the order listed by the playlist.  PlayListOrderChoices.reverse reverses that order.  PlayListOrderChoices.random causes yt-dlp to randomly choose the next video.  If you use the default value of PlayListOrderChoices.@default and no config file changes the order with --playlist-random, --playlist-reverse, or something like ""-I ::-1"", yt-dlp will act like PlayListOrder is set to PlayListOrderChoices.normal.", typeof(DownLoadsGroup),
			@"--playlist-random", @"--playlist-items")]
		public OneOpt<PlayListOrderChoices> PlayListOrder
		{
			get;
		} = new(PlayListOrderChoices.@default, string.Empty)
		{
			ParamListGenerator =
				opt
					=> opt.CurVal switch
					{
						PlayListOrderChoices.@default
							=> [],

						PlayListOrderChoices.normal
							=> [@"--playlist-items", @"::1"],

						PlayListOrderChoices.reversed
							=> [@"--playlist-reverse"],

						PlayListOrderChoices.random
							=> [@"--playlist-random"],

						_
							=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<PlayListOrderChoices>(opt.CurVal, @"While selecting parameters for the playlist order"),
					},

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal switch
					{
						PlayListOrderChoices.@default
							=> [],

						PlayListOrderChoices.normal
							=> new DictionaryToOpt()
							{
								[@"playlist_items"] = opt.CurVal,
							},

						PlayListOrderChoices.reversed
							=> new DictionaryToOpt()
							{
								[@"playlistreverse"] = true,
							},

						PlayListOrderChoices.random
							=> new DictionaryToOpt()
							{
								[@"playlistrandom"] = true,
							},

						_
							=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<PlayListOrderChoices>(opt.CurVal, @"While selecting parameters for the playlist order"),
					}
		};

		/// <summary>
		/// If <see langword="true"/>, playlist entries won’t be processed until their information is needed.  If <see langword="false"/>, yt-dlp will download all playlist entries immediately.  If you use the default value of <see langword="null"/>, yt-dlp will act as though you specified <see langword="false"/> unless a config file specifies either --lazy-playlist or --no-lazy-playlist.  Note: The value of <see langword="true"/> disables <see cref="PlayListOrder"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadLazyPlayList), @"If true, playlist entries won’t be processed until their information is needed.  If false, yt-dlp will download all playlist entries immediately.  If you use the default value of null, yt-dlp will act as though you specified false unless a config file specifies either --lazy-playlist or --no-lazy-playlist.  Note: The value of true disables PlayListRandomOrder and PlayListReverseOrder.", typeof(DownLoadsGroup), @"--lazy-playlist", @"--no-lazy-playlist")]
		public ThreeWayOpt LazyPlayList
		{
			get;
		} = new(@"--lazy-playlist", @"--no-lazy-playlist", @"lazy_playlist", ThreeWayOpt.WhichValsToSendToPythonChoices.both);

		/// <summary>
		/// If <see langword="true"/>, yt-dlp uses the MPEG TS format to store videos in.  This improves the ability for some players to play the video while it’s still downloading.  If <see langword="false"/>, the video will be saved straight to the final format prior to any needed post processing that might change the file format.  If you use the default value of <see langword="null"/>, yt-dlp will act as though you used <see langword="true"/> for a live stream and <see langword="false"/> for other videos.  Both defaults though are if a config file specifies either --hls-use-mpegts or --no-hls-use-mpegts.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadHlsUseMPEGTS), @"If true, yt-dlp uses the MPEG TS format to store videos in.  This improves the ability for some players to play the video while it's still downloading.  If false, the video will be saved straight to the final format prior to any needed post processing that might change the file format.  If you use the default value of null, yt-dlp will act as though you used true for a live stream and false for other videos.  Both defaults though are overridden unless the config file specifies either --hls-use-mpegts or --no-hls-use-mpegts.", typeof(DownLoadsGroup), @"--hls-use-mpegts", @"--no-hls-use-mpegts")]
		public ThreeWayOpt HlsUseMPEGTS
		{
			get;
		} = new(@"--hls-use-mpegts", @"--no-hls-use-mpegts", @"hls_use_mpegts", ThreeWayOpt.WhichValsToSendToPythonChoices.both);

		/// <summary>
		/// Download only chapters that match the regular expression.  A “*” prefix denotes time-range instead of chapter.  Negative timestamps are calculated from the end.  “*from-url” can be used to download between the “start_time” and “end_time” extracted from the URL.  Needs ffmpeg.  If you need to download multiple sections, use an array, e.g. <c>[""*10:15-inf"", ""intro""]</c>.  No assistance is provided here beyond the array because of the edge case where this is needed.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strDownLoadSections), @"Download only chapters that match the regular expression.  A “*” prefix denotes time-range instead of chapter.  Negative timestamps are calculated from the end.  “*from-url” can be used to download between the “start_time” and “end_time” extracted from the URL.  Needs ffmpeg.  If you need to download multiple sections, use an array, e.g. [""*10:15-inf"", ""intro""].  No assistance is provided here beyond the array because of the edge case where this is needed.", typeof(DownLoadsGroup), @"--download-sections")]
		public OneOpt<System.Collections.Generic.IReadOnlyCollection<string>> Sections
		{
			get;
		} = new([], @"--download-sections")
		{
			PythonParamsGenerator =
				(in opt)
					=>
				{
					using Py.GILState lockInfo = Py.GIL();

					if(opt.CurVal.Count == 0)
						return new DictionaryToOpt();

					dynamic? result = YtDlpWrapper.yt?.util.parse_chapters(opt.strParamName, opt.CurVal.Select(strCurStr => strCurStr.ToPython()), true);

					return result is null
						? []
						: new DictionaryToOpt()
						{
							[@"download_ranges"] = YtDlpWrapper.yt?.util.download_range_func(result.chapters, result.ranges),
						};
				},
		};

		/// <summary>
		/// Groups all choices about which downloader to use
		/// </summary>
		public ChoicesForDownLoadingGroup WhatToUse
		{
			get;
		} = new();

		/// <summary>
		/// All these options specify what to pass to each downloader
		/// </summary>
		public ArgsForDownLoaderGroup Args
		{
			get;
		} = new();


		/// <summary>
		/// Returns a list of all options inside <see cref="DownLoadsGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				ConcurrentFragments,
				LimitRate,
				ThrottledRate,
				MaxDownLoads,
				..MaxRetries.AllOpt,
				..RetrySleep.AllOpt,
				SkipUnavailableFragments,
				KeepFragments,
				BufferSize,
				ResizeBuffer,
				LazyPlayList,
				HlsUseMPEGTS,
				Sections,
				..WhatToUse.AllOpt,
				..Args.AllOpt,
			];
	}
}