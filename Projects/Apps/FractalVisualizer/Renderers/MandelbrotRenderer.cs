using System.Numerics;
using FractalVisualizer.Utilities;
using Silk.NET.OpenGL;

namespace FractalVisualizer.Renderers;

public class MandelbrotRenderer : IDisposable
{
	private readonly GL _gl;

	private GlUtils _glUtils = null!;

	private uint _shaderProgram;
	private uint _vertexArray;
	private uint _vertexBuffer;

	public MandelbrotRenderer(GL gl)
	{
		_gl = gl;

		Initialize();
	}

	public void Dispose()
	{
		_gl.DeleteProgram(_shaderProgram);
		_gl.DeleteVertexArray(_vertexArray);
		_gl.DeleteBuffer(_vertexBuffer);
	}

	private void Initialize()
	{
		var vertexShader = CreateShader(ShaderType.VertexShader, GetVertexShaderSource());
		var fragmentShader = CreateShader(ShaderType.FragmentShader, GetFragmentShaderSource());

		_shaderProgram = _gl.CreateProgram();

		_gl.AttachShader(_shaderProgram, vertexShader);
		_gl.AttachShader(_shaderProgram, fragmentShader);
		_gl.LinkProgram(_shaderProgram);

		_gl.DeleteShader(vertexShader);
		_gl.DeleteShader(fragmentShader);

		_gl.GetProgram(_shaderProgram, GLEnum.LinkStatus, out var status);

		if (status == 0)
			throw new Exception($"Program linking failed: {_gl.GetProgramInfoLog(_shaderProgram)}");

		float[] vertices =
		[
			-1.0f, -1.0f,
			1.0f, -1.0f,
			1.0f, 1.0f,
			-1.0f, 1.0f
		];

		_vertexArray = _gl.GenVertexArray();
		_vertexBuffer = _gl.GenBuffer();

		_gl.BindVertexArray(_vertexArray);
		_gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);
		_gl.BufferData(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

		_gl.EnableVertexAttribArray(0);
		_gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

		_gl.BindVertexArray(0);

		_glUtils = new GlUtils(_gl, _shaderProgram);
	}

	private uint CreateShader(ShaderType type, string source)
	{
		var shader = _gl.CreateShader(type);
		_gl.ShaderSource(shader, source);
		_gl.CompileShader(shader);

		_gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);

		if (status == 0)
			throw new Exception($"Error compiling {type}: {_gl.GetShaderInfoLog(shader)}");

		return shader;
	}

	public void Render(FractalControls controls)
	{
		_gl.UseProgram(_shaderProgram);

		_glUtils.SetUniform("resolution",
			new Vector2(_gl.GetInteger(GetPName.MaxFramebufferWidth), _gl.GetInteger(GetPName.MaxFramebufferHeight)));
		_glUtils.SetUniform("center", controls.Center);
		_glUtils.SetUniform("scale", controls.Scale);
		_glUtils.SetUniform("maxIterations", controls.MaxIterations);
		_glUtils.SetUniform("color1", controls.Color1);
		_glUtils.SetUniform("color2", controls.Color2);

		_gl.BindVertexArray(_vertexArray);
		_gl.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
		_gl.BindVertexArray(0);
	}

	private string GetVertexShaderSource()
	{
		return File.ReadAllText("Shaders/Mandelbrot.vert");
	}

	private string GetFragmentShaderSource()
	{
		return File.ReadAllText("Shaders/Mandelbrot.frag");
	}
}