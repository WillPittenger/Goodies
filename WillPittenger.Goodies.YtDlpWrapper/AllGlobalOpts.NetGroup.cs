namespace WillPittenger.Goodies.YtDlpWrapper;

using System.Linq;

using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Groups all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#network-options">Network Options section of yt-dlp’s readme.md</a>.
	/// </summary>
	[System.ComponentModel.ImmutableObject(true)]
	public sealed class NetGroup
	{
		public NetGroup()
			=> IpAddrMode = new(IpAddrModeVal.anyIpVersion, string.Empty)
			{
				CustomParamNameLookUp
				= typeCurVal
					=> typeCurVal?.ParamName ?? "",

				PythonParamsGenerator =
					(in opt)
						=> SrcIpAddr.CurVal == null && opt.CurVal == IpAddrModeVal.v6Only
							? new DictionaryToOpt()
							{
								[@"source_address"] = @"::",
							}
							: [],
			};

		public NetGroup(in NetGroup copyThis)
		{
			ProxyURL = new(copyThis.ProxyURL);
			SocketTimeOut = new(copyThis.SocketTimeOut);
			SrcIpAddr = new(copyThis.SrcIpAddr);
			IpAddrMode = new(copyThis.IpAddrMode);
		}


		/// <summary>
		/// Lists your options for which type of IP address to use.  The only valid values are static readonly members of <see cref="IpAddrModeVal"/>.
		/// </summary>
		[System.ComponentModel.ImmutableObject(true)]
		public sealed class IpAddrModeVal
		{
			/// <summary>
			/// Constructs new instances.
			/// </summary>
			/// <param name="strParamName">The name of the associated parameter.</param>
			private IpAddrModeVal(in string? strParamName)
				=> ParamName = strParamName;


			/// <summary>
			/// Stores the name of the parameter
			/// </summary>
			public string? ParamName
			{
				get;
			}


			/// <summary>
			/// Force yt-dlp to use IPv4.
			/// </summary>
			public static readonly IpAddrModeVal v4Only = new("-4");

			/// <summary>
			/// Force yt-dlp to use IPv6
			/// </summary>
			public static readonly IpAddrModeVal v6Only = new("-6");

			/// <summary>
			/// Let yt-dlp choose.
			/// </summary>
			public static readonly IpAddrModeVal anyIpVersion = new(null);
		}


		/// <summary>
		/// Specifies a URI that's invalid so <see cref="ProxyURL"/> can have multiple values that are special.  See the documentation on it for more information.
		/// </summary>
		public static readonly System.Uri uriInvalid = new(@"http://invalid");


		/// <summary>
		/// Use the specified HTTP/HTTPS/SOCKS proxy.  To enable SOCKS proxy, specify a proper scheme, e.g. <c>"socks5://user:pass@127.0.0.1:1080/"</c>.   <see langword="null"/> prevents a parameter from being emitted.  The value in <see cref="uriInvalid"/> gives you a direct connection.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strNetProxyURL), @"Use the specified HTTP/HTTPS/SOCKS proxy.  To enable SOCKS proxy, specify a proper scheme,  e.g. socks5://user:pass@127.0.0.1:1080/.  Null prevents a parameter from being emitted.  The value in uriInvalid gives you a direct connection", typeof(NetGroup), @"--proxy")]
		public OneOpt<System.Uri?> ProxyURL
		{
			get;
		} = new(null, @"--proxy")
		{
			ParamListGenerator =
				opt
					=>
						opt.CurVal == null
							? []
							: opt.CurVal == uriInvalid
								? [string.Empty]
								: [opt.CurVal.AbsoluteUri],

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is null
						? []
						: opt.CurVal == uriInvalid
							? new DictionaryToOpt()
							{
								[@"proxy"] = string.Empty,
							}
							: new DictionaryToOpt()
							{
								[@"proxy"] = opt.CurVal.AbsoluteUri,
							},
		};

		/// <summary>
		/// Time to wait before giving up.  The value null prevents this parameter from being emitted.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strNetSocketTimeOut), @"Time to wait before giving up.  The value null prevents this parameter from being emitted.", typeof(NetGroup), @"--socket-timeout")]
		public OneOpt<System.TimeSpan?> SocketTimeOut
		{
			get;
		} = new(null, @"--socket-timeout")
		{
			ValTextLookUp =
				curVal
					=> curVal?.TotalSeconds.ToString(),

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is null
						? []
						: new DictionaryToOpt()
						{
							[@"socket_timeout"] = opt.CurVal.Value.TotalSeconds,
						}
		};

		/// <summary>
		/// Client-side IP address to bind to.  The value null prevents this parameter from being emitted.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strNetIpAddr), @"Client-side IP address to bind to.  The value null prevents this parameter from being emitted.", typeof(NetGroup), @"--socket-address")]
		public OneOpt<System.Net.IPAddress?> SrcIpAddr
		{
			get;
		} = new(null, @"--source-address")
		{
			CustomParamNameLookUp =
				curVal
					=> curVal?.ToString() ?? "",

			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is null
						? []
						: new DictionaryToOpt()
						{
							[@"source_address"] = opt.CurVal.ToString(),
						},
		};

		/// <summary>
		/// Use to select between IP v4 and IP v6.  Get values from <see cref="IpAddrModeVal"/>.  Use <see cref="IpAddrModeVal.v4Only"/> to force IP version 4. Use <see cref="IpAddrModeVal.v6Only"/> to force IP version 6.  Use  <see cref="IpAddrModeVal.anyIpVersion"/> to let yt-dlp select a mode.
		/// </summary>
		/// <description>
		/// Initialized by the constructor unlike other options.
		/// </description>
		[YtDlpOptDoc(nameof(Rsrcs.strNetIPAddrMode), @"Use to select between IP v4 and IP v6.  Get values from IpAddressModeOpt.  Use IpAddressModeOpt.v4 to force IP version 4.  Use IpAddressModeOpt.v6 to force IP version 6.  Use IpAddressModeOpt.anyIpVersion to let yt-dlp select a mode.", typeof(NetGroup), @"--force-ipv4", @"--force-ipv6")]
		public OneOpt<IpAddrModeVal> IpAddrMode
		{
			get;

			set;
		}


		/// <summary>
		/// Lists all options inside <see cref="NetGroup"/>.  Parents of this instance use it to merge their list of options into theirs.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				ProxyURL,
				SocketTimeOut,
				SrcIpAddr,
				IpAddrMode,
			];
	}
}