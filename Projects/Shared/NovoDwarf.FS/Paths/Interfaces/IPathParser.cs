namespace NovoDwarf.FS.Paths.Interfaces;

public interface IPathParser
{
	/// <inheritdoc cref="Path.GetDirectoryName(string)"/>
	public string? GetDirectoryName(string? path);

	/// <inheritdoc cref="Path.GetFileName(string)"/>
	public string? GetFileName(string? path);

	/// <inheritdoc cref="Path.GetDirectoryName(string)"/>
	public string? GetFileNameWithoutExtension(string? path);

	/// <inheritdoc cref="Path.GetExtension(string)"/>
	public string? GetExtension(string? path);

	/// <inheritdoc cref="Path.ChangeExtension"/>
	public string? ChangeExtension(string? path, string? extension);

	/// <inheritdoc cref="Path.HasExtension(string)"/>
	public bool HasExtension(string? path);
}