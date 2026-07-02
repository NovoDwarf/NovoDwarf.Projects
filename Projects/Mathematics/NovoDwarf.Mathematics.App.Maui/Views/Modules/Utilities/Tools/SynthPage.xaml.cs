using NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;
using NovoDwarf.Mathematics.App.Views.Graphics;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Tools;

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