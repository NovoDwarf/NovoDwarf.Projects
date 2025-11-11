using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace ShaderEditor.Events;

public record WindowCloseEvent;

public record WindowLoadEvent(IWindow Window, GL GL);

public record WindowRenderEvent(double DeltaTime);

public record WindowUpdateEvent(double DeltaTime);



