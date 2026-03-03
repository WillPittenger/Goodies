namespace WillPittenger.Goodies.YtDlpWrapper;

using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{

	/// <summary>
	/// Groups all options listed in the <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#geo-restriction">Geo-Restrictions Options section of yt-dlp’s readme.md</a>.
	/// </summary>
	[System.ComponentModel.ImmutableObject(true)]
	public sealed class GeoRestrictionGroup
	{
		public GeoRestrictionGroup()
		{
		}

		public GeoRestrictionGroup(in GeoRestrictionGroup copyThis)
		{
			VerificationProxy = new(copyThis.VerificationProxy);
			XFF = new(copyThis.XFF);
		}


		/// <summary>
		/// Use this proxy to verify the IP address for some geo-restricted sites.  The default proxy specified by <see cref="NetGroup.ProxyURL"/> (or none, if the option is  <see langword="null"/>) is used for the actual downloading.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeoRestrictionVerificationProxy), @"Use this proxy to verify the IP address for some geo-restricted sites.  The default proxy specified by Net.ProxyURL (or none, if the option is null) is used for the actual downloading.", typeof(GeoRestrictionGroup), @"--geo-verification-proxy")]
		public OneOpt<System.Uri?> VerificationProxy
		{
			get;

			set;
		} = new(null, @"--geo-verification-proxy")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is null
						? []
						: new DictionaryToOpt()
						{
							[@"proxy"] = opt.CurVal.AbsoluteUri,
						},
		};

		/// <summary>
		/// How to fake X-Forwarded-For HTTP header to try bypassing geographic restriction.  One of “default” (only when known to be useful), “never”, an IP block in CIDR notation, or a two-letter ISO 3166-2 country code.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strGeoRestrictionXFF), @"How to fake X-Forwarded-For HTTP header to try bypassing geographic restriction.  One of “default” (only when known to be useful), “never”, an IP block in CIDR notation, or a two-letter ISO 3166-2 country code.", typeof(GeoRestrictionGroup),
			@"--xff")]
		public OneOpt<string> XFF
		{
			get;
		} = new(string.Empty, @"--xff")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal is null
						? []
						: new DictionaryToOpt()
						{
							[@"geo-bypass"] = opt.CurVal,
						},
		};


		/// <summary>
		/// Lists all options inside <see cref="GeoRestrictionGroup"/>.  The parents of this instance will use <see cref="AllOpt"/> to build their own list of all options.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				VerificationProxy,
				XFF,
			];
	}
}