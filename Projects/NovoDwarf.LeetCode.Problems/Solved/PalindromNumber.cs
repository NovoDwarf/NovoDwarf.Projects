namespace NovoDwarf.LeetCode.Problems.Solved;

public class PalindromNumber
{
	// TODO: simple solve with strings... maybe try without it?
	
	public bool Solve(int x)
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