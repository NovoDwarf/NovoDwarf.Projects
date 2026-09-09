using SmartHome.Maui.ViewModels;

namespace SmartHome.Maui.Pages;

public partial class KettlesPage : ContentPage
{
    private readonly KettlesViewModel _vm;

    public KettlesPage(KettlesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.Kettles.Count == 0)
            _vm.RefreshCommand.Execute(null);
    }

    private void OnToggleClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SmartHome.Maui.Models.KettleState kettle)
            _vm.ToggleCommand.Execute(kettle);
    }

    private void OnTemp60Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetTemperatureCommand.Execute((entityId, 60));
    }

    private void OnTemp70Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetTemperatureCommand.Execute((entityId, 70));
    }

    private void OnTemp80Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetTemperatureCommand.Execute((entityId, 80));
    }

    private void OnTemp90Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetTemperatureCommand.Execute((entityId, 90));
    }

    private void OnTemp100Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetTemperatureCommand.Execute((entityId, 100));
    }
}
