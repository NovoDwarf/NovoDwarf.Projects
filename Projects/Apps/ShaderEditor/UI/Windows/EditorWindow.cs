using System.Numerics;
using ImGuiNET;
using ShaderEditor.UI.Panels;

namespace ShaderEditor.UI.Windows;

public class EditorWindow
{
	private readonly EditorPanel _editorPanel = new();
	private readonly ViewportPanel _viewportPanel = new();

	public void Render()
	{
		//var viewport = ImGui.GetMainViewport();

		//ImGui.SetNextWindowPos(viewport.Pos);
		//	ImGui.SetNextWindowSize(viewport.Size);
		//ImGui.SetNextWindowViewport(viewport.ID);

		var windowFlags = ImGuiWindowFlags.MenuBar
		                  // | ImGuiWindowFlags.NoDocking 
		                  | ImGuiWindowFlags.NoTitleBar
		                  | ImGuiWindowFlags.NoCollapse
		                  | ImGuiWindowFlags.NoResize
		                  | ImGuiWindowFlags.NoMove
		                  | ImGuiWindowFlags.NoBringToFrontOnFocus
		                  | ImGuiWindowFlags.NoNavFocus;

		ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
		ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));

		ImGui.Begin("MainWindow", windowFlags);
		ImGui.PopStyleVar(3);

		// Main Menu Bar
		if (ImGui.BeginMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				if (ImGui.MenuItem("New"))
				{
					// TODO: Создать новый шейдер
				}

				if (ImGui.MenuItem("Open"))
				{
					// TODO: Открыть шейдер
				}

				if (ImGui.MenuItem("Save"))
				{
					// TODO: Сохранить шейдер
				}

				ImGui.Separator();
				if (ImGui.MenuItem("Exit"))
				{
					// TODO: Выход из приложения
				}

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Edit"))
			{
				if (ImGui.MenuItem("Compile Shader"))
				{
					// TODO: Компиляция шейдера
				}

				ImGui.EndMenu();
			}

			ImGui.EndMenuBar();
		}

		// Разделяем окно на две секции
		var availableSize = ImGui.GetContentRegionAvail();

		// Левая секция - редактор шейдера (50% ширины)
		//ImGui.BeginChild("ShaderEditorSection", availableSize with { X = availableSize.X * 0.5f }, 
		//ImGuiChildFlags.Border);
		ImGui.Text("Shader Editor");
		ImGui.Separator();
		_editorPanel.Render();
		ImGui.EndChild();

		ImGui.SameLine();

		// Правая секция - viewport (50% ширины)
		//	ImGui.BeginChild("ViewportSection", availableSize with { X = 0 }, 
		//	ImGuiChildFlags.Border);
		ImGui.Text("Viewport");
		ImGui.Separator();
		_viewportPanel.Render();
		ImGui.EndChild();

		ImGui.End();
	}
}