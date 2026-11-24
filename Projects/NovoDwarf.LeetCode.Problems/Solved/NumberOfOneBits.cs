namespace NovoDwarf.LeetCode.Problems.Solved;

public static class NumberOfOneBits
{
	public static int Solve(int n)
	{
		var counter = 0;
		
		while (n != 0)
		{
			n &= n - 1;
			counter++;
		}

		return counter;
	}
}