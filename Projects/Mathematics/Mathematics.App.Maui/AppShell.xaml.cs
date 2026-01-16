using Mathematics.App.Maui.Systems.Application.Handlers;

namespace Mathematics.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		RoutingHandler.Register(typeof(AppShell).Assembly);
	}
}