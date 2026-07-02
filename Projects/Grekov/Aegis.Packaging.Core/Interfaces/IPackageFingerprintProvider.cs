using Aegis.Packaging.Core.Entities;

namespace Aegis.Packaging.Core.Interfaces;

public interface IPackageFingerprintProvider
{
	string ComputePackageFingerprint(PackageInstance package);
	string ComputeLocaleFingerprint(PackageInstance package);
}
