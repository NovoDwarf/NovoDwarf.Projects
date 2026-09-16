using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NovoDwarf.Utilities.Extensions;

public static class ObservableCollectionExtensions
{
	extension<TSource>(ObservableCollection<TSource> source)
	{
		public ObservableCollection<TSource> Sort<TKey>(Func<TSource, TKey> keySelector)
		{
			var sortedList = source.OrderBy(keySelector).ToList();
		
			source.Clear();
		
			foreach (var sortedItem in sortedList)
				source.Add(sortedItem);

			return source;
		}
	}
}