using Messager.Entity.Resources;
using Messager.Interfaces.Receivers;
using ShaderEditor.Events;
using ShaderEditor.Interfaces;
using Silk.NET.OpenGL;

namespace ShaderEditor.Services;

public sealed class GLService : IDisposable
{
	private uint _vbo;
	private uint _vao;

	private GL? _gl;

	private readonly DisposableList _disposables = new();
	public GLService(
		IReceiver<WindowLoadEvent> loadReceiver,
		IReceiver<WindowUpdateEvent> updateReceiver,
		IReceiver<WindowRenderEvent> renderReceiver,
		IReceiver<WindowCloseEvent> closeReceiver)
	{
		var load = loadReceiver.Subscribe(OnLoad);
		var update = updateReceiver.Subscribe(OnUpdate);
		var render = renderReceiver.Subscribe(OnRender);
		var close = closeReceiver.Subscribe(OnClose);
		
		_disposables.Add(load, update, render, close);
	}

	public void Dispose()
	{
		_disposables.Dispose();
	}

	private void OnLoad(WindowLoadEvent evt)
	{
		_gl = evt.GL;
		
		_vbo = _gl.GenBuffer();
		_vao = _gl.GenVertexArray();
	}

	private void OnUpdate(WindowUpdateEvent evt)
	{
		
	}
	
	private void OnRender(WindowRenderEvent evt)
	{
		
	}
	
	private void OnClose(WindowCloseEvent evt)
	{
		_gl?.DeleteBuffer(_vbo);
		_gl?.DeleteVertexArray(_vao);
	}
}