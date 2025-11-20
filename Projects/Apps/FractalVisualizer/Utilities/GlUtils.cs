using System.Numerics;
using Silk.NET.OpenGL;

namespace FractalVisualizer.Utilities;

public class GlUtils
{
	private readonly GL _gl;
	private readonly uint _shaderProgram;

	public GlUtils(GL gl, uint shaderProgram)
	{
		_gl = gl;
		_shaderProgram = shaderProgram;
	}

	public void SetUniform(string name, Vector2 value)
	{
		var location = _gl.GetUniformLocation(_shaderProgram, name);
		_gl.Uniform2(location, value.X, value.Y);
	}

	public void SetUniform(string name, Vector3 value)
	{
		var location = _gl.GetUniformLocation(_shaderProgram, name);
		_gl.Uniform3(location, value.X, value.Y, value.Z);
	}

	public void SetUniform(string name, float value)
	{
		var location = _gl.GetUniformLocation(_shaderProgram, name);
		_gl.Uniform1(location, value);
	}

	public void SetUniform(string name, int value)
	{
		var location = _gl.GetUniformLocation(_shaderProgram, name);
		_gl.Uniform1(location, value);
	}
}