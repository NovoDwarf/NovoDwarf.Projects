using System;
using System.Collections.Generic;
using Grekov.Packaging.Interfaces;

namespace SultanDynasty.Avalonia.Services;

public sealed class AvaloniaPackageLoadOrderStore : IPackageLoadOrderStore
{
	public IReadOnlyDictionary<string, bool> LoadEnabledOverrides()
	{
		return new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
	}
}
