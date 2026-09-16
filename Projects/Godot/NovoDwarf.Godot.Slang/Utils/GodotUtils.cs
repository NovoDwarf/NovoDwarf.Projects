using System;
using System.IO;
using Godot;

namespace NovoDwarf.Godot.Slang.Utils;

internal static class GodotUtils
{
	public static RDShaderFile GetShader(string shaderResPath)
	{
		return ResourceLoader.Load<RDShaderFile>(shaderResPath)
		       ?? throw new FileNotFoundException($"Slang shader resource was not found: {shaderResPath}", shaderResPath);
	}

	public static RDShaderSpirV GetSpirV(RDShaderFile shaderFile, int kernelIndex)
	{
		_ = kernelIndex;
		return shaderFile.GetSpirV();
	}

	public static T RunOnRenderThread<T>(Func<T> action)
	{
		return action();
	}

	public static void RunOnRenderThread(Action action)
	{
		action();
	}
}
