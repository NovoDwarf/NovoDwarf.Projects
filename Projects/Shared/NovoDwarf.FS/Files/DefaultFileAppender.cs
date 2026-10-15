using System.Text;
using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc cref="IFileAppender" />
public sealed class DefaultFileAppender : IFileAppender
{
    /// <inheritdoc />
    public void AppendAllText(string path, string? contents) => File.AppendAllText(path, contents);

    /// <inheritdoc />
    public void AppendAllText(string path, string? contents, Encoding encoding) => File.AppendAllText(path, contents, encoding);

    /// <inheritdoc />
    public void AppendAllLines(string path, IEnumerable<string> contents) => File.AppendAllLines(path, contents);

    /// <inheritdoc />
    public void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding) => File.AppendAllLines(path, contents, encoding);

    /// <inheritdoc />
    public void AppendAllBytes(string path, ReadOnlySpan<byte> bytes)
    {
        using FileStream stream = new(path, FileMode.Append, FileAccess.Write, FileShare.Read);
        stream.Write(bytes);
    }

    /// <inheritdoc />
    public Task AppendAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default)
        => File.AppendAllTextAsync(path, contents, cancellationToken);

    /// <inheritdoc />
    public Task AppendAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = default)
        => File.AppendAllTextAsync(path, contents, encoding, cancellationToken);

    /// <inheritdoc />
    public Task AppendAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default)
        => File.AppendAllLinesAsync(path, contents, cancellationToken);

    /// <inheritdoc />
    public Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default)
        => File.AppendAllLinesAsync(path, contents, encoding, cancellationToken);

    /// <inheritdoc />
    public async Task AppendAllBytesAsync(string path, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
    {
        await using FileStream stream = new(path, FileMode.Append, FileAccess.Write, FileShare.Read, bufferSize: 4096, useAsync: true);
        await stream.WriteAsync(bytes, cancellationToken);
    }
}
