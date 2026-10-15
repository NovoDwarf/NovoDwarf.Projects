using NovoDwarf.FS.Paths.Interfaces;

namespace NovoDwarf.FS.Paths;

/// <inheritdoc cref="IPathGenerator" />
public sealed class DefaultPathGenerator : IPathGenerator
{
    /// <inheritdoc />
    public string GetRandomFileName() => Path.GetRandomFileName();

    /// <inheritdoc />
    public string GetTempFileName() => Path.GetTempFileName();

    /// <inheritdoc />
    public string GetTempPath() => Path.GetTempPath();
}
