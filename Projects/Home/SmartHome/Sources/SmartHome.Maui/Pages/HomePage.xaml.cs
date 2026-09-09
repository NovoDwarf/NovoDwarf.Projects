using SmartHome.Maui.ViewModels;

namespace SmartHome.Maui.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _vm;

    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.RefreshCommand.Execute(null);
    }

    private void OnLightsClicked(object sender, EventArgs e)
        => Shell.Current.GoToAsync("//lights");

    private void OnHumidifiersClicked(object sender, EventArgs e)
        => Shell.Current.GoToAsync("//humidifiers");

    private void OnMediaClicked(object sender, EventArgs e)
        => Shell.Current.GoToAsync("//media");

    private void OnKettlesClicked(object sender, EventArgs e)
        => Shell.Current.GoToAsync("//kettles");

    private void OnScenesClicked(object sender, EventArgs e)
        => Shell.Current.GoToAsync("//scenes");

    private void OnHealthClicked(object sender, EventArgs e)
        => Shell.Current.GoToAsync("//health");

    private void OnSettingsClicked(object sender, EventArgs e)
        => Shell.Current.GoToAsync("//settings");
}
