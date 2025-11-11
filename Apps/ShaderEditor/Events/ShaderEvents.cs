using Silk.NET.OpenGL;

namespace ShaderEditor.Events;

public record ShaderCompileEvent(ShaderType Type, string ShaderContent);
public record ShaderCompiledEvent(ShaderType Type, uint ShaderId);