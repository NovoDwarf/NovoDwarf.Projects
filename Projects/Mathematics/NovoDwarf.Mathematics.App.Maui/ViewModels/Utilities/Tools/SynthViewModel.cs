using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Views.Graphics;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

public class SynthViewModel : BaseViewModel
{
	public PianoRollDrawable PianoRollDrawable { get; } = new();
}