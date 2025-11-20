using Messager.NET.Interfaces.Senders;
using Microsoft.Extensions.Logging;
using ShaderEditor.Events;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace ShaderEditor.Services;

public sealed class EditorService
{
	private readonly ISender<WindowCloseEvent> _closeSender;

	private readonly CancellationTokenSource _cts = new();

	private readonly ISender<WindowLoadEvent> _loadSender;
	private readonly ILogger<EditorService> _logger;
	private readonly ISender<WindowRenderEvent> _renderSender;
	private readonly ISender<WindowResizeEvent> _resizeSender;
	private readonly ISender<WindowUpdateEvent> _updateSender;

	private IWindow _window = null!;

	private Task? _windowTask;

	public EditorService(
		ILogger<EditorService> logger,
		ISender<WindowLoadEvent> loadSender,
		ISender<WindowResizeEvent> resizeSender,
		ISender<WindowUpdateEvent> updateSender,
		ISender<WindowRenderEvent> renderSender,
		ISender<WindowCloseEvent> closeSender)
	{
		_logger = logger;

		_loadSender = loadSender;
		_resizeSender = resizeSender;
		_updateSender = updateSender;
		_renderSender = renderSender;
		_closeSender = closeSender;
	}

	public async Task StartAsync(CancellationToken cancellationToken = default)
	{
		var options = WindowOptions.Default with
		{
			Title = "Shader Editor",
			Size = new Vector2D<int>(1280, 800),
			FramesPerSecond = 0,
			UpdatesPerSecond = 0,
			API = GraphicsAPI.None
		};

		_window = Window.Create(options);

		_window.FramebufferResize += OnFramebufferResize;
		_window.Load += OnLoad;
		_window.Update += OnUpdate;
		_window.Render += OnRender;
		_window.Closing += OnClose;

		_windowTask = Task.Run(RunAsync, cancellationToken);

		await _windowTask;
	}

	public async Task StopAsync(CancellationToken cancellationToken = default)
	{
		await _cts.CancelAsync();

		if (_windowTask != null)
			try
			{
				await _windowTask.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);
			}
			catch (TimeoutException ex)
			{
				_logger.LogWarning(ex, "Window task did not complete in time");
			}
	}

	private Task RunAsync()
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

	private void OnFramebufferResize(Vector2D<int> size)
	{
		_resizeSender.Send(new WindowResizeEvent(size));
	}

	private void OnLoad()
	{
		_loadSender.Send(new WindowLoadEvent(_window));
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