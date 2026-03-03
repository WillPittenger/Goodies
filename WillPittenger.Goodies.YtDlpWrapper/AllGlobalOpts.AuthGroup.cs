// Ignore Spelling: Dlp Vals uri Sel wget Xattr Hls Sys Arounds Vid Fmts Pwd Executables Evt Exe Evts Loc Locs Json Dest Hdrs Langs

namespace WillPittenger.Goodies.YtDlpWrapper;

using IReadOnlyDictionaryToOptLists = System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<object>?>;
using DictionaryToOptLists = System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<object>?>;
using IReadOnlyDictionaryToOpt = System.Collections.Generic.IReadOnlyDictionary<string, object?>;
using DictionaryToOpt = System.Collections.Generic.Dictionary<string, object?>;

public partial class AllGlobalOpts
{
	/// <summary>
	/// Groups various options related to authentication.  All correspond to options from <a href="https://github.com/yt-dlp/yt-dlp/blob/master/README.md#authentication-options">Authentication Options section of yt-dlp’s readme.md</a>.
	/// </summary>
	public sealed class AuthGroup
	{
		public AuthGroup()
		{
		}

		public AuthGroup(in AuthGroup copyThis)
		{
			UserNameAndPwd = new(copyThis.UserNameAndPwd);
			TwoFactor = new(copyThis.TwoFactor);
			NetRC = new(copyThis.NetRC);
			VidSpecificPwd = new(copyThis.VidSpecificPwd);
		}


		/// <summary>
		/// Represents a user name and password combination in a single value.  Note: This value is ignored if either <see cref="UserName"/> or <see cref="Pwd"/> are empty.
		/// </summary>
		public sealed class UserNameAndPwdOpt : BaseOneOpt<UserNameAndPwdOpt>
		{
			public UserNameAndPwdOpt()
				=> PythonParamsGenerator = GetPythonParams;

			public UserNameAndPwdOpt(in UserNameAndPwdOpt copyThis)
			{
				PythonParamsGenerator = GetPythonParams;

				UserName = copyThis.UserName;
				Pwd = copyThis.Pwd;
			}


			/// <summary>
			/// The user name
			/// </summary>
			public string UserName
			{
				get;

				set;
			} = string.Empty;

			/// <summary>
			/// The password
			/// </summary>
			public string Pwd
			{
				get;

				set;
			} = string.Empty;

			/// <inheritdoc/>
			public override System.Collections.Generic.IEnumerable<string> Params
			{
				get
				{
					if(UserName == string.Empty && Pwd != string.Empty)
						throw new System.InvalidOperationException("If you specify a password, you must specify a user name.");

#pragma warning disable IDE0046 // Convert to conditional expression
					if(Pwd == string.Empty && UserName != string.Empty)
						throw new System.InvalidOperationException("If you specify a user name, you must specify a password.");
#pragma warning restore IDE0046 // Convert to conditional expression

					return [@"--username", UserName, @"--password", Pwd];
				}
			}

			internal override DictionaryToOpt? PythonParams
				=> PythonParamsGenerator?.Invoke(this);

			private DictionaryToOpt? GetPythonParams(in UserNameAndPwdOpt opt)
				=> opt.UserName == string.Empty || opt.Pwd == string.Empty
					? []
					: new DictionaryToOpt()
					{
						[@"username"] = opt.UserName,
						[@"password"] = opt.Pwd,
					};
		}


		/// <summary>
		/// Specifies how to log in with user name and password.  Note: If you use a user name, you must provide a password and vice-versa.  Support depends on the website.
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strAuthenticationUserNameAndPwd), @"Specifies how to log in with user name and password.  Note: If you use a user name, you must provide a password and vice-versa.  Support depends on the website.", typeof(AuthGroup), @"--username", @"--password")]
		public UserNameAndPwdOpt UserNameAndPwd
		{
			get;
		} = new();

		/// <summary>
		/// Two-factor authentication code
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strAuthenticationTwoFactor), @"Two-factor authentication code", typeof(AuthGroup), @"--twofactor")]
		public OneOpt<string> TwoFactor
		{
			get;
		} = new(string.Empty, @"--twofactor", @"twofactor");

		/// <summary>
		/// Use .netrc authentication data
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strAuthenticationNetRC), @"Use .netrc authentication data", typeof(AuthGroup), @"--netrc")]
		public OneOpt<bool> NetRC
		{
			get;
		} = new(false, @"--netrc")
		{
			PythonParamsGenerator =
				(in opt)
					=> opt.CurVal
						? new DictionaryToOpt()
						{
							[@"usenetrc"] = true,
						}
						: [],
		};

		/// <summary>
		/// Video-specific password
		/// </summary>
		[YtDlpOptDoc(nameof(Rsrcs.strAuthenticationVidSpecificPwd), @"Video-specific password", typeof(AuthGroup), @"--video-password")]
		public OneOpt<string> VidSpecificPwd
		{
			get;
		} = new("", @"--video-password", @"videopassword");


		/// <summary>
		/// Lists all options in <see cref="AuthGroup"/>.  The owner of this instance will use <see cref="AllOpt"/> to build its own list of options.
		/// </summary>
		public System.Collections.Generic.IEnumerable<BaseOneOpt> AllOpt
			=> [
				UserNameAndPwd,
				TwoFactor,
				NetRC,
				VidSpecificPwd,
			];
	}
}