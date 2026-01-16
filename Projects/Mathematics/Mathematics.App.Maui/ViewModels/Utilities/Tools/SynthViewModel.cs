using Mathematics.App.Maui.UI.Base;
using Mathematics.App.Maui.UI.Graphics;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

public class SynthViewModel : BaseViewModel
{
	public PianoRollDrawable PianoRollDrawable { get; } = new();
}