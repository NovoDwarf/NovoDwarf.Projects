namespace NovoDwarf.LeetCode.Problems.Solved;

public static class LongestCommonPrefix
{
	public static string Solve(string[] strs)
	{
		var str0 = strs[0];
		var index = str0.Length;

		if (index == 0)
			return string.Empty;
			
		for (var i = 1; i < strs.Length; i++)
		{
			var str = strs[i];
			var j = 0;

			while (j < str.Length && j < index)
			{
				var curr = str[j];

				if (str0[j] != curr)
					break;

				j++;
			}

			index = Math.Min(index, j);
		}

		return str0[..index];
	}
}