namespace NovoDwarf.LeetCode.Problems.Solved;

public static class ContainsDuplicate
{
	public static bool Solve(int[] nums)
	{
		var hashset = new HashSet<int>();
		
		foreach (var num in nums)
		{
			if (!hashset.Add(num))
				return true;
		}
		
		return false;
	}
}