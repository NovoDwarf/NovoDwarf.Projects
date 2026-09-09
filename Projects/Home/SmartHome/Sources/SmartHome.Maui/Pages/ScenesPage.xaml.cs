using SmartHome.Maui.ViewModels;

namespace SmartHome.Maui.Pages;

public partial class ScenesPage : ContentPage
{
    private readonly ScenesViewModel _vm;

    public ScenesPage(ScenesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.Automations.Count == 0 && _vm.Scripts.Count == 0)
            _vm.RefreshCommand.Execute(null);
    }

    private void OnTriggerClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SmartHome.Maui.Models.AutomationState automation)
            _vm.TriggerCommand.Execute(automation);
    }

    private void OnToggleAutomationClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SmartHome.Maui.Models.AutomationState automation)
            _vm.ToggleAutomationCommand.Execute(automation);
    }

    private void OnRunScriptClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SmartHome.Maui.Models.ScriptState script)
            _vm.RunScriptCommand.Execute(script);
    }
}
