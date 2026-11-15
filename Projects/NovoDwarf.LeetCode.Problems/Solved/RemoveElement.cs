namespace NovoDwarf.LeetCode.Problems.Solved;

public static class RemoveElement
{
	public static int Solve(int[] nums, int val)
	{
		var write = 0;

		for (var read = 0; read < nums.Length; read++)
		{
			if (nums[read] == val)
				continue;
			
			nums[write] = nums[read];
			write++;
		}

		return write;
	}
}