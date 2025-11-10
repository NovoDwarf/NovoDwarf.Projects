using Messager.Entity.Resources;
using Messager.Interfaces.Receivers;
using Microsoft.Extensions.Logging;
using Serilog;
using ShaderEditor.Events;
using Silk.NET.OpenGL;

namespace ShaderEditor.Services;

public sealed class ShaderService : IDisposable
{
	private readonly ILogger<ShaderService> _logger;
	private readonly DisposableList _disposables = new();
	
	public ShaderService(ILogger<ShaderService> logger, 
		IReceiver<WindowLoadEvent> loadReceiver,
		IReceiver<WindowUpdateEvent> updateReceiver,
		IReceiver<WindowRenderEvent> renderReceiver,
		IReceiver<WindowCloseEvent> closeReceiver)
	{
		_logger = logger;
		
		var load = loadReceiver.Subscribe(OnLoad);
		var update = updateReceiver.Subscribe(OnUpdate);
		var render = renderReceiver.Subscribe(OnRender);
		var close = closeReceiver.Subscribe(OnClose);
		
		_disposables.Add(load, update, render, close);
	}

	private GL _gl = null!;
	
	public void Dispose()
	{
		_disposables.Dispose();
	}

	private uint Compile(string shader, ShaderType type)
	{
		var shaderId = type switch
		{
			ShaderType.VertexShader => CompileVertex(shader),
			ShaderType.FragmentShader => CompileFragment(shader),
			
			_ => throw new ArgumentException("Invalid shader type")
		};

		var log = _gl.GetShaderInfoLog(shaderId);
		
		if (!string.IsNullOrWhiteSpace(log))
			_logger.LogInformation("Compile shader: {Output}", log);
		
		return shaderId;
	}
	
	private void OnLoad(WindowLoadEvent evt)
	{
		_gl = evt.GL;
		
		_logger.LogInformation("Successfully loaded");
	}
	
	private void OnClose(WindowCloseEvent evt)
	{
		_gl = null!;
	}
	
	private void OnUpdate(WindowUpdateEvent evt)
	{
		
	}
	
	private void OnRender(WindowRenderEvent evt)
	{
		
	}

	private uint CompileVertex(string shader)
	{
		var shaderId = _gl.CreateShader(ShaderType.VertexShader);
		
		return CompileShader(shaderId, shader);
	}
	
	private uint CompileFragment(string shader)
	{
		var shaderId = _gl.CreateShader(ShaderType.FragmentShader);
		
		return CompileShader(shaderId, shader);
	}

	private uint CompileShader(uint id, string shader)
	{
		_gl.ShaderSource(id, shader);
		_gl.CompileShader(id);

		return id;
	}
}