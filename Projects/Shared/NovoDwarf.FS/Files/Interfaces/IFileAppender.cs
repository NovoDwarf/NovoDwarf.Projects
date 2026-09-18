using System.Text;

namespace NovoDwarf.FS.Files.Interfaces;

public interface IFileAppender
{
	public void AppendAllText(string path, string? contents);

	public void AppendAllText(string path, string? contents, Encoding encoding);

	public void AppendAllLines(string path, IEnumerable<string> contents);

	public void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding);

	public void AppendAllBytes(string path, ReadOnlySpan<byte> bytes);

	public Task AppendAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default);

	public Task AppendAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = default);

	public Task AppendAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default);

	public Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default);

	public Task AppendAllBytesAsync(string path, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default);
}