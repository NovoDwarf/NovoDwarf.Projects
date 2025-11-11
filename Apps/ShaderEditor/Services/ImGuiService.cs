using Messager.Entity.Resources;
using Messager.Interfaces.Receivers;
using Microsoft.Extensions.Logging;
using ShaderEditor.Events;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace ShaderEditor.Services;

public sealed class ImGuiService : IDisposable
{
	private readonly DisposableList _disposableList = new();
	private readonly ILogger<ImGuiService> _logger;
	
	public ImGuiService(
		IReceiver<WindowLoadEvent> loadReceiver,
		IReceiver<WindowUpdateEvent> updateReceiver,
		IReceiver<WindowRenderEvent> renderReceiver,
		IReceiver<WindowCloseEvent> closeReceiver, 
		ILogger<ImGuiService> logger)
	{
		_logger = logger;
		var load = loadReceiver.Subscribe(OnLoad);
		var update = updateReceiver.Subscribe(OnUpdate);
		var render = renderReceiver.Subscribe(OnRender);
		var close = closeReceiver.Subscribe(OnClose);
		
		_disposableList.Add(load, update, render, close);
	}

	private GL? _gl;
	private ImGuiController? _imGuiController;
	
	private IInputContext? _input;
	private IWindow? _window;
	
	public void Dispose()
	{
		_imGuiController?.Dispose();
		_disposableList.Dispose();
		
		_gl = null;
		_imGuiController = null;
		_input = null;
		_window = null;
	}

	private void OnLoad(WindowLoadEvent evt)
	{
		_gl = evt.GL;
		_window = evt.Window;
		_input = evt.Window.CreateInput();
		
		_imGuiController = new ImGuiController(_gl, _window, _input);

		foreach (var keyboard in _input.Keyboards)
		{
			keyboard.KeyDown += OnKeyDown;
		}
	}
	
	private void OnClose(WindowCloseEvent evt)
	{
		
	}
	
	private void OnUpdate(WindowUpdateEvent evt)
	{
		_imGuiController?.Update((float)evt.DeltaTime);
	}
	
	private void OnRender(WindowRenderEvent evt)
	{
		_imGuiController?.Render();
	}

	private void OnKeyDown(IKeyboard keyboard, Key key, int arg3)
	{
		if (_window == null)
			return;
		
		if (key == Key.Escape)
			_window.Close();
	}
}