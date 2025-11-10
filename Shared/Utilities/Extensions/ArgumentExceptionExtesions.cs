using System;
using System.Collections.Generic;

namespace Utilities.Extensions;


public static class ArgumentOutOfRangeExceptionExtensions
{
	extension(ArgumentOutOfRangeException)
	{
		public static void ThrowIfOutOfRange(IList<double> list, int min, int max)
		{
			foreach (var item in list)
			{
				if (item < min || item > max)
					throw new ArgumentException($"Item {item} is out of range [{min}, {max}]");
			}
		}
		
		public static void ThrowIfEmptyOrLess<T>(IList<T> list, int count)
		{
			ArgumentException.ThrowIfNullOrEmpty(list);
			
			if (list.Count < count)
				throw new ArgumentException($"List must contain at least {count} elements");
		}

		public static void ThrowIfEmptyOrGreater<T>(IList<T>? list, int count)
		{
			ArgumentException.ThrowIfNullOrEmpty(list);
			
			if (list!.Count > count)
				throw new ArgumentException($"List must contain at least {count} elements");
		}
	}
}

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