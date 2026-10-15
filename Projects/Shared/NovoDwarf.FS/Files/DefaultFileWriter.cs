using System.Text;
using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc cref="IFileWriter" />
public class DefaultFileWriter : IFileWriter
{
    /// <inheritdoc />
    public void WriteAllText(string path, string? contents) => File.WriteAllText(path, contents);

    /// <inheritdoc />
    public void WriteAllText(string path, string? contents, Encoding encoding)
        => File.WriteAllText(path, contents, encoding);

    /// <inheritdoc />
    public void WriteAllBytes(string path, byte[] bytes) => File.WriteAllBytes(path, bytes);

    /// <inheritdoc />
    public void WriteAllBytes(string path, ReadOnlySpan<byte> bytes) => File.WriteAllBytes(path, bytes);

    /// <inheritdoc />
    public void WriteAllLines(string path, IEnumerable<string> contents)
        => File.WriteAllLines(path, contents);

    /// <inheritdoc />
    public void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        => File.WriteAllLines(path, contents, encoding);

    /// <inheritdoc />
    public Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default)
        => File.WriteAllTextAsync(path, contents, cancellationToken);

    /// <inheritdoc />
    public Task WriteAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = default)
        => File.WriteAllTextAsync(path, contents, encoding, cancellationToken);

    /// <inheritdoc />
    public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
        => File.WriteAllBytesAsync(path, bytes, cancellationToken);

    /// <inheritdoc />
    public Task WriteAllBytesAsync(string path, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
        => File.WriteAllBytesAsync(path, bytes, cancellationToken);

    /// <inheritdoc />
    public Task WriteAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default)
        => File.WriteAllLinesAsync(path, contents, cancellationToken);

    /// <inheritdoc />
    public Task WriteAllLinesAsync(
        string path,
        IEnumerable<string> contents,
        Encoding encoding,
        CancellationToken cancellationToken = default)
        => File.WriteAllLinesAsync(path, contents, encoding, cancellationToken);
}
