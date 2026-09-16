using System.Text.RegularExpressions;

namespace NovoDwarf.Utilities.Extensions;

public static partial class StringExtensions
{
	[GeneratedRegex("(?<=[a-z])([A-Z])")]
	private static partial Regex SnakeCase();
	
    extension(string str)
	{
		public string SnakeCase()
		{
			return SnakeCase().Replace(str, "_$1").ToLower();
		}


    }
}