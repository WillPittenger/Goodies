namespace WillPittenger.Goodies.YtDlpWrapper.Exceptions;

using WillPittenger.Goodies.Tools.Ext;

public class PythonException : System.Exception
{
	internal PythonException(in Reasons reason, in string? strExtraDesc = null, in System.Exception? exInner = null)
		: base(null, exInner)
	{
		this.reason = reason;
		this.strExtraDesc = strExtraDesc;
	}

	public readonly Reasons reason;
	public readonly string? strExtraDesc;

	public enum Reasons : byte
	{
		pythonMissing,
		pythonNotNewEnough,
		ytDlpNotFoundByPython,
		failedToStartPython,
		ytDlpNotReady,
	}

	public override string Message
		=> reason switch
			{
				Reasons.pythonMissing
					=> strExtraDesc == null
						? Rsrcs.strPythonNotFound
						: Rsrcs.strPythonNotFoundExtra.Fmt(strExtraDesc),

				Reasons.pythonNotNewEnough
					=> strExtraDesc == null
						? Rsrcs.strPythonNotNewEnough
						: Rsrcs.strPythonNotNewEnoughExtra.Fmt(strExtraDesc),

				Reasons.ytDlpNotFoundByPython
					=> strExtraDesc == null
						? Rsrcs.strPythonDidNotFindYtDlp
						: Rsrcs.strPythonDidNotFindYtDlpExtra.Fmt(strExtraDesc),

				Reasons.failedToStartPython
					=> strExtraDesc == null
						? Rsrcs.strPythonDidNotStart
						: Rsrcs.strPythonDidNotStartExtra.Fmt(strExtraDesc),

				Reasons.ytDlpNotReady
					=> Rsrcs.strPythonYtDlpNotReady,

				_
					=> throw new Tools.Exceptions.UnknownOrInvalidEnumException<Reasons>(reason, @"While describing another exception"),
			};
}