using NovoDwarf.Mathematics.App.Systems.Application.Handlers;

namespace NovoDwarf.Mathematics.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		RoutingHandler.Register(typeof(AppShell).Assembly);
	}
}