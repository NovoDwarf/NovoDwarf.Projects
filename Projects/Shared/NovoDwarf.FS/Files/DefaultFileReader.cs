using System.Text;
using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc cref="IFileReader" />
public sealed class DefaultFileReader : IFileReader
{
    /// <inheritdoc />
    public string ReadAllText(string path) => File.ReadAllText(path);

    /// <inheritdoc />
    public string ReadAllText(string path, Encoding encoding) => File.ReadAllText(path, encoding);

    /// <inheritdoc />
    public string[] ReadAllLines(string path) => File.ReadAllLines(path);

    /// <inheritdoc />
    public string[] ReadAllLines(string path, Encoding encoding) => File.ReadAllLines(path, encoding);

    /// <inheritdoc />
    public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);

    /// <inheritdoc />
    public IEnumerable<string> ReadLines(string path) => File.ReadLines(path);

    /// <inheritdoc />
    public IEnumerable<string> ReadLines(string path, Encoding encoding) => File.ReadLines(path, encoding);

    /// <inheritdoc />
    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
        => File.ReadAllTextAsync(path, cancellationToken);

    /// <inheritdoc />
    public Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
        => File.ReadAllTextAsync(path, encoding, cancellationToken);

    /// <inheritdoc />
    public Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default)
        => File.ReadAllLinesAsync(path, cancellationToken);

    /// <inheritdoc />
    public Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
        => File.ReadAllLinesAsync(path, encoding, cancellationToken);

    /// <inheritdoc />
    public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        => File.ReadAllBytesAsync(path, cancellationToken);
}
