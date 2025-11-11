using System.Drawing;
using Messager.Entity.Resources;
using Messager.Interfaces.Receivers;
using Microsoft.Extensions.Logging;
using ShaderEditor.Events;
using ShaderEditor.Interfaces;
using Silk.NET.OpenGL;

namespace ShaderEditor.Services;

public sealed class GLService : IDisposable
{
	private readonly ILogger<GLService> _logger;
	private readonly DisposableList _disposables = new();
	
	public GLService(
		ILogger<GLService> logger,
		IReceiver<WindowLoadEvent> loadReceiver,
		IReceiver<WindowUpdateEvent> updateReceiver,
		IReceiver<WindowRenderEvent> renderReceiver,
		IReceiver<WindowCloseEvent> closeReceiver,
		IReceiver<ShaderCompiledEvent> compiledReceiver)
	{
		_logger = logger;
		
		var load = loadReceiver.Subscribe(OnLoad);
		var update = updateReceiver.Subscribe(OnUpdate);
		var render = renderReceiver.Subscribe(OnRender);
		var close = closeReceiver.Subscribe(OnClose);
		var compiled = compiledReceiver.Subscribe(OnCompiled);
		
		_disposables.Add(load, update, render, close, compiled);
	}
	
	private uint _vbo;
	private uint _vao;
	private uint _ebo;
	
	private uint _program;

	private GL? _gl;

	public void Dispose()
	{
		_disposables.Dispose();
	}

	private void OnCompiled(ShaderCompiledEvent evt)
	{
		if (_program == 0)
		{
			if (_gl != null)
			{
				_program = _gl.CreateProgram();
			}
		}

		_gl?.AttachShader(_program, evt.ShaderId);
	}
	
	private unsafe void OnLoad(WindowLoadEvent evt)
	{
		_gl = evt.GL;
		
		_vbo = _gl.GenBuffer();
		_vao = _gl.GenVertexArray();
		_ebo = _gl.GenBuffer();
		
		float[] vertices =
		[
			1f,  1f, 0.0f,
			1f, -1f, 0.0f,
			-1f, -1f, 0.0f,
			-1f,  1f, 0.0f
		];

		fixed (float* buf = vertices)
			_gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
		
		_gl.BindVertexArray(_vao);
		_gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
		_gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
		
		_gl.ClearColor(Color.CornflowerBlue);
	}

	private void OnUpdate(WindowUpdateEvent evt)
	{
		
	}
	
	private void OnRender(WindowRenderEvent evt)
	{
		_gl?.Clear(ClearBufferMask.ColorBufferBit);
	}
	
	private void OnClose(WindowCloseEvent evt)
	{
		_gl?.DeleteBuffer(_vbo);
		_gl?.DeleteVertexArray(_vao);
	}
}