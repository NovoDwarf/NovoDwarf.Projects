using System.Text;

namespace NovoDwarf.FS.Files.Interfaces;

public interface IFileWriter
{
	public void WriteAllText(string path, string? contents);

	public void WriteAllText(string path, string? contents, Encoding encoding);

	public void WriteAllBytes(string path, byte[] bytes);

	public void WriteAllBytes(string path, ReadOnlySpan<byte> bytes);

	public void WriteAllLines(string path, IEnumerable<string> contents);

	public void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding);

	public Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default);

	public Task WriteAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = default);

	public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);

	public Task WriteAllBytesAsync(string path, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default);

	public Task WriteAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default);

	public Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default);
}