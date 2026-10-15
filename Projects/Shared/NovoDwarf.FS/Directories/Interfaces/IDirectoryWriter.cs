namespace NovoDwarf.FS.Directories.Interfaces;

/// <summary>Creates, moves, and deletes directories and changes the process working directory.</summary>
public interface IDirectoryWriter
{
    /// <inheritdoc cref="Directory.CreateDirectory(string)" />
    public IFileSystemEntry CreateDirectory(string path);

    /// <summary>Creates a subdirectory and returns its read-only metadata handle.</summary>
    public IFileSystemEntry CreateSubdirectory(string path, string subdirectoryName);

    /// <inheritdoc cref="Directory.Delete(string, bool)" />
    public void Delete(string path, bool recursive = false);

    /// <inheritdoc cref="Directory.Move(string, string)" />
    public void Move(string sourceDirName, string destDirName);

    /// <inheritdoc cref="Directory.SetCurrentDirectory(string)" />
    public void SetCurrentDirectory(string path);

    /// <inheritdoc cref="Directory.CreateTempSubdirectory(string)" />
    public IFileSystemEntry CreateTempSubdirectory(string? prefix = null);
}
