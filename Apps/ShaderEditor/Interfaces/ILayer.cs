using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace ShaderEditor.Interfaces;

public interface ILayer
{
	public void OnLoad(IWindow window, GL gl);
}