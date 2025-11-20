using System.Text;

namespace Mathematics.SourceGenerators.Utilities;

public class StringUtils
{
	public static string SplitPascalCase(string input)
	{
		if (string.IsNullOrEmpty(input))
			return string.Empty;

		var words = new List<string>();
		var currentWord = new StringBuilder();

		for (var i = 0; i < input.Length; i++)
		{
			if (i > 0 && char.IsUpper(input[i]))
			{
				words.Add(currentWord.ToString());
				currentWord.Clear();
			}
			currentWord.Append(input[i]);
		}

		if (currentWord.Length > 0)
			words.Add(currentWord.ToString());

		return string.Join("_", words.ToArray());
	}
}