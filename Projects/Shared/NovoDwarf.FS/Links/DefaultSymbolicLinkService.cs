namespace NovoDwarf.FS.Links;

/// <inheritdoc cref="ISymbolicLinkService" />
public sealed class DefaultSymbolicLinkService : ISymbolicLinkService
{
    /// <inheritdoc />
    public IFileSystemEntry CreateFileLink(string path, string pathToTarget)
    {
        return new LocalFileSystemEntry(File.CreateSymbolicLink(path, pathToTarget));
    }

    /// <inheritdoc />
    public IFileSystemEntry CreateDirectoryLink(string path, string pathToTarget)
    {
        return new LocalFileSystemEntry(Directory.CreateSymbolicLink(path, pathToTarget));
    }

    /// <inheritdoc />
    public IFileSystemEntry? ResolveFileLink(string linkPath, bool returnFinalTarget)
    {
        FileSystemInfo? target = File.ResolveLinkTarget(linkPath, returnFinalTarget);

        return target is null ? null : new LocalFileSystemEntry(target);
    }

    /// <inheritdoc />
    public IFileSystemEntry? ResolveDirectoryLink(string linkPath, bool returnFinalTarget)
    {
        FileSystemInfo? target = Directory.ResolveLinkTarget(linkPath, returnFinalTarget);

        return target is null ? null : new LocalFileSystemEntry(target);
    }
}
