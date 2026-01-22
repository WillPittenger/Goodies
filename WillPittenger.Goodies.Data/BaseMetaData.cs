namespace WillPittenger.Goodies.Data;

[System.ComponentModel.ImmutableObject(true)]
public abstract record BaseMetaData
{
	public static class FieldNames
	{
		public const string strKind = @"Kind";
	}
}