using NovoDwarf.FS.Paths.Interfaces;

namespace NovoDwarf.FS.Paths;

/// <inheritdoc cref="IPathResolver" />
public sealed class DefaultPathResolver : IPathResolver
{
    /// <inheritdoc />
    public string GetFullPath(string path) => Path.GetFullPath(path);

    /// <inheritdoc />
    public string GetFullPath(string path, string basePath) => Path.GetFullPath(path, basePath);

    /// <inheritdoc />
    public string GetRelativePath(string relativeTo, string path) => Path.GetRelativePath(relativeTo, path);

    /// <inheritdoc />
    public string? GetPathRoot(string? path) => Path.GetPathRoot(path);

    /// <inheritdoc />
    public bool IsPathFullyQualified(string path) => Path.IsPathFullyQualified(path);

    /// <inheritdoc />
    public bool IsPathRooted(string? path) => Path.IsPathRooted(path);
}
