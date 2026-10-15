namespace NovoDwarf.FS.Paths.Interfaces;

/// <summary>Extracts or changes individual path components.</summary>
public interface IPathParser
{
    /// <inheritdoc cref="Path.GetDirectoryName(string)" />
    public string? GetDirectoryName(string? path);

    /// <inheritdoc cref="Path.GetFileName(string)" />
    public string? GetFileName(string? path);

    /// <inheritdoc cref="Path.GetFileNameWithoutExtension(string)" />
    public string? GetFileNameWithoutExtension(string? path);

    /// <inheritdoc cref="Path.GetExtension(string)" />
    public string? GetExtension(string? path);

    /// <inheritdoc cref="Path.ChangeExtension(string, string)" />
    public string? ChangeExtension(string? path, string? extension);

    /// <inheritdoc cref="Path.HasExtension(string)" />
    public bool HasExtension(string? path);
}
