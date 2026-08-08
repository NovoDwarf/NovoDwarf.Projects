using Grekov.Core.Enums;

namespace Grekov.Packaging.Entities;

public sealed record PackageDependency(
	string PackageId,
	string? Version = null,
	PackageDependencyOperator Operator = PackageDependencyOperator.Any,
	bool Required = true);
