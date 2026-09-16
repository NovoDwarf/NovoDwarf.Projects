using System;

namespace NovoDwarf.Godot.Slang.Exceptions;

public sealed class SlangException : Exception
{
	public SlangException(string message)
		: base(message)
	{
	}

	public SlangException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
