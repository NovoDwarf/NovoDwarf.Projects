namespace NovoDwarf.FS.Paths.Interfaces;

public interface IPathCombiner
{
	/// <inheritdoc cref="Path.Combine(string, string)"/>
	public string Combine(string path1, string path2);

	/// <inheritdoc cref="Path.Combine(string, string, string)"/>
	public string Combine(string path1, string path2, string path3);

	/// <inheritdoc cref="Path.Combine(string, string, string, string)"/>
	public string Combine(string path1, string path2, string path3, string path4);

	/// <inheritdoc cref="Path.Combine(string[])"/>
	public string Combine(params string[] paths);

	/// <inheritdoc cref="Path.Join(string, string)"/>
	public string Join(string? path1, string? path2);

	/// <inheritdoc cref="Path.Join(string, string, string)"/>
	public string Join(string? path1, string? path2, string? path3);

	/// <inheritdoc cref="Path.Join(string, string, string, string)"/>
	public string Join(string? path1, string? path2, string? path3, string? path4);

	/// <inheritdoc cref="Path.Join(string[])"/>
	public string Join(params string?[] paths);
}