namespace NovoDwarf.LeetCode.Problems.Solved;

public class MajorityElement
{
	public static int Solve(int[] nums)
	{
		var dict = new Dictionary<int, int>();
		var threshold = nums.Length / 2;

		foreach (var num in nums)
		{
			if (!dict.TryAdd(num, 1))
				dict[num]++;

			if (dict[num] > threshold)
				return num;
		}

		return -1;
	}
}