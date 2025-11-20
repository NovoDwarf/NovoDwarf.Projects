using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace ShaderEditor.Events;

public record WindowResizeEvent(Vector2D<int> Size);

public record WindowCloseEvent;

public record WindowLoadEvent(IWindow Window);

public record WindowRenderEvent(double DeltaTime);

public record WindowUpdateEvent(double DeltaTime);



