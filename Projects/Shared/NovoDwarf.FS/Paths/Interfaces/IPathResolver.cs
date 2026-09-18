namespace NovoDwarf.FS.Paths.Interfaces;

public interface IPathResolver
{
	/// <inheritdoc cref="Path.GetFullPath(string)"/>
	public string GetFullPath(string path);

	/// <inheritdoc cref="Path.GetFullPath(string, string)"/>
	public string GetFullPath(string path, string basePath);

	/// <inheritdoc cref="Path.GetRelativePath(string, string)"/>
	public string GetRelativePath(string relativeTo, string path);

	/// <inheritdoc cref="Path.GetPathRoot(string)"/>
	public string? GetPathRoot(string? path);

	/// <inheritdoc cref="Path.IsPathFullyQualified(string)"/>
	public bool IsPathFullyQualified(string path);

	/// <inheritdoc cref="Path.IsPathRooted(string)"/>
	public bool IsPathRooted(string? path);
}