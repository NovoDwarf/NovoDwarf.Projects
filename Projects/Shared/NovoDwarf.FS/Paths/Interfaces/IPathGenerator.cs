namespace NovoDwarf.FS.Paths.Interfaces;

public interface IPathGenerator 
{
	/// <inheritdoc cref="Path.GetRandomFileName"/>
	public string GetRandomFileName();

	/// <inheritdoc cref="Path.GetTempFileName"/>

	public string GetTempFileName();

	/// <inheritdoc cref="Path.GetTempPath"/>
	public string GetTempPath();
}