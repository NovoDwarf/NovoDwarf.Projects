using System.Text;

namespace NovoDwarf.LeetCode.Problems.Solved;

public static class AddStrings
{
	public static string Solve(string num1, string num2)
	{
		var sb = new StringBuilder();

		var i = num1.Length - 1;
		var j = num2.Length - 1;
		var carry = 0;

		while (i >= 0 || j >= 0 || carry > 0)
		{
			var a = i >= 0 ? num1[i] - '0' : 0;
			var b = j >= 0 ? num2[j] - '0' : 0;

			var sum = a + b + carry;

			sb.Append(sum % 10);
			carry = sum / 10;

			i--;
			j--;
		}

		var chars = sb.ToString().ToCharArray();
		Array.Reverse(chars);

		return new string(chars);
	}
}