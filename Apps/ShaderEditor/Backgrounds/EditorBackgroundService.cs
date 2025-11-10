using Microsoft.Extensions.Hosting;
using ShaderEditor.Hosts;

namespace ShaderEditor.Backgrounds;

public class EditorBackgroundService(EditorService editor, IHostApplicationLifetime hostLifetime) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken cts)
	{
		await editor.StartAsync(cts);
		await editor.StopAsync(cts);
		
		hostLifetime.StopApplication();
	}
}