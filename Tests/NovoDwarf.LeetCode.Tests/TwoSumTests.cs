using NovoDwarf.LeetCode.Problems.Solved;

namespace NovoDwarf.LeetCode.Tests;

public class TwoSumTests
{
	[TestCase(new[] { 2, 7, 11, 15 }, 9)]
	[TestCase(new[] { 3, 2, 4 }, 6)]
	[TestCase(new[] { 3, 3 }, 6)]
	[TestCase(new[] { -3, 4, 3, 90 }, 0)]
	[TestCase(new[] { 0, 4, 3, 0 }, 0)]
	public void Solve_ValidInput_ReturnsValidPair(int[] nums, int target)
	{
		var result = TwoSum.Solve(nums, target);

        using (Assert.EnterMultipleScope())
        {
			Assert.That(result, Has.Length.EqualTo(2));

			Assert.That(result[0], Is.InRange(0, nums.Length - 1));
			Assert.That(result[1], Is.InRange(0, nums.Length - 1));

			Assert.That(result[0], Is.Not.EqualTo(result[1]));

			Assert.That((long)nums[result[0]] + nums[result[1]], Is.EqualTo((long)target));
		}
	}
}