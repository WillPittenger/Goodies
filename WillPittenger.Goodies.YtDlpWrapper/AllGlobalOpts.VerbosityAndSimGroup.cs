// Ignore Spelling: Dlp Vals uri Sel wget Xattr Hls Sys Arounds Vid Fmts Pwd Executables Evt Exe Evts Loc Locs Json Dest Hdrs Langs

namespace WillPittenger.Goodies.YtDlpWrapper;

using IReadOnlyDictionaryToOptLists = System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<object>?>;
using IReadOnlyDictionaryToOpt = System.Collections.Generic.IReadOnlyDictionary<string, object?>;
using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Groups various options related to verbosity and simulation.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#verbosity-and-simulation-options">Verbosity and Simulation Options section of yt-dlp’s readme.md</a>.
	/// </summary>
	public sealed class VerbosityAndSimGroup
	{
		public VerbosityAndSimGroup()
		{
		}

		public VerbosityAndSimGroup(in VerbosityAndSimGroup copyThis)
		{
			QuietMode = copyThis.QuietMode;
			NoWarnings = copyThis.NoWarnings;
			SimMode = copyThis.SimMode;
			IgnoreNoFmtsError = copyThis.IgnoreNoFmtsError;
			SkipDownLoad = copyThis.SkipDownLoad;
			PrintTemplates = new(copyThis.PrintTemplates);
			PrintToFile = new(copyThis.PrintToFile);
			ProgressMode = copyThis.ProgressMode;
			ForceWriteArchive = copyThis.ForceWriteArchive;
			DeltaOfProgressUpdates = copyThis.DeltaOfProgressUpdates;
			DumpPages = copyThis.DumpPages;
			WritePages = copyThis.WritePages;
			PrintTraffic = copyThis.PrintTraffic;
		}


		/// <summary>
		/// Provides a way to specify all the parameters defined by yt-dlp's --print-to-file option.  You need to pass in a file and specify a template before anything will be emitted to yt-dlp.
		/// </summary>
		public sealed class PrintAndTemplateFileOpt : BaseOneOpt<PrintAndTemplateFileOpt>
		{
			/// <param name="strPrefix">One of the prefix strings that yt-dlp is looking for.  Use a raw string.</param>
			public PrintAndTemplateFileOpt(in string strPrefix)
			{
				PythonParamsGenerator = GetPythonParams;

				this.strPrefix = strPrefix;
			}

			public PrintAndTemplateFileOpt(in PrintAndTemplateFileOpt copyThis)
			{
				PythonParamsGenerator = GetPythonParams;

				fileDest = copyThis.fileDest;
				strTemplate = new(copyThis.strTemplate);
				strPrefix = new(copyThis.strPrefix);
			}


			/// <summary>
			/// The file that you want.  The file doesn’t need to already exist, but the file name does need to be valid.
			/// </summary>
			public System.IO.FileInfo? fileDest = null;

			/// <summary>
			/// The template of what you want printed.  Note: Until this reaches yt-dlp, no verification of the syntax is done.
			/// </summary>
			public string strTemplate = string.Empty;

			/// <summary>
			/// The exact prefix text yt-dlp is looking for
			/// </summary>
			public readonly string strPrefix;

			/// <summary>
			/// Generates the value text in the syntax that yt-dlp is looking for.
			/// </summary>
			/// <remarks>
			///		<para>Nothing will be emitted if any of the following is true:</para>
			///		<list type="bullet">
			///			<item><see cref="strTemplate"/> is <see cref="string.Empty"/></item>
			///			<item><see cref="fileDest"/> is <see langword="null"/></item>
			///			<item><see cref="fileDest"/> is <see cref="fileInvalid"/></item>
			///		</list>
			/// </remarks>
			public override System.Collections.Generic.IEnumerable<string> Params
				=> strTemplate == string.Empty || fileDest == null || fileDest == fileInvalid
					? []
					: [@"--print-to-file", $"{strPrefix}:{strTemplate}", fileDest.FullName];

			internal override DictionaryToOpt? PythonParams
				=> PythonParamsGenerator?.Invoke(this);

			private DictionaryToOpt? GetPythonParams(in PrintAndTemplateFileOpt opt)
				=> opt.strTemplate == string.Empty || opt.fileDest == null || opt.fileDest == fileInvalid
					? []
					: new DictionaryToOpt()
					{
						[@"print_to_file"] = new DictionaryToOpt()
						{
							[opt.strPrefix] = new System.Collections.Generic.List<string>()
									{
										opt.strTemplate,
										opt.fileDest.FullName,
									},
						}
					};
		}


		/// <summary>
		/// Groups all options that invoke the yt-dlp option --print.
		/// </summary>
		public sealed class PrintTemplatesGroup
		{
			public PrintTemplatesGroup()
			{
			}

			public PrintTemplatesGroup(in PrintTemplatesGroup copyThis)
			{
				PreProcess = copyThis.PreProcess;
				AfterFilter = copyThis.AfterFilter;
				Vid = copyThis.Vid;
				BeforeDownLoad = copyThis.BeforeDownLoad;
				PostProcess = copyThis.PostProcess;
				AfterMove = copyThis.AfterMove;
				AfterVid = copyThis.AfterVid;
				PlayList = copyThis.PlayList;
			}


			/// <summary>
			/// Field name or output template to print to screen during pre-processing.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplatePreProcess), @"Field name or output template to print to screen during pre-processing.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate. is used.", typeof(VerbosityAndSimGroup), @"--print")]
			public OneOptWithPrefix<string> PreProcess
			{
				get;
			} = new(string.Empty, @"--print", @"pre_process")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal == string.Empty
							? []
							: new DictionaryToOpt()
							{
								[@"forceprint"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = opt.CurVal,
								},
								[@"noprogress"] = true,
								[@"quiet"] = true,
								[@"simulate"] = true,
							}
			};

			/// <summary>
			/// Field name or output template to print to screen once an item passes filtering.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplateAfterFilter), @"Field name or output template to print to screen once an item passes filtering.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print")]
			public OneOptWithPrefix<string> AfterFilter
			{
				get;
			} = new(string.Empty, @"--print", @"after_filter")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal == string.Empty
							? []
							: new DictionaryToOpt()
							{
								[@"forceprint"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = opt.CurVal,
								},
								[@"noprogress"] = true,
								[@"quiet"] = true,
								[@"simulate"] = true,
							}
			};

			/// <summary>
			/// Field name or output template to print to screen once an item is ready to download.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplateVid), @"Field name or output template to print to screen once an item is ready to download.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print")]
			public OneOptWithPrefix<string> Vid
			{
				get;
			} = new(string.Empty, @"--print", @"video")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal == string.Empty
							? []
							: new DictionaryToOpt()
							{
								[@"forceprint"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = opt.CurVal,
								},
								[@"noprogress"] = true,
								[@"quiet"] = true,
								[@"simulate"] = true,
							}
			};

			/// <summary>
			/// Field name or output template to print to screen just before a download.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplateBeforeDownLoad), @"Field name or output template to print to screen just before a download.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print")]
			public OneOptWithPrefix<string> BeforeDownLoad
			{
				get;
			} = new(string.Empty, @"--print", @"before_dl")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal == string.Empty
							? []
							: new DictionaryToOpt()
							{
								[@"forceprint"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = opt.CurVal,
								},
								[@"noprogress"] = true,
								[@"quiet"] = true,
								[@"simulate"] = true,
							}
			};

			/// <summary>
			/// Field name or output template to print to screen just before post-processing.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplatePostProcess), @"Field name or output template to print to screen just before post-processing.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print")]
			public OneOptWithPrefix<string> PostProcess
			{
				get;
			} = new(string.Empty, @"--print", @"post_process")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal == string.Empty
							? []
							: new DictionaryToOpt()
							{
								[@"forceprint"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = opt.CurVal,
								},
								[@"noprogress"] = true,
								[@"quiet"] = true,
								[@"simulate"] = true,
							}
			};

			/// <summary>
			/// Field name or output template to print to screen after moving the file.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplateAfterMove), @"Field name or output template to print to screen after moving the file.  Implies QuietMode=true/--quiet. Implies SimModeMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print")]
			public OneOptWithPrefix<string> AfterMove
			{
				get;
			} = new(string.Empty, @"--print", @"after_move")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal == string.Empty
							? []
							: new DictionaryToOpt()
							{
								[@"forceprint"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = opt.CurVal,
								},
								[@"noprogress"] = true,
								[@"quiet"] = true,
								[@"simulate"] = true,
							}
			};

			/// <summary>
			/// Field name or output template to print to screen after a video is completely processed.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplateAfterVid), @"Field name or output template to print to screen after a video is completely processed.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimModeMode=false/--no-simulate.",
				typeof(VerbosityAndSimGroup), @"--print")]
			public OneOptWithPrefix<string> AfterVid
			{
				get;
			} = new(string.Empty, @"--print", @"after_video")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal == string.Empty
							? []
							: new DictionaryToOpt()
							{
								[@"forceprint"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = opt.CurVal,
								},
								[@"noprogress"] = true,
								[@"quiet"] = true,
								[@"simulate"] = true,
							}
			};

			/// <summary>
			/// Field name or output template to print to screen after all videos in a playlist are done.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless  <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate. is used.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTemplatePlayList), @"Field name or output template to print to screen after all videos in a playlist are done.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimModeMode=false/--no-simulate.",
				typeof(VerbosityAndSimGroup), @"--print")]
			public OneOptWithPrefix<string> PlayList
			{
				get;
			} = new(string.Empty, @"--print", @"playlist")
			{
				PythonParamsGenerator =
					(in opt)
						=> opt.CurVal == string.Empty
							? []
							: new DictionaryToOpt()
							{
								[@"forceprint"] = new DictionaryToOpt()
								{
									[opt.strValPrefix] = opt.CurVal,
								},
								[@"noprogress"] = true,
								[@"quiet"] = true,
								[@"simulate"] = true,
							}
			};


			/// <summary>
			/// Lists all options inside <see cref="PrintTemplatesGroup"/>.  The parent of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					PreProcess,
					AfterFilter,
					Vid,
					BeforeDownLoad,
					PostProcess,
					AfterMove,
					AfterVid,
					PlayList,
				];
		}

		/// <summary>
		/// Groups all options that invoke the yt-dlp option --print-to-file.
		/// </summary>
		public sealed class PrintToFileGroup
		{
			public PrintToFileGroup()
			{
			}

			public PrintToFileGroup(in PrintToFileGroup copyThis)
			{
				PreProcess = copyThis.PreProcess;
				AfterFilter = copyThis.AfterFilter;
				Vid = copyThis.Vid;
				BeforeDownLoad = copyThis.BeforeDownLoad;
				PostProcess = copyThis.PostProcess;
				AfterMove = copyThis.AfterMove;
				AfterVid = copyThis.AfterVid;
				PlayList = copyThis.PlayList;
			}


			/// <summary>
			/// Field name or output template to print to screen during pre-processing.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet. Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFilePreProcess), @"Field name or output template to print to screen during pre-processing.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print-to-file")]
			public PrintAndTemplateFileOpt PreProcess
			{
				get;
			} = new(@"pre_process");

			/// <summary>
			/// Field name or output template to print to screen once an item passes filtering.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFileAfterFilter), @"Field name or output template to print to screen once an item passes filtering.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print-to-file")]
			public PrintAndTemplateFileOpt AfterFilter
			{
				get;
			} = new(@"after_filter");

			/// <summary>
			/// Field name or output template to print to screen once an item is ready to download.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFileVid), @"Field name or output template to print to screen once an item is ready to " +
				@"download.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print-to-file")]
			public PrintAndTemplateFileOpt Vid
			{
				get;
			} = new(@"video");

			/// <summary>
			/// Field name or output template to print to screen just before a download.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFileBeforeDownLoad), @"Field name or output template to print to screen just before a download.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print-to-file")]
			public PrintAndTemplateFileOpt BeforeDownLoad
			{
				get;
			} = new(@"before_dl");

			/// <summary>
			/// Field name or output template to print to screen just before post-processing.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFilePostProcess), @"Field name or output template to print to screen just before post-processing.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print-to-file")]
			public PrintAndTemplateFileOpt PostProcess
			{
				get;
			} = new(@"post_process");

			/// <summary>
			/// Field name or output template to print to screen after moving the file.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFileAfterMove), @"Field name or output template to print to screen after moving the file.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print-to-file")]
			public PrintAndTemplateFileOpt AfterMove
			{
				get;
			} = new(@"after_move");

			/// <summary>
			/// Field name or output template to print to screen after a video is completely processed.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFileAfterVid), @"Field name or output template to print to screen after a video is completely processed.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print-to-file")]
			public PrintAndTemplateFileOpt AfterVid
			{
				get;
			} = new(@"after_video");

			/// <summary>
			/// Field name or output template to print to screen after all videos in a playlist are done.  Implies <c><see cref="QuietMode"/>=<see langword="true"/></c>/--quiet.  Implies <c><see cref="SimMode"/>=<see langword="true"/></c>/--simulate unless <c><see cref="SimMode"/>=<see langword="false"/></c>/--no-simulate.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintToFilePlayList), @"Field name or output template to print to screen after all videos in a playlist are done.  Implies QuietMode=true/--quiet. Implies SimMode=true/--simulate unless SimMode=false/--no-simulate.", typeof(VerbosityAndSimGroup), @"--print-to-file")]
			public PrintAndTemplateFileOpt PlayList
			{
				get;
			} = new(@"playlist");


			/// <summary>
			/// Lists all options in <see cref="PrintToFileGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options. </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					PreProcess,
					AfterFilter,
					Vid,
					BeforeDownLoad,
					PostProcess,
					AfterMove,
					AfterVid,
					PlayList,
				];
		}

		/// <summary>
		/// Groups options that specify a template string for displaying progress.
		/// </summary>
		public sealed class ProgressTemplateGroup
		{
			public ProgressTemplateGroup()
			{
			}

			public ProgressTemplateGroup(in ProgressTemplateGroup copyThis)
			{
				DownLoad = copyThis.DownLoad;
				DownLoadTitle = copyThis.DownLoadTitle;
				PostProcess = copyThis.PostProcess;
				PostProcessTitle = copyThis.PostProcessTitle;
			}


			/// <summary>
			/// Specifies the template for displaying the progress of a download.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. <c><see cref="DownLoad"/>="%(info.id)s-%(progress.eta)s"</c>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationProgressTemplateDownLoad), @"Specifies the template for displaying the progress of a download.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. DownLoad=""%(info.id)s-%(progress.eta)s"".", typeof(VerbosityAndSimGroup), @"--progress-template")]
			public OneOptWithPrefix<string> DownLoad
			{
				get;
			} = new(string.Empty, @"--progress-template", @"download", @"progress_template");

			/// <summary>
			/// Specifies the template for displaying the progress of a download in the titlebar.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. <c><see cref="DisplayProgressInConsoleTitle"/>=<see langword="true"/>; <see cref="DownLoadTitle"/>="%(info.id)s-%(progress.eta)s"</c>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationProgressTemplateDownLoadTitle), @"Specifies the template for displaying the progress of a download in the titlebar.  The video’s fields are accessible under the “info” key and  the progress attributes are accessible under “progress” key.  E.g. DisplayProgressInConsoleTitle=true; DownLoadTitle=""%(info.id)s-%(progress.eta)s"".", typeof(VerbosityAndSimGroup),
				@"--progress-template")]
			public OneOptWithPrefix<string> DownLoadTitle
			{
				get;
			} = new(string.Empty, @"--progress-template", @"download-title", @"progress_template");

			/// <summary>
			/// Specifies the template for displaying the progress of post-processing.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. <c><see cref="PostProcess"/>=""%(info.id)s-%(progress.eta)s"</c>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationProgressTemplatePostProcess), @"Specifies the template for displaying the progress of post-processing.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. PostProcess=""%(info.id)s-%(progress.eta)s"".", typeof(VerbosityAndSimGroup), @"--progress-template")]
			public OneOptWithPrefix<string> PostProcess
			{
				get;
			} = new(string.Empty, @"--progress-template", @"postprocess", @"progress_template");

			/// <summary>
			/// Specifies the template for displaying the progress of post-processing in the title bar.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. <c><see cref="DisplayProgressInConsoleTitle"/>=<see langword="true"/>; <see cref="PostProcessTitle"/>="%(info.id)s-%(progress.eta)s"</c>.
			/// </summary>
			[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationProgressTemplatePostProcessTitle), @"Specifies the template for displaying the progress of post-processing in the title bar.  The video’s fields are accessible under the “info” key and the progress attributes are accessible under “progress” key.  E.g. DisplayProgressInConsoleTitle=true; PostProcessTitle=""%(info.id)s-%(progress.eta)s"".", typeof(VerbosityAndSimGroup), @"--progress-template")]
			public OneOptWithPrefix<string> PostProcessTitle
			{
				get;
			} = new(string.Empty, @"postprocess-title", @"post_process", @"progress_template");


			/// <summary>
			/// Lists all options in <see cref="ProgressTemplateGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
			/// </summary>
			public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
				=> [
					DownLoad,
					DownLoadTitle,
					PostProcess,
					PostProcessTitle,
				];
		}

		/// <summary>
		/// Lists various modes that can be used to display progress.
		/// </summary>
		public enum ProgressModes
		{
			/// <summary>
			/// Allows yt-dlp to select a build-dependent method of displaying the progress of a download unless a config file uses --newline, --no-progress, or
			/// --progress.
			/// </summary>
			@default,

			/// <summary>
			/// Disables display of progress indicators.
			/// </summary>
			none,

			/// <summary>
			/// Causes yt-dlp to start a new line in the console whenever it updates the progress of a download.
			/// </summary>
			newLines,

			/// <summary>
			/// Show the progress bar even in quiet mode (<see cref="QuietMode"/>).
			/// </summary>
			progressBarEvenInQuietMode,
		}


		/// <summary>
		/// If <see langword="true"/>, quiet mode will be on.  If you turn on VerboseMode or a config file uses -v/--verbose, the log written to stderr.  If <see langword="false"/>, quiet mode will be disabled.  The default value of <see langword="null"/> allows the config file value to take precedence if one specifies --quiet or --no-quiet.  Otherwise, yt-dlp will act as though QuietMode is <see langword="false"/>.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationQuietMode), @"If true, quiet mode will be on.  If you turn on VerboseMode or a config file uses -v/--verbose, the log written to stderr.  If false, quiet mode will be disabled.  The default value of null allows the config file value to take precedence if one specifies --quiet or --no-quiet.  Otherwise, yt-dlp will act as though QuietMode is false.", typeof(VerbosityAndSimGroup),
			@"--quiet", @"--no-quiet")]
		public ThreeWayOpt QuietMode
		{
			get;
		} = new(@"--quiet", @"--no-quiet")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.Val == true
						? new DictionaryToOpt()
						{
							[@"quiet"] = true,
							[@"noprogress"] = true,
						}
						: [],
		};

		/// <summary>
		/// Causes warnings to be suppressed if true.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationNoWarnings), @"Causes warnings to be suppressed if true.", typeof(VerbosityAndSimGroup), @"--no-warnings")]
		public OneOpt<bool> NoWarnings
		{
			get;
		} = new(false, @"--no-warnings")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
						{
							[@"no_warnings"] = true,
						}
						: [],
		};

		/// <summary>
		/// If <see langword="true"/>, everything will be simulated except as noted below.  If <see langword="false"/>, everything happens for real.  If you use the default value of <see langword="null"/>, the value form the config file takes precedence.  If yt-dlp still doesn’t have a value, it acts as though <see cref="SimMode"/> is <see langword="false"/>.  Note: If <see cref="GeneralGroup.MarkWatched"/> is true or a config file specifies --mark-watched, videos will be marked as watched even in simulate mode unless you set <see cref="GeneralGroup.MarkWatched"/> to <see langword="false"/>!!!
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationSimMode), @"If true, everything will be simulated except as noted below.  If false, everything happens for real.  If you use the default value of null, the value form the config file takes precedence.  If yt-dlp still doesn't have a value, it acts as though SimMode is false.  Note: If General.MarkWatched is true or a config file specifies --mark-watched, videos will be marked as watched even in simulate mode unless you set General.MarkWatched to false!!!", typeof(VerbosityAndSimGroup), @"--simulate",
			@"--no-simulate")]
		public ThreeWayOpt SimMode
		{
			get;
		} = new(@"--simulate", @"--no-simulate", @"simulate", ThreeWayOpt.WhichValsToSendToPythonChoices.both);

		/// <summary>
		/// If <see langword="true"/>, yt-dlp will continue to process items that have no available formats.  If <see langword="false"/>, it will immediately fail.  If you use the default value of <see langword="null"/>, yt-dlp will act as though <see cref="IgnoreNoFmtsError"/> is <see langword="false"/> unless a config file species either --ignore-no-formats-error or --no-ignore-no-formats-error.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationIgnoreNoFmtsError), @"If true, yt-dlp will continue to process items that have no available formats.  If false, it will immediately fail.  If you use the default value of null, yt-dlp will act as though IgnoreNoFmtsError is false unless a config file species either --ignore-no-formats-error or --no-ignore-no-formats-error.", typeof(VerbosityAndSimGroup),
			@"--ignore-no-formats-error", @"--no-ignore-no-formats-error")]
		public ThreeWayOpt IgnoreNoFmtsError
		{
			get;
		} = new(@"--ignore-no-formats-error", @"--no-ignore-no-formats-error", @"ignore_no_formats_error", ThreeWayOpt.WhichValsToSendToPythonChoices.trueOnly);

		/// <summary>
		/// Do not download the video but write all related files.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationSkipDownLoad), @"Do not download the video but write all related files.", typeof(VerbosityAndSimGroup), @"--skip-download")]
		public OneOpt<bool> SkipDownLoad
		{
			get;
		} = new(false, @"--skip-download")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
						{
							[@"skip_download"] = true,
						}
						: [],
		};

		/// <summary>
		/// Provides a <see cref="PrintTemplatesGroup"/> instance.
		/// </summary>
		public PrintTemplatesGroup PrintTemplates
		{
			get;
		} = new();

		/// <summary>
		/// Provides a <see cref="PrintAndTemplateFileOpt"/> instance.
		/// </summary>
		public PrintToFileGroup PrintToFile
		{
			get;
		} = new();

		/// <summary>
		/// If you set this to <see cref="ProgressModes.@default"/>, yt-dlp uses the mode set in that build unless a config file changes it.  If you use <see cref="ProgressModes.none"/>, yt-dlp won't display the progress.  If you use <see cref="ProgressModes.newLines"/>, yt-dlp will print a new line every time it updates the progress.  If you use <see cref="ProgressModes.progressBarEvenInQuietMode"/>, quiet mode won't disable the progress bar.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationProgressMode), @"If you set this to ProgressModes.@default, yt-dlp uses the mode set in that build unless a config file changes it.  If you use ProgressModes.none, yt-dlp won't display the progress.  If you use ProgressModes.newLine, yt-dlp will print a new line every time it updates the progress.  If you use ProgressModes.ProgressEvenInQuietMode, quiet mode won't disable the progress bar.", typeof(VerbosityAndSimGroup), @"--no-progress", @"--newline", @"--progress")]
		public OneOpt<ProgressModes> ProgressMode
		{
			get;
		} = new(ProgressModes.@default, string.Empty)
		{
			ParamListGenerator =
				opt
					=> opt.CurVal switch
					{
						ProgressModes.@default
							=> [],

						ProgressModes.none
							=> [@"--no-progress"],

						ProgressModes.newLines
							=> [@"--newline"],

						ProgressModes.progressBarEvenInQuietMode
							=> [@"--progress"],

						_
							=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<ProgressModes>(opt.CurVal, @"While selecting a parameter based on the progress " +
								@"bar mode"),
					},

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal switch
					{
						ProgressModes.@default or ProgressModes.progressBarEvenInQuietMode
							=> [],

						ProgressModes.none
							=> new DictionaryToOpt()
							{
								[@"noprogress"] = true,
							},

						ProgressModes.newLines
							=> new DictionaryToOpt()
							{
								[@"progress_with_newline"] = true,
							},

						_
							=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<ProgressModes>(opt.CurVal, @"While selecting a parameter based on the progress " +
								@"bar mode"),
					},
		};

		/// <summary>
		/// Display progress in the console title bar.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationDisplayProgressInConsoleTitle), @"Display progress in the console title bar.", typeof(VerbosityAndSimGroup), @"--console-title")]
		public OneOpt<bool> DisplayProgressInConsoleTitle
		{
			get;
		} = new(false, @"--console-title")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
						{
							[@"consoletitle"] = true,
						}
						: [],
		};

		/// <summary>
		/// Provides an instance of <see cref="ProgressTemplateGroup"/>
		/// </summary>
		public ProgressTemplateGroup ProgressTemplates
		{
			get;
		} = new();

		/// <summary>
		/// Force download archive entries to be written as far as no errors occur, even if <see cref="SimMode"/>/-s/--simulate or another simulation option is used.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationForceWriteArchive), @"Force download archive entries to be written as far as no errors occur, even if SimMode/-s/--simulate or another simulation option is used.", typeof(VerbosityAndSimGroup), @"--force-write-archive")]
		public OneOpt<bool> ForceWriteArchive
		{
			get;
		} = new(false, @"--force-write-archive")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
						{
							[@"force_write_download_archive"] = true,
						}
						: [],
		};

		/// <summary>
		/// Time between progress output.  The default lets yt-dlp choose unless a config file uses --progress-delta.  Use <c><see cref="DeltaOfProgressUpdates"/>=0</c> to force yt-dlp to use its own default.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationDeltaOfProgressUpdates), @"Time between progress output.  The default lets yt-dlp choose unless a config file uses --progress-delta.  Use DeltaOfProgressUpdates=0 to force yt-dlp to use its own default.", typeof(VerbosityAndSimGroup), @"--progress-delta")]
		public OneOpt<double?> DeltaOfProgressUpdates
		{
			get;
		} = new(null, @"--progress-delta")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt is null || opt.CurVal < 0
						? []
						: new DictionaryToOpt()
						{
							[@"progress_delta"] = opt.CurVal,
						},
		};

		/// <summary>
		/// Print downloaded pages encoded using base64 to debug problems (very verbose)
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationDumpPages), @"Print downloaded pages encoded using base64 to debug problems (very verbose)", typeof(VerbosityAndSimGroup), @"--dump-pages")]
		public OneOpt<bool> DumpPages
		{
			get;
		} = new(false, @"--dump-pages")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
						{
							[@"dump_intermediate_pages"] = true,
						}
						: [],
		};

		/// <summary>
		/// Write downloaded intermediary pages to files in the current directory to debug problems
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationWritePages), @"Write downloaded intermediary pages to files in the current directory to debug problems", typeof(VerbosityAndSimGroup), @"--write-pages")]
		public OneOpt<bool> WritePages
		{
			get;
		} = new(false, @"--write-pages")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
						{
							[@"write_pages"] = true,
						}
						: [],
		};

		/// <summary>
		/// Display sent and read HTTP traffic
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strVerbosityAndSimulationPrintTraffic), @"Display sent and read HTTP traffic", typeof(VerbosityAndSimGroup), @"--print-traffic")]
		public OneOpt<bool> PrintTraffic
		{
			get;
		} = new(false, @"--print-traffic")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
						{
							[@"debug_printtraffic"] = true,
						}
						: [],
		};


		/// <summary>
		/// Lists all options in a <see cref="VerbosityAndSimGroup"/> instance.  The parent of this instance will use <see cref="AllOpt"/> to build its own list.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				QuietMode,
				NoWarnings,
				SimMode,
				IgnoreNoFmtsError,
				SkipDownLoad,
				..PrintTemplates.AllOpt,
				..PrintToFile.AllOpt,
				ProgressMode,
				DisplayProgressInConsoleTitle,
				..ProgressTemplates.AllOpt,
				ForceWriteArchive,
				DeltaOfProgressUpdates,
				DumpPages,
				WritePages,
				PrintTraffic,
			];
	}
}