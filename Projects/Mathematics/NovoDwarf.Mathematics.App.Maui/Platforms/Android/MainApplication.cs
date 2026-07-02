using Android.App;
using Android.Runtime;
using Application = Android.App.Application;

namespace NovoDwarf.Mathematics.App;

[Application(Label = "Математический органайзер")]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership) 
	{ }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}