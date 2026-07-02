using System.Globalization;
using Aegis.Packaging.Core.Entities;

namespace Aegis.Packaging.Services;

public static class PackageSort
{
	public static int CompareForLoadOrder(PackageInstance? a, PackageInstance? b)
	{
		if (ReferenceEquals(a, b))
			return 0;

		if (a is null)
			return -1;

		if (b is null)
			return 1;

		var user = a.UserOrderIndex.CompareTo(b.UserOrderIndex);
		
		if (user != 0)
			return user;
		
		var id = string.Compare(a.Id, b.Id, CultureInfo.InvariantCulture, CompareOptions.OrdinalIgnoreCase);
		
		return id != 0
			? id
			: string.Compare(a.RootPath, b.RootPath, CultureInfo.InvariantCulture, CompareOptions.OrdinalIgnoreCase);
	}
}