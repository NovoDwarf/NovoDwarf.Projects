using Aegis.Packaging.Core.Entities;

namespace Aegis.Packaging.Core.Constants;

public static class PackageIssueStrings
{
	public static string PackageIdIsEmpty()
	{
		return "Package ID is empty.";
	}

	public static string PackageVersionIsInvalid(string version)
	{
		return $"Package version [{version}] is not a valid version.";
	}

	public static string PackageVersionIsEmpty()
	{
		return "Package version is empty.";
	}

	public static string DependencyIsInvalid(string rawDependency)
	{
		return $"Invalid dependency [{rawDependency}].";
	}

	public static string DependencyIsMissing(string packageId)
	{
		return $"Missing dependency [{packageId}].";
	}

	public static string DependencyIsDisabled(string packageId)
	{
		return $"Dependency [{packageId}] is disabled.";
	}

	public static string DependencyVersionCannotBeValidated(string packageId, string dependency)
	{
		return $"Dependency [{packageId}] has unknown/invalid version; constraint [{dependency}]' can't be validated.";
	}

	public static string DependencyVersionMismatched(string dependency, PackageInstance dependencyPackage)
	{
		return
			$"Dependency constraint [{dependency}] is not satisfied by '{dependencyPackage.Id}' ({dependencyPackage.ParsedVersion}).";
	}

	public static string PackageIsIncompatible(string packageId)
	{
		return $"Package is incompatible with [{packageId}].";
	}

	public static string PackageCompatibilityMismatch(IReadOnlyList<string> compatibilityVersions, string gameVersion)
	{
		return
			$"Package declares compatibility [{string.Join(", ", compatibilityVersions)}], but game is [{gameVersion}].";
	}

	public static string DuplicatePackageId(string packageId, string otherRootPath)
	{
		return $"Duplicate package ID [{packageId}]. Also discovered at [{otherRootPath}].";
	}

	public static string DependencyCycleDetected()
	{
		return "Dependency cycle detected (package is not loadable).";
	}

	public static string LoaderFailed(string loaderName, string reason)
	{
		return $"Loader [{loaderName}] failed: {reason}";
	}

	public static string RollbackFailed(string reason)
	{
		return $"Rollback failed: {reason}";
	}
}
