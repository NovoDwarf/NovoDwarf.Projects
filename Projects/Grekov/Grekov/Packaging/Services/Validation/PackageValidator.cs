using Grekov.Packaging.Entities;

namespace Grekov.Packaging.Services.Validation;

internal sealed class PackageValidator
{
	public void Validate(IReadOnlyList<PackageInstance> packages)
	{
		var groups = packages.GroupBy(static package => package.Id, StringComparer.OrdinalIgnoreCase);
		
		foreach (var group in groups.Where(static group => group.Count() > 1))
			foreach (var package in group)
				package.AddIssue(PackageIssues.DuplicatePackageIssue(package.Id));

		foreach (var package in packages)
			if (string.IsNullOrWhiteSpace(package.Id))
				package.AddIssue(PackageIssues.MissingManifestIdIssue(package.RootPath));
	}
}
