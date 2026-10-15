namespace NovoDwarf.FS.Links;

/// <summary>Creates and resolves symbolic links without exposing mutable backend objects.</summary>
public interface ISymbolicLinkService
{
    /// <summary>Creates a symbolic link to a file and returns the link's metadata handle.</summary>
    IFileSystemEntry CreateFileLink(string path, string pathToTarget);

    /// <summary>Creates a symbolic link to a directory and returns the link's metadata handle.</summary>
    IFileSystemEntry CreateDirectoryLink(string path, string pathToTarget);

    /// <summary>Resolves a file link, returning null for a non-link.</summary>
    /// <param name="linkPath">The path of the link.</param>
    /// <param name="returnFinalTarget">Whether to follow the full link chain.</param>
    IFileSystemEntry? ResolveFileLink(string linkPath, bool returnFinalTarget);

    /// <summary>Resolves a directory link, returning null for a non-link.</summary>
    /// <param name="linkPath">The path of the link.</param>
    /// <param name="returnFinalTarget">Whether to follow the full link chain.</param>
    IFileSystemEntry? ResolveDirectoryLink(string linkPath, bool returnFinalTarget);
}
