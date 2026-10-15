using System.Text;

namespace NovoDwarf.FS.Files.Interfaces;

/// <summary>Reads file contents synchronously or asynchronously.</summary>
public interface IFileReader
{
    /// <inheritdoc cref="File.ReadAllText(string)" />
    string ReadAllText(string path);

    /// <inheritdoc cref="File.ReadAllText(string, Encoding)" />
    string ReadAllText(string path, Encoding encoding);

    /// <inheritdoc cref="File.ReadAllLines(string)" />
    string[] ReadAllLines(string path);

    /// <inheritdoc cref="File.ReadAllLines(string, Encoding)" />
    string[] ReadAllLines(string path, Encoding encoding);

    /// <inheritdoc cref="File.ReadAllBytes(string)" />
    byte[] ReadAllBytes(string path);

    /// <inheritdoc cref="File.ReadLines(string)" />
    IEnumerable<string> ReadLines(string path);

    /// <inheritdoc cref="File.ReadLines(string, Encoding)" />
    IEnumerable<string> ReadLines(string path, Encoding encoding);

    /// <inheritdoc cref="File.ReadAllTextAsync(string, CancellationToken)" />
    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.ReadAllTextAsync(string, Encoding, CancellationToken)" />
    Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.ReadAllLinesAsync(string, CancellationToken)" />
    Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.ReadAllLinesAsync(string, Encoding, CancellationToken)" />
    Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.ReadAllBytesAsync(string, CancellationToken)" />
    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);
}
