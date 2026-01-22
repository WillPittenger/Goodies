namespace WillPittenger.Goodies.YtDlpWrapper.Exceptions;

using WillPittenger.Goodies.Tools.Ext;

public class YtDlpException : System.Exception
{
	internal YtDlpException(in Reasons reason, in string? strExtraDesc = null, in System.Exception? exInner = null)
		: base(null, exInner)
	{
		this.reason = reason;
		this.strExtraDesc = strExtraDesc;
	}

	public readonly Reasons reason;
	public readonly string? strExtraDesc;

	public enum Reasons : byte
	{
		missingExe,
		enclosed,
		tooOld,
	}

	public override string Message
		=> reason switch
			{
				Reasons.missingExe
					=> strExtraDesc == null
						? Rsrcs.strYtDlpExceptionReasonMissingExe
						: Rsrcs.strYtDlpExceptionReasonMissingExeExtra.Fmt(strExtraDesc),

				Reasons.enclosed
					=> strExtraDesc == null
						? Rsrcs.strYtDlpExceptionReasonEnclosed
						: Rsrcs.strYtDlpExceptionReasonEnclosedExtra.Fmt(strExtraDesc),

				Reasons.tooOld
					=> strExtraDesc == null
						? Rsrcs.strYtDlpTooOld
						: Rsrcs.strYtDlpTooOldExtra.Fmt(strExtraDesc),

				_
					=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<Reasons>(reason, @"While describing another exception"),
			};
}