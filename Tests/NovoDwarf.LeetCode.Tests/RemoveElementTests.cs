using NovoDwarf.LeetCode.Problems.Solved;

namespace NovoDwarf.LeetCode.Tests;

public class RemoveElementTests
{
	[TestCase(new[] { 3, 2, 2, 3}, 3)]
	public void Solve_ValidInput_ReturnsValidPair(int[] nums, int val)
	{
		var result = RemoveElement.Solve(nums, val);

		using (Assert.EnterMultipleScope())
		{
			
		}
	}
}