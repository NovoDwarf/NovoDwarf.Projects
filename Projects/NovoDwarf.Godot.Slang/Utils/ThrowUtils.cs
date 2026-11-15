using Godot;
using NovoDwarf.Godot.Slang.Exceptions;

namespace NovoDwarf.Godot.Slang.Utils;

internal static class ThrowUtils
{
	public static void ThrowIfSlangError(RDShaderFile shaderFile)
	{
		if (!string.IsNullOrWhiteSpace(shaderFile.BaseError))
			throw new SlangException(shaderFile.BaseError);
	}
}
