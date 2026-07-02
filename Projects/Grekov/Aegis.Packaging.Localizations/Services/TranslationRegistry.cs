namespace Aegis.Packaging.Localizations;

internal sealed class TranslationRegistry
{
	private readonly ILocalizationRuntime _runtime;
	private readonly LocalizationServerState _state;

	public TranslationRegistry(ILocalizationRuntime runtime, LocalizationServerState state)
	{
		_runtime = runtime;
		_state = state;
	}

	public void Clear()
	{
		foreach (var tr in _state.LoadedByPackage.Values.SelectMany(static list => list))
			_runtime.RemoveTranslation(tr);

		_state.LoadedByPackage.Clear();
		_state.PackageRoots.Clear();
		_state.KnownKeys.Clear();
	}

	public List<ILocalizationTranslation> GetOrCreateTranslationList(string packageId)
	{
		if (_state.LoadedByPackage.TryGetValue(packageId, out var existing))
			return existing;

		var created = new List<ILocalizationTranslation>();
		_state.LoadedByPackage[packageId] = created; 
		
		return created;
	}

	public void RemoveTranslations(IEnumerable<ILocalizationTranslation> translations)
	{
		foreach (var translation in translations)
			_runtime.RemoveTranslation(translation);
	}

	public void RebuildKnownKeys()
	{
		_state.KnownKeys.Clear();

		foreach (var translation in _state.LoadedByPackage.Values.SelectMany(static list => list))
		foreach (var key in translation.GetKeys())
		{
			if (!string.IsNullOrWhiteSpace(key))
				_state.KnownKeys.Add(key);
		}
	}
}