namespace NovoDwarf.LeetCode.Problems.Solved;

public class TwoSum
{
	public static int[] Solve(int[] nums, int target)
	{
		var dict = new Dictionary<int, int>();

		for (var index = 0; index < nums.Length; index++) 
		{
			var comp = target - nums[index];

			if (dict.TryGetValue(comp, out var compIndex))
				return [compIndex, index];

			dict[nums[index]] = index;
		}

		throw new InvalidOperationException("No valid pair founded");
	}
}