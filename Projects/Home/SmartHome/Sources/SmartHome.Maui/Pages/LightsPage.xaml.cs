using SmartHome.Maui.ViewModels;

namespace SmartHome.Maui.Pages;

public partial class LightsPage : ContentPage
{
    private readonly LightsViewModel _vm;

    public LightsPage(LightsViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.Lights.Count == 0)
            _vm.RefreshCommand.Execute(null);
    }

    private void OnToggleClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SmartHome.Maui.Models.LightState light)
            _vm.ToggleCommand.Execute(light);
    }

    private void OnBrightness25Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetBrightnessCommand.Execute((entityId, (int)Math.Round(0.25 * 255)));
    }

    private void OnBrightness50Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetBrightnessCommand.Execute((entityId, (int)Math.Round(0.50 * 255)));
    }

    private void OnBrightness75Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetBrightnessCommand.Execute((entityId, (int)Math.Round(0.75 * 255)));
    }

    private void OnBrightness100Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetBrightnessCommand.Execute((entityId, 255));
    }
}
