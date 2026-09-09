using SmartHome.Maui.ViewModels;

namespace SmartHome.Maui.Pages;

public partial class HumidifiersPage : ContentPage
{
    private readonly HumidifiersViewModel _vm;

    public HumidifiersPage(HumidifiersViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.Humidifiers.Count == 0)
            _vm.RefreshCommand.Execute(null);
    }

    private void OnToggleClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SmartHome.Maui.Models.HumidifierState humidifier)
            _vm.ToggleCommand.Execute(humidifier);
    }

    private void OnHumidity40Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetHumidityCommand.Execute((entityId, 40));
    }

    private void OnHumidity50Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetHumidityCommand.Execute((entityId, 50));
    }

    private void OnHumidity60Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetHumidityCommand.Execute((entityId, 60));
    }

    private void OnHumidity70Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetHumidityCommand.Execute((entityId, 70));
    }
}
