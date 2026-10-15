using System.Text;

namespace NovoDwarf.FS.Files.Interfaces;

/// <summary>Writes file contents, replacing existing contents.</summary>
public interface IFileWriter
{
    /// <inheritdoc cref="File.WriteAllText(string, string)" />
    public void WriteAllText(string path, string? contents);

    /// <inheritdoc cref="File.WriteAllText(string, string, Encoding)" />
    public void WriteAllText(string path, string? contents, Encoding encoding);

    /// <inheritdoc cref="File.WriteAllBytes(string, byte[])" />
    public void WriteAllBytes(string path, byte[] bytes);

    /// <inheritdoc cref="File.WriteAllBytes(string, ReadOnlySpan{byte})" />
    public void WriteAllBytes(string path, ReadOnlySpan<byte> bytes);

    /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string})" />
    public void WriteAllLines(string path, IEnumerable<string> contents);

    /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string}, Encoding)" />
    public void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding);

    /// <inheritdoc cref="File.WriteAllTextAsync(string, string, CancellationToken)" />
    public Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.WriteAllTextAsync(string, string, Encoding, CancellationToken)" />
    public Task WriteAllTextAsync(
        string path,
        string? contents,
        Encoding encoding,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.WriteAllBytesAsync(string, byte[], CancellationToken)" />
    public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.WriteAllBytesAsync(string, ReadOnlyMemory{byte}, CancellationToken)" />
    public Task WriteAllBytesAsync(
        string path,
        ReadOnlyMemory<byte> bytes,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.WriteAllLinesAsync(string, IEnumerable{string}, CancellationToken)" />
    public Task WriteAllLinesAsync(
        string path,
        IEnumerable<string> contents,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.WriteAllLinesAsync(string, IEnumerable{string}, Encoding, CancellationToken)" />
    public Task WriteAllLinesAsync(
        string path,
        IEnumerable<string> contents,
        Encoding encoding,
        CancellationToken cancellationToken = default);
}
