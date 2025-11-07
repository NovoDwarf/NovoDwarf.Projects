namespace Mathematics.Extensions;

public static class ExceptionExtesions
{
	extension(ArgumentException)
	{
		public static void ThrowIfNullOrEmpty<T>(IList<T>? list)
		{
			if (list == null || list.Count == 0)
				throw new ArgumentException("List cannot be null or empty");
		}

		public static void ThrowIfNullOrLess<T>(IList<T>? list, int count)
		{
			if (list == null || list.Count == 0)
				throw new ArgumentException("List cannot be null or empty");
			
			if (list.Count < count)
				throw new ArgumentException($"List must contain at least {count} elements");
		}
		
		public static void ThrowIfNullOrGreater<T>(IList<T>? list, int count)
		{
			if (list == null || list.Count == 0)
				throw new ArgumentException("List cannot be null or empty");
			
			if (list.Count > count)
				throw new ArgumentException($"List must contain at least {count} elements");
		}
	}
}