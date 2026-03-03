// Ignore Spelling: Dlp Vals uri Sel wget Xattr Hls Sys Arounds Vid Fmts Pwd Executables Evt Exe Evts Loc Locs Json Dest Hdrs Langs

namespace WillPittenger.Goodies.YtDlpWrapper;

using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Groups a series of options related to thumbnails.  All are listed under <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#thumbnail-options">Thumbnail Options section of yt-dlp’s readme.md</a>.
	/// </summary>
	public sealed class ThumbGroup
	{
		public ThumbGroup()
		{
		}

		public ThumbGroup(in ThumbGroup copyThis)
		{
			Write = new(copyThis.Write);
			List = new(copyThis.List);
		}


		/// <summary>
		/// yt-dlp defines several different options for what thumbnails it writes out.  <see cref="WriteChoices"/> lists those.
		/// </summary>
		public enum WriteChoices
		{
			/// <summary>
			/// Causes yt-dlp to write video thumbnails.
			/// </summary>
			yes,

			/// <summary>
			/// Prevents yt-dlp from ever writing thumbnails even if a config file sets either --write-thumbnail or --write-all-thumbnails.
			/// </summary>
			never,

			/// <summary>
			/// Causes yt-dlp to write all thumbnails out.
			/// </summary>
			all,

			/// <summary>
			/// If you use <see cref="@default"/>, yt-dlp will act like you actually specified <see cref="never"/> unless a config file sets --write-thumbnail, --no-write-thumbnail, or --write-all-thumbnails.
			/// </summary>
			@default,
		}

		/// <summary>
		/// If you set this to <see cref="WriteChoices.yes"/>, yt-dlp will write thumbnails for videos.  If you use <see cref="WriteChoices.never"/>, it will never write thumbnails for videos.  If you use <see cref="WriteChoices.all"/>, it will write all thumbnails.  <see cref="WriteChoices.@default"/> causes yt-dlp to act as though you specified <see cref="WriteChoices.never"/> unless a config file specifies either --write-thumbnail, --no-write-thumbnail, or --write-all-thumbnails.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strThumbWrite), @"If you set this to WriteChoices.yes, yt-dlp will write thumbnails for videos.  If you use WriteChoices.never, it will never write thumbnails for videos.  If you use WriteChoices.all, it will write all thumbnails.  WriteChoices.@default causes yt-dlp to act as though you specified WriteChoices.never unless a config file specifies either --write-thumbnail, --no-write-thumbnail, or --write-all-thumbnails.", typeof(ThumbGroup), @"--write-thumbnail", @"--no-write-thumbnail", @"--write-all-thumbnails")]
		public OneOpt<WriteChoices> Write
		{
			get;
		} = new(WriteChoices.@default, string.Empty)
		{
			ParamListGenerator =
				opt
					=> opt.CurVal switch
					{
						WriteChoices.@default
							=> [],

						WriteChoices.yes
							=> [@"--write-thumbnail"],

						WriteChoices.never
							=> [@"--no-write-thumbnail"],

						WriteChoices.all
							=> [@"--write-all-thumbnails"],

						_
							=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<WriteChoices>(opt.CurVal, @"While converting a WriteChoices value into a yt-dlp parameter"),
					},

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal switch
					{
						WriteChoices.@default
							=> [],

						WriteChoices.yes
							=> new DictionaryToOpt()
							{
								[@"writethumbnail"] = true,
							},

						WriteChoices.all
							=> new DictionaryToOpt()
							{
								[@"write_all_thumbnails"] = true,
							},

						WriteChoices.never or _
							=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<WriteChoices>(opt.CurVal, @"While converting a WriteChoices value into a yt-dlp parameter"),
					},
		};

		/// <summary>
		/// If true, causes yt-dlp to list available thumbnails of each video.  Note: Thumbnail information is also available via other methods such as getting the JSON or using the types in the namespace <see cref="Goodies.YtDlpWrapper"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strThumbList), @"If true, causes yt-dlp to list available thumbnails of each video.  Note: Thumbnail information is also available via other methods such as getting the JSON or using the types in the namespace WillPittenger.Goodies.YtDlpWrapper.", typeof(ThumbGroup), @"--list-thumbnails")]
		public OneOpt<bool> List
		{
			get;
		} = new(false, @"--list-thumbnails")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
						{
							[@"list_thumbnails"] = true,
						}
						: [],
		};


		/// <summary>
		/// Lists all options inside <see cref="ThumbGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to generate its own list of options.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				Write,
				List,
			];
	}
}