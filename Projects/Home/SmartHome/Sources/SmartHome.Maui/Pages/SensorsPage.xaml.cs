using SmartHome.Maui.ViewModels;

namespace SmartHome.Maui.Pages;

public partial class SensorsPage : ContentPage
{
    private readonly SensorsViewModel _vm;

    public SensorsPage(SensorsViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.Sensors.Count == 0 && _vm.BinarySensors.Count == 0)
            _vm.RefreshCommand.Execute(null);
    }
}
