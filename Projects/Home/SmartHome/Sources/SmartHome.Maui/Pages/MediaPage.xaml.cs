using SmartHome.Maui.ViewModels;

namespace SmartHome.Maui.Pages;

public partial class MediaPage : ContentPage
{
    private readonly MediaViewModel _vm;

    public MediaPage(MediaViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.Players.Count == 0)
            _vm.RefreshCommand.Execute(null);
    }

    private void OnToggleClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SmartHome.Maui.Models.MediaPlayerState player)
            _vm.ToggleCommand.Execute(player);
    }

    private void OnPlayClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.PlayCommand.Execute(entityId);
    }

    private void OnPauseClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.PauseCommand.Execute(entityId);
    }

    private void OnMuteClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SmartHome.Maui.Models.MediaPlayerState player)
            _vm.ToggleMuteCommand.Execute(player);
    }

    private void OnVolume10Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetVolumeCommand.Execute((entityId, 10));
    }

    private void OnVolume30Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetVolumeCommand.Execute((entityId, 30));
    }

    private void OnVolume60Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetVolumeCommand.Execute((entityId, 60));
    }

    private void OnVolume90Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string entityId)
            _vm.SetVolumeCommand.Execute((entityId, 90));
    }
}
