using Mathematics.App.Maui.UI.Graphics;
using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class SynthPage : ContentPage
{
	private PianoRollDrawable Drawable =>
		(PianoRollDrawable)BindingContext!.GetType()
			.GetProperty(nameof(PianoRollDrawable))!
			.GetValue(BindingContext)!;

	public SynthPage(SynthViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}

	private void OnStartInteraction(object sender, TouchEventArgs e)
	{
		Drawable.OnPointerDown(e.Touches[0].X, e.Touches[0].Y);
		((GraphicsView)sender).Invalidate();
	}

	private void OnDragInteraction(object sender, TouchEventArgs e)
	{
		Drawable.OnPointerMove(e.Touches[0].X, e.Touches[0].Y);
		((GraphicsView)sender).Invalidate();
	}

	private void OnEndInteraction(object sender, TouchEventArgs e)
	{
		Drawable.OnPointerUp();
		((GraphicsView)sender).Invalidate();
	}
}