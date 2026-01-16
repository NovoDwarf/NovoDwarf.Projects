using Mathematics.App.Maui.UI.Views.Common;
using Mathematics.App.ViewModels.Common;

namespace Mathematics.App.Views.Common;

public partial class PreviewSquareView : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(PreviewTableView), string.Empty,
            propertyChanged: OnPropertiesChanged);

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(nameof(Description), typeof(string), typeof(PreviewTableView), string.Empty,
            propertyChanged: OnPropertiesChanged);

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Description), typeof(string), typeof(PreviewTableView), string.Empty,
            propertyChanged: OnPropertiesChanged);

    public static readonly BindableProperty ViewProperty =
        BindableProperty.Create(nameof(View), typeof(Type), typeof(PreviewTableView),
            propertyChanged: OnPropertiesChanged);

    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(View), typeof(Type), typeof(PreviewTableView),
            propertyChanged: OnPropertiesChanged);

    public static readonly BindableProperty PayloadProperty = BindableProperty.Create(nameof(Payload), typeof(object), typeof(PreviewTableView),
            propertyChanged: OnPropertiesChanged);

    private readonly PreviewViewModel? _viewModel;

    public PreviewSquareView()
    {
        InitializeComponent();

        _viewModel = new PreviewViewModel();

        BindingContext = _viewModel;
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public Type? View
    {
        get => (Type?)GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }

    public Type? ViewModel
    {
        get => (Type?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public object? Payload
    {
        get => GetValue(PayloadProperty);
        set => SetValue(PayloadProperty, value);
    }

    private static void OnPropertiesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PreviewSquareView)bindable;
        view.UpdateViewModel();
    }

    private void UpdateViewModel()
    {
        if (_viewModel == null)
            return;

        _viewModel.Title = Title;
        _viewModel.Description = Description;
        _viewModel.View = View;
        _viewModel.ViewModel = ViewModel;
        _viewModel.Icon = new FileImageSource { File = Icon };

        if (Payload == null)
            return;

        var payloadProperty = _viewModel.GetType().GetProperty("Payload");

        if (payloadProperty != null && payloadProperty.CanWrite)
            payloadProperty.SetValue(_viewModel, Payload);
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        UpdateViewModel();
    }
}