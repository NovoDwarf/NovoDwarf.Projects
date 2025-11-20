// [NOTE]: [11.11.2025: 1:51]
// я не знаю какой из вариантов лучше, поэтому оставлю пока этот рабочий костыль.
// idk which variant is better, so I'll leave this for now.

using Microsoft.Extensions.Hosting;
using ShaderEditor.Services;

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