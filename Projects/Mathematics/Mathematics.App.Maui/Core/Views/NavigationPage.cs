using Mathematics.App.Maui.Systems.Application.Services;

namespace Mathematics.App.Core.Views;

public abstract class NavigationPage : ContentPage, INavigable
{
	public abstract string Route { get; }
}