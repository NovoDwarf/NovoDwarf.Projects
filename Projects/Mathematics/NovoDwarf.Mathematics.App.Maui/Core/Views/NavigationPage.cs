using NovoDwarf.Mathematics.App.Systems.Application.Services;

namespace NovoDwarf.Mathematics.App.Core.Views;

public abstract class NavigationPage : ContentPage, INavigable
{
	public abstract string Route { get; }
}