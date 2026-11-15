namespace NovoDwarf.LeetCode.Problems.Solved;

/// <summary>
/// Given an integer n, return >true if it is a power of two. Otherwise, return false. <br/>
/// An integer n is a power of two, if there exists an integer x such that n == 2x.
/// </summary>
public static class PowerOfTwo
{
	/// <summary>
	/// 8 -> 0000 1000 <br/>
	/// 8 - 1 -> 7 -> 0000 0111 <br/>
	/// 8 & 7 -> 0 -> 0000 0000 -> true <br/>
	/// <br/>
	/// 12 -> 0000 1100 <br/>
	/// 12 - 1 -> 11 -> 0000 0111 <br/>
	/// 12 & 11 -> 0000 0100 -> false <br/>
	/// </summary>
	/// <param name="n"></param>
	/// <returns></returns>
	public static bool Solve(int n)
	{
		return n > 0 && (n & (n - 1)) == 0;
	}
}