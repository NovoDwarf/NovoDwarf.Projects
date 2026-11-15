using System.Text;

namespace NovoDwarf.FS.Files.Interfaces;

/// <summary>Appends data to files, creating them when necessary.</summary>
public interface IFileAppender
{
    /// <inheritdoc cref="File.AppendAllText(string, string)" />
    public void AppendAllText(string path, string? contents);

    /// <inheritdoc cref="File.AppendAllText(string, string, Encoding)" />
    public void AppendAllText(string path, string? contents, Encoding encoding);

    /// <inheritdoc cref="File.AppendAllLines(string, IEnumerable{string})" />
    public void AppendAllLines(string path, IEnumerable<string> contents);

    /// <inheritdoc cref="File.AppendAllLines(string, IEnumerable{string}, Encoding)" />
    public void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding);

    /// <summary>Appends bytes to a file, creating it when necessary.</summary>
    public void AppendAllBytes(string path, ReadOnlySpan<byte> bytes);

    /// <inheritdoc cref="File.AppendAllTextAsync(string, string, CancellationToken)" />
    public Task AppendAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.AppendAllTextAsync(string, string, Encoding, CancellationToken)" />
    public Task AppendAllTextAsync(
        string path,
        string? contents,
        Encoding encoding,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.AppendAllLinesAsync(string, IEnumerable{string}, CancellationToken)" />
    public Task AppendAllLinesAsync(
        string path,
        IEnumerable<string> contents,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.AppendAllLinesAsync(string, IEnumerable{string}, Encoding, CancellationToken)" />
    public Task AppendAllLinesAsync(
        string path,
        IEnumerable<string> contents,
        Encoding encoding,
        CancellationToken cancellationToken = default);

    /// <summary>Appends bytes to a file, creating it when necessary.</summary>
    public Task AppendAllBytesAsync(
        string path,
        ReadOnlyMemory<byte> bytes,
        CancellationToken cancellationToken = default);
}
