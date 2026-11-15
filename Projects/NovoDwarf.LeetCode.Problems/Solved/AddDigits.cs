namespace NovoDwarf.LeetCode.Problems.Solved;

public static class AddDigits
{
	public static int Solve(int num)
	{
		while (num >= 10)
		{
			num = Math.DivRem(num, 10, out var reminder);
			num += reminder;
		}

		return num;
	}
}