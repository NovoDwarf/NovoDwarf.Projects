using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc cref="IFileStreamer" />
public class DefaultFileStreamer : IFileStreamer
{
    /// <inheritdoc />
    public TextWriter AppendText(string path) => File.AppendText(path);

    /// <inheritdoc />
    public FileSystemStream Open(string path, FileMode mode) => new(File.Open(path, mode));

    /// <inheritdoc />
    public FileSystemStream Open(string path, FileMode mode, FileAccess access)
        => new(File.Open(path, mode, access));

    /// <inheritdoc />
    public FileSystemStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        => new(File.Open(path, mode, access, share));

    /// <inheritdoc />
    public FileSystemStream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
        => new(new FileStream(path, mode, access, share, bufferSize));

    /// <inheritdoc />
    public FileSystemStream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
        => new(new FileStream(path, mode, access, share, bufferSize, options));

    /// <inheritdoc />
    public FileSystemStream OpenRead(string path) => new(File.OpenRead(path));

    /// <inheritdoc />
    public FileSystemStream OpenWrite(string path) => new(File.OpenWrite(path));

    /// <inheritdoc />
    public TextReader OpenText(string path) => File.OpenText(path);
}
