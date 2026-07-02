using CommunityToolkit.Mvvm.ComponentModel;

namespace NovoDwarf.Mathematics.App.Systems.Application.Entities;

public class LocaleItem(string cultureCode, string displayName) : ObservableObject
{
	public string CultureCode { get; init; } = cultureCode;
	public string DisplayName { get; init; } = displayName;

	public override string ToString()
	{
		return DisplayName;
	}
}