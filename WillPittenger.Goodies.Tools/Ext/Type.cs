namespace WillPittenger.Goodies.Tools.Ext;

public static class Type
{
	public static bool IsDerivedFrom(this System.Type typeDerivedTypeToTest, System.Type typeBaseTypeToTest)
		=> typeDerivedTypeToTest == null
			? throw new System.ArgumentNullException(nameof(typeDerivedTypeToTest), @"While testing for derivation")
			: typeDerivedTypeToTest == typeBaseTypeToTest || typeDerivedTypeToTest.BaseType == typeBaseTypeToTest ||
				(typeDerivedTypeToTest.BaseType != null && typeDerivedTypeToTest.BaseType != typeof(object) &&
				typeDerivedTypeToTest.BaseType.IsDerivedFrom(typeBaseTypeToTest));
}