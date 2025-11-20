namespace ShaderEditor.UI.Syntax;

public class GLSLAutocompleteProvider
{
	private readonly List<string> _allSuggestions = new();

	public GLSLAutocompleteProvider()
	{
		InitializeSuggestions();
	}

	private void InitializeSuggestions()
	{
		// Keywords
		_allSuggestions.AddRange(new[]
		{
			"if", "else", "for", "while", "do", "switch", "case", "default",
			"break", "continue", "return", "discard", "struct", "void",
			"uniform", "attribute", "varying", "in", "out", "inout",
			"const", "precision", "highp", "mediump", "lowp",
			"true", "false", "layout", "location", "flat", "smooth", "noperspective"
		});

		// Types
		_allSuggestions.AddRange(new[]
		{
			"float", "int", "uint", "bool", "vec2", "vec3", "vec4",
			"ivec2", "ivec3", "ivec4", "uvec2", "uvec3", "uvec4",
			"bvec2", "bvec3", "bvec4", "mat2", "mat3", "mat4",
			"mat2x2", "mat2x3", "mat2x4", "mat3x2", "mat3x3", "mat3x4",
			"mat4x2", "mat4x3", "mat4x4", "sampler2D", "samplerCube",
			"sampler2DShadow", "samplerCubeShadow", "sampler2DArray",
			"sampler2DArrayShadow", "isampler2D", "usampler2D"
		});

		// Built-in functions
		_allSuggestions.AddRange(new[]
		{
			"abs", "acos", "all", "any", "asin", "atan", "atan2",
			"ceil", "clamp", "cos", "cross", "degrees", "distance",
			"dot", "exp", "exp2", "floor", "fract", "inversesqrt",
			"length", "log", "log2", "max", "min", "mix", "mod",
			"normalize", "pow", "radians", "reflect", "refract",
			"round", "sign", "sin", "smoothstep", "sqrt", "step",
			"tan", "texture", "texture2D", "textureCube", "texture2DProj",
			"textureCubeProj", "textureLod", "texture2DLod", "textureCubeLod",
			"textureProjLod", "textureGrad", "texture2DGrad", "textureCubeGrad",
			"textureProjGrad", "dFdx", "dFdy", "fwidth"
		});

		// Built-in variables
		_allSuggestions.AddRange(new[]
		{
			"gl_Position", "gl_FragColor", "gl_FragCoord", "gl_FragDepth",
			"gl_PointSize", "gl_VertexID", "gl_InstanceID", "gl_FrontFacing",
			"gl_PointCoord", "gl_FragCoord", "gl_DepthRange"
		});

		_allSuggestions.Sort();
	}

	public List<string> GetSuggestions(string prefix, int cursorPosition, string fullText)
	{
		if (string.IsNullOrEmpty(prefix))
			return _allSuggestions.Take(20).ToList();

		var lowerPrefix = prefix.ToLowerInvariant();
		return _allSuggestions
			.Where(s => s.StartsWith(lowerPrefix, StringComparison.OrdinalIgnoreCase))
			.Take(20)
			.ToList();
	}

	public string? GetContextualPrefix(string fullText, int cursorPosition)
	{
		if (cursorPosition <= 0 || cursorPosition > fullText.Length)
			return null;

		var start = cursorPosition - 1;
		
		// Find the start of the current word
		while (start >= 0 && (char.IsLetterOrDigit(fullText[start]) || fullText[start] == '_'))
			start--;

		start++; // Move past the non-word character

		if (start >= cursorPosition)
			return null;

		var length = cursorPosition - start;
		if (length > 0 && length < 100) // Reasonable limit
		{
			return fullText.Substring(start, length);
		}

		return null;
	}

	public bool ShouldShowAutocomplete(char lastChar, string fullText, int cursorPosition)
	{
		// Show autocomplete after typing letters, numbers, or underscore
		if (char.IsLetterOrDigit(lastChar) || lastChar == '_')
			return true;

		// Show autocomplete after certain trigger characters
		return lastChar == '.' || lastChar == '(' || lastChar == ' ';
	}
}


