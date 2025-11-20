using System.Numerics;
using FractalVisualizer.Renderers;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace FractalVisualizer;

internal class Program
{
	private static void Main(string[] args)
	{
		var fract = new FractalRenderer();

		fract.Run();
	}
}

public class FractalRenderer
{
	private readonly ImGuiControls _imGuiControls = new();
	private GL _gl = null!;

	private ImGuiController _imGuiController = null!;
	private IWindow _window = null!;

	public void Run()
	{
		var options = WindowOptions.Default;
		options.Size = new Vector2D<int>(1200, 800);
		options.Title = "Fractal Visualizer";

		_window = Window.Create(options);

		Subscibe();

		_window.Run();
	}

	private void Subscibe()
	{
		_window.Load += OnLoad;
		_window.Render += OnRender;
		_window.Closing += OnClose;
	}

	private void Unsubscribe()
	{
		_window.Load -= OnLoad;
		_window.Render -= OnRender;
		_window.Closing -= OnClose;
	}

	private void OnLoad()
	{
		_gl = GL.GetApi(_window);
		_imGuiController = new ImGuiController(_gl, _window, _window.CreateInput());
	}


	private void OnRender(double deltaTime)
	{
		_gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
		_gl.Clear(ClearBufferMask.ColorBufferBit);

		RenderFractal();
		RenderImGui();
	}

	private void OnClose()
	{
		Unsubscribe();
	}

	private void RenderFractal()
	{
		var mandelbrotRenderer = new MandelbrotRenderer(_gl);

		mandelbrotRenderer.Render(_imGuiControls.FractalControls);
	}

	private void RenderImGui()
	{
		_imGuiController.Update((float)_window.Time);

		_imGuiControls.RenderFractalControls();

		_imGuiController.Render();
	}
}

public class FractalControls
{
	public Vector2 Center = new(-0.5f, 0.0f);

	public Vector3 Color1 = new(0, 30, 40);
	public Vector3 Color2 = new(40, 2, 1);
	public FractalType CurrentFractal = FractalType.Mandelbrot;
	public int MaxIterations = 100;
	public float Scale = 1.0f;
}

public class ImGuiControls
{
	public readonly FractalControls FractalControls = new();

	public void RenderFractalControls()
	{
		ImGui.Begin("Fractal Controls");

		ImGui.SliderFloat2("Center", ref FractalControls.Center, -2.0f, 2.0f);
		ImGui.SliderFloat("Scale", ref FractalControls.Scale, 0.001f, 2.0f);
		ImGui.SliderInt("Max Iterations", ref FractalControls.MaxIterations, 10, 1000);

		if (ImGui.Button("Mandelbrot"))
			FractalControls.CurrentFractal = FractalType.Mandelbrot;

		ImGui.SameLine();

		if (ImGui.Button("Julia"))
			FractalControls.CurrentFractal = FractalType.Julia;

		ImGui.End();
	}
}