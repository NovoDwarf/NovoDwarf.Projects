namespace NovoDwarf.LeetCode.Problems.Solved;

public class FizzBuzz
{
	public static IList<string> Solve(int n)
	{
		var list = new string[n];
		
		for (var index = 1; index <= n; index++)
		{
			if (index % 3 == 0 && index % 5 == 0)
				list[index - 1] = "FizzBuzz";
			else if (index % 3 == 0)
				list[index - 1] = "Fizz";
			else if (index % 5 == 0)
				list[index - 1] = "Buzz";
			else
				list[index - 1] = index.ToString();
		}

		return list;
	}
}