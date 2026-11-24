namespace NovoDwarf.LeetCode.Problems.Solved;

public static class PalindromNumber
{
	// TODO: simple solve with strings... maybe try without it?
	
	public static bool Solve(int x)
	{
		var s = x.ToString().ToCharArray();
		var left = 0;
		var right = s.Length - 1;

		while (left < right)
		{
			if (s[left] != s[right])
				return false;
			
			left++;
			right--;
		}

		return true;
	}
}