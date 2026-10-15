using NovoDwarf.FS.Paths.Interfaces;

namespace NovoDwarf.FS.Paths;

/// <inheritdoc cref="IPathParser" />
public sealed class DefaultPathParser : IPathParser
{
    /// <inheritdoc />
    public string? GetDirectoryName(string? path) => Path.GetDirectoryName(path);

    /// <inheritdoc />
    public string? GetFileName(string? path) => Path.GetFileName(path);

    /// <inheritdoc />
    public string? GetFileNameWithoutExtension(string? path) => Path.GetFileNameWithoutExtension(path);

    /// <inheritdoc />
    public string? GetExtension(string? path) => Path.GetExtension(path);

    /// <inheritdoc />
    public string? ChangeExtension(string? path, string? extension) => Path.ChangeExtension(path, extension);

    /// <inheritdoc />
    public bool HasExtension(string? path) => Path.HasExtension(path);
}
