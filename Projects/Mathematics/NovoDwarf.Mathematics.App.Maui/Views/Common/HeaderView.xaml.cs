namespace NovoDwarf.Mathematics.App.Views.Common;

public partial class HeaderView : ContentView
{
	public HeaderView()
	{
		InitializeComponent();
	}

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public double FontSize
	{
		get => (double)GetValue(FontSizeProperty);
		set => SetValue(FontSizeProperty, value);
	}

	public Color LineColor
	{
		get => (Color)GetValue(LineColorProperty);
		set => SetValue(LineColorProperty, value);
	}

	public double LineThickness
	{
		get => (double)GetValue(LineThicknessProperty);
		set => SetValue(LineThicknessProperty, value);
	}

	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(HeaderView), defaultValue: string.Empty);

	public static readonly BindableProperty FontSizeProperty =
		BindableProperty.Create(nameof(FontSize), typeof(double), typeof(HeaderView), defaultValue: 16d);

	public static readonly BindableProperty LineColorProperty =
		BindableProperty.Create(nameof(LineColor), typeof(Color), typeof(HeaderView), defaultValue: Colors.LightGray);

	public static readonly BindableProperty LineThicknessProperty =
		BindableProperty.Create(nameof(LineThickness), typeof(double), typeof(HeaderView), defaultValue: 1d);

}