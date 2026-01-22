namespace WillPittenger.Goodies.Tools.Exceptions;

public static class AssertOrThrow
{
	public static void TestIt(bool bCondition, System.Action actThrower)
	{
		System.Diagnostics.Debug.Assert(bCondition);

		if(!bCondition)
			actThrower();
	}
}