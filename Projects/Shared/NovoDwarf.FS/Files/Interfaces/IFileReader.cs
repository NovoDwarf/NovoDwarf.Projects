using System.Text;

namespace NovoDwarf.FS.Files.Interfaces;

public interface IFileReader
{
	string ReadAllText(string path);

	string ReadAllText(string path, Encoding encoding);

	string[] ReadAllLines(string path);

	string[] ReadAllLines(string path, Encoding encoding);

	byte[] ReadAllBytes(string path);

	IEnumerable<string> ReadLines(string path);

	IEnumerable<string> ReadLines(string path, Encoding encoding);

	Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);

	Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);

	Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default);

	Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);

	Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);
}