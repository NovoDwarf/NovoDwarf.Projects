using Aegis.Packaging.Core.Enums;

namespace Aegis.Packaging.Core.Entities;

public readonly record struct PackageIssue(PackageIssueSeverity Severity, string Code, string Message);