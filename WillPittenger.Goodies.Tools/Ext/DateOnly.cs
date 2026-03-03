namespace WillPittenger.Goodies.Tools.Ext;

public static class DateOnly
{
	public static System.DateTime AsDateTime(in this System.DateOnly doIn)
		=> new(doIn.Year, doIn.Month, doIn.Day);
}