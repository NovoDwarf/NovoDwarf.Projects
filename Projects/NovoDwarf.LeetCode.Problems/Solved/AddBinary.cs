using System.Text;

namespace NovoDwarf.LeetCode.Problems.Solved;

public static class AddBinary
{
	public static string Solve(string a, string b)
	{
		var sb = new StringBuilder();

		var i = a.Length - 1;
		var j = b.Length - 1;
		var carry = 0;

		while (i >= 0 || j >= 0 || carry > 0)
		{
			var iBit = i >= 0 ? a[i] - '0' : 0;
			var jBit = j >= 0 ? b[j] - '0' : 0;

			var sum = iBit + jBit + carry;

			sb.Append(sum % 2);
			carry = sum / 2;

			i--;
			j--;
		}

		var chars = sb.ToString().ToCharArray();
		Array.Reverse(chars);

		return new string(chars);
	}
}