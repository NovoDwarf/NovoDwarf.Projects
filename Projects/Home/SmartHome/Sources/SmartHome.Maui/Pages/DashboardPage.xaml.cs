using SmartHome.Maui.ViewModels;

namespace SmartHome.Maui.Pages;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DashboardViewModel { HasData: false } vm)
            vm.RefreshCommand.Execute(null);
    }
}
