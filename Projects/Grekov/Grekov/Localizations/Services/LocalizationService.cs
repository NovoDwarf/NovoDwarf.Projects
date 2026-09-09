using Grekov.Definitions.Interfaces;
using Grekov.Localizations.Entities;
using Grekov.Packaging;
using Grekov.Packaging.Entities;
using Grekov.Packaging.Interfaces;

namespace Grekov.Localizations.Services;

internal sealed class LocalizationService : IPackageContentLoader
{
	private readonly IDefCatalog _defs;
	private readonly LocalizationServerState _state;
	private readonly TranslationRegistry _translations;

	public LocalizationService(
		IDefCatalog defs,
		LocalizationServerState state,
		TranslationRegistry translations)
	{
		_defs = defs;
		_state = state;
		_translations = translations;
	}

	public int Stage => PackageStages.Resources;

	public void Clear()
	{
		_state.LoadedByPackageId.Clear();
	}

	public void LoadPackage(PackageLoadContext context)
	{
		var defs = _defs.All<LocalizedStringDef>()
			.Where(def => string.Equals(def.PackageId, context.Package.Id, StringComparison.OrdinalIgnoreCase))
			.ToArray();

		_state.LoadedByPackageId[context.Package.Id] = [.. defs];
		_translations.Apply(defs);
	}

	public void UnloadPackage(string packageId)
	{
		_state.LoadedByPackageId.Remove(packageId);
		_translations.RemovePackage(packageId);
	}
}
