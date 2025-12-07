using CommunityToolkit.Mvvm.Input;
using Mathematics.App.Maui.Factories;
using Mathematics.App.Maui.Services;
using Mathematics.App.Maui.UI.ViewModels.Common;

namespace Mathematics.App.Maui.UI.Views.Common;

public partial class PreviewView : ContentView
{
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(PreviewView), string.Empty);
    
    public static readonly BindableProperty DescriptionProperty = 
        BindableProperty.Create(nameof(Description), typeof(string), typeof(PreviewView), string.Empty);
    
    public static readonly BindableProperty PageProperty =
        BindableProperty.Create(nameof(Page), typeof(Type), typeof(PreviewView), null);
    
    private readonly IPageFactory _factory;
    private readonly NavigationService _navigationService;
    
    public PreviewView()
    {
        InitializeComponent();
        
        _factory = InjectionService.Resolve<IPageFactory>();
        _navigationService = InjectionService.Resolve<NavigationService>();
    }
    
    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public Type? Page
    {
        get => (Type?)GetValue(PageProperty);
        set => SetValue(PageProperty, value);
    }
    
    [RelayCommand]
    public async Task Open()
    {
        if (Page == null)
            return;

        var page = _factory.Create(Page);
        
        if (page != null)
        {
            await _navigationService.PushAsync(page);
        }
    }
}