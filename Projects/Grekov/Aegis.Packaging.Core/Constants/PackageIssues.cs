namespace Aegis.Packaging.Core.Constants;

public static class PackageIssues
{
	public const string IdMissing = "PKG_ID_MISSING";
	public const string IdDuplicate = "PKG_ID_DUPLICATE";
	public const string VersionInvalid = "PKG_VERSION_INVALID";
	public const string VersionMissing = "PKG_VERSION_MISSING";
	public const string DependencyInvalid = "PKG_DEP_INVALID";
	public const string DependencyMissing = "PKG_DEP_MISSING";
	public const string DependencyDisabled = "PKG_DEP_DISABLED";
	public const string DependencyVersionUnknown = "PKG_DEP_VERSION_UNKNOWN";
	public const string DependencyVersionMismatch = "PKG_DEP_VERSION_MISMATCH";
	public const string DependencyCycle = "PKG_DEP_CYCLE";
	public const string Incompatible = "PKG_INCOMPATIBLE";
	public const string CompatibilityMismatch = "PKG_COMPAT_MISMATCH";
	public const string LoaderFailure = "PKG_LOADER_FAILURE";
	public const string RollbackFailure = "PKG_ROLLBACK_FAILURE";

	public static bool IsStaticIssue(string code) 
		=> code is IdMissing or IdDuplicate or VersionInvalid or VersionMissing or DependencyInvalid;

	public static bool BlocksLoading(string code) 
		=> code is IdMissing or IdDuplicate;
}
