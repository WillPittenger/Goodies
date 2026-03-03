// Ignore Spelling: Dlp Vals uri Sel wget Xattr Hls Sys Arounds Vid Fmts Pwd Executables Evt Exe Evts Loc Locs Json Dest Hdrs Langs

namespace WillPittenger.Goodies.YtDlpWrapper;

using DictionaryToOpt = new(System.Collections).Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Groups all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#filesystem-options">File System Options section of yt-dlp’s readme.md</a>.
	/// </summary>
	[System.ComponentModel.ImmutableObject(true)]
	public sealed class FileSysGroup
	{
		public FileSysGroup()
		{
		}

		public FileSysGroup(in FileSysGroup copyThis)
		{
			Paths = new(copyThis.Paths);
			OutputTemplates = new(copyThis.OutputTemplates);
			PlaceHolderTextInGeneratedFileNames = new(copyThis.PlaceHolderTextInGeneratedFileNames);
			RestrictFileNames = new(copyThis.RestrictFileNames);
			UseWindowsFileNames = new(copyThis.UseWindowsFileNames);
			MaxFileNameLength = new(copyThis.MaxFileNameLength);
			WhenToAllowOverWrites = new(copyThis.WhenToAllowOverWrites);
			ResumePartialDownLoads = new(copyThis.ResumePartialDownLoads);
			UsePartFiles = new(copyThis.UsePartFiles);
			MTime = new(copyThis.MTime);
			DescShouldBeWritten = new(copyThis.DescShouldBeWritten);
			InfoJsonShouldBeWritten = new(copyThis.InfoJsonShouldBeWritten);
			PlayListMetaFilesShouldBeWritten = new(copyThis.PlayListMetaFilesShouldBeWritten);
			CommentsShouldBeWritten = new(copyThis.CommentsShouldBeWritten);
			CookiesFromFile = new(copyThis.CookiesFromFile);
			CookiesFromBrowser = new(copyThis.CookiesFromBrowser);
			CacheDir = new(copyThis.CacheDir);
		}


		/// <summary>
		/// Provides selections on if yt-dlp should overwrite files or not.
		/// </summary>
		[System.ComponentModel.ImmutableObject(true)]
		public sealed class OverWriteChoices : IOneOptVal
		{
			/// <summary>
			/// Constructs a new instance
			/// </summary>
			/// <param name="strName">The name to use.  This is mainly for documentation.</param>
			/// <param name="strParamToPass">The parameter to emit to yt-dlp</param>
			private OverWriteChoices(in string strName, in string strParamToPass)
			{
				this.strName = strName;
				this.strParamToPass = strParamToPass;
			}


			/// <summary>
			/// The name to use.  This is mainly for documentation.
			/// </summary>
			public readonly string strName;

			/// <summary>
			/// The parameter to emit to yt-dlp
			/// </summary>
			public readonly string strParamToPass;


			/// <summary>
			/// Disallows overwriting files
			/// </summary>
			public static readonly OverWriteChoices disallow = new(@"disallow", @"--no-overwrites");

			/// <summary>
			/// Forces files that exist to be overwritten
			/// </summary>
			public static readonly OverWriteChoices force = new(@"force", @"--force-overwrites");

			/// <summary>
			/// Overwrites related files, but not the actual target
			/// </summary>
			public static readonly OverWriteChoices allowForRelatedFilesOnly = new(@"allowed for related files only",
				@"--no-force-overwrites");


			public string ValText
				=> strName;
		}

		/// <summary>
		/// Specifies which browser to get cookies from.  Optionally also specifies the profile and, on operating systems other than Windows, a keyring.
		/// </summary>
		/// <param name="Browser">A value from <see cref="SupportedBrowsers"/> that specifies the browser to use.</param>
		/// <remarks>
		///		<para>Add the profile with <see cref="Profile"/>.  Add the keyring with <see cref="KeyRing"/>.  Those are properties, so use the property initialization syntax.</para>
		/// </remarks>
		public class CookiesFromBrowserSpecVal(CookiesFromBrowserSpecVal.SupportedBrowsers Browser) : IOneOptVal
		{
			/// <summary>
			/// A value from <see cref="SupportedBrowsers"/> that specifies the browser to use.
			/// </summary>
			public SupportedBrowsers Browser
			{
				get;

				set;
			} = Browser;

			/// <summary>
			/// Which KeyRing to use.  Must be a value from <see cref="SupportedKeyRings"/>.
			/// </summary>
			public SupportedKeyRings? KeyRing
			{
				get;

				set;
			} = null;

			/// <summary>
			/// Gets or sets the profile.  Specify a profile name or path.
			/// </summary>
			public string Profile
			{
				get;

				set;
			} = new(string.Empty);

			/// <summary>
			/// Container name (if Firefox) ("none" for no container)
			/// </summary>
			public string Container
			{
				get;

				set;
			} = new(string.Empty);


			/// <summary>
			/// Lists all browsers that yt-dlp supports.  These are all <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members of <see cref="SupportedBrowsers"/>.  You aren’t allowed to create your own instances of <see cref="SupportedBrowsers"/> as that would require yt-dlp to support new browsers.
			/// </summary>
			public sealed class SupportedBrowsers
			{
				/// <summary>
				/// Constructs a new instance
				/// </summary>
				/// <param name="strName">The name of the browser as expected by yt-dlp.  Be sure to pass a raw string.</param>
				private SupportedBrowsers(in string strName)
					=> this.strName = strName;


				/// <summary>
				/// The exact name yt-dlp is looking for
				/// </summary>
				public readonly string strName;


				/// <summary>
				/// Specifies the Brave browser.
				/// </summary>
				public static readonly SupportedBrowsers brave = new(@"brave");

				/// <summary>
				/// Specifies the Google Chrome browser.  For Chromium, use <see cref="chromium"/>.
				/// </summary>
				public static readonly SupportedBrowsers chrome = new(@"chrome");

				/// <summary>
				/// Specifies the Chromium browser.  If you want Google Chrome, use <see cref="chrome"/>.
				/// </summary>
				public static readonly SupportedBrowsers chromium = new(@"chromium");

				/// <summary>
				/// Specifies the Microsoft Edge browser.
				/// </summary>
				public static readonly SupportedBrowsers edge = new(@"edge");

				/// <summary>
				/// Specifies Mozilla Firefox.
				/// </summary>
				public static readonly SupportedBrowsers firefox = new(@"firefox");

				/// <summary>
				/// Specifies the Opera browser
				/// </summary>
				public static readonly SupportedBrowsers opera = new(@"opera");

				/// <summary>
				/// Specifies the Safari browser
				/// </summary>
				public static readonly SupportedBrowsers safari = new(@"safari");

				/// <summary>
				/// Specifies the Vivaldi browser
				/// </summary>
				public static readonly SupportedBrowsers vivaldi = new(@"vivaldi");

				/// <summary>
				/// Specifies the Whale browser
				/// </summary>
				public static readonly SupportedBrowsers whale = new(@"whale");


				/// <summary>
				/// Disables the cookies.
				/// </summary>
				public static readonly SupportedBrowsers none = new(string.Empty);
			}

			/// <summary>
			/// Lists choices for which key ring you want to use.  These are all <c><see langword="public"/> <see langword="static"/> <see langword="readonly"/></c> members of <see cref="SupportedKeyRings"/>.  You aren’t allowed to create your own instances of <see cref="SupportedKeyRings"/> as that would require yt-dlp to support new browsers.
			/// </summary>
			public sealed class SupportedKeyRings
			{
				/// <summary>
				/// Constructs a new instance.
				/// </summary>
				/// <param name="strName">The exact text yt-dlp is looking for.  Use a raw string.</param>
				private SupportedKeyRings(in string strName)
					=> this.strName = strName;


				/// <summary>
				/// The exact text yt-dlp is looking for.
				/// </summary>
				public readonly string strName;


				/// <summary>
				/// Specifies the key ring should be basic text.  This might be unencrypted.
				/// </summary>
				public static readonly SupportedKeyRings basicText = new(@"basictext");

				/// <summary>
				/// Specifies the Gnome keyring.
				/// </summary>
				public static readonly SupportedKeyRings gnome = new(@"gnomekeyring");

				/// <summary>
				/// Specifies a KWallet keyring.  The version isn’t specified.
				/// </summary>
				public static readonly SupportedKeyRings kwallet = new(@"kwallet");

				/// <summary>
				/// Specifies a KWallet 5 keyring.
				/// </summary>
				public static readonly SupportedKeyRings kwallet5 = new(@"kwallet5");

				/// <summary>
				/// Specifies a KWallet 6 keyring.
				/// </summary>
				public static readonly SupportedKeyRings kwallet6 = new(@"kwallet6");
			}


			/// <summary>
			/// Generates the text that yt-dlp is looking for.  The output depends on which fields are <see langword="null"/> or non-<see langword="null"/>.
			/// </summary>
			public string? ValText
			{
				get
				{
					string? strKeyRing = KeyRing?.strName;

					return strKeyRing is null && Profile == "" && Container == ""
									? Browser.strName
									: strKeyRing is null && Profile == "" && Container != ""
										? $"{Browser.strName}::{Container}"
										: strKeyRing is null && Profile != "" && Container == ""
											? $"{Browser.strName}:{Profile}"
											: strKeyRing is null && Profile != "" && Container != ""
												? $"{Browser.strName}:{Profile}::{Container}"
												: strKeyRing is not null && Profile == "" && Container == ""
													? $"{Browser.strName}+{strKeyRing}"
													: strKeyRing is not null && Profile == "" && Container != ""
														? $"{Browser.strName}+{strKeyRing}::{Container}"
														: strKeyRing is not null && Profile != "" && Container == ""
															? $"{Browser.strName}+{strKeyRing}:{Profile}"
															: $"{Browser.strName}+{strKeyRing}:{Profile}::{Container}";
				}
			}

			/// <summary>
			/// Generates a <see cref="CookiesFromBrowserSpecVal"/> from a <see cref="SupportedBrowsers"/> instance.
			/// </summary>
			/// <param name="browser"></param>
			public static implicit operator CookiesFromBrowserSpecVal(in SupportedBrowsers browser)
				=> new(browser);
		}

		/// <summary>
		/// Groups all options that emit --paths.
		/// </summary>
		public sealed class PathsGroup
		{
			public PathsGroup()
			{
			}

			public PathsGroup(in PathsGroup copyThis)
			{
				Home = new(copyThis.Home);
				Temp = new(copyThis.Temp);
				Subs = new(copyThis.Subs);
				Thumbs = new(copyThis.Thumbs);
				Desc = new(copyThis.Desc);
				InfoJSON = new(copyThis.InfoJSON);
				Links = new(copyThis.Links);
				PlayListThumb = new(copyThis.PlayListThumb);
				PlayListDesc = new(copyThis.PlayListDesc);
				PlayListThumb = new(copyThis.PlayListThumb);
				PlayListInfoJSON = new(copyThis.PlayListInfoJSON);
				Chapters = new(copyThis.Chapters);
				PlayListEntries = new(copyThis.PlayListEntries);
			}


			/// <summary>
			/// All post-processed files will be downloaded to this path if it isn't null.  Ignored by yt-dlp if --output without a prefix specifies an absolute path.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsHome), @"All post-processed files will be downloaded to this path if it isn't null.  Ignored by yt-dlp if --output without a prefix specifies an absolute path.", typeof(FileSysGroup), @"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> Home
			{
				get;
			} = new(null, @"--paths", @"home");

			/// <summary>
			/// This path will be used while files are being downloaded if you change it from the default value of <see langword="null"/>.  Once post-processing is complete, the file will be moved to the home path.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsTemp), @"This path will be used while files are being downloaded if you change it from the default value of null.  Once post-processing is complete, the file will be moved to the home path.", typeof(FileSysGroup), @"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> Temp
			{
				get;
			} = new(null, @"--paths", @"temp");

			/// <summary>
			/// All subtitle files will be saved to this path if it isn't <see langword="null"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsSubs), @"All subtitle files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> Subs
			{
				get;
			} = new(null, @"--paths", @"subtitle");

			/// <summary>
			/// All thumbnail files will be saved to this path if it isn't <see langword="null"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsThumbs), @"All thumbnail files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> Thumbs
			{
				get;
			} = new(null, @"--paths", @"thumbnail");

			/// <summary>
			/// All description files for anything that isn’t a playlist will be saved here if this isn’t <see langword="null"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsDesc), @"All description files for anything that isn't a playlist will be saved here if this isn’t null.", typeof(FileSysGroup), @"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> Desc
			{
				get;
			} = new(null, @"--paths", "description");

			/// <summary>
			/// All .info.json files will be saved to this path if it isn’t <see langword="null"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsInfoJSON), @"All .info.json files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> InfoJSON
			{
				get;
			} = new(null, @"--paths", @"infojson");

			/// <summary>
			/// All link files will be saved to this path if it isn’t <see langword="null"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsLinks), @"All link files will be saved to this path if it isn't null.", typeof(FileSysGroup),
				@"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> Links
			{
				get;
			} = new(null, @"--paths", @"link");

			/// <summary>
			/// All playlist thumbnail files will be saved to this path if it isn’t <see langword="null"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsPlayListThumbs), @"All playlist thumbnail files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> PlayListThumb
			{
				get;
			} = new(null, @"--paths", @"pl_thumbnail");

			/// <summary>
			/// All playlist description files will be saved to this path if it isn’t <see langword="null"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsPlayListDesc), @"All playlist description files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> PlayListDesc
			{
				get;
			} = new(null, @"--paths", @"pl_description");

			/// <summary>
			/// All playlist .info.json files will be saved to this path if it isn’t <see langword="null"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsPlayListInfoJSON), @"All playlist .info.json files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> PlayListInfoJSON
			{
				get;
			} = new(null, @"--paths", @"pl_infojson");

			/// <summary>
			/// All chapter files will be saved to this path if it isn’t <see langword="null"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsChapter), @"All chapter files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> Chapters
			{
				get;
			} = new(null, @"--paths", @"chapter");

			/// <summary>
			/// All playlist video files will be saved to this path if it isn’t <see langword="null"/>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysPathsPlayListEntries), @"All playlist video files will be saved to this path if it isn't null.", typeof(FileSysGroup), @"--paths")]
			public OneOptWithPrefix<System.IO.DirectoryInfo?> PlayListEntries
			{
				get;
			} = new(null, @"pl_video", @"playlist");


			/// <summary>
			/// Lists all the options inside <see cref="FileSysGroup"/>.  THe parent of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					Home,
					Temp,
					Subs,
					Thumbs,
					Desc,
					InfoJSON,
					Links,
					PlayListThumb,
					PlayListDesc,
					PlayListInfoJSON,
					Chapters,
					PlayListEntries,
				];
		}

		/// <summary>
		/// Groups various options that all use --output.  These just use different prefixes.  Note: You won’t find the no-prefix version of --output here as all relevant Cmdlets in <see cref="YtDlpWrapper"/> declare that directly with <see cref="BaseVidCmdLet.FileNameFmt"/> and that would conflict with anything in here for that.  You also won’t find anything for the annotation template as that’s deprecated.
		/// </summary>
		public sealed class TemplatesForOutputFilesGroup
		{
			public TemplatesForOutputFilesGroup()
			{
			}

			public TemplatesForOutputFilesGroup(in TemplatesForOutputFilesGroup copyThis)
			{
				Subs = new(copyThis.Subs);
				Thumbs = new(copyThis.Thumbs);
				Desc = new(copyThis.Desc);
				InfoJSON = new(copyThis.InfoJSON);
				Links = new(copyThis.Links);
				PlayListThumbs = new(copyThis.PlayListThumbs);
				PlayListDesc = new(copyThis.PlayListDesc);
				PlayListInfoJSON = new(copyThis.PlayListInfoJSON);
				Chapters = new(copyThis.Chapters);
				PlayListEntries = new(copyThis.PlayListEntries);
			}


			/// <summary>
			/// Applies to all subtitle files
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatesSubs), @"Applies to all subtitle files", typeof(FileSysGroup),
				@"--output")]
			public OneOptWithPrefix<string> Subs
			{
				get;
			} = new(string.Empty, @"--output", @"subtitle", @"outtmpl");

			/// <summary>
			/// Applies to all thumbnail files
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatesThumbs), @"Applies to all thumbnail files", typeof(FileSysGroup),
				@"--output")]
			public OneOptWithPrefix<string> Thumbs
			{
				get;
			} = new(string.Empty, @"--output", @"infojson", @"outtmpl");

			/// <summary>
			/// Applies to all description files
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatesDesc), @"Applies to all description files", typeof(FileSysGroup),
				@"--output")]
			public OneOptWithPrefix<string> Desc
			{
				get;
			} = new(string.Empty, @"--output", @"link", @"outtmpl");

			/// <summary>
			/// Applies to all .info.json files
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatesInfoJSON), @"Applies to all .info.json files", typeof(FileSysGroup),
				@"--output")]
			public OneOptWithPrefix<string> InfoJSON
			{
				get;
			} = new(string.Empty, @"--output", @"infojson", @"outtmpl");

			/// <summary>
			/// Applies to all link files
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatesLinks), @"Applies to all link files", typeof(FileSysGroup), @"--output")]
			public OneOptWithPrefix<string> Links
			{
				get;
			} = new(string.Empty, @"--output", @"link", @"outtmpl");

			/// <summary>
			/// Applies to all playlist thumbnail files
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatePlayListThumbs), @"Applies to all playlist thumbnail files", typeof(FileSysGroup), @"--output")]
			public OneOptWithPrefix<string> PlayListThumbs
			{
				get;
			} = new(string.Empty, @"--output", @"pl_thumbnail", @"outtmpl");

			/// <summary>
			/// Applies to all playlist description files
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatePlayListDesc), @"Applies to all playlist description files", typeof(FileSysGroup), @"--output")]
			public OneOptWithPrefix<string> PlayListDesc
			{
				get;
			} = new(string.Empty, @"--output", @"pl_description", @"outtmpl");

			/// <summary>
			/// Applies to all playlist .info.json files
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatePlayListInfoJSON), @"Applies to all playlist .info.json files", typeof(FileSysGroup), @"--output")]
			public OneOptWithPrefix<string> PlayListInfoJSON
			{
				get;
			} = new(string.Empty, @"--output", @"pl_infojson", @"outtmpl");

			/// <summary>
			/// Applies to all chapter files
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplateChapters), @"Applies to all chapter files", typeof(FileSysGroup),
				@"--output")]
			public OneOptWithPrefix<string> Chapters
			{
				get;
			} = new(string.Empty, @"--output", @"chapter", @"outtmpl");

			/// <summary>
			/// Applies to all playlist video files
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strFileSysOutputTemplatePlayListEntries), @"Applies to all playlist video files", typeof(FileSysGroup), @"--output")]
			public OneOptWithPrefix<string> PlayListEntries
			{
				get;
			} = new(string.Empty, @"--output", @"pl_video", @"outtmpl");


			/// <summary>
			/// Lists all options inside <see cref="TemplatesForOutputFilesGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to generate it’s own list.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					Subs,
					Thumbs,
					Desc,
					InfoJSON,
					Links,
					PlayListThumbs,
					PlayListDesc,
					PlayListInfoJSON,
					Chapters,
					PlayListEntries,
				];
		}


		/// <summary>
		/// Lets you access options inside <see cref="PathsGroup"/>.
		/// </summary>
		public PathsGroup Paths
		{
			get;

			private init;
		} = new();

		/// <summary>
		/// Provides access to <see cref="TemplatesForOutputFilesGroup"/>.
		/// </summary>
		public TemplatesForOutputFilesGroup OutputTemplates
		{
			get;

			private init;
		} = new();

		/// <summary>
		/// Placeholder for unavailable fields.  If you leave this at the default, yt-dlp uses “NA” unless a config file provides another value with
		/// --output-na-placeholder.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysPlaceHolderTextInGeneratedFileNames), @"Placeholder for unavailable fields.  If you leave this at the default, yt-dlp uses “NA” unless a config file provides another value with --output-na-placeholder.", typeof(FileSysGroup), @"--output-na-placeholder")]
		public OneOpt<string> PlaceHolderTextInGeneratedFileNames
		{
			get;

			private init;
		} = new(string.Empty, @"--output-na-placeholder", @"outtmpl_na_placeholder");

		/// <summary>
		/// If <see langword="true"/>, file names are limited to ASCII characters with no spaces or ampersands (‘&amp;’).  If <see langword="false"/>, no such
		/// restrictions are in place.  If you use the default value of <see langword="null"/> and no config file specifies a value, yt-dlp will act as though
		/// you set RestrictFileNames to <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFIleSysRestrictFileNames), @"If true, file names are limited to ASCII characters with no spaces or ampersands (‘&’).  If false, no such restrictions are in place.  If you use the default value of null and no config file specifies a value, yt-dlp will act as though you set restrictFileNames to false.", typeof(FileSysGroup), @"--restrict-filenames", @"--no-restrict-filenames")]
		public ThreeWayOpt RestrictFileNames
		{
			get;

			private init;
		} = new(@"--restrict-filenames", @"--no-restrict-filenames", @"restrictfilenames", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// If <see langword="true"/>, all file names must be Windows compatible even if yt-dlp is run on a non-Windows OS.  If <see langword="false"/>, only
		/// minimal sanitation is done.  If you use the default value of <see langword="null"/> and --windows-filenames isn’t set by a config file, yt-dlp’s
		/// default changes based on if you’re running it in Windows or something else.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysUseWindowsNames), @"If true, all file names must be Windows compatible even if yt-dlp is run on a non-Windows OS.  If false, only minimal sanitation is done.  If you use the default value of null and this isn’t set by a config file, yt-dlp’s default changes based on if you’re running it in Windows or something else.", typeof(FileSysGroup), @"--windows-filenames",
			@"--no-windows-filenames")]
		public ThreeWayOpt UseWindowsFileNames
		{
			get;

			private init;
		} = new(@"--windows-filenames", @"--no-windows-filenames", @"windowsfilenames", ThreeWayOpt.WhichValsToSendToPythonChoices.both);

		/// <summary>
		/// Limit the filename length (excluding extension) to the specified number of characters
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysMaxFileNameLength), @"Limit the filename length (excluding extension) to the specified number of characters", typeof(FileSysGroup), @"--trim-filenames")]
		public OneOpt<ushort?> MaxFileNameLength
		{
			get;

			private init;
		} = new(null, @"--trim-filenames", @"trim_file_name");

		/// <summary>
		/// Controls how and when yt-dlp overwrites files.  The default value of <see langword="null"/> means that unless a config file species --no-overwrites,
		/// --force-overwrites, or --no-force-overwrites, yt-dlp acts as though you chose OverWriteChoices.allowForRelatedFilesOnly.  Your choices are
		/// OverWriteChoices.disallow (never overwrite), OverWriteChoices.force (always overwrite), and OverWriteChoices.allowForRelatedFilesOnly.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysWhenToAllowOverWrites), @"Controls how and when yt-dlp overwrites files.  The default value of null means that unless a config file species --no-overwrites, --force-overwrites, or --no-force-overwrites, yt-dlp acts as though you chose OverWriteChoices.allowForRelatedFilesOnly.  Your choices are OverWriteChoices.disallow (never overwrite), OverWriteChoices.force (always overwrite), and OverWriteChoices.allowForRelatedFilesOnly.", typeof(FileSysGroup), @"--no-overwrites", @"--force-overwrites", @"--no-force-overwrites")]
		public OneOpt<OverWriteChoices?> WhenToAllowOverWrites
		{
			get;
		} = new(null, string.Empty)
		{
			ParamListGenerator =
				opt
					=>
						opt.CurVal is null
							? []
							: opt.CurVal == OverWriteChoices.disallow
								? [@"--no-overwrites"]
								: opt.CurVal == OverWriteChoices.force
									? [@"--force-overwrites"]
									: opt.CurVal == OverWriteChoices.allowForRelatedFilesOnly
										? [@"--no-force-overwrites"]
										: throw new Tools.Exceptions.UnknownOrInvalidEnumException<OverWriteChoices>(opt.CurVal, "While mapping an OverWriteChoices value to a parameter"),

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is null || opt.CurVal == OverWriteChoices.allowForRelatedFilesOnly
						? []
						: opt.CurVal == OverWriteChoices.disallow
							? new DictionaryToOpt()
							{
								[@"overwrites"] = false,
							}
							: opt.CurVal == OverWriteChoices.force
								? new DictionaryToOpt()
								{
									[@"continuedl"] = false,
									[@"overwrites"] = true,
								}
								: throw new Tools.Exceptions.UnknownOrInvalidEnumException<OverWriteChoices>(opt.CurVal, "While mapping an OverWriteChoices value to a parameter"),
		};

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will attempt to restart any interrupted downloads from a previous yt-dlp call.  If <see langword="false"/>, yt-dlp
		/// will always restart such downloads.  The default value of <see langword="null"/> acts like <see langword="true"/> unless a config file forces another
		/// behavior with -c, --continue, or --no-continue.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysResumePartialDownLoads), @"If true, yt-dlp will attempt to restart any interrupted downloads from a previous yt-dlp call.  If false, yt-dlp will always restart such downloads.  The default value of null acts like true unless a config file forces another behavior with -c, --continue, or --no-continue.", typeof(FileSysGroup), @"--continue", @"--no-continue")]
		public ThreeWayOpt ResumePartialDownLoads
		{
			get;

			private init;
		} = new(@"--continue", @"--no-continue", @"continuedl", ThreeWayOpt.WhichValsToSendToPythonChoices.falseOnly);

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will write into part files instead of the main file.  If <see langword="false"/>, it will write instead into the
		/// main file.  The default value of <see langword="null"/> acts like true unless a config file forces another value with either --part or --no-part.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysUsePartFiles), @"If true, yt-dlp will write into part files instead of the main file.  If false, it will write instead into the main file.  The default value of null acts like true unless a config file forces another value with either --part or --no-part.",
			typeof(FileSysGroup), @"--part", @"--no-part")]
		public ThreeWayOpt UsePartFiles
		{
			get;

			private init;
		} = new(@"--part", @"--no-part", @"nopart", ThreeWayOpt.WhichValsToSendToPythonChoices.falseReversedOnly);

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will use the Last-Modified header to set the file modified time.  If <see langword="false"/>, it won’t.  The default
		/// value of <see langword="null"/>, if <see cref="MTime"/> isn’t set by a config file, causes yt-dlp to act as though <see cref="MTime"/> is
		/// <see langword="true"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysMTime), @"If true, yt-dlp will use the Last-Modified header to set the file modified time.  If false, it won’t.  The default value of null, if MTime isn’t set by a config file, causes yt-dlp to act as though MTime is true.", typeof(FileSysGroup), @"--mtime", @"--no-mtime")]
		public ThreeWayOpt MTime
		{
			get;

			private init;
		} = new(@"--mtime", @"--no-mtime", @"updatetime", ThreeWayOpt.WhichValsToSendToPythonChoices.both);

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will create a .description file for each video.  If <see langword="false"/>, no such file will be created.  The
		/// default value of <see langword="null"/>, if no config file specifies one, causes yt-dlp to act as though you specified <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysDescShouldBeWritten), @"If true, yt-dlp will create a .description file for each video.  If false, no such file will be created.  The default value of null, if no config file specifies one, causes yt-dlp to act as though you specified false.", typeof(FileSysGroup), @"--write-description", @"--no-write-description")]
		public ThreeWayOpt DescShouldBeWritten
		{
			get;

			private init;
		} = new(@"--write-description", @"--no-write-description", @"writedescription", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will create a .info.json file for each video.  If <see langword="false"/>, no such file will be created.  The
		/// default value of <see langword="null"/>, if no config file specifies one, causes yt-dlp to act as though you specified <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysInfoJsonShouldBeWritten), @"If true, yt-dlp will create a .info.json file for each video.  If false, no such file will be created.  The default value of null, if no config file specifies one, causes yt-dlp to act as though you specified false.", typeof(FileSysGroup), @"--write-infojson", @"--no-write-infojson")]
		public ThreeWayOpt InfoJsonShouldBeWritten
		{
			get;

			private init;
		} = new(@"--write-info-json", @"--no-write-info-json", @"writeinfojson", ThreeWayOpt.WhichValsToSendToPythonChoices.both);

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will write a playlist metadata file for each video.  If <see langword="false"/>, no such file will be created.  The
		/// default value of <see langword="null"/>, if no config file specifies one, causes yt-dlp to act as though you specified <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysPlayListMetaDataShouldBeWritten), @"If true, yt-dlp will write a playlist metadata file for each video.  If false, no such file will be created.  The default value of null, if no config file specifies one, causes yt-dlp to act as though you specified false.", typeof(FileSysGroup), @"--write-playlist-metafiles", @"--no-write-playlist-metafiles")]
		public ThreeWayOpt PlayListMetaFilesShouldBeWritten
		{
			get;

			private init;
		} = new(@"--write-playlist-metafiles", @"--no-write-playlist-metafiles", @"allow_playlist_files", ThreeWayOpt.WhichValsToSendToPythonChoices.falseOnly);

		/// <summary>
		/// If <see langword="true"/>, comments will be included in any .info.json files.  If <see langword="false"/>, comments won’t be included unless the
		/// extraction is known to be quick.  The default value of <see langword="null"/>, if no value is in a config file, causes yt-dlp to act as though you
		/// used value is <see langword="false"/>.  Ignored by yt-dlp unless you also specify set <see cref="InfoJsonShouldBeWritten"/> to a
		/// non-<see langword="null"/> value or a config file specifies --write-comments.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysCommentsShouldBeWritten), @"If true, comments will be included in any .info.json files.  If false, comments won’t be included unless the extraction is known to be quick.  The default value of null, if no value is in a config file, causes yt-dlp to act as though you used value is false.  Ignored by yt-dlp unless you also specify set InfoJsonShouldBeWritten to a non-null value or a config file specifies --write-comments.", typeof(FileSysGroup), @"--write-comments", @"--no-write-comments")]
		public ThreeWayOpt CommentsShouldBeWritten
		{
			get;

			private init;
		} = new(@"--write-comments", @"--no-write-comments")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.Val is null || opt.Val == false
						? []
						: new DictionaryToOpt()
						{
							[@"getcomments"] = true,
							[@"writeinfojson"] = true,
						},
		};

		/// <summary>
		/// If you specify a value other than <see langword="null"/> or <see cref="fileInvalid"/>, this must be a Netscape formatted file to read cookies from
		///  dump cookie jar into.  If you use <see cref="fileInvalid"/>, it will disable any value from a config file.  If you use the default value of
		/// <see langword="null"/>, yt-dlp will not use a cookies file unless you set <see cref="CookiesFromBrowser"/> or a config file specifies a cookie file
		/// with --cookies.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysCookieFile), @"If you specify a value other than null or fileInvalid, this must be a Netscape formatted file to read cookies from and dump cookie jar into.  If you use fileInvalid, it will disable any value from a config file.  If you use the default value of null, yt-dlp will not use a cookies file unless you set CookiesFromBrowser or a config file specifies a cookie file with --cookies.", typeof(FileSysGroup),
			@"--cookies", @"--no-cookies")]
		public OneOpt<System.IO.FileInfo?> CookiesFromFile
		{
			get;

			private init;
		} = new(null, @"--cookies")
		{
			ParamListGenerator =
				opt
					=> opt.CurVal == null
						? []
						: opt.CurVal == fileInvalid
							? [@"--no-cookies"]
							: [@"--cookies", opt.CurVal.FullName],

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is System.IO.FileInfo fileWithCookies && opt.CurVal != fileInvalid
						? new DictionaryToOpt()
						{
							[@"cookiefile"] = new(fileWithCookies.FullName),
						}
						: [],
		};

		/// <summary>
		/// If you specify a non-<see langword="null"/> value with a browser other than <see cref="CookiesFromBrowserSpecVal.SupportedBrowsers.none"/>, yt-dlp
		/// will attempt to get cookies from the specified browser.  The default value of <see langword="null"/> doesn't cause cookies to be loaded from a
		/// browser though yt-dlp might do so anyway if a config file specifies --cookies-from-browser or you use <see cref="CookiesFromFile"/>.  Specify a
		/// non-<see langword="null"/> value and set <see cref="CookiesFromBrowserSpecVal.Browser"/> to <see cref="CookiesFromBrowserSpecVal.SupportedBrowsers
		/// .none"/> if you want to disable any value from a config file.  If you don’t need to specify a profile or keyring, just pass the <see
		/// cref="CookiesFromBrowserSpecVal.SupportedBrowsers"/> instance.  It will be implicitly typecast to the needed type.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysCookiesFromBrowser), @"If you specify a non-null value with a browser other than CookiesFromBrowserSpecOpt.SupportedBrowsers.none, yt-dlp will attempt to get cookies from the specified browser.  The default value of null doesn't cause cookies to be loaded from a browser though yt-dlp might do so anyway if a config file specifies --cookies-from-browser or you use CookiesFromFile.  Specify a non-null value and set browser to CookiesFromBrowserSpecOpt.SupportedBrowsers.none if you want to disable any value from a config file.  If you don’t need to specify a profile or keyring, just pass the CookiesFromBrowserSpecOpt.SupportedBrowsers instance.  It will be implicitly typecast to the needed type.", typeof(FileSysGroup), @"--cookies-from-browser", @"--no-cookies-from-browser")]
		public OneOpt<CookiesFromBrowserSpecVal?> CookiesFromBrowser
		{
			get;

			private init;
		} = new(null, string.Empty)
		{
			ParamListGenerator =
				opt
					=>
						opt.CurVal is not null
							? opt.CurVal.Browser == new(CookiesFromBrowserSpecVal.SupportedBrowsers).none
								? [@"--no-cookies-from-browser"]
								: opt.CurVal.ValText is not null
									? [@"--cookies-from-browser", opt.CurVal.ValText]
									: []
							: [],

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is not null && opt.CurVal.Browser != new(CookiesFromBrowserSpecVal.SupportedBrowsers).none
						? new DictionaryToOpt()
						{
							[@"cookiesfrombrowser"] =
									new System.Collections.Generic.List<object?>()
									{
										opt.CurVal.Browser.strName,
										opt.CurVal.KeyRing?.strName,
										opt.CurVal.Profile,
										opt.CurVal.Container,
									},
						}
						: [],
		};

		/// <summary>
		/// Location in the file system where yt-dlp can store some downloaded information (such as client IDs "and signatures) permanently.  The default value
		/// of <see langword="null"/>, unless --cache-dir or --no-cache-dir is in a config file, causes yt-dlp to look for an environment variable called
		/// XDG_CACHE_HOME.  If it exists and contains a valid path, yt-dlp creates a subfolder named “yt-dlp” inside there and uses that.  If you set <see
		/// cref="CacheDir"/> to dirInvalid, yt-dlp will disable caching.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strFileSysCacheDir), @"Location in the file system where yt-dlp can store some downloaded information (such as client IDs and signatures) permanently.  The default value of null, unless --cache-dir or --no-cache-dir is in a config file, causes yt-dlp to look for an environment variable called XDG_CACHE_HOME.  If it exists and contains a valid path, yt-dlp creates a subfolder named “yt-dlp” inside there and uses that.  If you set CacheDir to dirInvalid, yt-dlp will disable caching.", typeof(FileSysGroup), @"--cache-dir", @"--no-cache-dir")]
		public OneOpt<System.IO.DirectoryInfo?> CacheDir
		{
			get;

			private init;
		} = new(null, string.Empty)
		{
			ParamListGenerator =
				opt
					=> opt.CurVal is null
						? []
						: opt.CurVal == dirInvalid
							? [@"--no-cache-dir"]
							: [@"--cache-dir", opt.CurVal.FullName],

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is null
						? []
						: opt.CurVal == dirInvalid
							? new DictionaryToOpt()
							{
								[@"cachedir"] = false,
							}
							: new DictionaryToOpt()
							{
								[@"cachedir"] = new(opt.CurVal).FullName,
							},
		};


		/// <summary>
		/// Lists all options inside <see cref="FileSysGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to build its own list of options.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				..Paths.AllOpt,
				..OutputTemplates.AllOpt,
				PlaceHolderTextInGeneratedFileNames,
				RestrictFileNames,
				UseWindowsFileNames,
				MaxFileNameLength,
				WhenToAllowOverWrites,
				ResumePartialDownLoads,
				UsePartFiles,
				MTime,
				DescShouldBeWritten,
				InfoJsonShouldBeWritten,
				PlayListMetaFilesShouldBeWritten,
				CommentsShouldBeWritten,
				CookiesFromFile,
				CookiesFromBrowser,
				CacheDir,
			];
	}
}