namespace WillPittenger.Goodies.Tools.Ext;

public static class DateTime
{
	public static System.DateOnly AsDateOnly(in this System.DateTime dtIn)
		=> new(dtIn.Year, dtIn.Month, dtIn.Day);
}