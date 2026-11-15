using NovoDwarf.LeetCode.Problems.Solved;

namespace NovoDwarf.LeetCode.Tests;

[TestFixture]
public class FizzBuzzTests
{
	[TestCase(3)]
	public void Solve_ValidInput_ReturnsValidPair(int n)
	{
		var result = FizzBuzz.Solve(n);

		using (Assert.EnterMultipleScope())
		{
			Assert.True(result.Count == n);
		}
	}
}