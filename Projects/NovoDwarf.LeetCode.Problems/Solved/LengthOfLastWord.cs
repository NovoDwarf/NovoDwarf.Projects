namespace NovoDwarf.LeetCode.Problems.Solved;

public class LengthOfLastWord
{
	public static int Solve(string s)
	{
		var counter = 0;

		for (var index = s.Length - 1; index >= 0; index--)
		{
			var c = s[index];
		
			switch (c)
			{
				case ' ' when counter == 0: continue;
				case ' ' when counter > 0: return counter;
			}

			if (c != ' ')
				counter++;
		}

		return counter;
	}
}