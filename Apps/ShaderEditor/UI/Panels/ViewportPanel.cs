using ImGuiNET;
using Silk.NET.Maths;

namespace ShaderEditor.UI.Panels;

public class ViewportPanel
{
	public void Render()
	{
		var availableSize = ImGui.GetContentRegionAvail();
		
		// Отображаем viewport с рамкой
		var drawList = ImGui.GetWindowDrawList();
		var min = ImGui.GetCursorScreenPos();
		var max = new System.Numerics.Vector2(min.X + availableSize.X, min.Y + availableSize.Y);
		
		// Рисуем рамку
		drawList.AddRect(min, max, ImGui.GetColorU32(ImGuiCol.Border));
		
		// Устанавливаем область для рендеринга OpenGL
		ImGui.InvisibleButton("##Viewport", availableSize);
		
		// Здесь будет рендеринг OpenGL содержимого
		// Viewport будет установлен через DX11Service
	}
	
	public Vector2D<int> GetViewportSize()
	{
		var size = ImGui.GetContentRegionAvail();
		return new Vector2D<int>((int)size.X, (int)size.Y);
	}
}