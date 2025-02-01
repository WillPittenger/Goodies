namespace WillPittenger.Goodies.Tools.Ext;

/// <summary>
/// Contains an extension method for testing if a type is derived from a second type or not.
/// </summary>
public static class Type
{
	/// <summary>
	/// Tests if two types are related via derivation.  
	/// </summary>
	/// <param name="typeDerivedTypeToTest">The unknown type you want to test</param>
	/// <param name="typeBaseTypeToTest">What might be the base type.  This can occur in any level of the derivation chain.</param>
	/// <returns><see langword="true"/> if <paramref name="typeDerivedTypeToTest"/> derives from <paramref name="typeBaseTypeToTest"/> and <see langword="false"/>
	/// otherwise.</returns>
	/// <exception cref="System.ArgumentNullException"><paramref name="typeDerivedTypeToTest"/> is <see langword="null"/></exception>
	public static bool IsDerivedFrom(this System.Type typeDerivedTypeToTest, System.Type typeBaseTypeToTest)
		=> typeDerivedTypeToTest == null
			? throw new System.ArgumentNullException(nameof(typeDerivedTypeToTest), @"While testing for derivation")
			: typeDerivedTypeToTest == typeBaseTypeToTest || typeDerivedTypeToTest.BaseType == typeBaseTypeToTest ||
				(typeDerivedTypeToTest.BaseType != null && typeDerivedTypeToTest.BaseType != typeof(object) &&
				typeDerivedTypeToTest.BaseType.IsDerivedFrom(typeBaseTypeToTest));
}