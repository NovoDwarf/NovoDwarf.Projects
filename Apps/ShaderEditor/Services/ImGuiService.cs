using ImGuiNET;
using Messager.NET.Entity.Resources;
using Messager.NET.Interfaces.Receivers;
using Microsoft.Extensions.Logging;
using ShaderEditor.Events;
using ShaderEditor.UI.Windows;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace ShaderEditor.Services;

public sealed class ImGuiService
{
	private readonly DisposableList _disposableList = new();
	private readonly ILogger<ImGuiService> _logger;
	
	public ImGuiService(
		IReceiver<WindowLoadEvent> loadReceiver,
		IReceiver<WindowUpdateEvent> updateReceiver,
		IReceiver<WindowRenderEvent> renderReceiver,
		IReceiver<GLDisposingEvent> disposingReceiver,
		ILogger<ImGuiService> logger)
	{
		_logger = logger;
		
	}

	//private ImGuiController? _imGuiController;
	private EditorWindow? _editorWindow;
	
	private IInputContext? _input;
	private IWindow? _window;

	
	private void OnLoad(WindowLoadEvent evt)
	{
		_window = evt.Window;
		_input = evt.Window.CreateInput();
		
		//_imGuiController = new ImGuiController(_gl, _window, _input);
		_editorWindow = new EditorWindow();
	}
	
	private void OnUpdate(WindowUpdateEvent evt)
	{
		//_imGuiController?.Update((float)evt.DeltaTime);
	}
	
	private void OnRender(WindowRenderEvent evt)
	{
		_editorWindow?.Render();
		
		ImGui.Render();
		
		//_imGuiController?.Render();
	}
	
	private void OnClose(GLDisposingEvent evt)
	{
		//_imGuiController?.Dispose();
		_input?.Dispose();
		_disposableList.Dispose();
	}
}