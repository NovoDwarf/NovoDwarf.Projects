using System.Numerics;
using ImGuiNET;
using ShaderEditor.UI.Syntax;

namespace ShaderEditor.UI.Panels;

public class EditorPanel
{
	private readonly GLSLAutocompleteProvider _autocompleteProvider = new();

	private readonly GLSLSyntaxHighlighter _highlighter = new();
	private string _autocompletePrefix = string.Empty;
	private List<string> _currentSuggestions = new();
	private int _cursorPosition;
	private char _lastChar = '\0';

	private string _lastShaderCode = string.Empty;
	private int _selectedAutocompleteIndex;

	private string _shaderCode = """

	                             #version 410 core

	                             void main()
	                             {
	                                 // Your shader code here
	                             }

	                             """.Trim();

	private bool _showAutocomplete;

	public void Render()
	{
		var availableSize = ImGui.GetContentRegionAvail();

		// Create two columns: line numbers and editor
		ImGui.Columns(2, "EditorColumns", false);
		ImGui.SetColumnWidth(0, 40.0f);

		// Line numbers column
		//ImGui.BeginChild("LineNumbers", availableSize with { X = 0 }, ImGuiChildFlags.None);
		RenderLineNumbers();
		ImGui.EndChild();

		ImGui.NextColumn();

		// Editor column
		ImGui.BeginChild("ShaderEditor", availableSize with { X = 0 });

		// Track text changes for autocomplete
		var textChanged = _shaderCode != _lastShaderCode;
		if (textChanged)
		{
			_lastShaderCode = _shaderCode;

			// Try to detect cursor position (approximate)
			// ImGui doesn't provide direct cursor position, so we estimate
			var io = ImGui.GetIO();
			if (io.WantTextInput && _shaderCode.Length > 0) _cursorPosition = _shaderCode.Length; // Approximation

			// Check if we should show autocomplete
			if (_shaderCode.Length > 0)
			{
				_lastChar = _shaderCode[^1];
				if (_autocompleteProvider.ShouldShowAutocomplete(_lastChar, _shaderCode, _cursorPosition))
				{
					_autocompletePrefix = _autocompleteProvider.GetContextualPrefix(_shaderCode, _cursorPosition) ??
					                      string.Empty;
					_currentSuggestions =
						_autocompleteProvider.GetSuggestions(_autocompletePrefix, _cursorPosition, _shaderCode);
					_showAutocomplete = _currentSuggestions.Count > 0;
					_selectedAutocompleteIndex = 0;
				}
				else
				{
					_showAutocomplete = false;
				}
			}
		}

		// Render syntax highlighted text
		RenderSyntaxHighlightedText();

		// Render autocomplete popup
		if (_showAutocomplete && _currentSuggestions.Count > 0) RenderAutocompletePopup();

		// Open autocomplete popup if needed
		if (_showAutocomplete && _currentSuggestions.Count > 0 && !ImGui.IsPopupOpen("AutocompletePopup"))
			ImGui.OpenPopup("AutocompletePopup");

		ImGui.EndChild();

		ImGui.Columns(1);
	}

	private void RenderLineNumbers()
	{
		if (string.IsNullOrEmpty(_shaderCode))
		{
			ImGui.Text("1");
			return;
		}

		var lines = _shaderCode.Split('\n');

		ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));

		// Render all line numbers (ImGui will handle clipping)
		for (var i = 0; i < lines.Length; i++)
		{
			ImGui.Text($"{i + 1:D}");
			ImGui.NewLine();
		}

		ImGui.PopStyleColor();
	}

	private void RenderSyntaxHighlightedText()
	{
		// Render syntax-highlighted text manually
		// We'll use InputTextMultiline for input handling, but render highlighted version
		var tokens = _highlighter.Tokenize(_shaderCode);
		var lines = _shaderCode.Split('\n');

		// Store the start position
		var startPos = ImGui.GetCursorScreenPos();
		var tokenIndex = 0;
		var currentPos = 0;

		// Render each line with syntax highlighting
		for (var lineIdx = 0; lineIdx < lines.Length; lineIdx++)
		{
			var line = lines[lineIdx];
			var lineStart = currentPos;
			var lineEnd = currentPos + line.Length;

			// Render tokens that belong to this line
			while (tokenIndex < tokens.Count)
			{
				var token = tokens[tokenIndex];

				if (token.StartIndex >= lineStart && token.StartIndex < lineEnd)
				{
					// Render any text before this token (whitespace, etc.)
					if (token.StartIndex > currentPos)
					{
						var beforeText = _shaderCode.Substring(currentPos, token.StartIndex - currentPos);
						ImGui.Text(beforeText);
						ImGui.SameLine(0, 0);
					}

					// Render the token with color
					var color = _highlighter.GetColorForToken(token.Type);
					ImGui.PushStyleColor(ImGuiCol.Text, color);
					ImGui.Text(token.Text);
					ImGui.PopStyleColor();
					ImGui.SameLine(0, 0);

					currentPos = token.StartIndex + token.Length;
					tokenIndex++;
				}
				else if (token.StartIndex >= lineEnd)
				{
					// Token is on next line
					break;
				}
				else
				{
					tokenIndex++;
				}
			}

			// Render any remaining text on this line (whitespace at end)
			if (currentPos < lineEnd)
			{
				var remainingText = _shaderCode.Substring(currentPos, lineEnd - currentPos);
				ImGui.Text(remainingText);
			}

			currentPos = lineEnd + 1; // +1 for newline character
			ImGui.NewLine();
		}

		// Overlay InputTextMultiline for actual input
		// Position it exactly over the highlighted text
		ImGui.SetCursorScreenPos(startPos);

		// Make input transparent but functional
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 0, 0.01f)); // Almost transparent
		ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 1, 1, 0.01f)); // Almost transparent text

		ImGui.InputTextMultiline(
			"##ShaderEditorInput",
			ref _shaderCode,
			10000,
			ImGui.GetContentRegionAvail(),
			ImGuiInputTextFlags.None);

		ImGui.PopStyleColor(2);
	}

	private void RenderAutocompletePopup()
	{
		if (_currentSuggestions.Count == 0)
			return;

		var cursorScreenPos = ImGui.GetCursorScreenPos();
		var lineHeight = ImGui.GetTextLineHeight();

		// Position popup near cursor
		ImGui.SetNextWindowPos(cursorScreenPos with { Y = cursorScreenPos.Y + lineHeight });
		ImGui.SetNextWindowSize(new Vector2(200, Math.Min(_currentSuggestions.Count * lineHeight + 10, 200)));

		if (ImGui.BeginPopup("AutocompletePopup", ImGuiWindowFlags.NoTitleBar))
		{
			for (var i = 0; i < _currentSuggestions.Count; i++)
			{
				var suggestion = _currentSuggestions[i];
				var isSelected = i == _selectedAutocompleteIndex;

				if (isSelected)
					ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.26f, 0.59f, 0.98f, 0.4f));

				if (ImGui.Selectable(suggestion, isSelected))
				{
					InsertAutocompleteSuggestion(suggestion);
					_showAutocomplete = false;
					ImGui.CloseCurrentPopup();
				}

				if (isSelected)
					ImGui.PopStyleColor();
			}

			// Handle keyboard navigation
			var io = ImGui.GetIO();
			/*if (ImGui.IsKeyPressed(ImGuiKey.UpArrow) && _selectedAutocompleteIndex > 0)
			{
				_selectedAutocompleteIndex--;
			}
			if (ImGui.IsKeyPressed(ImGuiKey.DownArrow) && _selectedAutocompleteIndex < _currentSuggestions.Count - 1)
			{
				_selectedAutocompleteIndex++;
			}
			if (ImGui.IsKeyPressed(ImGuiKey.Enter) || ImGui.IsKeyPressed(ImGuiKey.Tab))
			{
				if (_selectedAutocompleteIndex < _currentSuggestions.Count)
				{
					InsertAutocompleteSuggestion(_currentSuggestions[_selectedAutocompleteIndex]);
					_showAutocomplete = false;
					ImGui.CloseCurrentPopup();
				}
			}
			if (ImGui.IsKeyPressed(ImGuiKey.Escape))
			{
				_showAutocomplete = false;
				ImGui.CloseCurrentPopup();
			}*/

			ImGui.EndPopup();
		}
	}

	private void InsertAutocompleteSuggestion(string suggestion)
	{
		if (string.IsNullOrEmpty(_autocompletePrefix))
		{
			_shaderCode += suggestion;
		}
		else
		{
			// Replace the prefix with the full suggestion
			var lastIndex = _shaderCode.LastIndexOf(_autocompletePrefix, StringComparison.OrdinalIgnoreCase);
			if (lastIndex >= 0)
				_shaderCode = _shaderCode.Remove(lastIndex, _autocompletePrefix.Length)
					.Insert(lastIndex, suggestion);
			else
				_shaderCode += suggestion;
		}
	}

	public string GetShaderCode()
	{
		return _shaderCode;
	}

	public void SetShaderCode(string code)
	{
		_shaderCode = code;
	}
}