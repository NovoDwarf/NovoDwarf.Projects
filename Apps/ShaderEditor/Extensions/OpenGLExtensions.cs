using Silk.NET.OpenGL;

namespace ShaderEditor.Extensions;

public static class OpenGLExtensions
{
	extension (GL gl) 
	{
		public void NullifyBuffer(ref uint buffer)
		{
			gl?.DeleteBuffer(buffer);
		
			buffer = 0;
		}

		public void NullifyVertexArray(ref uint vertexArray)
		{
			gl?.DeleteVertexArray(vertexArray);

			vertexArray = 0;
		}
	}
}