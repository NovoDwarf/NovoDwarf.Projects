using System;
using System.Collections.Generic;
using System.IO;
using Grekov.Packaging.Interfaces;

namespace SultanDynasty.Avalonia.Services;

public sealed class AvaloniaPackageCatalog : IPackageCatalog
{
	public IEnumerable<string> GetPackageRoots()
	{
		yield return Path.Combine(AppContext.BaseDirectory, "Assets");
	}
}
