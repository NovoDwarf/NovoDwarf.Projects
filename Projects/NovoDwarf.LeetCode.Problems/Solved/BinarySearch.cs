namespace NovoDwarf.LeetCode.Problems.Solved;

public static class BinarySearch
{
	public static int Solve(int[] nums, int target)
	{
		var left = 0;
		var right = nums.Length - 1;

		while (left <= right)
		{
			var mid = left + (right - left) / 2;

			if (nums[mid] < target)
				left = mid + 1;
			else if (nums[mid] > target)
				right = mid - 1;
			else
				return mid;
		}

		return -1;
	}
}