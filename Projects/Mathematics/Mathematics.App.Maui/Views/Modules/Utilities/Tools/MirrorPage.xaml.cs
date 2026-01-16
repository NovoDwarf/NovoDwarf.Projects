using Camera.MAUI;
using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class MirrorPage : ContentPage
{
	public MirrorPage(MirrorViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();

		cameraView.CamerasLoaded += CameraView_CamerasLoaded;
	}

	private void CameraView_CamerasLoaded(object? sender, EventArgs e)
	{
		if (cameraView.NumCamerasDetected <= 0)
			return;

		if (cameraView.NumMicrophonesDetected > 0)
			cameraView.Microphone = cameraView.Microphones.First();

		cameraView.Camera = cameraView.Cameras.Last();

		MainThread.BeginInvokeOnMainThread(async () =>
		{
			if (await cameraView.StartCameraAsync() == CameraResult.Success)
			{ }
		});
	}
}