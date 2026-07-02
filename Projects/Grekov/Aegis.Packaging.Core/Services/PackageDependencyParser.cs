using System.Diagnostics.CodeAnalysis;
using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;

namespace Aegis.Packaging.Core.Services;

public static class PackageDependencyParser
{
	private static string[] Delimiters = [">=", "@", "=", "<", ">"];
	
	public static bool TryParse(string raw, [NotNullWhen(true)] out PackageDependency? dependency)
	{
		dependency = null;

		var packageId = TryGetPackageId(raw);
		if (string.IsNullOrWhiteSpace(packageId))
			return false;

		raw = raw.Trim();

		if (raw.Contains(">=", StringComparison.Ordinal))
		{
			var parts = raw.Split(">=", 2, StringSplitOptions.TrimEntries);
			if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]))
				return false;

			if (!TryParseVersion(parts[1], out var version))
				return false;

			dependency = new PackageDependency(parts[0], PackageDependencyOperator.AtLeast, version);
			return true;
		}

		if (raw.Contains('@', StringComparison.Ordinal))
		{
			var parts = raw.Split('@', 2, StringSplitOptions.TrimEntries);
			if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]))
				return false;

			if (!TryParseVersion(parts[1], out var version))
				return false;

			dependency = new PackageDependency(parts[0], PackageDependencyOperator.Exact, version);
			return true;
		}

		dependency = new PackageDependency(raw, PackageDependencyOperator.Any, null);
		return true;
	}

	public static string? TryGetPackageId(string? raw)
	{
		if (string.IsNullOrWhiteSpace(raw))
			return null;

		raw = raw.Trim();
		
		
		foreach (var delimiter in Delimiters)
		{
			var index = raw.IndexOf(delimiter, StringComparison.Ordinal);
			if (index <= 0)
				continue;

			var packageId = raw[..index].Trim();
			return string.IsNullOrWhiteSpace(packageId) ? null : packageId;
		}

		return raw;
	}

	internal static bool TryParseVersion(string raw, [NotNullWhen(true)] out Version? version)
	{
		version = null;

		if (string.IsNullOrWhiteSpace(raw))
			return false;

		if (!Version.TryParse(raw.Trim(), out var parsed))
			return false;

		version = parsed;
		return true;
	}
}
