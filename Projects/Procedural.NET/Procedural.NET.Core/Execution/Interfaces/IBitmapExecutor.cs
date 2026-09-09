using NovoDwarf.Primitives.Models;
using ProCed.NET.Core.Execution;

namespace Procedural.NET.Core.Execution.Interfaces;

/// <summary>Single-output bitmap executor.</summary>
public interface IBitmapExecutor
{
	RgbaBitmap EvaluateBitmap(GraphNode model, IGraphExecutionContext graph, int width, int height);
}

/// <summary>Multi-output bitmap executor.</summary>
public interface IMultiBitmapExecutor
{
	RgbaBitmap EvaluateBitmap(GraphNode model, IGraphExecutionContext graph, string outputKey, int width, int height);
}
