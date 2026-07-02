using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;
using Aegis.Packaging.Core.Interfaces;
using Aegis.Packaging.Core.Services;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Localizations;

public sealed class LocalizationService : IPackageContentLoader
{
	public const string AuditSkipMetaName = "skip_translation_audit";

	private readonly ILocalizationAudit _audit;
	private readonly LocalizationContent _content;
	private readonly LocalizationServerState _state = new();

	public LocalizationService(
		ILogger<LocalizationService> logger,
		IFileSystemService fileSystem,
		IPackageFingerprintProvider fingerprints,
		ILocalizationRuntime runtime,
		ILocalizationAuditFactory? auditFactory = null)
	{
		_content = new LocalizationContent(logger, fileSystem, fingerprints, runtime, _state);
		_audit = (auditFactory ?? new NullLocalizationAuditFactory()).Create(_state);
	}

	public bool AuditEnabled => _audit.AuditEnabled;
	public PackageContentStage Stage => PackageContentStage.Translations;

	public void Clear()
	{
		_content.Clear();
		_audit.Clear();
	}

	public void LoadPackage(PackageInstance package, PackageConflictRegistry packageConflicts)
	{
		_audit.EnsureTreeHook();
		_state.PackageRoots[package.Id] = package.RootPath.TrimEnd('/');

		var list = _content.GetOrCreateTranslationList(package.Id);
		
		_content.TryLoadXmlTranslations(package, list, packageConflicts);

		_content.RebuildKnownKeys();
		_audit.AuditExistingTree();
	}

	public void UnloadPackage(string packageId)
	{
		if (!_state.LoadedByPackage.TryGetValue(packageId, out var list))
			return;

		_content.RemoveTranslations(list);
		_state.LoadedByPackage.Remove(packageId);
		_state.PackageRoots.Remove(packageId);

		_content.RebuildKnownKeys();
		_audit.AuditExistingTree();
	}

	public void SetAuditEnabled(bool enabled)
	{
		_audit.SetAuditEnabled(enabled);
	}

	public IReadOnlyList<string> GetAuditPackageIds()
	{
		return _audit.GetAuditPackageIds();
	}
}
