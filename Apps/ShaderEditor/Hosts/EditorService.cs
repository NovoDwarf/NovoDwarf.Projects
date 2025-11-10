using Messager.Interfaces.Senders;
using Microsoft.Extensions.Logging;
using Serilog;
using ShaderEditor.Events;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace ShaderEditor.Hosts;

public sealed class EditorService : IDisposable
{
	private readonly CancellationTokenSource _cts = new();

	private readonly ISender<WindowLoadEvent> _loadSender;
	private readonly ISender<WindowUpdateEvent> _updateSender;
	private readonly ISender<WindowRenderEvent> _renderSender;
	private readonly ISender<WindowCloseEvent> _closeSender;
	
	private readonly ILogger<EditorService> _logger;
	
	public EditorService( 
		ISender<WindowLoadEvent> loadSender, 
		ISender<WindowUpdateEvent> updateSender, 
		ISender<WindowRenderEvent> renderSender, 
		ISender<WindowCloseEvent> closeSender, 
		ILogger<EditorService> logger)
	{
		_loadSender = loadSender;
		_updateSender = updateSender;
		_renderSender = renderSender;
		_closeSender = closeSender;
		
		_logger = logger;
	}
	
	private IWindow _window = null!;
	private GL _gl = null!;

	private Task? _windowTask;
	
	public async Task StartAsync(CancellationToken cancellationToken = default)
	{
		var options = WindowOptions.Default with
		{
			Title = "Shader Editor",
			Size = new Vector2D<int>(1280, 800),
			FramesPerSecond = 0,
			UpdatesPerSecond = 0,
			API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 1)),
		};
            
		_window = Window.Create(options);

		_window.Load += OnLoad;
		_window.Update += OnUpdate;
		_window.Render += OnRender;
		_window.Closing += OnClose;

		_windowTask = Task.Run(Run, cancellationToken);
		
		await _windowTask;
	}

	public async Task StopAsync(CancellationToken cancellationToken = default)
	{
		await _cts.CancelAsync();

		if (_windowTask != null)
		{
			try
			{
				await _windowTask.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);
			}
			catch (TimeoutException ex)
			{
				_logger.LogWarning(ex,  "Window task did not complete in time");
			}
		}
	}
	
	public void Dispose()
	{
		_cts.Dispose();
		_window?.Dispose();
	}

	private Task Run()
	{
		try
		{
			_window.Run();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error during window loop");
		}
		
		return Task.CompletedTask;
	}
	
	private void OnLoad()
	{
		_gl = _window.CreateOpenGL(); 
		
		_loadSender.Send(new WindowLoadEvent(_window, _gl));
	}

	private void OnUpdate(double deltaTime)
	{
		_updateSender.Send(new WindowUpdateEvent(deltaTime));
	}
	
	private void OnRender(double deltaTime)
	{
		_renderSender.Send(new WindowRenderEvent(deltaTime));
	}
	
	private void OnClose()
	{
		_closeSender.Send(new WindowCloseEvent());
		
		_cts.Cancel();
	}
}