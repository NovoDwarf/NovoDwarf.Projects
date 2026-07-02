using Camera.MAUI;
using NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Tools;

public partial class MirrorPage : ContentPage
{
	public MirrorPage(MirrorViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();

		CameraView.CamerasLoaded += CameraView_CamerasLoaded;
	}

	private void CameraView_CamerasLoaded(object? sender, EventArgs e)
	{
		if (CameraView.NumCamerasDetected <= 0)
			return;

		if (CameraView.NumMicrophonesDetected > 0)
			CameraView.Microphone = CameraView.Microphones.First();

		CameraView.Camera = CameraView.Cameras.Last();

		MainThread.BeginInvokeOnMainThread(async () =>
		{
			if (await CameraView.StartCameraAsync() == CameraResult.Success)
			{ }
		});
	}
}