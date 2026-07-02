using Aegis.Packaging.Core.Constants;
using Aegis.Packaging.Core.Enums;
using Aegis.Packaging.Core.Services;

namespace Aegis.Packaging.Core.Entities;

public sealed class PackageInstance
{
	public required PackageManifest Meta { get; init; }
	public required string RootPath { get; init; }

	public string Id => Meta.Id;
	public Version? ParsedVersion { get; internal set; }
	public int UserOrderIndex { get; set; } = int.MaxValue;

	public bool Enabled { get; set; } = true;
	public PackageState State { get; set; } = PackageState.Discovered;

	public List<PackageDependency> Dependencies { get; internal set; } = [];
	public List<PackageIssue> Issues { get; } = [];

	public bool HasErrors => Issues.Any(issue => issue.Severity == PackageIssueSeverity.Error);

	public void AddIssue(PackageIssueSeverity severity, string code, string message)
	{
		Issues.Add(new PackageIssue(severity, code, message));
	}

	public void ClearIssues()
	{
		Issues.Clear();
	}

	public void ParseMeta()
	{
		ParsedVersion = null;
		Dependencies = [];

		if (string.IsNullOrWhiteSpace(Meta.Id))
			AddIssue(PackageIssueSeverity.Error, PackageIssues.IdMissing, PackageIssueStrings.PackageIdIsEmpty());

		if (!string.IsNullOrWhiteSpace(Meta.Version))
		{
			if (PackageDependencyParser.TryParseVersion(Meta.Version, out var v))
				ParsedVersion = v;
			else
				AddIssue(PackageIssueSeverity.Warning, PackageIssues.VersionInvalid, PackageIssueStrings.PackageVersionIsInvalid(Meta.Version));
		}
		else
		{
			AddIssue(PackageIssueSeverity.Warning, PackageIssues.VersionMissing, PackageIssueStrings.PackageVersionIsEmpty());
		}

		foreach (var raw in Meta.DependenciesRaw)
		{
			if (!PackageDependencyParser.TryParse(raw, out var dep))
			{
				AddIssue(PackageIssueSeverity.Warning, PackageIssues.DependencyInvalid, PackageIssueStrings.DependencyIsInvalid(raw));
				
				continue;
			}

			Dependencies.Add(dep.Value);
		}
	}
}