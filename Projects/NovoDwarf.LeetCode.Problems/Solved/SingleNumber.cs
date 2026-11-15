namespace NovoDwarf.LeetCode.Problems.Solved;

public static class SingleNumber
{
	public static int Solve(int[] nums)
	{
		var result = 0;

		foreach (var num in nums)
		{
			result ^= num;
		}

		return result;
	}
}