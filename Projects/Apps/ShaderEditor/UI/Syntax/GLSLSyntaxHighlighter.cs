using System.Numerics;

namespace ShaderEditor.UI.Syntax;

public class GLSLSyntaxHighlighter
{
	private static readonly HashSet<string> Keywords = new()
	{
		"if", "else", "for", "while", "do", "switch", "case", "default",
		"break", "continue", "return", "discard", "struct", "void",
		"uniform", "attribute", "varying", "in", "out", "inout",
		"const", "precision", "highp", "mediump", "lowp",
		"true", "false", "layout", "location", "flat", "smooth", "noperspective"
	};

	private static readonly HashSet<string> Types = new()
	{
		"float", "int", "uint", "bool", "vec2", "vec3", "vec4",
		"ivec2", "ivec3", "ivec4", "uvec2", "uvec3", "uvec4",
		"bvec2", "bvec3", "bvec4", "mat2", "mat3", "mat4",
		"mat2x2", "mat2x3", "mat2x4", "mat3x2", "mat3x3", "mat3x4",
		"mat4x2", "mat4x3", "mat4x4", "sampler2D", "samplerCube",
		"sampler2DShadow", "samplerCubeShadow", "sampler2DArray",
		"sampler2DArrayShadow", "isampler2D", "usampler2D"
	};

	private static readonly HashSet<string> BuiltinFunctions = new()
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
	};

	public List<Token> Tokenize(string code)
	{
		var tokens = new List<Token>();
		if (string.IsNullOrEmpty(code))
			return tokens;

		var i = 0;
		var length = code.Length;

		while (i < length)
		{
			// Skip whitespace
			if (char.IsWhiteSpace(code[i]))
			{
				i++;
				continue;
			}

			// Check for preprocessor directives
			if (code[i] == '#' && (i == 0 || code[i - 1] == '\n'))
			{
				var start = i;
				while (i < length && code[i] != '\n')
					i++;
				tokens.Add(new Token(code.Substring(start, i - start), GLSLTokenType.Preprocessor, start, i - start));
				continue;
			}

			// Check for single-line comments
			if (i < length - 1 && code[i] == '/' && code[i + 1] == '/')
			{
				var start = i;
				while (i < length && code[i] != '\n')
					i++;
				tokens.Add(new Token(code.Substring(start, i - start), GLSLTokenType.Comment, start, i - start));
				continue;
			}

			// Check for multi-line comments
			if (i < length - 1 && code[i] == '/' && code[i + 1] == '*')
			{
				var start = i;
				i += 2;
				while (i < length - 1 && !(code[i] == '*' && code[i + 1] == '/'))
					i++;
				if (i < length - 1)
					i += 2;
				tokens.Add(new Token(code.Substring(start, i - start), GLSLTokenType.Comment, start, i - start));
				continue;
			}

			// Check for strings
			if (code[i] == '"')
			{
				var start = i;
				i++;
				while (i < length && code[i] != '"')
					if (code[i] == '\\' && i + 1 < length)
						i += 2;
					else
						i++;
				if (i < length)
					i++;
				tokens.Add(new Token(code.Substring(start, i - start), GLSLTokenType.String, start, i - start));
				continue;
			}

			// Check for numbers
			if (char.IsDigit(code[i]) || (code[i] == '.' && i + 1 < length && char.IsDigit(code[i + 1])))
			{
				var start = i;
				if (code[i] == '.')
					i++;
				while (i < length && (char.IsDigit(code[i]) || code[i] == '.' || code[i] == 'e' || code[i] == 'E' ||
				                      code[i] == '+' || code[i] == '-'))
					i++;
				tokens.Add(new Token(code.Substring(start, i - start), GLSLTokenType.Number, start, i - start));
				continue;
			}

			// Check for identifiers and keywords
			if (char.IsLetter(code[i]) || code[i] == '_')
			{
				var start = i;
				while (i < length && (char.IsLetterOrDigit(code[i]) || code[i] == '_'))
					i++;

				var text = code.Substring(start, i - start);
				var tokenType = GLSLTokenType.Normal;

				if (Keywords.Contains(text))
					tokenType = GLSLTokenType.Keyword;
				else if (Types.Contains(text))
					tokenType = GLSLTokenType.Type;
				else if (BuiltinFunctions.Contains(text))
					tokenType = GLSLTokenType.Function;

				tokens.Add(new Token(text, tokenType, start, i - start));
				continue;
			}

			// Default: single character
			tokens.Add(new Token(code[i].ToString(), GLSLTokenType.Normal, i, 1));
			i++;
		}

		return tokens;
	}

	public Vector4 GetColorForToken(GLSLTokenType type)
	{
		return type switch
		{
			GLSLTokenType.Keyword => new Vector4(0.26f, 0.59f, 0.98f, 1.0f), // Blue
			GLSLTokenType.Type => new Vector4(0.78f, 0.47f, 0.98f, 1.0f), // Purple
			GLSLTokenType.Function => new Vector4(0.98f, 0.78f, 0.26f, 1.0f), // Yellow
			GLSLTokenType.String => new Vector4(0.47f, 0.78f, 0.47f, 1.0f), // Green
			GLSLTokenType.Comment => new Vector4(0.5f, 0.5f, 0.5f, 1.0f), // Gray
			GLSLTokenType.Preprocessor => new Vector4(0.98f, 0.59f, 0.26f, 1.0f), // Orange
			GLSLTokenType.Number => new Vector4(0.78f, 0.78f, 0.47f, 1.0f), // Light yellow
			_ => new Vector4(1.0f, 1.0f, 1.0f, 1.0f) // White
		};
	}

	public record Token(string Text, GLSLTokenType Type, int StartIndex, int Length);
}