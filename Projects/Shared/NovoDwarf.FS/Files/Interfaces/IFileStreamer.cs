namespace NovoDwarf.FS.Files.Interfaces;

/// <summary>Opens owned streams and text readers or writers. The caller must dispose them.</summary>
public interface IFileStreamer
{
    /// <inheritdoc cref="File.AppendText(string)" />
    public TextWriter AppendText(string path);

    /// <summary>Opens an owned stream with the specified file mode and access options.</summary>
    public FileSystemStream Open(string path, FileMode mode);

    /// <summary>Opens an owned stream with the specified file mode and access options.</summary>
    public FileSystemStream Open(string path, FileMode mode, FileAccess access);

    /// <summary>Opens an owned stream with the specified file mode and access options.</summary>
    public FileSystemStream Open(string path, FileMode mode, FileAccess access, FileShare share);

    /// <summary>Opens an owned stream with the specified file mode and access options.</summary>
    public FileSystemStream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize);

    /// <summary>Opens an owned stream with the specified file mode and access options.</summary>
    public FileSystemStream Open(
        string path,
        FileMode mode,
        FileAccess access,
        FileShare share,
        int bufferSize,
        FileOptions options);

    /// <inheritdoc cref="File.OpenRead(string)" />
    public FileSystemStream OpenRead(string path);

    /// <inheritdoc cref="File.OpenWrite(string)" />
    public FileSystemStream OpenWrite(string path);

    /// <inheritdoc cref="File.OpenText(string)" />
    public TextReader OpenText(string path);
}
