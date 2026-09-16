namespace NovoDwarf.Utilities.Extensions;

public static class ArgumentExceptionExtesions
{
	extension(ArgumentException)
	{
		public static void ThrowIfNullOrEmpty<T>(IList<T>? list)
		{
			ArgumentNullException.ThrowIfNull(list);

			if (list.Count == 0)
				throw new ArgumentException("List cannot be empty");
		}
	}
}