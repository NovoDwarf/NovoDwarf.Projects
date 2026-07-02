using Aegis.Packaging.Core.Enums;

namespace Aegis.Packaging.Core.Entities;

public readonly record struct PackageDependency(string PackageId, PackageDependencyOperator Operator, Version? Version);