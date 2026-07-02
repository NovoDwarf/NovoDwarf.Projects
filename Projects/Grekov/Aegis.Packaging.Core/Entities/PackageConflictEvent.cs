namespace Aegis.Packaging.Core.Entities;

public readonly record struct PackageConflictEvent(string Key, string PackageId, string PreviousPackageId);