using Aegis.Packaging.Core.Constants;
using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;
using Aegis.Packaging.Core.Interfaces;

namespace Aegis.Packaging.Services;

public sealed class PackageValidator
{
	private readonly IGameVersionProvider _gameVersionProvider;

	public PackageValidator(IGameVersionProvider gameVersionProvider)
	{
		_gameVersionProvider = gameVersionProvider;
	}

	public void Validate(PackageDiscoveryResult discovery)
	{
		var gameVersion = _gameVersionProvider.Version;
		var gameMajorMinor = gameVersion.Count(c => c == '.') >= 2
			? string.Join('.', gameVersion.Split('.', 3).Take(2))
			: gameVersion;

		ResetDynamicIssues(discovery.Packages);

		foreach (var pkg in discovery.Packages.Where(pkg => pkg.Enabled))
		{
			ValidateDependencies(pkg, discovery);
			ValidateIncompatibilities(pkg, discovery);
			ValidateCompatibility(pkg, gameVersion, gameMajorMinor);
		}
	}

	private static void ResetDynamicIssues(IEnumerable<PackageInstance> packages)
	{
		foreach (var package in packages)
		{
			var staticIssues = package.Issues.Where(issue => PackageIssues.IsStaticIssue(issue.Code)).ToList();
			package.ClearIssues();
			package.Issues.AddRange(staticIssues);
		}
	}

	private static void ValidateDependencies(PackageInstance package, PackageDiscoveryResult discovery)
	{
		foreach (var dependency in package.Dependencies)
		{
			if (!discovery.PackagesById.TryGetValue(dependency.PackageId, out var dependencyPackage))
			{
				package.AddIssue(PackageIssueSeverity.Error, PackageIssues.DependencyMissing,
					PackageIssueStrings.DependencyIsMissing(dependency.PackageId));
				continue;
			}

			if (!dependencyPackage.Enabled)
			{
				package.AddIssue(PackageIssueSeverity.Error, PackageIssues.DependencyDisabled,
					PackageIssueStrings.DependencyIsDisabled(dependency.PackageId));
				continue;
			}

			if (dependency.Operator == PackageDependencyOperator.Any)
				continue;

			var formattedDependency = FormatDep(dependency);
			if (dependencyPackage.ParsedVersion == null)
			{
				package.AddIssue(
					PackageIssueSeverity.Warning,
					PackageIssues.DependencyVersionUnknown,
					PackageIssueStrings.DependencyVersionCannotBeValidated(dependency.PackageId, formattedDependency));
				continue;
			}

			if (dependency.Version == null ||
			    IsDependencyConstraintSatisfied(dependency, dependencyPackage.ParsedVersion))
				continue;

			package.AddIssue(
				PackageIssueSeverity.Error,
				PackageIssues.DependencyVersionMismatch,
				PackageIssueStrings.DependencyVersionMismatched(formattedDependency, dependencyPackage));
		}
	}

	private static bool IsDependencyConstraintSatisfied(PackageDependency dependency, Version actualVersion)
	{
		return dependency.Operator switch
		{
			PackageDependencyOperator.Exact => actualVersion.Equals(dependency.Version),
			PackageDependencyOperator.AtLeast => actualVersion >= dependency.Version,
			_ => true
		};
	}

	private static void ValidateIncompatibilities(PackageInstance package, PackageDiscoveryResult discovery)
	{
		foreach (var rawPackageId in package.Meta.IncompatiblePackages)
		{
			var packageId = rawPackageId?.Trim();
			if (string.IsNullOrWhiteSpace(packageId))
				continue;

			if (!discovery.PackagesById.TryGetValue(packageId, out var otherPackage) || !otherPackage.Enabled)
				continue;

			package.AddIssue(PackageIssueSeverity.Error, PackageIssues.Incompatible,
				PackageIssueStrings.PackageIsIncompatible(packageId));
		}
	}

	private static void ValidateCompatibility(PackageInstance package, string gameVersion, string gameMajorMinor)
	{
		if (package.Meta.CompatibilityVersions.Count == 0)
			return;

		var isCompatible =
			package.Meta.CompatibilityVersions.Any(version => MatchesGameVersion(version, gameVersion, gameMajorMinor));
		if (isCompatible)
			return;

		package.AddIssue(PackageIssueSeverity.Warning,
			PackageIssues.CompatibilityMismatch,
			PackageIssueStrings.PackageCompatibilityMismatch(package.Meta.CompatibilityVersions, gameVersion));
	}

	private static bool MatchesGameVersion(string? rawVersion, string gameVersion, string gameMajorMinor)
	{
		var normalizedVersion = rawVersion?.Trim();
		
		if (string.IsNullOrWhiteSpace(normalizedVersion))
			return false;

		return string.Equals(normalizedVersion, gameVersion, StringComparison.OrdinalIgnoreCase) ||
		       string.Equals(normalizedVersion, gameMajorMinor, StringComparison.OrdinalIgnoreCase);
	}

	private static string FormatDep(PackageDependency dep)
	{
		return dep.Operator switch
		{
			PackageDependencyOperator.Any => dep.PackageId,
			PackageDependencyOperator.Exact => $"{dep.PackageId}@{dep.Version}",
			PackageDependencyOperator.AtLeast => $"{dep.PackageId}>={dep.Version}",
			_ => dep.PackageId
		};
	}
}